using ErdCsharp.Domain.Data.Properties;
using ErdCsharp.Provider.Dtos.API.Common;

namespace ErdCsharp.Domain.Data.Common
{
    /// <summary>
    /// /accounts/{address}/roles/tokens | /{token} endpoints
    /// </summary>
    public class TokenAccountRole
    {
        public TokenManagerProperties ManagerProperties { get; private set; }
        public string[] Roles { get; private set; }

        private TokenAccountRole() { }

        public static TokenAccountRole From(TokenAccountRoleDto role)
        {
            if (role == null) return null;

            return new TokenAccountRole()
            {
                ManagerProperties = TokenManagerProperties.From(role.CanLocalMint,
                                                                role.CanLocalBurn),
            };
        }
    }
}
