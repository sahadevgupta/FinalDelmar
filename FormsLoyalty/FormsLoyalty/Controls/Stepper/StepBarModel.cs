using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace FormsLoyalty.Controls.Stepper
{
    public enum StepBarStatus
    {
        Completed,
        InProgress,
        Pending
    }
    public class StepBarModel : BindableBase
    {
        public int StepID { get; set; }

        private string _stepName;
        public string StepName
        {
            get { return _stepName; }
            set { SetProperty(ref _stepName, value); }
        }
       

        public bool IsNotLast { get; set; }
        public double ListWidth { get; set; }

        private StepBarStatus _status;
        public StepBarStatus Status
        {
            get { return _status; }
            set { SetProperty(ref _status, value); }
        }

        private double _progressValue;
        public double ProgressValue
        {
            get { return _progressValue; }
            set { SetProperty(ref _progressValue, value); }
        }

        private ContentView _mainContent;
        public ContentView MainContent
        {
            get { return _mainContent; }
            set { SetProperty(ref _mainContent, value); }
        }

        private bool _isCurrentContent;
        public bool IsCurrentContent
        {
            get { return _isCurrentContent; }
            set { SetProperty(ref _isCurrentContent, value); }
        }
    }
}
