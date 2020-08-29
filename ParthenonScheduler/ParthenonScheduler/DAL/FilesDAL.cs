using ParthenonScheduler.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace ParthenonScheduler.DAL
{
    class FilesDAL : Base_DAL<Files>
    {
        public List<Files> GetRecentFiles(DateTimeOffset? last)
        {
            try
            {
                const int _newFilesDays = -30;
                string sinceDate = DateTime.Now.AddDays(_newFilesDays).ToShortDateString();
                if (last.HasValue)
                {
                    sinceDate = last.Value.DateTime.ToShortDateString();
                }

                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    string query = "SELECT [file_name],[comment],[create_date] FROM [dbo].[files]" +
                        " WHERE [create_date] > CAST('" + sinceDate + "' AS DATE)";

                    Trace.TraceInformation($"Using sql {query}");
                    SqlCommand cmd = new SqlCommand(query, con);
                    con.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();

                    return PopulateModel(rdr).Cast<Files>().ToList();
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError($"Exception during get recent files {ex.Message}");
            }

            return new List<Files>();
        }
    }
}
