using System;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace VotKwik
{
	public partial class CandidateVoteCount : ContentPage
	{
#region Global Variables
		string _adminName = "";
		string _aadharNumber, _mobileNumber;
		long _userId;
		int flag = 0;
		int maxInt, maxIndex;
		HttpResponseMessage response;
		HttpClient httpClient;
		string BaseURL = "http://votkwik.somee.com";
#endregion

		async void Handle_Clicked(object sender, System.EventArgs e)
		{
			await Navigation.PushAsync(new VotingTokenPage(_userId, _adminName, _aadharNumber, _mobileNumber));
		}

		void Handle_ItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
		{
			lstView.SelectedItem = null;
		}

		public CandidateVoteCount(string BallotNo, string BallotTitle, long userId, string adminName, string aadharNumber, string mobileNumber)
		{
			try
			{
				InitializeComponent();
				_userId = userId;
				_adminName = adminName;
				_aadharNumber = aadharNumber;
				_mobileNumber = mobileNumber;
				if (!string.IsNullOrEmpty(BallotTitle))
					this.Title = BallotTitle;
				activityIndicator.IsRunning = true;
				activityIndicator.IsVisible = true;
				httpClient = new HttpClient();
				response = httpClient.GetAsync(BaseURL + "/VOTKWIK/" + BallotNo + "/GetResult").Result;

				if (response.IsSuccessStatusCode)
				{
					// Get the response
					var result = JsonConvert.DeserializeObject<List<BoothResultPOCO>>(response.Content.ReadAsStringAsync().Result);
					for (int i = 0; i < result.Count; i++)
					{
						int value = result[i].VoteCount;
						if (value > maxInt)
						{
							flag = 1;
							maxInt = value;
							maxIndex = i;
						}
					}

					for (int i = maxIndex + 1; i < result.Count; i++)
					{
						int value2 = maxInt;
						if (result[maxIndex].VoteCount == result[i].VoteCount)
						{
							flag = 0;
							winnerName.Text = "It's a Tie";
						}
					}

					if (flag == 1)
						winnerName.Text = "Winner is: " + result[maxIndex].CandidateName;

					activityIndicator.IsRunning = false;
					activityIndicator.IsVisible = false;
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
