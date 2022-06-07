using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using MySqlConnector;
using Newtonsoft.Json;
using Shiny;
using Shiny.Locations;
using Shiny.Notifications;
using VKR.Models;
using VKR.Models.Admin;
using VKR.Models.Watcher;
using VKR.Models.Worker;
using Xamarin.Essentials;

namespace VKR.Data
{
	public class DataBase
	{
		public WorkerType wtype;
		public int emp_id;
		public int work_group;
		public bool inWorkTimeBool;
		private GeoTimer geoTimer;
		public MySqlConnection connection = new MySqlConnection();

		public DataBase() { geoTimer = new GeoTimer(); wtype = WorkerType.NoAuth; }

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
			if (dr.Read())
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
						connection.Close();
						return false;
				}
				dr.Close();
				App.Current.Properties["login"] = login;
				App.Current.Properties["password"] = password;
				string query = $"SELECT work_group_id FROM employee WHERE emp_id = {emp_id};";
				command = new MySqlCommand(query, connection);
				work_group = (int)command.ExecuteScalar();
				connection.Close();
				Xamarin.Forms.Device.StartTimer(TimeSpan.FromMinutes(1), () =>
				{
					inWorkTimeBool = App.DataBase.InWorkTime();
					return wtype != WorkerType.NoAuth;
				});
				geoTimer.StopTimer();
				geoTimer.StartTimer();
				return true;
			}
			dr.Close();
			wtype = WorkerType.NoAuth;
			connection.Close();
			return false;
		}

		public ObservableCollection<WatchWorker> GetWatcher_Data() 
		{
			ObservableCollection<WatchWorker> lww = new ObservableCollection<WatchWorker>();
			string query = "SELECT * FROM watcher_all";
			connection.Open();
			if (this.connection.State == System.Data.ConnectionState.Open) 
			{
				MySqlCommand cmd = new MySqlCommand(query, this.connection);
				MySqlDataReader dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					lww.Add(new WatchWorker((int)dr["emp_id"], dr["ФИО"] as string, dr["Должность"] as string, (int)dr["work_group_id"], dr["Название подразделения"] as string, (int)dr["Количество пропусков"], (TimeSpan)dr["Время пропусков"]));
				}
				dr.Close();
				connection.Close();
				foreach (WatchWorker ww in lww) 
				{
					ww.skip_list = GetWatcher_Data_Person(ww.id);
				}
			}
			else
				App.Current.MainPage.DisplayAlert("Ошибка подключения", "Невозможно установить соединение с БД, проверьте связь", "Ок");
			if(connection.State == System.Data.ConnectionState.Open)
				connection.Close();
			return lww;
		}
		public ObservableCollection<Worker_skip> GetWatcher_Data_Person(int id) 
		{
			ObservableCollection<Worker_skip> skip_list = new ObservableCollection<Worker_skip>();
			connection.Open();
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
			connection.Close();
			return skip_list;
		}
		public bool DeleteWTS(int skip_id) 
		{
			connection.Open();
			if (connection.State == System.Data.ConnectionState.Open)
			{
				string query = $"Delete FROM watcher_table_skip WHERE id = {skip_id}";
				MySqlCommand cmd = new MySqlCommand(query, connection);
				cmd.ExecuteNonQuery();
				connection.Close();
				return true;
			}
			connection.Close();
			return false;
		}

		public ObservableCollection<Group> GetAdmin_Group_Data() 
		{
			ObservableCollection<Group> Lg = new ObservableCollection<Group>();
			string query = "SELECT * FROM work_group_name";
			connection.Open();
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
			connection.Close();
			return Lg;

		}
		public bool SaveGroup(string groupName, int groupId = -1)
		{
			connection.Open();
			if (connection.State == System.Data.ConnectionState.Open)
			{
				string query = (groupId != - 1) ? $"UPDATE `work_group_name` SET `group_name` = '{groupName}' WHERE `work_group_id` = {groupId}" : $"INSERT INTO `work_group_name` (`work_group_id`, `group_name`) VALUES (NULL, '{groupName}')";
				MySqlCommand cmd = new MySqlCommand(query, connection);
				cmd.ExecuteNonQuery();
				connection.Close();
				return true;
			}
			connection.Close();
			return false;
		}
		public bool DeleteGroup(int groupId) 
		{
			connection.Open();
			if (connection.State == System.Data.ConnectionState.Open)
			{
				string query = $"Delete FROM work_group_name WHERE work_group_id = {groupId}";
				MySqlCommand cmd = new MySqlCommand(query, connection);
				cmd.ExecuteNonQuery();
				connection.Close();
				return true;
			}
			connection.Close();
			return false;
		}
		public ObservableCollection<Shedule> GetShedules(int groupId) 
		{
			ObservableCollection<Shedule> ls = new ObservableCollection<Shedule>();
			string query = $"SELECT * FROM time_table WHERE  work_group_id = {groupId}";
			connection.Open();
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
			connection.Close();
			return ls;
		}
		public bool SaveShedule(Shedule shedule, int groupId) 
		{
			connection.Open();
			if (connection.State == System.Data.ConnectionState.Open)
			{
				string query = (shedule.id != -1) ? $"UPDATE `time_table` SET `week_day` = '{(int)shedule.weekDay}', `time_start` = '{shedule.time_start_str}',  `time_end` = '{shedule.time_end_str}'  WHERE `id` = {shedule.id}"
					: $"INSERT INTO `time_table` VALUES (NULL, '{groupId}', '{(int)shedule.weekDay}','{shedule.time_start_str}','{shedule.time_end_str}')";
				MySqlCommand cmd = new MySqlCommand(query, connection);
				cmd.ExecuteNonQuery();
				query = $"DROP EVENT IF EXISTS `wg_{groupId}_{(int)shedule.weekDay}`; CREATE DEFINER =`root`@`%` EVENT `wg_{groupId}_{(int)shedule.weekDay}` ON SCHEDULE EVERY 1 WEEK STARTS '2022-04-0{3+((int)shedule.weekDay)} {shedule.time_end_str}' ON COMPLETION PRESERVE ENABLE DO CALL proc_skipt0(1, '{shedule.time_start_str}', '{shedule.time_end_str}')";
				cmd = new MySqlCommand(query, connection);
				cmd.ExecuteNonQuery();
				connection.Close();
				return true;
			}
			connection.Close();
			return false;
		}
		public bool DeleteShedule(Shedule shedule, int groupId) 
		{
			connection.Open();
			if (connection.State == System.Data.ConnectionState.Open)
			{
				string query = $"Delete FROM time_table WHERE id = {shedule.id}";
				MySqlCommand cmd = new MySqlCommand(query, connection);
				cmd.ExecuteNonQuery();
				query = $"DROP EVENT IF EXISTS `wg_{groupId}_{(int)shedule.weekDay}`;";
				cmd = new MySqlCommand(query, connection);
				cmd.ExecuteNonQuery();
				connection.Close();
				return true;
			}
			connection.Close();
			return false;
		}
		public ObservableCollection<Employee> GetAdminWorkers(int groupId)
		{
			ObservableCollection<Employee> le = new ObservableCollection<Employee>();
			string query = $"SELECT * FROM admin_employeers WHERE  work_group_id = {groupId}";
			connection.Open();
			if (this.connection.State == System.Data.ConnectionState.Open)
			{
				MySqlCommand cmd = new MySqlCommand(query, this.connection);
				MySqlDataReader dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					string pos = dr["Position"] == System.DBNull.Value ? "NULL" : (string)dr["Position"];
					le.Add(new Employee((int)dr["emp_id"], (string)dr["emp_fio"], (string)dr["emp_pos"], (string)dr["mobile_status"], pos));
				}
				dr.Close();
			}
			else
				App.Current.MainPage.DisplayAlert("Ошибка подключения", "Невозможно установить соединение с БД, проверьте связь", "Ок");
			connection.Close();
			return le;
		}
		public bool SaveEmployee(Employee employee, int groupId)
		{
			connection.Open();
			if (connection.State == System.Data.ConnectionState.Open)
			{
				if (employee.id == -1)
				{
					string query = $"INSERT INTO `employee` VALUES (NULL, '{employee.FIO}', '{employee.Position}','{groupId}', NULL);";
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
				connection.Close();
				return true;
			}
			connection.Close();
			return false;
		}
		public bool DeleteEmployee(Employee employee)
		{
			connection.Open();
			if (connection.State == System.Data.ConnectionState.Open)
			{
				string query = $"Delete FROM employee WHERE `emp_id` = '{employee.id}'";
				MySqlCommand cmd = new MySqlCommand(query, connection);
				cmd.ExecuteNonQuery();
				connection.Close();
				return true;
			}
			connection.Close();
			return false;
		}
		public ObservableCollection<Marker> GetMarkers(int groupId) 
		{
			ObservableCollection<Marker> lm = new ObservableCollection<Marker>();
			string query = $"SELECT * FROM marker_table WHERE  group_id = {groupId}";
			connection.Open();
			if (connection.State == System.Data.ConnectionState.Open)
			{
				MySqlCommand cmd = new MySqlCommand(query, connection);
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
			connection.Close();
			return lm;
		}
		public bool SaveMarker(Marker marker, int groupId)
		{
			connection.Open();
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
				connection.Close();
				return true;
			}
			connection.Close();
			return false;
		}
		public bool DeleteMarker(Marker marker)
		{
			connection.Open();
			if (connection.State == System.Data.ConnectionState.Open)
			{
				string query = $"Delete FROM `marker_table` WHERE `marker_id` = '{marker.uid}'";
				MySqlCommand cmd = new MySqlCommand(query, connection);
				cmd.ExecuteNonQuery();
				connection.Close();
				return true;
			}
			connection.Close();
			return false;
		}

		public WatchWorker GetEmployee()
		{
			string query = $"SELECT * FROM watcher_all WHERE emp_id = {emp_id}";
			WatchWorker res = null;
			connection.Open();
			if (this.connection.State == System.Data.ConnectionState.Open)
			{
				MySqlCommand cmd = new MySqlCommand(query, this.connection);
				MySqlDataReader dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					res = new WatchWorker((int)dr["emp_id"], dr["ФИО"] as string, dr["Должность"] as string, (int)dr["work_group_id"], dr["Название подразделения"] as string, (int)dr["Количество пропусков"], (TimeSpan)dr["Время пропусков"]);
				}
				dr.Close();
				connection.Close();
				res.skip_list = GetWatcher_Data_Person(emp_id);
			}
			else
				App.Current.MainPage.DisplayAlert("Ошибка подключения", "Невозможно установить соединение с БД, проверьте связь", "Ок");
			if(connection.State == System.Data.ConnectionState.Open)
				connection.Close();
			return res;
		}

		//INSERT INTO `watcher_table` (`id`, `date_time`, `emp_id`, `marker_id`, `inside`) VALUES (NULL, CURRENT_TIMESTAMP, '3', '6', '1')
		public bool AddWatcherTable(int marker_id, bool inside) 
		{
			connection.Open();
			if (connection.State == System.Data.ConnectionState.Open)
			{
				string query = $"INSERT INTO `watcher_table` VALUES (NULL, CURRENT_TIMESTAMP, '{emp_id}', '{marker_id}', '{Convert.ToInt32(inside)}');";
				MySqlCommand cmd = new MySqlCommand(query, connection);
				cmd.ExecuteNonQuery();
				connection.Close();
				return true;
			}
			connection.Close();
			return false;
		}
		public bool InWorkTime()
		{
			MySqlConnection connection2 = new MySqlConnection($"server=192.168.1.2;port=3306;username=common_user;database=vkrdb");
			connection2.Open();
			if (connection2.State == System.Data.ConnectionState.Open)
			{
				string query = $"SELECT COUNT(id) FROM `time_table` WHERE week_day = WEEKDAY(CURDATE()) AND TIMEDIFF(CURTIME(), time_start)>0 AND TIMEDIFF(CURTIME(), time_end)<0 AND work_group_id = {work_group};";
				MySqlCommand cmd = new MySqlCommand(query, connection2);
				bool buff = (int)(long)(cmd.ExecuteScalar()) > 0;
				return buff;
			}
			connection2.Close();
			return false;
		}

		public async Task<bool> AddAllGeofensingAsync() 
		{
			var geofenceManager = ShinyHost.Resolve<IGeofenceManager>();
			var accessgeo = await geofenceManager.RequestAccess();
			if (accessgeo != AccessState.Available)
			{
				return false;
			}
			await geofenceManager.StopAllMonitoring();
			//???
			var notificationManager = ShinyHost.Resolve<INotificationManager>();
			var accessnot = await notificationManager.RequestAccess();
            if (accessnot != AccessState.Available)
            {
                return false;
            }

			ObservableCollection<Marker> markers = GetMarkers(work_group);
			foreach (Marker marker in markers) 
			{
				var geozone = new GeofenceRegion(
					marker.uid.ToString(),
					new Shiny.Locations.Position(marker.pin.Position.Latitude, marker.pin.Position.Longitude),
					Shiny.Distance.FromMeters(marker.circle.Radius.Meters)
					)
				{
					NotifyOnEntry = true,
					NotifyOnExit = true,
					SingleUse = false
				};
				await geofenceManager.StartMonitoring(geozone);
			}
			return true;
		}

		public bool addPosition(Location location) 
		{
			connection.Open();
			if (connection.State == System.Data.ConnectionState.Open)
			{
				string query = $"UPDATE `employee` SET `Position` = '{JsonConvert.SerializeObject(location)}'  WHERE `emp_id` = {emp_id};";
				MySqlCommand cmd = new MySqlCommand(query, connection);
				cmd.ExecuteNonQuery();
			}
			connection.Close();
			return false;
		}
	}
}
