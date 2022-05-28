﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using MySqlConnector;
using Newtonsoft.Json;
using VKR.Models;
using VKR.Models.Admin;
using VKR.Models.Watcher;


namespace VKR.Data
{
	public class DataBase
	{
		public WorkerType wtype;
		public int emp_id;
		public MySqlConnection connection = new MySqlConnection();

		public DataBase() { wtype = WorkerType.NoAuth; }

		static string Hash(string input)
		{
			var hash = new SHA1Managed().ComputeHash(Encoding.UTF8.GetBytes(input));
			return string.Concat(hash.Select(b => b.ToString("x2")));
		}

		public bool standartConnection(string login="common_user", string password="") 
		{
			if (connection.State == System.Data.ConnectionState.Closed)
				try
				{
					connection = new MySqlConnection($"server=192.168.1.2;port=3306;username={login};password={password};database=vkrdb");
					connection.Open();
				}
				catch
				{
					return false;
				}
			return connection.State != System.Data.ConnectionState.Closed;
		}
		public bool closeConnection() 
		{
			if (connection.State != System.Data.ConnectionState.Closed) 
				connection.Close();
			wtype = WorkerType.NoAuth;
			return connection.State == System.Data.ConnectionState.Closed;
		}
		public bool autorization(string login, string password)
		{
			if (connection.State == System.Data.ConnectionState.Closed)
				if (!standartConnection())
					return false;
			MySqlCommand command = new MySqlCommand($"SELECT emp_id, mobile_status FROM auth_table WHERE login = '{login}' AND SHA1(SHA1(PASSWORD)) = '{Hash(Hash(password))}'", connection);
			MySqlDataReader dr = command.ExecuteReader();
			while (dr.Read())
			{
				emp_id = (int)dr["emp_id"];
				switch ((string)dr["mobile_status"])
				{
					case "Worker":
						closeConnection();
						standartConnection();
						wtype = WorkerType.Worker;
						break;
					case "Watcher":
						closeConnection();
						standartConnection("watcher", "mywok/MIvyPWH!)S");
						wtype = WorkerType.Watcher;
						break;
					case "Admin":
						closeConnection();
						standartConnection("root", "SqlAdmin");
						wtype = WorkerType.Admin;
						break;
					default:
						dr.Close();
						return false;
				}
				dr.Close();
				return true;
			}
			dr.Close();
			wtype = WorkerType.NoAuth;
			return false;
		}

		public ObservableCollection<WatchWorker> GetWatcher_Data() 
		{
			ObservableCollection<WatchWorker> lww = new ObservableCollection<WatchWorker>();
			string query = "SELECT * FROM watcher_all";

			if (this.connection.State == System.Data.ConnectionState.Open) 
			{
				MySqlCommand cmd = new MySqlCommand(query, this.connection);
				MySqlDataReader dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					lww.Add(new WatchWorker((int)dr["emp_id"], dr["ФИО"] as string, dr["Должность"] as string, dr["Название подразделения"] as string, (int)dr["Количество пропусков"], (TimeSpan)dr["Время пропусков"]));
				}
				dr.Close();
				foreach (WatchWorker ww in lww) 
				{
					ww.skip_list = GetWatcher_Data_Person(ww.id);
				}
			}
			else
				App.Current.MainPage.DisplayAlert("Ошибка подключения", "Невозможно установить соединение с БД, проверьте связь", "Ок");
			return lww;
		}
		public ObservableCollection<Worker_skip> GetWatcher_Data_Person(int id) 
		{
			ObservableCollection<Worker_skip> skip_list = new ObservableCollection<Worker_skip>();
			if (connection.State == System.Data.ConnectionState.Open)
			{
				string query = $"SELECT * FROM watcher_table_skip WHERE emp_id = {id}";
				MySqlCommand cmd = new MySqlCommand(query, App.DataBase.connection);
				MySqlDataReader dr = cmd.ExecuteReader();
				while (dr.Read())
					skip_list.Add(new Worker_skip((int)dr["id"], ((DateTime)dr["date"]).ToShortDateString(), (TimeSpan)dr["skip_time"], (TimeSpan)dr["start_time"], (TimeSpan)dr["end_time"], (DBNull.Value.Equals(dr["w_id"])) ? 0 : (int)dr["w_id"]));
				dr.Close();
			}
			else
				App.Current.MainPage.DisplayAlert("Ошибка подключения", "Невозможно установить соединение с БД, проверьте связь", "Ок");
			return skip_list;
		}
		public bool DeleteWTS(int skip_id) 
		{
			if (connection.State == System.Data.ConnectionState.Open)
			{
				string query = $"Delete FROM watcher_table_skip WHERE id = {skip_id}";
				MySqlCommand cmd = new MySqlCommand(query, connection);
				cmd.ExecuteNonQuery();
				return true;
			}
			return false;
		}

