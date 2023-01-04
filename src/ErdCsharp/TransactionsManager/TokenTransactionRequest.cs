﻿using System;
using System.Numerics;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ErdCsharp.Domain;
using ErdCsharp.Domain.Values;
using ErdCsharp.Domain.Data.Account;
using ErdCsharp.Domain.Data.Network;
using ErdCsharp.Domain.Data.Properties;
using static ErdCsharp.Domain.Const.Constants;
using static ErdCsharp.Domain.Const.TokenConst;

namespace ErdCsharp.TransactionsManager
{
    public class TokenTransactionRequest
    {
        private static readonly Regex _nameValidation = new Regex("^[a-zA-Z0-9]{3,20}$");
        private static readonly Regex _tickerValidation = new Regex("^[A-Z0-9]{3,10}$");
        private static readonly Regex _decimalsValidation = new Regex("^([0-9]|1[0-8])$");

        private static readonly Address SYSTEM_SMART_CONTRACT_ADDRESS = Address.FromBech32(ESDT_SMART_CONTRACT);

        private const string ESDT_TRANSFER = "ESDTTransfer";
        private const string ISSUE = "issue";
        private const string ESDT_LOCAL_MINT = "ESDTLocalMint";
        private const string ESDT_LOCAL_BURN = "ESDTLocalBurn";
        private const string PAUSE = "pause";
        private const string UNPAUSE = "unPause";
        private const string FREEZE = "freeze";
        private const string UNFREEZE = "unFreeze";
        private const string WIPE = "wipe";
        private const string SET_SPECIAL_ROLE = "setSpecialRole";
        private const string UNSET_SPECIAL_ROLE = "unSetSpecialRole";
        private const string TRANSFER_OWNERSHIP = "transferOwnership";
        private const string CONTROL_CHANGES = "controlChanges";

        /// <summary>
        /// Create transaction request - FungibleESDT Transfer
        /// </summary>
        /// <param name="networkConfig">MultiversX Network Configuration</param>
        /// <param name="account">Sender Account</param>
        /// <param name="receiver">Receiver address</param>
        /// <param name="tokenIdentifier">Token identifier</param>
        /// <param name="quantity">Nominated quantity (with decimals applied) to transfer</param>
        /// <returns></returns>
        public static TransactionRequest TokenTransfer(
            NetworkConfig networkConfig,
            Account account,
            Address receiver,
            string tokenIdentifier,
            BigInteger quantity)
        {
            var transaction = TransactionRequest.CreateCallSmartContractTransactionRequest(networkConfig,
                                                                                           account,
                                                                                           receiver,
                                                                                           ESDTAmount.Zero(),
                                                                                           ESDT_TRANSFER,
                                                                                           ESDTIdentifierValue.From(tokenIdentifier),
                                                                                           NumericValue.BigUintValue(quantity));

            transaction.SetGasLimit(new GasLimit(500000));

            return transaction;
        }

        /// <summary>
        /// Create transaction request - FungibleESDT Transfer to Smart Contract with default gas limit
        /// </summary>
        /// <param name="networkConfig">MultiversX Network Configuration</param>
        /// <param name="account">Sender Account</param>
        /// <param name="smartContract">Smart Contract destination address</param>
        /// <param name="tokenIdentifier">Token identifier</param>
        /// <param name="quantity">Nominated quantity (with decimals applied) to transfer</param>
        /// <param name="methodName">Smart Contract method to call</param>
        /// <param name="methodArgs">Smart Contract method arguments</param>
        /// <returns></returns>
        public static TransactionRequest TokenTransferToSmartContract(
            NetworkConfig networkConfig,
            Account account,
            Address smartContract,
            string tokenIdentifier,
            BigInteger quantity,
            string methodName,
            params IBinaryType[] methodArgs)
        {
            var arguments = new List<IBinaryType>
            {
                ESDTIdentifierValue.From(tokenIdentifier),
                NumericValue.BigUintValue(quantity),
                BytesValue.FromUtf8(methodName)
            };
            arguments.AddRange(methodArgs);

            var transaction = TransactionRequest.CreateCallSmartContractTransactionRequest(networkConfig,
                                                                                           account,
                                                                                           smartContract,
                                                                                           ESDTAmount.Zero(),
                                                                                           ESDT_TRANSFER,
                                                                                           arguments.ToArray());
            //GasLimit: 500000 + extra for SC call
            transaction.SetGasLimit(500000 + GasLimit.FromData(networkConfig, transaction.Data));

            return transaction;
        }

