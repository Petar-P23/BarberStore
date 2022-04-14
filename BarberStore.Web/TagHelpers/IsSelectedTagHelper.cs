using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace BarberStore.Web.TagHelpers
{
    [HtmlTargetElement(Attributes = "is-selected")]
    public class IsSelectedTagHelper : TagHelper
    {
        private IUrlHelper urlHelper;

        public IsSelectedTagHelper(IUrlHelper urlHelper)
        {
            this.urlHelper = urlHelper;
        }
        public string CssClass { get; set; } = "selected";
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.Attributes.Remove(output.Attributes["is-selected"]);

            var url = output.Attributes["href"].Value.ToString();
            var currentRouteUrl = this.urlHelper.Action();
            if (url == currentRouteUrl)
            {
                var linkTag = new TagBuilder("a");
                linkTag.Attributes.Add("class", this.CssClass);
                output.MergeAttributes(linkTag);
            }
        }
    }
}
