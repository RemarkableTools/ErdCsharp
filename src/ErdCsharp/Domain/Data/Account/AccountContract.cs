using System;
using System.Linq;
using ErdCsharp.Domain.Data.Common;
using ErdCsharp.Domain.Helper;
using ErdCsharp.Domain.Values;
using ErdCsharp.Provider.Dtos.API.Account;

namespace ErdCsharp.Domain.Data.Account
{
    public class AccountContract
    {
        /// <summary>
        /// Contract address
        /// </summary>
        public Address Address { get; set; }

        /// <summary>
        /// Contract deploy transactions jash
        /// </summary>
        public string DeployTxHash { get; set; }

        /// <summary>
        /// Contract deploy date
        /// </summary>
        public DateTime DeployDate { get; set; }

        /// <summary>
        /// Contract assets
        /// </summary>
        public Assets Assets { get; set; }

        private AccountContract() { }

        /// <summary>
        /// Creates a new array of AccountContract objects from data
        /// </summary>
        /// <param name="accountContracts">Array of AccountContract Data Objects from API</param>
        /// <returns>Array of AccountContract objects</returns>
        public static AccountContract[] From(AccountContractDto[] accountContracts)
        {
            return accountContracts.Select(accountContract => new AccountContract()
            {
                Address = Address.FromBech32(accountContract.Address),
                DeployTxHash = accountContract.DeployTxHash,
                DeployDate = Converter.TimestampToDateTime(accountContract.Timestamp),
                Assets = Assets.From(accountContract.Assets)
            }).ToArray();
        }
    }
}
