using System.Collections.ObjectModel;
using WPFTemplate.Helpers;
using WPFTemplate.Models;

namespace WPFTemplate.ViewModels
{
    public class AdminViewModel : ObservableObject
    {
        private ObservableCollection<UserModel> _users = new();
        private UserModel? _selectedUser;
        private string _errorMessage = string.Empty;
        
        private string _formName = string.Empty;
        private string _formLogin = string.Empty;
        private string _formPassword = string.Empty;

        public ObservableCollection<UserModel> Users { get => _users; set { _users = value; OnPropertyChanged(); } }
        public string ErrorMessage { get => _errorMessage; set { _errorMessage = value; OnPropertyChanged(); } }
        
        public string FormName { get => _formName; set { _formName = value; OnPropertyChanged(); } }
        public string FormLogin { get => _formLogin; set { _formLogin = value; OnPropertyChanged(); } }
        public string FormPassword { get => _formPassword; set { _formPassword = value; OnPropertyChanged(); } }

        public UserModel? SelectedUser 
        { 
            get => _selectedUser; 
            set 
            { 
                _selectedUser = value; 
                OnPropertyChanged();

                if (_selectedUser == null) return;
                
                FormName = _selectedUser.Name;
                FormLogin = _selectedUser.Login;
                FormPassword = _selectedUser.Password;
                ErrorMessage = string.Empty;
            } 
        }

        public RelayCommand AddCommand { get; }
        public RelayCommand EditCommand { get; }
        public RelayCommand ClearFormCommand { get; }

        public AdminViewModel()
        {
            AddCommand = new RelayCommand(ExecuteAdd);
            EditCommand = new RelayCommand(ExecuteEdit, _ => SelectedUser != null);
            ClearFormCommand = new RelayCommand(_ => ClearForm());

            LoadData();
        }

        private void LoadData()
        {
            // using (var db = new ApplicationDbContext()) {
            //     Users = new ObservableCollection<User>(db.Users.Include(u => u.Role).ToList());
            // }

            Users = new ObservableCollection<UserModel>
            {
                new UserModel { Id = 1, Name = "Тестовый Админ", Login = "admin", Password = "123", RoleName = "Администратор" },
                new UserModel { Id = 2, Name = "Иванов Иван", Login = "user1", Password = "123", RoleName = "Клиент" }
            };
        }

        private void ExecuteAdd(object? parameter)
        {
            if (string.IsNullOrWhiteSpace(FormName) || string.IsNullOrWhiteSpace(FormLogin))
            {
                ErrorMessage = "Заполните ФИО и Логин!";
                return;
            }
            
            if (!ValidationHelper.IsValidEmail(FormLogin))
            {
	            ErrorMessage = "Ошибка: Неверный формат Email (Логина)!";
	            return;
            }
            
            if (!ValidationHelper.IsValidPassword(FormPassword))
            {
	            ErrorMessage = "Ошибка: Пароль должен быть от 8 символов, содержать заглавную букву, цифру и спецсимвол!";
	            return;
            }
            
            if (Users.Any(u => u.Login.ToLower() == FormLogin.ToLower()))
            {
                ErrorMessage = "Ошибка: Пользователь с таким логином уже существует!";
                return;
            }
            
            var newUser = new UserModel
            {
                Id = Users.Count > 0 ? Users.Max(u => u.Id) + 1 : 1,
                Name = FormName,
                Login = FormLogin,
                Password = FormPassword,
                RoleName = "Клиент"
            };

            Users.Add(newUser);
            ErrorMessage = "Пользователь успешно добавлен!";
            ClearForm();
        }

        private void ExecuteEdit(object? parameter)
        {
            if (SelectedUser == null) return;
            
            if (Users.Any(u => u.Login.ToLower() == FormLogin.ToLower() && u.Id != SelectedUser.Id))
            {
                ErrorMessage = "Ошибка: Этот логин уже занят другим пользователем!";
                return;
            }
            
            SelectedUser.Name = FormName;
            SelectedUser.Login = FormLogin;
            SelectedUser.Password = FormPassword;
            
            ErrorMessage = "Данные успешно изменены!";
            
            var index = Users.IndexOf(SelectedUser);
            Users.RemoveAt(index);
            Users.Insert(index, SelectedUser);
            SelectedUser = Users[index];
        }

        private void ClearForm()
        {
            SelectedUser = null;
            FormName = string.Empty;
            FormLogin = string.Empty;
            FormPassword = string.Empty;
            ErrorMessage = string.Empty;
        }
    }
}