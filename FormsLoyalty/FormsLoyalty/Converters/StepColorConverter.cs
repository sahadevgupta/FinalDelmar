using FormsLoyalty.Controls.Stepper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace FormsLoyalty.Converters
{
    public class StepColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value.ToString() != "")
            {
                if (value.ToString() == StepBarStatus.Completed.ToString())
                {
                    return Color.FromHex("#127ABF");
                }
                else if (value.ToString() == StepBarStatus.InProgress.ToString())
                {
                    return Color.FromHex("#127ABF");
                }
                else
                {
                    return Color.Silver;
                }
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
