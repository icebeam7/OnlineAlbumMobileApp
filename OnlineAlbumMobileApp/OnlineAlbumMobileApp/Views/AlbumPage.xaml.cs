using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using OnlineAlbumMobileApp.Models;
using OnlineAlbumMobileApp.Services;

namespace OnlineAlbumMobileApp.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AlbumPage : ContentPage
	{
		public AlbumPage ()
		{
			InitializeComponent ();
		}

        void Loading(bool show)
        {
            indicator.IsEnabled = show;
            indicator.IsRunning = show;
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            Loading(true);
            AlbumListView.ItemsSource = await BlobStorageService.GetBlobList();
            Loading(false);
        }

        private async void AddImageButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ImagePage(new OnlineImage()));
        }

        private async void AlbumListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            try
            {
                if (e.SelectedItem != null)
                {
                    var item = (OnlineImage)e.SelectedItem;
                    await Navigation.PushAsync(new ImagePage(item));
                    AlbumListView.SelectedItem = null;
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}