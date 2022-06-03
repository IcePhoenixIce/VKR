using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VKR
{
	public enum WorkerType
	{
		Worker,
		Watcher,
		Admin,
		NoAuth
	}

	public partial class App : Application
	{
		static VKR.Data.DataBase dataBase;

		public static VKR.Data.DataBase DataBase 
		{
			get 
			{
				if (dataBase == null) 
				{
					dataBase = new Data.DataBase();
				}
				return dataBase;
			}
		}

		public App()
		{
			InitializeComponent();
			if (Current.Properties.ContainsKey("login") && Current.Properties.ContainsKey("password")) 
			{
				if (DataBase.autorization((string)Current.Properties["login"], (string)Current.Properties["password"])) 
				{
					DataBase.AddAllGeofensingAsync();
					MainPage = new VKR.AppShell();
					return;
				}
			}
			MainPage = new VKR.Views.Login();
		}

		protected override void OnStart()
		{
			if (Current.Properties.ContainsKey("login") && Current.Properties.ContainsKey("password"))
			{
				if (DataBase.autorization((string)Current.Properties["login"], (string)Current.Properties["password"]))
				{
					DataBase.AddAllGeofensingAsync();
					return;
				}
			}
		}

		protected override void OnSleep()
		{
		}

		protected override void OnResume()
		{
		}
	}
}
