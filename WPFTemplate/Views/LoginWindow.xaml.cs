using System.Windows;
using WPFTemplate.ViewModels;

namespace WPFTemplate.Views
{
	public partial class LoginWindow : Window
	{
		public LoginWindow()
		{
			InitializeComponent();
            
			var viewModel = new LoginViewModel
			{
				CloseAction = Close
			};

			DataContext = viewModel;
		}
	}
}