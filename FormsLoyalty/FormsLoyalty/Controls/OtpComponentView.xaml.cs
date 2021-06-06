using FormsLoyalty.Models;
using FormsLoyalty.ViewModels;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FormsLoyalty.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OtpComponentView : ContentView
    {
        private ObservableCollection<OtpModel> _otpModelList;
        public ObservableCollection<OtpModel> OtpModelList
        {
            get { return _otpModelList; }
            set
            {
                _otpModelList = value;
                OnPropertyChanged();
            }
        }

        private string _otpValue;

        public string OtpValue
        {
            get { return _otpValue; }
            set
            {
                _otpValue = value;
                if (_otpValue.Length <= 4)
                {
                    IsError = false;
                }
                OnPropertyChanged();
            }
        }


        private bool _IsError;
        public bool IsError
        {
            get { return _IsError; }
            set
            {
                _IsError = value;
                if (_IsError)
                {
                    footerLabel.IsVisible = false;
                    errorLabel.IsVisible = true;
                }
                else
                {
                    footerLabel.IsVisible = true;
                    errorLabel.IsVisible = false;
                }
                OnPropertyChanged();
            }
        }

        private string _otpTimer;
        public string OtpTimer
        {
            get { return _otpTimer; }
            set
            {
                _otpTimer = value;
                Device.BeginInvokeOnMainThread(() =>
                {
                    footerLabel.Text = _otpTimer;
                });
                OnPropertyChanged();
            }
        }


        public OtpComponentView(string mainLabelText, Binding expiresBinding, ViewModelBase vm, int OtpCount = 4)
        {
            InitializeComponent();
            BindingContext = vm;
            otpFlex.BindingContext = this;
            IsError = false;
           

           // footerLabel.SetBinding(Label.TextProperty, new Binding(nameof(expiresBinding),BindingMode.TwoWay));

            if (OtpCount != 0)
            {
                OtpModelList = new ObservableCollection<OtpModel>();
                for (int i = 0; i < OtpCount; i++)
                {
                    OtpEntry newentry = new OtpEntry()
                    {
                        FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Entry)),
                        HorizontalTextAlignment = TextAlignment.Center,
                        InputTransparent = true,
                        Keyboard = Keyboard.Numeric,
                        MaxLength = 1,
                        HeightRequest = 50 ,
                        FontAttributes = FontAttributes.Bold,
                        WidthRequest = 50
                         
                    };
                    newentry.SetBinding(Entry.TextProperty, "Value", BindingMode.TwoWay);
                    newentry.TextChanged += OtpEntry_TextChanged;
                    newentry.OnBackspace += OtpEntry_BackSpace;
                    OtpModelList.Add(new OtpModel() { ID = i, entry = newentry });
                }
            }
            
        }

        private void FrameGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            if (OtpModelList != null && OtpModelList.Count > 0)
            {
                int isemptyCount = OtpModelList.Count(x => !string.IsNullOrEmpty(x.Value));
                if (isemptyCount == OtpModelList.Count())
                {
                    OtpModelList[isemptyCount - 1].entry.Focus();
                }
                else if (isemptyCount == 0)
                {
                    OtpModelList[0].entry.Focus();
                }
                else
                {
                    OtpModelList.FirstOrDefault(x => string.IsNullOrEmpty(x.Value)).entry.Focus();
                }
            }
        }

        public void GetEnteredOTP()
        {
            OtpValue = "";
            if (OtpModelList != null && OtpModelList.Count > 0)
            {
                foreach (OtpModel model in OtpModelList)
                {
                    OtpValue += model.Value;
                }
            }
        }

        public void FillReceivedOTP(string otp)
        {
            if (!string.IsNullOrEmpty(otp))
            {
                char[] otpvalues = otp.ToCharArray();
                if (OtpModelList != null && OtpModelList.Count > 0)
                {
                    for (int i = 0; i < otpvalues.Count(); i++)
                    {
                        OtpModelList[i].entry.Text = otpvalues[i].ToString();
                    }
                }
            }
        }

        private void OtpEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            OtpEntry currentry = sender as OtpEntry;
            OtpModel count = currentry.BindingContext as OtpModel;
            OtpModel firstelement = OtpModelList.FirstOrDefault();
            OtpModel lastelement = OtpModelList.Last();
            int previous = count.ID - 1;
            int next = count.ID + 1;
            if (count.ID == firstelement.ID)
            {
                if (count.entry.Text.Length >= 1)
                {
                    OtpModelList[next].entry.Focus();
                }
            }
            else if (count.ID == lastelement.ID)
            {
                if (count.entry.Text.Length == 0)
                {
                    OtpModelList[previous].entry.Focus();
                }
            }
            else
            {
                if (count.entry.Text.Length == 0)
                {
                    OtpModelList[previous].entry.Focus();
                }
                if (count.entry.Text.Length >= 1)
                {
                    OtpModelList[next].entry.Focus();
                }
            }
            GetEnteredOTP();
        }

        private void OtpEntry_BackSpace(object sender, EventArgs e)
        {
            OtpEntry currentry = sender as OtpEntry;
            OtpModel count = currentry.BindingContext as OtpModel;
            OtpModel firstelement = OtpModelList.FirstOrDefault();

            if (count.ID != firstelement.ID)
            {
                if (string.IsNullOrEmpty(count.Value))
                {
                    int previous = count.ID - 1;
                    OtpModelList[previous].entry.Focus();
                }
            }
            GetEnteredOTP();
        }
    }

    public class OtpModel : BindableBase
    {
        public int ID { get; set; }
        public string Value { get; set; }
        private OtpEntry _entry;

        public OtpEntry entry
        {
            get
            {
                return _entry;
            }
            set
            {
                _entry = value;
                
            }
        }
    }

    public class OtpEntry : Entry
    {
        public delegate void BackspaceEventHandler(object sender, EventArgs e);

        public delegate void OnTextChangedEventHandler(object sender, EventArgs e);

        public event BackspaceEventHandler OnBackspace;

        public OtpEntry()
        {
        }

        public void OnBackspacePressed()
        {
            if (OnBackspace != null)
            {
                OnBackspace(this, null);
            }
        }
    }
}