﻿using System.Collections.Generic;

using Infrastructure.Data.SQLite.DB.DTO;
using LSRetail.Omni.Domain.DataModel.Loyalty.Members;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.Domain.DataModel.Base.Utils;
using LSRetail.Omni.Domain.DataModel.Base.Setup;
using LSRetail.Omni.Domain.DataModel.Loyalty.Setup;

namespace Infrastructure.Data.SQLite.MemberContacts
{
    public class MemberContactFactory
    {
        public MemberContact BuildEntity(MemberContactData memberContact)
        {
            var entity = new MemberContact(memberContact.ContactId)
                {
                    Email = memberContact.Email,
                    FirstName = memberContact.FirstName,
                    Initials = memberContact.Initials,
                    LastName = memberContact.LastName,
                    MiddleName = memberContact.MiddleName,
                    Phone = memberContact.Phone,
                    MobilePhone = memberContact.Mobile,
                    UserName = memberContact.UserName,
                    Addresses = new List<Address>()
                        {
                            new Address() {
                                Address1 = memberContact.AddressOne,
                                Address2 = memberContact.AddressTwo,
                                City = memberContact.City,
                                StateProvinceRegion = memberContact.State,
                                PostCode = memberContact.PostCode,
                                Country = memberContact.Country,
                            }
                        },
                    Account = new Account(memberContact.MemberAccountId)
                        {
                            PointBalance = memberContact.PointBalance,
                            Scheme = new Scheme()
                                {
                                    Description = memberContact.MemberSchemeDescription,
                                    Perks = memberContact.MemberSchemePerks,
                                    PointsNeeded = memberContact.MemberSchemePointsNeeded,
                                    NextScheme = new Scheme()
                                        {
                                            Description = memberContact.NextMemberSchemeDescription,
                                            Perks = memberContact.NextMemberSchemePerks,
                                            PointsNeeded = memberContact.NextMemberSchemePointsNeeded,
                                        }
                                }
                        },
                    Environment = new OmniEnvironment()
                        {
                            Version = memberContact.Version,
                            Currency = new Currency(memberContact.CurrencyId)
                                {
                                    DecimalPlaces = memberContact.CurrencyDecimalPlaces,
                                    DecimalSeparator = memberContact.CurrencyDecimalSeparator,
                                    Postfix = memberContact.CurrencyPostfix,
                                    Prefix = memberContact.CurrencyPrefix,
                                    Symbol = memberContact.CurrencySymbol,
                                    ThousandSeparator = memberContact.CurrencyThousandSeparator,
                                }
                        }

                };

            return entity;
        }

        public MemberContactData BuildEntity(MemberContact memberContact)
        {
            var entity = new MemberContactData()
            {
                Email = memberContact.Email,
                FirstName = memberContact.FirstName,
                Initials = memberContact.Initials,
                LastName = memberContact.LastName,
                MiddleName = memberContact.MiddleName,
                Phone = memberContact.Phone,
                Mobile = memberContact.MobilePhone,
                UserName = memberContact.UserName,
                ContactId = memberContact.Id,
                MemberAccountId = memberContact.Account.Id,
                PointBalance = memberContact.Account.PointBalance,
            };

            if (memberContact.Addresses != null && memberContact.Addresses.Count > 0)
            {
                entity.AddressOne = memberContact.Addresses[0].Address1;
                entity.AddressTwo = memberContact.Addresses[0].Address2;
                entity.City = memberContact.Addresses[0].City;
                entity.State = memberContact.Addresses[0].StateProvinceRegion;
                entity.PostCode = memberContact.Addresses[0].PostCode;
                entity.Country = memberContact.Addresses[0].Country;
            }

            if (memberContact.Environment != null)
            {

                entity.Version = memberContact.Environment.Version;
                entity.CurrencyId = memberContact.Environment.Currency.Id;
                entity.CurrencyDecimalPlaces = memberContact.Environment.Currency.DecimalPlaces;
                entity.CurrencyDecimalSeparator = memberContact.Environment.Currency.DecimalSeparator;
                entity.CurrencyPostfix = memberContact.Environment.Currency.Postfix;
                entity.CurrencyPrefix = memberContact.Environment.Currency.Prefix;
                entity.CurrencySymbol = memberContact.Environment.Currency.Symbol;
                entity.CurrencyThousandSeparator = memberContact.Environment.Currency.ThousandSeparator;
            }

            if (memberContact.Account.Scheme is object)
            {
                entity.MemberSchemeDescription = memberContact.Account.Scheme.Description;
                entity.MemberSchemePerks = memberContact.Account.Scheme.Perks;
                entity.MemberSchemePointsNeeded = memberContact.Account.Scheme.PointsNeeded;
                entity.NextMemberSchemeDescription = memberContact.Account.Scheme.NextScheme.Description;
                entity.NextMemberSchemePerks = memberContact.Account.Scheme.NextScheme.Perks;
                entity.NextMemberSchemePointsNeeded = memberContact.Account.Scheme.NextScheme.PointsNeeded;
            }

            return entity;
        }
    }
}
