using BMWDALInterfacesAndDTOs.DTOs;
using BMWDALInterfacesAndDTOs.Interfaces;
using BMWDALInterfacesAndDTOs.Exceptions;
using System.Data.SqlClient;
using BMWDomain.Entities;
using InviteCodeException = BMWDALInterfacesAndDTOs.Exceptions.InviteCodeException;

namespace BMW.Dal;

public class InviteCodeRepository : IInviteCodeRepository
{
    private readonly string _connectionString;

    public InviteCodeRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

public InviteCodeDTO GetInviteCodeByCode(string inviteCode)
{
    using (SqlConnection connection = new SqlConnection(_connectionString))
    {
        connection.Open();
        string query =
            "SELECT Code, ValidUntil, MaxUses, ClubId FROM InviteCode WHERE Code = @inviteCode AND ValidUntil > GETDATE() AND MaxUses > 0";

        try
        {
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@inviteCode", inviteCode);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string code = reader.GetString(reader.GetOrdinal("Code"));
                        DateTime expirationDate = reader.GetDateTime(reader.GetOrdinal("ValidUntil"));
                        int maxUses = reader.GetInt32(reader.GetOrdinal("MaxUses"));
                        int clubId = reader.GetInt32(reader.GetOrdinal("ClubId"));

                        // Close the reader before executing the update query
                        reader.Close();

                        // Decrease the max uses count by 1 in the database
                        string updateQuery = "UPDATE InviteCode SET MaxUses = MaxUses - 1 WHERE Code = @inviteCode";
                        using (SqlCommand updateCommand = new SqlCommand(updateQuery, connection))
                        {
                            updateCommand.Parameters.AddWithValue("@inviteCode", inviteCode);
                            updateCommand.ExecuteNonQuery();
                        }
                        
                        reader.Close();
                        
                        if (maxUses - 1 <= 0)
                        {
                            // If the max uses count is 0, delete the invite code
                            string deleteQuery = "DELETE FROM InviteCode WHERE Code = @inviteCode";
                            using (SqlCommand deleteCommand = new SqlCommand(deleteQuery, connection))
                            {
                                deleteCommand.Parameters.AddWithValue("@inviteCode", inviteCode);
                                deleteCommand.ExecuteNonQuery();
                            }
                        }

                        

                        // Return the InviteCodeDTO
                        return new InviteCodeDTO
                        {
                            Code = code,
                            ExpirationDate = expirationDate,
                            MaxUses = maxUses - 1, // Decrease max uses by 1 as it's being used now
                            ClubId = clubId
                        };
                    }
                    else
                    {
                        // If the reader doesn't read anything, the invite code is invalid
                        throw new InviteCodeException("Invalid invite code");
                    
                    }
                }
            }
        }
        catch (Exception e)
        {
            throw new InviteCodeException("Error in InviteCodeRepository", e);   
        }
        
    }
}




    public void CreateInviteCode(InviteCodeDTO inviteCode)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            SqlTransaction transaction = connection.BeginTransaction();
            string query =
                "INSERT INTO InviteCode (ValidUntil, MaxUses, ClubId, UserId) OUTPUT INSERTED.InviteCodeId, INSERTED.Code VALUES (@ValidUntil, @MaxUses, @ClubId, @UserId)";

            try
            {
                using (SqlCommand command = new SqlCommand(query, connection, transaction))
                {
                    command.Parameters.AddWithValue("@ValidUntil", inviteCode.ExpirationDate);
                    command.Parameters.AddWithValue("@MaxUses", inviteCode.MaxUses);
                    command.Parameters.AddWithValue("@ClubId", inviteCode.ClubId);
                    command.Parameters.AddWithValue("@UserId", inviteCode.UserId);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            inviteCode.Id = reader.GetInt32(0); // Assuming Id is integer
                            inviteCode.Code = reader.GetString(1); // Assuming Code is string
                        }
                    }

                    transaction.Commit();
                }
            }
            catch (SqlException ex) when (ex.Number == 53 || (ex.InnerException is System.ComponentModel.Win32Exception && ((System.ComponentModel.Win32Exception)ex.InnerException).NativeErrorCode == 53))
            {
                // Handle network issues specifically
                transaction.Rollback();
                throw new NetworkException("Network-related error occurred while connecting to the database.", ex);
            }
            catch (Exception e)
            {
                transaction.Rollback();
                throw new DalException("Error while creating invite code", e);
            }
            finally
            {
                connection.Close();
            }
        }
    }
    
    public List<InviteCodeDTO> GetInviteCodesByClubId(int clubId)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            string query = "SELECT Code, ValidUntil, MaxUses, ClubId, UserId, InviteCodeId  FROM InviteCode WHERE ClubId = @clubId";

            try
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@clubId", clubId);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        List<InviteCodeDTO> inviteCodes = new List<InviteCodeDTO>();
                        while (reader.Read())
                        {
                            inviteCodes.Add(new InviteCodeDTO
                            {
                                Code = reader.GetString(reader.GetOrdinal("Code")),
                                ExpirationDate = reader.GetDateTime(reader.GetOrdinal("ValidUntil")),
                                MaxUses = reader.GetInt32(reader.GetOrdinal("MaxUses")),
                                ClubId = reader.GetInt32(reader.GetOrdinal("ClubId")),
                                UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                                Id = reader.GetInt32(reader.GetOrdinal("InviteCodeId"))
                            });
                        }

                        return inviteCodes;
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
                throw new DalException("Error while fetching invite codes", e);
            }
        }
    }

}

