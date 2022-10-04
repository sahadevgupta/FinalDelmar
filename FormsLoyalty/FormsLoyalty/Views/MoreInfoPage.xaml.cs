﻿using FormsLoyalty.Interfaces;
using FormsLoyalty.Utils;
using FormsLoyalty.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using XF.Material.Forms.Dialogs;
using ZXing.Net.Mobile.Forms;

namespace FormsLoyalty.Views
{
    public partial class MoreInfoPage : ContentPage
    {
        MoreInfoPageViewModel _viewModel;
        public MoreInfoPage()
        {
            InitializeComponent();
            _viewModel = BindingContext as MoreInfoPageViewModel;
            _viewModel.navigation = Navigation;
        }

        private async void menu_Tapped(object sender, System.EventArgs e)
        {
            var view = (View)sender;
            view.BackgroundColor = Color.FromHex("#efefef");
            view.Opacity = 0;
            await view.FadeTo(1, 250);
           
            await _viewModel.DrawerSelected((e as TappedEventArgs).Parameter as DrawerMenuItem);
            view.BackgroundColor = Color.Transparent;
            view.Opacity = 1;
        }
       
        private async void BarcodeScan_Tapped(object sender, System.EventArgs e)
        {

            var view = (View)sender;
            view.BackgroundColor = Color.LightPink;
            view.Opacity = 0;
            await view.FadeTo(1, 250);

            await ScanImage();
            view.BackgroundColor = Color.Transparent;
            view.Opacity = 1;



           
        }
        private async Task ScanImage()
        {
            _viewModel.IsPageEnabled = true;

            var options = new ZXing.Mobile.MobileBarcodeScanningOptions();
            options.PossibleFormats = new List<ZXing.BarcodeFormat>() { ZXing.BarcodeFormat.EAN_13, ZXing.BarcodeFormat.EAN_8 };



            var overlay = new ZXingDefaultOverlay
            {
                ShowFlashButton = false,
                TopText = AppResources.ScannerViewScannerTopText,
                BottomText = AppResources.ScannerViewScannerBottomText,

            };
            overlay.BindingContext = overlay;


            ZXingScannerPage scan = new ZXingScannerPage(options, overlay);
            scan.DefaultOverlayTopText = "Title";
            scan.DefaultOverlayBottomText = "TEXT";
            scan.AutoFocus();
            scan.DefaultOverlayShowFlashButton = true;
            scan.HasTorch = true;
            scan.Title = "SCAN";
            

            await Navigation.PushAsync(scan);
            scan.OnScanResult += (result) =>
            {
                scan.IsScanning = false;
                

                Device.BeginInvokeOnMainThread(async () =>
                {
                    DependencyService.Get<INotify>().ShowToast($"Scan Successful!!, Code : {result.Text}");
                    await Navigation.PopAsync();
                    string barcode = result.Text;
                   await _viewModel.NavigateToItemPage(barcode);
                });
            };

            _viewModel.IsPageEnabled = false;
        }

        private async void Profile_Tapped(object sender, System.EventArgs e)
        {
            var view = (View)sender;
            view.Opacity = 0;
            await view.FadeTo(1, 250);

            await _viewModel.NavigateToProfile();
            
            view.Opacity = 1;
        }
        private async void Settings_Tapped(object sender, System.EventArgs e)
        {
            var view = (View)sender;
           
            view.Opacity = 0;
            await view.FadeTo(1, 250);

            await _viewModel.OnSettingSelected((e as TappedEventArgs).Parameter.ToString());
            
            view.Opacity = 1;
        }
    }
}
