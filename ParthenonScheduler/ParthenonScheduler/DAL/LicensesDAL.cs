using ParthenonScheduler.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace ParthenonScheduler.DAL
{
    class LicensesDAL : Base_DAL<Licenses>
    {
        public List<Licenses> GetExpiringLicensesByCompanyId(int id)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    string query = "SELECT [id],[type_id],[oem_id],[user_id],[machine_id],[machine_serial],[location_id],[pc]," +
                        "[comment],[start_date],[end_date],[create_date],[serial_number] FROM [dbo].[licenses]" +
                        " WHERE [end_date] > CURRENT_TIMESTAMP AND [end_date] < CAST('" + _expLicensesSpan + "' AS DATE)";

                    if (id != 5)
                        query += "AND [eu_id] = " + id;

                    Trace.TraceInformation($"Sql query for expiring licenses {query}");
                    SqlCommand cmd = new SqlCommand(query, con);
                    con.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();

                    return PopulateModel(rdr).Cast<Licenses>().ToList();
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError($"Exception during database connection {ex.Message}");
            }

            return new List<Licenses>();
        }
    }
}

