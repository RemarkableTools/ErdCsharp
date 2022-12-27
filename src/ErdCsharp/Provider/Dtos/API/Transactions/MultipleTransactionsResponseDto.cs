using System.Collections.Generic;

namespace ErdCsharp.Provider.Dtos.API.Transactions
{
    public class MultipleTransactionsResponseDto
    {
        public int NumOfSentTxs { get; set; }
        public Dictionary<string, string> TxsHashes { get; set; }
    }
}
