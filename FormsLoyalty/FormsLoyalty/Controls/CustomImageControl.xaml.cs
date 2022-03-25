using FFImageLoading.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FormsLoyalty.Controls
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CustomImageControl : ContentView
	{
		public readonly static BindableProperty ImageHeightProperty = BindableProperty.Create
			(nameof(ImageHeight), typeof(double), typeof(CustomImageControl), default(double),BindingMode.TwoWay);

		public readonly static BindableProperty ImageWeightProperty = BindableProperty.Create
			(nameof(ImageWeight), typeof(double), typeof(CustomImageControl), default(double), BindingMode.TwoWay);

		public readonly static BindableProperty UriImageSourceProperty = BindableProperty.Create
			(nameof(UriImageSource), typeof(string), typeof(CustomImageControl), default(string), BindingMode.TwoWay);


		public double ImageHeight
		{
			get { return (double)GetValue(ImageHeightProperty); }
			set { SetValue(ImageHeightProperty, value); }
		}

		public double ImageWeight
		{
			get { return (double)GetValue(ImageWeightProperty); }
			set { SetValue(ImageWeightProperty, value); }
		}

		public string UriImageSource
		{
			get { return (string)GetValue(UriImageSourceProperty); }
			set { SetValue(UriImageSourceProperty, value); }
		}

		public CustomImageControl ()
		{
			InitializeComponent ();
			img.SetBinding(CachedImage.SourceProperty, new Binding(nameof(UriImageSource), BindingMode.TwoWay, source: this));
		}
	}
}