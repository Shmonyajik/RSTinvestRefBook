using System.Collections.Generic;
using System.Threading.Tasks;

namespace RSTinvestRefBook.Repositories
{
    public interface BaseRepository<T>
    {

        //Task<T> GetByIdAsync(string id);
        Task<T> GetByHexIdAsync(string id);
        Task<IEnumerable<T>> GetAllAsync();

        Task EditAsync(List<T> entitys);



    }
}
