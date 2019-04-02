using System;
using System.IO;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CourseSchedulingSystem.Pages.Docs
{
    public static class DocsNavPages
    {
        public static string IndexNavClass(ViewContext viewContext)
        {
            return PageNavClass(viewContext, "Index");
        }

        public static string OtherPage(ViewContext viewContext)
        {
            return PageNavClass(viewContext, "OtherPage");
        }

        private static string PageNavClass(ViewContext viewContext, string page)
        {
            var activePage = viewContext.ViewData["ActivePage"] as string
                             ?? Path.GetFileNameWithoutExtension(viewContext.ActionDescriptor.DisplayName);
            return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase) ? "active" : null;
        }
    }
}