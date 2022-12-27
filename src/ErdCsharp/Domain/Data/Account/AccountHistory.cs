using System;
using ErdCsharp.Domain.Helper;
using ErdCsharp.Domain.Values;
using ErdCsharp.Provider.Dtos.API.Account;

namespace ErdCsharp.Domain.Data.Account
{
    public class AccountHistory
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

        private AccountHistory() { }

        public static AccountHistory From(AccountHistoryDto accountHistory)
        {
            return new AccountHistory()
            {
                Address = Address.FromBech32(accountHistory.Address),
                Balance = ESDTAmount.From(accountHistory.Balance),
                HistoryTime = Converter.TimestampToDateTime(accountHistory.Timestamp),
                IsSender = accountHistory.IsSender,
            };
        }
    }
}