        /// <summary>
        /// Create transaction request - FungibleESDT Transfer to Smart Contract
        /// </summary>
        /// <param name="networkConfig">MultiversX Network Configuration</param>
        /// <param name="account">Sender Account</param>
        /// <param name="smartContract">Smart Contract destination address</param>
        /// <param name="gasLimit">Gas limit for transaction</param>
        /// <param name="tokenIdentifier">Token identifier</param>
        /// <param name="quantity">Nominated quantity (with decimals applied) to transfer</param>
        /// <param name="methodName">Smart Contract method to call</param>
        /// <param name="methodArgs">Smart Contract method arguments</param>
        /// <returns></returns>
        public static TransactionRequest TokenTransferToSmartContract(
            NetworkConfig networkConfig,
            Account account,
            Address smartContract,
            GasLimit gasLimit,
            string tokenIdentifier,
            BigInteger quantity,
            string methodName,
            params IBinaryType[] methodArgs)
        {
            var arguments = new List<IBinaryType>
            {
                ESDTIdentifierValue.From(tokenIdentifier),
                NumericValue.BigUintValue(quantity),
                BytesValue.FromUtf8(methodName)
            };
            arguments.AddRange(methodArgs);

            var transaction = TransactionRequest.CreateCallSmartContractTransactionRequest(networkConfig,
                                                                                           account,
                                                                                           smartContract,
                                                                                           ESDTAmount.Zero(),
                                                                                           ESDT_TRANSFER,
                                                                                           arguments.ToArray());

            transaction.SetGasLimit(gasLimit);

            return transaction;
        }

