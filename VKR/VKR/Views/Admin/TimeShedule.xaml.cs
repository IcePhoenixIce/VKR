using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using VKR.Models.Admin;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VKR.Views.Admin
{
	public partial class TimeShedule : ContentPage, INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged(string propName)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propName));
		}

		private VKR.Models.Admin.Group _group
		{
			get; set;
		}

		public VKR.Models.Admin.Group group 
		{
			get 
			{
				return _group;
			}
			set 
			{
				_group = value;
				OnPropertyChanged("LS");
				OnPropertyChanged("group");
			}
		}

		public TimeShedule(ref VKR.Models.Admin.Group group)
		{
			group.LS = App.DataBase.GetShedules(group.GroupId);
			this.group = group;
			InitializeComponent();
			collView.SetBinding(ItemsView.ItemsSourceProperty, nameof(this.group.LS));
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
			group.LS = App.DataBase.GetShedules(group.GroupId);
			this.BindingContext = group;
		}

		private async void AddToolbar_Clicked(object sender, EventArgs e)
		{
			VKR.Models.Admin.Shedule s = new VKR.Models.Admin.Shedule();
			await Navigation.PushAsync(new TimeCreate(ref s, group.GroupId));
		}

		private async void CollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (e.CurrentSelection != null)
			{
				VKR.Models.Admin.Shedule shedule = e.CurrentSelection.FirstOrDefault() as VKR.Models.Admin.Shedule;
				await Navigation.PushAsync(new TimeCreate(ref shedule, group.GroupId));
			}
		}
	}
}