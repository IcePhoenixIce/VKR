using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Newtonsoft.Json;

using VKR.Models.Watcher;
using VKR.Data;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace VKR.Views.Watcher
{
	public partial class AllData : ContentPage, INotifyPropertyChanged
	{
		public ObservableCollection<WatchWorker> Lww { get; set; }
		public event PropertyChangedEventHandler PropertyChanged;
		private async void OnCollectionViewSelectionChanged(object sender, SelectionChangedEventArgs e) 
		{
			if (e.CurrentSelection != null) 
			{
				WatchWorker ww = e.CurrentSelection.FirstOrDefault() as WatchWorker;
				/*string jason = await Task.Run(() => JsonConvert.SerializeObject(new AboutWorker(ww.id, ww.FIO, ww.Position, ww.WorkGroup, ww.NumberOfAbsence, TimeSpan.Parse(ww.SumTimeOfAbsence))));*/
				await Navigation.PushAsync(new PersonData(ww));
			}
		}

		protected override void OnAppearing()
		{

			base.OnAppearing();
			Lww = App.DataBase.GetWatcher_Data();
			OnPropertyChanged("Lww");
		}

		public AllData()
		{
			InitializeComponent();
			collview.SetBinding(ItemsView.ItemsSourceProperty, nameof(Lww), BindingMode.TwoWay);
			this.OnAppearing();
			BindingContext = this;
		}

		protected void OnPropertyChanged(string propName)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propName));
		}
	}
}