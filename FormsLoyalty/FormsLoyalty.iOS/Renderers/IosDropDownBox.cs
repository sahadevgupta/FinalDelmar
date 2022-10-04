using CoreAnimation;
using CoreGraphics;
using Foundation;
using FormsLoyalty.Controls;
using FormsLoyalty.iOS.Renderers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using Rectangle = System.Drawing.Rectangle;

namespace FormsLoyalty.iOS.Renderers
{

    /// <summary>
        ///  Creates a UIView with dropdown with a similar API and behavior to UWP's AutoSuggestBox
        /// </summary>
    [Register("UIAutoCompleteTextField")]

    public class IosDropDownBox : UIKit.UIView, IUITextFieldDelegate
    {
        private nfloat keyboardHeight;
        private NSLayoutConstraint bottomConstraint;
        private Func<object, string> textFunc;
        private bool showBottomBorder = true;
        private CALayer border;
        private UIView _background;
        private CGRect _drawnFrame;
        private UIViewController _parentViewController;

        public CGRect _rect { get; private set; }

        private UIScrollView _scrollView;
        public int AutocompleteTableViewHeight { get; set; } = 150;


        public static event EventHandler OnBackButtonClicked;

        public static List<IosDropDownBox> Instances = new List<IosDropDownBox>();

        /// <summary>
                /// Gets a reference to the text field in the view
                /// </summary>
        public UITextField InputTextField { get; }
        /// <summary>
        /// Gets a reference to the drop down selection list in the view
        /// </summary>
        public AutoCompleteTableView SelectionList { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="iOSAutoSuggestBox"/>.
        /// </summary>
        ///

        private static int count = 1;
        public IosDropDownBox()
        {

            InputTextField = new UITextField()
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                BorderStyle = UITextBorderStyle.None,
                ReturnKeyType = UIReturnKeyType.Search,
                AutocorrectionType = UITextAutocorrectionType.No,
                AdjustsFontSizeToFitWidth = true,
            };

            UITapGestureRecognizer tapGesture = new UITapGestureRecognizer(()=>OnTapped(InputTextField));
            tapGesture.CancelsTouchesInView = true;
            InputTextField.AddGestureRecognizer(tapGesture);
            InputTextField.ShouldBeginEditing = t =>
            {
                return false;
            };
            InputTextField.Tag = count++;
            InputTextField.ShouldReturn = InputText_OnShouldReturn;
            InputTextField.EditingDidBegin += OnEditingDidBegin;
            InputTextField.EditingDidEnd += OnEditingDidEnd;
            InputTextField.EditingChanged += InputText_EditingChanged;
            AddSubview(InputTextField);
            InputTextField.TopAnchor.ConstraintEqualTo(TopAnchor).Active = true;
            InputTextField.LeftAnchor.ConstraintEqualTo(LeftAnchor).Active = true;
            InputTextField.WidthAnchor.ConstraintEqualTo(WidthAnchor).Active = true;
            InputTextField.HeightAnchor.ConstraintEqualTo(HeightAnchor).Active = true;

            UIKeyboard.Notifications.ObserveWillShow(OnKeyboardShow);
            UIKeyboard.Notifications.ObserveWillHide(OnKeyboardHide);

            Instances.Add(this);


        }

        public static void HideDropDown()
        {
            if (Instances is object)
            {
                Instances.ForEach(x => x.IsSuggestionListOpen = false);
            }

        }



        public void Draw(UIViewController viewController, CALayer layer, UIScrollView scrollView, CGRect rect)
        {
            _scrollView = scrollView;
            _drawnFrame = layer.Frame;
            _parentViewController = viewController;
            _rect = rect;
            //Make new tableview and do some settings
            SelectionList = new AutoCompleteTableView(_scrollView)
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                ScrollEnabled = true,
                AllowsSelection = true,
                Bounces = false,
                Hidden = false,
                ContentInset = UIEdgeInsets.Zero,
                AutoresizingMask = UIViewAutoresizing.FlexibleHeight | UIViewAutoresizing.FlexibleWidth,
                TableFooterView = new UIView()
            };


            //Some textfield settings
            //AutocorrectionType = UITextAutocorrectionType.No;
            InputTextField.ClearButtonMode = UITextFieldViewMode.Never;


            CGRect frame;
            UIView view;
            view = _parentViewController.View;

            frame = new CGRect(rect.X, rect.Y + rect.Height + 5, this.Frame.Width, AutocompleteTableViewHeight);
            framewidth = this.Frame.Width;
            SelectionList.Layer.CornerRadius = 5;

