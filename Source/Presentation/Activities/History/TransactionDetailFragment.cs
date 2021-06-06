using System;
using System.Linq;

using Android.Content;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Presentation.Activities.Base;
using Presentation.Activities.Items;
using Presentation.Adapters;
using Presentation.Models;
using Presentation.Util;
using LSRetail.Omni.Domain.DataModel.Base.SalesEntries;
using Presentation.Views;
using LSRetail.Omni.Domain.DataModel.Loyalty.Util;
using HockeyApp.Android;
using LSRetail.Omni.Domain.DataModel.Loyalty.Baskets;

namespace Presentation.Activities.History
{
    public class TransactionDetailFragment : LoyaltyFragment, IRefreshableActivity, IItemClickListener
    {
        private ItemModel itemModel;

        private string transactionId;
        private SalesEntry transaction;
        private TransactionModel model;

        private RecyclerView transactionDetailRecyclerView;
        private View loadingView;
        private View content;
        private ViewSwitcher switcher;
        private ProgressButton ReorderBtn;
        private BasketModel basketModel;

        private TransactionDetailAdapter adapter;

        public static TransactionDetailFragment NewInstance()
        {
            var transactionDetail = new TransactionDetailFragment() { Arguments = new Bundle() };
            return transactionDetail;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            if (container == null)
            {
                // Currently in a layout without a container, so no reason to create our view.
                return null;
            }
            basketModel = new BasketModel(Activity, this);

            //progressDialog = new CustomProgressDialog(Activity);

            Bundle data = Arguments;
            transactionId = data.GetString(BundleConstants.TransactionId);

            model = new TransactionModel(Activity, this);
            itemModel = new ItemModel(Activity, this);

            var view = Util.Utils.ViewUtils.Inflate(inflater, Resource.Layout.TransactionDetail, null);

            var toolbar = view.FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.TransactionDetailScreenToolbar);
            (Activity as LoyaltyFragmentActivity).SetSupportActionBar(toolbar);

            ReorderBtn = view.FindViewById<ProgressButton>(Resource.Id.HistoryViewReorderButton);
            ReorderBtn.Click += ReorderBtnEvent;
            switcher = view.FindViewById<ViewSwitcher>(Resource.Id.TransactionDetailViewSwitcher);
            content = view.FindViewById(Resource.Id.TransactionDetailViewContent);
            loadingView = view.FindViewById(Resource.Id.TransactionDetailViewLoadingSpinner);

            transactionDetailRecyclerView = view.FindViewById<RecyclerView>(Resource.Id.TransactionDetailViewTransactionDetailList);
            transactionDetailRecyclerView.SetLayoutManager(new LinearLayoutManager(Activity, LinearLayoutManager.Vertical, false));
            transactionDetailRecyclerView.AddItemDecoration(new TransactionDetailAdapter.TransactionDetailItemDecoration(Activity));
            transactionDetailRecyclerView.HasFixedSize = true;

            adapter = new TransactionDetailAdapter(Activity, this);
            transactionDetailRecyclerView.SetAdapter(adapter);

            transaction = AppData.Device.UserLoggedOnToDevice.SalesEntries.FirstOrDefault(x => x.Id == transactionId);

            LoadTransaction();

            return view;
        }

        private void ReorderBtnEvent(object sender, EventArgs e)
        {

            if (ReorderBtn.State == ProgressButton.ProgressButtonState.Normal)
            {
                ReorderBtn.State = ProgressButton.ProgressButtonState.Loading;
                AddToBasket(transaction);
            }
            else if (ReorderBtn.State == ProgressButton.ProgressButtonState.Done)
            {
                if (Activity is LoyaltyFragmentActivity)
                {
                    (Activity as LoyaltyFragmentActivity).OpenDrawer((int)GravityFlags.End);
                }

                ReorderBtn.State = ProgressButton.ProgressButtonState.Normal;
            }

        }

