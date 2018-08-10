using System;
namespace VotKwik
{
	public class ApiBallotDetailsPost
	{

 		public string VotingBallotName { get; set; }

		public string VotingBallotInfo { get; set; }

		public DateTime StarteDate { get; set; }

		public DateTime EndDate { get; set; }
	}
}
