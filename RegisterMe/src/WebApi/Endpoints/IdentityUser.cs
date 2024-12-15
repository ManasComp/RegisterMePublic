// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#region

using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using RegisterMe.Application.Common.Interfaces;
using RegisterMe.Application.Interfaces;
using RegisterMe.Domain.Entities;
using WebApi.Endpoints.Request;
using WebApi.Endpoints.Responses;

#endregion


namespace WebApi.Endpoints;

/// <summary>
///     Provides extension methods for <see cref="IEndpointRouteBuilder" /> to add identity endpoints.
/// </summary>
public static class IdentityApiEndpointRouteBuilderExtensions
{
    // Validate the email address using DataAnnotations like the UserValidator does when RequireUniqueEmail = true.
    private static readonly EmailAddressAttribute EmailAddressAttribute = new();

    /// <summary>
    ///     Add endpoints for registering, logging in, and logging out using ASP.NET Core Identity.
    /// </summary>
    /// <param name="endpoints">
    ///     The <see cref="IEndpointRouteBuilder" /> to add the identity endpoints to.
    ///     Call <see cref="EndpointRouteBuilderExtensions.MapGroup(IEndpointRouteBuilder, string)" /> to add a prefix to all
    ///     the endpoints.
    /// </param>
    /// <returns>An <see cref="IEndpointConventionBuilder" /> to further customize the added endpoints.</returns>
    public static IEndpointConventionBuilder MapIdentityUserApi(this IEndpointRouteBuilder endpoints)
    {
        ArgumentNullException.ThrowIfNull(endpoints);

        TimeProvider timeProvider = endpoints.ServiceProvider.GetRequiredService<TimeProvider>();
        IOptionsMonitor<BearerTokenOptions> bearerTokenOptions =
            endpoints.ServiceProvider.GetRequiredService<IOptionsMonitor<BearerTokenOptions>>();
        LinkGenerator linkGenerator = endpoints.ServiceProvider.GetRequiredService<LinkGenerator>();

        // We'll figure out a unique endpoint name based on the final route pattern during endpoint generation.
        string? confirmEmailEndpointName = null;

        RouteGroupBuilder routeGroup = endpoints.MapGroup("");

        routeGroup.MapPost("/register", async Task<Results<Ok, ValidationProblem>>
        ([FromBody] RegisterReq registration, HttpContext context,
            [FromServices] UserManager<ApplicationUser> userManager,
            [FromServices] IUserStore<ApplicationUser> userStore,
            [FromServices] IEmailSender emailSender) =>
        {
            IUserEmailStore<ApplicationUser> emailStore = (IUserEmailStore<ApplicationUser>)userStore;
            if (!userManager.SupportsUserEmail)
            {
                throw new NotSupportedException(
                    $"{nameof(MapIdentityUserApi)} requires a user store with email support.");
            }

            string email = registration.Email;

            if (string.IsNullOrEmpty(email) || !EmailAddressAttribute.IsValid(email))
            {
                return CreateValidationProblem(IdentityResult.Failed(userManager.ErrorDescriber.InvalidEmail(email)));
            }

            ApplicationUser user = new();
            await userStore.SetUserNameAsync(user, email, CancellationToken.None);
            await emailStore.SetEmailAsync(user, email, CancellationToken.None);
            user.FirstName = registration.FirstName;
            user.LastName = registration.LastName;
            user.DateOfBirth = registration.DateOfBirth;
            user.PhoneNumber = registration.PhoneNumber;
            IdentityResult result = await userManager.CreateAsync(user, registration.Password);

            if (!result.Succeeded)
            {
                return CreateValidationProblem(result);
            }

            await SendConfirmationEmailAsync(user, emailSender, userManager, context, email);
            return TypedResults.Ok();
        });

        routeGroup.MapPost("/login", async Task<Results<Ok<AccessTokenResponse>, EmptyHttpResult, ProblemHttpResult>>
        ([FromBody] LoginReq login, [FromQuery] bool? useCookies, [FromQuery] bool? useSessionCookies,
            [FromServices] SignInManager<ApplicationUser> signInManager) =>
        {
            bool useCookieScheme = useCookies == true || useSessionCookies == true;
            bool isPersistent = useCookies == true && useSessionCookies != true;
            signInManager.AuthenticationScheme =
                useCookieScheme ? IdentityConstants.ApplicationScheme : IdentityConstants.BearerScheme;

            SignInResult result =
                await signInManager.PasswordSignInAsync(login.Email, login.Password, isPersistent, true);

            if (result.RequiresTwoFactor)
            {
                if (!string.IsNullOrEmpty(login.TwoFactorCode))
                {
                    result = await signInManager.TwoFactorAuthenticatorSignInAsync(login.TwoFactorCode, isPersistent,
                        isPersistent);
                }
                else if (!string.IsNullOrEmpty(login.TwoFactorRecoveryCode))
                {
                    result = await signInManager.TwoFactorRecoveryCodeSignInAsync(login.TwoFactorRecoveryCode);
                }
            }

            if (!result.Succeeded)
            {
                return TypedResults.Problem(result.ToString(), statusCode: StatusCodes.Status401Unauthorized);
            }

            // The signInManager already produced the needed response in the form of a cookie or bearer token.
            return TypedResults.Empty;
        });

        routeGroup.MapPost("/refresh",
            async Task<Results<Ok<AccessTokenResponse>, UnauthorizedHttpResult, SignInHttpResult, ChallengeHttpResult>>
            ([FromBody] RefreshReq refreshRequest,
                [FromServices] SignInManager<ApplicationUser> signInManager) =>
            {
                ISecureDataFormat<AuthenticationTicket> refreshTokenProtector =
                    bearerTokenOptions.Get(IdentityConstants.BearerScheme).RefreshTokenProtector;
                AuthenticationTicket? refreshTicket = refreshTokenProtector.Unprotect(refreshRequest.RefreshToken);

                // Reject the /refresh attempt with a 401 if the token expired or the security stamp validation fails
                if (refreshTicket?.Properties.ExpiresUtc is not { } expiresUtc ||
                    timeProvider.GetUtcNow() >= expiresUtc ||
                    await signInManager.ValidateSecurityStampAsync(refreshTicket.Principal) is not { } user)

                {
                    return TypedResults.Challenge();
                }

                ClaimsPrincipal newPrincipal = await signInManager.CreateUserPrincipalAsync(user);
                return TypedResults.SignIn(newPrincipal, authenticationScheme: IdentityConstants.BearerScheme);
            });

        routeGroup.MapGet("/confirmEmail", async Task<Results<ContentHttpResult, UnauthorizedHttpResult>>
            ([FromQuery] string userId, [FromQuery] string code, [FromQuery] string? changedEmail,
                [FromServices] UserManager<ApplicationUser> userManager) =>
            {
                if (await userManager.FindByIdAsync(userId) is not { } user)
                {
                    // We could respond with a 404 instead of a 401 like Identity UI, but that feels like unnecessary information.
                    return TypedResults.Unauthorized();
                }

                try
                {
                    code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
                }
                catch (FormatException)
                {
                    return TypedResults.Unauthorized();
                }

                IdentityResult result;

                if (string.IsNullOrEmpty(changedEmail))
                {
                    result = await userManager.ConfirmEmailAsync(user, code);
                }
                else
                {
                    // As with Identity UI, email and user name are one and the same. So when we update the email,
                    // we need to update the user name.
                    result = await userManager.ChangeEmailAsync(user, changedEmail, code);

                    if (result.Succeeded)
                    {
                        result = await userManager.SetUserNameAsync(user, changedEmail);
                    }
                }

                if (!result.Succeeded)
                {
                    return TypedResults.Unauthorized();
                }

                return TypedResults.Text("Thank you for confirming your email.");
            })
            .Add(endpointBuilder =>
            {
                string? finalPattern = ((RouteEndpointBuilder)endpointBuilder).RoutePattern.RawText;
                confirmEmailEndpointName = $"{nameof(MapIdentityUserApi)}-{finalPattern}";
                endpointBuilder.Metadata.Add(new EndpointNameMetadata(confirmEmailEndpointName));
            });

        routeGroup.MapPost("/resendConfirmationEmail", async Task<Ok>
        ([FromBody] ResendConfirmationEmailReq resendRequest, HttpContext context,
            [FromServices] UserManager<ApplicationUser> userManager, [FromServices] IEmailSender emailSender) =>
        {
            if (await userManager.FindByEmailAsync(resendRequest.Email) is not { } user)
            {
                return TypedResults.Ok();
            }

            await SendConfirmationEmailAsync(user, emailSender, userManager, context, resendRequest.Email);
            return TypedResults.Ok();
        });

        routeGroup.MapPost("/forgotPassword", async Task<Results<Ok, ValidationProblem>>
        ([FromBody] ForgotPasswordReq resetRequest, [FromServices] UserManager<ApplicationUser> userManager,
            [FromServices] IEmailSender emailSender) =>
        {
            ApplicationUser? user = await userManager.FindByEmailAsync(resetRequest.Email);

            if (user is not null && await userManager.IsEmailConfirmedAsync(user))
            {
                string code = await userManager.GeneratePasswordResetTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                await emailSender.SendEmailAsync(resetRequest.Email, "Reset Password",
                    $"Please reset your password by with this code '{HtmlEncoder.Default.Encode(code)}'.");
            }

            // Don't reveal that the user does not exist or is not confirmed, so don't return a 200 if we would have
            // returned a 400 for an invalid code given a valid user email.
            return TypedResults.Ok();
        });

        routeGroup.MapPost("/resetPassword", async Task<Results<Ok, ValidationProblem>>
            ([FromBody] ResetPasswordReq resetRequest, [FromServices] UserManager<ApplicationUser> userManager) =>
        {
            ApplicationUser? user = await userManager.FindByEmailAsync(resetRequest.Email);

            if (user is null || !await userManager.IsEmailConfirmedAsync(user))
            {
                // Don't reveal that the user does not exist or is not confirmed, so don't return a 200 if we would have
                // returned a 400 for an invalid code given a valid user email.
                return CreateValidationProblem(IdentityResult.Failed(userManager.ErrorDescriber.InvalidToken()));
            }

            IdentityResult result;
            try
            {
                string code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(resetRequest.ResetCode));
                result = await userManager.ResetPasswordAsync(user, code, resetRequest.NewPassword);
            }
            catch (FormatException)
            {
                result = IdentityResult.Failed(userManager.ErrorDescriber.InvalidToken());
            }

            if (!result.Succeeded)
            {
                return CreateValidationProblem(result);
            }

            return TypedResults.Ok();
        });

