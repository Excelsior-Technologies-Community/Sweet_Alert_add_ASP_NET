using Sweet_Alert_add_ASP_NET.Models.Employee;
using System.Data;
using System.Data.SqlClient;

namespace Sweet_Alert_add_ASP_NET.Repository
{
    public class EmployeeRepository
    {
        private readonly string _connectionString;

        public EmployeeRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public List<EmployeeSave> Upsert(long EmployeeId, string Name, int Age, string City, string State, string ActionMode)
        {
            List<EmployeeSave> result = new List<EmployeeSave>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("SP_Employee_Save", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Id", EmployeeId);
                cmd.Parameters.AddWithValue("@Name", Name);
                cmd.Parameters.AddWithValue("@Age", Age);
                cmd.Parameters.AddWithValue("@City", City);
                cmd.Parameters.AddWithValue("@State", State);
                cmd.Parameters.AddWithValue("@Action", ActionMode);

                conn.Open();

                int rowsAffected = cmd.ExecuteNonQuery(); 

                // Return success status
                if (rowsAffected > 0)
                {
                    result.Add(new EmployeeSave
                    {
                        EmployeeId = EmployeeId.ToString(),
                        ReturnStatus = "1"
                    });
                }
                else
                {
                    result.Add(new EmployeeSave
                    {
                        EmployeeId = EmployeeId.ToString(),
                        ReturnStatus = "0"
                    });
                }
            }

            return result;
        }


        public bool EmployeeDelete(Int64 EmployeeId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_Employee_Delete", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", EmployeeId);

                    conn.Open();
                    cmd.ExecuteNonQuery();

                    return true;
                }
            }
        }

        public List<EmployeeList> EmployeeList()
        {
            List<EmployeeList> ulr = new List<EmployeeList>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_Employee_List", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ulr.Add(new EmployeeList
                            {
                                EmployeeId = Convert.ToInt64(reader["EmployeeID"]),
                                Name = reader["Name"].ToString(),
                                Age = reader["Age"].ToString(),
                                City = reader["City"].ToString(),
                                State = reader["State"].ToString()

                            });

                        }
                    }
                }

            }
            return ulr;
        }

        public EmployeeEdit EmployeeEdit(long EmployeeId)
        {
            EmployeeEdit emp = new EmployeeEdit();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_Employee_GetById", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;


                    cmd.Parameters.AddWithValue("@Id", EmployeeId);

                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            emp.EmployeeId = Convert.ToInt64(reader["EmployeeID"]);
                            emp.Name = reader["Name"].ToString();
                            emp.Age = reader["Age"].ToString();
                            emp.City = reader["City"].ToString();
                            emp.State = reader["State"].ToString();
                        }
                    }
                }
            }

            return emp;
        }

        public bool IsNameExists(string name, long employeeId = 0)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = "SELECT COUNT(*) FROM Employee WHERE Name = @Name";

                if (employeeId > 0)
                    query += " AND EmployeeID <> @EmployeeId";

                using (var cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Name", name);

                    if (employeeId > 0)
                        cmd.Parameters.AddWithValue("@EmployeeId", employeeId);

                    int count = (int)cmd.ExecuteScalar();
                    return count > 0;
                }
            }
        }
    }
}
