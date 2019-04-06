using System;

namespace backgroundr.domain {
    public class NoPhotoFound : Exception
    {
        public NoPhotoFound(string userId, string tags) : base($"No photos found for user {userId} and tags {tags}. Have you checked your credentials ?")
        {
        }
    }
}