using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Android.Content;
using LSRetail.Omni.Domain.DataModel.Base.SalesEntries;
using LSRetail.Omni.Domain.Services.Loyalty.Transactions;
using LSRetail.Omni.Infrastructure.Data.Omniservice.Loyalty.Orders;
using Presentation.Util;

namespace Presentation.Models
{
    public class TransactionModel : BaseModel
    {
        private TransactionService service;
        private TransactionLocalService localService;

        public TransactionModel(Context context, IRefreshableActivity refreshableActivity) : base(context, refreshableActivity)
        {
            this.localService = new TransactionLocalService(new Infrastructure.Data.SQLite.Transactions.TransactionRepository());
        }

        public async Task<SalesEntry> GetTransactionByReceiptNo(string documentId, DocumentIdType documentType)
        {
            SalesEntry transaction = null;

            ShowIndicator(true);

            BeginWsCall();

            try
            {
                transaction = await service.SalesEntryGetByIdAsync(documentId, documentType);
            }
            catch (Exception ex)
            {
               // await HandleUIExceptionAsync(ex);
            }

            ShowIndicator(false);

            return transaction;
        }

        public async Task<List<SalesEntry>> GetTransactionsByCardId(string cardId)
        {
            List<SalesEntry> transactions = null;

            BeginWsCall();

            ShowIndicator(true);

            try
            {
                transactions = await service.GetSalesEntriesAsync(cardId, Int32.MaxValue);
                await SaveLocalTransactions(transactions);
            }
            catch (Exception ex)
            {
                await HandleUIExceptionAsync(ex);
            }

            ShowIndicator(false);

            return transactions;
        }

        private async Task SaveLocalTransactions(List<SalesEntry> transactions)
        {
            await Task.Run(() => localService.SaveTransactions(transactions));
        }

        protected override void CreateService()
        {
            this.service = new TransactionService(new TransactionRepository());
        }
    }
}