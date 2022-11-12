using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using SgupsPlanner.Database.Entities;
using SgupsPlanner.Database.Interfaces;

namespace SgupsPlanner.Database.Database
{
    public class DataBaseFile : IDataBaseFile<FileDto, bool>
    {
        private readonly DatabaseConnection _client;
        public DataBaseFile()
        {
            _client = DatabaseConnection.Source;
        }

        #region Implementation of IDatabase<FileDto,bool>

        public async Task<List<FileDto>> SelectAllAsync(bool includeNestedData)
        {
            const string command = "SELECT * FROM Files";
            var files = new List<FileDto>();
            try
            {
                using (var cmd = new SqliteCommand(command, _client.OpenConnection()))
                {
                    var dataReader = await cmd.ExecuteReaderAsync();
                    while (dataReader.Read())
                    {
                        var id = dataReader.GetInt32(0);
                        var eventId = dataReader.GetInt32(1);
                        var fileName = dataReader.GetString(2);
                        var createDate = dataReader.GetDateTime(3);

                        files.Add(new FileDto(id, eventId,fileName,createDate));
                    }
                }
                _client.CloseConnection();
                
                return files;
            }
            catch (Exception exception)
            {
                Debug.WriteLine("[DatabaseFile.SelectAllAsync()] Error: " + exception.Message);
                _client.CloseConnection();
                return null;
            }
            finally
            {
                _client.CloseConnection();
            }
        }

        public async Task<FileDto> SelectByIdAsync(int idObject, bool includeNestedData = false)
        {
            const string command = "SELECT * FROM Files WHERE Id = @ID";
            var file = new FileDto();
            try
            {
                using (var cmd = new SqliteCommand(command, _client.OpenConnection()))
                {
                    cmd.Parameters.AddWithValue("@ID", idObject);
                    var dataReader = await cmd.ExecuteReaderAsync();
                    while (dataReader.Read())
                    {
                        var id = dataReader.GetInt32(0);
                        var eventId = dataReader.GetInt32(1);
                        var fileName = dataReader.GetString(2);
                        var createDate = dataReader.GetDateTime(3);

                        file = new FileDto(id, eventId, fileName, createDate);
                    }
                }
                _client.CloseConnection();

                return file;
            }
            catch (Exception exception)
            {
                Debug.WriteLine("[DatabaseFile.SelectByIdAsync()] Error: " + exception.Message);
                _client.CloseConnection();
                return null;
            }
            finally
            {
                _client.CloseConnection();
            }
        }

        public async Task<bool> UpdateAsync(FileDto newObject)
        {
            var updateCommand = "UPDATE Files " +
                                   $"SET FileName = @FileName, " +
                                   $"CreateDate = @Created " +
                                   $"FROM(SELECT * FROM Files WHERE Id = @ID) AS Selected WHERE Files.Id = Selected.Id";

            try
            {
                using (var cmd = new SqliteCommand(updateCommand, _client.OpenConnection()))
                {

                    cmd.Parameters.Add("@FileName", SqliteType.Text);
                    cmd.Parameters["@FileName"].Value = newObject.FileName;

                    cmd.Parameters.Add("@CreateDate", SqliteType.Text);
                    cmd.Parameters["@CreateDate"].Value = newObject.CreateDate;

                    cmd.Parameters.Add("@ID", SqliteType.Integer);
                    cmd.Parameters["@ID"].Value = newObject.Id;

                    var row = await cmd.ExecuteNonQueryAsync();
                }
                _client.CloseConnection();
                return true;
            }
            catch (Exception exception)
            {
                Debug.WriteLine("[DatabaseFile.Update()] Error: " + exception.Message);
                _client.CloseConnection();
                return false;
            }
            finally
            {
                _client.CloseConnection();
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var commandDelete = $"DELETE Files WHERE Id = @ID";
            try
            {
                using (var cmd = new SqliteCommand(commandDelete, _client.OpenConnection()))
                {
                    cmd.Parameters.Add("@ID", SqliteType.Integer);
                    cmd.Parameters["@ID"].Value = id;

                    var row = await cmd.ExecuteNonQueryAsync();
                }
                _client.CloseConnection();
                return true;
            }
            catch (Exception exception)
            {
                Debug.WriteLine("[DatabaseFile.Delete()] Error: " + exception.Message);
                _client.CloseConnection();
                return false;
            }
            finally
            {
                _client.CloseConnection();
            }
        }

        #endregion

        #region Implementation of IDataBaseFile<FileDto,bool>

        public async Task<List<FileDto>> GetFilesByEventAsync(int idEvent, bool enableSubQuery = false)
        {
            _client.CloseConnection();
            const string command = "SELECT * FROM Files WHERE Files.EventId = @ID";
            var files = new List<FileDto>();
            try
            {
                using (var cmd = new SqliteCommand(command, _client.OpenConnection()))
                {
                    cmd.Parameters.AddWithValue("@ID", idEvent);
                    var dataReader = await cmd.ExecuteReaderAsync();
                    while (dataReader.Read())
                    {
                        var id = dataReader.GetInt32(0);
                        var eventId = dataReader.GetInt32(1);
                        var fileName = dataReader.GetString(2);
                        var createDate = dataReader.GetDateTime(3);

                        files.Add(new FileDto(id, eventId, fileName, createDate));
                    }
                }
                _client.CloseConnection();

                return files;
            }
            catch (Exception exception)
            {
                Debug.WriteLine("[DatabaseFile.GetFilesByStudentAsync()] Error: " + exception.Message);
                _client.CloseConnection();
                return null;
            }
            finally
            {
                _client.CloseConnection();
            }
        }

        public async Task<bool> CreateAsync(int idEvent, FileDto newObject)
        {
            const string command = "INSERT INTO Files" +
                                   " (EventId, FileName, CreateDate)" +
                                   " VALUES(@EventId, @Name, @Date)";

            _client.CloseConnection();
            try
            {
                using (var cmd = new SqliteCommand(command, _client.OpenConnection()))
                {
                    cmd.Parameters.AddWithValue("@EventId", idEvent);
                    cmd.Parameters.AddWithValue("@Name", newObject.FileName);
                    cmd.Parameters.AddWithValue("@Date", newObject.CreateDate);
                    var row = await cmd.ExecuteNonQueryAsync();
                    Debug.WriteLine("[DatabaseFile.CreateAsync()] Rows: " + row);
                }
                _client.CloseConnection();
                return true;
            }
            catch (Exception exception)
            {
                Debug.WriteLine("[DatabaseFile.CreateAsync()] Error: " + exception.Message);
                _client.CloseConnection();
                return false;
            }
            finally
            {
                _client.CloseConnection();
            }
        }

        #endregion
    }
}