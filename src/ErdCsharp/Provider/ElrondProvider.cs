using ErdCsharp.Configuration;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ErdCsharp.Domain.Helper;
using ErdCsharp.Domain.Exceptions;
using System.Collections.Generic;
using System.Linq;
using ErdCsharp.Domain.Const;
using ErdCsharp.Provider.Dtos.Gateway;
using ErdCsharp.Provider.Dtos.Gateway.Network;
using ErdCsharp.Provider.Dtos.API.Account;
using ErdCsharp.Provider.Dtos.API.Collection;
using ErdCsharp.Provider.Dtos.API.Network;
using ErdCsharp.Provider.Dtos.API.NFT;
using ErdCsharp.Provider.Dtos.API.Exchange;
using ErdCsharp.Provider.Dtos.API.Query;
using ErdCsharp.Provider.Dtos.API.Transactions;
using ErdCsharp.Provider.Dtos.API.Common;
using ErdCsharp.Provider.Dtos.API.Token;

namespace ErdCsharp.Provider
{
    public class ElrondProvider : IElrondProvider
    {
        private readonly HttpClient _httpAPIClient;
        private readonly HttpClient _httpGatewayClient;
        public readonly ElrondNetworkConfiguration NetworkConfiguration;

        public ElrondProvider(ElrondNetworkConfiguration configuration)
        {
            NetworkConfiguration = configuration;

            _httpAPIClient = new HttpClient
            {
                BaseAddress = configuration.APIUri
            };

            _httpGatewayClient = new HttpClient
            {
                BaseAddress = configuration.GatewayUri
            };
        }

        #region accounts

        public async Task<AccountDto> GetAccount(string address)
        {
            var response = await _httpAPIClient.GetAsync($"accounts/{address}");
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new APIException(JsonSerializerWrapper.Deserialize<APIException>(content));

            var result = JsonSerializerWrapper.Deserialize<AccountDto>(content);
            return result;
        }

        public async Task<AccountTokenDto[]> GetAccountTokens(string address, int size = 100, int from = 0, Dictionary<string, string> parameters = null)
        {
            return await GetAccountTokensCustom<AccountTokenDto>(address, size, from, parameters);
        }

        public async Task<AccountToken[]> GetAccountTokensCustom<AccountToken>(string address, int size = 100, int from = 0, Dictionary<string, string> parameters = null)
        {
            size = size > 10000 ? 10000 : size;
            string args = "";
            if (parameters != null)
                args = $"&{string.Join("&", parameters.Select(e => $"{e.Key}={e.Value}"))}";
            var response = await _httpAPIClient.GetAsync($"accounts/{address}/tokens?from={from}&size={size}{args}");
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new APIException(JsonSerializerWrapper.Deserialize<APIException>(content));

            var result = JsonSerializerWrapper.Deserialize<AccountToken[]>(content);
            return result;
        }

        public async Task<string> GetAccountTokensCount(string address)
        {
            var response = await _httpAPIClient.GetAsync($"accounts/{address}/tokens/count");
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new APIException(JsonSerializerWrapper.Deserialize<APIException>(content));

            return content;
        }

        public async Task<AccountTokenDto> GetAccountToken(string address, string tokenIdentifier)
        {
            return await GetAccountTokenCustom<AccountTokenDto>(address, tokenIdentifier);
        }

        public async Task<Token> GetAccountTokenCustom<Token>(string address, string tokenIdentifier)
        {
            var response = await _httpAPIClient.GetAsync($"accounts/{address}/tokens/{tokenIdentifier}");
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new APIException(JsonSerializerWrapper.Deserialize<APIException>(content));

            var result = JsonSerializerWrapper.Deserialize<Token>(content);
            return result;
        }

        public async Task<AccountCollectionRoleDto[]> GetAccountCollectionsRole(string address, int size = 100, int from = 0, Dictionary<string, string> parameters = null)
        {
            size = size > 10000 ? 10000 : size;
            string args = "";
            if (parameters != null)
                args = $"&{string.Join("&", parameters.Select(e => $"{e.Key}={e.Value}"))}";
            var response = await _httpAPIClient.GetAsync($"accounts/{address}/roles/collections?from={from}&size={size}{args}");
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new APIException(JsonSerializerWrapper.Deserialize<APIException>(content));

            var result = JsonSerializerWrapper.Deserialize<AccountCollectionRoleDto[]>(content);
            return result;
        }

