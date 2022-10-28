using FormsLoyalty.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FormsLoyalty.Controls
{
    public partial class ExtendedEntry : ContentView
    {
        /// <summary>
        /// set title for the control
        /// </summary>
        public static readonly BindableProperty TitleProperty =
            BindableProperty.Create(nameof(Title), typeof(string), typeof(ExtendedEntry), default(string), BindingMode.OneWay);



        /// <summary>
        /// set sub-title for the control
        /// </summary>
        public static readonly BindableProperty SubTitleProperty =
            BindableProperty.Create(nameof(SubTitle), typeof(string), typeof(ExtendedEntry), default(string), BindingMode.OneWay);

        /// <summary>
        /// set sub-title for the control
        /// </summary>
        public static readonly BindableProperty TextProperty =
            BindableProperty.Create(nameof(Text), typeof(string), typeof(ExtendedEntry), default(string), BindingMode.TwoWay);

        /// <summary>
        /// set sub-title for the control
        /// </summary>
        public static readonly BindableProperty PlaceholderTextProperty =
            BindableProperty.Create(nameof(PlaceholderText), typeof(string), typeof(ExtendedEntry), default(string), BindingMode.OneWay);

        /// <summary>
        /// set sub-title for the control
        /// </summary>
        public static readonly BindableProperty ErrorTextProperty =
            BindableProperty.Create(nameof(ErrorText), typeof(string), typeof(ExtendedEntry), default(string), BindingMode.TwoWay);

        /// <summary>
        /// set sub-title for the control
        /// </summary>
        public static readonly BindableProperty FooterTextProperty =
            BindableProperty.Create(nameof(FooterText), typeof(string), typeof(ExtendedEntry), default(string), BindingMode.TwoWay);

        /// <summary>
        /// set sub-title for the control
        /// </summary>
        public static readonly BindableProperty IconProperty =
            BindableProperty.Create(nameof(Icon), typeof(string), typeof(ExtendedEntry), default(string), BindingMode.TwoWay);

        /// <summary>
        /// set sub-title for the control
        /// </summary>
        public static readonly BindableProperty KeyboardTypeProperty =
            BindableProperty.Create(nameof(KeyboardType), typeof(Keyboard), typeof(ExtendedEntry), default(Keyboard), BindingMode.TwoWay, propertyChanged: OnKeyboardTypeChanged);


        /// <summary>
        /// set sub-title for the control
        /// </summary>
        public static readonly BindableProperty KeyboardReturnTypeProperty =
            BindableProperty.Create(nameof(KeyboardReturnType), typeof(ReturnType), typeof(ExtendedEntry), default(ReturnType), BindingMode.TwoWay);

        /// <summary>
        /// set sub-title for the control
        /// </summary>
        public static readonly BindableProperty MaxLengthProperty =
            BindableProperty.Create(nameof(MaxLength), typeof(int), typeof(ExtendedEntry), int.MaxValue, BindingMode.OneWay);

        /// <summary>
        /// Identifies the <see cref="IsEntryFocus"/> bindable property.
        /// </summary>
        public static readonly BindableProperty IsEntryFocusProperty =
            BindableProperty.Create(nameof(IsEntryFocus), typeof(bool), typeof(ExtendedEntry), false, BindingMode.TwoWay);

        public bool IsEntryFocus
        {
            get { return (bool)GetValue(IsEntryFocusProperty); }
            set { SetValue(IsEntryFocusProperty, value); }
        }

        private static void OnKeyboardTypeChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (newValue is Keyboard type && type == Keyboard.Numeric)
            {
                var control = bindable as ExtendedEntry;
                control.customentry.Behaviors.Add(new DecimalNumberValidationBehavior());
            }
        }

        private static void OnEntryFocusedChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (newValue is bool value)
            {
                if (value)
                {
                    var control = bindable as ExtendedEntry;
                    control.customentry.Focus();
                }

            }
        }

        /// <summary>
        /// set sub-title for the control
        /// </summary>
        public static readonly BindableProperty IsPasswordProperty =
            BindableProperty.Create(nameof(IsPassword), typeof(bool), typeof(ExtendedEntry), default(bool), BindingMode.TwoWay);


        /// <summary>
        /// set sub-title for the control
        /// </summary>
        public static readonly BindableProperty IsErrorProperty =
            BindableProperty.Create(nameof(IsError), typeof(bool), typeof(ExtendedEntry), default(bool), BindingMode.TwoWay);

        /// <summary>
        /// set sub-title for the control
        /// </summary>
        public static readonly BindableProperty IsFontImageProperty =
            BindableProperty.Create(nameof(IsFontImage), typeof(bool), typeof(ExtendedEntry), default(bool), BindingMode.TwoWay);

        /// <summary>
        /// Identifies the <see cref="OnLoadFocus"/> bindable property.
        /// </summary>
        public static readonly BindableProperty OnLoadFocusProperty =
            BindableProperty.Create(nameof(OnLoadFocus), typeof(bool), typeof(ExtendedEntry), false, BindingMode.TwoWay);


        /// <summary>
        /// set sub-title for the control
        /// </summary>
        public static readonly BindableProperty TitleStyleProperty =
            BindableProperty.Create(nameof(TitleStyle), typeof(Style), typeof(ExtendedEntry), (Style)Application.Current.Resources["TitleStyle"], BindingMode.TwoWay);

        /// <summary>
        /// set sub-title for the control
        /// </summary>
        public static readonly BindableProperty SubTitleStyleProperty =
            BindableProperty.Create(nameof(SubTitleStyle), typeof(Style), typeof(ExtendedEntry), default(Style), BindingMode.TwoWay);

        /// <summary>
        /// set sub-title for the control
        /// </summary>
        public static readonly BindableProperty FrameStyleProperty =
            BindableProperty.Create(nameof(FrameStyle), typeof(Style), typeof(ExtendedEntry), (Style)Application.Current.Resources["FrameStyle"], BindingMode.TwoWay);

        /// <summary>
        /// set sub-title for the control
        /// </summary>
        public static readonly BindableProperty FooterStyleProperty =
            BindableProperty.Create(nameof(FooterStyle), typeof(Style), typeof(ExtendedEntry), default(Style), BindingMode.TwoWay);

        /// <summary>
        /// set sub-title for the control
        /// </summary>
        public static readonly BindableProperty EntryStyleProperty =
            BindableProperty.Create(nameof(EntryStyle), typeof(Style), typeof(ExtendedEntry), (Style)Application.Current.Resources["MainEntryStyle"], BindingMode.TwoWay);

        /// <summary>
        /// set sub-title for the control
        /// </summary>
        public static readonly BindableProperty ShowScannerViewCommandProperty =
            BindableProperty.Create(nameof(ShowScannerViewCommand), typeof(ICommand), typeof(ExtendedEntry), default(ICommand), defaultBindingMode: BindingMode.TwoWay);

        public static readonly BindableProperty CommandParameterProperty =
           BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(ExtendedEntry), null, defaultBindingMode: BindingMode.TwoWay);


        public static readonly BindableProperty ReturnTypeCommandProperty =
            BindableProperty.Create(nameof(ReturnTypeCommand), typeof(ICommand), typeof(ExtendedEntry), default(ICommand), defaultBindingMode: BindingMode.TwoWay);

        public static readonly BindableProperty ReturnTypeCommandParameterProperty =
           BindableProperty.Create(nameof(ReturnTypeCommandParameter), typeof(object), typeof(ExtendedEntry), null, defaultBindingMode: BindingMode.TwoWay);


        public ICommand ShowScannerViewCommand
        {
            get { return (ICommand)GetValue(ShowScannerViewCommandProperty); }
            set { SetValue(ShowScannerViewCommandProperty, value); }
        }

        public object CommandParameter
        {
            get { return GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        public ICommand ReturnTypeCommand
        {
            get { return (ICommand)GetValue(ReturnTypeCommandProperty); }
            set { SetValue(ReturnTypeCommandProperty, value); }
        }

        public object ReturnTypeCommandParameter
        {
            get { return GetValue(ReturnTypeCommandParameterProperty); }
            set { SetValue(ReturnTypeCommandParameterProperty, value); }
        }

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }
        public string SubTitle
        {
            get { return (string)GetValue(SubTitleProperty); }
            set { SetValue(SubTitleProperty, value); }
        }
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        public string PlaceholderText
        {
            get { return (string)GetValue(PlaceholderTextProperty); }
            set { SetValue(PlaceholderTextProperty, value); }
        }
        public string ErrorText
        {
            get { return (string)GetValue(ErrorTextProperty); }
            set { SetValue(ErrorTextProperty, value); }
        }
        public string FooterText
        {
            get { return (string)GetValue(FooterTextProperty); }
            set { SetValue(FooterTextProperty, value); }
        }

        public Keyboard KeyboardType
        {
            get { return (Keyboard)GetValue(KeyboardTypeProperty); }
            set { SetValue(KeyboardTypeProperty, value); }
        }

        public ReturnType KeyboardReturnType
        {
            get { return (ReturnType)GetValue(KeyboardReturnTypeProperty); }
            set { SetValue(KeyboardReturnTypeProperty, value); }
        }
        public int MaxLength
        {
            get { return (int)GetValue(MaxLengthProperty); }
            set { SetValue(MaxLengthProperty, value); }
        }
        public bool IsPassword
        {
            get { return (bool)GetValue(IsPasswordProperty); }
            set { SetValue(IsPasswordProperty, value); }
        }

        public bool IsError
        {
            get { return (bool)GetValue(IsErrorProperty); }
            set { SetValue(IsErrorProperty, value); }
        }
        public bool IsFontImage
        {
            get { return (bool)GetValue(IsFontImageProperty); }
            set { SetValue(IsFontImageProperty, value); }
        }

        public bool OnLoadFocus
        {
            get { return (bool)GetValue(OnLoadFocusProperty); }
            set { SetValue(OnLoadFocusProperty, value); }
        }

        public Style TitleStyle
        {
            get { return (Style)GetValue(TitleStyleProperty); }
            set { SetValue(TitleStyleProperty, value); }
        }

        public Style SubTitleStyle
        {
            get { return (Style)GetValue(SubTitleStyleProperty); }
            set { SetValue(SubTitleStyleProperty, value); }
        }

        public Style EntryStyle
        {
            get { return (Style)GetValue(EntryStyleProperty); }
            set { SetValue(EntryStyleProperty, value); }
        }

        public Style FrameStyle
        {
            get { return (Style)GetValue(FrameStyleProperty); }
            set { SetValue(FrameStyleProperty, value); }
        }

        public Style FooterStyle
        {
            get { return (Style)GetValue(FooterStyleProperty); }
            set { SetValue(FooterStyleProperty, value); }
        }

        [TypeConverter(typeof(ImageSource))]
        public string Icon
        {
            get { return (string)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        public static void Execute(ICommand command)
        {
            if (command == null) return;
            if (command.CanExecute(null))
            {
                command.Execute(null);
            }
        }

        private static void OnTitlePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ExtendedEntry control = bindable as ExtendedEntry;
            if (control != null && control.CommandParameter == null)
                control.CommandParameter = newValue;
        }

        public event EventHandler<TextChangedEventArgs> OnEntryTextChanged;
        public ExtendedEntry()
        {
            InitializeComponent();

        }

        private void customentry_TextChanged(object sender, TextChangedEventArgs e)
        {
            OnEntryTextChanged?.Invoke(this, e);
        }
    }


}