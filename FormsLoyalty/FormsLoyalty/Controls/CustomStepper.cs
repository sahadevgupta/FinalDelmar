using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Xamarin.CommunityToolkit.Behaviors;
using Xamarin.Forms;

namespace FormsLoyalty.Controls
{
    public class CustomStepper : StackLayout
    {

        Button PlusBtn;
        Button MinusBtn;
        Entry Entry;

        public event EventHandler ValueChanged;

        public static readonly BindableProperty TextProperty =
          BindableProperty.Create(
             propertyName: "Text",
              returnType: typeof(int),
              declaringType: typeof(CustomStepper),
              defaultValue: 1,
              defaultBindingMode: BindingMode.TwoWay);

        public int Text
        {
            get { return (int)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        public CustomStepper()
        {
            PlusBtn = new Button { Text = "+", WidthRequest = 40, HeightRequest = 40, FontAttributes = FontAttributes.Bold, FontSize = 17, BackgroundColor = Color.Transparent };
            MinusBtn = new Button { Text = "-", WidthRequest = 40, HeightRequest = 40, FontAttributes = FontAttributes.Bold, FontSize = 17, BackgroundColor = Color.Transparent };

            Orientation = StackOrientation.Horizontal;
            PlusBtn.Clicked += PlusBtn_Clicked;
            MinusBtn.Clicked += MinusBtn_Clicked;
            Entry = new Entry
            {
                PlaceholderColor = Color.Gray,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                WidthRequest = 45,
                HorizontalTextAlignment = TextAlignment.Center,
                BackgroundColor = Color.FromHex("#3FFF"),
                FontSize = 15,
                MaxLength = 2,
                TextColor= Color.Black,
                Keyboard = Keyboard.Numeric,
                
            };
            Entry.SetBinding(Entry.TextProperty, new Binding(nameof(Text), BindingMode.TwoWay, source: this));
            Entry.TextChanged += Entry_TextChanged;

           

            //Entry.Behaviors.Add(charactersValidationBehavior);

            Children.Add(MinusBtn);
            Children.Add(Entry);
            Children.Add(PlusBtn);
           
        }

        private void Entry_TextChanged(object sender, TextChangedEventArgs args)
        {

            if (!string.IsNullOrWhiteSpace(args.NewTextValue))
            {
                bool isvalid = args.NewTextValue.ToCharArray().All(x => char.IsDigit(x));
                ((Entry)sender).Text = isvalid ? args.NewTextValue : args.NewTextValue.Remove(args.NewTextValue.Length - 1);
            }


            ValueChanged?.Invoke(this, args);
        }



        private void MinusBtn_Clicked(object sender, EventArgs e)
        {
            if (Text > 1)
                Text--;
        }

        private void PlusBtn_Clicked(object sender, EventArgs e)
        {
            Text++;
        }
    }
}
