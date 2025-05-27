
using System.Data.SqlClient;
using BMWDALInterfacesAndDTOs.DTOs;
using BMWDALInterfacesAndDTOs.Interfaces;
using BMWDALInterfacesAndDTOs.Exceptions;

namespace BMW.Dal
{
    public class UserRepository : IUserRepository

    {

        private readonly string _connectionString;
        
        public UserRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public UserDTO GetUserById(int id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT UserId, Username, Email FROM [User] WHERE UserId = @id;";
                
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", id);

                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    try
                    {
                        if (reader.Read())
                        {
                            UserDTO user = new UserDTO();
                            user.UserId = reader.GetInt32(0);
                            user.Name = reader.GetString(1);
                            user.Email = reader.GetString(2);
                            return user;

                        }
                        else
                        {
                            throw new NotFoundException($"user not {id} found");
                        }
                    }
                    catch (SqlException ex) when (ex.Number == 53 || (ex.InnerException is System.ComponentModel.Win32Exception && ((System.ComponentModel.Win32Exception)ex.InnerException).NativeErrorCode == 53))
                    {
                        // Handle network issues specifically
                        throw new NetworkException("Network-related error occurred while connecting to the database.", ex);
                    }
                    catch (Exception e)
                    {
                        throw new DalException("error while retrieving user", e);
                    }
                    finally
                    {
                        reader.Close();
                    }
                }
            }
        }

        public void RegisterUser(AccountDTO account)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    string query = "INSERT INTO [User] (Username, Password, Email) VALUES (@Username, @Password, @Email); SELECT SCOPE_IDENTITY();";

                    using (SqlCommand command = new SqlCommand(query, connection, transaction))
                    {
                        command.Parameters.AddWithValue("@Username", account.Name);
                        command.Parameters.AddWithValue("@Password", account.Password);
                        command.Parameters.AddWithValue("@Email", account.Email);

                        int id = Convert.ToInt32(command.ExecuteScalar());

                        transaction.Commit();
                        account.UserId = id;
                        
                    }
                }
                catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
                {
                    // Handle duplicate entry specifically
                    transaction.Rollback();
                    throw new DuplicateEntryException("A user with this combination of UserId, ClubId, and RoleId already exists.", ex);
                }
                catch (SqlException ex) when (ex.Number == 53 || (ex.InnerException is System.ComponentModel.Win32Exception && ((System.ComponentModel.Win32Exception)ex.InnerException).NativeErrorCode == 53))
                {
                    // Handle network issues specifically
                    throw new NetworkException("Network-related error occurred while connecting to the database.", ex);
                }
                catch(Exception e)
                {
                    transaction.Rollback();
                    throw new DalException("error while registering user", e);


                }
            }
        }

        public void GetUserByNameOrEmail(LoginDTO login)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT UserId, Password FROM [User] WHERE Username = @Name OR Email = @Name;";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", login.User); 

                    try
                    {
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();

                        if (reader.Read())
                        {
                            login.Password = (reader["Password"] as byte[])!;
                            login.Id = (int)reader["UserId"];
                        }
                        else
                        {
                            throw new NotFoundException($"user {login.User} not found");
                        }
                    }
                    catch (SqlException ex) when (ex.Number == 53 || (ex.InnerException is System.ComponentModel.Win32Exception && ((System.ComponentModel.Win32Exception)ex.InnerException).NativeErrorCode == 53))
                    {
                        // Handle network issues specifically
                        throw new NetworkException("Network-related error occurred while connecting to the database.", ex);
                    }
                    catch (Exception e)
                    {
                        throw new DalException("error while retrieving user", e);
                    }
                }
            }
        }
    }


}
