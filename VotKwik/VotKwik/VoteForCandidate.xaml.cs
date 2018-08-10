using System;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace VotKwik
{
	public partial class VoteForCandidate : ContentPage
	{
		#region Global Variables

		string BaseURL = "http://votkwik.somee.com";
		HttpResponseMessage response;
		string ballotCode = "";
		long _userId = 0;
		#endregion

		void Handle_ItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
		{
			lstView.SelectedItem = null;
		}

		async void Handle_ItemTapped(object sender, Xamarin.Forms.ItemTappedEventArgs e)
		{
			try
			{
				var contact = e.Item as BoothResultPOCO;
				var displayAlert = await DisplayAlert("Confirm!", "Vote for " + contact.CandidateName, "YES", "NO");
				abc.IsVisible = true;
				abc.IsRunning = true;
				if (displayAlert)
				{
					using (var httpClient = new HttpClient())
					{
						response = httpClient.PostAsync(BaseURL + "/VOTKWIK/" + _userId + "/AddVote?ballotNum=" + ballotCode + "&candidateId=" + contact.UserId, null).Result;
						if (response.IsSuccessStatusCode)
						{
							abc.IsVisible = false;
							abc.IsRunning = false;

							await Navigation.PushAsync(new ThankYouPage());
						}
						else
						{
							abc.IsVisible = false;
							abc.IsRunning = false;

							await DisplayAlert("Retry", "Please Vote Again", "OK");
						}
					}
				}
			}
			catch (Exception ex) { }
		}



		public VoteForCandidate(long userId, string BallotCode, string VotingBallotTitle)
		{
			try
			{
				InitializeComponent();
				_userId = userId;
				ballotCode = BallotCode;
				this.Title = VotingBallotTitle;

				var httpClient = new HttpClient();
				response = httpClient.GetAsync(BaseURL + "/VOTKWIK/" + ballotCode + "/GetResult").Result;

				if (response.IsSuccessStatusCode)
				{
					// Get the response
					var result = JsonConvert.DeserializeObject<List<BoothResultPOCO>>(response.Content.ReadAsStringAsync().Result);
					lstView.ItemsSource = result;
				}
				else
				{
					DisplayAlert("Error", "Not Conected to internet", "OK");
				}
			}
			catch (Exception ex) { }
		}

	}
}
