using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Client
{
    /// <summary>
    /// Used to hide/show information based on if user role is a Nurse
    /// </summary>
    public class CustomIsNurseConverter : IValueConverter
    {
        public CustomIsNurseConverter()
        {
        }

        public object Convert(object value, Type targetType,
            object parameter, string language)
        {
            var role = (string)value;
            if (role == "Nurse")
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
