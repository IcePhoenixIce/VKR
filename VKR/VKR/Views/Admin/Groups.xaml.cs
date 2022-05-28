using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VKR.Views.Admin
{
	public partial class Groups : ContentPage, INotifyPropertyChanged
	{
		public ObservableCollection<VKR.Models.Admin.Group> LG { get; private set; }

		public Groups()
		{
			InitializeComponent();
			this.OnAppearing();
			BindingContext = this;
			Routing.RegisterRoute("GroupCreate", typeof(VKR.Views.Admin.GroupCreate));
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
			this.LG = App.DataBase.GetAdmin_Group_Data();
			OnPropertyChanged("LG");
		}

		private async void AddToolbar_Clicked(object sender, EventArgs e)
		{
			VKR.Models.Admin.Group g = new VKR.Models.Admin.Group();
			await Navigation.PushAsync(new GroupCreate(ref g));
		}

		private async void collview_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (e.CurrentSelection != null)
			{
				VKR.Models.Admin.Group ww = e.CurrentSelection.FirstOrDefault() as VKR.Models.Admin.Group;
				//Затычка на перевод в Лист со всеми редакциями!!!
				await Navigation.PushAsync(new Group(ref ww));
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged(string propName)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propName));
		}
	}
}