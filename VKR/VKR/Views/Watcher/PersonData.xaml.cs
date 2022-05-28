using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VKR.Models.Watcher;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace VKR.Views.Watcher
{
	public partial class PersonData : ContentPage, INotifyPropertyChanged
	{
        protected override void OnAppearing()
        {
            base.OnAppearing();
			this.BindingContext = aboutWorker;
		}

		public event PropertyChangedEventHandler PropertyChanged;
		private WatchWorker AboutWorker
		{
			get; set;	
		}
		public WatchWorker aboutWorker {
			get 
			{
				return AboutWorker;
			} 
			set 
			{
				AboutWorker = value;
				OnPropertyChanged("aboutWorker");
				OnPropertyChanged("FIO");
				OnPropertyChanged("Position");
				OnPropertyChanged("WorkGroup");
				OnPropertyChanged("NumberOfAbsence");
				OnPropertyChanged("SumTimeOfAbsence");
			}
		}

		public PersonData(WatchWorker aw)
		{
			aboutWorker = aw;
			InitializeComponent();
			collView.SetBinding(ItemsView.ItemsSourceProperty, nameof(aboutWorker.skip_list));
		}

        private async void CollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
			var ans = await DisplayAlert("Удаление записи", "Вы уверены, что данный пропуск был совершен на законных причинах", "Да", "Нет");
			if (ans == true)
            {
				if (e.CurrentSelection != null)
				{
					Worker_skip ws = e.CurrentSelection.FirstOrDefault() as Worker_skip;
					if (App.DataBase.DeleteWTS(ws.skip_id)) 
					{
						aboutWorker.NumberOfAbsence--;
						TimeSpan ts = TimeSpan.Parse(aboutWorker.SumTimeOfAbsence);
						ts = ts.Subtract(TimeSpan.Parse(ws.skip_time));
						aboutWorker.SumTimeOfAbsence = ts.ToString();
						aboutWorker.skip_list.Remove(ws);
						OnPropertyChanged("aboutWorker");
					}
					else 
						await DisplayAlert("Удаление записи", "Не удалось подключится к БД для удаления пропуска", "Ок");
				}
			}
		}

		protected void OnPropertyChanged(string propName)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propName));
		}
	}
}