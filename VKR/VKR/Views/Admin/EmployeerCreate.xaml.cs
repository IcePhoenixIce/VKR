using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VKR.Views.Admin
{
    public partial class EmployeerCreate : ContentPage
    {
        VKR.Models.Admin.Employee emp;
        int GroupId;

        public EmployeerCreate(ref VKR.Models.Admin.Employee emp, int GroupId)
        {
            this.emp = emp;
            this.GroupId = GroupId;
            InitializeComponent();
            if (emp.id == -1)
            {
                deleteb.IsEnabled = deleteb.IsVisible = check.IsVisible = check.IsEnabled =  false;
                checkbox.IsChecked = picker.IsEnabled = picker.IsVisible = Login.IsEnabled = Login.IsVisible = Password.IsEnabled = Password.IsVisible = true;
            }
            else 
            {
                switch (emp.wt) 
                {
                    case WorkerType.Worker:
                        picker.SelectedIndex = 0;
                        break;
                    case WorkerType.Watcher:
                        picker.SelectedIndex = 1;
                        break;
                    case WorkerType.Admin:
                        picker.SelectedIndex = 2;
                        break;
                    default:
                        picker.SelectedIndex = -1;
                        break;
                }
                deleteb.IsEnabled = deleteb.IsVisible = check.IsVisible = check.IsEnabled = true;
                picker.IsEnabled = picker.IsVisible = Login.IsEnabled = Login.IsVisible = Password.IsEnabled = Password.IsVisible = false;
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            this.BindingContext = emp;
        }

        private async void SaveButtonClicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(emp.FIO) && !string.IsNullOrEmpty(emp.Position) && (!checkbox.IsChecked || (!string.IsNullOrEmpty(emp.Login) && !string.IsNullOrEmpty(emp.Password) && picker.SelectedIndex != -1)))
            {
                switch (picker.SelectedIndex) 
                {
                    case 0:
                        emp.wt = WorkerType.Worker;
                        break;
                    case 1:
                        emp.wt = WorkerType.Watcher;
                        break;
                    case 2:
                        emp.wt = WorkerType.Admin;
                        break;
                    default:
                        break;
                }
                App.DataBase.SaveEmployee(emp, GroupId);
                await Navigation.PopAsync();
            }
            else
                await DisplayAlert("Пустые поля!", "Заполните поля, прежде чем сохранять работника!", "Ок");
        }

        private async void DeleteButtonClicked(object sender, EventArgs e)
        {
            var ans = await DisplayAlert("Удаление пользователя", "Вы точно хотите удалить пользователя?", "Да", "Нет");
            if (ans == true)
            {
                App.DataBase.DeleteEmployee(emp);
                await Navigation.PopAsync();
            }
        }

        private void CheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            picker.IsEnabled = picker.IsVisible = Login.IsEnabled = Login.IsVisible = Password.IsEnabled = Password.IsVisible = ((CheckBox)sender).IsChecked;
        }
    }
}