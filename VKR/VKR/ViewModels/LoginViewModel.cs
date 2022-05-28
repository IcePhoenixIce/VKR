using System;
using System.Collections.Generic;
using System.Text;

using System.ComponentModel;
using VKR.ViewModels;
using System.Windows.Input;
using Xamarin.Forms;

namespace VKR.ViewModels
{
	public class LoginViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		string login;
		public string Login 
		{
			get { return login; }
			set 
			{
				if (login != value)
				{
					login = value;
					OnPropertyChanged("Login");
				}
			}

		}

		string password;
		public string Password
		{
			get { return password; }
			set
			{
				if (password != value)
				{
					password = value;
					OnPropertyChanged("Password");
				}
			}

		}

		public ICommand LoginCommand { protected set; get; }

		public LoginViewModel() 
		{
			LoginCommand = new Command(auth);
		}

		private async void auth() 
		{
			App.DataBase.closeConnection();
			if (this.Login != null && this.Password != null)
				if (App.DataBase.autorization(this.Login, this.Password))
					App.Current.MainPage = new AppShell();
				else
					await Application.Current.MainPage.DisplayAlert("Ошибка!", "Проверьте соединение и правильность введеных данных", "Ok");
			else
				await Application.Current.MainPage.DisplayAlert("Ошибка!", "Не все поля заполнены!", "Ok");
		}

		protected void OnPropertyChanged(string propName)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propName));
		}
	}
}
