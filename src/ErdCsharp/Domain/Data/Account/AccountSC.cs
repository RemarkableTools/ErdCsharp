﻿using System;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using ErdCsharp.Domain.Data.Common;
using ErdCsharp.Domain.Helper;
using ErdCsharp.Domain.Values;
using ErdCsharp.Provider;
using ErdCsharp.Provider.Dtos.API.Account;

namespace ErdCsharp.Domain.Data.Account
{
    /// <summary>
    /// Account object for an Smart Contract
    /// </summary>
    public class AccountSC
    {
        /// <summary>
        /// Account address
        /// </summary>
        public Address Address { get; private set; }

        /// <summary>
        /// Account EGLD balance
        /// </summary>
        public ESDTAmount Balance { get; private set; }

        /// <summary>
        /// Account nonce
        /// </summary>
        public ulong Nonce { get; private set; }

        /// <summary>
        /// Account shard
        /// </summary>
        public long Shard { get; private set; }

        /// <summary>
        /// Account assets
        /// </summary>
        public dynamic Assets { get; private set; } //JSON data

        /// <summary>
        /// Smart Contract code
        /// </summary>
        public string Code { get; private set; }

        /// <summary>
        /// Smart Contract code hash
        /// </summary>
        public string CodeHash { get; private set; }

        /// <summary>
        /// Account root hash
        /// </summary>
        public string RootHash { get; private set; }

        /// <summary>
        /// The number of transactions of Account
        /// </summary>
        public BigInteger TxCount { get; private set; }

        /// <summary>
        /// The number of transactions with smart contracts of Account
        /// </summary>
        public BigInteger SrcCount { get; private set; }

        /// <summary>
        /// Account user name (herotag)
        /// </summary>
        public string UserName { get; private set; }

        /// <summary>
        /// Account developer reward
        /// </summary>
        public string DeveloperReward { get; private set; }

        /// <summary>
        /// Account scam info
        /// </summary>
        public ScamInfo ScamInfo { get; private set; }

        /// <summary>
        /// Smart Contract owner address
        /// </summary>
        public string OwnerAddress { get; private set; }

        /// <summary>
        /// Smart Contract deployed time
        /// </summary>
        public DateTime DeployedAt { get; private set; }

        /// <summary>
        /// Smart Contract is upgradable
        /// </summary>
        public bool IsUpgradable { get; private set; }

        /// <summary>
        /// Smart Contract is readable
        /// </summary>
        public bool IsReadable { get; private set; }

        /// <summary>
        /// Smart Contract is payable
        /// </summary>
        public bool IsPayable { get; private set; }

        /// <summary>
        /// Smart Contract is payable by other Smart Contracts
        /// </summary>
        public bool IsPayableSmartContract { get; private set; }

        private AccountSC() { }

        /// <summary>
        /// Synchronizes Smart Contract account properties with the ones queried from the Network
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public async Task Sync(IMultiversxProvider provider)
        {
            var accountDto = await provider.GetAccount(Address.Bech32);

            Balance = ESDTAmount.From(accountDto.Balance, ESDT.EGLD());
            Nonce = accountDto.Nonce;
            Shard = accountDto.Shard;
            Assets = accountDto.Assets;
            Code = accountDto.Code;
            if (accountDto.CodeHash != null) CodeHash = Encoding.UTF8.GetString(Convert.FromBase64String(accountDto.CodeHash));
            if (accountDto.RootHash != null) RootHash = Encoding.UTF8.GetString(Convert.FromBase64String(accountDto.RootHash));
            TxCount = accountDto.TxCount;
            SrcCount = accountDto.ScrCount;
            UserName = accountDto.UserName;
            DeveloperReward = accountDto.DeveloperReward;
            OwnerAddress = accountDto.OwnerAddress;
            DeployedAt = Converter.TimestampToDateTime(accountDto.DeployedAt);
            IsUpgradable = accountDto.IsUpgradable;
            IsReadable = accountDto.IsReadable;
            IsPayable = accountDto.IsPayable;
            IsPayableSmartContract = accountDto.IsPayableSmartContract;
            ScamInfo = ScamInfo.From(accountDto.ScamInfo);
        }

        /// <summary>
        /// Creates a new Smart Contract account object from data
        /// </summary>
        /// <param name="account"></param>
        /// <returns>AccountSC object</returns>
        public static AccountSC From(AccountDto account)
        {
            return new AccountSC()
            {
                Address = Address.FromBech32(account.Address),
                Balance = ESDTAmount.From(account.Balance, ESDT.EGLD()),
                Nonce = account.Nonce,
                Shard = account.Shard,
                Assets = account.Assets,
                Code = account.Code,
                CodeHash = Encoding.UTF8.GetString(Convert.FromBase64String(account.CodeHash)),
                RootHash = Encoding.UTF8.GetString(Convert.FromBase64String(account.RootHash)),
                TxCount = account.TxCount,
                SrcCount = account.ScrCount,
                UserName = account.UserName,
                DeveloperReward = account.DeveloperReward,
                OwnerAddress = account.OwnerAddress,
                DeployedAt = Converter.TimestampToDateTime(account.DeployedAt),
                IsUpgradable = account.IsUpgradable,
                IsReadable = account.IsReadable,
                IsPayable = account.IsPayable,
                IsPayableSmartContract = account.IsPayableSmartContract,
                ScamInfo = ScamInfo.From(account.ScamInfo)
            };
        }
    }
}
