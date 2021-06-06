using FormsLoyalty.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace FormsLoyalty.Services
{
    /// <summary>
    /// Defines the <see cref="CommonServices" />.
    /// </summary>
    public static class CommonServices
    {
        /// <summary>
        /// The ListenToSmsRetriever.
        /// </summary>
        public static void ListenToSmsRetriever()
        {
            DependencyService.Get<IListenToSmsRetriever>()?.ListenToSmsRetriever();
        }

       
    }
}