        public async Task<string> GetAccountCollectionsRoleCount(string address, Dictionary<string, string> parameters = null)
        {
            string args = "";
            if (parameters != null)
                args = $"?{string.Join("&", parameters.Select(e => $"{e.Key}={e.Value}"))}";
            var response = await _httpAPIClient.GetAsync($"accounts/{address}/roles/collections/count{args}");
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new APIException(JsonSerializerWrapper.Deserialize<APIException>(content));

            return content;
        }

        public async Task<AccountCollectionRoleDto> GetAccountCollectionRole(string address, string collectionIdentifier)
        {
            var response = await _httpAPIClient.GetAsync($"accounts/{address}/roles/collections/{collectionIdentifier}");
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new APIException(JsonSerializerWrapper.Deserialize<APIException>(content));

            var result = JsonSerializerWrapper.Deserialize<AccountCollectionRoleDto>(content);
            return result;
        }

        public async Task<AccountTokenRoleDto[]> GetAccountTokensRole(string address, int size = 100, int from = 0, Dictionary<string, string> parameters = null)
        {
            size = size > 10000 ? 10000 : size;
            string args = "";
            if (parameters != null)
                args = $"&{string.Join("&", parameters.Select(e => $"{e.Key}={e.Value}"))}";
            var response = await _httpAPIClient.GetAsync($"accounts/{address}/roles/tokens?from={from}&size={size}{args}");
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new APIException(JsonSerializerWrapper.Deserialize<APIException>(content));

            var result = JsonSerializerWrapper.Deserialize<AccountTokenRoleDto[]>(content);
            return result;
        }

        public async Task<string> GetAccountTokensRoleCount(string address, Dictionary<string, string> parameters = null)
        {
            string args = "";
            if (parameters != null)
                args = $"?{string.Join("&", parameters.Select(e => $"{e.Key}={e.Value}"))}";
            var response = await _httpAPIClient.GetAsync($"accounts/{address}/roles/tokens/count{args}");
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new APIException(JsonSerializerWrapper.Deserialize<APIException>(content));

            return content;
        }

        public async Task<AccountTokenRoleDto> GetAccountTokenRole(string address, string tokenIdentifier)
        {
            var response = await _httpAPIClient.GetAsync($"accounts/{address}/roles/tokens/{tokenIdentifier}");
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new APIException(JsonSerializerWrapper.Deserialize<APIException>(content));

            var result = JsonSerializerWrapper.Deserialize<AccountTokenRoleDto>(content);
            return result;
        }

        public async Task<NFTDto[]> GetAccountNFTs(string address, int size = 100, int from = 0, Dictionary<string, string> parameters = null)
        {
            return await GetAccountNFTsCustom<NFTDto>(address, size, from, parameters);
        }

        public async Task<NFT[]> GetAccountNFTsCustom<NFT>(string address, int size = 100, int from = 0, Dictionary<string, string> parameters = null)
        {
            size = size > 10000 ? 10000 : size;
            string args = "";
            if (parameters != null)
                args = $"&{string.Join("&", parameters.Select(e => $"{e.Key}={e.Value}"))}";
            var response = await _httpAPIClient.GetAsync($"accounts/{address}/nfts?type={ESDTTokenType.NonFungibleESDT},{ESDTTokenType.SemiFungibleESDT}&from={from}&size={size}{args}");
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new APIException(JsonSerializerWrapper.Deserialize<APIException>(content));

            var result = JsonSerializerWrapper.Deserialize<NFT[]>(content);
            return result;
        }

        public async Task<string> GetAccountNFTsCount(string address, Dictionary<string, string> parameters = null)
        {
            string args = "";
            if (parameters != null)
                args = $"&{string.Join("&", parameters.Select(e => $"{e.Key}={e.Value}"))}";
            var response = await _httpAPIClient.GetAsync($"accounts/{address}/nfts/count?type={ESDTTokenType.NonFungibleESDT},{ESDTTokenType.SemiFungibleESDT}{args}");
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new APIException(JsonSerializerWrapper.Deserialize<APIException>(content));

            return content;
        }

