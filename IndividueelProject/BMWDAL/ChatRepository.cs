using BMWDALInterfacesAndDTOs.DTOs;
using BMWDALInterfacesAndDTOs.Interfaces;
using BMWDALInterfacesAndDTOs.Exceptions;
using System.Data.SqlClient;


namespace BMW.Dal;

public class ChatRepository : IChatRepository
{
    private readonly string _connectionString;

    public ChatRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public void CreateMessage(MessageDTO dto)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            SqlTransaction transaction = connection.BeginTransaction();

            try
            {
                string query =
                    "INSERT INTO Chat (UserId, ClubId, message) VALUES (@userId, @clubId, @message); SELECT SCOPE_IDENTITY();";

                using (SqlCommand command = new SqlCommand(query, connection, transaction))
                {
                    command.Parameters.AddWithValue("@userId", dto.UserId);
                    command.Parameters.AddWithValue("@clubId", dto.ClubId);
                    command.Parameters.AddWithValue("@message", dto.Content);

                    int id = Convert.ToInt32(command.ExecuteScalar());
                    transaction.Commit();
                    dto.MessageId = id;
                }
            }
            catch (SqlException ex) when (ex.Number == 53 || (ex.InnerException is System.ComponentModel.Win32Exception && ((System.ComponentModel.Win32Exception)ex.InnerException).NativeErrorCode == 53))
            {
                // Handle network issues specifically
                throw new NetworkException("Network-related error occurred while connecting to the database.", ex);
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw new DalException("error while creating message");
                
            }
            finally
            {
                connection.Close();
            }
        }
        
    }

    public List<MessageDTO> GetClubMessages(int clubId)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            string query = "SELECT ChatId, UserId, ClubId, Message FROM Chat WHERE ClubId = @clubId";
            List<MessageDTO> messages = new List<MessageDTO>();
            try
            {
                    using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@clubId", clubId);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            messages.Add(new MessageDTO
                            {
                                MessageId = reader.GetInt32(reader.GetOrdinal("ChatId")),
                                UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                                ClubId = reader.GetInt32(reader.GetOrdinal("ClubId")),
                                Content = reader.GetString(reader.GetOrdinal("Message"))
                            });
                        }
                    }
                }
            }
            catch (SqlException ex) when (ex.Number == 53 || (ex.InnerException is System.ComponentModel.Win32Exception && ((System.ComponentModel.Win32Exception)ex.InnerException).NativeErrorCode == 53))
            {
                // Handle network issues specifically
                throw new NetworkException("Network-related error occurred while connecting to the database.", ex);
            }
            catch (SqlException e)
            {
                throw new DalException("Error in ChatRepository", e);
            }
            return messages;
        }
        
    }
    
    

    
    
}