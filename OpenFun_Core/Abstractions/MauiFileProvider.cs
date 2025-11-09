namespace OpenFun_Core.Abstractions
{
    public class MauiFileProvider : IAppFileProvider
    {
        public Task<Stream> OpenAppPackageFileAsync(string filename)
            => FileSystem.OpenAppPackageFileAsync(Path.Combine("Resources", "Raw", filename));
    }
}
