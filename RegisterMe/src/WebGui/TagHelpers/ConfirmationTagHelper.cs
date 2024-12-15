#region

using Microsoft.AspNetCore.Razor.TagHelpers;
using Newtonsoft.Json;

#endregion

namespace WebGui.TagHelpers;

[HtmlTargetElement("Confirmation")]
public class ConfirmationTagHelper : TagHelper
{
    /// <summary>
    ///     Render output
    /// </summary>
    /// <param name="context"></param>
    /// <param name="output"></param>
    /// <returns></returns>
    public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        if (!AspController.EndsWith("Controller"))
        {
            throw new Exception("Controller name must end with 'Controller'");
        }

        string controllerNameWithoutController = AspController[..^"Controller".Length];
        string link = "/" + AspArea + "/" + controllerNameWithoutController + "/" + AspAction;

        output.TagName = "button";
        output.Attributes.SetAttribute("type", "button");
        output.Attributes.SetAttribute("class", Class);

        string serializedData = JsonConvert.SerializeObject(FormData);

        string onClickValue = $"ShowConfirmation('{link}', {serializedData}, '{Title}', '{Text}', '{ConfirmText}')";
        output.Attributes.SetAttribute("onclick", onClickValue);

        return Task.CompletedTask;
    }

    #region Proprietes

    public string AspAction { get; set; } = null!;
    public string AspController { get; set; } = null!;
    public string AspArea { get; set; } = null!;
    public Dictionary<string, string> FormData { get; set; } = null!;
    public string Title { get; set; } = "Jste si jistý/á";
    public string Text { get; set; } = string.Empty;
    public string ConfirmText { get; set; } = "Zrušit";
    public string Class { get; set; } = "btn btn-danger";

    #endregion
}
