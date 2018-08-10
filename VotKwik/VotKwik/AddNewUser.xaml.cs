using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Newtonsoft.Json;
using ZXing.Net.Mobile.Forms;

using Xamarin.Forms;

namespace VotKwik
{
	public partial class AddNewUser : ContentPage
	{
		string BaseURL = "http://votkwik.somee.com";

		AddUserPOCO objUserAddPOCO = new AddUserPOCO();
		QRCodeDetails obj = new QRCodeDetails();
		string matched;
		long userId = 0;
		string qrcode;
		string _oldString;

		async void DisplayResults(object sender, System.EventArgs e)
		{
			var ScannedQrcode = QRScanning();
			HttpClient client = new HttpClient();
			client.BaseAddress = new Uri(BaseURL);
			var response = client.GetAsync("/VOTKWIK/" + obj.uid + "/AuthenticateUser" + "?contactNumber=" + "91" + mobile_number.Text).Result;
			activityIndicator.IsVisible = false;
			grid.IsEnabled = true;
			if (response.StatusCode == HttpStatusCode.OK)
			{
				userId = Convert.ToInt64(await response.Content.ReadAsStringAsync());
				await Navigation.PushAsync(new VotingTokenPage(userId, obj.name, obj.uid, mobile_number.Text));
			}
			else
			{

			}
		}

		public async Task<string> QRScanning()
		{
			try
			{

				var scanPage = new ZXingScannerPage();
				await Navigation.PushAsync(scanPage);

				scanPage.OnScanResult += (result) =>
				{
					// Stop scanning
					scanPage.IsScanning = false;
					Device.BeginInvokeOnMainThread(async () =>
				   {
					   await Navigation.PopAsync();
						if (result.Text.Contains("xml") && result.Text.Contains("uid"))
						{
						   await DisplayAlert("Scanning Complete", "", "Done!");

						   activityIndicator.IsVisible = true;
						   grid.IsEnabled = false;

							qrcode = result.Text;
					   _oldString = "\"utf-8?\"";
					   //_newString = "<? xml version = \"1.0\" encoding = \"UTF-8\"?>";
					   if (result.Text.Contains(_oldString))
					   {
						   qrcode = qrcode.Replace(_oldString, "\"utf-8\" ?");
					   }
					   HttpClient objClient = new HttpClient();
					   string uId = ExtractFromString(qrcode, "uid=\"", "\" name");
					   var userId = Convert.ToInt64(uId);
					   var response = objClient.GetAsync(BaseURL + "/VOTKWIK/" + userId + "/AuthenticateQrCode?"
														 + "&Qrcode=" + qrcode).Result;
					   if (response.StatusCode == HttpStatusCode.OK)
					   {
						   await DisplayAlert("Note!", "User has already been registered", "OK");
					   }
					   else
					   {
						   objUserAddPOCO.UserQrCode = qrcode;
						   var test = XElement.Parse(qrcode);
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
						   aadharNo.Text = obj.uid;
						   await SaveUser();
					   }
				   }
					                               else
					{
						   await DisplayAlert("Error!","It is not an aadhar qr code","OK");
					}
				   });

				};
				return qrcode;
			}
			catch
			{

			}
			return "";
		}
		public async Task<int> SaveUser()
		{
			if (objUserAddPOCO.UserQrCode != null)
			{
				objUserAddPOCO.AdharNo = obj.uid;
				objUserAddPOCO.UserName = obj.name;
				objUserAddPOCO.Age = Convert.ToInt32(obj.yob);
				objUserAddPOCO.ContactNumber = "+91" + mobile_number.Text;

				using (var httpClient = new HttpClient())
				{
					var response = httpClient.PostAsync(BaseURL + "/VOTKWIK/User", new StringContent(JsonConvert.SerializeObject(objUserAddPOCO).ToString(), Encoding.UTF8, "application/json")).Result;
					activityIndicator.IsVisible = false;
					grid.IsEnabled = true;
					if (response.IsSuccessStatusCode)
					{
						if (response.StatusCode == HttpStatusCode.NotModified)
							await DisplayAlert("Age is less", "Grow big", "OK");
						else
						{
							userId = Convert.ToInt64(await response.Content.ReadAsStringAsync());
							lblResult.Text = "User Added Successfully" + response.Content.ReadAsStringAsync().Result;
							await Navigation.PushAsync(new VotingTokenPage(userId, obj.name, obj.uid, mobile_number.Text));
						}
					}
					else
					{
						lblResult.Text = "Error While Calling Web API";
					}
				}
			}
			return 1;
		}

		private string ExtractFromString(string text, string startString, string endString)
		{
			int indexStart = 0, indexEnd = 0;
			bool exit = false;
			while (!exit)
			{
				indexStart = text.IndexOf(startString);
				indexEnd = text.IndexOf(endString);
				if (indexStart != -1 && indexEnd != -1)
				{
					matched = (text.Substring(indexStart + startString.Length,
						indexEnd - indexStart - startString.Length));
					text = text.Substring(indexEnd + endString.Length);
				}
				else
					exit = true;
			}
			return matched;
		}


		public AddNewUser()
		{
			InitializeComponent();
		}
	}
}
