﻿using System;
using ErdCsharp.Domain.Data.Common;
using ErdCsharp.Domain.Data.Properties;
using ErdCsharp.Domain.Helper;
using ErdCsharp.Domain.Values;
using ErdCsharp.Provider.Dtos.API.Account;

namespace ErdCsharp.Domain.Data.Account
{
    /// <summary>
    /// Account Collection object with roles
    /// </summary>
    public class AccountCollectionRole
    {
        /// <summary>
        /// Collection identifier
        /// </summary>
        public ESDTIdentifierValue CollectionIdentifier { get; private set; }

        /// <summary>
        /// Collection type
        /// </summary>
        public string Type { get; private set; }

        /// <summary>
        /// Collection name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Collection ticker
        /// </summary>
        public string Ticker { get; private set; }

        /// <summary>
        /// Collection owner
        /// </summary>
        public Address Owner { get; private set; }

        /// <summary>
        /// Collection creation date
        /// </summary>
        public DateTime CreationDate { get; private set; }

        /// <summary>
        /// Collection properties
        /// </summary>
        public CollectionProperties Properties { get; private set; }

        /// <summary>
        /// Collection decimals (only for Meta ESDT collection)
        /// </summary>
        public int Decimals { get; private set; }

        /// <summary>
        /// Collection assets
        /// </summary>
        public dynamic Assets { get; private set; }

        /// <summary>
        /// The account role in Collection
        /// </summary>
        public CollectionAccountRole Role { get; private set; }

        /// <summary>
        /// Collection scam info
        /// </summary>
        public ScamInfo ScamInfo { get; private set; }

        private AccountCollectionRole() { }

        /// <summary>
        /// Creates a new AccountCollection object from data
        /// </summary>
        /// <param name="collection">AccountCollection Data Object from API</param>
        /// <returns></returns>
        public static AccountCollectionRole From(AccountCollectionRoleDto collection)
        {
            return new AccountCollectionRole()
            {
                CollectionIdentifier = ESDTIdentifierValue.From(collection.Collection),
                Type = collection.Type,
                Name = collection.Name,
                Ticker = collection.Ticker,
                Owner = Address.From(collection.Owner),
                CreationDate = Converter.TimestampToDateTime(collection.Timestamp),
                Properties = CollectionProperties.From(collection.CanFreeze,
                                                       collection.CanWipe,
                                                       collection.CanPause,
                                                       collection.CanTransferNFTCreateRole,
                                                       collection.CanChangeOwner,
                                                       collection.CanUpgrade,
                                                       collection.CanAddSpecialRoles),
                Decimals = collection.Decimals,
                Role = CollectionAccountRole.From(collection.Role),
                ScamInfo = ScamInfo.From(collection.ScamInfo)
            };
        }
    }
}
