using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VKR.Views.Admin
{
	public partial class TimeCreate : ContentPage
	{
		VKR.Models.Admin.Shedule shedule;
		int GroupId;

		public TimeCreate(ref VKR.Models.Admin.Shedule shedule, int GroupId)
		{
			this.shedule = shedule;
			this.GroupId = GroupId;
			InitializeComponent();
			picker.SetBinding(ItemsView.ItemsSourceProperty, nameof(VKR.Models.Admin.WeekDay));
			if (shedule.id == -1)
				deleteb.IsEnabled = deleteb.IsVisible = false;
			else
				picker.SelectedIndex = ((int)this.shedule.weekDay);
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
			this.BindingContext = shedule;
		}

		private async void SaveButtonClicked(object sender, EventArgs e)
		{
			if (picker.SelectedIndex != -1)
			{
				shedule.weekDay = (Models.Admin.WeekDay)picker.SelectedIndex;
				App.DataBase.SaveShedule(shedule, GroupId);
				await Navigation.PopAsync();
			}
			else
				await DisplayAlert("Пустые поля!", "Заполните поля, прежде чем сохранять!", "Ок");
		}
		private async void DeleteButtonClicked(object sender, EventArgs e)
		{
			var ans = await DisplayAlert("Удаление расписания", "Удалить данное расписание?", "Да", "Нет");
			if (ans == true)
			{
				App.DataBase.DeleteShedule(shedule, GroupId);
				await Navigation.PopAsync();
			}
		}
	}
}