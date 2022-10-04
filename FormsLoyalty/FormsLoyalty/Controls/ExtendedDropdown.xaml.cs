using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FormsLoyalty.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ExtendedDropdown : Grid
    {
        /// <summary>
        /// Identifies the <see cref="Title"/> bindable property.
        /// </summary>
        public static readonly BindableProperty TitleProperty =
            BindableProperty.Create(nameof(Title), typeof(string), typeof(ExtendedDropdown), default(string), BindingMode.OneWay);

        /// <summary>
        /// Identifies the <see cref="Title"/> bindable property.
        /// </summary>
        public static readonly BindableProperty PlaceholderTextProperty =
            BindableProperty.Create(nameof(PlaceholderText), typeof(string), typeof(ExtendedDropdown), default(string), BindingMode.TwoWay);

        /// <summary>
        /// Identifies the <see cref="ItemSource"/> bindable property.
        /// </summary>
        public static readonly BindableProperty ItemSourceProperty =
            BindableProperty.Create(nameof(ItemSource), typeof(IList), typeof(ExtendedDropdown), default(IList), BindingMode.TwoWay);

        /// <summary>
        /// Identifies the <see cref="SelectedItem"/> bindable property.
        /// </summary>
        public static readonly BindableProperty SelectedItemProperty =
            BindableProperty.Create(nameof(SelectedItem), typeof(object), typeof(ExtendedDropdown), null, BindingMode.TwoWay);

        /// <summary>
        /// Identifies the <see cref="Title"/> bindable property.
        /// </summary>
        public static readonly BindableProperty ErrorTextProperty =
            BindableProperty.Create(nameof(ErrorText), typeof(string), typeof(ExtendedDropdown), default(string), BindingMode.TwoWay);

        /// <summary>
        /// Identifies the <see cref="DisplayMemberPath"/> bindable property.
        /// </summary>
        public static readonly BindableProperty DisplayMemberPathProperty =
            BindableProperty.Create(nameof(DisplayMemberPath), typeof(string), typeof(ExtendedDropdown), default(string), BindingMode.OneWay);

        /// <summary>
        /// Identifies the <see cref="CanEdit"/> bindable property.
        /// </summary>
        public static readonly BindableProperty CanEditProperty =
            BindableProperty.Create(nameof(CanEdit), typeof(bool), typeof(ExtendedDropdown), false, BindingMode.OneWay);

        /// <summary>
        /// Identifies the <see cref="OnLoadShowDropdown"/> bindable property.
        /// </summary>
        public static readonly BindableProperty OnLoadShowDropdownProperty =
            BindableProperty.Create(nameof(OnLoadShowDropdown), typeof(bool), typeof(ExtendedDropdown), false, BindingMode.TwoWay);

        /// <summary>
        /// set sub-title for the control
        /// </summary>
        public static readonly BindableProperty IconProperty =
            BindableProperty.Create(nameof(Icon), typeof(string), typeof(ExtendedDropdown), FormsLoyalty.Resources.FontAwesomeIcons.ChevronDown, BindingMode.TwoWay);

        /// <summary>
        /// set sub-title for the control
        /// </summary>
        public static readonly BindableProperty IsErrorPoperty =
            BindableProperty.Create(nameof(IsError), typeof(bool), typeof(ExtendedDropdown), default(bool), BindingMode.TwoWay);

        /// <summary>
        /// set sub-title for the control
        /// </summary>
        public static readonly BindableProperty IsFontImagePoperty =
            BindableProperty.Create(nameof(IsFontImage), typeof(bool), typeof(ExtendedDropdown), true, BindingMode.TwoWay);


        /// <summary>
        /// set sub-title for the control
        /// </summary>
        public static readonly BindableProperty TitleStyleProperty =
            BindableProperty.Create(nameof(TitleStyle), typeof(Style), typeof(ExtendedDropdown), (Style)Application.Current.Resources["TitleStyle"], BindingMode.TwoWay);

        /// <summary>
        /// set sub-title for the control
        /// </summary>
        public static readonly BindableProperty FrameStyleProperty =
            BindableProperty.Create(nameof(FrameStyle), typeof(Style), typeof(ExtendedDropdown), (Style)Application.Current.Resources["FrameStyle"], BindingMode.TwoWay);

        public static readonly BindableProperty InputViewStyleProperty =
           BindableProperty.Create(nameof(InputViewStyle), typeof(Style), typeof(ExtendedDropdown), (Style)Application.Current.Resources["ExtendedDropDownInputViewStyle"], BindingMode.TwoWay);

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
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
        public object SelectedItem
        {
            get { return (object)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }
        public bool IsError
        {
            get { return (bool)GetValue(IsErrorPoperty); }
            set { SetValue(IsErrorPoperty, value); }
        }
        public bool IsFontImage
        {
            get { return (bool)GetValue(IsFontImagePoperty); }
            set { SetValue(IsFontImagePoperty, value); }
        }

        public Style TitleStyle
        {
            get { return (Style)GetValue(TitleStyleProperty); }
            set { SetValue(TitleStyleProperty, value); }
        }
        public Style InputViewStyle
        {
            get { return (Style)GetValue(InputViewStyleProperty); }
            set { SetValue(InputViewStyleProperty, value); }
        }
        public Style FrameStyle
        {
            get { return (Style)GetValue(FrameStyleProperty); }
            set { SetValue(FrameStyleProperty, value); }
        }

        [TypeConverter(typeof(ImageSource))]
        public string Icon
        {
            get { return (string)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }
        public IList ItemSource
        {
            get { return (IList)GetValue(ItemSourceProperty); }
            set { SetValue(ItemSourceProperty, value); }
        }
        public string DisplayMemberPath
        {
            get { return (string)GetValue(DisplayMemberPathProperty); }
            set { SetValue(DisplayMemberPathProperty, value); }
        }
        public bool CanEdit
        {
            get { return (bool)GetValue(CanEditProperty); }
            set { SetValue(CanEditProperty, value); }
        }

        public bool OnLoadShowDropdown
        {
            get { return (bool)GetValue(OnLoadShowDropdownProperty); }
            set { SetValue(OnLoadShowDropdownProperty, value); }
        }

        #region Events
        public event EventHandler<string> TextChanged;
        public ICommand TextChangeCommand { get; set; }
        #endregion

        public ExtendedDropdown()
        {
            InitializeComponent();

            dropdown.SetBinding(DropdownView.ShowDropdownProperty, new Binding(nameof(OnLoadShowDropdown), BindingMode.TwoWay, source: this));
        }

        private void dropdown_TextChanged(object sender, string searchText)
        {
            TextChangeCommand?.Execute(searchText);
            TextChanged?.Invoke(sender, searchText);
        }

        private void LifecycleEffect_Loaded(object sender, EventArgs e)
        {
            dropdown.ShowDropdown = OnLoadShowDropdown;
        }

        void TapGestureRecognizer_Tapped(System.Object sender, System.EventArgs e)
        {
            dropdown.ShowDropdown = !dropdown.ShowDropdown;
        }
    }
}