        public async Task<NFTDto> GetAccountNFT(string address, string nft)
        {
            return await GetAccountNFTCustom<NFTDto>(address, nft);
        }

        public async Task<NFT> GetAccountNFTCustom<NFT>(string address, string nft)
        {
            var response = await _httpAPIClient.GetAsync($"accounts/{address}/nfts/{nft}");
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new APIException(JsonSerializerWrapper.Deserialize<APIException>(content));

            var result = JsonSerializerWrapper.Deserialize<NFT>(content);
            return result;
        }

        public async Task<MetaESDTDto[]> GetAccountMetaESDTs(string address, int size = 100, int from = 0, Dictionary<string, string> parameters = null)
        {
            return await GetAccountMetaESDTsCustom<MetaESDTDto>(address, size, from, parameters);
        }

        public async Task<MetaESDT[]> GetAccountMetaESDTsCustom<MetaESDT>(string address, int size = 100, int from = 0, Dictionary<string, string> parameters = null)
        {
            size = size > 10000 ? 10000 : size;
            string args = "";
            if (parameters != null)
                args = $"&{string.Join("&", parameters.Select(e => $"{e.Key}={e.Value}"))}";
            var response = await _httpAPIClient.GetAsync($"accounts/{address}/nfts?type={ESDTTokenType.MetaESDT}&from={from}&size={size}{args}");
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new APIException(JsonSerializerWrapper.Deserialize<APIException>(content));

            var result = JsonSerializerWrapper.Deserialize<MetaESDT[]>(content);
            return result;
        }

        public async Task<string> GetAccountMetaESDTsCount(string address, Dictionary<string, string> parameters = null)
        {
            string args = "";
            if (parameters != null)
                args = $"&{string.Join("&", parameters.Select(e => $"{e.Key}={e.Value}"))}";
            var response = await _httpAPIClient.GetAsync($"accounts/{address}/nfts/count?type={ESDTTokenType.MetaESDT}{args}");
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new APIException(JsonSerializerWrapper.Deserialize<APIException>(content));

            return content;
        }

        public async Task<MetaESDTDto> GetAccountMetaESDT(string address, string nft)
        {
            return await GetAccountMetaESDTCustom<MetaESDTDto>(address, nft);
        }

        public async Task<MetaESDT> GetAccountMetaESDTCustom<MetaESDT>(string address, string nft)
        {
            var response = await _httpAPIClient.GetAsync($"accounts/{address}/nfts/{nft}");
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new APIException(JsonSerializerWrapper.Deserialize<APIException>(content));

            var result = JsonSerializerWrapper.Deserialize<MetaESDT>(content);
            return result;
        }

        public async Task<AccountSCStakeDto[]> GetAccountStake(string address)
        {
            var response = await _httpAPIClient.GetAsync($"accounts/{address}/stake");
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new APIException(JsonSerializerWrapper.Deserialize<APIException>(content));

            var result = JsonSerializerWrapper.Deserialize<AccountSCStakeDto[]>(content);
            return result;
        }
        
        public async Task<AccountContractDto[]> GetAccountContracts(string address)
        {
            var response = await _httpAPIClient.GetAsync($"accounts/{address}/contracts");
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new APIException(JsonSerializerWrapper.Deserialize<APIException>(content));

            var result = JsonSerializerWrapper.Deserialize<AccountContractDto[]>(content);
            return result;
        }

        public async Task<string> GetAccountContractsCount(string address)
        {
            var response = await _httpAPIClient.GetAsync($"accounts/{address}/contracts/count");
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new APIException(JsonSerializerWrapper.Deserialize<APIException>(content));

            return content;
        }

        public async Task<AccountHistoryDto[]> GetAccountHistory(string address, int size = 100, int from = 0)
        {
            size = size > 10000 ? 10000 : size;
            var response = await _httpAPIClient.GetAsync($"accounts/{address}/history?from={from}&size={size}");
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new APIException(JsonSerializerWrapper.Deserialize<APIException>(content));

            var result = JsonSerializerWrapper.Deserialize<AccountHistoryDto[]>(content);
            return result;
        }

