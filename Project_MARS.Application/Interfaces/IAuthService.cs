using Project_MARS.Application.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_MARS.Application.Interfaces
{
    public interface IAuthService
    {
        Task<Result> RegisterAsync(string username, string email, string password);
        Task<Result> LogoutAsync(string refreshAsync);
        Task<Result<AuthResult>> LoginAsync(string password, string email, bool rememberMe);
        Task<Result<AuthResult>> RefreshAsync(string refreshAsync);
    }
}