        /// <summary>
        /// Create transaction request - Issue a FungibleESDT Token with optional properties
        /// </summary>
        /// <param name="networkConfig">MultiversX Network Configuration</param>
        /// <param name="account">Sender Account</param>
        /// <param name="tokenName">The Token name, length between 3 and 20 characters (alphanumeric characters only)</param>
        /// <param name="tokenTicker">The Token ticker, length between 3 and 10 characters (alphanumeric UPPERCASE only)</param>
        /// <param name="initialSupply">The initial supply</param>
        /// <param name="numberOfDecimals">The number of decimals, should be a numerical value between 0 and 18</param>
        /// <param name="properties">The Token properties</param>
        /// <returns></returns>
        public static TransactionRequest IssueToken(
            NetworkConfig networkConfig,
            Account account,
            string tokenName,
            string tokenTicker,
            BigInteger initialSupply,
            int numberOfDecimals,
            TokenProperties properties = null,
            params IBinaryType[] args)
        {
            var cost = networkConfig.ChainId == "T" ? ESDTAmount.EGLD("5") : ESDTAmount.EGLD("0.05");

            if (!_nameValidation.IsMatch(tokenName))
                throw new ArgumentException("Length should be between 3 and 20 characters, alphanumeric characters only", nameof(tokenName));

            if (!_tickerValidation.IsMatch(tokenTicker))
                throw new ArgumentException("Length should be between 3 and 10 characters, alphanumeric UPPERCASE characters only", nameof(tokenTicker));

            if (!_decimalsValidation.IsMatch(numberOfDecimals.ToString()))
                throw new ArgumentException("Numerical value should be between 0 and 18", nameof(numberOfDecimals));

            var arguments = new List<IBinaryType>
            {
                BytesValue.FromUtf8(tokenName),
                ESDTIdentifierValue.From(tokenTicker),
                NumericValue.BigUintValue(initialSupply),
                NumericValue.I32Value(numberOfDecimals)
            };

            if (properties != null)
            {
                arguments.Add(BytesValue.FromUtf8(ESDTTokenProperties.CanFreeze));
                arguments.Add(BooleanValue.From(properties.CanFreeze));
                arguments.Add(BytesValue.FromUtf8(ESDTTokenProperties.CanWipe));
                arguments.Add(BooleanValue.From(properties.CanWipe));
                arguments.Add(BytesValue.FromUtf8(ESDTTokenProperties.CanPause));
                arguments.Add(BooleanValue.From(properties.CanPause));
                arguments.Add(BytesValue.FromUtf8(ESDTTokenProperties.CanMint));
                arguments.Add(BooleanValue.From(properties.CanMint));
                arguments.Add(BytesValue.FromUtf8(ESDTTokenProperties.CanBurn));
                arguments.Add(BooleanValue.From(properties.CanBurn));
                arguments.Add(BytesValue.FromUtf8(ESDTTokenProperties.CanChangeOwner));
                arguments.Add(BooleanValue.From(properties.CanChangeOwner));
                arguments.Add(BytesValue.FromUtf8(ESDTTokenProperties.CanUpgrade));
                arguments.Add(BooleanValue.From(properties.CanUpgrade));
                arguments.Add(BytesValue.FromUtf8(ESDTTokenProperties.CanAddSpecialRoles));
                arguments.Add(BooleanValue.From(properties.CanAddSpecialRoles ?? true));
                arguments.AddRange(args);
            }
            var transaction = TransactionRequest.CreateCallSmartContractTransactionRequest(networkConfig,
                                                                                           account,
                                                                                           SYSTEM_SMART_CONTRACT_ADDRESS,
                                                                                           cost,
                                                                                           ISSUE,
                                                                                           arguments.ToArray());

            transaction.SetGasLimit(new GasLimit(60000000));

            return transaction;
        }

        /// <summary>
        /// Create transaction request - Local Mint operation
        /// 'ESDTRoleLocalMint' role must have been assigned to account
        /// </summary>
        /// <param name="networkConfig">MultiversX Network Configuration</param>
        /// <param name="account">Sender Account</param>
        /// <param name="tokenIdentifier">Token identifier</param>
        /// <param name="supplyToMint">The new ESDT token supply to add locally</param>
        /// <returns></returns>
        public static TransactionRequest LocalMint(
            NetworkConfig networkConfig,
            Account account,
            string tokenIdentifier,
            BigInteger supplyToMint)
        {
            var transaction = TransactionRequest.CreateCallSmartContractTransactionRequest(networkConfig,
                                                                                           account,
                                                                                           SYSTEM_SMART_CONTRACT_ADDRESS,
                                                                                           ESDTAmount.Zero(),
                                                                                           ESDT_LOCAL_MINT,
                                                                                           ESDTIdentifierValue.From(tokenIdentifier),
                                                                                           NumericValue.BigUintValue(supplyToMint));

            transaction.SetGasLimit(new GasLimit(500000));

            return transaction;
        }

        /// <summary>
        /// Create transaction request - Local Burn operation
        /// 'ESDTRoleLocalBurn' role must have been assigned to account
        /// </summary>
        /// <param name="networkConfig">MultiversX Network Configuration</param>
        /// <param name="account">Sender Account</param>
        /// <param name="tokenIdentifier">Token identifier</param>
        /// <param name="supplyToBurn">The ESDT supply to burn locally</param>
        /// <returns></returns>
        public static TransactionRequest LocalBurn(
            NetworkConfig networkConfig,
            Account account,
            string tokenIdentifier,
            BigInteger supplyToBurn)
        {
            var transaction = TransactionRequest.CreateCallSmartContractTransactionRequest(networkConfig,
                                                                                           account,
                                                                                           SYSTEM_SMART_CONTRACT_ADDRESS,
                                                                                           ESDTAmount.Zero(),
                                                                                           ESDT_LOCAL_BURN,
                                                                                           ESDTIdentifierValue.From(tokenIdentifier),
                                                                                           NumericValue.BigUintValue(supplyToBurn));

            transaction.SetGasLimit(new GasLimit(500000));

            return transaction;
        }

