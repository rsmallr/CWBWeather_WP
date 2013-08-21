using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using CWBWeather.ViewModel;
using System.IO.IsolatedStorage;

namespace CWBWeather.View
{
    public partial class SettingPage : PhoneApplicationPage
    {
        private SettingViewModel mSettingViewModel;

        public SettingPage()
        {
            InitializeComponent();

            ThemeManager.ToLightTheme();

            mSettingViewModel = new SettingViewModel();
            LayoutRoot.DataContext = mSettingViewModel;

            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs args)
        {
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;

            if (settings.Contains("enableGPS"))
            {
                bool enableGPS = (bool)settings["enableGPS"];
                mSettingViewModel.EnableGPS = enableGPS;

                ToggleGPS.IsChecked = enableGPS;
                ToggleGPS.Checked += ToggleGPS_Checked;
                ToggleGPS.Unchecked += ToggleGPS_Unchecked;
            }
        }

        private void ToggleGPS_Checked(object sender, RoutedEventArgs e)
        {
            mSettingViewModel.EnableGPS = true;
        }

        private void ToggleGPS_Unchecked(object sender, RoutedEventArgs e)
        {
            mSettingViewModel.EnableGPS = false;
        }
    }
}