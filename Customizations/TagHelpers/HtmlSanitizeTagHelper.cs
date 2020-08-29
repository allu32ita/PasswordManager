using System.Threading.Tasks;
using AngleSharp.Html.Dom;
using Ganss.XSS;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace PasswordManager.Customizations.TagHelpers
{
    [HtmlTargetElement(Attributes = "html-sanitize")]
    public class HtmlSanitizeTagHelper : TagHelper
    {
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            //otteniamo il contenuto del tag
            TagHelperContent var_TagHelperContent = await output.GetChildContentAsync(NullHtmlEncoder.Default);
            string var_Content = var_TagHelperContent.GetContent(NullHtmlEncoder.Default);

            //sanitizzazione
            var var_Sanitizer = new HtmlSanitizer();
            var_Content = var_Sanitizer.Sanitize(var_Content);

            //reimpostiamo il contenuto del tag
            output.Content.SetHtmlContent(var_Content);
        }

        private static HtmlSanitizer CreateSanitizer()
        {
            var var_Sanitizer = new HtmlSanitizer();

            //tag consentiti
            var_Sanitizer.AllowedTags.Clear();
            var_Sanitizer.AllowedTags.Add("b");
            var_Sanitizer.AllowedTags.Add("i");
            var_Sanitizer.AllowedTags.Add("p");
            var_Sanitizer.AllowedTags.Add("br");
            var_Sanitizer.AllowedTags.Add("ul");
            var_Sanitizer.AllowedTags.Add("ol");
            var_Sanitizer.AllowedTags.Add("li");
            var_Sanitizer.AllowedTags.Add("iframe");
            
            //attributi consentiti
            var_Sanitizer.AllowedAttributes.Clear();
            var_Sanitizer.AllowedAttributes.Add("src");
            var_Sanitizer.AllowDataAttributes = false;

            //stili consentiti
            var_Sanitizer.AllowedCssProperties.Clear();

            var_Sanitizer.FilterUrl += FilterUrl;
            var_Sanitizer.PostProcessNode += ProcessIFrames;

            return var_Sanitizer;
        }

        private static void FilterUrl(object sender, FilterUrlEventArgs par_FilterUrlEventArgs)
        {
            if ((par_FilterUrlEventArgs.OriginalUrl.StartsWith("//www.youtube.com/") == false) && (par_FilterUrlEventArgs.OriginalUrl.StartsWith("https://www.youtube.com/") == false))
            {
                par_FilterUrlEventArgs.SanitizedUrl = null;
            }
        }

        private static void ProcessIFrames(object sender, PostProcessNodeEventArgs par_PostProcessNodeEventArgs)
        {
            var var_Iframe = par_PostProcessNodeEventArgs.Node as IHtmlInlineFrameElement;
            if (var_Iframe == null)
            {
                return;
            }
            var var_Container = par_PostProcessNodeEventArgs.Document.CreateElement("span");
            var_Container.ClassName = "video-container";
            var_Container.AppendChild(var_Iframe.Clone(true));
            par_PostProcessNodeEventArgs.ReplacementNodes.Add(var_Container);
        }
    }
}