        /// <summary>
        /// Create transaction request - Pause operation (suspend all transactions of the token, except minting, freezing/unfreezing and wiping)
        /// </summary>
        /// <param name="networkConfig">MultiversX Network Configuration</param>
        /// <param name="account">Sender Account</param>
        /// <param name="tokenIdentifier">Token identifier</param>
        /// <returns></returns>
        public static TransactionRequest Pause(
            NetworkConfig networkConfig,
            Account account,
            string tokenIdentifier)
        {
            var transaction = TransactionRequest.CreateCallSmartContractTransactionRequest(networkConfig,
                                                                                           account,
                                                                                           SYSTEM_SMART_CONTRACT_ADDRESS,
                                                                                           ESDTAmount.Zero(),
                                                                                           PAUSE,
                                                                                           ESDTIdentifierValue.From(tokenIdentifier));

            transaction.SetGasLimit(new GasLimit(500000));

            return transaction;
        }

        /// <summary>
        /// Create transaction request - Unpause operation (allow transactions with the token again)
        /// </summary>
        /// <param name="networkConfig">MultiversX Network Configuration</param>
        /// <param name="account">Sender Account</param>
        /// <param name="tokenIdentifier">Token identifier</param>
        /// <returns></returns>
        public static TransactionRequest Unpause(
            NetworkConfig networkConfig,
            Account account,
            string tokenIdentifier)
        {
            var transaction = TransactionRequest.CreateCallSmartContractTransactionRequest(networkConfig,
                                                                                           account,
                                                                                           SYSTEM_SMART_CONTRACT_ADDRESS,
                                                                                           ESDTAmount.Zero(),
                                                                                           UNPAUSE,
                                                                                           ESDTIdentifierValue.From(tokenIdentifier));

            transaction.SetGasLimit(new GasLimit(500000));

            return transaction;
        }

        /// <summary>
        /// Create transaction request - Freeze operation (freeze the tokens held by a specific account - no tokens may be transferred to or from the frozen address)
        /// </summary>
        /// <param name="networkConfig">MultiversX Network Configuration</param>
        /// <param name="account">Sender Account</param>
        /// <param name="tokenIdentifier">Token identifier</param>
        /// <param name="receiver">Address to freeze the Token</param>
        /// <returns></returns>
        public static TransactionRequest Freeze(
            NetworkConfig networkConfig,
            Account account,
            string tokenIdentifier,
            Address receiver)
        {
            var transaction = TransactionRequest.CreateCallSmartContractTransactionRequest(networkConfig,
                                                                                           account,
                                                                                           SYSTEM_SMART_CONTRACT_ADDRESS,
                                                                                           ESDTAmount.Zero(),
                                                                                           FREEZE,
                                                                                           ESDTIdentifierValue.From(tokenIdentifier),
                                                                                           receiver);

            transaction.SetGasLimit(new GasLimit(500000));

            return transaction;
        }

