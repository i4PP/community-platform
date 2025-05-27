using System.Data.SqlClient;
using BMWDALInterfacesAndDTOs.DTOs;
using BMWDALInterfacesAndDTOs.Exceptions;
using BMWDALInterfacesAndDTOs.Interfaces;

namespace BMW.Dal;

public class DiscussionThreadRepository : IDiscussionThreadRepository
{
    private readonly string _connectionString;

    public DiscussionThreadRepository(string connectionString)
    {
        _connectionString = connectionString;
    }


    public void CreateThread(DiscussionThreadDTO threadDto)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            var transaction = connection.BeginTransaction();

            try
            {
                var query =
                    "INSERT INTO Thread (Title, Text, OwnerId, CreatedAt, TopicId)  OUTPUT INSERTED.ThreadId  VALUES (@Title, @Text, @OwnerId, @CreatedAt, @TopicId)";

                using (var command = new SqlCommand(query, connection, transaction))
                {
                    command.Parameters.AddWithValue("@Title", threadDto.Title);
                    command.Parameters.AddWithValue("@Text", threadDto.Text);
                    command.Parameters.AddWithValue("@OwnerId", threadDto.OwnerId);
                    command.Parameters.AddWithValue("@CreatedAt", threadDto.CreatedAt);
                    command.Parameters.AddWithValue("@TopicId", threadDto.TopicId);

                    var id = Convert.ToInt32(command.ExecuteScalar());

                    transaction.Commit();
                    threadDto.ThreadId = id;
                }
            }
            catch (SqlException ex) when (ex.Number == -1 || ex.InnerException is System.Net.Sockets.SocketException)
            {
                // Handle network issues specifically
                transaction.Rollback();
                throw new NetworkException("Network-related error occurred while connecting to the database.", ex);
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new DalException("An error occurred in the Dal", ex);
            }
        }
    }


    public DiscussionThreadDTO GetThreadById(int id)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            var query =
                "SELECT ThreadID, Title, Text, OwnerId, CreatedAt, TopicId, IsEdited FROM Thread WHERE ThreadId = @Value1";

            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Value1", id);

                connection.Open();
                var reader = command.ExecuteReader();
                try
                {
                    if (reader.Read())
                    {
                        var ThreadDto = new DiscussionThreadDTO
                        {
                            ThreadId = reader.GetInt32(reader.GetOrdinal("ThreadID")),
                            Title = reader.GetString(reader.GetOrdinal("Title")),
                            Text = reader.GetString(reader.GetOrdinal("Text")),
                            OwnerId = reader.GetInt32(reader.GetOrdinal("OwnerId")),
                            CreatedAt = reader.GetDateTimeOffset(reader.GetOrdinal("CreatedAt")),
                            TopicId = reader.GetInt32(reader.GetOrdinal("TopicId")),
                            IsEdited = reader.GetBoolean(reader.GetOrdinal("IsEdited"))
                        };

                        return ThreadDto;
                    }
                    else
                    {
                        throw new NotFoundException($"Thread with ID {id} not found.");
                    }
                }
                catch (SqlException ex) when (ex.Number == 53 || (ex.InnerException is System.ComponentModel.Win32Exception && ((System.ComponentModel.Win32Exception)ex.InnerException).NativeErrorCode == 53))
                {
                    // Handle network issues specifically
                    throw new NetworkException("Network-related error occurred while connecting to the database.", ex);
                }
                catch (Exception e)
                {
                    throw new DalException("An error occurred in the Dal", e);
                }
                finally
                {
                    reader.Close();
                }
            }
        }
    }


    public List<DiscussionThreadDTO> GetAllThreads()
    {
        var threads = new List<DiscussionThreadDTO>();

        using (var connection = new SqlConnection(_connectionString))
        {
            var query =
                "SELECT ThreadId, Title, Text, OwnerId, CreatedAt, TopicId, IsEdited FROM Thread WHERE IsDeleted = 0";

            using (var command = new SqlCommand(query, connection))
            {
                connection.Open();
                var reader = command.ExecuteReader();


                try
                {
                    while (reader.Read())
                    {
                        var thread = new DiscussionThreadDTO
                        {
                            ThreadId = reader.GetInt32(reader.GetOrdinal("ThreadID")),
                            Title = reader.GetString(reader.GetOrdinal("Title")),
                            Text = reader.GetString(reader.GetOrdinal("Text")),
                            OwnerId = reader.GetInt32(reader.GetOrdinal("OwnerId")),
                            CreatedAt = reader.GetDateTimeOffset(reader.GetOrdinal("CreatedAt")),
                            TopicId = reader.GetInt32(reader.GetOrdinal("TopicId")),
                            IsEdited = reader.GetBoolean(reader.GetOrdinal("IsEdited"))
                        };
                        threads.Add(thread);
                    }
                }
                catch (SqlException ex) when (ex.Number == 53 || (ex.InnerException is System.ComponentModel.Win32Exception && ((System.ComponentModel.Win32Exception)ex.InnerException).NativeErrorCode == 53))
                {
                    // Handle network issues specifically
                    throw new NetworkException("Network-related error occurred while connecting to the database.", ex);
                }
                catch (Exception e)
                {
                    throw new DalException("An error occurred in the Dal", e);
                }
                finally
                {
                    reader.Close();
                }
            }
        }

        return threads;
    }

    public DeletedResultDTO DeleteThreadById(int id)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            var transaction = connection.BeginTransaction();


            try
            {
                var query =
                    "UPDATE Thread SET Title = '[Removed]', Text = '[Removed]', IsDeleted = 1 WHERE ThreadId = @Value1 ";

                using (var command = new SqlCommand(query, connection, transaction))
                {
                    command.Parameters.AddWithValue("@Value1", id);

                    var rowEffected = command.ExecuteNonQuery();
                    transaction.Commit();

                    var deletedResultDTO = new DeletedResultDTO
                    {
                        DeletedAmount = rowEffected
                    };

                    return deletedResultDTO;
                }
            }
            catch (SqlException ex) when (ex.Number == 53 || (ex.InnerException is System.ComponentModel.Win32Exception && ((System.ComponentModel.Win32Exception)ex.InnerException).NativeErrorCode == 53))
            {
                // Handle network issues specifically
                throw new NetworkException("Network-related error occurred while connecting to the database.", ex);
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new DalException("Database error", ex);
            }
        }
    }


    public List<DiscussionThreadDTO> GetThreadsByUserId(int id)
    {
        var UserTheards = new List<DiscussionThreadDTO>();

        using (var connection = new SqlConnection(_connectionString))
        {
            var query =
                "SELECT ThreadId, Title, Text, OwnerId, CreatedAt, TopicId, IsEdited FROM Thread WHERE OwnerId = @Value1 AND IsDeleted = 0";


            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Value1", id);
                connection.Open();
                var reader = command.ExecuteReader();

                try
                {
                    while (reader.Read())
                    {
                        var thread = new DiscussionThreadDTO
                        {
                            ThreadId = reader.GetInt32(reader.GetOrdinal("ThreadID")),
                            Title = reader.GetString(reader.GetOrdinal("Title")),
                            Text = reader.GetString(reader.GetOrdinal("Text")),
                            OwnerId = reader.GetInt32(reader.GetOrdinal("OwnerId")),
                            CreatedAt = reader.GetDateTimeOffset(reader.GetOrdinal("CreatedAt")),
                            TopicId = reader.GetInt32(reader.GetOrdinal("TopicId")),
                            IsEdited = reader.GetBoolean(reader.GetOrdinal("IsEdited"))
                        };
                        UserTheards.Add(thread);
                    }

                    return UserTheards;
                }
                catch (SqlException ex) when (ex.Number == 53 || (ex.InnerException is System.ComponentModel.Win32Exception && ((System.ComponentModel.Win32Exception)ex.InnerException).NativeErrorCode == 53))
                {
                    // Handle network issues specifically
                    throw new NetworkException("Network-related error occurred while connecting to the database.", ex);
                }
                catch (Exception e)
                {
                    throw new DalException("An error occurred in the Dal", e);
                }
                finally
                {
                    reader.Close();
                }
            }
        }
    }

    public void EditThread(DiscussionThreadDTO threadDto)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            var transaction = connection.BeginTransaction();

            try
            {
                var query =
                    "UPDATE Thread SET Title = @Title, Text = @Text, OwnerId = @OwnerId, CreatedAt = @CreatedAt, TopicId = @TopicId, IsEdited = 1 WHERE ThreadId = @Value1";

                using (var command = new SqlCommand(query, connection, transaction))
                {
                    command.Parameters.AddWithValue("@Title", threadDto.Title);
                    command.Parameters.AddWithValue("@Text", threadDto.Text);
                    command.Parameters.AddWithValue("@OwnerId", threadDto.OwnerId);
                    command.Parameters.AddWithValue("@CreatedAt", threadDto.CreatedAt);
                    command.Parameters.AddWithValue("@TopicId", threadDto.TopicId);
                    command.Parameters.AddWithValue("@Value1", threadDto.ThreadId);

                    command.ExecuteNonQuery();
                    transaction.Commit();
                }
            }
            catch (SqlException ex) when (ex.Number == 53 || (ex.InnerException is System.ComponentModel.Win32Exception && ((System.ComponentModel.Win32Exception)ex.InnerException).NativeErrorCode == 53))
            {
                // Handle network issues specifically
                throw new NetworkException("Network-related error occurred while connecting to the database.", ex);
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new DalException("An error occurred in the Dal", ex);
            }
        }
    }
}