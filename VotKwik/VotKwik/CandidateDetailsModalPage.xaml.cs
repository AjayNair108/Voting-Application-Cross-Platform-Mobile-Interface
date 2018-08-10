using System;
using System.Collections.ObjectModel;
using System.IO;
using Plugin.Media;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VotKwik
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class CandidateDetailsModalPage : ContentPage
	{
		#region Global Variables
		private long _userId { get; set; }
		public static ObservableCollection<CandidateDetails> _candidate;
		Stream input;
		string imageAsString;
		string FileExtenstion { get; set; }
		byte[] FileArr { get; set; }
		#endregion

		#region Submit Button Activation
		private void candNameEntryUnfocused(object sender, FocusEventArgs e)
		{
			if (AllFieldsValid())
			{
				ShowSubmit();
			}
		}

		private void candMobNumberUnfocused(object sender, FocusEventArgs e)
		{
			if (AllFieldsValid())
			{
				ShowSubmit();
			}
		}

		private bool AllFieldsValid()
		{
			return !string.IsNullOrEmpty(candNameEntry.Text) && !string.IsNullOrEmpty(candMobNumber.Text);
		}

		private void ShowSubmit()
		{
			doneButton.BackgroundColor = Color.FromHex("#ec2e40");
			doneButton.TextColor = Color.White;
		}
		#endregion

		public void AddCandidateDetils()
		{
			_candidate.Add(new CandidateDetails
			{
				CandName = (string)candNameEntry.Text,
				CandEmailId = (string)candEmailId.Text,
				CandMob = (string)candMobNumber.Text,
				ImageBase64 = imageAsString,
				FileArr = FileArr,
				FileExtenstion = FileExtenstion
			});
			return;
		}


		async void OnDoneClicked(object sender, System.EventArgs e)
		{
			try
			{
				var test = AllFieldsValid();
				if (test)
				{
					var phoneNumberValidation = candMobNumber.Text;
					if (phoneNumberValidation.Length != 10)
					{
						await DisplayAlert("ERROR", "Incorrect Mobile Number", "OK");
					}

					else if (string.IsNullOrEmpty(imageAsString))
						await DisplayAlert("Error", "Pick an image", "OK");

					else
					{
						AddCandidateDetils();
						await Navigation.PopModalAsync();
					}
				}
				else
				{
					await DisplayAlert("Error", "Fields are empty", "OK");
				}
			}
			catch (Exception ex) { }

		}

		async void Handle_Clicked(object sender, System.EventArgs e)
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


		public CandidateDetailsModalPage()
		{
			InitializeComponent();
			if (_candidate == null)
				_candidate = new ObservableCollection<CandidateDetails>();

		}
	}
}
