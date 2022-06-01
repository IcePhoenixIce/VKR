using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VKR.Models.Admin;
using VKR.Models.Watcher;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VKR
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AppShell : Shell
	{
		public Group group;
		public WatchWorker employee_info;

		public AppShell()
		{
			InitializeComponent();

			Routing.RegisterRoute("Login", typeof(VKR.Views.Login));

			MenuItem logoutmenu = new MenuItem()
			{
				Text = "Logout",
				IconImageSource = "icon.png"
			};
			logoutmenu.Clicked += Logout_Clikced;

			switch (App.DataBase.wtype)
			{
				case WorkerType.Worker:
					{
						ShellSection shellSection = new ShellSection();
						ShellSection shellSection1 = new ShellSection();
						ShellSection shellSection2 = new ShellSection();
						shellSection.Title = "Расписание";
						shellSection1.Title = "Карта";
						shellSection2.Title = "Информация о работнике";
						employee_info = App.DataBase.GetEmployee();
						group = new Models.Admin.Group(employee_info.wgi, employee_info.WorkGroup);
						shellSection.Items.Add(new ShellContent()
						{
							Content = new VKR.Views.Admin.TimeShedule(ref group),
							Route = nameof(Views.Worker.Shedule)
						});

						shellSection1.Items.Add(new ShellContent()
						{

							Content = new VKR.Views.Worker.Map(),
							Route = nameof(Views.Worker.Map)
						});

						shellSection2.Items.Add(new ShellContent()
						{
							//Заглушка на конструктор информации о пользователе
							Content = new VKR.Views.Watcher.PersonData(employee_info),
							Route = nameof(Views.Watcher.PersonData)
						});

						myshell.Items.Add(shellSection);
						myshell.Items.Add(shellSection1);
						myshell.Items.Add(shellSection2);
						myshell.Items.Add(logoutmenu);
						myshell.CurrentItem = shellSection2;
						break;
					}

				case WorkerType.Watcher:
					{
						ShellSection shellSection = new ShellSection();
						shellSection.Title = "Посещения";

						shellSection.Items.Add(new ShellContent()
						{
							Content = new VKR.Views.Watcher.AllData(),
							Route = nameof(Views.Watcher.AllData)
						});

						myshell.Items.Add(shellSection);
						myshell.Items.Add(logoutmenu);
						myshell.CurrentItem = shellSection;
						break;
					}

				case WorkerType.Admin:
					{
						ShellSection shellSection = new ShellSection();
						ShellSection shellSection3 = new ShellSection();

						shellSection.Title = "Группы";
						shellSection3.Title = "Посещения";
						shellSection.Items.Add(new ShellContent()
						{
							Content = new VKR.Views.Admin.Groups(),
							Route = nameof(Views.Admin.Groups)
						});
						shellSection3.Items.Add(new ShellContent()
						{
							Content = new VKR.Views.Watcher.AllData(),
							Route = nameof(Views.Watcher.AllData)
						});
						myshell.Items.Add(shellSection);
						myshell.Items.Add(shellSection3);
						myshell.Items.Add(logoutmenu);
						myshell.CurrentItem = shellSection;
						break;
					}
			}
		}

		private async void Logout_Clikced(object sender, EventArgs e)
		{
			if (App.DataBase.closeConnection()) 
			{
				await Shell.Current.GoToAsync("Login");
				Shell.Current.FlyoutIsPresented = false;
			}
			else
				await DisplayAlert("Ошибка!","Вы не можете отключится от БД прямо сейчас!","Ok");
		}
	}
}