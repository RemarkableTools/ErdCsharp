using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErdCsharp.Configuration
{
    public class MultiversxNetworkConfiguration
    {
        public Network Network { get; }
        public Uri APIUri { get; }
        public Uri GatewayUri { get; }
        public Uri ExplorerUri { get; }
        public Uri WebWalletUri { get; set; }

        public MultiversxNetworkConfiguration(Network network)
        {
            Network = network;
            switch (network)
            {
                case Network.MainNet:
                    APIUri = new Uri("https://api.elrond.com/");
                    GatewayUri = new Uri("https://gateway.elrond.com/");
                    ExplorerUri = new Uri("https://explorer.elrond.com/");
                    WebWalletUri = new Uri("https://wallet.elrond.com/");
                    break;
                case Network.DevNet:
                    APIUri = new Uri("https://devnet-api.elrond.com/");
                    GatewayUri = new Uri("https://devnet-gateway.elrond.com/");
                    ExplorerUri = new Uri("https://devnet-explorer.elrond.com/");
                    WebWalletUri = new Uri("https://devnet-wallet.elrond.com/");
                    break;
                case Network.TestNet:
                    APIUri = new Uri("https://testnet-api.elrond.com/");
                    GatewayUri = new Uri("https://testnet-gateway.elrond.com/");
                    ExplorerUri = new Uri("https://testnet-explorer.elrond.com/");
                    WebWalletUri = new Uri("https://testnet-wallet.elrond.com/");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(network), network, null);
            }
        }
    }
}
