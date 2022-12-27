using ErdCsharp.Provider.Dtos.API.Common;

namespace ErdCsharp.Provider.Dtos.API.Account
{
    public class AccountContractDto
    {
        public string Address { get; set; }
        public string DeployTxHash { get; set; }
        public long Timestamp { get; set; }
        public AssetsDto Assets { get; set; }
    }
}
