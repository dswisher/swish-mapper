
using System.IO;

namespace SwishMapper.Tests.TestHelpers
{
    public static class FileFinder
    {
        public static string FindXsd(string name)
        {
            return Path.Join(FindDataDir().FullName, "xsd", name);
        }


        public static string FindProject(string name)
        {
            return Path.Join(FindDataDir().FullName, "projects", name);
        }


        private static DirectoryInfo FindDataDir()
        {
            const string dataDirectory = "TestData";

            var directory = new DirectoryInfo(Directory.GetCurrentDirectory());
            while ((directory != null) && (directory.GetDirectories(dataDirectory).Length == 0))
            {
                directory = directory.Parent;
            }

            if (directory == null)
            {
                throw new DirectoryNotFoundException($"Could not find {dataDirectory} directory.");
            }

            return directory.GetDirectories(dataDirectory)[0];
        }
    }
}