        public async Task<AccountHistoryTokenDto[]> GetAccountHistoryToken(string address, string tokenIdentifier, int size = 100, int from = 0)
        {
            size = size > 10000 ? 10000 : size;
            var response = await _httpAPIClient.GetAsync($"accounts/{address}/history/{tokenIdentifier}?from={from}&size={size}");
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new APIException(JsonSerializerWrapper.Deserialize<APIException>(content));

            var result = JsonSerializerWrapper.Deserialize<AccountHistoryTokenDto[]>(content);
            return result;
        }

        #endregion

        #region collections

        public async Task<CollectionDto[]> GetCollections(int size = 100, int from = 0, Dictionary<string, string> parameters = null)
        {
            return await GetCollectionsCustom<CollectionDto>(size, from, parameters);
        }

        public async Task<Collection[]> GetCollectionsCustom<Collection>(int size = 100, int from = 0, Dictionary<string, string> parameters = null)
        {
            size = size > 10000 ? 10000 : size;
            string args = "";
            if (parameters != null)
                args = $"&{string.Join("&", parameters.Select(e => $"{e.Key}={e.Value}"))}";
            var response = await _httpAPIClient.GetAsync($"collections?from={from}&size={size}{args}");
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new APIException(JsonSerializerWrapper.Deserialize<APIException>(content));

            var result = JsonSerializerWrapper.Deserialize<Collection[]>(content);
            return result;
        }

        public async Task<string> GetCollectionsCount(Dictionary<string, string> parameters = null)
        {
            string args = "";
            if (parameters != null)
                args = $"?{string.Join("&", parameters.Select(e => $"{e.Key}={e.Value}"))}";
            var response = await _httpAPIClient.GetAsync($"collections/count{args}");
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new APIException(JsonSerializerWrapper.Deserialize<APIException>(content));

            return content;
        }

        public async Task<CollectionDto> GetCollection(string collectionIdentifier)
        {
            return await GetCollectionCustom<CollectionDto>(collectionIdentifier);
        }

        public async Task<Collection> GetCollectionCustom<Collection>(string collectionIdentifier)
        {
            var response = await _httpAPIClient.GetAsync($"collections/{collectionIdentifier}");
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new APIException(JsonSerializerWrapper.Deserialize<APIException>(content));

            var result = JsonSerializerWrapper.Deserialize<Collection>(content);
            return result;
        }

        #endregion

        #region network

        public async Task<GatewayNetworkConfigDataDto> GetGatewayNetworkConfig()
        {
            var response = await _httpGatewayClient.GetAsync("network/config");
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new APIException(JsonSerializerWrapper.Deserialize<APIException>(content));

            var result = JsonSerializerWrapper.Deserialize<GatewayResponseDto<GatewayNetworkConfigDataDto>>(content);
            result.EnsureSuccessStatusCode();
            return result.Data;
        }

        public async Task<GatewayNetworkEconomicsDataDto> GetGatewayNetworkEconomics()
        {
            var response = await _httpGatewayClient.GetAsync("network/economics");
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new APIException(JsonSerializerWrapper.Deserialize<APIException>(content));

            var result = JsonSerializerWrapper.Deserialize<GatewayResponseDto<GatewayNetworkEconomicsDataDto>>(content);
            result.EnsureSuccessStatusCode();
            return result.Data;
        }

        public async Task<NetworkEconomicsDto> GetNetworkEconomics()
        {
            var response = await _httpAPIClient.GetAsync("economics");
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new APIException(JsonSerializerWrapper.Deserialize<APIException>(content));

            var result = JsonSerializerWrapper.Deserialize<NetworkEconomicsDto>(content);
            return result;
        }

        public async Task<NetworkStatsDto> GetNetworkStats()
        {
            var response = await _httpAPIClient.GetAsync("stats");
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new APIException(JsonSerializerWrapper.Deserialize<APIException>(content));

            var result = JsonSerializerWrapper.Deserialize<NetworkStatsDto>(content);
            return result;
        }

        #endregion

        #region nfts

        public async Task<NFTDto[]> GetNFTs(int size = 100, int from = 0, Dictionary<string, string> parameters = null)
        {
            return await GetNFTsCustom<NFTDto>(size, from, parameters);
        }

