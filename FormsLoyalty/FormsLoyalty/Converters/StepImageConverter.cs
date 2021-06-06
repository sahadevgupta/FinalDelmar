using FormsLoyalty.Controls.Stepper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace FormsLoyalty.Converters
{
    public class StepImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value.ToString() != "")
            {
                return value.ToString() == StepBarStatus.Completed.ToString() ? ImageSource.FromFile("check.png") : (object)"";
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
