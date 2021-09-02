using System;
using System.Globalization;
using Xamarin.Forms;
using Xamarin.Essentials;
using Xamarin.Forms.Xaml;

namespace RealEstateApp.Converters
{
	public class HeightchangeConverter : IValueConverter, IMarkupExtension
	{
		public object ProvideValue(IServiceProvider serviceProvider)
		{
			return this;
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			double heightchange = (double)value;
			if (heightchange == 0f)
			{
				return string.Empty;
			}
			else if (heightchange < 0f)
			{
				return " " + heightchange.ToString("N2") + "m";
			}
			else
			{
				return " +" + heightchange.ToString("N2") + "m";
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value;
		}
	}
}