        public async Task<NFT[]> GetNFTsCustom<NFT>(int size = 100, int from = 0, Dictionary<string, string> parameters = null)
        {
            size = size > 10000 ? 10000 : size;
            string args = "";
            if (parameters != null)
                args = $"&{string.Join("&", parameters.Select(e => $"{e.Key}={e.Value}"))}";
            var response = await _httpAPIClient.GetAsync($"nfts?type={ESDTTokenType.NonFungibleESDT},{ESDTTokenType.SemiFungibleESDT}&from={from}&size={size}{args}");
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new APIException(JsonSerializerWrapper.Deserialize<APIException>(content));

            var result = JsonSerializerWrapper.Deserialize<NFT[]>(content);
            return result;
        }

        public async Task<string> GetNFTsCount(Dictionary<string, string> parameters = null)
        {
            string args = "";
            if (parameters != null)
                args = $"&{string.Join("&", parameters.Select(e => $"{e.Key}={e.Value}"))}";
            var response = await _httpAPIClient.GetAsync($"nfts/count?type={ESDTTokenType.NonFungibleESDT},{ESDTTokenType.SemiFungibleESDT}{args}");
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new APIException(JsonSerializerWrapper.Deserialize<APIException>(content));

            return content;
        }

        public async Task<MetaESDTDto[]> GetMetaESDTs(int size = 100, int from = 0, Dictionary<string, string> parameters = null)
        {
            return await GetMetaESDTsCustom<MetaESDTDto>(size, from, parameters);
        }

        public async Task<MetaESDT[]> GetMetaESDTsCustom<MetaESDT>(int size = 100, int from = 0, Dictionary<string, string> parameters = null)
        {
            size = size > 10000 ? 10000 : size;
            string args = "";
            if (parameters != null)
                args = $"&{string.Join("&", parameters.Select(e => $"{e.Key}={e.Value}"))}";
            var response = await _httpAPIClient.GetAsync($"nfts?type={ESDTTokenType.MetaESDT}&from={from}&size={size}{args}");
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new APIException(JsonSerializerWrapper.Deserialize<APIException>(content));

            var result = JsonSerializerWrapper.Deserialize<MetaESDT[]>(content);
            return result;
        }

        public async Task<string> GetMetaESDTsCount(Dictionary<string, string> parameters = null)
        {
            string args = "";
            if (parameters != null)
                args = $"&{string.Join("&", parameters.Select(e => $"{e.Key}={e.Value}"))}";
            var response = await _httpAPIClient.GetAsync($"nfts/count?type={ESDTTokenType.MetaESDT}{args}");
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new APIException(JsonSerializerWrapper.Deserialize<APIException>(content));

            return content;
        }

        public async Task<NFTDto> GetNFT(string nftIdentifier)
        {
            return await GetNFTCustom<NFTDto>(nftIdentifier);
        }

        public async Task<NFT> GetNFTCustom<NFT>(string nftIdentifier)
        {
            var response = await _httpAPIClient.GetAsync($"nfts/{nftIdentifier}");
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new APIException(JsonSerializerWrapper.Deserialize<APIException>(content));

            var result = JsonSerializerWrapper.Deserialize<NFT>(content);
            return result;
        }

        public async Task<MetaESDTDto> GetMetaESDT(string nftIdentifier)
        {
            return await GetMetaESDTCustom<MetaESDTDto>(nftIdentifier);
        }

        public async Task<MetaESDT> GetMetaESDTCustom<MetaESDT>(string nftIdentifier)
        {
            var response = await _httpAPIClient.GetAsync($"nfts/{nftIdentifier}");
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new APIException(JsonSerializerWrapper.Deserialize<APIException>(content));

            var result = JsonSerializerWrapper.Deserialize<MetaESDT>(content);
            return result;
        }

        public async Task<AddressBalanceDto[]> GetNFTAccounts(string identifier, int size = 100, int from = 0)
        {
            size = size > 10000 ? 10000 : size;
            var response = await _httpAPIClient.GetAsync($"nfts/{identifier}/accounts?from={from}&size={size}");
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new APIException(JsonSerializerWrapper.Deserialize<APIException>(content));

            var result = JsonSerializerWrapper.Deserialize<AddressBalanceDto[]>(content);
            return result;
        }

        public async Task<string> GetNFTAccountsCounter(string identifier)
        {
            var response = await _httpAPIClient.GetAsync($"nfts/{identifier}/accounts/count");
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new APIException(JsonSerializerWrapper.Deserialize<APIException>(content));

            return content;
        }

