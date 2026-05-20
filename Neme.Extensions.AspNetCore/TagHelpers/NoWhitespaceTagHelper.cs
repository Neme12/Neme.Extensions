using HtmlAgilityPack;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Neme.Extensions.AspNetCore.TagHelpers;

[HtmlTargetElement("no-whitespace")]
public sealed class NoWhitespaceTagHelper : TagHelper
{
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = null;

        var childContent = await output.GetChildContentAsync(NullHtmlEncoder.Default);
        var content = childContent.GetContent();

        var document = new HtmlDocument();
        document.LoadHtml(content);

        RemoveWhitespace(document);

        output.Content.SetHtmlContent(document.DocumentNode.InnerHtml);
    }

    private static void RemoveWhitespace(HtmlDocument document)
    {
        RemoveWhitespace(document.DocumentNode);

        static bool RemoveWhitespace(HtmlNode node)
        {
            if (node is HtmlTextNode textNode)
            {
                var trimmed = textNode.Text.Trim();
                if (trimmed.Length == 0)
                {
                    textNode.ParentNode.RemoveChild(textNode);
                    return true;
                }
                else
                {
                    textNode.Text = trimmed;
                    return false;
                }
            }

            for (int i = 0; i < node.ChildNodes.Count;)
            {
                if (!RemoveWhitespace(node.ChildNodes[i]))
                    ++i;
            }

            return false;
        }
    }
}
