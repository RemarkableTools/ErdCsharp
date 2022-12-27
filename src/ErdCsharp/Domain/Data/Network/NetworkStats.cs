using ErdCsharp.Provider;
using ErdCsharp.Provider.Dtos.API.Network;
using System.Threading.Tasks;

namespace ErdCsharp.Domain.Data.Network
{
    public class NetworkStats
    {
        public int Shards { get; set; }
        public long Blocks { get; set; }
        public long Accounts { get; set; }
        public long Transactions { get; set; }
        public long RefreshRate { get; set; }
        public long Epoch { get; set; }
        public long RoundsPassed { get; set; }
        public long RoundsPerEpoch { get; set; }

        private NetworkStats() { }

        private NetworkStats(NetworkStatsDto stats)
        {
            Shards = stats.Shards;
            Blocks = stats.Blocks;
            Accounts = stats.Accounts;
            Transactions = stats.Transactions;
            RefreshRate = stats.RefreshRate;
            Epoch = stats.Epoch;
            RoundsPassed = stats.RoundsPassed;
            RoundsPerEpoch = stats.RoundsPerEpoch;
        }

        /// <summary>
        /// Gets the Network Stats
        /// </summary>
        /// <param name="provider">Elrond provider</param>
        /// <returns>NetworkEconomics</returns>
        public static async Task<NetworkStats> GetFromNetwork(IElrondProvider provider)
        {
            return new NetworkStats(await provider.GetNetworkStats());
        }

        /// <summary>
        /// New empty Stats
        /// </summary>
        /// <returns>NetworkConfig</returns>
        public static NetworkStats New()
        {
            return new NetworkStats();
        }
    }
}
