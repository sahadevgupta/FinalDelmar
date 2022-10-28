﻿using System;
using Infrastructure.Data.SQLite.DB.DTO;
using LSRetail.Omni.Domain.DataModel.Loyalty.Setup;
using LSRetail.Omni.Domain.DataModel.Loyalty.Setup.SpecialCase;
using LSRetail.Omni.Domain.DataModel.Loyalty.Members;

namespace Infrastructure.Data.SQLite.Devices
{
    internal class DeviceFactory
    {
        public Device BuildEntity(DeviceData deviceData)
        {
            if (deviceData == null)
                return new UnknownDevice();

            Device entity = new Device();
            entity.Id = deviceData.DeviceId;
            entity.Manufacturer = deviceData.Make;
            entity.Model = deviceData.Model;
            entity.SecurityToken = deviceData.SecurityToken;
            entity.CardId = deviceData.CardId;
           
            if (deviceData.UserLoggedOnToDevice == true)
            {
                entity.UserLoggedOnToDevice = new MemberContact(deviceData.ContactId);
                entity.UserLoggedOnToDevice.UserName = deviceData.UserName;
                entity.UserLoggedOnToDevice.Cards = new System.Collections.Generic.List<LSRetail.Omni.Domain.DataModel.Base.Retail.Card>();
                entity.UserLoggedOnToDevice.Cards.Add(new LSRetail.Omni.Domain.DataModel.Base.Retail.Card(deviceData.CardId));
            }
            else
            {
                entity.UserLoggedOnToDevice = null;
            }
            
            return entity;
        }
    }
}
