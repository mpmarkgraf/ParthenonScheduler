using ParthenonScheduler.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text;

namespace ParthenonScheduler.DAL
{
    class UsersDAL : Base_DAL<Users>
    {
        public List<Users> GetSubscribers(string subType)
        {
            List<Users> subscribers = new List<Users>();

            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    string query = "SELECT [email],[company_id] FROM [dbo].[users] WHERE " + subType;

                    SqlCommand cmd = new SqlCommand(query, con);
                    con.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();

                    subscribers = PopulateUsers(rdr);
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError($"Exception during database connection {ex.Message}");
            }

            return subscribers;
        }

        private List<Users> PopulateUsers(SqlDataReader rdr)
        {
            List<Users> users = new List<Users>();

            while (rdr.Read())
            {
                users.Add(
                    new Users()
                    {
                        Email = rdr[0].ToString(),
                        CompanyId = Convert.ToInt32(rdr[1])
                    }
                );
            }

            return users;
        }
    }
}
