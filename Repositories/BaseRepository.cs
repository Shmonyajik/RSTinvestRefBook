using RSTinvestRefBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSTinvestRefBook.Repositories
{
    public interface BaseRepository<T>
    {
        Task<T> GetByIdAsync(string id);
        Task<IEnumerable<T>> GetAllAsync();

        Task EditAsync(List<T> entitys);
        Task CreateMultipleAsync(List<T> entitys);
        void Create(T entity);

        Task EditByIdAsync(string id, T newEntity);
        Task DeleteByIdAsync(string id);
        Task DeleteMultipleAsync(IEnumerable<string> ids);
    }
}
