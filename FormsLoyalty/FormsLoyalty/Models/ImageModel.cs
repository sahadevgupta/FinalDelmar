using FormsLoyalty.Utils;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.Domain.Services.Base.Image;
using LSRetail.Omni.Infrastructure.Data.Omniservice.Base;
using Microsoft.AppCenter.Crashes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FormsLoyalty.Models
{
    public class ImageModel
    {
        private ImageService service;
        private string securityTokenInUse;
        public ImageModel()
        {
            
        }
        public async Task<ImageView> ImageGetById(string id, ImageSize imageSize)
        {
            ImageView imageView = null;

            if (imageSize.Height == 0)
            {
                imageSize.Height = Int32.MaxValue;
            }

            if (imageSize.Width == 0)
            {
                imageSize.Width = Int32.MaxValue;
            }

            BeginWsCall();

            try
            {
                imageView = await service.ImageGetByIdAsync(id, imageSize);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }

            return imageView;
        }
        protected void BeginWsCall()
        {
            if (string.IsNullOrEmpty(securityTokenInUse) || AppData.Device.SecurityToken != securityTokenInUse)
            {
                CreateService();
            }

            this.securityTokenInUse = AppData.Device.SecurityToken;
        }

        private void CreateService()
        {
            service = new ImageService(new ImageRepository());
        }
    }
}
