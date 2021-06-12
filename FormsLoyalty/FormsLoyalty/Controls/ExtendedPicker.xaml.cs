using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FormsLoyalty.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ExtendedPicker : ContentView
    {

		public static readonly BindableProperty TitleProperty = BindableProperty.Create(
		nameof(Title),
		typeof(string),
		typeof(ExtendedPicker),
		null
		);

		public static readonly BindableProperty PlaceholderTextProperty = BindableProperty.Create(
		nameof(PlaceholderText),
		typeof(string),
		typeof(ExtendedPicker),
		null
		);

		public static readonly BindableProperty DisplayMemberPathProperty = BindableProperty.Create(
		nameof(DisplayMemberPath),
		typeof(string),
		typeof(ExtendedPicker),
		null,
		BindingMode.TwoWay);

		public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
	   propertyName: nameof(ItemsSource),
	   returnType: typeof(IList),
	   declaringType: typeof(ExtendedPicker),
	   default(IList),
	   BindingMode.TwoWay
	   );

		public static readonly BindableProperty SelectedItemProperty =
	   BindableProperty.Create(
		   nameof(SelectedItem),
		   typeof(object),
		   typeof(ExtendedPicker),
		   null,
		   BindingMode.TwoWay
		   );

		/// <summary>
		/// Identifies the <see cref="TextColor"/> bindable property.
		/// </summary>
		public static readonly BindableProperty TextColorProperty =
			BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(ExtendedPicker), Color.Black, BindingMode.OneWay, null, null);


		public static readonly BindableProperty ImageProperty =
			BindableProperty.Create(nameof(Image), typeof(string), typeof(ExtendedPicker), string.Empty);

		public static readonly BindableProperty ImageHeightProperty =
			BindableProperty.Create(nameof(ImageHeight), typeof(int), typeof(ExtendedPicker), 20);

		public static readonly BindableProperty ImageWidthProperty =
			BindableProperty.Create(nameof(ImageWidth), typeof(int), typeof(ExtendedPicker), 20);


		/// <summary>
		/// Gets or sets the foreground color of the control
		/// </summary>
		/// <seealso cref="Text"/>
		public Color TextColor
		{
			get { return (Color)GetValue(TextColorProperty); }
			set { SetValue(TextColorProperty, value); }
		}

		public string Title
		{
			get => (string)GetValue(TitleProperty);
			set => SetValue(TitleProperty, value);
		}
		public string PlaceholderText
		{
			get => (string)GetValue(PlaceholderTextProperty);
			set => SetValue(PlaceholderTextProperty, value);
		}
		public string DisplayMemberPath
		{
			get => (string)GetValue(DisplayMemberPathProperty);
			set
			{
				SetValue(DisplayMemberPathProperty, value);
				picker.ItemDisplayBinding = new Binding(nameof(value),BindingMode.TwoWay,source:this);
			}
		}
		public IList ItemsSource
		{
			get { return (IList)GetValue(ItemsSourceProperty); }
			set { SetValue(ItemsSourceProperty, value); }
		}

		public object SelectedItem
		{
			get { return GetValue(SelectedItemProperty); }
			set { SetValue(SelectedItemProperty, value); }
		}
		public string Image
		{
			get { return (string)GetValue(ImageProperty); }
			set { SetValue(ImageProperty, value); }
		}
		public int ImageHeight
		{
			get { return (int)GetValue(ImageHeightProperty); }
			set { SetValue(ImageHeightProperty, value); }
		}
		public int ImageWidth
		{
			get { return (int)GetValue(ImageWidthProperty); }
			set { SetValue(ImageWidthProperty, value); }
		}

		public ExtendedPicker()
        {
            InitializeComponent();
			picker.SetBinding(CustomPicker.TitleProperty, new Binding(nameof(Title), BindingMode.TwoWay, source: this));
			picker.SetBinding(CustomPicker.ItemsSourceProperty, new Binding(nameof(ItemsSource), BindingMode.TwoWay, source: this));
			picker.SetBinding(CustomPicker.ImageHeightProperty, new Binding(nameof(ImageHeight), BindingMode.TwoWay, source: this));
			picker.SetBinding(CustomPicker.ImageWidthProperty, new Binding(nameof(ImageWidth), BindingMode.TwoWay, source: this));
			picker.SetBinding(CustomPicker.ImageProperty, new Binding(nameof(Image), BindingMode.TwoWay, source: this));
			picker.SetBinding(CustomPicker.TextColorProperty, new Binding(nameof(TextColor), BindingMode.TwoWay, source: this));
			picker.SetBinding(CustomPicker.SelectedItemProperty, new Binding(nameof(SelectedItem), BindingMode.TwoWay, source: this));
			//picker.SetBinding(CustomPicker., new Binding(nameof(SelectedItem), BindingMode.TwoWay, source: this));
		}
    }
}