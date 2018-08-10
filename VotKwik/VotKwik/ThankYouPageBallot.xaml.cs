using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace VotKwik
{
	public partial class ThankYouPageBallot : ContentPage
	{
		async void Handle_Clicked(object sender, System.EventArgs e)
		{
			await Navigation.PushAsync(new UserAuthentication());
		}

		protected override bool OnBackButtonPressed()
		{
			return true;
		}

		public ThankYouPageBallot()
		{
			InitializeComponent();
		}
	}
}