        /// <summary>
        /// Create transaction request - Unfreeze operation (allow transactions with the token to and from the address)
        /// </summary>
        /// <param name="networkConfig">MultiversX Network Configuration</param>
        /// <param name="account">Sender Account</param>
        /// <param name="tokenIdentifier">Token identifier</param>
        /// <param name="receiver">Address to unfreeze the Token</param>
        /// <returns></returns>
        public static TransactionRequest Unfreeze(
            NetworkConfig networkConfig,
            Account account,
            string tokenIdentifier,
            Address receiver)
        {
            var transaction = TransactionRequest.CreateCallSmartContractTransactionRequest(networkConfig,
                                                                                           account,
                                                                                           SYSTEM_SMART_CONTRACT_ADDRESS,
                                                                                           ESDTAmount.Zero(),
                                                                                           UNFREEZE,
                                                                                           ESDTIdentifierValue.From(tokenIdentifier),
                                                                                           receiver);

            transaction.SetGasLimit(new GasLimit(500000));

            return transaction;
        }

        /// <summary>
        /// Create transaction request - Wipe operation (wipe out all the tokens held by a frozen address)
        /// Account must be frozen before the wipe operation
        /// </summary>
        /// <param name="networkConfig">MultiversX Network Configuration</param>
        /// <param name="account">Sender Account</param>
        /// <param name="tokenIdentifier">Token identifier</param>
        /// <param name="receiver">Address to wipe the Token</param>
        /// <returns></returns>
        public static TransactionRequest Wipe(
            NetworkConfig networkConfig,
            Account account,
            string tokenIdentifier,
            Address receiver)
        {
            var transaction = TransactionRequest.CreateCallSmartContractTransactionRequest(networkConfig,
                                                                                           account,
                                                                                           SYSTEM_SMART_CONTRACT_ADDRESS,
                                                                                           ESDTAmount.Zero(),
                                                                                           WIPE,
                                                                                           ESDTIdentifierValue.From(tokenIdentifier),
                                                                                           receiver);

            transaction.SetGasLimit(new GasLimit(500000));

            return transaction;
        }

        /// <summary>
        /// Create transaction request - Set special role(s) for a given address
        /// 'canAddSpecialRoles' property for token collection must be true
        /// </summary>
        /// <param name="networkConfig">MultiversX Network Configuration</param>
        /// <param name="account">Sender Account</param>
        /// <param name="receiver">Receiver address</param>
        /// <param name="tokenIdentifier">Token Identifier</param>
        /// <param name="roles">Roles to assign to receiver address</param>
        /// <returns></returns>
        public static TransactionRequest SetSpecialRole(
            NetworkConfig networkConfig,
            Account account,
            string tokenIdentifier,
            Address receiver,
            params string[] roles)
        {
            var rolesValue = roles.Select(r => (IBinaryType)BytesValue.FromUtf8(r)).ToArray();

            var arguments = new List<IBinaryType>
            {
                ESDTIdentifierValue.From(tokenIdentifier),
                receiver
            };
            arguments.AddRange(rolesValue);

            var transaction = TransactionRequest.CreateCallSmartContractTransactionRequest(
                                                                                           networkConfig,
                                                                                           account,
                                                                                           SYSTEM_SMART_CONTRACT_ADDRESS,
                                                                                           ESDTAmount.Zero(),
                                                                                           SET_SPECIAL_ROLE,
                                                                                           arguments.ToArray());

            transaction.SetGasLimit(new GasLimit(60000000));

            return transaction;
        }

        /// <summary>
        /// Create transaction request - Unset special role(s) for a given address
        /// 'canAddSpecialRoles' property for token collection must be true
        /// </summary>
        /// <param name="networkConfig">MultiversX Network Configuration</param>
        /// <param name="account">Sender Account</param>
        /// <param name="receiver">Receiver address</param>
        /// <param name="tokenIdentifier">Token identifier</param>
        /// <param name="roles">Roles to unassign for receiver address</param>
        /// <returns></returns>
        public static TransactionRequest UnsetSpecialRole(
            NetworkConfig networkConfig,
            Account account,
            string tokenIdentifier,
            Address receiver,
            params string[] roles)
        {
            var rolesValue = roles.Select(r => (IBinaryType)BytesValue.FromUtf8(r)).ToArray();

            var arguments = new List<IBinaryType>
            {
                ESDTIdentifierValue.From(tokenIdentifier),
                receiver
            };
            arguments.AddRange(rolesValue);

            var transaction = TransactionRequest.CreateCallSmartContractTransactionRequest(
                                                                                           networkConfig,
                                                                                           account,
                                                                                           SYSTEM_SMART_CONTRACT_ADDRESS,
                                                                                           ESDTAmount.Zero(),
                                                                                           UNSET_SPECIAL_ROLE,
                                                                                           arguments.ToArray());

            transaction.SetGasLimit(new GasLimit(60000000));

            return transaction;
        }

