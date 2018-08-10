
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Xml.Linq;
using Xamarin.Forms;
using ZXing.Net.Mobile.Forms;

namespace VotKwik
{
	public partial class UserAuthentication : ContentPage
	{
		#region Global Variables
		string superAdminCodeName = "";
		string superAdminCodeNo = "";
		HttpResponseMessage response;
		long userId;
		string _oldString;
		public string userName = "";
		string BaseURL = "http://votkwik.somee.com";
		public static LocalAdminDetails objLocalAdmin = new LocalAdminDetails();
		public static AddUserPOCO objUserAddPOCO = new AddUserPOCO();
		public static QRCodeDetails obj = new QRCodeDetails();
		//string BaseURL = "http://192.168.1.101/VOTKWIK_API_TEST/";
		#endregion

		#region Button Click Events
  #region User clicked on the icon
		async void Handle_Clicked(object sender, System.EventArgs e)
		{
			try
			{
				await Navigation.PushAsync(new SuperAdminLogin());
			}
			catch (Exception ex) { }
		}
#endregion
		async void SendToSuperAdmin()
		{
			try
			{
				await Navigation.PushAsync(new SuperAdminLogin());
			}
			catch (Exception ex) { }
		}

		async void Submit_Clicked(object sender, System.EventArgs e)
		{
			try
			{
				superAdminCodeName = aadhar_number.Text;
				superAdminCodeNo = mobile_number.Text;
				if (superAdminCodeName.Equals("52431526") && superAdminCodeNo.Equals("62513425"))
					SendToSuperAdmin();
				else
				{
					try
					{
						if (!string.IsNullOrEmpty(aadhar_number.Text) && !string.IsNullOrEmpty(mobile_number.Text))
						{
							HttpClient client = new HttpClient();
							client.BaseAddress = new Uri(BaseURL);
							response = client.GetAsync("VOTKWIK/" + aadhar_number.Text + "/AuthenticateUser" +
													   "?contactNumber=" + "91" + mobile_number.Text).Result;
							if (response.StatusCode == HttpStatusCode.OK)
							{
								userId = Convert.ToInt64(await response.Content.ReadAsStringAsync());
								var scanPage = new ZXingScannerPage();
								await Navigation.PushAsync(scanPage);
								scanPage.OnScanResult += (result) =>
								{
									// Stop scanning
									scanPage.IsScanning = false;
									Device.BeginInvokeOnMainThread(async () =>
									   {
										   await Navigation.PopAsync();

											abslyt.IsVisible = false;
										   activity.IsRunning = true;
										   activity.IsVisible = true;

										if (result.Text.Contains("xml") && result.Text.Contains("uid"))
									  	 {
										   HttpClient objClient = new HttpClient();
										   string qrcode = result.Text;

										   _oldString = "\"utf-8?\"";
										   //_newString = "<? xml version = \"1.0\" encoding = \"UTF-8\"?>";
										   if (result.Text.Contains(_oldString))
										   {
											   qrcode = qrcode.Replace(_oldString, "\"utf-8\" ?");
										   }

										   response = objClient.GetAsync(BaseURL + "/VOTKWIK/" + userId + "/AuthenticateQrCode?" + "&Qrcode=" + qrcode).Result;
											   if (response.IsSuccessStatusCode)
											   {
												   if (response.StatusCode == HttpStatusCode.OK)
												   {
													   objUserAddPOCO.UserQrCode = qrcode;
													   XElement test = XElement.Parse(qrcode);

													   foreach (var node in test.Attributes())
													   {
														   if (node.Name == "uid")
															   obj.uid = node.Value;
														   else if (node.Name == "dist")
															   obj.dist = node.Value;
														   else if (node.Name == "gender")
															   obj.gender = node.Value;
														   else if (node.Name == "gname")
															   obj.gname = node.Value;
														   else if (node.Name == "lm")
															   obj.lm = node.Value;
														   else if (node.Name == "loc")
															   obj.loc = node.Value;
														   else if (node.Name == "name")
															   obj.name = node.Value;
														   else if (node.Name == "pc")
															   obj.pc = node.Value;
														   else if (node.Name == "state")
															   obj.state = node.Value;
														   else if (node.Name == "vtc")
															   obj.vtc = node.Value;
														   else if (node.Name == "yob")
															   obj.yob = node.Value;
													   }
													   objUserAddPOCO.ContactNumber = mobile_number.Text;
													abslyt.IsVisible = true;
													activity.IsRunning = false;
													activity.IsVisible = false;
													   await DisplayAlert("Scanning Complete!", "", "OK");
													   await Navigation.PushAsync(new VotingTokenPage(userId, obj.name, aadhar_number.Text, mobile_number.Text));
												   }
											   }
											   else 
											{
												await DisplayAlert("Error!", "It is not an aadhar qr code", "OK");
											}
										   };
									   });
								};
							}
							else
							{
								await DisplayAlert("Error", "Invalid User", "OK");
							}
						}
					}
					catch
					{
						await DisplayAlert("Error", "Application is not Connected to the internet", "OK");
					}
				}
			}
			catch (Exception ex) { }
		}
#endregion
		#region Text Entries Checker
		private void mobile_number_unfocused(object sender, FocusEventArgs e)
		{
			if (AllFieldsValid())
			{
				ShowSubmit();
			}
		}

		private void aadhar_number_unfocused(object sender, FocusEventArgs e)
		{
			if (AllFieldsValid())
			{
				ShowSubmit();
			}
		}

		private bool AllFieldsValid()
		{
			return !string.IsNullOrEmpty(aadhar_number.Text) && !string.IsNullOrEmpty(mobile_number.Text) && mobile_number.Text.Length == 10;
		}

		private void ShowSubmit()
		{
			nextButton.BackgroundColor = Color.FromHex("#ec2e40");
			nextButton.TextColor = Color.White;
		}
		#endregion

		public UserAuthentication()
		{
			InitializeComponent();
		}
	}
}
