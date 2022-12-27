using ErdCsharp.Domain;
using ErdCsharp.Provider.Dtos.API.Transactions;

namespace ErdCsharpPrivateSigner.Helper
{
    public static class TransactionRequestHelper
    {
        public static TransactionRequestDto GetTransactionRequest(this TransactionRequest transactionRequest)
        {
            return new TransactionRequestDto()
            {
                ChainID = transactionRequest.ChainId,
                Data = transactionRequest.Data,
                GasLimit = transactionRequest.GasLimit.Value,
                GasPrice = transactionRequest.GasPrice,
                Nonce = transactionRequest.Nonce,
                Receiver = transactionRequest.Receiver.Bech32,
                Sender = transactionRequest.Sender.Bech32,
                Signature = null,
                Value = transactionRequest.Value.ToString(),
                Version = transactionRequest.TransactionVersion
            };
        }
    }
}
