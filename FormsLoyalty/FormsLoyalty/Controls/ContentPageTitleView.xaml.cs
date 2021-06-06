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
    public partial class ContentPageTitleView : ContentView
    {
        private static readonly BindableProperty StepperContentPageTitleProperty = BindableProperty.Create(
                                                       propertyName: nameof(StepperContentPageTitle),
                                                       returnType: typeof(string),
                                                       declaringType: typeof(ContentPageTitleView),
                                                       defaultValue: "",
                                                       defaultBindingMode: BindingMode.TwoWay);

       

        public string StepperContentPageTitle
        {
            get
            {
                return (string)GetValue(StepperContentPageTitleProperty);
            }
            set
            {
                SetValue(StepperContentPageTitleProperty, value);
            }
        }

        public ContentPageTitleView()
        {
            InitializeComponent();
            StepperPageTitle.SetBinding(Label.TextProperty, new Binding(nameof(StepperContentPageTitle), BindingMode.TwoWay, source: this));
        }
    }
}