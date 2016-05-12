using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace Client
{
    /// <summary>
    /// Used to hide/show information based on user role
    /// </summary>
    public class CustomProviderConverter
    {
        public CustomProviderConverter()
        {
        }

        public object Convert(object value, Type targetType,
            object parameter, string language)
        {
            var role = (string)value;
            if (role == "Administrator" || role == "Physician")
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
