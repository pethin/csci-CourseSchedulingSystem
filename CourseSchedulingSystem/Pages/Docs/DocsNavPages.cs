using System;
using System.IO;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CourseSchedulingSystem.Pages.Docs
{
    public static class DocsNavPages
    {
        //Button for front page of documentation
        public static string IndexNavClass(ViewContext viewContext)
        {
            return PageNavClass(viewContext, "Index");
        }

        //Button for the main menu section of tutorial
        public static string MainMenu(ViewContext viewContext)
        {
            return PageNavClass(viewContext, "MainMenu");
        }

        //Button for User Settings tutorial
        public static string Settings(ViewContext viewContext)
        {
            return PageNavClass(viewContext, "Settings");
        }

        //Button for the Add and Manage section of tutorial
        public static string AddManage(ViewContext viewContext)
        {
            return PageNavClass(viewContext, "AddManage");
        }

        //Button for Term tutorial
        public static string Term(ViewContext viewContext)
        {
            return PageNavClass(viewContext, "Term");
        }

        //Button for instructional methods tutorial
        public static string Methods(ViewContext viewContext)
        {
            return PageNavClass(viewContext, "Methods");
        }

        //Button for courses tutorial
        public static string Courses(ViewContext viewContext)
        {
            return PageNavClass(viewContext, "Courses");
        }

        //Button for Subjects tutorial
        public static string Subjects(ViewContext viewContext)
        {
            return PageNavClass(viewContext, "Subjects");
        }

        //Button for Schedule Types tutorial
        public static string Schedule(ViewContext viewContext)
        {
            return PageNavClass(viewContext, "Schedule");
        }

        //Button for Attributes tutorial
        public static string Attributes(ViewContext viewContext)
        {
            return PageNavClass(viewContext, "Attributes");
        }

        //Button for buildings tutorial
        public static string Buildings(ViewContext viewContext)
        {
            return PageNavClass(viewContext, "Buildings");
        }

        //Button for rooms tutorial
        public static string Rooms(ViewContext viewContext)
        {
            return PageNavClass(viewContext, "Rooms");
        }

        //Button for users tutorial
        public static string Users(ViewContext viewContext)
        {
            return PageNavClass(viewContext, "Users");
        }

        //Button for groups tutorial
        public static string Groups(ViewContext viewContext)
        {
            return PageNavClass(viewContext, "Groups");
        }

        private static string PageNavClass(ViewContext viewContext, string page)
        {
            var activePage = viewContext.ViewData["ActivePage"] as string
                             ?? Path.GetFileNameWithoutExtension(viewContext.ActionDescriptor.DisplayName);
            return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase) ? "active" : null;
        }
    }
}