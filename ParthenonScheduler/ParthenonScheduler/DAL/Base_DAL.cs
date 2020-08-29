using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Reflection;

namespace ParthenonScheduler.DAL
{
    class Base_DAL<T>
    {
        #region Members
        // Connection string
        protected readonly string _connectionString = ConfigurationManager.AppSettings["connectionString"];

        // Threshold, in days, to check for data. Future: get as a custom value per User from DB.
        public static readonly int _expLicensesDays = 30;
        
        // Threshold date objects
        protected readonly string _expLicensesSpan = DateTime.Now.AddDays(_expLicensesDays).ToShortDateString();
        #endregion

        #region Model Methods
        public List<object> PopulateModel(SqlDataReader rdr)
        {
            List<object> modelList = new List<object>();
            var type = typeof(T);
            var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            while (rdr.Read())
            {
                var obj = Activator.CreateInstance(type);

                for (int i = 0; i < props.Length; i++)
                {
                    if (!Convert.IsDBNull(rdr[i]))
                    {
                        var value = Convert.ChangeType(rdr[i], props[i].PropertyType);
                        props[i].SetValue(obj, value);
                    }
                }

                modelList.Add(obj);
            }

            return modelList;
        }
        #endregion
    }
}
