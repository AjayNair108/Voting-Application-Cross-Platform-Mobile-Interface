using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace VotKwik
{
	public partial class BallotInformation : ContentPage
	{
		private long _userId { get; set; }
		ApiBallotDetailsPost ballotInfo = new ApiBallotDetailsPost();

		async void CandidateDetails_Clicked(object sender, System.EventArgs e)
		{
			try
			{
				if (!string.IsNullOrEmpty(ballotTitleEntry.Text) && !string.IsNullOrEmpty(ballotDescEntry.Text))
				{
					ballotInfo.VotingBallotName = (string)ballotTitleEntry.Text;
					ballotInfo.VotingBallotInfo = (string)ballotDescEntry.Text;
					ballotInfo.StarteDate = Convert.ToDateTime(ballotStartDateEntry.Date);
					ballotInfo.EndDate = Convert.ToDateTime(ballotEndDateEntry.Date);
					await Navigation.PushAsync(new CandidateSelectedLists(_userId, ballotInfo));
				}
				else
					await DisplayAlert("Error!", "Complete all the fields", "OK");
			}
			catch (Exception ex) { }
		}





		public BallotInformation(long userId, string adminName)
		{
			InitializeComponent();
			_userId = userId;
			title.Title = "Local Admin: " + adminName;
		}
	}
}
