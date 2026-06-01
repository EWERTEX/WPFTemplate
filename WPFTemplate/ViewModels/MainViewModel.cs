using System.Windows;
using Microsoft.EntityFrameworkCore;
using WPFTemplate.Helpers;

namespace WPFTemplate.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        private object _currentView;
        public object CurrentView { get => _currentView; set { _currentView = value; OnPropertyChanged(); } }
        
        private double _menuWidth = 200;
        //private DbContext _db = new DbContext();
        private Visibility _menuContentVisibility = Visibility.Visible;
        public Visibility MenuContentVisibility { get => _menuContentVisibility; set { _menuContentVisibility = value; OnPropertyChanged(); } }
        
        public double MenuWidth { get => _menuWidth; set { _menuWidth = value; OnPropertyChanged(); } }
        
        public Visibility AdminVisibility => SessionContext.RoleId == 1 ? Visibility.Visible : Visibility.Collapsed;
        public Visibility ManagerVisibility => (SessionContext.RoleId == 1 || SessionContext.RoleId == 2) ? Visibility.Visible : Visibility.Collapsed;

        
        
        public string CurrentUserName => $"Пользователь: {SessionContext.RoleId}";

        public RelayCommand ToggleMenuCommand { get; }
        public RelayCommand NavigateCommand { get; }
        public RelayCommand LogoutCommand { get; }
        public RelayCommand GenerateReportCommand { get; }

        public MainViewModel()
        {
            NavigateCommand = new RelayCommand(Navigate);
            LogoutCommand = new RelayCommand(Logout);
            GenerateReportCommand = new RelayCommand(GenerateTestReport);
            // CurrentView = new UniversalCrudViewModel<User>(_db);
            
            ToggleMenuCommand = new RelayCommand(_ => 
            {
	            if (MenuWidth == 200)
	            {
		            MenuWidth = 40;
		            MenuContentVisibility = Visibility.Collapsed;
	            }
	            else
	            {
		            MenuWidth = 200;
		            MenuContentVisibility = Visibility.Visible;
	            }
            });
        }

        private void Navigate(object? viewName)
        {
	        if (viewName?.ToString() == "Emulator")
	        {
		        CurrentView = new EmulatorViewModel();
	        }
	        
            /* if (viewName.ToString() == "Users") CurrentView = new UniversalCrudViewModel<User>(_db);
            if (viewName.ToString() == "Products") CurrentView = new UniversalCrudViewModel<Product>(_db);
            */
        }

        private static void Logout(object? obj)
        {
            SessionContext.Clear();
            var loginWindow = new Views.LoginWindow();
            loginWindow.Show();
            
            foreach (Window window in Application.Current.Windows)
            {
                if (window is Views.MainWindow) window.Close();
            }
        }
        
        private void GenerateTestReport(object? obj)
        {
	        var testCases = new List<(string Action, string Expected, string Result)>
	        {
		        ("Ввод верного логина и пароля", "Успешная авторизация, переход на главное окно", "Успешно (Открыто главное окно)"),
		        ("Ввод неверного пароля 3 раза", "Блокировка кнопки 'Войти' на 10 секунд", "Успешно (Система заблокирована)"),
		        ("Добавление пользователя с существующим логином", "Вывод ошибки 'Логин занят'", "Успешно (Ошибка выведена)"),
		        ("Проверка алгоритма валидации СНИЛС", "СНИЛС 112-233-445 95 должен быть валидным", "Ошибка (Алгоритм вернул false)"),
		        ("Запрос к API эмулятора (localhost:8080)", "Получение списка JSON данных", "Провал (Связь с сервером отсутствует)")
	        };

	        Services.WordDocumentService.GenerateTestCaseReport("ТестКейс.docx", testCases);
        }
    }
}