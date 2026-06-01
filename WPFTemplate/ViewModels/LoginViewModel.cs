using System.Windows.Threading;
using WPFTemplate.Helpers;

namespace WPFTemplate.ViewModels
{
    public class LoginViewModel : ObservableObject
    {
        private string _login = string.Empty;
        private string _errorMessage = string.Empty;
        private int _failedAttempts = 0;
        private readonly DispatcherTimer _lockoutTimer;
        private int _lockoutSeconds = 10;
        
        private bool _isCaptchaVisible = false;
        private string _captchaText = string.Empty;
        private string _userCaptchaInput = string.Empty;
        
        private bool _isLoginEnabled = true;

        public Action? CloseAction { get; set; }

        public string Login { get => _login; set { _login = value; OnPropertyChanged(); } }
        public string ErrorMessage { get => _errorMessage; set { _errorMessage = value; OnPropertyChanged(); } }
        
        public bool IsCaptchaVisible { get => _isCaptchaVisible; set { _isCaptchaVisible = value; OnPropertyChanged(); } }
        public string CaptchaText { get => _captchaText; set { _captchaText = value; OnPropertyChanged(); } }
        public string UserCaptchaInput { get => _userCaptchaInput; set { _userCaptchaInput = value; OnPropertyChanged(); } }
        public bool IsLoginEnabled { get => _isLoginEnabled; set { _isLoginEnabled = value; OnPropertyChanged(); } }

        public RelayCommand LoginCommand { get; }

        public LoginViewModel()
        {
            LoginCommand = new RelayCommand(ExecuteLogin, _ => IsLoginEnabled);
            
            _lockoutTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _lockoutTimer.Tick += OnLockoutTimerTick;
        }

        private void ExecuteLogin(object? parameter)
        {
            var passwordBox = parameter as System.Windows.Controls.PasswordBox;
            var password = passwordBox?.Password ?? string.Empty;

            if (string.IsNullOrWhiteSpace(Login) || string.IsNullOrWhiteSpace(password))
            {
                HandleFailedAttempt("Заполните все поля!");
                return;
            }
            
            if (IsCaptchaVisible && !UserCaptchaInput.Equals(CaptchaText, StringComparison.CurrentCultureIgnoreCase))
            {
                HandleFailedAttempt("Неверная капча!");
                return;
            }

            /* --------------------------------------------------------------------------------
            using (var db = new ApplicationDbContext())
            {
                var user = db.Users.FirstOrDefault(u => u.Login == Login && u.Password == password);
                if (user != null)
                {
                    SessionContext.CurrentUser = user;
                    SessionContext.RoleId = user.Role.Name;
                    
                    var mainWindow = new Views.MainWindow();
                    mainWindow.Show();
                    
                    CloseAction?.Invoke();
                    return;
                }
            }
            -------------------------------------------------------------------------------- */
            
            HandleFailedAttempt("Неверный логин или пароль!");
        }

        private void HandleFailedAttempt(string message)
        {
            _failedAttempts++;
            ErrorMessage = message;

            switch (_failedAttempts)
            {
	            case >= 3:
		            StartLockout();
		            break;
	            
	            case >= 1:
		            IsCaptchaVisible = true;
		            GenerateCaptcha();
		            break;
            }
        }

        private void GenerateCaptcha()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            CaptchaText = new string(Enumerable.Repeat(chars, 4).Select(s => s[random.Next(s.Length)]).ToArray());
            UserCaptchaInput = string.Empty;
        }

        private void StartLockout()
        {
            IsLoginEnabled = false;
            _lockoutSeconds = 10;
            ErrorMessage = $"Система заблокирована. Ожидайте: {_lockoutSeconds} сек.";
            _lockoutTimer.Start();
        }

        private void OnLockoutTimerTick(object? sender, EventArgs e)
        {
            _lockoutSeconds--;
            ErrorMessage = $"Система заблокирована. Ожидайте: {_lockoutSeconds} сек.";

            if (_lockoutSeconds <= 0)
            {
                _lockoutTimer.Stop();
                IsLoginEnabled = true;
                ErrorMessage = string.Empty;
                _failedAttempts = 0;
                GenerateCaptcha();
            }
        }
    }
}