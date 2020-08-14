using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using PasswordManager.Models.InputModels;

namespace PasswordManager.Customizations.TagHelpers
{
    //<order-link order-by="Id" Input="@Model.Input">Id</order-link>
    //<a asp-route-orderby="Id" 
       //asp-route-ascending="@(Model.Input.Orderby == "Id" ? !Model.Input.Ascending : true)" 
       //asp-route-search="@Model.Input.Search">Id @if(Model.Input.Orderby == "Id") { if(Model.Input.Ascending == true) { <h6>v</h6> }else{ <h6>^</h6> } }</a> 
    public class OrderLinkTagHelper : AnchorTagHelper
    {
        public string OrderBy {get; set; }
        public PasswordListInputModel Input {get; set;}
        public OrderLinkTagHelper(IHtmlGenerator generator) : base(generator)
        {
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "a";
            RouteValues["search"] = Input.Search;
            RouteValues["orderby"] = OrderBy;
            if ((Input.Orderby == OrderBy) & (Input.Ascending == true))
            {
                RouteValues["ascending"] = "false";
            }
            else
            {
                RouteValues["ascending"] = "true";
            }
            base.Process(context, output);
            if (Input.Orderby == OrderBy)
            {
                if (Input.Ascending == true)
                {
                    output.PostContent.SetHtmlContent($"<h6>v</h6>");
                }
                else
                {
                    output.PostContent.SetHtmlContent($"<h6>^</h6>");
                }
            }
            
        }
    }
}