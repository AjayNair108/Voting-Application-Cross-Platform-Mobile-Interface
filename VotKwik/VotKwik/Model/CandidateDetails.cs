using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace VotKwik
{
	public class CandidateDetails : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		private string imageBase64;
		public string ImageBase64
		{
			get 
			{ 
				return imageBase64; 
			}
			set
			{
				imageBase64 = value;
				OnPropertyChanged("ImageBase64");
				CandImage = ImageSource.FromStream(
					() => new MemoryStream(Convert.FromBase64String(imageBase64)));
			}
		}
		private ImageSource _candImage;
		public ImageSource CandImage
		{
			get { return _candImage; }
			set
			{
				_candImage = value;
				OnPropertyChanged("CandImage");
			}
		}

		public string _candName; 
		public string CandName
		{
			get { return _candName; }
			set
			{
				if (_candName == value)
					return;

				_candName = value;

				OnPropertyChanged(CandName);

			}
		}
		//public string CandInst { get; set; }
		public string _candInst;
		public string CandInst
		{
			get { return _candInst; }
			set
			{
				if (_candInst == value)
					return;

				_candInst = value;

				OnPropertyChanged(CandInst);

			}
		}
		//public string CandEmailId { get; set;}
		public string _candEmailId;
		public string CandEmailId
		{
			get { return _candEmailId; }
			set
			{
				if (_candEmailId == value)
					return;

				_candEmailId = value;

				OnPropertyChanged(CandEmailId);

			}
		}
		//public int CandMob { get; set;}
		public string _candMob;
		public string CandMob
		{
			get { return _candMob; }
			set
			{
				if (_candMob == value)
					return;

				_candMob = value;

				OnPropertyChanged(CandMob);

			}
		}



		public byte[] FileArr {
			get;set;
		}

		public string FileExtenstion { get; set; }
		public string FileName { get; set; }
		//private bool _isFavorite;
		//public bool IsFavorite
		//{
		//	get { return _isFavorite; }
		//	set
		//	{
		//		if (_isFavorite == value)
		//			return;
		//		_isFavorite = value;
		//		OnPropertyChanged();
		//		OnPropertyChanged(nameof(Color));
		//	}
		//}

		private void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

	}
}
