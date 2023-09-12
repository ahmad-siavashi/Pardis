using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlServerCe;
using Pardis_Monetary_Fund.DataModels;
using Pardis_Monetary_Fund.Utilities;
using System.Collections.ObjectModel;
using System.Windows;
using NLog;

namespace Pardis_Monetary_Fund.InformationLayer
{
    public static partial class DatabaseAccess
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public static string DatabaseFileName;
        public static string DatabaseConnectionString;
        // Revised.

        static DatabaseAccess()
        {
            DatabaseFileName = "Database.sdf";
            DatabaseConnectionString = string.Format(@"Data Source = {0}", DatabaseFileName);
        }

        public static int Insert(String TableName, String PrimaryKey, object item, List<String> IgnoredColumns)
        {
            SqlCeConnection conn = new SqlCeConnection(DatabaseConnectionString);
            if (IgnoredColumns == null)
            {
                IgnoredColumns = new List<string>();
            }
            try
            {
                conn.Open();
                String query = "Insert Into " + TableName + " ( ";
                String values = "Values(";
                foreach (System.Reflection.PropertyInfo p in item.GetType().GetProperties())
                {
                    if (!IgnoredColumns.Contains(p.Name) && !String.Equals(p.Name, PrimaryKey, StringComparison.OrdinalIgnoreCase))
                    {
                        query += p.Name + ", ";
                        values += "@" + p.Name + ", ";
                    }
                }
                query = query.Substring(0, query.LastIndexOf(",")) + ")";
                values = values.Substring(0, values.LastIndexOf(",")) + ")";
                query += " " + values;
                SqlCeCommand cmd = new SqlCeCommand(query, conn);
                foreach (System.Reflection.PropertyInfo p in item.GetType().GetProperties())
                {
                    if (!IgnoredColumns.Contains(p.Name) && !String.Equals(p.Name, PrimaryKey, StringComparison.OrdinalIgnoreCase))
                    {
                        cmd.Parameters.AddWithValue("@" + p.Name, p.GetValue(item));
                    }
                }
                cmd.ExecuteNonQuery();
                cmd.CommandText = "SELECT @@IDENTITY";
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Stop, MessageBoxResult.OK, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                logger.Error(ex.InnerException, "Message: {0}, StackTrace: {1}, TargetSite: {2}, Data: {3}, Source: {4}", ex.Message, ex.StackTrace, ex.TargetSite, ex.Data, ex.Source);
                Debug.DebugBreak();
                return -1;
            }
            finally
            {
                conn.Close();
            }
        }

        // Revised.
        public static bool DeleteAll(String TableName, String PrimaryKey, List<object> objects)
        {
            try
            {
                foreach (var o in objects)
                {
                    Delete(TableName, PrimaryKey, o.GetType().GetProperty(PrimaryKey.First().ToString().ToUpper() + PrimaryKey.Substring(1)).GetValue(o));
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Stop, MessageBoxResult.OK, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                logger.Error(ex.InnerException, "Message: {0}, StackTrace: {1}, TargetSite: {2}, Data: {3}, Source: {4}", ex.Message, ex.StackTrace, ex.TargetSite, ex.Data, ex.Source);
                return false;
            }
        }

        // Revised.
        public static bool Delete(String TableName, String Field, object Condition)
        {
            SqlCeConnection conn = new SqlCeConnection(DatabaseConnectionString);
            try
            {
                conn.Open();
                String query = "Delete From " + TableName;
                query += " where " + Field + " = @condition";
                SqlCeCommand cmd = new SqlCeCommand(query, conn);
                cmd.Parameters.AddWithValue("@condition", Condition);
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Stop, MessageBoxResult.OK, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                logger.Error(ex.InnerException, "Message: {0}, StackTrace: {1}, TargetSite: {2}, Data: {3}, Source: {4}", ex.Message, ex.StackTrace, ex.TargetSite, ex.Data, ex.Source);
                return false;
            }
            finally
            {
                conn.Close();
            }
        }


        // Revised.
        public static DataTable Retrieve(String TableName, String PrimaryKey, int PK)
        {
            SqlCeConnection conn = new SqlCeConnection(DatabaseConnectionString);
            try
            {
                string query = "Select * from " + TableName + " where " + PrimaryKey + " = " + PK;
                return ExecuteQuery(query);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Stop, MessageBoxResult.OK, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                logger.Error(ex.InnerException, "Message: {0}, StackTrace: {1}, TargetSite: {2}, Data: {3}, Source: {4}", ex.Message, ex.StackTrace, ex.TargetSite, ex.Data, ex.Source);
                return null;
            }
        }

        // Revised.
        public static bool Update(String TableName, String PrimaryKey, int PK, object item, List<String> IgnoredColumns)
        {
            SqlCeConnection conn = new SqlCeConnection(DatabaseConnectionString);
            try
            {
                conn.Open();
                String query = "Update " + TableName + " Set ";
                List<SqlCeParameter> parameters = new List<SqlCeParameter>();
                foreach (System.Reflection.PropertyInfo p in item.GetType().GetProperties())
                {
                    if (!IgnoredColumns.Contains(p.Name) && !String.Equals(p.Name, PrimaryKey, StringComparison.OrdinalIgnoreCase))
                    {
                        query += p.Name + " = @" + p.Name + ", ";
                        parameters.Add(new SqlCeParameter("@" + p.Name, p.GetValue(item)));
                    }
                }
                query = query.Substring(0, query.LastIndexOf(","));
                query += " where " + PrimaryKey + " = " + PK;
                SqlCeCommand cmd = new SqlCeCommand(query, conn);
                foreach (var p in parameters)
                    cmd.Parameters.Add(p);
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Stop, MessageBoxResult.OK, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                logger.Error(ex.InnerException, "Message: {0}, StackTrace: {1}, TargetSite: {2}, Data: {3}, Source: {4}", ex.Message, ex.StackTrace, ex.TargetSite, ex.Data, ex.Source);
                return false;
            }
            finally
            {
                conn.Close();
            }
        }
    }
}
