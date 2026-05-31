using WPFTemplate.Helpers;

namespace WPFTemplate.Models
{
	public class UserModel : ObservableObject
	{
		private int _id;
		private string _name = string.Empty;
		private string _login = string.Empty;
		private string _password = string.Empty;
		private string _roleName = string.Empty;

		public int Id { get => _id; set { _id = value; OnPropertyChanged(); } }
		public string Name { get => _name; set { _name = value; OnPropertyChanged(); } }
		public string Login { get => _login; set { _login = value; OnPropertyChanged(); } }
		public string Password { get => _password; set { _password = value; OnPropertyChanged(); } }
		public string RoleName { get => _roleName; set { _roleName = value; OnPropertyChanged(); } }
	}
}