        public async Task<AddressBalanceDto[]> GetMetaESDTAccounts(string identifier, int size = 100, int from = 0)
        {
            size = size > 10000 ? 10000 : size;
            var response = await _httpAPIClient.GetAsync($"nfts/{identifier}/accounts?from={from}&size={size}");
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new APIException(JsonSerializerWrapper.Deserialize<APIException>(content));

            var result = JsonSerializerWrapper.Deserialize<AddressBalanceDto[]>(content);
            return result;
        }

        public async Task<string> GetMetaESDTAccountsCounter(string identifier)
        {
            var response = await _httpAPIClient.GetAsync($"nfts/{identifier}/accounts/count");
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new APIException(JsonSerializerWrapper.Deserialize<APIException>(content));

            return content;
        }

        #endregion

        #region tokens

        public async Task<TokenDto[]> GetTokens(int size = 100, int from = 0, Dictionary<string, string> parameters = null)
        {
            return await GetTokensCustom<TokenDto>(size, from, parameters);
        }

        public async Task<Token[]> GetTokensCustom<Token>(int size = 100, int from = 0, Dictionary<string, string> parameters = null)
        {
            size = size > 10000 ? 10000 : size;
            string args = "";
            if (parameters != null)
                args = $"&{string.Join("&", parameters.Select(e => $"{e.Key}={e.Value}"))}";
            var response = await _httpAPIClient.GetAsync($"tokens?from={from}&size={size}{args}");
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new APIException(JsonSerializerWrapper.Deserialize<APIException>(content));

            var result = JsonSerializerWrapper.Deserialize<Token[]>(content);
            return result;
        }

        public async Task<string> GetTokensCount(Dictionary<string, string> parameters = null)
        {
            string args = "";
            if (parameters != null)
                args = $"?{string.Join("&", parameters.Select(e => $"{e.Key}={e.Value}"))}";
            var response = await _httpAPIClient.GetAsync($"tokens/count{args}");
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new APIException(JsonSerializerWrapper.Deserialize<APIException>(content));

            return content;
        }

        public async Task<TokenDto> GetToken(string tokenIdentifier)
        {
            return await GetTokenCustom<TokenDto>(tokenIdentifier);
        }

        public async Task<Token> GetTokenCustom<Token>(string tokenIdentifier)
        {
            var response = await _httpAPIClient.GetAsync($"tokens/{tokenIdentifier}");
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new APIException(JsonSerializerWrapper.Deserialize<APIException>(content));

            var result = JsonSerializerWrapper.Deserialize<Token>(content);
            return result;
        }

        public async Task<AddressBalanceDto[]> GetTokenAccounts(string tokenIdentifier, int size = 100, int from = 0)
        {
            size = size > 10000 ? 10000 : size;
            var response = await _httpAPIClient.GetAsync($"tokens/{tokenIdentifier}/accounts?from={from}&size={size}");
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new APIException(JsonSerializerWrapper.Deserialize<APIException>(content));

            var result = JsonSerializerWrapper.Deserialize<AddressBalanceDto[]>(content);
            return result;
        }

        public async Task<string> GetTokenAccountsCounter(string tokenIdentifier)
        {
            var response = await _httpAPIClient.GetAsync($"tokens/{tokenIdentifier}/accounts/count");
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new APIException(JsonSerializerWrapper.Deserialize<APIException>(content));

            return content;
        }

        #endregion

        #region transactions

        public async Task<TransactionDto[]> GetTransactions(int size = 100, int from = 0, Dictionary<string, string> parameters = null)
        {
            return await GetTransactionsCustom<TransactionDto>(size, from, parameters);
        }

        public async Task<Transaction[]> GetTransactionsCustom<Transaction>(int size = 100, int from = 0, Dictionary<string, string> parameters = null)
        {
            size = size > 10000 ? 10000 : size;
            string args = "";
            if (parameters != null)
                args = $"&{string.Join("&", parameters.Select(e => $"{e.Key}={e.Value}"))}";
            var response = await _httpAPIClient.GetAsync($"transactions?from={from}&size={size}{args}");
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new APIException(JsonSerializerWrapper.Deserialize<APIException>(content));

            var result = JsonSerializerWrapper.Deserialize<Transaction[]>(content);
            return result;
        }

