using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VKR.Views.Admin
{
	public partial class GroupCreate : ContentPage
	{

		VKR.Models.Admin.Group group;
		public GroupCreate(ref VKR.Models.Admin.Group group)
		{
			InitializeComponent();
			this.BindingContext = group;
			this.group = group;
			if (group.GroupId == -1) 
			{
				dButton.IsVisible = dButton.IsEnabled = false;
			}
		}

		private async void OnSaveButtonClicked(object sender, EventArgs e)
		{

			if (!string.IsNullOrEmpty(group.NameOfGroup)) 
			{
				App.DataBase.SaveGroup(group.NameOfGroup, group.GroupId);
				await Navigation.PopAsync();
			}
			else
				await DisplayAlert("Пустые поля!", "Для добавление в базу данных новой группы необходимо заполнить все поля!", "Ок");

		}

		private async void OnDeleteButtonClicked(object sender, EventArgs e)
		{
			var ans = await DisplayAlert("Удаление группы", "Удалить группу? Все связанные с ней данные так же будут удалены!", "Да", "Нет");
			if (ans == true)
			{
				App.DataBase.DeleteGroup(group.GroupId);
				await Navigation.PopAsync();
			}
		}
	}
}