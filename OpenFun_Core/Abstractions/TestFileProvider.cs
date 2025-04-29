using System.Text.RegularExpressions;

namespace OpenFun_Core.Abstractions
{
    public class TestFileProvider : IAppFileProvider
    {
        public Task<Stream> OpenAppPackageFileAsync(string filename)
        {
            string fullPath = Directory.GetCurrentDirectory();
            string pattern = @"OpenFun_CoreTests";

            string replacement = "OpenFun";

            string updatedPath = Regex.Replace(fullPath, pattern, replacement);

            updatedPath += "\\win10-x64\\Resources\\Raw";

            updatedPath = Path.Combine(updatedPath, filename);

            Stream stream = File.OpenRead(updatedPath);
            return Task.FromResult(stream);
        }
    }
}
