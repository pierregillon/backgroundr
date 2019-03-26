using System.Collections.Generic;
using System.Threading.Tasks;

namespace backgroundr.domain
{
    public interface IPhotoProvider
    {
        Task<IReadOnlyCollection<string>> GetPhotos();
    }
}