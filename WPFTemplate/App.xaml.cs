using System.Windows;
using System.Windows.Threading;

namespace WPFTemplate
{
	public partial class App : Application
	{
		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);
			
			DispatcherUnhandledException += AppDispatcherUnhandledException;
			
			AppDomain.CurrentDomain.UnhandledException += CurrentDomainUnhandledException;
		}

		private static void AppDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
		{
			MessageBox.Show($"Критическая ошибка: {e.Exception.Message}\nПроверьте подключение к базе данных.", 
				"Сбой системы", MessageBoxButton.OK, MessageBoxImage.Error);
			
			e.Handled = true; 
		}

		private static void CurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			if (e.ExceptionObject is Exception ex)
			{
				MessageBox.Show($"Системная ошибка: {ex.Message}", "Сбой", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}
	}
}