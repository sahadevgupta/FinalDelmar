﻿using FormsLoyalty.Helpers;
using FormsLoyalty.Views;
using LSRetail.Omni.Domain.DataModel.Loyalty.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.PancakeView;
using Xamarin.Forms.Xaml;

namespace FormsLoyalty.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SidebarControl : ContentView
    {
        bool IsMenuOpen;
        public static readonly BindableProperty CurrentCategoryChangedCommandProperty = BindableProperty.Create(nameof(CurrentCategoryChangedCommand), typeof(ICommand), typeof(SidebarControl), null);
        public static readonly BindableProperty CategoriesProperty = BindableProperty.Create(nameof(Categories), typeof(IEnumerable<ItemCategory>), typeof(SidebarControl), null);
        public static readonly BindableProperty CategorySelectedProperty = BindableProperty.Create(nameof(CategorySelected), typeof(ItemCategory), typeof(SidebarControl), null, BindingMode.TwoWay, propertyChanged: CurrentItemChange);
        public IEnumerable<ItemCategory> Categories
        {
            get { return (IEnumerable<ItemCategory>)GetValue(CategoriesProperty); }
            set { SetValue(CategoriesProperty, value); }
        }
        public ICommand CurrentCategoryChangedCommand
        {
            get { return (ICommand)GetValue(CurrentCategoryChangedCommandProperty); }
            set { SetValue(CurrentCategoryChangedCommandProperty, value); }
        }
        public ItemCategory CategorySelected
        {
            get { return (ItemCategory)GetValue(CategorySelectedProperty); }
            set { SetValue(CategorySelectedProperty, value); }
        }

        static void CurrentItemChange(object bindable, object oldValue, object newValue)
        {
            var control = (SidebarControl)bindable;
            var categories = control.FindByName("categoriesContainer") as StackLayout;
            foreach (StackLayout stackLayout in categories.Children)
            {
                var label = stackLayout.FindByName<Label>("categoryTitle");
                var grid = stackLayout.FindByName<Grid>("sectionIndicator");

                if ((ItemCategory)newValue != null)
                {
                    if (FormsLoyalty.Helpers.Settings.RTL)
                    {
                        if (((ItemCategory)newValue).ArabicDescription == label.Text)
                        {
                            VisualStateManager.GoToState(label, "Selected");
                            grid.IsVisible = true;
                        }
                        else
                        {
                            VisualStateManager.GoToState(label, "Normal");
                            grid.IsVisible = false;
                        }
                    }
                    else
                    {
                        if (((ItemCategory)newValue).Description == label.Text)
                        {
                            VisualStateManager.GoToState(label, "Selected");
                            grid.IsVisible = true;
                        }
                        else
                        {
                            VisualStateManager.GoToState(label, "Normal");
                            grid.IsVisible = false;
                        }
                    }
                }


            }
        }

        public SidebarControl()
        {
            InitializeComponent();
        }

        void Category_Clicked(object sender, EventArgs e)
        {
            var label = ((StackLayout)sender).Children[0] as Label;
            if (Settings.RTL)
            {
                CategorySelected = Categories.FirstOrDefault(c => c.ArabicDescription == label.Text);
            }
           else
                CategorySelected = Categories.FirstOrDefault(c => c.Description == label.Text);

            if (CurrentCategoryChangedCommand != null)
                if (CurrentCategoryChangedCommand.CanExecute(CategorySelected))
                    CurrentCategoryChangedCommand.Execute(CategorySelected);
        }

        //protected override void OnSizeAllocated(double width, double height)
        //{
        //    base.OnSizeAllocated(width, height);
        //    //Fix for Indicator
        //    foreach (StackLayout stackLayout in categoriesContainer.Children)
        //    {
        //        var label = stackLayout.FindByName<Button>("categoryTitle");
        //        if (CategorySelected.Description == label.Text)
        //        {
        //            MoveActiveIndicator(stackLayout);
        //        }

        //    }
        //}

        void MoveActiveIndicator(StackLayout target)
        {
            var parent = target.Parent as StackLayout;

           
            
            var sectionIndicator = target.FindByName<Grid>("sectionIndicator");
            sectionIndicator.TranslateTo(0, target.Y + parent.Y, 300, Easing.SpringOut);
        }

        //void Toggle_Menu(System.Object sender, System.EventArgs e)
        //{
        //    Animation animateSection;
        //    if (IsMenuOpen)
        //        animateSection = new Animation(d => menuContainer.WidthRequest = d, 300, 80);
        //    else
        //        animateSection = new Animation(d => menuContainer.WidthRequest = d, 80, 300);
        //    animateSection.Commit(menuContainer, "MoreLikeSectionToggleAnimation", 16, 450, Easing.SpringIn);
        //    IsMenuOpen = !IsMenuOpen;
        //}

        //void SwipeGestureRecognizer_Swiped(System.Object sender, Xamarin.Forms.SwipedEventArgs e)
        //{
        //    Animation animateSection;
        //    if (IsMenuOpen)
        //        animateSection = new Animation(d => menuContainer.WidthRequest = d, 300, 80);
        //    else
        //        animateSection = new Animation(d => menuContainer.WidthRequest = d, 80, 300);
        //    animateSection.Commit(menuContainer, "MoreLikeSectionToggleAnimation", 16, 450, Easing.SpringIn);
        //    IsMenuOpen = !IsMenuOpen;
        //}
    }
}