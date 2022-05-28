using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VKR.Views.Admin
{
    public partial class Employeers : ContentPage, INotifyPropertyChanged
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
				OnPropertyChanged("LE");
				OnPropertyChanged("group");
			}
		}

		public Employeers(ref VKR.Models.Admin.Group group)
		{
			group.LE = App.DataBase.GetAdminWorkers(group.GroupId);
			this.group = group;
			InitializeComponent();
			collView.SetBinding(ItemsView.ItemsSourceProperty, nameof(this.group.LE));
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
			group.LE = App.DataBase.GetAdminWorkers(group.GroupId);
			this.BindingContext = group;
		}

		private async void AddToolbar_Clicked(object sender, EventArgs e)
		{
			VKR.Models.Admin.Employee s = new VKR.Models.Admin.Employee();
			await Navigation.PushAsync(new EmployeerCreate(ref s, group.GroupId));
		}

		private async void CollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (e.CurrentSelection != null)
			{
				VKR.Models.Admin.Employee employee = e.CurrentSelection.FirstOrDefault() as VKR.Models.Admin.Employee;
				await Navigation.PushAsync(new EmployeerCreate(ref employee, group.GroupId));
			}
		}
	}
}