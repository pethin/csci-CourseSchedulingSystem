using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace CourseSchedulingSystem.Utilities
{
    public static class PathUtilities
    {
        public static string GetApplicationRoot()
        {
            var exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
            var appPathMatcher = new Regex(@"(?<!fil)[A-Za-z]:\\+[\S\s]*?(?=\\+bin)");
            var appRoot = appPathMatcher.Match(exePath).Value;
            return appRoot;
        }

        public static string GetResourceDirectory()
        {
            return Path.Combine(GetApplicationRoot(), "Resources");
        }
    }
}