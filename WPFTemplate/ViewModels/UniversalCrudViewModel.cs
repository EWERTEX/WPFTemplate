using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using Microsoft.EntityFrameworkCore;
using WPFTemplate.Helpers;

namespace WPFTemplate.ViewModels
{
	public class UniversalCrudViewModel<T> : BaseCrudViewModel where T : class
	{
		private readonly DbContext? _context;

		public ObservableCollection<T> Items { get; set; }
		public ICollectionView ItemsView { get; }

		private string _searchText = string.Empty;

		public string SearchText
		{
			get => _searchText;
			set
			{
				_searchText = value;
				OnPropertyChanged();
				ItemsView.Refresh();
			}
		}

		private T? _selectedItem;

		public T? SelectedItem
		{
			get => _selectedItem;
			set
			{
				_selectedItem = value;
				OnPropertyChanged();
			}
		}

		public RelayCommand SaveChangesCommand { get; }
		public RelayCommand DeleteCommand { get; }

		public UniversalCrudViewModel(DbContext context)
		{
			_context = context;

			_context.Set<T>().Load();

			Items = _context.Set<T>().Local.ToObservableCollection();

			ItemsView = CollectionViewSource.GetDefaultView(Items);
			ItemsView.Filter = DynamicSearchFilter;

			SaveChangesCommand = new RelayCommand(SaveChanges);
			DeleteCommand = new RelayCommand(DeleteRow, _ => SelectedItem != null);
		}

		public UniversalCrudViewModel(IEnumerable<T> dummyData)
		{
			Items = new ObservableCollection<T>(dummyData);
			ItemsView = CollectionViewSource.GetDefaultView(Items);
			ItemsView.Filter = DynamicSearchFilter;

			SaveChangesCommand = new RelayCommand(_ => MessageBox.Show("БД не подключена! Это тестовый режим."));
			DeleteCommand = new RelayCommand(DeleteRow, _ => SelectedItem != null);
		}

		private bool DynamicSearchFilter(object obj)
		{
			if (string.IsNullOrWhiteSpace(SearchText)) return true;

			var properties = obj.GetType().GetProperties();
			return properties.Select(prop => prop.GetValue(obj)).OfType<object>().Any(value =>
				value.ToString().Contains(SearchText, StringComparison.OrdinalIgnoreCase));
		}

		private void SaveChanges(object? parameter)
		{
			if (_context == null) return;
			try
			{
				var modifiedEntries = _context.ChangeTracker.Entries()
					.Where(e => e.State is EntityState.Added or EntityState.Modified);

				foreach (var entry in modifiedEntries)
				{
					var entityType = entry.Entity.GetType();

					if (!entityType.Name.Contains("User")) continue;
					var login = entityType.GetProperty("Login")?.GetValue(entry.Entity)?.ToString() ?? "";
					var password = entityType.GetProperty("Password")?.GetValue(entry.Entity)?.ToString() ?? "";
							
					if (!ValidationHelper.IsValidEmail(login))
					{
						System.Windows.MessageBox.Show($"Ошибка: Неверный формат Email у пользователя {login}!",
							"Валидация", System.Windows.MessageBoxButton.OK,
							System.Windows.MessageBoxImage.Warning);
						return;
					}

					if (ValidationHelper.IsValidPassword(password)) continue;
					
					MessageBox.Show(
						$"Ошибка: Пароль для {login} слишком простой! Нужно: от 8 символов, Заглавная, цифра, спецсимвол.",
						"Валидация", MessageBoxButton.OK,
						MessageBoxImage.Warning);
					return;

					/*
					if (entityType.Name.Contains("Product")) {
						var price = (decimal)(entityType.GetProperty("Price")?.GetValue(entry.Entity) ?? 0m);
						if (price <= 0) { MessageBox.Show("Цена должна быть больше нуля!"); return; }
					}
					*/
				}
					
				_context.SaveChanges();
				MessageBox.Show("Изменения успешно сохранены в базу данных!", "Успех",
					MessageBoxButton.OK, MessageBoxImage.Information);
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Ошибка сохранения: {ex.InnerException?.Message ?? ex.Message}",
					"Ошибка БД", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void DeleteRow(object? parameter)
		{
			if (SelectedItem != null)
			{
				Items.Remove(SelectedItem);
			}
		}
	}
}