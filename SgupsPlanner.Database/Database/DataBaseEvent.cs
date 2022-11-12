using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using SgupsPlanner.Database.Entities;
using SgupsPlanner.Database.Interfaces;

namespace SgupsPlanner.Database.Database
{
    public class DataBaseEvent : IDataBaseEvent<EventDto, bool>
    {
        private readonly DatabaseConnection _client;
        public DataBaseEvent()
        {
            _client = DatabaseConnection.Source;
        }

        public Exception GetRootException()
        {
            return _client.RootException;
        }

        #region Implementation of IDatabase<EventDto,bool>

        public async Task<List<EventDto>> SelectAllAsync(bool includeNestedData)
        {
            const string command = "SELECT * FROM Events";
            var events = new List<EventDto>();
            try
            {
                using (var cmd = new SqliteCommand(command, _client.OpenConnection()))
                {
                    var dataReader = await cmd.ExecuteReaderAsync();
                    while (dataReader.Read())
                    {
                        var id = dataReader.GetInt32(0);
                        var eventName = dataReader.GetString(1);
                        var description = dataReader.GetString(2);
                        var isActive = dataReader.GetBoolean(3);
                        var isRepeatable = dataReader.GetBoolean(4);
                        var deadline = dataReader.GetDateTime(5);
                        var startNotify = dataReader.GetDateTime(6);
                        var interval = dataReader.GetInt32(7);
                        var created = dataReader.GetDateTime(8);

                        events.Add(new EventDto(id, eventName, description, isActive, isRepeatable, deadline, startNotify, (RepeatInterval)interval, created));
                    }
                }
                _client.CloseConnection();

                if (includeNestedData)
                {
                    foreach (var e in events)
                    {
                        e.SetFiles(await GetEventFilesAsync(e.Id));
                    }
                }
                else
                {
                    foreach (var e in events)
                    {
                        e.SetFiles(new List<FileDto>());
                    }
                }

                return events;
            }
            catch (Exception exception)
            {
                Debug.WriteLine("[DatabaseEvent.SelectAllAsync()] Error: " + exception.Message);
                _client.RootException = exception;
                _client.CloseConnection();
                return null;
            }
            finally
            {
                _client.CloseConnection();
            }
        }

        public async Task<EventDto> SelectByIdAsync(int idObject, bool includeNestedData)
        {
            _client.CloseConnection();
            const string command = "SELECT * FROM Events WHERE Id = @ID";
            var eventDto = new EventDto();
            try
            {
                using (var cmd = new SqliteCommand(command, _client.OpenConnection()))
                {
                    cmd.Parameters.AddWithValue("@ID", idObject);
                    var dataReader = await cmd.ExecuteReaderAsync();
                    while (dataReader.Read())
                    {
                        var id = dataReader.GetInt32(0);
                        var eventName = dataReader.GetString(1);
                        var description = dataReader.GetString(2);
                        var isActive = dataReader.GetBoolean(3);
                        var isRepeatable = dataReader.GetBoolean(4);
                        var deadline = dataReader.GetDateTime(5);
                        var startNotify = dataReader.GetDateTime(6);
                        var interval = dataReader.GetInt32(7);
                        var created = dataReader.GetDateTime(8);

                        eventDto = new EventDto(id, eventName, description, isActive, isRepeatable, deadline, startNotify, (RepeatInterval)interval, created);
                    }
                }

                _client.CloseConnection();
                if (includeNestedData)
                {
                    eventDto.SetFiles(await GetEventFilesAsync(eventDto.Id));
                }
                else
                {
                    eventDto.SetFiles(new List<FileDto>());
                }

                return eventDto;
            }
            catch (Exception exception)
            {
                Debug.WriteLine("[DatabaseEvent.SelectByIdAsync()] Error: " + exception.Message);
                _client.CloseConnection();
                return null;
            }
            finally
            {
                _client.CloseConnection();
            }
        }

        private async Task<List<FileDto>> GetEventFilesAsync(int eventId)
        {
            var db = new DataBaseFile();
            return await db.GetFilesByEventAsync(eventId);
        }

