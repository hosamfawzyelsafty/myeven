using System;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Microsoft.WindowsAzure.MobileServices;
using System.Threading.Tasks;

namespace myeven.Droid
{
	[Activity (Label = "myeven.Droid",
		Icon = "@drawable/icon",
		MainLauncher = true,
		ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation,
		Theme = "@android:style/Theme.Holo.Light")]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity , IAuthenticate
    {
        private MobileServiceUser user;
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Initialize Azure Mobile Apps
			Microsoft.WindowsAzure.MobileServices.CurrentPlatform.Init();

			// Initialize Xamarin Forms
			global::Xamarin.Forms.Forms.Init (this, bundle);
            App.Init((IAuthenticate)this);

			// Load the main application
			LoadApplication (new App ());
		}
        public async Task<bool> Authenticate()
        {
            bool success = false;
            string massage = string.Empty;
            try
            { user = await TodoItemManager.DefaultManager.CurrentClient.LoginAsync(this, MobileServiceAuthenticationProvider.Twitter);
                if (user != null) {
                    massage = string.Format("you are now sign in {0} ", user.UserId);
                    success = true;
                }
            }
            catch(Exception e)
            {
                massage = e.Message;
            }
            AlertDialog.Builder builder = new AlertDialog.Builder(this);
            builder.SetMessage(massage);
            builder.SetTitle("sign-in massage");
            builder.Create().Show();
            return success;
        }
	}
}

