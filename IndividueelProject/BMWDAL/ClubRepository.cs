using BMWDALInterfacesAndDTOs.DTOs;
using BMWDALInterfacesAndDTOs.Interfaces;
using BMWDALInterfacesAndDTOs.Exceptions;
using System.Data.SqlClient;


namespace BMW.Dal;

public class ClubRepository : IClubRepository
{
    private readonly string _connectionString;

    public ClubRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public void CreateClub(CreateClubDTO createClubDto)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            SqlTransaction transaction = connection.BeginTransaction();

            try
            {
                string query =
                    "INSERT INTO Club (Name, [Desc], CreatedAt, Land ) VALUES (@name, @desc, CONVERT(date, GETDATE()), @land); SELECT SCOPE_IDENTITY();";

                using (SqlCommand command = new SqlCommand(query, connection, transaction))
                {
                    command.Parameters.AddWithValue("@name", createClubDto.Name);
                    command.Parameters.AddWithValue("@desc", createClubDto.Desc);
                    command.Parameters.AddWithValue("@land", createClubDto.Land);

                    int clubId = Convert.ToInt32(command.ExecuteScalar());
                    transaction.Commit();
                    createClubDto.ClubId = clubId;
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
                throw new DalException("error while creating club", e);
            }
            finally
            {
                connection.Close();
            }
        }
        
    }
    
    public void RegisterUserToClub(ClubMembershipDTO clubMembershipDto)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            SqlTransaction transaction = connection.BeginTransaction();

            try
            {
                string query =
                    "INSERT INTO ClubUserRole (UserId, ClubId, RoleId) VALUES (@userId, @clubId, @roleId);";

                using (SqlCommand command = new SqlCommand(query, connection, transaction))
                {
                    command.Parameters.AddWithValue("@userId", clubMembershipDto.UserId);
                    command.Parameters.AddWithValue("@clubId", clubMembershipDto.ClubId);
                    command.Parameters.AddWithValue("@roleId", clubMembershipDto.RoleId);

                    command.ExecuteNonQuery();
                    transaction.Commit();
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
            catch (Exception e)
            {
                transaction.Rollback();
                throw new DalException("Error while joining club", e);
            }
            finally
            {
                connection.Close();
            }
        }
    }

    public List<ClubDTO> GetUserClub(int userId)
    {
        List<ClubDTO> clubs = new List<ClubDTO>();
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            string query = "SELECT Club.ClubId, Club.Name, Club.[Desc], Club.Land, Club.CreatedAt FROM Club JOIN ClubUserRole ON Club.ClubId = ClubUserRole.ClubId WHERE UserId = @userId;";
            try
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@userId", userId);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ClubDTO club = new ClubDTO()
                            {
                                ClubId = reader.GetInt32(reader.GetOrdinal("ClubId")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Desc = reader.GetString(reader.GetOrdinal("Desc")),
                                Land = reader.GetString(reader.GetOrdinal("Land")),
                                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"))

                            };
                            clubs.Add(club);
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
                throw new DalException("error while fetching clubs", e);
            }
            finally
            {
                connection.Close();
            }


    
        }
        return clubs;
        
        
    }

    public ClubDetailsDTO GetClubDetail(int clubId)
    {
        ClubDetailsDTO club = new ClubDetailsDTO();
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            string query = "SELECT Club.ClubId, Club.Name, Club.[Desc], Club.Land, Club.CreatedAt, ClubUserRole.UserId, ClubUserRole.RoleId FROM Club JOIN ClubUserRole ON Club.ClubId = ClubUserRole.ClubId WHERE Club.ClubId = @clubId;";
            try
            {
                club.Members = new List<ClubMembershipDTO>();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@clubId", clubId);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            club.ClubId = reader.GetInt32(reader.GetOrdinal("ClubId"));
                            club.Name = reader.GetString(reader.GetOrdinal("Name"));
                            club.Desc = reader.GetString(reader.GetOrdinal("Desc"));
                            club.Land = reader.GetString(reader.GetOrdinal("Land"));
                            club.CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"));
                            ClubMembershipDTO member = new ClubMembershipDTO()
                            {
                                ClubId = reader.GetInt32(reader.GetOrdinal("ClubId")),
                                UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                                RoleId = reader.GetInt32(reader.GetOrdinal("RoleId"))
                            };
                            club.Members.Add(member);
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
                throw new DalException("error while fetching clubs", e);
            }
            finally
            {
                connection.Close();
            }
        }
        return club;
        
        
    }
    
    public void UpdateMembershipRole(ClubMembershipDTO clubMembershipDto)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            SqlTransaction transaction = connection.BeginTransaction();

            try
            {
                string query =
                    "UPDATE ClubUserRole SET RoleId = @roleId WHERE UserId = @userId AND ClubId = @clubId;";

                using (SqlCommand command = new SqlCommand(query, connection, transaction))
                {
                    command.Parameters.AddWithValue("@userId", clubMembershipDto.UserId);
                    command.Parameters.AddWithValue("@clubId", clubMembershipDto.ClubId);
                    command.Parameters.AddWithValue("@roleId", clubMembershipDto.RoleId);

                    command.ExecuteNonQuery();
                    transaction.Commit();
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
                throw new DalException("Error while updating role", e);
            }
            finally
            {
                connection.Close();
            }
        }
    }
    
    

}
    
