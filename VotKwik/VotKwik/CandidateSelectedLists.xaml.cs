using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VotKwik
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class CandidateSelectedLists : ContentPage
	{
		#region Global Variables
		private long _userId { get; set; }
		ApiBallotDetailsPost _ballotInfo;
		//string BaseURL = "http://192.168.1.101/VOTKWIK_API_TEST/";
		string BaseURL = "http://votkwik.somee.com/";
		long UserSystemDetailsID = 0;
		#endregion

		#region Button Events
		async void Cancel_Clicked(object sender, System.EventArgs e)
		{
			await Navigation.PopAsync();
		}

		void Delete_Clicked(object sender, System.EventArgs e)
		{
			try
			{
				var deleteCandidate = (sender as MenuItem).CommandParameter as CandidateDetails;
				CandidateDetailsModalPage._candidate.Remove(deleteCandidate);
			}
			catch (Exception ex) { }
		}

		async void CreateBallot_Clicked(object sender, System.EventArgs e)
		{
			try
			{
				if (CandidateDetailsModalPage._candidate.Count == 0 || CandidateDetailsModalPage._candidate.Count == 1)
					await DisplayAlert("Error!", "Select Candidates", "OK");
				else
				{
					var displayAlert = await DisplayAlert("Confirm!", "Sure to create voting ballot", "YES", "NO");
					if (displayAlert)
					{
						var client = new HttpClient();
						//upload image
						activityIndicator.IsVisible = true;
						activityIndicator.IsRunning = true;
						stckLayout.IsVisible = false;
						foreach (var a in CandidateDetailsModalPage._candidate)
						{
							string api = BaseURL + "/VOTKWIK/UploadImage?fileExtension=" + a.FileExtenstion;

							HttpContent bytesContent = new ByteArrayContent(a.FileArr);
							using (var formData = new MultipartFormDataContent())
							{
								formData.Add(bytesContent, "image");
								var response = client.PostAsync(api, formData).Result;

								if (response.IsSuccessStatusCode)
								{
									var ImageName = response.Content.ReadAsStringAsync().Result;
									a.FileName = ImageName.Trim();
									//This is the image name which we need to add in Observable collection object of Candidates against particular candidate.
								}
							}
						}
						//Add User System Details i.e Add Ballot Information in User System Details table
						UserSystemDetail objUserSystemDetail = new UserSystemDetail()
						{
							VotingBallotName = _ballotInfo.VotingBallotName,
							VotingBallotInfo = _ballotInfo.VotingBallotInfo,
							StarteDate = _ballotInfo.StarteDate,
							EndDate = _ballotInfo.EndDate
						};
						using (var httpClient = new HttpClient())
						{
							var response = httpClient.PostAsync(BaseURL + "/VOTKWIK/" + _userId + "/AddSystemDetails",
								new StringContent(JsonConvert.SerializeObject(objUserSystemDetail).ToString(), Encoding.UTF8, "application/json")).Result;
							if (response.IsSuccessStatusCode)
							{
								UserSystemDetailsID = Convert.ToInt64(response.Content.ReadAsStringAsync().Result);
								AddCandidateDetails(UserSystemDetailsID, httpClient);
							}
							else
							{
								await DisplayAlert("Warning", "Error While Calling Web API", "OK");
							}
						}
						activityIndicator.IsVisible = false;
						activityIndicator.IsRunning = false;
						stckLayout.IsVisible = true;
						stckLayout.IsEnabled = true;
						await Navigation.PushAsync(new ThankYouPageBallot());
					}
				}
			}
			catch (Exception ex)
			{

			}
		}

		async void OnAddCandidate(object sender, System.EventArgs e)
		{
			await Navigation.PushModalAsync(new CandidateDetailsModalPage());
		}
		#endregion

		private async void AddCandidateDetails(long UserSystemDetailsID, HttpClient client)
		{
			try
			{
				List<ApiCandidateDetailPOCO> lstCandidateDetailPOCO = new List<ApiCandidateDetailPOCO>();
				foreach (var a in CandidateDetailsModalPage._candidate)
				{
					lstCandidateDetailPOCO.Add(
						new ApiCandidateDetailPOCO()
						{
							CandidateName = a.CandName,
							CandidateParty = a.CandInst,
							ContactNumber = a.CandMob,
							EmailId = a.CandEmailId,
							ImageName = a.FileName
						});
				}

				var response = client.PostAsync(BaseURL + "/VOTKWIK/" + _userId + "/AddCandidate?UserSystemDetailID=" + UserSystemDetailsID,
								   new StringContent(JsonConvert.SerializeObject(lstCandidateDetailPOCO).ToString(), Encoding.UTF8, "application/json")).Result;

				if (response.IsSuccessStatusCode)
				{

				}
				else
				{
					await DisplayAlert("WARNING!", "Error While Calling Web API", "OK");
				}
			}
			catch (Exception ex) { }
		}

		protected override void OnAppearing()
		{
			try
			{
				candidateListView.ItemsSource = CandidateDetailsModalPage._candidate;
				base.OnAppearing();
			}
			catch (Exception ex) { }
		}

		public CandidateSelectedLists(long userID, ApiBallotDetailsPost obj)
		{
			InitializeComponent();
			_userId = userID;
			_ballotInfo = obj;
		}
	}
}
