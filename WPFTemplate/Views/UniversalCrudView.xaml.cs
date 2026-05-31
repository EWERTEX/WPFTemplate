using System.Windows.Controls;

namespace WPFTemplate.Views
{
	public partial class UniversalCrudView : UserControl
	{
		public UniversalCrudView()
		{
			InitializeComponent();
		}
		
		private void DataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
		{
			if (e.PropertyName == "Id" || e.PropertyName.Contains("Id")) 
			{
				e.Cancel = true;
				return;
			}

			e.Column.Header = e.PropertyName switch
			{
				"Name" => "Наименование / ФИО",
				"Login" => "Логин пользователя",
				"Password" => "Пароль",
				"Price" => "Цена (руб)",
				"Article" => "Артикул",
				_ => e.Column.Header
			};
		}
	}
}