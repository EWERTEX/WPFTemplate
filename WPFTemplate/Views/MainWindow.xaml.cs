using System.Windows;
using WPFTemplate.ViewModels;

namespace WPFTemplate.Views;

public partial class MainWindow : Window
{
	public MainWindow()
	{
		InitializeComponent();
		
		DataContext = new MainViewModel();
	}
}