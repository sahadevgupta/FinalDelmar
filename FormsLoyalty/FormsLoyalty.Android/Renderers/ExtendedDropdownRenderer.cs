using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using FormsLoyalty.Controls;
using FormsLoyalty.Droid.Extensions;
using FormsLoyalty.Droid.Renderers;
using Java.Lang;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Color = Xamarin.Forms.Color;
using Exception = System.Exception;
using Label = Xamarin.Forms.Label;

[assembly: ExportRenderer(typeof(DropdownView), typeof(ExtendedDropdownRenderer))]
namespace FormsLoyalty.Droid.Renderers
{
    public class ExtendedDropdownRenderer : ViewRenderer<DropdownView, AutoCompleteTextView>
    {
        private AutoCompleteTextView autoCompleteTextView;
        private DropdownView customDropdown;
        private SuggestCompleteAdapter adapter;

        private bool suppressTextChangedEvent;

        public ExtendedDropdownRenderer(Context context) : base(context)
        {

        }

        protected override void OnElementChanged(ElementChangedEventArgs<DropdownView> e)
        {
            base.OnElementChanged(e);
            if (e.OldElement != null)
            {

            }
            if (e.NewElement == null) return;

            if (Control is null)
            {
                autoCompleteTextView = new AutoCompleteTextView(this.Context);
                SetNativeControl(autoCompleteTextView);
            }

            customDropdown = e.NewElement as DropdownView;
            UpdateText();
            UpdateEnabledProperty();
            UpdateStyle();

            autoCompleteTextView.SetBackgroundColor(Color.Transparent.ToAndroid());
            autoCompleteTextView.Hint = customDropdown.PlaceholderText;
            if (!customDropdown.CanEdit)
            {
                autoCompleteTextView.InputType = Android.Text.InputTypes.TextFlagNoSuggestions;
                autoCompleteTextView.SetFocusable(Element, false);
                autoCompleteTextView.Clickable = true;
            }

            autoCompleteTextView.Threshold = 0;
            autoCompleteTextView.DropDownHeight = 350;
            

            autoCompleteTextView.Click += AutoCompleteTextView_Click;
            autoCompleteTextView.TextChanged += AutoCompleteTextView_TextChanged;
            autoCompleteTextView.ItemClick += AutoCompleteTextView_ItemClick;
            autoCompleteTextView.FocusChange += AutoCompleteTextView_FocusChange;
            adapter = new SuggestCompleteAdapter(Context, Resource.Layout.autocomplete_list_row, Resource.Id.testt);
            //adapter = new SuggestCompleteAdapter(Context, Android.Resource.Layout.auto, Android.Resource.Id.testt);

            UpdateItemsSource(Element?.ItemSource?.OfType<object>());

        }

        private void AutoCompleteTextView_FocusChange(object sender, FocusChangeEventArgs e)
        {
            autoCompleteTextView.ShowDropDown();
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName == DropdownView.ItemSourceProperty.PropertyName)
            {
                var items = ((DropdownView)sender).ItemSource;
                UpdateItemsSource(items?.OfType<object>());
            }
            else if (e.PropertyName == DropdownView.SelectedIndexProperty.PropertyName)
            {
                UpdateText();
            }
            else if (e.PropertyName == DropdownView.SelectedItemProperty.PropertyName)
            {
                UpdateText();
            }
            else if (e.PropertyName == DropdownView.ShowDropdownProperty.PropertyName)
            {
                UpdateShowDropdown();
            }
        }
        private void UpdateStyle()
        {
            var data = Element.InputViewStyle.Setters;
            foreach (var item in data)
            {
                if (item.Property == Label.FontFamilyProperty)
                {
                    Typeface tf = item.Value.ToString().ToTypeFace();
                    autoCompleteTextView.SetTypeface(tf, Android.Graphics.TypefaceStyle.Normal);
                }

                if (item.Property == Label.FontSizeProperty)
                {
                    var value = item.Value as Xamarin.Forms.OnIdiom<double>;
                    double size = Device.Idiom == TargetIdiom.Phone ? value.Phone : value.Tablet;
                    Control.SetTextSize(Android.Util.ComplexUnitType.Dip, (float)size);
                    autoCompleteTextView.SetTextSize(Android.Util.ComplexUnitType.Dip, (float)size);
                    //var size = Device.GetNamedSize(NamedSize.)
                }

                if (item.Property == Label.TextColorProperty)
                {
                    var resource = item.Value as Xamarin.Forms.Internals.DynamicResource;
                    var color = (Xamarin.Forms.Color)Xamarin.Forms.Application.Current.Resources[resource.Key];
                    autoCompleteTextView.SetTextColor(color.ToAndroid());
                    //SetPlaceholderTextColor(color);
                }
            }
        }
        private void UpdateEnabledProperty()
        {
            autoCompleteTextView.Enabled = Element.IsEnabled;
        }
        private void UpdateText()
        {
            if (customDropdown.SelectedItem is object)
            {
                string value = FormatType(customDropdown.SelectedItem, Element.DisplayMemberPath);
                autoCompleteTextView.SetText(value, true);
                Control.SetText(value, true);
            }
            else
            {
                autoCompleteTextView.SetText(string.Empty, true);
                Control.SetText(string.Empty, true);
            }
        }