        RouteGroupBuilder accountGroup = routeGroup.MapGroup("/manage").RequireAuthorization();

        accountGroup.MapPost("/2fa", async Task<Results<Ok<TwoFactorResponse>, ValidationProblem, NotFound>>
        ([FromServices] IUser? claimsPrincipal, [FromBody] TwoFactorReq tfaRequest,
            [FromServices] SignInManager<ApplicationUser> signInManager) =>
        {
            UserManager<ApplicationUser> userManager = signInManager.UserManager;
            if (claimsPrincipal?.User is null)
            {
                return TypedResults.NotFound();
            }

            if (await userManager.GetUserAsync(claimsPrincipal.User) is not { } user)
            {
                return TypedResults.NotFound();
            }

            if (tfaRequest.Enable == true)
            {
                if (tfaRequest.ResetSharedKey)
                {
                    return CreateValidationProblem("CannotResetSharedKeyAndEnable",
                        "Resetting the 2fa shared key must disable 2fa until a 2fa token based on the new shared key is validated.");
                }

                if (string.IsNullOrEmpty(tfaRequest.TwoFactorCode))
                {
                    return CreateValidationProblem("RequiresTwoFactor",
                        "No 2fa token was provided by the request. A valid 2fa token is required to enable 2fa.");
                }

                if (!await userManager.VerifyTwoFactorTokenAsync(user,
                        userManager.Options.Tokens.AuthenticatorTokenProvider, tfaRequest.TwoFactorCode))
                {
                    return CreateValidationProblem("InvalidTwoFactorCode",
                        "The 2fa token provided by the request was invalid. A valid 2fa token is required to enable 2fa.");
                }

                await userManager.SetTwoFactorEnabledAsync(user, true);
            }
            else if (tfaRequest.Enable == false || tfaRequest.ResetSharedKey)
            {
                await userManager.SetTwoFactorEnabledAsync(user, false);
            }

            if (tfaRequest.ResetSharedKey)
            {
                await userManager.ResetAuthenticatorKeyAsync(user);
            }

            string[]? recoveryCodes = null;
            if (tfaRequest.ResetRecoveryCodes ||
                (tfaRequest.Enable == true && await userManager.CountRecoveryCodesAsync(user) == 0))
            {
                IEnumerable<string>? recoveryCodesEnumerable =
                    await userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
                recoveryCodes = recoveryCodesEnumerable?.ToArray();
            }

            if (tfaRequest.ForgetMachine)
            {
                await signInManager.ForgetTwoFactorClientAsync();
            }

            string? key = await userManager.GetAuthenticatorKeyAsync(user);
            if (string.IsNullOrEmpty(key))
            {
                await userManager.ResetAuthenticatorKeyAsync(user);
                key = await userManager.GetAuthenticatorKeyAsync(user);

                if (string.IsNullOrEmpty(key))
                {
                    throw new NotSupportedException("The user manager must produce an authenticator key after reset.");
                }
            }

            return TypedResults.Ok(new TwoFactorResponse
            {
                SharedKey = key,
                RecoveryCodes = recoveryCodes,
                RecoveryCodesLeft = recoveryCodes?.Length ?? await userManager.CountRecoveryCodesAsync(user),
                IsTwoFactorEnabled = await userManager.GetTwoFactorEnabledAsync(user),
                IsMachineRemembered = await signInManager.IsTwoFactorClientRememberedAsync(user)
            });
        });

