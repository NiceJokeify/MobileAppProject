using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace WeatherUp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        //Navigation
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Remove the UI from the title bar if in-app back stack is empty.
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                AppViewBackButtonVisibility.Collapsed;
            base.OnNavigatedTo(e);
        }

        //navigate to display weather
        private void imgWeather_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(WeatherDisplay));
        }

        //navigate to display saved weather/location
        private void imgSavedWeather_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(StorageDisplay));
        }
    }
}
