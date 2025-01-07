using E_Learning.Models;
using iTextSharp.text;
using iTextSharp.tool.xml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Web;
using System.Web.Mvc;

namespace E_Learning.Common
{
    public class PdfHelper
    {
        public static string RenderViewToString(ControllerContext context, string viewName, object model)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = context.RouteData.GetRequiredString("action");

            context.Controller.ViewData.Model = model;

            using (StringWriter sw = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindView(context, viewName, null);
                ViewContext viewContext = new ViewContext(context, viewResult.View, context.Controller.ViewData, context.Controller.TempData, sw);
                viewResult.View.Render(viewContext, sw);
                return sw.GetStringBuilder().ToString();
            }
        }
    }
    public class UnicodeFontFactory : XMLWorkerFontProvider
    {
        private readonly Font _defaultFont;

        public UnicodeFontFactory(Font defaultFont)
        {
            _defaultFont = defaultFont;
        }

        public override iTextSharp.text.Font GetFont(string fontName, string encoding, bool embedded, float size, int style, BaseColor color)
        {
            if (_defaultFont != null) return _defaultFont;
            return base.GetFont(fontName, encoding, embedded, size, style, color);
        }
    }
}