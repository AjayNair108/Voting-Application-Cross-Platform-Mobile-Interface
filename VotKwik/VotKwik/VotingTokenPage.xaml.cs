using System;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace VotKwik
{
	public partial class VotingTokenPage : ContentPage
	{
		#region Global Variables
		long _userId;
		HttpClient httpClient = new HttpClient();
		HttpResponseMessage response;
		string _adminName = "";
		string BaseURL = "http://votkwik.somee.com";
		string _aadharNumber, _mobileNumber;
		#endregion

		async void Back_Clicked(object sender, System.EventArgs e)
		{
			try
			{
				await Navigation.PushAsync(new UserAuthentication());
			}
			catch (Exception ex)
			{ }
		}

		async void Profile_Button_Clicked(object sender, System.EventArgs e)
		{
			try
			{
				await Navigation.PushAsync(new AdminProfile(_userId, _adminName, _aadharNumber, _mobileNumber));
			}
			catch (Exception ex)
			{ }
		}

		async void Code_Entered(object sender, System.EventArgs e)
		{
			try
			{
				activityIndicator.IsRunning = true;
				activityIndicator.IsVisible = true;
				response = httpClient.GetAsync(BaseURL + "/VOTKWIK/" + ballotCode.Text.Trim() + "/AuthenticateBallot").Result;
				if (response.IsSuccessStatusCode)
				{
					var cbr = JsonConvert.DeserializeObject<CreatedBallotDetails>(response.Content.ReadAsStringAsync().Result);
					if (cbr.VotingBallotInfo != null && cbr.VotingBallotName != null)
					{
						response = httpClient.PostAsync(BaseURL + "/VOTKWIK/" + _userId + "/AddVote?ballotNum=" + ballotCode.Text.Trim() + "&candidateId=" + 0, null).Result;
						activityIndicator.IsRunning = false;
						activityIndicator.IsVisible = false;
						var message = response.Content.ReadAsStringAsync().Result;
						if (message.Equals("\"Sorry you already done voting for this ballot\""))
							await DisplayAlert("Warning!","You have already voted for the ballot","OK");
						else if(message.Equals("\"Sorry Voting date has been Expired\""))
							await DisplayAlert("Warning!", "Ballot has been expired", "OK");
						else
							await Navigation.PushAsync(new VoteCandidate(_userId, ballotCode.Text, cbr.VotingBallotName, cbr.VotingBallotInfo));
					}
					else
						await DisplayAlert("Error", "Ballot doesn't exist or has expired", "Ok");
				}
				else
					await DisplayAlert("Error", "Ballot doesn't exist", "Ok");
			}
			catch (Exception ex) { }
		}

		async void NewBallot_Clicked(object sender, System.EventArgs e)
		{
			try
			{
				await Navigation.PushAsync(new BallotInformation(_userId, _adminName));
			}
			catch (Exception ex) { }
		}


		public VotingTokenPage(long UserId, string adminName, string aadharNumber, string mobileNumber)
		{
			InitializeComponent();
			try
			{
				_userId = UserId;
				_adminName = adminName;
				_aadharNumber = aadharNumber;
				_mobileNumber = mobileNumber;
			}
			catch (Exception ex) { }
		}
		protected override bool OnBackButtonPressed()
		{
			return true;
		}
	}
}
