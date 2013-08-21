using Mvvm.ViewModels;
using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;

namespace CWBWeather.ViewModel
{
    class SettingViewModel : ViewModelBase
    {
        public SettingViewModel()
        {
        }

        private bool _enableGPS;

        public bool EnableGPS
        {
            get
            {
                return _enableGPS;
            }
            set
            {
                if (_enableGPS != value)
                {
                    _enableGPS = value;

                    IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
                    settings["enableGPS"] = _enableGPS;
                    settings.Save();
                }

                OnPropertyChanged("EnableGPS");
            }
        }
    }
}
