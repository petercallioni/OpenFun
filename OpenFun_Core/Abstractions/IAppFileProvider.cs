namespace OpenFun_Core.Abstractions
{
    public interface IAppFileProvider
    {
        Task<Stream> OpenAppPackageFileAsync(string filename);
    }
}
