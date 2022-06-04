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

namespace VKR.Views.Watcher
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MapPageGroup : ContentPage
	{
		public MapPageGroup()
		{
			LG = App.DataBase.GetAdmin_Group_Data();
			InitializeComponent();
			if (Application.Current.Properties.ContainsKey("currentMapPosition2"))
				MyMap.MoveToRegion(JsonConvert.DeserializeObject<MapSpan>((string)Application.Current.Properties["currentMapPosition2"]));
			this.BindingContext = this;
		}

		private void Picker_SelectedIndexChanged(object sender, EventArgs e)
		{
			MyMap.Pins.Clear();
			MyMap.MapElements.Clear();
			SelectedGroup.LM = App.DataBase.GetMarkers(SelectedGroup.GroupId);
			SelectedGroup.LE = App.DataBase.GetAdminWorkers(SelectedGroup.GroupId);
			foreach (Marker marker in SelectedGroup.LM)
			{
				MyMap.Pins.Add(marker.pin);
				MyMap.MapElements.Add(marker.circle);
			}
			foreach (Employee employee in SelectedGroup.LE)
			{
				if (employee.location != null)
				{
					Pin pin = new Pin();
					pin.Position = new Position(employee.location.Latitude, employee.location.Longitude);
					pin.Label = employee.FIO;
					pin.Address = employee.location.Timestamp.ToLocalTime().ToString();
					MyMap.Pins.Add(pin);
				}
			}
		}

		private void Map_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (MyMap.VisibleRegion != null)
				Application.Current.Properties["currentMapPosition2"] = JsonConvert.SerializeObject(new MapSpan(MyMap.VisibleRegion.Center, MyMap.VisibleRegion.LatitudeDegrees, MyMap.VisibleRegion.LongitudeDegrees));
		}

		public Group SelectedGroup { get { return _selectedGroup; } set { _selectedGroup = value; } }

		static Group _selectedGroup { get; set; }

		public IList<Group> LG { get; set; }
	}
}