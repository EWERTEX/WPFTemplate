using WPFTemplate.Helpers;
using WPFTemplate.Services;

namespace WPFTemplate.ViewModels
{
	public class EmulatorViewModel : ObservableObject
	{
		private string _apiUrl = "http://localhost:8080/api/data";
		private string _responseText = "Здесь появятся данные с эмулятора...";
		private bool _isLoading = false;

		public string ApiUrl { get => _apiUrl; set { _apiUrl = value; OnPropertyChanged(); } }
		public string ResponseText { get => _responseText; set { _responseText = value; OnPropertyChanged(); } }
        
		public bool IsLoading 
		{ 
			get => _isLoading; 
			set 
			{ 
				_isLoading = value; 
				OnPropertyChanged();
				RelayCommand.RaiseCanExecuteChanged(); 
			} 
		}

		public RelayCommand FetchDataCommand { get; }

		public EmulatorViewModel()
		{
			FetchDataCommand = new RelayCommand(ExecuteFetchData, _ => !IsLoading);
		}

		private async void ExecuteFetchData(object? parameter)
		{
			if (string.IsNullOrWhiteSpace(ApiUrl))
			{
				ResponseText = "Укажите адрес эмулятора!";
				return;
			}

			IsLoading = true;
			ResponseText = "Подключение к эмулятору... Ожидайте.";
			
			var result = await EmulatorService.FetchDataAsync(ApiUrl);
            
			ResponseText = result;
			IsLoading = false;
		}
	}
}