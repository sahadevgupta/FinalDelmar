using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using System.Collections;
using Xamarin.Forms.Internals;

namespace FormsLoyalty.Controls
{
    public class DropdownView : View
    {
        #region Properties
        /// <summary>
        /// Identifies the <see cref="ItemSource"/> bindable property.
        /// </summary>
        public static readonly BindableProperty ItemSourceProperty =
            BindableProperty.Create(nameof(ItemSource), typeof(IList), typeof(DropdownView), default(IList), BindingMode.TwoWay);

        /// <summary>
        /// Identifies the <see cref="PlaceholderText"/> bindable property.
        /// </summary>
        public static readonly BindableProperty PlaceholderTextProperty =
            BindableProperty.Create(nameof(PlaceholderText), typeof(string), typeof(DropdownView), default(string), BindingMode.OneWay);


        /// <summary>
        /// Identifies the <see cref="DisplayMemberPath"/> bindable property.
        /// </summary>
        public static readonly BindableProperty DisplayMemberPathProperty =
            BindableProperty.Create(nameof(DisplayMemberPath), typeof(string), typeof(DropdownView), default(string), BindingMode.OneWay);

        /// <summary>
        /// Identifies the <see cref="CanEdit"/> bindable property.
        /// </summary>
        public static readonly BindableProperty CanEditProperty =
            BindableProperty.Create(nameof(CanEdit), typeof(bool), typeof(DropdownView), true, BindingMode.OneWay);

        /// <summary>
        /// Identifies the <see cref="IsFocused"/> bindable property.
        /// </summary>
        public static readonly BindableProperty ShowDropdownProperty =
            BindableProperty.Create(nameof(ShowDropdown), typeof(bool), typeof(DropdownView), false, BindingMode.TwoWay);

        /// <summary>
        /// Identifies the <see cref="SelectedItem"/> bindable property.
        /// </summary>
        public static readonly BindableProperty SelectedItemProperty =
            BindableProperty.Create(nameof(SelectedItem), typeof(object), typeof(DropdownView), null, BindingMode.TwoWay);


        /// <summary>
        /// Identifies the <see cref="SelectedIndex"/> bindable property.
        /// </summary>
        public static readonly BindableProperty SelectedIndexProperty =
            BindableProperty.Create(nameof(SelectedIndex), typeof(int), typeof(DropdownView), -1, BindingMode.TwoWay);

        /// <summary>
        /// Identifies the <see cref="InputViewStyle"/> bindable property.
        /// </summary>
        public static readonly BindableProperty InputViewStyleProperty =
            BindableProperty.Create(nameof(InputViewStyle), typeof(Style), typeof(DropdownView), default(Style), BindingMode.TwoWay);


        /// <summary>
        /// Gets or sets the dropdown itemsource of the property that is displayed
        /// </summary>
        /// <value>
        /// The dropdown itemsource of the property that is displayed in
        /// the control. The default is null
        /// </value>
        public IList ItemSource
        {
            get { return (IList)GetValue(ItemSourceProperty); }
            set { SetValue(ItemSourceProperty, value); }
        }

        /// <summary>
        /// Gets or sets the placeholder of the property that is displayed for the control
        /// </summary>
        /// <value>
        /// The placeholder of the property that is displayed in
        /// the control. The default is an empty string ("")
        /// </value>
        public string PlaceholderText
        {
            get { return (string)GetValue(PlaceholderTextProperty); }
            set { SetValue(PlaceholderTextProperty, value); }
        }


        /// <summary>
        /// Gets or sets the name or path of the property that is displayed for each data item
        /// </summary>
        /// <value>
        /// the name or path of the property that is displayed for each data item in
        /// the control. The default is an empty string ("")
        /// </value>
        public string DisplayMemberPath
        {
            get { return (string)GetValue(DisplayMemberPathProperty); }
            set { SetValue(DisplayMemberPathProperty, value); }
        }

        /// <summary>
        /// Gets or sets the edit property of the control
        /// </summary>
        /// <value>
        /// The default is true
        /// </value>
        public bool CanEdit
        {
            get { return (bool)GetValue(CanEditProperty); }
            set { SetValue(CanEditProperty, value); }
        }

        /// <summary>
        /// Gets or sets the selected item for the control
        /// </summary>
        /// <value>
        ///  The default is null
        /// </value>
        public object SelectedItem
        {
            get { return (object)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        /// <summary>
        /// Gets or sets the selected index for the control
        /// </summary>
        /// <value>
        ///  The default is -1
        /// </value>
        public int SelectedIndex
        {
            get { return (int)GetValue(SelectedIndexProperty); }
            set { SetValue(SelectedIndexProperty, value); }
        }

        /// <summary>
        /// Gets or sets the input view style for the control
        /// </summary>
        public Style InputViewStyle
        {
            get { return (Style)GetValue(InputViewStyleProperty); }
            set { SetValue(InputViewStyleProperty, value); }
        }
        public bool ShowDropdown
        {
            get { return (bool)GetValue(ShowDropdownProperty); }
            set { SetValue(ShowDropdownProperty, value); }
        }
        #endregion

        #region Events
        public event EventHandler<string> TextChanged;
        public event EventHandler<ItemSelectedEventArgs> ItemSelected;
        public event EventHandler<SelectedIndexChangedEventArgs> SelectedIndexChanged;
        #endregion

        public void OnTextChanged(object arg)
        {
            TextChanged?.Invoke(this, arg.ToString());
        }

        public void OnItemSelected(object selectedItem)
        {
            UpdateSelectedIndex(selectedItem);
        }

        private static void OnSelectedIndexChanged(BindableObject bindable, object oldValue, object newValue)
        {
            DropdownView customDropDown = (DropdownView)bindable;
            customDropDown.UpdateSelectedItem();
            customDropDown.SelectedIndexChanged?.Invoke(bindable, new SelectedIndexChangedEventArgs((int)oldValue, (int)newValue));
        }

        private static void OnSelectedItemChanged(BindableObject bindable, object oldValue, object newValue)
        {
            //CustomDropDown customDropDown = (CustomDropDown)bindable;
            //customDropDown.UpdateSelectedIndex(newValue);
        }

        private static object CoerceSelectedIndex(BindableObject bindable, object value)
        {
            DropdownView customDropDown = (DropdownView)bindable;
            if (value is int)
            {
                return customDropDown.ItemSource == null ? -1 : ((int)value).Clamp(-1, customDropDown.ItemSource.Count - 1);
            }
            throw new InvalidOperationException("Selected Index must be an integer");
        }

        private void UpdateSelectedIndex(object selectedItem)
        {
            if (ItemSource != null)
            {
                var index = ItemSource.IndexOf(selectedItem);
                SelectedItem = ItemSource[index];
                ItemSelected?.Invoke(this, new ItemSelectedEventArgs { SelectedIndex = index });

            }
        }

        private void UpdateSelectedItem()
        {
            if (SelectedIndex == -1)
            {
                SelectedItem = null;
            }
            else if (ItemSource != null)
            {
                SelectedItem = ItemSource[SelectedIndex];
            }
        }
    }

    public class SelectedIndexChangedEventArgs : EventArgs
    {
        public SelectedIndexChangedEventArgs(int oldIndex, int newIndex)
        {
            OldIndex = oldIndex;
            NewIndex = newIndex;
        }

        public int OldIndex { get; private set; }
        public int NewIndex { get; set; }
    }

    public class ItemSelectedEventArgs : EventArgs
    {
        public int SelectedIndex { get; set; }
    }
}