            _background = new UIView() { BackgroundColor = UIColor.White, Hidden = true, TranslatesAutoresizingMaskIntoConstraints = false };
            _background.Layer.CornerRadius = 5; //rounded corners
            _background.Layer.MasksToBounds = false;
            _background.Layer.ShadowColor = UIColor.Black.CGColor;
            _background.Layer.ShadowOffset = new CGSize(0.0f, 4.0f);
            _background.Layer.ShadowOpacity = 0.25f;
            _background.Layer.ShadowRadius = 8f;
            _background.Layer.BorderColor = UIColor.LightGray.CGColor;
            _background.Layer.BorderWidth = 0.1f;

            //SelectionList.Frame = frame;

            view.AddSubview(_background);
            view.AddSubview(SelectionList);


        }

        private void OnTapped(UITextField arg)
        {

            ShowHideDropdown(arg);
            
        }
        private UIView GetImageView(string imagePath, int height, int width)
        {
            UIImageView uiImageView = new UIImageView(UIImage.FromBundle(imagePath))
            {
                Frame = new RectangleF(0, 0, width, height)
            };
            UIView objView = new UIView(new Rectangle(0, 0, width + 10, height));
            objView.AddSubview(uiImageView);
            return objView;
        }

        private void OnEditingDidBegin(object sender, EventArgs e)
        {
            IsSuggestionListOpen = true;
            EditingDidBegin?.Invoke(this, e);
        }
        private void OnEditingDidEnd(object sender, EventArgs e)
        {
            IsSuggestionListOpen = false;
            EditingDidEnd?.Invoke(this, e);
        }
        internal EventHandler EditingDidBegin;
        internal EventHandler EditingDidEnd;
        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
        }

        private void AddBottomBorder()
        {
            border = new CoreAnimation.CALayer();
            var width = 1f;
            border.BorderColor = UIColor.LightGray.CGColor;
            border.Frame = new CGRect(0, Frame.Size.Height - width, Frame.Size.Width, Frame.Size.Height);
            border.BorderWidth = width;
            border.Hidden = !showBottomBorder;
            Layer.AddSublayer(border);
            Layer.MasksToBounds = true;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to render a border line under the text field
        /// </summary>
        public bool ShowBottomBorder
        {
            get => showBottomBorder;
            set
            {
                showBottomBorder = value;
                if (border != null) border.Hidden = !value;
            }
        }

        /// <summary>
        /// Gets or sets the font of the <see cref="InputTextField"/>
        /// </summary>
        public virtual UIFont Font
        {
            get => InputTextField.Font;
            set => InputTextField.Font = value;
        }
        internal void SetItems(IEnumerable<object> items, Func<object, string> labelFunc)
        {
            this.textFunc = labelFunc;
            if (SelectionList is null)
                return;
            if (SelectionList.Source is TableSource<object> oldSource)
            {
                oldSource.TableRowSelected -= SuggestionTableSource_TableRowSelected;
            }
            SelectionList.Source = null;
            IEnumerable<object> suggestions = items?.OfType<object>();
            if (suggestions != null && suggestions.Any())
            {
                TableSource<object> suggestionTableSource = new TableSource<object>(suggestions, labelFunc);
                suggestionTableSource.TableRowSelected += SuggestionTableSource_TableRowSelected;
                SelectionList.Source = suggestionTableSource;

                SelectionList.ReloadData();

                var f = SelectionList.Frame;
                var height = Math.Min(AutocompleteTableViewHeight, (int)SelectionList.ContentSize.Height);
                var frame = new CGRect(f.X, f.Y, f.Width, height);
                SelectionList.Frame = frame;
                _background.Frame = frame;
            }
            else
            {
                IsSuggestionListOpen = false;
            }
        }
        /// <summary>
        /// Gets or sets the placeholder text to be displayed in the <see cref="InputTextField"/>.
        /// </summary>
        public virtual string PlaceholderText
        {
            get => InputTextField.Placeholder;
            set => InputTextField.Placeholder = value;
        }
        /// <summary>
        /// Gets or sets the color of the <see cref="PlaceholderText"/> in the <see cref="InputTextField" />.
        /// </summary>
        /// <param name="color">color</param>
        public virtual void SetPlaceholderTextColor(Xamarin.Forms.Color color)
        {
            // See https://github.com/xamarin/Xamarin.Forms/blob/4d9a5bf3706778770026a18ae81a7dd5c4c15db4/Xamarin.Forms.Platform.iOS/Renderers/EntryRenderer.cs#L260
            InputTextField.AttributedPlaceholder = new NSAttributedString(InputTextField.Placeholder ?? string.Empty, null, ColorExtensions.ToUIColor(color));
        }
        private bool _isSuggestionListOpen;
        private nfloat framewidth;


        /// <summary>
                /// Gets or sets a Boolean value indicating whether the drop-down portion of the AutoSuggestBox is open.
                /// </summary>
        public virtual bool IsSuggestionListOpen
        {
            get => _isSuggestionListOpen;
            set
            {


                _isSuggestionListOpen = value;
                if (SelectionList is object)
                {
                    SelectionList.Hidden = !value;

                    _background.Hidden = !value;

                    UpdateTableConstraints();
                }

                if (!value)
                {
                    InputTextField.ResignFirstResponder();
                }
                ShowSuggestionList?.Invoke(this, value);
            }
        }

        private void UpdateTableConstraints()
        {
            if (IsSuggestionListOpen)
            {
                SelectionList.TopAnchor.ConstraintEqualTo(InputTextField.BottomAnchor).Active = true;
                SelectionList.LeftAnchor.ConstraintEqualTo(InputTextField.LeftAnchor).Active = true;
                SelectionList.WidthAnchor.ConstraintEqualTo(InputTextField.WidthAnchor).Active = true;
                //bottomConstraint = SelectionList.BottomAnchor.ConstraintGreaterThanOrEqualTo(SelectionList.Superview.BottomAnchor, -keyboardHeight);
                //bottomConstraint.Active = true;
                SelectionList.HeightAnchor.ConstraintEqualTo(AutocompleteTableViewHeight).Active = true;
                SelectionList.UpdateConstraints();

                _background.TopAnchor.ConstraintEqualTo(InputTextField.BottomAnchor).Active = true;
                _background.LeftAnchor.ConstraintEqualTo(InputTextField.LeftAnchor).Active = true;
                _background.WidthAnchor.ConstraintEqualTo(InputTextField.WidthAnchor).Active = true;
                _background.HeightAnchor.ConstraintEqualTo(AutocompleteTableViewHeight).Active = true;
                _background.UpdateConstraints();
            }
        }

        public void ShowHideDropdown(UITextField arg)
        {
            if (Instances.Count > 1)
            {
                foreach (var item in Instances)
                {
                    if(item.InputTextField.Tag != arg.Tag)
                    {
                        item.IsSuggestionListOpen = false;
                    }
                }

                var data = Instances.FirstOrDefault(x => x.InputTextField.Tag == arg.Tag);
                if (data is object && data.IsSuggestionListOpen)
                {
                    data.IsSuggestionListOpen = false;
                }
                else
                {
                    data.IsSuggestionListOpen = true;
                }
               

            }
            
        }

        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
            System.Diagnostics.Debug.WriteLine("Touch began :" + touches.FirstOrDefault());

        }


        /// <summary>
        /// Gets or sets a value indicating whether items in the view will trigger an update of the editable text part of the AutoSuggestBox when clicked.
        /// </summary>
        public virtual bool UpdateTextOnSelect { get; set; } = true;
        private void OnKeyboardHide(object sender, UIKeyboardEventArgs e)
        {
            keyboardHeight = 0;
            if (bottomConstraint != null)
            {
                bottomConstraint.Constant = keyboardHeight;
                SelectionList.UpdateConstraints();
            }
        }
        private void OnKeyboardShow(object sender, UIKeyboardEventArgs e)
        {
            NSValue nsKeyboardBounds = (NSValue)e.Notification.UserInfo.ObjectForKey(UIKeyboard.FrameEndUserInfoKey);
            RectangleF keyboardBounds = nsKeyboardBounds.RectangleFValue;
            keyboardHeight = keyboardBounds.Height;
            if (bottomConstraint != null)
            {
                bottomConstraint.Constant = -keyboardHeight;
                //SelectionList.UpdateConstraints();

                var frame = new CGRect(_rect.X, _rect.Y - AutocompleteTableViewHeight, framewidth, AutocompleteTableViewHeight);
                SelectionList.Frame = frame;
                _background.Frame = frame;
            }


        }

        private bool InputText_OnShouldReturn(UITextField textField)
        {
            if (string.IsNullOrEmpty(textField.Text))
            {
                return false;
            }

            textField.ResignFirstResponder();
            return true;
        }
        public override bool BecomeFirstResponder()
        {
            return InputTextField.BecomeFirstResponder();
        }
        public override bool ResignFirstResponder()
        {
            return InputTextField.ResignFirstResponder();
        }

        public override bool IsFirstResponder => InputTextField.IsFirstResponder;


        private void SuggestionTableSource_TableRowSelected(object sender, TableRowSelectedEventArgs<object> e)
        {
            SelectionList.DeselectRow(e.SelectedItemIndexPath, false);
            object selection = e.SelectedItem;
            if (UpdateTextOnSelect)
            {
                InputTextField.Text = textFunc(selection);
            }
            SuggestionChosen?.Invoke(this, new AutoSuggestBoxSuggestionChosenEventArgs(selection));
            IsSuggestionListOpen = false;
        }
        private void InputText_EditingChanged(object sender, EventArgs e)
        {
            IsSuggestionListOpen = true;
        }
        /// <summary>
        /// Gets or sets the text displayed in the <see cref="InputTextField"/>
        /// </summary>
        public virtual string Text
        {
            get => InputTextField.Text;
            set
            {
                InputTextField.Text = value;
            }
        }
        /// <summary>
        /// Assigns the text color to the <see cref="InputTextField"/>
        /// </summary>
        /// <param name="color">color</param>
        public virtual void SetTextColor(Xamarin.Forms.Color color)
        {
            InputTextField.TextColor = ColorExtensions.ToUIColor(color);
        }
        /// <summary>
        /// Raised before the text content of the editable control component is updated.
        /// </summary>
        public event EventHandler<AutoSuggestBoxSuggestionChosenEventArgs> SuggestionChosen;
        public event EventHandler<bool> ShowSuggestionList;
        private class TableSource<T> : UITableViewSource
        {
            private readonly IEnumerable<T> _items;
            private readonly Func<T, string> _labelFunc;
            private readonly string _cellIdentifier;
            public TableSource(IEnumerable<T> items, Func<T, string> labelFunc)
            {
                _items = items;
                _labelFunc = labelFunc;
                _cellIdentifier = Guid.NewGuid().ToString();
            }
            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
            {
                UITableViewCell cell = tableView.DequeueReusableCell(_cellIdentifier);
                if (cell == null)
                    cell = new UITableViewCell(UITableViewCellStyle.Default, _cellIdentifier);
                T item = _items.ElementAt(indexPath.Row);
                cell.TextLabel.Text = _labelFunc(item);
                return cell;
            }
            public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
            {
                OnTableRowSelected(indexPath);
            }
            public override nint RowsInSection(UITableView tableview, nint section)
            {
                return _items.Count();
            }
            public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
            {
                return UITableView.AutomaticDimension;
            }
            public event EventHandler<TableRowSelectedEventArgs<T>> TableRowSelected;
            private void OnTableRowSelected(NSIndexPath itemIndexPath)
            {
                T item = _items.ElementAt(itemIndexPath.Row);
                string label = _labelFunc(item);
                TableRowSelected?.Invoke(this, new TableRowSelectedEventArgs<T>(item, label, itemIndexPath));
            }
        }
        private class TableRowSelectedEventArgs<T> : EventArgs
        {
            public TableRowSelectedEventArgs(T selectedItem, string selectedItemLabel, NSIndexPath selectedItemIndexPath)
            {
                SelectedItem = selectedItem;
                SelectedItemLabel = selectedItemLabel;
                SelectedItemIndexPath = selectedItemIndexPath;
            }
            public T SelectedItem { get; }
            public string SelectedItemLabel { get; }
            public NSIndexPath SelectedItemIndexPath { get; }
        }
    }
    public sealed class AutoSuggestBoxSuggestionChosenEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AutoSuggestBoxSuggestionChosenEventArgs"/> class.
        /// </summary>
        /// <param name="selectedItem"></param>
        internal AutoSuggestBoxSuggestionChosenEventArgs(object selectedItem)
        {
            SelectedItem = selectedItem;
        }
        /// <summary>
        /// Gets a reference to the selected item.
        /// </summary>
        /// <value>A reference to the selected item.</value>
        public object SelectedItem { get; }
    }
    public class AutoCompleteTableView : UITableView
    {
        private readonly UIScrollView _parentScrollView;

        public AutoCompleteTableView(UIScrollView parentScrollView)
        {
            _parentScrollView = parentScrollView;
        }

        public override bool Hidden
        {
            get { return base.Hidden; }
            set
            {
                base.Hidden = value;
                if (_parentScrollView == null) return;
                _parentScrollView.DelaysContentTouches = !value;
            }
        }
    }

}

