using System.IO;
using System.Text.RegularExpressions;

namespace CourseSchedulingSystem.Utilities
{
    public static class PathUtilities
    {
        public static string GetApplicationRoot()
        {
            var exePath = Path.GetDirectoryName(System.Reflection
                .Assembly.GetExecutingAssembly().CodeBase);
            Regex appPathMatcher = new Regex(@"(?<!fil)[A-Za-z]:\\+[\S\s]*?(?=\\+bin)");
            var appRoot = appPathMatcher.Match(exePath).Value;
            return appRoot;
        }

        public static string GetResourceDirectory()
        {
            return Path.Combine(PathUtilities.GetApplicationRoot(), "Resources");
        }
    }
}