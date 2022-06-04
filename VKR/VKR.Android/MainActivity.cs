using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using Android;
using Android.Content;
using Shiny;

namespace VKR.Droid
{
	[Activity(
		Label = "VKR",
		Icon = "@mipmap/icon",
		Theme = "@style/MainTheme",
		MainLauncher = true,
		ConfigurationChanges = ConfigChanges.ScreenSize |
			ConfigChanges.Orientation |
			ConfigChanges.UiMode |
			ConfigChanges.ScreenLayout |
			ConfigChanges.SmallestScreenSize )]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
	{
		const int RequestLocationId = 0;

		readonly string[] LocationPermissions =
		{
			Manifest.Permission.AccessCoarseLocation,
			Manifest.Permission.AccessFineLocation,
			Manifest.Permission.AccessBackgroundLocation,
			Manifest.Permission.ForegroundService,
			Manifest.Permission.AccessNetworkState,
			Manifest.Permission.WriteExternalStorage
		};

		protected override void OnCreate(Bundle savedInstanceState)
		{
			this.ShinyOnCreate();
			base.OnCreate(savedInstanceState);

			Xamarin.Essentials.Platform.Init(this, savedInstanceState);
			global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
			Xamarin.FormsMaps.Init(this, savedInstanceState);
			LoadApplication(new App());
		}

		protected override void OnStart()
		{
			base.OnStart();

			if ((int)Build.VERSION.SdkInt >= 23)
			{
				if (CheckSelfPermission(Manifest.Permission.AccessFineLocation) != Permission.Granted)
				{
					RequestPermissions(LocationPermissions, RequestLocationId);
				}
				else
				{
					Console.WriteLine("Location permissions already granted.");
				}
			}
		}

		protected override void OnNewIntent(Intent intent)
		{
			base.OnNewIntent(intent);
			this.ShinyOnNewIntent(intent);
		}

		protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
		{
			base.OnActivityResult(requestCode, resultCode, data);
			this.ShinyOnActivityResult(requestCode, resultCode, data);
		}

		public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
		{
			Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
			base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
			this.ShinyOnRequestPermissionsResult(requestCode, permissions, grantResults);
			if (requestCode == RequestLocationId)
			{
				if ((grantResults.Length == 1) && (grantResults[0] == (int)Permission.Granted))
				{
					Console.WriteLine("Location permissions granted.");
				}
				else
				{
					Console.WriteLine("Location permissions denied.");
				}
			}
		}
	}
}