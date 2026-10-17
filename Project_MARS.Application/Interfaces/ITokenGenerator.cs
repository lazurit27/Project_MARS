using Project_MARS.Application.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_MARS.Application.Interfaces
{
    public interface ITokenGenerator
    {
        string GenerateAccessToken(string UserId,string UserName,string userRole);
        RefreshTokenResult GenerateRefreshToken();
    }
}
