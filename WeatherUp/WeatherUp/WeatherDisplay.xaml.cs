using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Services.Maps;
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
    public sealed partial class WeatherDisplay : Page
    {

        Geolocator geolocator;

        public WeatherDisplay()
        {
            this.InitializeComponent();
            //weather
            this.setupGeoLocation();
            //location
            this.SetupGeo();
        }

        //Navigation back
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {


            Frame rootFrame = Window.Current.Content as Frame;

            if (rootFrame.CanGoBack)
            {
                // Show UI in title bar if opted-in and in-app backstack is not empty.
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                    AppViewBackButtonVisibility.Visible;
                SystemNavigationManager.GetForCurrentView().BackRequested += WeatherDisplay_BackRequested;
            }
            else
            {
                // Remove the UI from the title bar if in-app back stack is empty.
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                    AppViewBackButtonVisibility.Collapsed;
            }
        }

        private void WeatherDisplay_BackRequested(object sender, BackRequestedEventArgs e)
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

        //geoloc setup
        private async void setupGeoLocation()
        {
            // ask for permission 
            var accessStatus = await Geolocator.RequestAccessAsync();
            switch (accessStatus)
            {
                case GeolocationAccessStatus.Allowed:
                    {
                        MessageDialog myMsg = new MessageDialog(" GeoLocation in use.");
                        await myMsg.ShowAsync();


                        break;
                    }
                case GeolocationAccessStatus.Denied:
                    {
                        MessageDialog myMsg = new MessageDialog("Please turn on GeoLocation Data.");
                        await myMsg.ShowAsync();
                        break;
                    }
                default:
                    {
                        MessageDialog myMsg = new MessageDialog("Please check your Connection.");
                        await myMsg.ShowAsync();
                        break;
                    }
            }
        }


        

        private async void btnSaveWeather_Tapped(object sender, TappedRoutedEventArgs e)
        {
            StorageFolder weatherFolder = ApplicationData.Current.LocalFolder;

            StorageFile weatherFile;
            string fileText = "";

            try
            {
                weatherFile = await weatherFolder.GetFileAsync("weather.txt");
                fileText = await Windows.Storage.FileIO.ReadTextAsync(weatherFile);
            }
            catch (Exception E)
            {
                string message = E.Message;
                weatherFile = await weatherFolder.CreateFileAsync("weather.txt");
            }

            string resultOutput;

            resultOutput = tblLocation.Text.ToString();

            await Windows.Storage.FileIO.WriteTextAsync(weatherFile, fileText + resultOutput + System.Environment.NewLine + System.Environment.NewLine);

            MessageDialog clearDialog = new MessageDialog("Weather + Location Saved");
            await clearDialog.ShowAsync();

            btnSaveWeather.Visibility = Visibility.Collapsed;

        }

        private async void getWeather_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //var position = await LocationManager.GetPosition();
            var position = await LocationManager.GetPosition();

            WeatherModel.RootObject myWeather = await WeatherModel.GetWeather(position.Coordinate.Latitude, position.Coordinate.Longitude);

            tblLocation.Text = "Location:   " + myWeather.name + System.Environment.NewLine +
             "Temperature:  " + ((int)myWeather.main.temp).ToString() + "°C" + System.Environment.NewLine +
             "Description:  " + myWeather.weather[0].description + System.Environment.NewLine +
             "Latitude: " + myWeather.coord.lat + "  Longtitude: " + myWeather.coord.lon + System.Environment.NewLine +
             "Wind Speed:   " + myWeather.wind.speed + " m/s" + System.Environment.NewLine +
            "Pressure:  " + myWeather.main.pressure + "hpa" + System.Environment.NewLine +
            "Humidity:  " + myWeather.main.humidity + "%" + System.Environment.NewLine +
            "Country:   " + myWeather.sys.country + System.Environment.NewLine +
            "Temp Min:  " + myWeather.main.temp_min + "°C" + " Temp Max: " + myWeather.main.temp_max + "°C" + System.Environment.NewLine +
            "Time:    " + position.Coordinate.Timestamp.ToString();

            //tblLocation.Text = "Location:   " + myWeather.name;

            getWeather.Visibility = Visibility.Collapsed;
            tblWeather.Visibility = Visibility.Visible;
        }






        //setup more exact location settings

        private void SetupGeo()
        {
            geolocator = new Geolocator();
            geolocator.DesiredAccuracy = PositionAccuracy.High;


        }

        private async void getDetail(Geoposition pos)
        {
            //locator setup
            BasicGeoposition location = new BasicGeoposition();
            location.Latitude = pos.Coordinate.Latitude;
            location.Longitude = pos.Coordinate.Longitude;
            Geopoint pointToReverseGeocode = new Geopoint(location);

            //return result of map location query
            MapLocationFinderResult result =
                  await MapLocationFinder.FindLocationsAtAsync(pointToReverseGeocode);
        
            if (result.Status == MapLocationFinderStatus.Success)
            {
                //simply getting whatever we need from locator
                tblLocation2.Text += "Country:    " + result.Locations[0].Address.Country.ToString() + System.Environment.NewLine +
                    "City:    " + result.Locations[0].Address.Town.ToString() + System.Environment.NewLine +
                    "District:  " + result.Locations[0].Address.District.ToString() + System.Environment.NewLine +
                    "Street:  " + result.Locations[0].Address.Street.ToString() + System.Environment.NewLine +
                    
                    "Time:    " + pos.Coordinate.Timestamp.ToString() + System.Environment.NewLine +
                    "Longitude: " + pos.Coordinate.Latitude.ToString() + System.Environment.NewLine +
                    "Latitude:  " + pos.Coordinate.Longitude.ToString() + System.Environment.NewLine + System.Environment.NewLine +
                    "Formatted Address:  " + result.Locations[0].Address.FormattedAddress.ToString() ;
            }
        }





        private async void btnGetPos_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Geoposition pos = await geolocator.GetGeopositionAsync();
            getDetail(pos);

            //hide button after getting data
            btnGetPos.Visibility = Visibility.Collapsed;
            //show tblExactLocation after btnGetPos gets clicked/tapped
            tblExactLocation.Visibility = Visibility.Visible;
        }
    }
}
