using BMWDALInterfacesAndDTOs.DTOs;
using BMWDALInterfacesAndDTOs.Interfaces;
using BMWDALInterfacesAndDTOs.Exceptions;
using System.Data.SqlClient;


namespace BMW.Dal;

public class CommentRepository : ICommentRepository
{
    private readonly string _connectionString;
        
    public CommentRepository(string connectionString)
    {
        _connectionString = connectionString;
    }
    
    public void CreateComment(CommentDTO commentDto)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            SqlTransaction transaction = connection.BeginTransaction();

            try
            {
                string query = "INSERT INTO Comment (Text, CreatedAt, OwnerId, ThreadId, Parent_id) VALUES (@text, @createdAt, @ownerId, @threadId, @parent_Id); SELECT SCOPE_IDENTITY();";

                using (SqlCommand command = new SqlCommand(query, connection, transaction))
                {
                    command.Parameters.AddWithValue("@ownerId", commentDto.OwnerId);
                    command.Parameters.AddWithValue("@threadId", commentDto.ThreadId);
                    command.Parameters.AddWithValue("@text", commentDto.Text);
                    command.Parameters.AddWithValue("@createdAt", commentDto.CreatedAt);
                    if (commentDto.ParentId == -1)
                    {
                        command.Parameters.AddWithValue("@parent_Id", DBNull.Value);
                    }
                    else
                    {
                        command.Parameters.AddWithValue("@parent_Id", commentDto.ParentId);
                    }

                    int commentId = Convert.ToInt32(command.ExecuteScalar());
                    transaction.Commit();
                    commentDto.CommentId = commentId;
                }
            }
            catch (SqlException ex) when (ex.Number == 53 || (ex.InnerException is System.ComponentModel.Win32Exception && ((System.ComponentModel.Win32Exception)ex.InnerException).NativeErrorCode == 53))
            {
                // Handle network issues specifically
                throw new NetworkException("Network-related error occurred while connecting to the database.", ex);
            }
            catch (Exception e)
            {
                transaction.Rollback();
                throw new DalException("error while creating comment", e);
            }
            finally
            {
                connection.Close();
            }
        }
    }
    
    public RootAndChilderenCommentsDTO GetCommentsByThreadId(int threadId)
    {
        List<CommentDTO> rootcomments = new List<CommentDTO>();
        List<CommentDTO> childcomments = new List<CommentDTO>();

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();

            try
            {
                string query = "SELECT CommentId, Text, OwnerId, ThreadId, CreatedAt, Parent_id FROM Comment WHERE ThreadId = @threadId";
                
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@threadId", threadId);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (reader.IsDBNull(reader.GetOrdinal("Parent_id")))
                            {
                                CommentDTO comment = new CommentDTO
                                {
                                    CommentId = reader.GetInt32(reader.GetOrdinal("CommentId")),
                                    Text = reader.GetString(reader.GetOrdinal("Text")),
                                    CreatedAt = reader.GetDateTimeOffset(reader.GetOrdinal("CreatedAt")),
                                    OwnerId = reader.GetInt32(reader.GetOrdinal("OwnerId")),
                                    ThreadId = reader.GetInt32(reader.GetOrdinal("ThreadId")),
                                    ParentId = -1,
                                };

                                rootcomments.Add(comment);
                            }
                            else
                            {
                                CommentDTO comment = new CommentDTO
                                {
                                    CommentId = reader.GetInt32(reader.GetOrdinal("CommentId")),
                                    Text = reader.GetString(reader.GetOrdinal("Text")),
                                    CreatedAt = reader.GetDateTimeOffset(reader.GetOrdinal("CreatedAt")),
                                    OwnerId = reader.GetInt32(reader.GetOrdinal("OwnerId")),
                                    ThreadId = reader.GetInt32(reader.GetOrdinal("ThreadId")),
                                    ParentId = reader.GetInt32(reader.GetOrdinal("Parent_id"))
                                };

                                childcomments.Add(comment);
                            }

                        }
                    }
                }
            }
            catch (SqlException ex) when (ex.Number == 53 || (ex.InnerException is System.ComponentModel.Win32Exception && ((System.ComponentModel.Win32Exception)ex.InnerException).NativeErrorCode == 53))
            {
                // Handle network issues specifically
                throw new NetworkException("Network-related error occurred while connecting to the database.", ex);
            }
            catch (Exception e)
            {
                throw new DalException("error while retrieving comments", e);
            }
            finally
            {
                connection.Close();
            }
        }
        
        return new RootAndChilderenCommentsDTO
        {
            RootComments = rootcomments,
            ChildComments = childcomments
        };
    }
}