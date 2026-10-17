using Project_MARS.Application.Interfaces;
using Project_MARS.Domain.Entities;
using Project_MARS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_MARS.Infrastructure.Repositories
{
    public class RefreshTokenRepository(AppDbContext dbContext) : IRefreshTokenRepository
    {
        public async Task<RefreshToken?> GetByTokenAsync(string token)
        {
            return await dbContext.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == token);
        }
        public async Task AddAsync(RefreshToken refreshToken)
        {
            await dbContext.RefreshTokens.AddAsync(refreshToken);
            await dbContext.SaveChangesAsync();
        }

        public async Task RemoveAsync(RefreshToken refreshToken)
        {
            dbContext.RefreshTokens.Remove(refreshToken);
            await dbContext.SaveChangesAsync();
        }
    }
}
