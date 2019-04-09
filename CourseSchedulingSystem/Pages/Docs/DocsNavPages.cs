using System;
using System.IO;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CourseSchedulingSystem.Pages.Docs
{
    public static class DocsNavPages
    {
        //Button for front page of tutorial
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
        /*public static string Settings(ViewContext viewContext)
        {
            return PageNavClass(viewContext, "Settings");
        }*/

        //Button for the Add and Manage tutorial
        public static string AddManage(ViewContext viewContext)
        {
            return PageNavClass(viewContext, "AddManage");
        }

        //schedule making tutorial button
        public static string SchedulingCourses(ViewContext viewContext)
        {
            return PageNavClass(viewContext, "SchedulingCourses");
        }

        //Term button
        public static string Term(ViewContext viewContext)
        {
            return PageNavClass(viewContext, "Term");
        }

        //instructional methods button
        public static string Methods(ViewContext viewContext)
        {
            return PageNavClass(viewContext, "Methods");
        }

        //courses button
        public static string Courses(ViewContext viewContext)
        {
            return PageNavClass(viewContext, "Courses");
        }

        //Subjects button
        public static string Subjects(ViewContext viewContext)
        {
            return PageNavClass(viewContext, "Subjects");
        }

        //Schedule Types button
        public static string ScheduleTypes(ViewContext viewContext)
        {
            return PageNavClass(viewContext, "ScheduleTypes");
        }

        //Attributes button
        public static string Attributes(ViewContext viewContext)
        {
            return PageNavClass(viewContext, "Attributes");
        }

        //departments button
        public static string Departments(ViewContext viewContext)
        {
            return PageNavClass(viewContext, "Departments");
        }

        //Instructors button
        public static string Instructors(ViewContext viewContext)
        {
            return PageNavClass(viewContext, "Instructors");
        }

        //buildings button
        public static string Buildings(ViewContext viewContext)
        {
            return PageNavClass(viewContext, "Buildings");
        }

        //rooms button
        public static string Rooms(ViewContext viewContext)
        {
            return PageNavClass(viewContext, "Rooms");
        }

        //users button
        public static string Users(ViewContext viewContext)
        {
            return PageNavClass(viewContext, "Users");
        }

        private static string PageNavClass(ViewContext viewContext, string page)
        {
            var activePage = viewContext.ViewData["ActivePage"] as string
                             ?? Path.GetFileNameWithoutExtension(viewContext.ActionDescriptor.DisplayName);
            return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase) ? "active" : null;
        }
    }
}