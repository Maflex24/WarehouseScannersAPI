using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace WarehouseScannersAPI
{
    public class AuthenticationSettings
    {
        public string JwtKey { get; set; }
        public int JwtExpireHours { get; set; }
        public string JwtIssuer { get; set; }
        private static AuthenticationSettings _authenticationSettings { get; set; }

        private AuthenticationSettings()
        {
            
        }

        public static AuthenticationSettings NewSettings()
        {
            if (_authenticationSettings == null)
            {
                _authenticationSettings = new AuthenticationSettings();
            }

            return _authenticationSettings;
        }
    }
}