        accountGroup.MapGet("/info", async Task<Results<Ok<InfoResponse>, ValidationProblem, NotFound>>
            ([FromServices] IUser? claimsPrincipal, [FromServices] UserManager<ApplicationUser> userManager) =>
        {
            if (claimsPrincipal?.User is null)
            {
                return TypedResults.NotFound();
            }

            if (await userManager.GetUserAsync(claimsPrincipal.User) is not { } user)
            {
                return TypedResults.NotFound();
            }

            return TypedResults.Ok(await CreateInfoResponseAsync(user, userManager));
        });

        accountGroup.MapPost("/info", async Task<Results<Ok<InfoResponse>, ValidationProblem, NotFound>>
        ([FromServices] IUser? claimsPrincipal, [FromBody] InfoReq infoRequest, HttpContext context,
            [FromServices] UserManager<ApplicationUser> userManager, [FromServices] IEmailSender emailSender) =>
        {
            if (claimsPrincipal?.User is null)
            {
                return TypedResults.NotFound();
            }

            if (await userManager.GetUserAsync(claimsPrincipal.User) is not { } user)
            {
                return TypedResults.NotFound();
            }

            if (!string.IsNullOrEmpty(infoRequest.NewEmail) && !EmailAddressAttribute.IsValid(infoRequest.NewEmail))
            {
                return CreateValidationProblem(
                    IdentityResult.Failed(userManager.ErrorDescriber.InvalidEmail(infoRequest.NewEmail)));
            }

            if (!string.IsNullOrEmpty(infoRequest.NewPassword))
            {
                if (string.IsNullOrEmpty(infoRequest.OldPassword))
                {
                    return CreateValidationProblem("OldPasswordRequired",
                        "The old password is required to set a new password. If the old password is forgotten, use /resetPassword.");
                }

                IdentityResult changePasswordResult =
                    await userManager.ChangePasswordAsync(user, infoRequest.OldPassword, infoRequest.NewPassword);
                if (!changePasswordResult.Succeeded)
                {
                    return CreateValidationProblem(changePasswordResult);
                }
            }

            if (!string.IsNullOrEmpty(infoRequest.PhoneNumber))
            {
                await userManager.SetPhoneNumberAsync(user, infoRequest.PhoneNumber);
            }

            if (!string.IsNullOrEmpty(infoRequest.NewEmail))
            {
                string? email = await userManager.GetEmailAsync(user);

                if (email != infoRequest.NewEmail)
                {
                    await SendConfirmationEmailAsync(user, emailSender, userManager, context, infoRequest.NewEmail,
                        true);
                }
            }

            return TypedResults.Ok(await CreateInfoResponseAsync(user, userManager));
        });

