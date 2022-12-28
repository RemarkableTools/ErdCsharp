# Advanced usage

## Token Transfer

#### In the following example we will create a [`Token Transaction Request`](https://github.com/RemarkableTools/ErdCsharp/blob/master/src/ErdCsharp/TransactionsManager/TokenTransactionRequest.cs), sign it and send it to the network
Get the [`ElrondProvider`](https://github.com/RemarkableTools/ErdCsharp/blob/master/src/ErdCsharp/Configuration/ElrondNetworkConfiguration.cs) instance
```csharp
var provider = new ElrondProvider(new ElrondNetworkConfiguration(Network.DevNet));
```
Get a valid [`NetworkConfig`](https://github.com/RemarkableTools/ErdCsharp/blob/master/src/ErdCsharp/Domain/Data/Network/NetworkConfig.cs) instance
```csharp
var networkConfig = await NetworkConfig.GetFromNetwork(provider);
```
Create a [`Wallet`](https://github.com/RemarkableTools/ErdCsharp/blob/master/src/ErdCsharp/Domain/Wallet.cs) instance by providing the key file and the associated password
```csharp
var filePath = "PATH/TO/KEYFILE.json";
var password = "PASSWORD";
var wallet = Wallet.DeriveFromKeyFile(KeyFile.FromFilePath(filePath), password);
```
Set up Sender Account and Receiver Address
```csharp
var bech32Address = "BECH32_ADDRESS";
var account = Account.From(await provider.GetAccount(bech32Address));
var receiverAddress = Address.FromBech32("RECEIVER_ADDRESS");
```
Get a token from network
```csharp
var token = Token.From(await provider.GetToken("OFE-29eb54"));
```
Create the [`Transaction Request`](https://github.com/RemarkableTools/ErdCsharp/blob/master/src/ErdCsharp/Domain/TransactionRequest.cs)
```csharp
var transactionRequest = TokenTransactionRequest.TokenTransfer(
    networkConfig,
    account,
    receiverAddress,
    token.Identifier.Value,
    ESDTAmount.ESDT("100", token.GetESDT()).Value);
```
Use the [`Private Signer`](https://github.com/RemarkableTools/ErdCsharp/blob/master/src/ErdCsharp-PrivateSigner/PrivateSigner.cs) to sign the transaction
```csharp
var signedTransaction = transactionRequest.Sign(wallet);
```
POST the transaction to Elrond API
```csharp
var response = await provider.SendTransaction(signedTransaction);
```
Get the [`Transaction`](https://github.com/RemarkableTools/ErdCsharp/blob/master/src/ErdCsharp/Domain/Data/Transaction/Transaction.cs) from response and await for execution to finalize
```csharp
var transaction = Transaction.From(response.TxHash);
await transaction.AwaitExecuted(provider);
Console.WriteLine($"Transaction executed with status {transaction.Status}");
```

---

## Smart Contract interaction
The example is created for this [adder](https://github.com/ElrondNetwork/elrond-wasm-rs/tree/master/contracts/examples/adder) contract.
#### Create a [`EGLD Transaction Request`](https://github.com/RemarkableTools/ErdCsharp/blob/master/src/ErdCsharp/TransactionsManager/EGLDTransactionRequest.cs) to a Smart Contract, sign it and send it to the network
```csharp
var transactionRequest = EGLDTransactionRequest.EGLDTransferToSmartContract(
    networkConfig,
    account,
    smartContractAddress,
    ESDTAmount.Zero(),
    "add",
    NumericValue.BigUintValue(10));
var signedTransaction = transactionRequest.Sign(wallet);
var response = await provider.SendTransaction(signedTransaction);
var transaction = Transaction.From(response.TxHash);
await transaction.AwaitExecuted(provider);
Console.WriteLine($"Transaction executed with status {transaction.Status}");
```
#### Query smart contract
```csharp
var outputType = TypeValue.BigUintTypeValue;
var queryResult = await SmartContract.QuerySmartContract<NumericValue>(provider,
                                                                       smartContractAddress,
                                                                       outputType,
                                                                       "getSum");
Console.WriteLine(queryResult.Number);
```