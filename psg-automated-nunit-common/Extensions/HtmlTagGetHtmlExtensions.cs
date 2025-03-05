using AngleSharp.Html;
using AngleSharp.Html.Parser;
using HtmlTags;
using psg_automated_nunit_common.Configurations;
using psg_automated_nunit_common.Managers;


namespace psg_automated_nunit_common.Extensions
{
    public static class HtmlTagGetHtmlExtensions
    {
        /// <summary>
        /// Gets the Html and adds the Css styles in front
        /// </summary>      
        /// <returns></returns>
        public static string GetHtml(this HtmlTag tag, TestRunnerConfiguration config)
        {
            var html = tag.ToString();

            var styleCss = GetStyles("style.scss");
         

            var placeholderReplacer = new PlaceholderReplacer(new Dictionary<string, string>
            {
                {"url", config?.Url ?? ""},
                {"apiSecret", config?.Secret ?? ""},                
            });

            var script = GetScripts("script.js", placeholderReplacer);
            
            html = styleCss + html + script;


            string indentedText;
            //indent and parse the html

            var parser = new HtmlParser();
            var document = parser.ParseDocument(html);

            using (var writer = new StringWriter())
            {
                document.ToHtml(writer, new PrettyMarkupFormatter
                {
                    Indentation = "\t",
                    NewLine = "\n"

                });
                indentedText = writer.ToString();
            }

            indentedText = indentedText.Sanitise();

            return indentedText;
        }

        private static string GetStyles(string name)
        {
            var css = CssManager.GetCss(name);
            css = $"<style>{css}</style>";

            return css;
        }

        private static string GetScripts(string name, PlaceholderReplacer? replacer = default)
        {
            var script = ScriptManager.GetScript(name);

            if(replacer != null)
            {
                script = replacer.ReplacePlaceholders(script);
            }

            script = $"<script>{script}</script>";

            return script;
        }
    }
}
