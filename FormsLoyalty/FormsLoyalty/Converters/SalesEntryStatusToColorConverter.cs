using LSRetail.Omni.Domain.DataModel.Base.SalesEntries;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace FormsLoyalty.Converters
{
    class SalesEntryStatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((SalesEntryStatus)Enum.Parse((typeof(SalesEntryStatus)), value.ToString(), false))
            {
                case SalesEntryStatus.Created:
                    return Color.FromHex("#06A9B1");
                case SalesEntryStatus.Pending:
                    return Color.FromHex("#E2BF79");
                case SalesEntryStatus.Processing:
                    return Color.FromHex("#858585");
                case SalesEntryStatus.Complete:
                    return Color.FromHex("#080C0C");
                case SalesEntryStatus.Canceled:
                    return Color.FromHex("#B80606");
                default:
                    return Color.FromHex("#06A9B1");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
