using System;
using System.Text.RegularExpressions;
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
          
            //var context = (type as ImageSource).BindingContext;
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
                    try
                    {
                        Span<byte> buffer = new Span<byte>(new byte[img.Length]);
                        var IsBase64 = System.Convert.TryFromBase64String(img, buffer, out int bytesParsed);
                        if (IsBase64)
                        {
                            byte[] Base64Stream = System.Convert.FromBase64String(img);
                            imgSource = ImageSource.FromStream(() => new MemoryStream(Base64Stream));
                        }
                        else
                            imgSource = ImageSource.FromFile("noimage.png");

                    }
                    catch (Exception ex)
                    {

                        imgSource = ImageSource.FromFile("noimage.png");
                    }

                    
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

    public class DefaultImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is string img && !string.IsNullOrEmpty(img))
            {
               
                   return img.Equals("noimage.png");
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
