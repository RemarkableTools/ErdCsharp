using System;
using ErdCsharp.Domain.Helper;
using ErdCsharp.Domain.Values;
using ErdCsharp.Provider.Dtos.API.Account;

namespace ErdCsharp.Domain.Data.Account
{
    public class AccountHistoryToken
    {
        /// <summary>
        /// Account address
        /// </summary>
        public Address Address { get; set; }

        /// <summary>
        /// Account EGLD Balance
        /// </summary>
        public ESDTAmount Balance { get; set; }

        /// <summary>
        /// History moment
        /// </summary>
        public DateTime HistoryTime { get; set; }

        /// <summary>
        /// Account is sender at that moment
        /// </summary>
        public bool? IsSender { get; set; }

        /// <summary>
        /// Token used at that moment
        /// </summary>
        public string Token { get; set; }

        private AccountHistoryToken() { }

        public static AccountHistoryToken From(AccountHistoryTokenDto accountHistoryToken)
        {
            return new AccountHistoryToken()
            {
                Address = Address.FromBech32(accountHistoryToken.Address),
                Balance = ESDTAmount.From(accountHistoryToken.Balance),
                HistoryTime = Converter.TimestampToDateTime(accountHistoryToken.Timestamp),
                IsSender = accountHistoryToken.IsSender,
                Token = accountHistoryToken.Token
            };
        }
    }
}