        public async Task<TransactionResponseDto> SendTransaction(TransactionRequestDto transactionRequestDto)
        {
            var raw = JsonSerializerWrapper.Serialize(transactionRequestDto);
            var payload = new StringContent(raw, Encoding.UTF8, "application/json");
            var response = await _httpAPIClient.PostAsync("transactions", payload);
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new APIException(JsonSerializerWrapper.Deserialize<APIException>(content));

            var result = JsonSerializerWrapper.Deserialize<TransactionResponseDto>(content);
            return result;
        }

        public async Task<MultipleTransactionsResponseDto> SendTransactions(TransactionRequestDto[] transactionsRequestDto)
        {
            var raw = JsonSerializerWrapper.Serialize(transactionsRequestDto);
            var payload = new StringContent(raw, Encoding.UTF8, "application/json");
            var response = await _httpAPIClient.PostAsync("transaction/send-multiple", payload);
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new APIException(JsonSerializerWrapper.Deserialize<APIException>(content));

            var result = JsonSerializerWrapper.Deserialize<GatewayResponseDto<MultipleTransactionsResponseDto>>(content);
            result.EnsureSuccessStatusCode();
            return result.Data;
        }

        public async Task<string> GetTransactionsCount(Dictionary<string, string> parameters = null)
        {
            string args = "";
            if (parameters != null)
                args = $"?{string.Join("&", parameters.Select(e => $"{e.Key}={e.Value}"))}";
            var response = await _httpAPIClient.GetAsync($"transactions/count{args}");
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new APIException(JsonSerializerWrapper.Deserialize<APIException>(content));

            return content;
        }

        public async Task<TransactionDto> GetTransaction(string txHash)
        {
            return await GetTransactionCustom<TransactionDto>(txHash);
        }

        public async Task<Transaction> GetTransactionCustom<Transaction>(string txHash)
        {
            var response = await _httpAPIClient.GetAsync($"transactions/{txHash}");
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new APIException(JsonSerializerWrapper.Deserialize<APIException>(content));

            var result = JsonSerializerWrapper.Deserialize<Transaction>(content);
            return result;
        }

        #endregion

        #region usernames

        public async Task<AccountDto> GetAccountByUsername(string username)
        {
            var response = await _httpAPIClient.GetAsync($"usernames/{username}");
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new APIException(JsonSerializerWrapper.Deserialize<APIException>(content));

            var result = JsonSerializerWrapper.Deserialize<AccountDto>(content);
            return result;
        }

        #endregion

        #region query

        //public async Task<QueryVmResultDataDto> QueryVm(QueryVmRequestDto queryVmRequestDto)
        //{
        //    var raw = JsonSerializerWrapper.Serialize(queryVmRequestDto);
        //    var payload = new StringContent(raw, Encoding.UTF8, "application/json");
        //    var response = await _httpGatewayClient.PostAsync("vm-values/query", payload);
        //    var content = await response.Content.ReadAsStringAsync();
        //    if (!response.IsSuccessStatusCode)
        //        throw new APIException(JsonSerializerWrapper.Deserialize<APIException>(content));

        //    var result = JsonSerializerWrapper.Deserialize<GatewayResponseDto<QueryVmResultDataDto>>(content);
        //    return result;
        //}

        public async Task<QueryResponseDto> Query(QueryRequestDto queryVmRequestDto)
        {
            var raw = JsonSerializerWrapper.Serialize(queryVmRequestDto);
            var payload = new StringContent(raw, Encoding.UTF8, "application/json");
            var response = await _httpAPIClient.PostAsync("query", payload);
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new APIException(JsonSerializerWrapper.Deserialize<APIException>(content));

            var result = JsonSerializerWrapper.Deserialize<QueryResponseDto>(content);
            return result;
        }

        #endregion

        #region xExchange

        public async Task<MexEconomicsDto> GetMexEconomics()
        {
            var response = await _httpAPIClient.GetAsync("mex/economics");
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new APIException(JsonSerializerWrapper.Deserialize<APIException>(content));

            var result = JsonSerializerWrapper.Deserialize<MexEconomicsDto>(content);
            return result;
        }

        #endregion
    }
}
