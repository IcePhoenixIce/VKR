using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace VKR.Models.Worker
{
    class GeoTimer: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private Location _location;
        public Location location { get { return _location; } set { this._location = value; OnPropertyChanged("location"); } }
        private bool timerWork;

        public GeoTimer()
        {
            
        }

        public void StartTimer() 
        {
            timerWork = true;
            Xamarin.Forms.Device.StartTimer(TimeSpan.FromMinutes(1), () =>
            {
                if (App.DataBase.inWorkTimeBool)
                {
                    Task.Run(async () =>
                    {
                        var request = new GeolocationRequest(GeolocationAccuracy.Best);
                        location = await Geolocation.GetLocationAsync(request);
                        if (location != null)
                            App.DataBase.addPosition(location);
                    });
                }
                return timerWork;
            });
        }

        public void StopTimer() 
        {
            timerWork = false;
        }

        protected void OnPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
    }
}
