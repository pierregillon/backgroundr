using System.Threading.Tasks;

namespace backgroundr.domain
{
    public interface IFileDownloader
    {
        Task<string> Download(string url);
    }
}