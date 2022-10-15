using FormsLoyalty.Models;
using FormsLoyalty.ViewModels;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace FormsLoyalty.Views
{
    public partial class RemainderPage : ContentPage
    {
        RemainderPageViewModel _viewModel;
        public RemainderPage()
        {
            InitializeComponent();

            _viewModel = BindingContext as RemainderPageViewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            var index = collectionView.ItemsSource.Cast<ReminderDate>().IndexOf(_viewModel.SelectedDate);
            if (index > 0)
            {
                collectionView.ScrollTo(index, position: ScrollToPosition.Center, animate: false);
            }


        }
        private void SelectDate(object sender, System.EventArgs e)
        {
            OnDataSelection((e as TappedEventArgs).Parameter as ReminderDate);
        }

        private void OnDataSelection(ReminderDate reminderDate)
        {
            var selectedAny = _viewModel.dates.FirstOrDefault(x => x.IsSelected);
            if (selectedAny != null)
            {
                selectedAny.IsSelected = false;
            }
            _viewModel.SelectedDate = reminderDate;
            _viewModel.SelectedDate.IsSelected = true;

            _viewModel.GetReminder();
        }

       
        private async void leftarrow_Tapped(object sender, EventArgs e)
        {
            var view = (Image)sender;
            view.Opacity = 0;
            await view.FadeTo(1, 250);

            var index = collectionView.ItemsSource.Cast<ReminderDate>().IndexOf(_viewModel.SelectedDate);
            if (index > 0)
            {
                var newIndex = index - 7;
                collectionView.ScrollTo(newIndex, position: ScrollToPosition.Start, animate: true);

                var date = collectionView.ItemsSource.Cast<ReminderDate>().ElementAtOrDefault(newIndex);
                if (date!=null)
                {
                    OnDataSelection(date);
                }
                
            }
            view.Opacity = 1;
        }

        private async void rightarrow_Tapped(object sender, EventArgs e)
        {
            var view = (Image)sender;
            view.Opacity = 0;
            await view.FadeTo(1, 250);

            var index = collectionView.ItemsSource.Cast<ReminderDate>().IndexOf(_viewModel.SelectedDate);
            if (DateTime.DaysInMonth(DateTime.Now.Year,DateTime.Now.Month) > index)
            {
                var newIndex = index +7;
                collectionView.ScrollTo(newIndex, position: ScrollToPosition.End, animate: true);

                var date = collectionView.ItemsSource.Cast<ReminderDate>().ElementAtOrDefault(newIndex);
                if (date != null)
                {
                    OnDataSelection(date);
                }
            }

            view.Opacity = 1;
        }

        private void Today_Clicked(object sender, EventArgs e)
        {
            var today = collectionView.ItemsSource.Cast<ReminderDate>().FirstOrDefault( x => x.day == DateTime.UtcNow.Day);
            if (today !=null)
            {
                collectionView.ScrollTo(today, position: ScrollToPosition.Center, animate: true);
                OnDataSelection(today);
            }
           

           
        }

        private async void AddBtn_Tapped(object sender, EventArgs e)
        {
            var view = (Grid)sender;
            view.Opacity = 0;
           await view.FadeTo(1, 250);

           await _viewModel.NavigateToAddReminderPage();
            view.Opacity = 1;
        }

        private void ImageButton_Clicked(object sender, EventArgs e)
        {
            var view = (Image)sender;
           var data = view.Parent.Parent.BindingContext;
            var key = data.GetType().GetProperty("Key").GetValue(data)?.ToString();
            _viewModel.DeleteReminder(key, ((e as TappedEventArgs).Parameter as MedicineReminder));
        }
    }
}
