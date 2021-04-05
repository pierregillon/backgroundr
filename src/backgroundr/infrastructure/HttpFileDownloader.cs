﻿using System.IO;
using System.Net;
using System.Threading.Tasks;
using backgroundr.domain;

namespace backgroundr.infrastructure
{
    public class HttpFileDownloader : IFileDownloader
    {
        public async Task<string> Download(string url)
        {
            if (IsRemote(url)) {
                return await DownloadUrl(url);
            }
            return url;
        }

        private static async Task<string> DownloadUrl(string url)
        {
            var httpWebRequest = (HttpWebRequest) WebRequest.Create(url);
            var httpWebResponse = (HttpWebResponse) await httpWebRequest.GetResponseAsync();
            await using var stream = httpWebResponse.GetResponseStream();
            var tempFilePath = Path.GetTempFileName();
            await using var fileStream = File.Create(tempFilePath);
            await stream.CopyToAsync(fileStream);
            return tempFilePath;
        }

        private static bool IsRemote(string url) => url.StartsWith("http");
    }
}