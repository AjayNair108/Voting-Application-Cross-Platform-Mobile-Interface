using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace VotKwik
{
	public partial class SuperAdminLogin : ContentPage
	{
		string _userName = "";
		string _userPass = "";
		async void Handle_Clicked(object sender, System.EventArgs e)
		{
			_userName = userName.Text;
			_userPass = userPass.Text;
			if ((_userName.Equals("VotkwikSU") || _userName.Equals("votkwiksu")) && _userPass.Equals("Welcome@123"))
				await Navigation.PushAsync(new AddNewUser());
			else
				await DisplayAlert("Error", "Not Super Admin", "OK");
		}

		public SuperAdminLogin()
		{
			InitializeComponent();
		}
	}
}
