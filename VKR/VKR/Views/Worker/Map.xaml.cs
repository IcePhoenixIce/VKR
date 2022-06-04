using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VKR.Models;
using VKR.Models.Admin;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace VKR.Views.Worker
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Map : ContentPage
    {
        
        public Map()
        {
            LM = App.DataBase.GetMarkers(App.DataBase.work_group);
            InitializeComponent();
            if (Application.Current.Properties.ContainsKey("currentMapPosition3"))
                MyMap.MoveToRegion(JsonConvert.DeserializeObject<MapSpan>((string)Application.Current.Properties["currentMapPosition3"]));
            foreach (Marker marker in LM)
            {
                MyMap.Pins.Add(marker.pin);
                MyMap.MapElements.Add(marker.circle);
            }
        }

        private void Map_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (MyMap.VisibleRegion != null)
                Application.Current.Properties["currentMapPosition3"] = JsonConvert.SerializeObject(new MapSpan(MyMap.VisibleRegion.Center, MyMap.VisibleRegion.LatitudeDegrees, MyMap.VisibleRegion.LongitudeDegrees));
        }

        public IList<Marker> LM { get; set; }
    }
}