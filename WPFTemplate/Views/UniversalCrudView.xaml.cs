using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace WPFTemplate.Views
{
	public partial class UniversalCrudView : UserControl
	{
		public UniversalCrudView()
		{
			InitializeComponent();
		}
		
		private void DataGridAutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
		{
			if (e.PropertyName == "Id") 
			{
				e.Cancel = true;
				return;
			}
			
			var type = e.PropertyType;
			if (type != typeof(string) && type.IsClass || typeof(IEnumerable).IsAssignableFrom(type) && type != typeof(string))
			{
				e.Cancel = true;
				return;
			}
			
			if (e.PropertyName == "Picture")
			{
				var templateColumn = new DataGridTemplateColumn { Header = "Фото товара" };
				
				var imageFactory = new FrameworkElementFactory(typeof(Image));
				imageFactory.SetValue(HeightProperty, 50.0);
				imageFactory.SetValue(WidthProperty, 50.0);
				imageFactory.SetValue(Image.StretchProperty, System.Windows.Media.Stretch.Uniform);
				
				var binding = new System.Windows.Data.Binding("Picture")
				{
					Converter = new Helpers.ImageConverter()
				};
				imageFactory.SetBinding(Image.SourceProperty, binding);
				
				templateColumn.CellTemplate = new DataTemplate { VisualTree = imageFactory };
				
				e.Column = templateColumn;
				return; 
			}

			e.Column.Header = e.PropertyName switch
			{
				"Name" => "Наименование",
				"ProductCategoryId" => "ID Категории",
				"ProductManufacturerId" => "ID Производителя",
				_ => e.Column.Header
			};
		}
	}
}