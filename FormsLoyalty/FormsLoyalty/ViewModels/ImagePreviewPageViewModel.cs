using FormsLoyalty.Helpers;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace FormsLoyalty.ViewModels
{
    public class ImagePreviewPageViewModel : ViewModelBase
    {
        #region Command
        public DelegateCommand<ImageView> ChangeImageCommand => new DelegateCommand<ImageView>(ChangeImage);
        #endregion

        #region Property
        private string _imageView;
        public string ImageView
        {
            get { return _imageView; }
            set { SetProperty(ref _imageView, value); }
        }

        private ObservableCollection<ImageView> _images;
        public ObservableCollection<ImageView> Images
        {
            get { return _images; }
            set { SetProperty(ref _images, value); }
        }

        #endregion

        public ImagePreviewPageViewModel(INavigationService navigationService) : base(navigationService)
        {

        }

        internal void ChangeImage(ImageView obj)
        {
            
            ImageView = obj.Location;

        }

       

        public override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);
            ImageView = parameters.GetValue<string>("previewImage");
            Images = new ObservableCollection<ImageView>(parameters.GetValue<List<ImageView>>("images"));
            
        }
    }
}