        public async Task<bool> UpdateAsync(EventDto newObject)
        {
            var updateCommand = "UPDATE Events " +
                                $"SET EventName = @EventName," +
                                $" [Description]=@Description," +
                                $" IsActive=@IsActive," +
                                $" IsRepeatable=@IsRepeatable, " +
                                $"DeadlineDate=@DeadlineDate, " +
                                $"StartNotifyDate=@StartNotifyDate," +
                                $" RepeatInterval=@RepeatInterval," +
                                $" CreateDate=@CreateDate, " +
                                $"FROM(SELECT * FROM Events WHERE Id = @ID) AS Selected WHERE Events.Id = Selected.Id";

            try
            {
                using (var cmd = new SqliteCommand(updateCommand, _client.OpenConnection()))
                {

                    cmd.Parameters.Add("@EventName", SqliteType.Text);
                    cmd.Parameters["@EventName"].Value = newObject.EventName;
                    cmd.Parameters.Add("@Description", SqliteType.Text);
                    cmd.Parameters["@Description"].Value = newObject.Description;
                    cmd.Parameters.Add("@IsActive", SqliteType.Integer);
                    cmd.Parameters["@IsActive"].Value = newObject.IsActive;
                    cmd.Parameters.Add("@IsRepeatable", SqliteType.Integer);
                    cmd.Parameters["@IsRepeatable"].Value = newObject.IsRepeatable;
                    cmd.Parameters.Add("@DeadlineDate", SqliteType.Text);
                    cmd.Parameters["@DeadlineDate"].Value = newObject.DeadlineDate;
                    cmd.Parameters.Add("@StartNotifyDate", SqliteType.Text);
                    cmd.Parameters["@StartNotifyDate"].Value = newObject.StartNotifyDate;
                    cmd.Parameters.Add("@RepeatInterval", SqliteType.Integer);
                    cmd.Parameters["@RepeatInterval"].Value = newObject.RepeatInterval;
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
                Debug.WriteLine("[DatabaseEvent.Update()] Error: " + exception.Message);
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
            var commandDelete = $"DELETE Events WHERE Id = @ID";
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
                Debug.WriteLine("[DatabaseEvent.Delete()] Error: " + exception.Message);
                _client.CloseConnection();
                return false;
            }
            finally
            {
                _client.CloseConnection();
            }
        }

        #endregion

        #region Implementation of IDataBaseEvent<EventDto,bool>

        public async Task<bool> CreateAsync(EventDto newObject)
        {
            const string command = "INSERT INTO Events " +
                                   "(EventName, Description, IsActive, IsRepeatable, DeadlineDate, StartNotifyDate, RepeatInterval, CreateDate) " +
                                   "VALUES(@EventName, @Description, @IsActive, @IsRepeatable, @DeadlineDate, @StartNotifyDate, @RepeatInterval, @CreateDate)";
            const string lastIndexCommand = "select last_insert_rowid()";

            decimal? lastIndex = null;

            try
            {
                using (var cmd = new SqliteCommand(command, _client.OpenConnection()))
                {
                    cmd.Parameters.AddWithValue("@EventName", newObject.EventName);
                    cmd.Parameters.AddWithValue("@Description", newObject.Description);
                    cmd.Parameters.AddWithValue("@IsActive", newObject.IsActive);
                    cmd.Parameters.AddWithValue("@IsRepeatable", newObject.IsRepeatable);
                    cmd.Parameters.AddWithValue("@DeadlineDate", newObject.DeadlineDate);
                    cmd.Parameters.AddWithValue("@StartNotifyDate", newObject.StartNotifyDate);
                    cmd.Parameters.AddWithValue("@RepeatInterval", newObject.RepeatInterval);
                    cmd.Parameters.AddWithValue("@CreateDate", newObject.CreateDate);

                    var row = await cmd.ExecuteNonQueryAsync();
                    Debug.WriteLine("[DatabaseEvent.CreateAsync()] Rows: " + row);
                }
                _client.CloseConnection();
                using (var cmd = new SqliteCommand(lastIndexCommand, _client.OpenConnection()))
                {
                    var dataReader = await cmd.ExecuteReaderAsync();
                    while (dataReader.Read())
                    {
                        lastIndex = dataReader.GetDecimal(0);
                    }
                }
                _client.CloseConnection();
                return await InsertEventFilesAsync((int?)lastIndex, newObject.Files);
            }
            catch (Exception exception)
            {
                Debug.WriteLine("[DatabaseEvent.CreateAsync()] Error: " + exception.Message);
                _client.CloseConnection();
                return false;
            }
            finally
            {
                _client.CloseConnection();
            }
        }

        private async Task<bool> InsertEventFilesAsync(int? lastIndex, IEnumerable<FileDto> files)
        {
            _client.CloseConnection();
            var result = false;
            try
            {
                if (lastIndex != null)
                {
                    var db = new DataBaseFile();
                    foreach (var file in files)
                    {
                        _client.CloseConnection();
                        result = await db.CreateAsync((int)lastIndex, file);
                    }

                    return result;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error: " + e.Message);
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