using System.IO;
using System.Net;
using System.Threading.Tasks;
using backgroundr.domain;

namespace backgroundr.infrastructure
{
    public class HttpFileDownloader : IFileDownloader
    {
        public async Task<string> Download(string url)
        {
            var httpWebRequest = (HttpWebRequest) WebRequest.Create(url);
            var httpWebReponse = (HttpWebResponse) httpWebRequest.GetResponse();
            var stream = httpWebReponse.GetResponseStream();
            var tempFilePath = Path.GetTempFileName();
            using (var fileStream = File.Create(tempFilePath)) {
                await stream.CopyToAsync(fileStream);
            }
            return tempFilePath;
        }
    }
}