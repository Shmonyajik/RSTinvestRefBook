using CsvHelper;
using RSTinvestRefBook.Models;
using System;
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
                //var rec = csv.GetRecords<Position>();
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
/// <summary>
/// ////////////////////////////////////////////
/// </summary>
/// <param name="positions"></param>
/// <returns></returns>
        public async Task CreateMultipleAsync(List<Position> positions)
        {
            using (var writer = new StreamWriter(FilePath, append: true))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                await csv.WriteRecordsAsync(positions);
            }
        }

        public void Create(Position position)
        {
            using (var writer = new StreamWriter(FilePath, append: true))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecord(position);
            }
            
        }
        public async Task EditByIdAsync(string id, Position newPosition)
        {
          
            var records = new List<Position>();

            using (var reader = new StreamReader(FilePath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                records = (await csv.GetRecordsAsync<Position>().ToListAsync());
            }

            var positionToEdit = records.FirstOrDefault(p => p.Id == id);

            if (positionToEdit != null)
            {
                positionToEdit.Name = newPosition.Name;

                using (var writer = new StreamWriter(FilePath))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    await csv.WriteRecordsAsync(records);
                }
            }
        }
        public async Task DeleteByIdAsync(string id)
        {
            
            var records = new List<Position>();

            using (var reader = new StreamReader(FilePath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                records = (await csv.GetRecordsAsync<Position>().ToListAsync());
            }

            var indexToRemove = records.FindIndex(p => p.Id == id);

            if (indexToRemove != -1)
            {

                records.RemoveAt(indexToRemove);

                using (var writer = new StreamWriter(FilePath))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    await csv.WriteRecordsAsync(records);
                 
                }
            }
        }

        public async Task DeleteMultipleAsync(IEnumerable<string> ids)
        {
            var records = new List<Position>();

            using (var reader = new StreamReader(FilePath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                records = (await csv.GetRecordsAsync<Position>().ToListAsync());
            }

            foreach (var id in ids)
            {
                var indexToRemove = records.FindIndex(p => p.Id == id);
                if (indexToRemove != -1)
                {
                    records.RemoveAt(indexToRemove);
                }
            }

            using (var writer = new StreamWriter(FilePath))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                await csv.WriteRecordsAsync(records);
            }
        }

    }
}
