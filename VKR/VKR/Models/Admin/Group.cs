using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;

namespace VKR.Models.Admin
{
	public enum WeekDay
	{
		Понедельник = 0,
		Вторник = 1,
		Среда = 2,
		Четверг = 3,
		Пятница = 4,
		Суббота = 5,
		Воскресенье = 6
	}

	public class Shedule
	{
		public int id;
		public WeekDay weekDay { get; set; }
		public TimeSpan time_start { get; set; }
		public string time_start_str { get { return time_start.ToString(); }}
		public TimeSpan time_end { get; set; }
		public string time_end_str { get { return time_end.ToString(); } }

		public Shedule(int id, int weekDay, TimeSpan time_start, TimeSpan time_end)
		{
			this.id = id;
			this.weekDay = (WeekDay)weekDay;
			this.time_start = time_start;
			this.time_end = time_end;
		}
		public Shedule() 
		{
			id = -1;
		}
	}

	public class Employee
	{
		public int id;
		public string FIO { get; set; }
		public string Position { get; set; }
		public string Login { get; set; }
		public string Password { get; set; }
		public WorkerType wt { get; set; }
		public string wt_str { get { return wt.ToString(); } }

		public Xamarin.Essentials.Location location { get; set; }
		public Employee(int id, string FIO, string Position, string wt, string location)
		{
			this.id = id;
			this.FIO = FIO;
			this.Position = Position;
			this.wt = (WorkerType)Enum.Parse(typeof(WorkerType), wt);
			if (location == "null" || location == "NULL")
				this.location = null;
			else
				this.location = Newtonsoft.Json.JsonConvert.DeserializeObject<Xamarin.Essentials.Location>(location);
		}
		public Employee()
		{
			id = -1;
		}
	}

	public class Group : INotifyPropertyChanged
	{
		public int GroupId { get; set; }
		public string NameOfGroup { get; set; }
		public ObservableCollection<Shedule> LS { get { return _LS; } set { _LS = value; OnPropertyChanged("LS"); } }
		private ObservableCollection<Shedule> _LS { get; set; }
		public ObservableCollection<Employee> LE { get { return _LE; } set { _LE = value; OnPropertyChanged("LE"); } }
		private ObservableCollection<Employee> _LE { get; set; }
		public ObservableCollection<Marker> LM { get { return _LM; } set { _LM = value; OnPropertyChanged("LM"); } }
		private ObservableCollection<Marker> _LM { get; set; }

		public Group() 
		{
			this.GroupId = -1;
		}

		public Group(int NumberOfGroup, string NameOfGroup) 
		{
			this.GroupId = NumberOfGroup;
			this.NameOfGroup = NameOfGroup;
		}

		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged(string propName)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propName));
		}
	}
}
