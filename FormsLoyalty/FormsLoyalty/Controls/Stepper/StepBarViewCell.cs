using FormsLoyalty.Converters;
using FormsLoyalty.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;
using Xamarin.Forms.PancakeView;

namespace FormsLoyalty.Controls.Stepper
{
    public class StepBarViewCell1 : ContentView
    {
        private readonly ProgressBar progress;
        private readonly ViewModelBase vm;
        private readonly Label mainLabel;
        private readonly Grid mainGrid;
        private readonly Grid trackergrid;

        public StepBarViewCell1(ViewModelBase viewmodel)
        {
            vm = viewmodel;
            mainGrid = new Grid()
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                ColumnSpacing = 0,
                Margin = 0,
                Padding = 0,
                RowDefinitions =
                  {
                      new RowDefinition { Height = GridLength.Auto},
                      new RowDefinition { Height = GridLength.Auto},
                  },
                ColumnDefinitions =
                  {
                        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) },
                       new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) },
                  }
            };
            trackergrid = new Grid()
            {
                WidthRequest = 20,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.Start,
                Padding = 0,
                Margin = new Thickness(0, 10, 0, 0),
                RowSpacing = 1,
                ColumnSpacing = 0,
                RowDefinitions =
                  {
                      new RowDefinition { Height = GridLength.Auto},
                  },
                ColumnDefinitions =
                  {
                        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) },
                  }
            };

            PancakeView trackerbg = new PancakeView()
            {
                CornerRadius = Device.RuntimePlatform == Device.iOS ? new CornerRadius(10, 10, 10, 10) : new CornerRadius(20, 20, 20, 20),
                HeightRequest = 20,
                WidthRequest = 20,
                IsClippedToBounds = false,
            };
            trackerbg.SetBinding(BackgroundColorProperty, "Status", converter: new StepColorConverter());

            Image img = new Image()
            {
                Aspect = Aspect.AspectFill,
                Margin = new Thickness(4)
            };
            img.SetBinding(Image.SourceProperty, "Status", converter: new StepImageConverter());
            trackerbg.Content = img;
            trackergrid.Children.Add(trackerbg, 0, 0);
            mainLabel = new Label()
            {
                FontSize = (double)NamedSize.Micro,
               
                TextColor = Color.FromHex("#333333"),
                HorizontalOptions = LayoutOptions.StartAndExpand,
                HorizontalTextAlignment = TextAlignment.Start,
                LineBreakMode = LineBreakMode.WordWrap,
                HeightRequest = 40
            };
          

            mainGrid.Children.Add(mainLabel, 0, 1);
            Grid.SetColumnSpan(mainLabel, 2);
            TapGestureRecognizer stepTapped = new TapGestureRecognizer();
            trackergrid.GestureRecognizers.Add(stepTapped);
            stepTapped.Tapped += StepTapped_Tapped;
            progress = new ProgressBar()
            {
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Margin = new Thickness(0, 10, 0, 0),
            };

            progress.SetDynamicResource(ProgressBar.ProgressColorProperty, "PrimayActionColor");

            if (Device.RuntimePlatform == Device.iOS)
            {
                progress.SetBinding(ProgressBar.ProgressProperty, "ProgressValue");
            }
            else
            {
                progress.SetBinding(ProgressBar.ProgressProperty, "ProgressValue", BindingMode.TwoWay);
            }
            progress.SetBinding(IsVisibleProperty, "IsNotLast");

            mainGrid.Children.Add(trackergrid, 0, 0);
            mainGrid.Children.Add(progress, 1, 0);
            Content = mainGrid;
        }

        private void StepTapped_Tapped(object sender, EventArgs e)
        {
            if (BindingContext is StepBarModel selectedmodel && (selectedmodel.Status == StepBarStatus.Completed || selectedmodel.Status == StepBarStatus.InProgress))
            {
                vm.OnStepTapped(BindingContext as StepBarModel);
                //if (vm.GetType() == typeof(RegisterViewModel))
                //{
                //    RegisterViewModel registrationviewmodel = (RegisterViewModel)vm;
                //    await registrationviewmodel.AddContentForTappedStep(BindingContext as StepBarModel);
                //}
            }
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            double cellwidth = Application.Current.MainPage.Width / (vm.StepListCount);
            this.WidthRequest = cellwidth;
            progress.WidthRequest = cellwidth - 20;
            mainLabel.WidthRequest = cellwidth - 8 - 20;
            if (BindingContext is StepBarModel selectedmodel)
            {
                if (selectedmodel.IsNotLast)
                {
                    trackergrid.HorizontalOptions = LayoutOptions.CenterAndExpand;
                }
                else
                {
                    trackergrid.HorizontalOptions = LayoutOptions.EndAndExpand;
                }
            }
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            if (BindingContext is StepBarModel selectedmodel)
            {
                mainLabel.Text = selectedmodel.StepName;

                if (selectedmodel.Status == StepBarStatus.Completed)
                {
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        await progress.ProgressTo(1, 800, Easing.Linear);
                        selectedmodel.ProgressValue = 1;
                    });
                }
                else
                {
                    progress.Progress = 0;
                }

                if (!selectedmodel.IsNotLast)
                {
                    mainGrid.ColumnDefinitions.RemoveAt(1);
                }
            }
        }
    }
}