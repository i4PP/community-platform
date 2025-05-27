
using System.Data.SqlClient;
using BMWDALInterfacesAndDTOs.DTOs;
using BMWDALInterfacesAndDTOs.Interfaces;
using BMWDALInterfacesAndDTOs.Exceptions;

namespace BMW.Dal
{
    public class TopicRepository : ITopicRepository
    {

        private readonly string _connectionString;
        
        public TopicRepository(string connectionString)
        {
            _connectionString = connectionString;
        }


        public List<TopicDTO> GetAllTopic()
        {
            List<TopicDTO> topics = new List<TopicDTO>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {

                try
                {
                    string query = "SELECT TopicId, Name FROM Topic";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            TopicDTO topic = new TopicDTO
                            {
                                TopicId = reader.GetInt32(reader.GetOrdinal("TopicId")),
                                Name = reader.GetString(reader.GetOrdinal("Name"))

                            };
                            topics.Add(topic);
                            
                        }

                    }
                    return topics;
                }
                catch (SqlException ex) when (ex.Number == -1 || ex.InnerException is System.Net.Sockets.SocketException)
                {
                    throw new NetworkException("Network-related error occurred while connecting to the database.", ex);
                }
                catch (Exception ex)
                {
                    throw new DalException("error while retrieving topics", ex);

                }
            }
        }

        public TopicDTO GetTopicById(int id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT Name FROM Topic WHERE TopicId = @Value1";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Value1", id);

                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    try
                    {
                        if (reader.Read())
                        {
                            TopicDTO topic = new TopicDTO
                            {
                                TopicId = id,
                                Name = reader.GetString(reader.GetOrdinal("Name"))

                            };
                            return topic;

                        }
                        else
                        {
                            throw new NotFoundException("Topic not found");
                        }

                    }
                    catch (SqlException ex) when (ex.Number == -1 || ex.InnerException is System.Net.Sockets.SocketException)
                    {
                        // Handle network issues specifically
                        throw new NetworkException("Network-related error occurred while connecting to the database.", ex);
                    }
                    catch (Exception e)
                    {
                        throw new DalException("Error while retrieving topic", e);
                    }
                    finally
                    {
                        reader.Close();
                    }
                    
                }
            }
        }   
    }
}
    

