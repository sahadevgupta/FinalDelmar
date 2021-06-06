using System;
using System.Collections.Generic;
using System.Text;

namespace FormsLoyalty.Interfaces
{
    public interface ISearchPage
    {
        void OnSearchBarTextChanged(in string text);
        event EventHandler<string> SearchBarTextChanged;
    }
}
