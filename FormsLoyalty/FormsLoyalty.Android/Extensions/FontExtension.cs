using System;
using Android.Graphics;
using Xamarin.Forms;

namespace FormsLoyalty.Droid.Extensions
{
    internal static class FontExtension
    {
        internal static Typeface ToTypeFace(this string fontfamily, FontAttributes attr = FontAttributes.None)
        {
            fontfamily = fontfamily ?? String.Empty;
            var style = ToTypefaceStyle(attr);
            return Typeface.Create(fontfamily, style);
        }

        public static TypefaceStyle ToTypefaceStyle(FontAttributes attrs)
        {
            var style = TypefaceStyle.Normal;
            if ((attrs & (FontAttributes.Bold | FontAttributes.Italic)) == (FontAttributes.Bold | FontAttributes.Italic))
                style = TypefaceStyle.BoldItalic;
            else if ((attrs & FontAttributes.Bold) != 0)
                style = TypefaceStyle.Bold;
            else if ((attrs & FontAttributes.Italic) != 0)
                style = TypefaceStyle.Italic;
            return style;
        }
    }
}

