using Firma.Mvc.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;
using System;
using System.Linq;

namespace Firma.Mvc.TagHelpers
{
  /// <summary>
  /// Tag helper za kreiranje vlastitih poveznica na stranice u rezultatu nekog upravljača
  /// Upotrebljava se kao atribut HTML oznake *pager* koju mijenja u div
  /// <example>
  /// Primjer upotrebe
  /// ```
  /// <pager page-info="@Model.PagingInfo" page-action="Index" page-title="Unesite željenu stranicu" class="float-right">
  /// </pager>
  /// ```
  /// U datoteku *_ViewImports.cshtml* potrebno dodati `@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers`  
  /// te u pogled uključiti vlastitu javascript datoteku *gotopage.js*
  /// </example>
  /// </summary>  
  [HtmlTargetElement(Attributes = "page-info")]
  public class PagerTagHelper : TagHelper
  {

    private readonly IUrlHelperFactory urlHelperFactory;
    private readonly AppSettings appData;
    public PagerTagHelper(IUrlHelperFactory helperFactory, IOptionsSnapshot<AppSettings> options)
    {
      urlHelperFactory = helperFactory;
      appData = options.Value;
    }

    [ViewContext]
    [HtmlAttributeNotBound]
    public ViewContext ViewContext { get; set; }

    /// <summary>
    /// Serijalizirani string koji sadrži informacije o trenutnoj i ukupnom broju stranicu 
    /// </summary>
    public PagingInfo PageInfo { get; set; }

    /// <summary>
    /// Serijalizirani string kojim se prenose informacije o aktivnom filtriranju podataka
    /// </summary>
    public IPageFilter PageFilter { get; set; }

    /// <summary>
    /// Akcija na koju poveznica treba voditi
    /// </summary>
    public string PageAction { get; set; }

    /// <summary>
    /// Tekst za tooltip za trenutni broj stranice i unos ciljane stranice
    /// </summary>
    public string PageTitle { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
      output.TagName = "nav";
      int offset = appData.PageOffset;
      TagBuilder paginationList = new TagBuilder("ul");
      paginationList.AddCssClass("pagination");

      if (PageInfo.CurrentPage - offset > 1) //create list item for the first page
      {
        var tag = BuildListItemForPage(1, "1..");
        paginationList.InnerHtml.AppendHtml(tag);
      }

      for (int i = Math.Max(1, PageInfo.CurrentPage - offset);
               i <= Math.Min(PageInfo.TotalPages, PageInfo.CurrentPage + offset);
               i++)
      {
        var tag = i == PageInfo.CurrentPage ? BuildListItemForCurrentPage(i) : BuildListItemForPage(i);
        paginationList.InnerHtml.AppendHtml(tag);
      }

      if (PageInfo.CurrentPage + offset < PageInfo.TotalPages) //create list item for the last page
      {
        var tag = BuildListItemForPage(PageInfo.TotalPages, ".. " + PageInfo.TotalPages);
        paginationList.InnerHtml.AppendHtml(tag);
      }

      output.Content.AppendHtml(paginationList);
    }

    /// <summary>
    /// Stvara oznaku za i-tu stranicu koristeći *i* kao sadržaj poveznice
    /// <seealso cref="BuildListItemForPage(int, string)"/>
    /// </summary>
    /// <param name="i">broj stranice</param>
    /// <returns>TagBuilder s pripremljenom poveznicom</returns>
    private TagBuilder BuildListItemForPage(int i)
    {
      return BuildListItemForPage(i, i.ToString());
    }

    /// <summary>
    ///  Stvara oznaku za i-tu stranicu koristeći argument text kao sadržaj poveznice
    /// </summary>
    /// <param name="i">broj stranice</param>
    /// <param name="text">tekst poveznice</param>
    /// <returns>TagBuilder s pripremljenom poveznicom</returns>
    private TagBuilder BuildListItemForPage(int i, string text)
    {
      IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);

      TagBuilder a = new TagBuilder("a");
      a.InnerHtml.Append(text);
      a.Attributes["href"] = urlHelper.Action(PageAction, new
      {
        page = i,
        sort = PageInfo.Sort,
        ascending = PageInfo.Ascending,
        filter = PageFilter?.ToString()
      });
      a.AddCssClass("page-link");

      TagBuilder li = new TagBuilder("li");
      li.AddCssClass("page-item");
      li.InnerHtml.AppendHtml(a);
      return li;
    }

    /// <summary>
    /// Stvara polje za prikaz trenutne stranice i unos željene stranice
    /// </summary>
    /// <param name="page">Broj trenutne stranice</param>
    /// <returns></returns>
    private TagBuilder BuildListItemForCurrentPage(int page)
    {
      IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);
      TagBuilder input = new TagBuilder("input");
      input.Attributes["type"] = "text";
      input.Attributes["value"] = page.ToString();
      input.Attributes["data-current"] = page.ToString();
      input.Attributes["data-min"] = "1";
      input.Attributes["data-max"] = PageInfo.TotalPages.ToString();
      input.Attributes["data-url"] = urlHelper.Action(PageAction, new
      {
        page = -1,
        sort = PageInfo.Sort,
        ascending = PageInfo.Ascending,
        filter = PageFilter?.ToString()
      });
      input.AddCssClass("page-link");
      input.AddCssClass("pagebox");//da ga se može pronaći i stilizirati

      if (!string.IsNullOrWhiteSpace(PageTitle))
      {
        input.Attributes["title"] = PageTitle;
      }

      TagBuilder li = new TagBuilder("li");
      li.AddCssClass("page-item active");
      li.InnerHtml.AppendHtml(input);

      return li;
    }

  }
}
