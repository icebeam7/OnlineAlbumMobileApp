using System;
using System.Threading.Tasks;

using Plugin.Media;
using Plugin.Media.Abstractions;

namespace OnlineAlbumMobileApp.Services
{
    public static class ImageService
    {
        public static async Task<MediaFile> PickPhoto()
        {
            try
            {
                await CrossMedia.Current.Initialize();
                return await CrossMedia.Current.PickPhotoAsync();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }

}