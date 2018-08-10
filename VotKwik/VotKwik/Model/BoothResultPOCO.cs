using System;
namespace VotKwik
{
	public class BoothResultPOCO
	{
		public string CandidateName { get; set; }
		public int VoteCount { get; set; }
		public long UserId { get; set; }
		//private string _imageURL = "http://192.168.1.101/VOTKWIK_API_TEST/Upload/";
		private string _imageURL = "http://votkwik.somee.com/Upload/";
		public string CandidateImage
		{
			get
			{
				return _imageURL;
			}
			set
			{
				_imageURL = _imageURL + value;
			}
		}
	}
}
