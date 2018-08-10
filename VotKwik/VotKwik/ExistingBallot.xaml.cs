using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using Xamarin.Forms;
namespace VotKwik
{
	public partial class ExistingBallot : ContentPage
	{
		#region Global Variables
		string _adminName;
		HttpResponseMessage response;
		long _userId;
		//string BaseURL = "http://192.168.1.101/VOTKWIK_API_TEST/";
		string BaseURL = "http://votkwik.somee.com/";
		string _aadharNumber, _mobileNumber;

		#endregion

		void Handle_ItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
		{
			listView.SelectedItem = null;
		}

		async void Handle_ItemTapped(object sender, Xamarin.Forms.ItemTappedEventArgs e)
		{
			try
			{
				var contact = e.Item as CreatedBallotDetails;
				await Navigation.PushAsync(new CandidateVoteCount(contact.BroadcastTokenNo, contact.VotingBallotName, _userId, _adminName, _aadharNumber, _mobileNumber));
			}
			catch (Exception ex) { }
		}

		async private void noBallotCreated()
		{
			await DisplayAlert("Sorry", "Not created any voting ballot", "OK");
			await Navigation.PushAsync(new VotingTokenPage(_userId, _adminName, _aadharNumber, _mobileNumber));
		}

		protected override bool OnBackButtonPressed()
		{
			return false;
		}

		public ExistingBallot(long userId, string adminName, string aadharNumber, string mobileNumber)
		{
			try
			{
				InitializeComponent();
				_userId = userId;
				_adminName = adminName;
				_aadharNumber = aadharNumber;
				_mobileNumber = mobileNumber;

				this.Title = "Welcome, " + adminName;
				HttpClient httpClient = new HttpClient();
				response = httpClient.GetAsync(BaseURL + "VOTKWIK/" + userId + "/GetAvailableBallots").Result;
				var result = new List<CreatedBallotDetails>();
				if (response.IsSuccessStatusCode)
				{
					result = JsonConvert.DeserializeObject<List<CreatedBallotDetails>>(response.Content.ReadAsStringAsync().Result);
				}
				else if (response.StatusCode == HttpStatusCode.NotAcceptable)
					noBallotCreated();
				else
				{
					DisplayAlert("Error", "Check your internet connection", "OK");
				}
				listView.ItemsSource = result;
			}
			catch (Exception ex) { }
		}

	}
}
