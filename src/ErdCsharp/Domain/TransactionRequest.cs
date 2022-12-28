﻿using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ErdCsharp.Domain.Codec;
using ErdCsharp.Domain.Const;
using ErdCsharp.Domain.Data.Account;
using ErdCsharp.Domain.Data.Network;
using ErdCsharp.Domain.Data.Transaction;
using ErdCsharp.Domain.Exceptions;
using ErdCsharp.Domain.Helper;
using ErdCsharp.Domain.SmartContracts;
using ErdCsharp.Domain.Values;
using ErdCsharp.Provider;
using ErdCsharp.Provider.Dtos.API.Transactions;

namespace ErdCsharp.Domain
{
    public class TransactionRequest
    {
        private static readonly BinaryCodec binaryCoder = new BinaryCodec();
        private readonly NetworkConfig networkConfig;

        public readonly string ChainId;
        public readonly int TransactionVersion = 4;
        public Account Account { get; }
        public Address Sender { get; }
        public ulong Nonce { get; }
        public long GasPrice { get; }
        public ESDTAmount Value { get; private set; }
        public Address Receiver { get; private set; }
        public GasLimit GasLimit { get; private set; }
        public string Data { get; private set; }

        private TransactionRequest(Account account, NetworkConfig networkConfig)
        {
            this.networkConfig = networkConfig;
            ChainId = networkConfig.ChainId;
            Account = account;
            Sender = account.Address;
            Receiver = Address.Zero();
            Value = ESDTAmount.Zero();
            Nonce = account.Nonce;
            GasLimit = new GasLimit(networkConfig.MinGasLimit);
            GasPrice = networkConfig.MinGasPrice;
        }

        public static TransactionRequest Create(Account account, NetworkConfig networkConfig)
        {
            return new TransactionRequest(account, networkConfig);
        }

        public static TransactionRequest Create(Account account, NetworkConfig networkConfig, Address receiver, ESDTAmount value)
        {
            return new TransactionRequest(account, networkConfig) { Receiver = receiver, Value = value };
        }

        public static TransactionRequest CreateEgldTransactionRequest(
            NetworkConfig networkConfig,
            Account account,
            Address address,
            ESDTAmount value,
            string message = null)
        {
            var transaction = Create(account, networkConfig, address, value);
            transaction.Data = message;
            transaction.SetGasLimit(GasLimit.ForTransaction(networkConfig, transaction));
            return transaction;
        }

        public static TransactionRequest CreateDeploySmartContractTransactionRequest(
            NetworkConfig networkConfig,
            Account account,
            CodeArtifact codeArtifact,
            CodeMetadata codeMetadata,
            params IBinaryType[] args)
        {
            var transaction = Create(account, networkConfig);
            var data = $"{codeArtifact.Value}@{Constants.ArwenVirtualMachine}@{codeMetadata.Value}";
            if (args.Any())
            {
                data = args.Aggregate(data,
                                      (c, arg) => c + $"@{Converter.ToHexString(binaryCoder.EncodeTopLevel(arg))}");
            }

            transaction.Data = DataCoder.EncodeData(data);
            transaction.SetGasLimit(GasLimit.ForSmartContractCall(networkConfig, transaction));
            return transaction;
        }

        public static TransactionRequest CreateCallSmartContractTransactionRequest(
            NetworkConfig networkConfig,
            Account account,
            Address address,
            ESDTAmount value,
            string methodName,
            params IBinaryType[] args)
        {
            var transaction = Create(account, networkConfig, address, value);
            var data = $"{methodName}";
            if (args.Any())
            {
                data = args.Aggregate(data,
                                      (c, arg) => c + $"@{Converter.ToHexString(binaryCoder.EncodeTopLevel(arg))}");
            }

            transaction.Data = DataCoder.EncodeData(data);
            transaction.SetGasLimit(GasLimit.ForSmartContractCall(networkConfig, transaction));
            return transaction;
        }

        public void SetGasLimit(GasLimit gasLimit)
        {
            GasLimit = gasLimit;
        }

        public ESDTAmount GetEstimatedFee()
        {
            if (GasLimit is null)
                throw new GasLimitException.UndefinedGasLimitException();

            var dataBytes = Data is null ? Array.Empty<byte>() : Convert.FromBase64String(Data);

            var dataGas = networkConfig.MinGasLimit + dataBytes.Length * networkConfig.GasPerDataByte;
            if (dataGas > GasLimit.Value)
                throw new GasLimitException.NotEnoughGasException($"Not Enough Gas ({GasLimit.Value}) for transaction");

            var gasPrice = networkConfig.MinGasPrice;
            var transactionGas = dataGas * gasPrice;
            if (dataGas == GasLimit.Value)
                return ESDTAmount.From(transactionGas);

            var remainingGas = GasLimit.Value - dataGas;
            var gasPriceModifier = networkConfig.GasPriceModifier;
            var modifiedGasPrice = gasPrice * double.Parse(gasPriceModifier);
            var surplusFee = remainingGas * modifiedGasPrice;

            return ESDTAmount.From($"{transactionGas + surplusFee}");
        }

        public void AddArgument(IBinaryType[] args)
        {
            if (!args.Any())
                return;

            var binaryCodec = new BinaryCodec();
            var decodedData = DataCoder.DecodeData(Data);
            var data = args.Aggregate(decodedData,
                                      (c, arg) => c + $"@{Converter.ToHexString(binaryCodec.EncodeTopLevel(arg))}");
            Data = DataCoder.EncodeData(data);
        }
    }
}
