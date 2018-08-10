using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using Plugin.Media;
using Xamarin.Forms;

namespace VotKwik
{
	public partial class AdminProfile : ContentPage
	{
		#region Global Variables
		HttpClient httpClient = new HttpClient();
		HttpResponseMessage response;
		long _userId;
		Stream input;
		string imageAsString;
		string ImageName = "";
		string _adminName;
		string FileExtenstion { get; set; }
		byte[] FileArr { get; set; }
		string _aadharNumber, _mobileNumber;
		string BaseURL = "http://votkwik.somee.com/";
		string userImageName = "http://votkwik.somee.com/Upload/";
		public static List<LocalAdminDetails> objLocalAdmin = new List<LocalAdminDetails>();
		#endregion
		async void Cancel_Button_Clicked(object sender, System.EventArgs e)
		{
			await Navigation.PopAsync();
		}

		async void Profile_Image_Clicked(object sender, System.EventArgs e)
		{
			try
			{
				if (!CrossMedia.Current.IsPickPhotoSupported)
				{
					await DisplayAlert("Photos Not Supported", ":( Permission not granted to photos.", "OK");
					return;
				}

				var file = await CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions
				{
					PhotoSize = Plugin.Media.Abstractions.PhotoSize.Small
				});

				if (file == null)
				{
					return;
				}

				input = file.GetStream();

				byte[] buffer = new byte[16 * 1024];
				using (MemoryStream ms = new MemoryStream())
				{
					int read;
					while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
					{
						ms.Write(buffer, 0, read);
					}
					imageAsString = Convert.ToBase64String(ms.ToArray());
					FileArr = ms.ToArray();
					FileExtenstion = System.IO.Path.GetExtension(file.Path);
					string FileName = "";
					var client = new HttpClient();
					//upload image

					string api = BaseURL + "VOTKWIK/UploadImage?fileExtension=" + FileExtenstion;

					HttpContent bytesContent = new ByteArrayContent(FileArr);
					using (var formData = new MultipartFormDataContent())
					{
						formData.Add(bytesContent, "image");
						response = client.PostAsync(api, formData).Result;
						if (response.IsSuccessStatusCode)
						{
							ImageName = response.Content.ReadAsStringAsync().Result;
							FileName = ImageName.Trim();
							//This is the image name which we need to add in Observable collection object of Candidates against particular candidate.
						}
					}


					api = BaseURL + "VOTKWIK/" + _userId + "/UpdateUserImage?ImageName=" + FileName;
					response = client.PostAsync(api, null).Result;

					if (response.IsSuccessStatusCode)
					{
						if (response.StatusCode == HttpStatusCode.OK)
						{
							ImageName = response.Content.ReadAsStringAsync().Result;
							ImageName = ImageName.Replace("\"", string.Empty).Trim();
							ImageName = ImageName.Replace("\\", string.Empty).Trim();
							userImageName += ImageName.Trim();
						}
					}
				}

				image.Source = ImageSource.FromStream(() =>
				{
					var stream = file.GetStream();
					file.Dispose();
					return stream;
				});
			}
			catch (Exception ex) { }

		}

		async void Existing_Ballot_Clicked(object sender, System.EventArgs e)
		{
			try
			{
				httpClient = new HttpClient();
				response = httpClient.GetAsync(BaseURL + "VOTKWIK/" + _userId + "/GetAvailableBallots").Result;
				var result = new List<CreatedBallotDetails>();
				if (response.IsSuccessStatusCode)
				{
					result = JsonConvert.DeserializeObject<List<CreatedBallotDetails>>(response.Content.ReadAsStringAsync().Result);
				}
				if (result.Count == 0)
				{
					await DisplayAlert("Note!", "No voting ballot created", "OK");
				}
				else
					await Navigation.PushAsync(new ExistingBallot(_userId, _adminName, _aadharNumber, _mobileNumber));
			}
			catch (Exception ex)
			{
				await DisplayAlert("Warning!", ex.Message, "OK");
			}
		}

		async void VotingTokenPageClicked(object sender, System.EventArgs e)
		{
			await Navigation.PopAsync();
		}

		public void GetUserImage()
		{
			response = httpClient.GetAsync(BaseURL + "VOTKWIK/" + _userId + "/GetUserImage").Result;

			if (response.IsSuccessStatusCode && response.StatusCode == HttpStatusCode.OK)
			{
				ImageName = response.Content.ReadAsStringAsync().Result;
				ImageName = ImageName.Replace("\"", string.Empty).Trim();
				ImageName = ImageName.Replace("\\", string.Empty).Trim();
				userImageName += ImageName.Trim();
			}
		}

		public AdminProfile(long userId, string adminName, string aadharNumber, string mobileNumber)
		{
			_userId = userId;
			InitializeComponent();
			GetUserImage();
			if (string.IsNullOrEmpty(ImageName) || userImageName.Equals("http://votkwik.somee.com/Upload/") || string.IsNullOrEmpty(userImageName) || ImageName == "null" || ImageName.Equals("null"))
			{
				activityIndicator.IsVisible = false;
				image.Source = "camera4.png";
			}
			else
			{
				activityIndicator.IsVisible = true;
				image.Source = userImageName;
			}
			_adminName = adminName;
			_aadharNumber = aadharNumber;
			_mobileNumber = mobileNumber;

			adminNameEntry.Text = adminName;
			aadharNumberEntry.Text = aadharNumber;
			contactNumberEntry.Text = mobileNumber;
		}
	}
}