		public ObservableCollection<Group> GetAdmin_Group_Data() 
		{
			ObservableCollection<Group> Lg = new ObservableCollection<Group>();
			string query = "SELECT * FROM work_group_name";
			if (this.connection.State == System.Data.ConnectionState.Open)
			{
				MySqlCommand cmd = new MySqlCommand(query, this.connection);
				MySqlDataReader dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					Lg.Add(new Group((int)dr["work_group_id"], dr["group_name"] as string));
				}
				dr.Close();
			}
			else
				App.Current.MainPage.DisplayAlert("Ошибка подключения", "Невозможно установить соединение с БД, проверьте связь", "Ок");
			return Lg;

		}
		public bool SaveGroup(string groupName, int groupId = -1)
		{
			if (connection.State == System.Data.ConnectionState.Open)
			{
				string query = (groupId != - 1) ? $"UPDATE `work_group_name` SET `group_name` = '{groupName}' WHERE `work_group_id` = {groupId}" : $"INSERT INTO `work_group_name` (`work_group_id`, `group_name`) VALUES (NULL, '{groupName}')";
				MySqlCommand cmd = new MySqlCommand(query, connection);
				cmd.ExecuteNonQuery();
				return true;
			}
			return false;
		}
		public bool DeleteGroup(int groupId) 
		{
			if (connection.State == System.Data.ConnectionState.Open)
			{
				string query = $"Delete FROM work_group_name WHERE work_group_id = {groupId}";
				MySqlCommand cmd = new MySqlCommand(query, connection);
				cmd.ExecuteNonQuery();
				return true;
			}
			return false;
		}
		public ObservableCollection<Shedule> GetShedules(int groupId) 
		{
			ObservableCollection<Shedule> ls = new ObservableCollection<Shedule>();
			string query = $"SELECT * FROM time_table WHERE  work_group_id = {groupId}";
			if (this.connection.State == System.Data.ConnectionState.Open)
			{
				MySqlCommand cmd = new MySqlCommand(query, this.connection);
				MySqlDataReader dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					ls.Add(new Shedule((int)dr["id"], (int)dr["week_day"], (TimeSpan)dr["time_start"], (TimeSpan)dr["time_end"]));
				}
				dr.Close();
			}
			else
				App.Current.MainPage.DisplayAlert("Ошибка подключения", "Невозможно установить соединение с БД, проверьте связь", "Ок");
			return ls;
		}
		public bool SaveShedule(Shedule shedule, int groupId) 
		{
			if (connection.State == System.Data.ConnectionState.Open)
			{
				string query = (shedule.id != -1) ? $"UPDATE `time_table` SET `week_day` = '{(int)shedule.weekDay}', `time_start` = '{shedule.time_start_str}',  `time_end` = '{shedule.time_end_str}'  WHERE `id` = {shedule.id}"
					: $"INSERT INTO `time_table` VALUES (NULL, '{groupId}', '{(int)shedule.weekDay}','{shedule.time_start_str}','{shedule.time_end_str}')";
				MySqlCommand cmd = new MySqlCommand(query, connection);
				cmd.ExecuteNonQuery();
				query = $"DROP EVENT IF EXISTS `wg_{groupId}_{(int)shedule.weekDay}`; CREATE DEFINER =`root`@`%` EVENT `wg_{groupId}_{(int)shedule.weekDay}` ON SCHEDULE EVERY 1 WEEK STARTS '2022-04-0{3+((int)shedule.weekDay)} {shedule.time_end_str}' ON COMPLETION PRESERVE ENABLE DO CALL proc_skipt0(1, '{shedule.time_start_str}', '{shedule.time_end_str}')";
				cmd = new MySqlCommand(query, connection);
				cmd.ExecuteNonQuery();
				return true;
			}
			return false;
		}
		public bool DeleteShedule(Shedule shedule, int groupId) 
		{
			if (connection.State == System.Data.ConnectionState.Open)
			{
				string query = $"Delete FROM time_table WHERE id = {shedule.id}";
				MySqlCommand cmd = new MySqlCommand(query, connection);
				cmd.ExecuteNonQuery();
				query = $"DROP EVENT IF EXISTS `wg_{groupId}_{(int)shedule.weekDay}`;";
				cmd = new MySqlCommand(query, connection);
				cmd.ExecuteNonQuery();
				return true;
			}
			return false;
		}
		public ObservableCollection<Employee> GetAdminWorkers(int groupId)
		{
			ObservableCollection<Employee> le = new ObservableCollection<Employee>();
			string query = $"SELECT * FROM admin_employeers WHERE  work_group_id = {groupId}";
			if (this.connection.State == System.Data.ConnectionState.Open)
			{
				MySqlCommand cmd = new MySqlCommand(query, this.connection);
				MySqlDataReader dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					le.Add(new Employee((int)dr["emp_id"], (string)dr["emp_fio"], (string)dr["emp_pos"], (string)dr["mobile_status"]));
				}
				dr.Close();
			}
			else
				App.Current.MainPage.DisplayAlert("Ошибка подключения", "Невозможно установить соединение с БД, проверьте связь", "Ок");
			return le;
		}
		public bool SaveEmployee(Employee employee, int groupId)
		{
			if (connection.State == System.Data.ConnectionState.Open)
			{
				if (employee.id == -1)
				{
					string query = $"INSERT INTO `employee` VALUES (NULL, '{employee.FIO}', '{employee.Position}','{groupId}');";
					MySqlCommand cmd = new MySqlCommand(query, connection);
					cmd.ExecuteNonQuery();
					query = $"SELECT `emp_id` FROM `employee` WHERE emp_fio = '{employee.FIO}' AND emp_pos = '{employee.Position}' AND `work_group_id` = '{groupId}';";
					cmd = new MySqlCommand(query, connection);
					employee.id = (int)cmd.ExecuteScalar();
					query = $"INSERT INTO `auth_table` VALUES ('{employee.id}', '{employee.Login}', '{employee.Password}', '{employee.wt_str}');";
					cmd = new MySqlCommand(query, connection);
					cmd.ExecuteNonQuery();
				}
				else
				{
					string query = $"UPDATE `employee` SET `emp_fio` = '{employee.FIO}', `emp_pos` = '{employee.Position}'  WHERE `emp_id` = {employee.id};";
					if (!(string.IsNullOrEmpty(employee.Login) || string.IsNullOrEmpty(employee.Password)))
					{
						query += $" UPDATE `auth_table` SET `login` = '{employee.Login}', `password` = '{employee.Password}, `mobile_status` = '{employee.wt_str}' WHERE `emp_id` = {employee.id}';";
					}
					MySqlCommand cmd = new MySqlCommand(query, connection);
					cmd.ExecuteNonQuery();
				}
				return true;
			}
			return false;
		}
		public bool DeleteEmployee(Employee employee)
		{
			if (connection.State == System.Data.ConnectionState.Open)
			{
				string query = $"Delete FROM employee WHERE `emp_id` = '{employee.id}'";
				MySqlCommand cmd = new MySqlCommand(query, connection);
				cmd.ExecuteNonQuery();
				return true;
			}
			return false;
		}
		public ObservableCollection<Marker> GetMarkers(int groupId) 
		{
			ObservableCollection<Marker> lm = new ObservableCollection<Marker>();
			string query = $"SELECT * FROM marker_table WHERE  group_id = {groupId}";
			if (this.connection.State == System.Data.ConnectionState.Open)
			{
				MySqlCommand cmd = new MySqlCommand(query, this.connection);
				MySqlDataReader dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					Marker buff = JsonConvert.DeserializeObject<Marker>((string)dr["Marker"]);
					buff.uid = (int)dr["marker_id"];
					lm.Add(buff);
				}
				dr.Close();
			}
			else
				App.Current.MainPage.DisplayAlert("Ошибка подключения", "Невозможно установить соединение с БД, проверьте связь", "Ок");


			return lm;
		}
		public bool SaveMarker(Marker marker, int groupId)
		{
			if (connection.State == System.Data.ConnectionState.Open)
			{
				if (marker.uid == -1)
				{
					string query = $"INSERT INTO `marker_table` VALUES (NULL, '{groupId}', '{JsonConvert.SerializeObject(marker)}');";
					MySqlCommand cmd = new MySqlCommand(query, connection);
					cmd.ExecuteNonQuery();
				}
				else
				{
					string query = $"UPDATE `marker_table` SET `Marker` = '{JsonConvert.SerializeObject(marker)}'  WHERE `marker_id` = {marker.uid};";
					MySqlCommand cmd = new MySqlCommand(query, connection);
					cmd.ExecuteNonQuery();
				}
				return true;
			}
			return false;
		}
		public bool DeleteMarker(Marker marker)
		{
			if (connection.State == System.Data.ConnectionState.Open)
			{
				string query = $"Delete FROM `marker_table` WHERE `marker_id` = '{marker.uid}'";
				MySqlCommand cmd = new MySqlCommand(query, connection);
				cmd.ExecuteNonQuery();
				return true;
			}
			return false;
		}
	}
}
