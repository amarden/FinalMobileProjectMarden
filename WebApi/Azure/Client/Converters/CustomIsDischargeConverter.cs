using Client.ClientObjects;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Client
{
    /// <summary>
    /// Used to hide/show the discharge button on the Patient detail page
    /// </summary>
    public class CustomIsDischargeConverter : IValueConverter
    {
        public CustomIsDischargeConverter()
        {
        }

        public object Convert(object value, Type targetType,
            object parameter, string language)
        {
            var status = (string)value;
            if (status == "discharge")
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, string language)
        {
            return null;
        }
    }
}
