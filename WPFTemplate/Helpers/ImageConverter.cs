using System.Globalization;
using System.Windows.Data;

namespace WPFTemplate.Helpers
{
	public class ImageConverter : IValueConverter
	{
		public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
		{
			var imageName = value as string;
			return string.IsNullOrWhiteSpace(imageName) ? "/Resources/picture.png" : $"/Resources/{imageName}";
		}

		public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
			=> throw new NotImplementedException();
	}
}