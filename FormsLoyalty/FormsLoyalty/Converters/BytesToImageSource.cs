using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Xamarin.Forms;

namespace FormsLoyalty.Converters
{
    public class BytesToImageSource : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ImageSource imgSource = null;

            if (value == null)
            {
                return null;
            }

            

            if (value is string img)
            {

                if ( img.Contains("noimage"))
                {
                    imgSource = ImageSource.FromFile("noimage.png");
                }
                else
                {
                    byte[] Base64Stream = System.Convert.FromBase64String(img);
                    imgSource = ImageSource.FromStream(() => new MemoryStream(Base64Stream));
                }
                    

                return imgSource;



            }
            byte[] FileName = value as byte[];
            if (FileName != null)
            {
               

               // var stream1 = new MemoryStream(FileName);
                imgSource = ImageSource.FromStream(() => new MemoryStream(FileName));
                return imgSource;
            }
            else
            {

                imgSource = ImageSource.FromFile("no_image.png");
                return imgSource;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
