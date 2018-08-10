using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace VotKwik
{
	public partial class ThankYouPage : ContentPage
	{
		async void Handle_Clicked(object sender, System.EventArgs e)
		{
			await Navigation.PushAsync(new UserAuthentication());
		}

		public ThankYouPage()
		{
			InitializeComponent();
		}

		protected override bool OnBackButtonPressed()
		{
			return true;
		}
	}
}
