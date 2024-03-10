using RSTinvestRefBook.Models;
using RSTinvestRefBook.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSTinvestRefBook.Services
{
    public interface IRefBookService
    {
        Task<BaseResponse<Position>> GetPositionByIdAsync(string id);
        Task<BaseResponse<IEnumerable<Position>>> GetAllPositionsAsync();
        Task<BaseResponse<bool>> EditPositionsListAsync(List<Position> positions);
        Task<BaseResponse<IEnumerable<Position>>> GetPositionsByHexIdsAsync(IEnumerable<string> hexIds);
    }
}
