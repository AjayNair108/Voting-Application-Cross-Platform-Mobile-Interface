using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace VotKwik
{
	public partial class VoteCandidate : ContentPage
	{
		string _ballotNo = "";
		long _userId;
		string ballotTitle = "";
		async void Back_Clicked(object sender, System.EventArgs e)
		{
			await Navigation.PopAsync();
		}

		async void Next_Clicked(object sender, System.EventArgs e)
		{
			await Navigation.PushAsync(new VoteForCandidate(_userId, _ballotNo, ballotTitle));
		}

		public VoteCandidate(long userId, string BallotNo, string VotingBallotTitle, string VotingBallotDesc)
		{
			InitializeComponent();
			_userId = userId;
			_ballotNo = BallotNo;
			ballotTitle = VotingBallotTitle;

			BallotTitle.Text = VotingBallotTitle;
			BallotDesc.Text = VotingBallotDesc;
		}
	}
}
