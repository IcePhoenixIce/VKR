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
				Text = "Выйти",
				IconImageSource = "icon.png"
			};
			logoutmenu.Clicked += Logout_Clikced;

			MenuItem spacemenu = new MenuItem()
			{
				Text = "",
				IconImageSource = ""
			};

			if (App.DataBase.wtype == WorkerType.Admin) 
			{
				ShellSection shellSection = new ShellSection();
				ShellSection shellSection1 = new ShellSection();
				ShellSection shellSection2 = new ShellSection();
				ShellSection shellSection3 = new ShellSection();

				shellSection.Title = "Группы админа";
				shellSection1.Title = "Посещения для смотрителя";
				shellSection2.Title = "Карта для смотрителя";
				shellSection3.Title = "Карта для смотрителя";

				shellSection.Items.Add(new ShellContent()
				{
					Content = new VKR.Views.Admin.Groups(),
					Route = nameof(Views.Admin.Groups)
				});
				shellSection1.Items.Add(new ShellContent()
				{
					Content = new VKR.Views.Watcher.AllData(),
					Route = nameof(Views.Watcher.AllData)
				});
				myshell.Items.Add(shellSection);
				myshell.Items.Add(shellSection3);
				myshell.Items.Add(spacemenu);
			}

			if(App.DataBase.wtype == WorkerType.Admin || App.DataBase.wtype == WorkerType.Watcher) 
			{
				ShellSection shellSection = new ShellSection();
				ShellSection shellSection1 = new ShellSection();
				shellSection.Title = "Посещения для смотрителя";
				shellSection1.Title = "Карта для смотрителя";

				shellSection.Items.Add(new ShellContent()
				{
					Content = new VKR.Views.Watcher.AllData(),
					Route = nameof(Views.Watcher.AllData)
				});
				shellSection1.Items.Add(new ShellContent()
				{
					Content = new VKR.Views.Watcher.MapPageGroup(),
					Route = nameof(Views.Watcher.MapPageGroup)
				});
				myshell.Items.Add(shellSection);
				myshell.Items.Add(shellSection1);
				myshell.Items.Add(spacemenu);
			}

			if (App.DataBase.wtype != WorkerType.NoAuth)
			{
				ShellSection shellSection = new ShellSection();
				ShellSection shellSection1 = new ShellSection();
				ShellSection shellSection2 = new ShellSection();
				shellSection.Title = "Расписание работника";
				shellSection1.Title = "Карта работника";
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