using Microsoft.AspNetCore.Http;
using Project_MARS.Application.Common;
using Project_MARS.Domain.Entities;
using Project_MARS.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_MARS.Application.Services
{
    public interface IAutoServiceService
    {
        Task<Result<PagedResult<AutoService>>> GetPagedAsync(int page, int pageSize, List<int> categoryIds, AutoServiceSortColumn sort, bool ascending, string? name);

        Task<Result<AutoService?>> GetByIdAasync(int id);

        Task<Result<AutoService>> AddAsync(string name, string adress, IFormFile? LogoFile,string userId);

        Task<Result> UpdateAsync(int id, string name, string adress, IFormFile? LogoFile);

        Task<Result> DeleteAsync(int id);
    }
}
