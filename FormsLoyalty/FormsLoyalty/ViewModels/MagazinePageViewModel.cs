﻿using FormsLoyalty.Models;
using FormsLoyalty.Services;
using FormsLoyalty.Utils;
using FormsLoyalty.Views;
using LSRetail.Omni.Domain.DataModel.Loyalty.Setup;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using MagazineModel = LSRetail.Omni.Domain.DataModel.Loyalty.Magazine.MagazineModel;

namespace FormsLoyalty.ViewModels
{
    public class MagazinePageViewModel : ViewModelBase
    {
        private ObservableCollection<MagazineModel> _magazines;
        public ObservableCollection<MagazineModel> Magazines
        {
            get { return _magazines; }
            set { SetProperty(ref _magazines, value); }
        }

       

        public MagazinePageViewModel(INavigationService navigationService) : base(navigationService)
        {
            Task.Run(async() =>
            {
               await LoadMagazine();
            });
           
        }

        private async  Task LoadMagazine()
        {
            IsPageEnabled = true;
            try
            {

               var magazines = await new CommonModel().GetMagazineAsync();
               Magazines = new ObservableCollection<MagazineModel>(magazines.Where(x => !string.IsNullOrEmpty(x.URL)));
                   
                
            }
            catch (Exception ex)
            {
               await App.dialogService.DisplayAlertAsync("Error!!", ex.Message, "OK");
                IsPageEnabled = false;
            }
            IsPageEnabled = false;
        }

        internal async Task NavigateToDetail(MagazineModel magazine)
        {

            await Launcher.TryOpenAsync(new Uri(magazine.URL));

        }
    }
}
