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
            string sAsc = "";
            string sDirection = "";
            if ((Input.Orderby == OrderBy) & (Input.Ascending == true))
            {
                sAsc = "false";
                sDirection = "^"; 
            }
            else
            {
                sAsc = "true";
                sDirection = "v"; 
            }

            output.TagName = "a";
            RouteValues["search"] = Input.Search;
            RouteValues["orderby"] = OrderBy;
            //RouteValues["ascending"] = (Input.Orderby == OrderBy ? !Input.Ascending : Input.Ascending).ToString().ToLowerInvariant();
            RouteValues["ascending"] = sAsc;
            base.Process(context, output);
            output.PostContent.SetHtmlContent($"<h6>{sDirection}</h6>");
        }
    }
}