using System;
using System.Collections.Generic;
using System.Text;

namespace FormsLoyalty.Interfaces
{
    public interface IAppSettings
    {
        void SwitchToBackground();

        bool CheckIfAppInstalledOrNot(string packageName);
        void ClearAllCookies();
        string GetOSVersion();
    }
}
