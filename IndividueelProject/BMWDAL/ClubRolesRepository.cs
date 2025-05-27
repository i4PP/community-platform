using BMWDALInterfacesAndDTOs.DTOs;
using BMWDALInterfacesAndDTOs.Interfaces;
using BMWDALInterfacesAndDTOs.Exceptions;
using System.Data.SqlClient;

namespace BMW.Dal;

public class ClubRolesRepository : IClubRolesRepository
{
    private readonly string _connectionString;

    public ClubRolesRepository(string connectionString)
    {
        _connectionString = connectionString;
    }
    
    public ClubRoleDTO GetRoleById(int roleId)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            string query = "SELECT RoleId, Name  FROM ClubRoles WHERE RoleId = @roleId";
            try
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@roleId", roleId);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new ClubRoleDTO
                            {
                                RoleId = reader.GetInt32(reader.GetOrdinal("RoleId")),
                                RoleName = reader.GetString(reader.GetOrdinal("Name"))

                            };
                        }
                        else
                        {
                            throw new DalException("Role not found");
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
                throw new DalException("Error in ClubRolesRepository", e);
            }


        }
    }

    public List<ClubRoleDTO> GetRoles()
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            string query = "SELECT RoleId, Name  FROM ClubRoles";
            try
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        List<ClubRoleDTO> roles = new List<ClubRoleDTO>();
                        while (reader.Read())
                        {
                            roles.Add(new ClubRoleDTO
                            {
                                RoleId = reader.GetInt32(reader.GetOrdinal("RoleId")),
                                RoleName = reader.GetString(reader.GetOrdinal("Name"))

                            });
                        }

                        return roles;
                    }
                }
            }
            catch (SqlException ex) when (ex.Number == 53 ||
                                          (ex.InnerException is System.ComponentModel.Win32Exception &&
                                           ((System.ComponentModel.Win32Exception)ex.InnerException).NativeErrorCode ==
                                           53))
            {
                // Handle network issues specifically
                throw new NetworkException("Network-related error occurred while connecting to the database.", ex);
            }
            catch (SqlException e)
            {
                throw new DalException("Error in ClubRolesRepository", e);
            }
        }
    }
}

