using System;
using System.Threading.Tasks;
using Shiny.Locations;
using Shiny.Notifications;
using Sample;
using Xamarin.Forms;

namespace VKR
{
	public class GeofenceDelegate : IGeofenceDelegate
	{
		readonly INotificationManager notificationManager;
		readonly SampleSqliteConnection conn;

		public GeofenceDelegate(INotificationManager notificationManager, SampleSqliteConnection conn)
		{
			this.notificationManager = notificationManager;
			this.conn = conn;
		}


		public async Task OnStatusChanged(GeofenceState newStatus, GeofenceRegion region)
		{
			/*            await this.conn.InsertAsync(new ShinyEvent
						{
							Text = region.Identifier,
							Detail = $"You {state} the geofence",
							Timestamp = DateTime.Now
						});*/

			if (newStatus == GeofenceState.Unknown)
				return;
			bool inside = newStatus == GeofenceState.Entered;
			//Отправка данных в table_watcher
			App.DataBase.AddWatcherTable(Convert.ToInt32(region.Identifier), inside);
			//Чтение данных о рабочем расписании в текущий день недели SQL, если рабочее время то в случае выхода из рабочей зоны, отправляем уведомление с предупреждением.
			if (!inside && App.DataBase.inWorkTimeBool) 
			{
				await this.notificationManager.Send(
				"Предупреждение!",
				$"Вы покинули рабочую область до окончания рабочего дня!\nВернитесь на рабочее место и продолжите работу!");
			}

			//ИДЕЯ ФИЧА
			//отправлять уведомление о входе, другого сожержания
		}
	}
}
