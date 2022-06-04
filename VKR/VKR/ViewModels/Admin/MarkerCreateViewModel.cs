using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using VKR.Models;
using Xamarin.Forms;

namespace VKR.ViewModels.Admin
{
	public class MarkerCreateViewModel : INotifyPropertyChanged
	{
		public Marker marker;
		public MarkerCreateViewModel()
		{
			marker = (MapPageViewModel.m == null) ? new Marker() : MapPageViewModel.m;
			this.CreateGeofence = new Command(async () => 
			{
				if (string.IsNullOrEmpty(MTitle)) 
				{
					await App.Current.MainPage.DisplayAlert("Нет данных!", "Необходимо заполнит название рабочей области!", "Ок!");
					return;
				}
				if (string.IsNullOrEmpty(MText)) 
				{
					await App.Current.MainPage.DisplayAlert("Нет данных!", "Необходимо заполнит примечания для рабочей области!", "Ок!");
					return;
				}
				if (MRadius<100)
				{
					await App.Current.MainPage.DisplayAlert("Некорректные данные!", "Радиус  не должен быть меньше 100", "Ок!");
					return;
				} 
				if (MapPageViewModel.buttonClickedType == ButtonClickedType.Change) 
				{
					MapPageViewModel.map.Pins.Remove(MapPageViewModel.m.pin);
					MapPageViewModel.map.MapElements.Remove(MapPageViewModel.m.circle);
					//Удаление геозоны
					if (!movemarker) 
					{
						MapPageViewModel.map.Pins.Add(marker.pin);
						MapPageViewModel.map.MapElements.Add(marker.circle);
						App.DataBase.SaveMarker(marker, MapPageViewModel.group.GroupId);
						MapPageViewModel.buttonClickedType = ButtonClickedType.None;
						//создание геозоны
						await App.Current.MainPage.Navigation.PopAsync();
						return;
					}
				}
				MapPageViewModel.m = marker;
				MapPageViewModel.flag = true;
				await App.Current.MainPage.Navigation.PopAsync();
			}
			);
			this.GoBack = new Command(async () => 
			{
				MapPageViewModel.buttonClickedType = ButtonClickedType.None;
				MapPageViewModel.m = null;				
				await App.Current.MainPage.Navigation.PopAsync(); 
			});
		}


		public ICommand CreateGeofence { get; }
		public ICommand GoBack { get; }

		public string MTitle
		{
			get { return marker.pin.Label; }
			set { marker.pin.Label = value; OnPropertyChanged("MTitle"); }
		}

		public string MText
		{
			get { return marker.pin.Address; }
			set { marker.pin.Address = value; OnPropertyChanged("MText"); }
		}

		public int MRadius
		{
			get { return (int)marker.circle.Radius.Meters; }
			set { marker.circle.Radius = new Xamarin.Forms.Maps.Distance(value); OnPropertyChanged("MRadius"); }
		}

		bool movemarker;
		public bool MoveMarker
		{
			get { return movemarker; }
			set { movemarker = value; OnPropertyChanged("MoveMarker"); }
		}

		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged(string propName)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propName));
		}
	}
}