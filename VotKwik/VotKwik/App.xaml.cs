using Xamarin.Forms;

namespace VotKwik
{
	public partial class App : Application
	{
		public App()
		{
			InitializeComponent();

			MainPage = new NavigationPage(new UserAuthentication())
			{
				BarTextColor = Color.White,
				BarBackgroundColor = Color.FromHex("#3B4371")
			};
		}

		protected override void OnStart()
		{
			// Handle when your app starts
		}

		protected override void OnSleep()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume()
		{
			// Handle when your app resumes
		}
	}
}
