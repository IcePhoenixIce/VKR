using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;
using VKR.Models;
using VKR.Models.Admin;
using VKR.Views.Admin;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace VKR.ViewModels.Admin
{
	public enum ButtonClickedType
	{
		Add,
		Change,
		Delete,
		None
	}

	class MapPageViewModel
    {
		public MapPageViewModel() 
		{
			m = null;
			flag = false;
			buttonClickedType = ButtonClickedType.None;
			map.MoveToLastRegionOnLayoutChange
				= map.IsShowingUser
				= map.HasZoomEnabled
				= map.HasScrollEnabled
				= true;
			if (group.LE == null || group.LE.Count == 0)
				App.DataBase.GetAdminWorkers(group.GroupId);
			map.MapClicked += Map_MapClicked;
			map.PropertyChanged += Map_PropertyChanged;
			if (Application.Current.Properties.ContainsKey("currentMapPosition"))
				map.MoveToRegion(JsonConvert.DeserializeObject<MapSpan>((string)Application.Current.Properties["currentMapPosition"]));
			mlist = App.DataBase.GetMarkers(group.GroupId);
			foreach (Marker marker in mlist) 
			{
				marker.pin.MarkerClicked += Pin_MarkerClicked;
				map.Pins.Add(marker.pin);
				map.MapElements.Add(marker.circle);
			}

			this.AddPin = new Command(async () =>
			{
				m = null;
				buttonClickedType = ButtonClickedType.Add;
				await Application.Current.MainPage.Navigation.PushAsync(new MarkerCreate());
			});

			this.ChangePin = new Command(() => { buttonClickedType = ButtonClickedType.Change; });
			this.DeletePin = new Command(() => { buttonClickedType = ButtonClickedType.Delete; });
		}

		public async void Pin_MarkerClicked(object sender, PinClickedEventArgs e) 
		{
			Pin pin = (Pin)sender;
			switch (buttonClickedType) 
			{
				case ButtonClickedType.Change:
					{
						foreach (Marker marker in mlist)
						{
							if (marker.pin == pin)
							{
								m = marker;
								break;
							}
						}
						await Application.Current.MainPage.Navigation.PushAsync(new MarkerCreate());
					}
					break;
				case ButtonClickedType.Delete:
					{
						foreach (Marker marker in mlist)
						{
							if (marker.pin == pin)
							{
								m = marker;
								break;
							}
						}
						App.DataBase.DeleteMarker(m);
						map.Pins.Remove(MapPageViewModel.m.pin);
						map.MapElements.Remove(MapPageViewModel.m.circle);
						mlist.Remove(m);
						buttonClickedType = ButtonClickedType.None;
						m = null;
					}
					break;
			}
		}

		private void Map_PropertyChanged(object sender, PropertyChangedEventArgs e) 
		{
			if (map.VisibleRegion != null)
				Application.Current.Properties["currentMapPosition"] = JsonConvert.SerializeObject(new MapSpan(map.VisibleRegion.Center, map.VisibleRegion.LatitudeDegrees, map.VisibleRegion.LongitudeDegrees));
		}

		private async void Map_MapClicked(object sender, MapClickedEventArgs e) 
		{
			if (flag) 
			{
				m.setPosition(e.Position);
				m.pin.MarkerClicked += Pin_MarkerClicked;
				map.Pins.Add(m.pin);
				map.MapElements.Add(m.circle);
				App.DataBase.SaveMarker(m, group.GroupId);
				mlist = App.DataBase.GetMarkers(group.GroupId);
				//заглушка
				//создание региона
				m = null;
				buttonClickedType = ButtonClickedType.None;
				flag = false;
			}
		}

		public ICommand AddPin { get; }
		public ICommand ChangePin { get; }
		public ICommand DeletePin { get; }

		public static Marker m;
		public static Models.Admin.Group group { get; set; }
		public static Map map { get; set; }
		public ObservableCollection<Marker> mlist { get; set; }
		public static ButtonClickedType buttonClickedType { get; set; }
		public static bool flag { get; set; }
	}
}
