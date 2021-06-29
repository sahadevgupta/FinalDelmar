using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FormsLoyalty.Interfaces
{
    public interface IAppRating
    {
        Task RateApp();
       void  OpenStoreListing(string appId);
    }
}
