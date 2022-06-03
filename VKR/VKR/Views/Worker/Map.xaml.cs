using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VKR.Views.Worker
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Map : ContentPage, INotifyPropertyChanged
    {
        public int counter { get { return _counter; } set { _counter = value; OnPropertyChanged("counter"); } }
        int _counter;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
        public Map()
        {
            InitializeComponent();
            Xamarin.Forms.Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                counter++;
                return counter <= 1000;
            });
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            this.BindingContext = this;
        }
    }
}