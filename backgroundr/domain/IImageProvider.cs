using System.Collections.Generic;
using System.Threading.Tasks;

namespace backgroundr.domain
{
    public interface IImageProvider
    {
        Task<IReadOnlyCollection<string>> GetImageUrls();
    }
}