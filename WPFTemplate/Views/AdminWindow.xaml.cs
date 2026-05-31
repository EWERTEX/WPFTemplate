using System.Windows;
using WPFTemplate.ViewModels;

namespace WPFTemplate.Views
{
	public partial class AdminWindow : Window
	{
		public AdminWindow()
		{
			InitializeComponent();
			DataContext = new AdminViewModel();
		}
	}
}