        async Task SendConfirmationEmailAsync(ApplicationUser user, IEmailSender emailSender,
            UserManager<ApplicationUser> userManager,
            HttpContext context,
            string email, bool isChange = false)
        {
            if (confirmEmailEndpointName is null)
            {
                throw new NotSupportedException("No email confirmation endpoint was registered!");
            }

            string code = isChange
                ? await userManager.GenerateChangeEmailTokenAsync(user, email)
                : await userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            string userId = await userManager.GetUserIdAsync(user);
            RouteValueDictionary routeValues = new() { ["userId"] = userId, ["code"] = code };

            if (isChange)
            {
                // This is validated by the /confirmEmail endpoint on change.
                routeValues.Add("changedEmail", email);
            }

            string confirmEmailUrl = linkGenerator.GetUriByName(context, confirmEmailEndpointName, routeValues)
                                     ?? throw new NotSupportedException(
                                         $"Could not find endpoint named '{confirmEmailEndpointName}'.");

            await emailSender.SendEmailAsync(email, "Confirm Email",
                $"Please confirm your email by clicking <a href='{HtmlEncoder.Default.Encode(confirmEmailUrl)}'>here</a>.");
        }

        return new IdentityEndpointsConventionBuilder(routeGroup);
    }

    private static ValidationProblem CreateValidationProblem(string errorCode, string errorDescription)
    {
        return TypedResults.ValidationProblem(new Dictionary<string, string[]> { { errorCode, [errorDescription] } });
    }

    private static ValidationProblem CreateValidationProblem(IdentityResult result)
    {
        // We expect a single error code and description in the normal case.
        // This could be golfed with GroupBy and ToDictionary, but perf! :P
        Debug.Assert(!result.Succeeded);
        Dictionary<string, string[]> errorDictionary = new(1);

        foreach (IdentityError error in result.Errors)
        {
            string[] newDescriptions;

            if (errorDictionary.TryGetValue(error.Code, out string[]? descriptions))
            {
                newDescriptions = new string[descriptions.Length + 1];
                Array.Copy(descriptions, newDescriptions, descriptions.Length);
                newDescriptions[descriptions.Length] = error.Description;
            }
            else
            {
                newDescriptions = [error.Description];
            }

            errorDictionary[error.Code] = newDescriptions;
        }

        return TypedResults.ValidationProblem(errorDictionary);
    }

    private static async Task<InfoResponse> CreateInfoResponseAsync(ApplicationUser user,
        UserManager<ApplicationUser> userManager)
    {
        return new InfoResponse
        {
            Email = await userManager.GetEmailAsync(user) ??
                    throw new NotSupportedException("Users must have an email."),
            IsEmailConfirmed = await userManager.IsEmailConfirmedAsync(user),
            PhoneNumber =
                await userManager.GetPhoneNumberAsync(user) ??
                throw new NotSupportedException("Users must have a phone number."),
            Id = await userManager.GetUserIdAsync(user) ?? throw new NotSupportedException("Users must have an ID.")
        };
    }

    // Wrap RouteGroupBuilder with a non-public type to avoid a potential future behavioral breaking change.
    private sealed class IdentityEndpointsConventionBuilder(RouteGroupBuilder inner) : IEndpointConventionBuilder
    {
        private IEndpointConventionBuilder InnerAsConventionBuilder => inner;

        public void Add(Action<EndpointBuilder> convention)
        {
            InnerAsConventionBuilder.Add(convention);
        }

        public void Finally(Action<EndpointBuilder> finallyConvention)
        {
            InnerAsConventionBuilder.Finally(finallyConvention);
        }
    }

    [AttributeUsage(AttributeTargets.Parameter)]
    private sealed class FromBodyAttribute : Attribute, IFromBodyMetadata
    {
    }

    [AttributeUsage(AttributeTargets.Parameter)]
    private sealed class FromServicesAttribute : Attribute, IFromServiceMetadata
    {
    }

    [AttributeUsage(AttributeTargets.Parameter)]
    private sealed class FromQueryAttribute : Attribute, IFromQueryMetadata
    {
        public string? Name => null;
    }
}
