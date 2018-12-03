using System;
using System.IO;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Plugin.Media.Abstractions;

using OnlineAlbumMobileApp.Helpers;
using OnlineAlbumMobileApp.Models;
using OnlineAlbumMobileApp.Services;

namespace OnlineAlbumMobileApp.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ImagePage : ContentPage
	{
        OnlineImage picture;
        MediaFile mediaFile;

        public ImagePage (OnlineImage picture)
		{
			InitializeComponent ();

            this.BindingContext = picture;
            this.picture = picture;

            if (string.IsNullOrWhiteSpace(picture.FileName))
                ToolbarItems.RemoveAt(2);
		}

        void Loading(bool mostrar)
        {
            indicator.IsEnabled = mostrar;
            indicator.IsRunning = mostrar;
        }

        private async void SelectImageButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                Loading(true);

                mediaFile = await ImageService.PickPhoto();

                if (mediaFile != null)
                {
                    PictureImage.Source = ImageSource.FromStream(mediaFile.GetStream);
                }
            }
            catch(Exception ex)
            {

            }
            finally
            {
                Loading(true);
            }
        }

        private async void UploadImageButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                Loading(true);
                var extension = Path.GetExtension(mediaFile.Path);
                var uri = await BlobStorageService.UploadBlob(mediaFile.GetStream(), extension);

                await DisplayAlert("Success!", "The image has been uploaded successfully!", "OK");
                await Navigation.PopAsync();
            }
            catch(Exception ex)
            {
                await DisplayAlert("Error!", $"Exception message: {ex.Message}", "OK");
            }
            finally
            {
                Loading(false);
            }
        }

        private async void DeleteImageButton_Clicked(object sender, EventArgs e)
        {
            if (await DisplayAlert("Warning!", "Are you sure?", "Yes", "No"))
            {
                Loading(true);

                if (await BlobStorageService.DeleteBlob(picture.FileName))
                {
                    await DisplayAlert("Success!", "Image deleted successfully", "OK");
                    await Navigation.PopAsync();
                }
                else
                    await DisplayAlert("Error!", "There was an error trying to remove the image", "OK");

                Loading(false);
            }
        }
    }
}