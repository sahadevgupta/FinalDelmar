using FormsLoyalty.Repos;
using LSRetail.Omni.Domain.DataModel.Loyalty.ScanSend;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace FormsLoyalty.Services
{
    public class ScanSendManager : IScanSendManager
    {
        IScanSendRepo _scanSendRepo;
        public ScanSendManager(IScanSendRepo scanSendRepo)
        {
            _scanSendRepo = scanSendRepo;
        }
        public async Task<bool> CreateScanSend(List<ScanSend> scanSends)
        {
            bool IsSuccess = false;
            foreach (var scansend in scanSends)
            {
                await Task.Run(() =>
                 {
                     try
                     {
                         var obj = _scanSendRepo.ScanSendCreate(scansend);
                         if (obj != null)
                         {
                             IsSuccess = true;
                         }
                         else
                             IsSuccess = false;
                     }
                     catch (Exception ex)
                     {
                         Debug.WriteLine(ex.Message);
                         IsSuccess = false;


                     }
                    
                    
                 });
                
            }
            return IsSuccess;
        }


    }
}
