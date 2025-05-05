using OpenFun_Core.Abstractions;
using System.Text.RegularExpressions;

namespace OpenFun_CoreTests.Mock
{
    public class TestFileProvider : IAppFileProvider
    {
        public Task<Stream> OpenAppPackageFileAsync(string filename)
        {
            string startDirectory = Directory.GetCurrentDirectory();
            string openFunRoot = FindOpenFunRoot(startDirectory)!;
            string targetPath = FindTargetPath(openFunRoot)!;
            string filepath = Path.Combine(targetPath, filename);

            Stream stream = File.OpenRead(filepath);
            return Task.FromResult(stream);
        }

        private string? FindOpenFunRoot(string currentDir)
        {
            while (currentDir != null)
            {
                string parentDir = Directory.GetParent(currentDir)!.FullName;
                if (Path.GetFileName(currentDir) == "OpenFun" && (parentDir == null || Path.GetFileName(parentDir) != "OpenFun"))
                {
                    return currentDir;
                }
                currentDir = parentDir;
            }
            return null;
        }

        private string? FindTargetPath(string openFunRoot)
        {
            string binPath = Path.Combine(openFunRoot, "OpenFun", "bin");
            if (!Directory.Exists(binPath))
                return null;

            IEnumerable<string> debugOrReleaseDirs = Directory.GetDirectories(binPath)
                .Where(dir => Regex.IsMatch(Path.GetFileName(dir), "^(Debug|Release)$", RegexOptions.IgnoreCase));

            foreach (string? debugOrRelease in debugOrReleaseDirs)
            {
                IEnumerable<string> windowsDirs = Directory.GetDirectories(debugOrRelease)
                    .Where(dir => Regex.IsMatch(dir, ".*windows.*", RegexOptions.IgnoreCase));

                foreach (string? windowsDir in windowsDirs)
                {
                    string[] subDirs = Directory.GetDirectories(windowsDir);
                    foreach (string subDir in subDirs)
                    {
                        string resourcesRawPath = Path.Combine(subDir, "Resources", "Raw");
                        if (Directory.Exists(resourcesRawPath))
                        {
                            return resourcesRawPath;
                        }
                    }
                }
            }
            return null;
        }

    }
}
