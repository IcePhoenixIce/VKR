using System;
using System.Collections.Generic;
using System.Text;
using MySqlConnector;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using System.ComponentModel;

namespace VKR.Models.Watcher
{
	public class Worker_skip
	{
		public int marker_id { get; set; }
		public int skip_id { get; set; }
		public string date { get; set; }
		public string skip_time { get; set; }
		public string start_time { get; set; }
		public string end_time { get; set; }

		[JsonConstructor]
		public Worker_skip(int skip_id, string date, TimeSpan skip_time, TimeSpan start_time, TimeSpan end_time, int marker_id = 0)
		{
			this.skip_id = skip_id;
			this.date = date;
			this.skip_time = skip_time.ToString();
			this.start_time = start_time.ToString();
			this.end_time = end_time.ToString();
			this.marker_id = marker_id;
		}
	}

	public class WatchWorker : INotifyPropertyChanged
	{
		[JsonConstructor]
		public WatchWorker(int id, string fio, string position, int wgi, string workGroup, int numberOfAbsence, TimeSpan sumTime) 
		{
			this.id = id;
			this.FIO = fio;
			this.Position = position;
			this.wgi = wgi;
			this.WorkGroup = workGroup;
			this.NumberOfAbsence = numberOfAbsence;
			this.SumTimeOfAbsence = sumTime.ToString();
			//skip_list = App.DataBase.GetWatcher_Data_Person(this.id);
			dlt_rows = Xamarin.Forms.SelectionMode.Single;
		}

		public WatchWorker(int id) 
		{
			string query = $"SELECT * FROM watcher_all WHERE emp_id = {id}";

			if (App.DataBase.connection.State == System.Data.ConnectionState.Open)
			{
				MySqlCommand cmd = new MySqlCommand(query, App.DataBase.connection);
				MySqlDataReader dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					FIO = dr["ФИО"] as string;
					Position = dr["Должность"] as string;
					wgi = (int)dr["work_group_id"];
					WorkGroup = dr["Название подразделения"] as string;
					NumberOfAbsence = (int)dr["Количество пропусков"];
					SumTimeOfAbsence = ((TimeSpan)dr["Время пропусков"]).ToString();
				}
				dr.Close();
			}
			skip_list = App.DataBase.GetWatcher_Data_Person(this.id);
			dlt_rows = Xamarin.Forms.SelectionMode.None;
		}

		public ObservableCollection<Worker_skip> skip_list { get { return _skip_list; } set { _skip_list = value; OnPropertyChanged("skip_list"); } }
		public Xamarin.Forms.SelectionMode dlt_rows { get; private set; }
		public int id { get { return _id; } set { _id = value; OnPropertyChanged("id"); } }
		public string FIO { get { return _FIO; } set { _FIO = value; OnPropertyChanged("FIO"); } }
		public string Position { get { return _Position; } set { _Position = value; OnPropertyChanged("Position"); } }
		public string WorkGroup { get { return _WorkGroup; } set { _WorkGroup = value; OnPropertyChanged("WorkGroup"); } }
		public int NumberOfAbsence { get { return _NumberOfAbsence; } set { _NumberOfAbsence = value; OnPropertyChanged("NumberOfAbsence"); } }
		public string SumTimeOfAbsence { get { return _SumTimeOfAbsence; } set { _SumTimeOfAbsence = value; OnPropertyChanged("SumTimeOfAbsence"); } }
		public int wgi { get { return _WGI; } set { _WGI = value; OnPropertyChanged("wgi"); } }

		private ObservableCollection<Worker_skip> _skip_list { get; set; }
		private Xamarin.Forms.SelectionMode _dlt_rows { get; set; }
		private int _id { get; set; }
		private string _FIO { get; set; }
		private string _Position { get; set; }
		private string _WorkGroup { get; set; }
		private int _NumberOfAbsence { get; set; }
		private string _SumTimeOfAbsence { get; set; }
		private int _WGI { get; set; }

		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged(string propName)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propName));
		}

	}
}
