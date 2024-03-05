using CsvHelper;
using RSTinvestRefBook.Models;
using RSTinvestRefBook.Repositories;
using RSTinvestRefBook.Responses;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSTinvestRefBook.Services
{
    public class RefBookService : IRefBookService
    {
        private readonly BaseRepository<Position> _positionRepository;
        public RefBookService(BaseRepository<Position> positionRepository)
        {
            _positionRepository = positionRepository;
        }
        public async Task<BaseResponse<Position>> GetPosition(string id)
        {
            var response = new BaseResponse<Position>();
            try
            {
                var position = await _positionRepository.GetByIdAsync(id);
                response.StatusCode = Enums.StatusCode.OK;
                response.Data = position;
            }
            catch (IOException ex)
            {
                response.Description = "Произошла ошибка ввода/вывода: " + ex.Message;
                response.StatusCode = Enums.StatusCode.NotFound;
            }
            catch (CsvHelperException ex)
            {
                response.Description = "Произошла ошибка при работе с CSV: " + ex.Message;
                response.StatusCode = Enums.StatusCode.NotFound;
            }
            catch (InvalidOperationException ex)
            {
                response.Description = ex.Message;
                response.StatusCode = Enums.StatusCode.NotFound;
            }
            catch (Exception ex)
            {
                response.Description = ex.Message;
                response.StatusCode = Enums.StatusCode.InternalServerError;
            }
            return response;
        }

        public async Task<BaseResponse<IEnumerable<Position>>> GetAllPositions()
        {
            var response = new BaseResponse<IEnumerable<Position>>();
            try
            {
                var positions = await _positionRepository.GetAllAsync();
                response.Data = positions;
                response.StatusCode = Enums.StatusCode.OK;
            }
            catch (IOException ex)
            {
                response.Description = "Произошла ошибка ввода/вывода: " + ex.Message;
                response.StatusCode = Enums.StatusCode.NotFound;
            }
            catch (CsvHelperException ex)
            {
                response.Description = "Произошла ошибка при работе с CSV: " + ex.Message;
                response.StatusCode = Enums.StatusCode.NotFound;
            }
            catch (InvalidOperationException ex)
            {
                response.Description = ex.Message;
                response.StatusCode = Enums.StatusCode.NotFound;
            }
            catch (Exception ex)
            {
                response.Description = ex.Message;
                response.StatusCode = Enums.StatusCode.InternalServerError;
            }
            return response;
        }

        public BaseResponse<bool> CreatePosition(Position position)
        {
            var response = new BaseResponse<bool>();
            try
            {
               if(string.IsNullOrEmpty(position.Name))
                {
                    response.StatusCode = Enums.StatusCode.ValidationError;
                    response.Description = "Имя позиции не может быть пустым.";
                    return response;
                }
                position.Id = GenerateHexId();
                _positionRepository.Create(position);
                response.StatusCode = Enums.StatusCode.OK;
            }
            catch (IOException ex)
            {
                response.Description = "Произошла ошибка ввода/вывода: " + ex.Message;
                response.StatusCode = Enums.StatusCode.NotFound;
            }
            catch (CsvHelperException ex)
            {
                response.Description = "Произошла ошибка при работе с CSV: " + ex.Message;
                response.StatusCode = Enums.StatusCode.NotFound;
            }
            catch (Exception ex)
            {
                response.Description = ex.Message;
                response.StatusCode = Enums.StatusCode.InternalServerError;
            }
            return response;
        }

        public async Task<BaseResponse<bool>> EditPositionsList(List<Position> positions)
        {
            var response = new BaseResponse<bool>();
            try
            {
                StringBuilder validationErrors = new StringBuilder();
                for (int i = 0; i < positions.Count; i++)
                {
                    if (string.IsNullOrEmpty(positions[i].Name))
                    {
                        response.StatusCode = Enums.StatusCode.ValidationError;
                        validationErrors.AppendLine($"Строка {i + 1}: Имя позиции не может быть пустым.");
                    }
                    if (positions[i].Quantity < 1)
                    {
                        response.StatusCode = Enums.StatusCode.ValidationError;
                        validationErrors.AppendLine($"Строка {i + 1}: Количество должно быть больше нуля.");
                    }
                    if (string.IsNullOrEmpty(positions[i].Id) && response.StatusCode!= Enums.StatusCode.ValidationError)
                    {
                        positions[i].Id = GenerateHexId();
                    }
                }
                if(validationErrors.Length > 0)
                {
                    response.Description = validationErrors.ToString();
                    return response;
                }

                await _positionRepository.EditAsync(positions);
                response.StatusCode = Enums.StatusCode.OK;
                
                
            }
            catch (IOException ex)
            {
                response.Description = "Произошла ошибка ввода/вывода: " + ex.Message;
                response.StatusCode = Enums.StatusCode.NotFound;
            }
            catch (CsvHelperException ex)
            {
                response.Description = "Произошла ошибка при работе с CSV: " + ex.Message;
                response.StatusCode = Enums.StatusCode.NotFound;
            }
            catch (Exception ex)
            {
                response.Description = ex.Message;
                response.StatusCode = Enums.StatusCode.InternalServerError;
            }
            return response;
        }


        private string GenerateHexId()
        {
            int.TryParse(ConfigurationManager.AppSettings.Get("HexIdLength"), out int length);
            // Генерируем случайные байты
            byte[] buffer = new byte[length / 2];
            new Random().NextBytes(buffer);

            // Преобразуем байты в HEX строку
            StringBuilder builder = new StringBuilder(length);
            for (int i = 0; i < buffer.Length; i++)
            {
                builder.Append(buffer[i].ToString("X2"));
            }
            return builder.ToString();
        }


    }
}
