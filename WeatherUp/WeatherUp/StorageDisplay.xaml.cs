using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace WeatherUp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class StorageDisplay : Page
    {
        public StorageDisplay()
        {
            this.InitializeComponent();
            this.DisplayWeather();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {


            Frame rootFrame = Window.Current.Content as Frame;

            if (rootFrame.CanGoBack)
            {
                // Show UI in title bar if opted-in and in-app backstack is not empty.
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                    AppViewBackButtonVisibility.Visible;
                SystemNavigationManager.GetForCurrentView().BackRequested += StorageDisplay_BackRequested;
            }
            else
            {
                // Remove the UI from the title bar if in-app back stack is empty.
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                    AppViewBackButtonVisibility.Collapsed;
            }
        }

        private void StorageDisplay_BackRequested(object sender, BackRequestedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame == null)
                return;

            // Navigate back if possible, and if the event has not 
            // already been handled .
            if (rootFrame.CanGoBack && e.Handled == false)
            {
                e.Handled = true;
                rootFrame.GoBack();

            }
        }


        private async void DisplayWeather()
        {
            //if no text make it invisible
            if (weatherDisplay.Text.ToString() == "")
            {
                btnClear.Visibility = Visibility.Visible;
            }
            //manage folder + content + show info about them
            //creating StorageFile
            StorageFolder weatherFolder = ApplicationData.Current.LocalFolder;
            StorageFile weatherFile;
            

            try
            {
                weatherFile = await weatherFolder.GetFileAsync("weather.txt");

            }
            catch (Exception E)
            {
                string message = E.Message;
                weatherDisplay.Text = "no weather file";
                return;
            }
            //reading from the file weatherFIle
            string fileText = await Windows.Storage.FileIO.ReadTextAsync(weatherFile);


            //print content to txtBox weatherDisplay
            weatherDisplay.Text = weatherDisplay.Text + fileText;


        }

        private async void btnClear_Tapped(object sender, TappedRoutedEventArgs e)
        {
            StorageFolder weatherFolder = ApplicationData.Current.LocalFolder;

            StorageFile weatherFile;
            

            try
            {
                weatherFile = await weatherFolder.GetFileAsync("weather.txt");

            }
            catch (Exception E)
            {
                string message = E.Message;
                weatherDisplay.Text = "File with saved weather/location does not exist";
                return;
            }

            string emptyText = "";
            await Windows.Storage.FileIO.WriteTextAsync(weatherFile, emptyText);

            weatherDisplay.Text = "";
     
            MessageDialog clearDialog = new MessageDialog("Data Cleared");
            await clearDialog.ShowAsync();
        }
    }
}
