using System.Collections.Generic;
using System.Text;
using ErdCsharp.Domain;
using ErdCsharp.Domain.Helper;
using ErdCsharp.Provider.Dtos.API.Transactions;
using ErdCsharpPrivateSigner.Helper;

namespace ErdCsharpPrivateSigner
{
    public static class PrivateSigner
    {
        public static TransactionRequestDto Sign(this TransactionRequest transactionRequest, Wallet wallet)
        {
            var transactionRequestDto = transactionRequest.GetTransactionRequest();
            var json = JsonSerializerWrapper.Serialize(transactionRequestDto);
            var message = Encoding.UTF8.GetBytes(json);

            transactionRequestDto.Signature = wallet.Sign(message);
            return transactionRequestDto;
        }

        public static TransactionRequestDto[] MultiSign(this TransactionRequest[] transactionsRequest, Wallet wallet)
        {
            var transactions = new List<TransactionRequestDto>();

            foreach (var transactionRequest in transactionsRequest)
            {
                var transactionRequestDto = transactionRequest.GetTransactionRequest();
                var json = JsonSerializerWrapper.Serialize(transactionRequestDto);
                var message = Encoding.UTF8.GetBytes(json);

                transactionRequestDto.Signature = wallet.Sign(message);
                transactions.Add(transactionRequestDto);
            }

            return transactions.ToArray();
        }
    }
}
