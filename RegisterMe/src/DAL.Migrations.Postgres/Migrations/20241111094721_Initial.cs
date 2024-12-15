using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DAL.Migrations.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DataProtectionKeys",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FriendlyName = table.Column<string>(type: "text", nullable: true),
                    Xml = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataProtectionKeys", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Groups",
                columns: table => new
                {
                    GroupId = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => x.GroupId);
                });

            migrationBuilder.CreateTable(
                name: "Organizations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Ico = table.Column<string>(type: "text", nullable: false),
                    TelNumber = table.Column<string>(type: "text", nullable: false),
                    Website = table.Column<string>(type: "text", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: false),
                    IsConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organizations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Prices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RentedCages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Length = table.Column<int>(type: "integer", nullable: false),
                    Width = table.Column<int>(type: "integer", nullable: false),
                    Height = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RentedCages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    DateOfBirth = table.Column<DateOnly>(type: "date", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false),
                    OrganizationId = table.Column<int>(type: "integer", nullable: true),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: false),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Exhibitions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Phone = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    IsPublished = table.Column<bool>(type: "boolean", nullable: false),
                    Url = table.Column<string>(type: "text", nullable: false),
                    BankAccount = table.Column<string>(type: "text", nullable: false),
                    Iban = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    RegistrationStart = table.Column<DateOnly>(type: "date", nullable: false),
                    RegistrationEnd = table.Column<DateOnly>(type: "date", nullable: false),
                    OrganizationId = table.Column<int>(type: "integer", nullable: false),
                    IsCancelled = table.Column<bool>(type: "boolean", nullable: false),
                    DeleteNotFinishedRegistrationsAfterHours = table.Column<int>(type: "integer", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exhibitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Exhibitions_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PriceToGroupsJoinTable",
                columns: table => new
                {
                    GroupsGroupId = table.Column<string>(type: "text", nullable: false),
                    PricesId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PriceToGroupsJoinTable", x => new { x.GroupsGroupId, x.PricesId });
                    table.ForeignKey(
                        name: "FK_PriceToGroupsJoinTable_Groups_GroupsGroupId",
                        column: x => x.GroupsGroupId,
                        principalTable: "Groups",
                        principalColumn: "GroupId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PriceToGroupsJoinTable_Prices_PricesId",
                        column: x => x.PricesId,
                        principalTable: "Prices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RentedTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CageId = table.Column<int>(type: "integer", nullable: false),
                    RentedType = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RentedTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RentedTypes_RentedCages_CageId",
                        column: x => x.CageId,
                        principalTable: "RentedCages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    ProviderKey = table.Column<string>(type: "text", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    RoleId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Exhibitors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Country = table.Column<string>(type: "text", nullable: false),
                    City = table.Column<string>(type: "text", nullable: false),
                    Street = table.Column<string>(type: "text", nullable: false),
                    HouseNumber = table.Column<string>(type: "text", nullable: false),
                    ZipCode = table.Column<string>(type: "text", nullable: false),
                    Organization = table.Column<string>(type: "text", nullable: false),
                    MemberNumber = table.Column<string>(type: "text", nullable: false),
                    AspNetUserId = table.Column<string>(type: "text", nullable: false),
                    IsPartOfCsch = table.Column<bool>(type: "boolean", nullable: false),
                    IsPartOfFife = table.Column<bool>(type: "boolean", nullable: false),
                    EmailToOrganization = table.Column<string>(type: "text", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exhibitors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Exhibitors_AspNetUsers_AspNetUserId",
                        column: x => x.AspNetUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Addresses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StreetAddress = table.Column<string>(type: "text", nullable: false),
                    Latitude = table.Column<string>(type: "text", nullable: false),
                    Longitude = table.Column<string>(type: "text", nullable: false),
                    ExhibitionId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Addresses_Exhibitions_ExhibitionId",
                        column: x => x.ExhibitionId,
                        principalTable: "Exhibitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Advertisements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Description = table.Column<string>(type: "text", nullable: false),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false),
                    ExhibitionId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Advertisements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Advertisements_Exhibitions_ExhibitionId",
                        column: x => x.ExhibitionId,
                        principalTable: "Exhibitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExhibitionDays",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    ExhibitionId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExhibitionDays", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExhibitionDays_Exhibitions_ExhibitionId",
                        column: x => x.ExhibitionId,
                        principalTable: "Exhibitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PriceAdjustmentWorkflows",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ExhibitionId = table.Column<int>(type: "integer", nullable: false),
                    WorkflowName = table.Column<string>(type: "text", nullable: false),
                    RuleExpressionType = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PriceAdjustmentWorkflows", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PriceAdjustmentWorkflows_Exhibitions_ExhibitionId",
                        column: x => x.ExhibitionId,
                        principalTable: "Exhibitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PriceTypeWorkflows",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ExhibitionId = table.Column<int>(type: "integer", nullable: false),
                    WorkflowName = table.Column<string>(type: "text", nullable: false),
                    RuleExpressionType = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PriceTypeWorkflows", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PriceTypeWorkflows_Exhibitions_ExhibitionId",
                        column: x => x.ExhibitionId,
                        principalTable: "Exhibitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RegistrationsToExhibition",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ExhibitionId = table.Column<int>(type: "integer", nullable: false),
                    ExhibitorId = table.Column<int>(type: "integer", nullable: false),
                    AdvertisementId = table.Column<int>(type: "integer", nullable: false),
                    LastNotificationSendOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrationsToExhibition", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegistrationsToExhibition_Advertisements_AdvertisementId",
                        column: x => x.AdvertisementId,
                        principalTable: "Advertisements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RegistrationsToExhibition_Exhibitions_ExhibitionId",
                        column: x => x.ExhibitionId,
                        principalTable: "Exhibitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RegistrationsToExhibition_Exhibitors_ExhibitorId",
                        column: x => x.ExhibitorId,
                        principalTable: "Exhibitors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ExhibitionDayPriceJoinTable",
                columns: table => new
                {
                    ExhibitionDayId = table.Column<int>(type: "integer", nullable: false),
                    PriceId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExhibitionDayPriceJoinTable", x => new { x.ExhibitionDayId, x.PriceId });
                    table.ForeignKey(
                        name: "FK_ExhibitionDayPriceJoinTable_ExhibitionDays_ExhibitionDayId",
                        column: x => x.ExhibitionDayId,
                        principalTable: "ExhibitionDays",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExhibitionDayPriceJoinTable_Prices_PriceId",
                        column: x => x.PriceId,
                        principalTable: "Prices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RentedCageAndExhibitionDayJoinTable",
                columns: table => new
                {
                    CagesForRentId = table.Column<int>(type: "integer", nullable: false),
                    ExhibitionDaysId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RentedCageAndExhibitionDayJoinTable", x => new { x.CagesForRentId, x.ExhibitionDaysId });
                    table.ForeignKey(
                        name: "FK_RentedCageAndExhibitionDayJoinTable_ExhibitionDays_Exhibiti~",
                        column: x => x.ExhibitionDaysId,
                        principalTable: "ExhibitionDays",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RentedCageAndExhibitionDayJoinTable_RentedCages_CagesForRen~",
                        column: x => x.CagesForRentId,
                        principalTable: "RentedCages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RulesEngineRules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RuleName = table.Column<string>(type: "text", nullable: true),
                    Properties = table.Column<string>(type: "text", nullable: true),
                    Operator = table.Column<string>(type: "text", nullable: true),
                    ErrorMessage = table.Column<string>(type: "text", nullable: true),
                    Enabled = table.Column<bool>(type: "boolean", nullable: false),
                    RuleExpressionType = table.Column<int>(type: "integer", nullable: false),
                    Expression = table.Column<string>(type: "text", nullable: true),
                    Actions = table.Column<string>(type: "text", nullable: true),
                    SuccessEvent = table.Column<string>(type: "text", nullable: true),
                    PriceAdjustmentWorkflowId = table.Column<int>(type: "integer", nullable: true),
                    PriceTypeWorkflowId = table.Column<int>(type: "integer", nullable: true),
                    RuleIDFK = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RulesEngineRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RulesEngineRules_PriceAdjustmentWorkflows_PriceAdjustmentWo~",
                        column: x => x.PriceAdjustmentWorkflowId,
                        principalTable: "PriceAdjustmentWorkflows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RulesEngineRules_PriceTypeWorkflows_PriceTypeWorkflowId",
                        column: x => x.PriceTypeWorkflowId,
                        principalTable: "PriceTypeWorkflows",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RulesEngineRules_RulesEngineRules_RuleIDFK",
                        column: x => x.RuleIDFK,
                        principalTable: "RulesEngineRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Cages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Length = table.Column<int>(type: "integer", nullable: false),
                    Width = table.Column<int>(type: "integer", nullable: false),
                    Height = table.Column<int>(type: "integer", nullable: false),
                    RegistrationToExhibitionId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cages_RegistrationsToExhibition_RegistrationToExhibitionId",
                        column: x => x.RegistrationToExhibitionId,
                        principalTable: "RegistrationsToExhibition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CatRegistrations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Note = table.Column<string>(type: "text", nullable: true),
                    RegistrationToExhibitionId = table.Column<int>(type: "integer", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatRegistrations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CatRegistrations_RegistrationsToExhibition_RegistrationToEx~",
                        column: x => x.RegistrationToExhibitionId,
                        principalTable: "RegistrationsToExhibition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PaymentInfos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PaymentRequestDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    PaymentCompletedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    PaymentIntentId = table.Column<string>(type: "text", nullable: true),
                    PaymentType = table.Column<int>(type: "integer", nullable: false),
                    SessionId = table.Column<string>(type: "text", nullable: true),
                    RegistrationToExhibitionId = table.Column<int>(type: "integer", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentInfos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentInfos_RegistrationsToExhibition_RegistrationToExhibi~",
                        column: x => x.RegistrationToExhibitionId,
                        principalTable: "RegistrationsToExhibition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PersonRegistrations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    City = table.Column<string>(type: "text", nullable: false),
                    Street = table.Column<string>(type: "text", nullable: false),
                    HouseNumber = table.Column<string>(type: "text", nullable: false),
                    ZipCode = table.Column<string>(type: "text", nullable: false),
                    Organization = table.Column<string>(type: "text", nullable: true),
                    MemberNumber = table.Column<string>(type: "text", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false),
                    Country = table.Column<string>(type: "text", nullable: false),
                    DateOfBirth = table.Column<DateOnly>(type: "date", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    PhoneNumber = table.Column<string>(type: "text", nullable: false),
                    RegistrationToExhibitionId = table.Column<int>(type: "integer", nullable: false),
                    IsPartOfCsch = table.Column<bool>(type: "boolean", nullable: false),
                    IsPartOfFife = table.Column<bool>(type: "boolean", nullable: false),
                    EmailToOrganization = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonRegistrations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PersonRegistrations_RegistrationsToExhibition_RegistrationT~",
                        column: x => x.RegistrationToExhibitionId,
                        principalTable: "RegistrationsToExhibition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RulesEngineScopedParams",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Expression = table.Column<string>(type: "text", nullable: false),
                    PriceAdjustmentWorkflowId = table.Column<int>(type: "integer", nullable: true),
                    PriceTypeWorkflowId = table.Column<int>(type: "integer", nullable: true),
                    RulesEngineRuleId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RulesEngineScopedParams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RulesEngineScopedParams_PriceAdjustmentWorkflows_PriceAdjus~",
                        column: x => x.PriceAdjustmentWorkflowId,
                        principalTable: "PriceAdjustmentWorkflows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RulesEngineScopedParams_PriceTypeWorkflows_PriceTypeWorkflo~",
                        column: x => x.PriceTypeWorkflowId,
                        principalTable: "PriceTypeWorkflows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RulesEngineScopedParams_RulesEngineRules_RulesEngineRuleId",
                        column: x => x.RulesEngineRuleId,
                        principalTable: "RulesEngineRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CatDays",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RentedCageTypeId = table.Column<int>(type: "integer", nullable: true),
                    ExhibitorsCage = table.Column<int>(type: "integer", nullable: true),
                    ExhibitionDayId = table.Column<int>(type: "integer", nullable: false),
                    CatRegistrationId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatDays", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CatDays_Cages_ExhibitorsCage",
                        column: x => x.ExhibitorsCage,
                        principalTable: "Cages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CatDays_CatRegistrations_CatRegistrationId",
                        column: x => x.CatRegistrationId,
                        principalTable: "CatRegistrations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CatDays_ExhibitionDays_ExhibitionDayId",
                        column: x => x.ExhibitionDayId,
                        principalTable: "ExhibitionDays",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CatDays_RentedTypes_RentedCageTypeId",
                        column: x => x.RentedCageTypeId,
                        principalTable: "RentedTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ExhibitedCats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Sex = table.Column<int>(type: "integer", nullable: false),
                    Neutered = table.Column<bool>(type: "boolean", nullable: false),
                    TitleBeforeName = table.Column<string>(type: "text", nullable: true),
                    TitleAfterName = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Ems = table.Column<string>(type: "text", nullable: false),
                    PedigreeNumber = table.Column<string>(type: "text", nullable: true),
                    Colour = table.Column<string>(type: "text", nullable: true),
                    Group = table.Column<int>(type: "integer", nullable: true),
                    BirthDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Breed = table.Column<string>(type: "text", nullable: false),
                    CatRegistrationId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExhibitedCats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExhibitedCats_CatRegistrations_CatRegistrationId",
                        column: x => x.CatRegistrationId,
                        principalTable: "CatRegistrations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Litters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PassOfOrigin = table.Column<string>(type: "text", nullable: true),
                    NameOfBreedingStation = table.Column<string>(type: "text", nullable: false),
                    BirthDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Breed = table.Column<string>(type: "text", nullable: false),
                    CatRegistrationId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Litters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Litters_CatRegistrations_CatRegistrationId",
                        column: x => x.CatRegistrationId,
                        principalTable: "CatRegistrations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Amounts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Currency = table.Column<int>(type: "integer", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    PriceId = table.Column<int>(type: "integer", nullable: true),
                    PaymentInfoId = table.Column<int>(type: "integer", nullable: true),
                    AdvertisementId = table.Column<int>(type: "integer", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Amounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Amounts_Advertisements_AdvertisementId",
                        column: x => x.AdvertisementId,
                        principalTable: "Advertisements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Amounts_PaymentInfos_PaymentInfoId",
                        column: x => x.PaymentInfoId,
                        principalTable: "PaymentInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Amounts_Prices_PriceId",
                        column: x => x.PriceId,
                        principalTable: "Prices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CatDayToGroupsJoinTable",
                columns: table => new
                {
                    CatDaysId = table.Column<int>(type: "integer", nullable: false),
                    GroupsGroupId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatDayToGroupsJoinTable", x => new { x.CatDaysId, x.GroupsGroupId });
                    table.ForeignKey(
                        name: "FK_CatDayToGroupsJoinTable_CatDays_CatDaysId",
                        column: x => x.CatDaysId,
                        principalTable: "CatDays",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CatDayToGroupsJoinTable_Groups_GroupsGroupId",
                        column: x => x.GroupsGroupId,
                        principalTable: "Groups",
                        principalColumn: "GroupId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Breeders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BreederIsSameAsExhibitor = table.Column<bool>(type: "boolean", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false),
                    Country = table.Column<string>(type: "text", nullable: false),
                    ExhibitedCatId = table.Column<int>(type: "integer", nullable: true),
                    LitterId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Breeders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Breeders_ExhibitedCats_ExhibitedCatId",
                        column: x => x.ExhibitedCatId,
                        principalTable: "ExhibitedCats",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Breeders_Litters_LitterId",
                        column: x => x.LitterId,
                        principalTable: "Litters",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Parents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TitleBeforeName = table.Column<string>(type: "text", nullable: true),
                    TitleAfterName = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Ems = table.Column<string>(type: "text", nullable: true),
                    PedigreeNumber = table.Column<string>(type: "text", nullable: true),
                    Colour = table.Column<string>(type: "text", nullable: true),
                    Breed = table.Column<string>(type: "text", nullable: true),
                    ExhibitedCatIsMotherOfId = table.Column<int>(type: "integer", nullable: true),
                    ExhibitedCatIsFatherOfId = table.Column<int>(type: "integer", nullable: true),
                    LitterIsMotherOfId = table.Column<int>(type: "integer", nullable: true),
                    LitterIsFatherOfId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Parents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Parents_ExhibitedCats_ExhibitedCatIsFatherOfId",
                        column: x => x.ExhibitedCatIsFatherOfId,
                        principalTable: "ExhibitedCats",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Parents_ExhibitedCats_ExhibitedCatIsMotherOfId",
                        column: x => x.ExhibitedCatIsMotherOfId,
                        principalTable: "ExhibitedCats",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Parents_Litters_LitterIsFatherOfId",
                        column: x => x.LitterIsFatherOfId,
                        principalTable: "Litters",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Parents_Litters_LitterIsMotherOfId",
                        column: x => x.LitterIsMotherOfId,
                        principalTable: "Litters",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_ExhibitionId",
                table: "Addresses",
                column: "ExhibitionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Advertisements_ExhibitionId",
                table: "Advertisements",
                column: "ExhibitionId");

            migrationBuilder.CreateIndex(
                name: "IX_Amounts_AdvertisementId",
                table: "Amounts",
                column: "AdvertisementId");

            migrationBuilder.CreateIndex(
                name: "IX_Amounts_PaymentInfoId",
                table: "Amounts",
                column: "PaymentInfoId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Amounts_PriceId",
                table: "Amounts",
                column: "PriceId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_Email",
                table: "AspNetUsers",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_OrganizationId",
                table: "AspNetUsers",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_UserName",
                table: "AspNetUsers",
                column: "UserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Breeders_ExhibitedCatId",
                table: "Breeders",
                column: "ExhibitedCatId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Breeders_LitterId",
                table: "Breeders",
                column: "LitterId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cages_RegistrationToExhibitionId",
                table: "Cages",
                column: "RegistrationToExhibitionId");

            migrationBuilder.CreateIndex(
                name: "IX_CatDays_CatRegistrationId",
                table: "CatDays",
                column: "CatRegistrationId");

            migrationBuilder.CreateIndex(
                name: "IX_CatDays_ExhibitionDayId",
                table: "CatDays",
                column: "ExhibitionDayId");

            migrationBuilder.CreateIndex(
                name: "IX_CatDays_ExhibitorsCage",
                table: "CatDays",
                column: "ExhibitorsCage");

            migrationBuilder.CreateIndex(
                name: "IX_CatDays_RentedCageTypeId",
                table: "CatDays",
                column: "RentedCageTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CatDayToGroupsJoinTable_GroupsGroupId",
                table: "CatDayToGroupsJoinTable",
                column: "GroupsGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_CatRegistrations_RegistrationToExhibitionId",
                table: "CatRegistrations",
                column: "RegistrationToExhibitionId");

            migrationBuilder.CreateIndex(
                name: "IX_ExhibitedCats_CatRegistrationId",
                table: "ExhibitedCats",
                column: "CatRegistrationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExhibitionDayPriceJoinTable_PriceId",
                table: "ExhibitionDayPriceJoinTable",
                column: "PriceId");

            migrationBuilder.CreateIndex(
                name: "IX_ExhibitionDays_ExhibitionId",
                table: "ExhibitionDays",
                column: "ExhibitionId");

            migrationBuilder.CreateIndex(
                name: "IX_Exhibitions_OrganizationId",
                table: "Exhibitions",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Exhibitors_AspNetUserId",
                table: "Exhibitors",
                column: "AspNetUserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Litters_CatRegistrationId",
                table: "Litters",
                column: "CatRegistrationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Parents_ExhibitedCatIsFatherOfId",
                table: "Parents",
                column: "ExhibitedCatIsFatherOfId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Parents_ExhibitedCatIsMotherOfId",
                table: "Parents",
                column: "ExhibitedCatIsMotherOfId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Parents_LitterIsFatherOfId",
                table: "Parents",
                column: "LitterIsFatherOfId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Parents_LitterIsMotherOfId",
                table: "Parents",
                column: "LitterIsMotherOfId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentInfos_RegistrationToExhibitionId",
                table: "PaymentInfos",
                column: "RegistrationToExhibitionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PersonRegistrations_RegistrationToExhibitionId",
                table: "PersonRegistrations",
                column: "RegistrationToExhibitionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PriceAdjustmentWorkflows_ExhibitionId",
                table: "PriceAdjustmentWorkflows",
                column: "ExhibitionId");

            migrationBuilder.CreateIndex(
                name: "IX_PriceToGroupsJoinTable_PricesId",
                table: "PriceToGroupsJoinTable",
                column: "PricesId");

            migrationBuilder.CreateIndex(
                name: "IX_PriceTypeWorkflows_ExhibitionId",
                table: "PriceTypeWorkflows",
                column: "ExhibitionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RegistrationsToExhibition_AdvertisementId",
                table: "RegistrationsToExhibition",
                column: "AdvertisementId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrationsToExhibition_ExhibitionId_ExhibitorId",
                table: "RegistrationsToExhibition",
                columns: new[] { "ExhibitionId", "ExhibitorId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RegistrationsToExhibition_ExhibitorId",
                table: "RegistrationsToExhibition",
                column: "ExhibitorId");

            migrationBuilder.CreateIndex(
                name: "IX_RentedCageAndExhibitionDayJoinTable_ExhibitionDaysId",
                table: "RentedCageAndExhibitionDayJoinTable",
                column: "ExhibitionDaysId");

            migrationBuilder.CreateIndex(
                name: "IX_RentedTypes_CageId",
                table: "RentedTypes",
                column: "CageId");

            migrationBuilder.CreateIndex(
                name: "IX_RulesEngineRules_PriceAdjustmentWorkflowId",
                table: "RulesEngineRules",
                column: "PriceAdjustmentWorkflowId");

            migrationBuilder.CreateIndex(
                name: "IX_RulesEngineRules_PriceTypeWorkflowId",
                table: "RulesEngineRules",
                column: "PriceTypeWorkflowId");

            migrationBuilder.CreateIndex(
                name: "IX_RulesEngineRules_RuleIDFK",
                table: "RulesEngineRules",
                column: "RuleIDFK");

            migrationBuilder.CreateIndex(
                name: "IX_RulesEngineScopedParams_PriceAdjustmentWorkflowId",
                table: "RulesEngineScopedParams",
                column: "PriceAdjustmentWorkflowId");

            migrationBuilder.CreateIndex(
                name: "IX_RulesEngineScopedParams_PriceTypeWorkflowId",
                table: "RulesEngineScopedParams",
                column: "PriceTypeWorkflowId");

            migrationBuilder.CreateIndex(
                name: "IX_RulesEngineScopedParams_RulesEngineRuleId",
                table: "RulesEngineScopedParams",
                column: "RulesEngineRuleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Addresses");

            migrationBuilder.DropTable(
                name: "Amounts");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "Breeders");

            migrationBuilder.DropTable(
                name: "CatDayToGroupsJoinTable");

            migrationBuilder.DropTable(
                name: "DataProtectionKeys");

            migrationBuilder.DropTable(
                name: "ExhibitionDayPriceJoinTable");

            migrationBuilder.DropTable(
                name: "Parents");

            migrationBuilder.DropTable(
                name: "PersonRegistrations");

            migrationBuilder.DropTable(
                name: "PriceToGroupsJoinTable");

            migrationBuilder.DropTable(
                name: "RentedCageAndExhibitionDayJoinTable");

            migrationBuilder.DropTable(
                name: "RulesEngineScopedParams");

            migrationBuilder.DropTable(
                name: "PaymentInfos");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "CatDays");

            migrationBuilder.DropTable(
                name: "ExhibitedCats");

            migrationBuilder.DropTable(
                name: "Litters");

            migrationBuilder.DropTable(
                name: "Groups");

            migrationBuilder.DropTable(
                name: "Prices");

            migrationBuilder.DropTable(
                name: "RulesEngineRules");

            migrationBuilder.DropTable(
                name: "Cages");

            migrationBuilder.DropTable(
                name: "ExhibitionDays");

            migrationBuilder.DropTable(
                name: "RentedTypes");

            migrationBuilder.DropTable(
                name: "CatRegistrations");

            migrationBuilder.DropTable(
                name: "PriceAdjustmentWorkflows");

            migrationBuilder.DropTable(
                name: "PriceTypeWorkflows");

            migrationBuilder.DropTable(
                name: "RentedCages");

            migrationBuilder.DropTable(
                name: "RegistrationsToExhibition");

            migrationBuilder.DropTable(
                name: "Advertisements");

            migrationBuilder.DropTable(
                name: "Exhibitors");

            migrationBuilder.DropTable(
                name: "Exhibitions");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Organizations");
        }
    }
}
