using CsvHelper;
using RSTinvestRefBook.Models;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RSTinvestRefBook.Repositories
{
    public class CSVPositionRepository : BaseRepository<Position>
    {
        private readonly string FilePath = ConfigurationManager.AppSettings["RefBookFilePath"];

        //public async Task<Position> GetByIdAsync(string id)
        //{
        //    using (var reader = new StreamReader(FilePath))
        //    using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        //    {
        //        while (await csv.ReadAsync())
        //        {
        //            var record = csv.GetRecord<Position>();
        //            if (record.Id == id)
        //                return record;
        //        }

        //    }
        //    throw new InvalidOperationException($"Элемент c id {id} не найден.");
        //}
        public async Task<Position> GetByHexIdAsync(string id)
        {
            using (var reader = new StreamReader(FilePath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                while (await csv.ReadAsync())
                {
                    var record = csv.GetRecord<Position>();
                    if (record.HexId == id)
                        return record;
                }

            }
            return null;
        }

        public async Task<IEnumerable<Position>> GetAllAsync()
        {
            using (var reader = new StreamReader(FilePath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var records = await csv.GetRecordsAsync<Position>().ToListAsync();
                return records;
            }

        }

        public async Task EditAsync(List<Position> positions)
        {
            using (var writer = new StreamWriter(FilePath, append: false))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteHeader<Position>();
                csv.NextRecord();
                await csv.WriteRecordsAsync(positions);
            }
        }

    }
}
