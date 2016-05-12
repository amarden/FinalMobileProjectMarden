using Client.ClientObjects;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.ClientObjects;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Client
{
    /// <summary>
    /// Used to hide/show procedures basd on the user role
    /// </summary>
    public class CustomProcedureConverter : IValueConverter
    {
        public Visibility TrueValue { get; set; }
        public Visibility FalseValue { get; set; }

        public CustomProcedureConverter()
        {
            // set defaults
            TrueValue = Visibility.Visible;
            FalseValue = Visibility.Collapsed;
        }

        public object Convert(object value, Type targetType,
            object parameter, string language)
        {
            var rules = (VisibilityRules)value;
            if (rules.Completed == true)
            {
                return Visibility.Collapsed;
            }
            else if(rules.procedureRole != rules.userRole)
            {
                return Visibility.Collapsed;
            }
            else
            {
                return Visibility.Visible;
            }
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, string language)
        {
            if (Equals(value, TrueValue))
                return true;
            if (Equals(value, FalseValue))
                return false;
            return null;
        }
    }
}
