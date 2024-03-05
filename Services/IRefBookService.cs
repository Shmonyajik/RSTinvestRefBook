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
        Task<BaseResponse<Position>> GetPosition(string id);
        Task<BaseResponse<IEnumerable<Position>>> GetAllPositions();
        Task<BaseResponse<bool>> EditPositionsList(List<Position> positions);
    }
}
