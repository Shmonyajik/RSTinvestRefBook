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
using System.Text.RegularExpressions;
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

        public async Task<BaseResponse<IEnumerable<Position>>> GetAllPositionsAsync()
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

        public async Task<BaseResponse<bool>> EditPositionsListAsync(List<Position> positions)
        {
            var response = new BaseResponse<bool>();
            try
            {
                StringBuilder allValidationErrors = new StringBuilder();
                for (int i = 0; i < positions.Count; i++)
                {
                    var validationErrors = ValidatePosition(positions[i]);

                    var positoion = positions.FirstOrDefault(x => x.HexId == positions[i].HexId && x.Id != positions[i].Id);
                    if(positoion != null)
                    {
                        allValidationErrors.AppendLine($"Строка {i + 1}: идентификатор {positions[i].HexId} не уникален;");
                    }
                    if(validationErrors.Count > 0)
                    {
                        allValidationErrors.AppendLine($"Строка {i + 1}: {string.Join(",", validationErrors)};");
                        response.StatusCode = Enums.StatusCode.ValidationError;
                    }
                }
                if(allValidationErrors.Length > 0)
                {
                    response.Description = allValidationErrors.ToString();
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

        public async Task<BaseResponse<IEnumerable<Position>>>GetPositionsByHexIdsAsync(IEnumerable<string> hexIds)
        {
            var response = new BaseResponse<IEnumerable<Position>>();
            try
            { 
                var positions = new List<Position>();
                var allValidationErrors = new StringBuilder();
                foreach (var id in hexIds)
                {
                    var position = await _positionRepository.GetByHexIdAsync(id.ToUpper());
                    
                    if (position != null)
                    {
                        
                        positions.Add(position);
                    }
                    else
                    {
                        allValidationErrors.AppendLine($"Идентификатор {id} не найден в справочнике");
                    }
                }
                if(allValidationErrors.Length > 0)
                {
                    response.Description = allValidationErrors.ToString();
                    response.StatusCode = Enums.StatusCode.ValidationError;
                    return response;
                }
                if (positions.Count == 0)
                {
                    response.StatusCode = Enums.StatusCode.NotFound;
                    response.Description = "В справочнике не найдено ни одного введенного HEX идентификатора.";
                    return response;
                }
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
        private List<string> ValidatePosition(Position position)
        {
            
            var validationErrors = new List<string>();
            string regexPattern = ConfigurationManager.AppSettings["HexIdRegexPattern"];//^[0-9A-Fa-f]{24}$
            if (string.IsNullOrWhiteSpace(regexPattern))
            {
                throw new ConfigurationErrorsException("HexIdRegexPattern не найден в файле конфигурации.");
            }
            if (string.IsNullOrEmpty(position.Name))
            {
                validationErrors.Add("Имя позиции не может быть пустым");
            }
            
            if(string.IsNullOrEmpty(position.HexId) || !Regex.IsMatch(position.HexId.ToUpper(), regexPattern))
            {
                
                validationErrors.Add("HEX идентификатор не соответствует формату");
            }
            position.HexId = position.HexId.ToUpper();
            return validationErrors;
        }
        #region OLD
        //private string GenerateHexId()
        //{
        //    int.TryParse(ConfigurationManager.AppSettings.Get("HexIdLength"), out int length);
        //    // Генерируем случайные байты
        //    byte[] buffer = new byte[length / 2];
        //    new Random().NextBytes(buffer);

        //    // Преобразуем байты в HEX строку
        //    StringBuilder builder = new StringBuilder(length);
        //    for (int i = 0; i < buffer.Length; i++)
        //    {
        //        builder.Append(buffer[i].ToString("X2"));
        //    }
        //    return builder.ToString();
        //}

        //public async Task<BaseResponse<Position>> GetPositionByIdAsync(string id)
        //{
        //    var response = new BaseResponse<Position>();
        //    try
        //    {
        //        var position = await _positionRepository.GetByHexIdAsync(id);
        //        response.Data = position;
        //        response.StatusCode = Enums.StatusCode.OK;
        //    }
        //    catch (IOException ex)
        //    {
        //        response.Description = "Произошла ошибка ввода/вывода: " + ex.Message;
        //        response.StatusCode = Enums.StatusCode.NotFound;
        //    }
        //    catch (CsvHelperException ex)
        //    {
        //        response.Description = "Произошла ошибка при работе с CSV: " + ex.Message;
        //        response.StatusCode = Enums.StatusCode.NotFound;
        //    }
        //    catch (InvalidOperationException ex)
        //    {
        //        response.Description = ex.Message;
        //        response.StatusCode = Enums.StatusCode.NotFound;
        //    }
        //    catch (Exception ex)
        //    {
        //        response.Description = ex.Message;
        //        response.StatusCode = Enums.StatusCode.InternalServerError;
        //    }
        //    return response;
        //}

        #endregion
    }
}