        private async void AddToBasket(SalesEntry transaction)
        {
            if (!EnabledItems.ForceLogin && AppData.Device.UserLoggedOnToDevice == null)
            {
                var intent = new Intent();
                intent.SetClass(Activity, typeof(LoginActivity));
                intent.PutExtra(BundleConstants.ErrorMessage, GetString(Resource.String.ApplicationMustBeLoggedIn));

                StartActivity(intent);

                return;
            }
            foreach (var line in transaction.Lines)
            {
                if (line.ItemId == GetString(Resource.String.ShipmentItemID))
                    continue;

                var Item = await itemModel.GetItemById(line.ItemId,false);


                OneListItem basketItem = new OneListItem()
                {
                    ItemId = Item.Id,
                    ItemDescription = Item.Description,
                    Image = Item.DefaultImage,
                    Quantity = line.Quantity,
                    Price = Item.AmtFromVariantsAndUOM(Item.SelectedVariant?.Id, Item.SelectedUnitOfMeasure?.Id)
                };

                if (Item.SelectedVariant != null)
                {
                    basketItem.VariantId = Item.SelectedVariant.Id;
                    basketItem.VariantDescription = Item.SelectedVariant.ToString();
                }

                if (Item.SelectedUnitOfMeasure != null)
                {
                    basketItem.UnitOfMeasureId = Item.SelectedUnitOfMeasure.Id;
                    basketItem.UnitOfMeasureDescription = Item.SelectedUnitOfMeasure.Description;
                }

                if (string.IsNullOrEmpty(basketItem.VariantId) && Item.VariantsRegistration != null && Item.VariantsRegistration.Count > 0)
                {
                    BaseModel.ShowStaticSnackbar(BaseModel.CreateStaticSnackbar(Activity, GetString(Resource.String.ItemViewPickVariant)));
                   // SelectVariant();
                }
                else
                {
                    await basketModel.AddItemToBasket(basketItem,openBasket:true ,ShowIndicatorOption:false);

                }





            }
            ReorderBtn.State = ProgressButton.ProgressButtonState.Done;


            /**
             *
             *
             * 
             * 
            if (!EnabledItems.ForceLogin && AppData.Device.UserLoggedOnToDevice == null)
            {
                var intent = new Intent();
                intent.SetClass(Activity, typeof(LoginActivity));
                intent.PutExtra(BundleConstants.ErrorMessage, GetString(Resource.String.ApplicationMustBeLoggedIn));

                StartActivity(intent);

                return;
            }

            OneListItem basketItem = new OneListItem()
            {
                ItemId = Item.Id,
                ItemDescription = Item.Description,
                Image = Item.DefaultImage,
                Quantity = qty,
                Price = Item.AmtFromVariantsAndUOM(Item.SelectedVariant?.Id, Item.SelectedUnitOfMeasure?.Id)
            };

            if (Item.SelectedVariant != null)
            {
                basketItem.VariantId = Item.SelectedVariant.Id;
                basketItem.VariantDescription = Item.SelectedVariant.ToString();
            }

            if (Item.SelectedUnitOfMeasure != null)
            {
                basketItem.UnitOfMeasureId = Item.SelectedUnitOfMeasure.Id;
                basketItem.UnitOfMeasureDescription = Item.SelectedUnitOfMeasure.Description;
            }

            if (string.IsNullOrEmpty(basketItem.VariantId) && Item.VariantsRegistration != null && Item.VariantsRegistration.Count > 0)
            {
                BaseModel.ShowStaticSnackbar(BaseModel.CreateStaticSnackbar(Activity,GetString(Resource.String.ItemViewPickVariant)));
                SelectVariant();
            }
            else
            {
                await basketModel.AddItemToBasket(basketItem);
                addToBasketButton.State = ProgressButton.ProgressButtonState.Done;
            }
             * 
             * 
             * 
             * 
             * 
             * */
        }


        private async void LoadTransaction()
        {
            try
            {
                var loadedTransation = await model.GetTransactionByReceiptNo(transaction.Id, transaction.IdType);
               

                if (loadedTransation == null)
                {
                    Activity.OnBackPressed();
                }
                else
                {
                    this.transaction = loadedTransation;
                    ShowDetails();
                }
            }
            catch (Exception ex)
            {

               await model.HandleUIExceptionAsync(ex);

            }
        }

        public override void OnDestroyView()
        {
            if(model != null)
                model.Stop();

            base.OnDestroyView();
        }

        private void ShowLoading()
        {
            if(switcher.CurrentView != loadingView)
                switcher.ShowPrevious();
        }

        private void ShowContent()
        {
            if (switcher.CurrentView != content)
                switcher.ShowNext();
        }

        private void ShowDetails()
        {
            var dateHeader = View.FindViewById<TextView>(Resource.Id.TransactionDetailViewDateHeader);
            if (dateHeader == null)
            {
                dateHeader.Visibility = ViewStates.Gone;
            }
            else
            {
                dateHeader.Text = transaction.DocumentRegTime.ToString("D");
            }

            adapter.SetTransaction(Activity, transaction);

            Crossfade();
        }

        private void Crossfade()
        {
            ShowContent();
        }

        public void ShowIndicator(bool show)
        {
            if(show)
                ShowLoading();
            else
                ShowContent();
        }

        public void ItemClicked(int type, string id, string id2, View view)
        {
            var intent = new Intent();

            var saleLine = transaction.Lines.FirstOrDefault(x => x.Id == id);

            if (saleLine != null)
            {
                intent.PutExtra(BundleConstants.ItemId, saleLine.ItemId);

                if (AppData.IsDualScreen)
                    intent.PutExtra(BundleConstants.LoadContainer, true);

                intent.SetClass(Activity, typeof(ItemActivity));
                StartActivity(intent);
            }
        }
    }
}