        private async void UpdateItemsSource(IEnumerable<object> items)
        {
            if (items == null)
                adapter.UpdateList(Enumerable.Empty<string>(), (o) => FormatType(o, Element.DisplayMemberPath));
            else
                adapter.UpdateList(items.OfType<object>(), (o) => FormatType(o, Element.DisplayMemberPath));

            UpdateAdapter();
            await Task.Delay(1000);
            UpdateShowDropdown();
        }

        private void UpdateShowDropdown()
        {
            if (Element == null) return;
            if (Element.ItemSource != null && Element.ItemSource.Count > 0)
            {
                if (Element.ShowDropdown)
                {
                    autoCompleteTextView.ShowDropDown();
                }
                else
                {
                    autoCompleteTextView.DismissDropDown();
                }
            }
        }

        private string FormatType(object instance, string displayMemberPath)
        {
            if (!string.IsNullOrEmpty(displayMemberPath))
            {
                return instance?.GetType().GetProperty(displayMemberPath)?.GetValue(instance)?.ToString() ?? "";
            }
            else
            {
                return instance?.ToString() ?? "";
            }
        }

        void UpdateAdapter()
        {
            try
            {
                autoCompleteTextView.Adapter = adapter;

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
            }
        }
        private void DismissKeyboard()
        {
            var imm = (global::Android.Views.InputMethods.InputMethodManager)Context.GetSystemService(Context.InputMethodService);
            imm.HideSoftInputFromWindow(WindowToken, 0);
        }
        private void AutoCompleteTextView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            DismissKeyboard();
            var selectedItem = customDropdown.ItemSource.OfType<object>().ElementAt(e.Position);
            if (Element.SelectedItem != selectedItem)
            {
                suppressTextChangedEvent = true;
                Element.OnItemSelected(selectedItem);
                autoCompleteTextView.ClearFocus();
                suppressTextChangedEvent = false;
            }
        }

        private void AutoCompleteTextView_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            if (!suppressTextChangedEvent)
                customDropdown.OnTextChanged(e.Text);
        }

        private void AutoCompleteTextView_Click(object sender, EventArgs e)
        {
            autoCompleteTextView.ShowDropDown();
        }
    }

    internal class SuggestCompleteAdapter : ArrayAdapter, IFilterable
    {
        readonly SuggestFilter filter = new SuggestFilter();
        List<object> resultList;
        Func<object, string> labelFunc;


        public SuggestCompleteAdapter(Context context, int resource, int textViewResourceId) : base(context, resource, textViewResourceId)
        {
            resultList = new List<object>();
            SetNotifyOnChange(true);
        }
        public void UpdateList(IEnumerable<object> list, Func<object, string> labelFunc)
        {
            this.labelFunc = labelFunc;
            resultList = list.ToList();
            filter.SetFilter(resultList.Select(x => labelFunc(x)));
            NotifyDataSetChanged();
        }
        public override int Count
        {
            get { return resultList.Count; }
        }

        public override Android.Views.View GetView(int position, Android.Views.View convertView, ViewGroup parent)
        {
            return base.GetView(position, convertView, parent);
        }

        public override Filter Filter => filter;

        public override Java.Lang.Object GetItem(int position)
        {
            return labelFunc(GetObject(position));
        }

        private object GetObject(int position)
        {
            return resultList[position];
        }

        public override long GetItemId(int position)
        {
            return base.GetItemId(position);
        }

        private class SuggestFilter : Filter
        {
            IEnumerable<string> resultList;

            public SuggestFilter()
            {

            }

            public void SetFilter(IEnumerable<string> list)
            {
                this.resultList = list;
            }

            protected override FilterResults PerformFiltering(ICharSequence constraint)
            {
                if (resultList == null)
                {
                    return new FilterResults() { Count = 0, Values = null };
                }

                var data = resultList.ToArray();
                return new FilterResults { Count = data.Length, Values = data };
            }

            protected override void PublishResults(ICharSequence constraint, FilterResults results)
            {

            }
        }
    }
}