        /// <summary>
        /// Create transaction request - Transfer management rights to another Account
        /// 'canChangeOwner' property for token collection must be true
        /// </summary>
        /// <param name="networkConfig">MultiversX Network Configuration</param>
        /// <param name="account">Sender Account</param>
        /// <param name="tokenIdentifier">Token identifier</param>
        /// <param name="receiver">Address to receive management rights</param>
        /// <returns></returns>
        public static TransactionRequest TransferOwnership(
            NetworkConfig networkConfig,
            Account account,
            string tokenIdentifier,
            Address receiver)
        {
            var transaction = TransactionRequest.CreateCallSmartContractTransactionRequest(networkConfig,
                                                                                           account,
                                                                                           SYSTEM_SMART_CONTRACT_ADDRESS,
                                                                                           ESDTAmount.Zero(),
                                                                                           TRANSFER_OWNERSHIP,
                                                                                           ESDTIdentifierValue.From(tokenIdentifier),
                                                                                           receiver);

            transaction.SetGasLimit(new GasLimit(60000000));

            return transaction;
        }

        /// <summary>
        /// Create transaction request - ESDT Token properties change
        /// </summary>
        /// <param name="networkConfig">MultiversX Network Configuration</param>
        /// <param name="account">Sender Account</param>
        /// <param name="tokenIdentifier">Token identifier</param>
        /// <param name="properties">Token properties</param>
        /// <returns></returns>
        public static TransactionRequest ChangeProperties(
            NetworkConfig networkConfig,
            Account account,
            string tokenIdentifier,
            TokenProperties properties,
            params IBinaryType[] args)
        {
            var arguments = new List<IBinaryType>
            {
                ESDTIdentifierValue.From(tokenIdentifier),
                BytesValue.FromUtf8(ESDTTokenProperties.CanFreeze),
                BooleanValue.From(properties.CanFreeze),
                BytesValue.FromUtf8(ESDTTokenProperties.CanWipe),
                BooleanValue.From(properties.CanWipe),
                BytesValue.FromUtf8(ESDTTokenProperties.CanPause),
                BooleanValue.From(properties.CanPause),
                BytesValue.FromUtf8(ESDTTokenProperties.CanMint),
                BooleanValue.From(properties.CanMint),
                BytesValue.FromUtf8(ESDTTokenProperties.CanBurn),
                BooleanValue.From(properties.CanBurn),
                BytesValue.FromUtf8(ESDTTokenProperties.CanChangeOwner),
                BooleanValue.From(properties.CanChangeOwner),
                BytesValue.FromUtf8(ESDTTokenProperties.CanUpgrade),
                BooleanValue.From(properties.CanUpgrade),
                BytesValue.FromUtf8(ESDTTokenProperties.CanAddSpecialRoles),
                BooleanValue.From(properties.CanAddSpecialRoles ?? true)
            };
            arguments.AddRange(args);

            var transaction = TransactionRequest.CreateCallSmartContractTransactionRequest(networkConfig,
                                                                                           account,
                                                                                           SYSTEM_SMART_CONTRACT_ADDRESS,
                                                                                           ESDTAmount.Zero(),
                                                                                           CONTROL_CHANGES,
                                                                                           arguments.ToArray());

            transaction.SetGasLimit(new GasLimit(60000000));

            return transaction;
        }
    }
}
