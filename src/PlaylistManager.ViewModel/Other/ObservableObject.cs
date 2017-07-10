using System.ComponentModel;


namespace PlaylistManager.ViewModel.Other
{
	public class ObservableObject : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged(PropertyChangedEventArgs _e)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			handler?.Invoke(this, _e);
		}

		protected void RaisePropertyChangedEvent(string _propertyName)
		{
			OnPropertyChanged(new PropertyChangedEventArgs(_propertyName));
		}
	}
}
