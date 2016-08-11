using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Data.SqlServerCe;
using System.Data;
using System.Reflection;
using Pardis_Monetary_Fund.Utilities;

namespace Pardis_Monetary_Fund.InformationLayer
{
    public static partial class DatabaseAccess
    {
        // Revised.
        public static T Get<T>(String TableName, String PrimaryKeyColumn, object PK) where T : class, new()
        {
            SqlCeConnection conn = null;
            try
            {
                T item = new T();
                Type type = typeof(T);
                var query = "Select * from " + TableName + " where " + PrimaryKeyColumn + " = " + "@PK";
                SqlCeCommand cmd = new SqlCeCommand(query);
                cmd.Parameters.AddWithValue("@PK", PK);
                SqlCeDataReader reader = ExecuteReader(cmd, ref conn);
                if (reader.Read())
                {
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        var column = reader.GetName(i);
                        var property = column.Substring(0, 1).ToUpper() + column.Substring(1);
                        Type propertyType = type.GetProperty(property).GetType();
                        type.GetProperty(property).SetValue(item, (reader[column] is DBNull) ? (propertyType.IsValueType ? Activator.CreateInstance(propertyType) : null) : reader[column]);
                    }
                    return item;
                }
                return default(T);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Stop, MessageBoxResult.OK, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                logger.Error(ex.InnerException, "Message: {0}, StackTrace: {1}, TargetSite: {2}, Data: {3}, Source: {4}", ex.Message, ex.StackTrace, ex.TargetSite, ex.Data, ex.Source);
                Debug.DebugBreak();
                return default(T);
            }
            finally
            {
                conn.Close();
            }
        }

        public static DataTable ExecuteQuery(String query)
        {
            SqlCeConnection conn = new SqlCeConnection(DatabaseConnectionString);
            try
            {
                SqlCeCommand cmd = new SqlCeCommand(query, conn);
                SqlCeDataAdapter dataAdapter = new SqlCeDataAdapter(cmd);
                DataTable dt = new DataTable();
                dataAdapter.Fill(dt);
                return dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Stop, MessageBoxResult.OK, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                logger.Error(ex.InnerException, "Message: {0}, StackTrace: {1}, TargetSite: {2}, Data: {3}, Source: {4}", ex.Message, ex.StackTrace, ex.TargetSite, ex.Data, ex.Source);
                Debug.DebugBreak();
                return null;
            }
            finally
            {
                conn.Close();
            }
        }

        public static bool ExecuteNonQuery(String query)
        {
            SqlCeConnection conn = new SqlCeConnection(DatabaseConnectionString);
            try
            {
                SqlCeCommand cmd = new SqlCeCommand(query, conn);
                conn.Open();
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Stop, MessageBoxResult.OK, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                logger.Error(ex.InnerException, "Message: {0}, StackTrace: {1}, TargetSite: {2}, Data: {3}, Source: {4}", ex.Message, ex.StackTrace, ex.TargetSite, ex.Data, ex.Source);
                Debug.DebugBreak();
                return false;
            }
            finally
            {
                conn.Close();
            }
        }

        public static object ExecuteScalar(String query)
        {
            SqlCeConnection conn = new SqlCeConnection(DatabaseConnectionString);
            try
            {
                SqlCeCommand cmd = new SqlCeCommand(query, conn);
                conn.Open();
                return cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Stop, MessageBoxResult.OK, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                logger.Error(ex.InnerException, "Message: {0}, StackTrace: {1}, TargetSite: {2}, Data: {3}, Source: {4}", ex.Message, ex.StackTrace, ex.TargetSite, ex.Data, ex.Source);
                Debug.DebugBreak();
                return null;
            }
            finally
            {
                conn.Close();
            }
        }

        public static SqlCeDataReader ExecuteReader(String query, ref SqlCeConnection conn)
        {
            conn = new SqlCeConnection(DatabaseConnectionString);
            try
            {
                SqlCeCommand cmd = new SqlCeCommand(query, conn);
                conn.Open();
                return cmd.ExecuteReader();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Stop, MessageBoxResult.OK, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                logger.Error(ex.InnerException, "Message: {0}, StackTrace: {1}, TargetSite: {2}, Data: {3}, Source: {4}", ex.Message, ex.StackTrace, ex.TargetSite, ex.Data, ex.Source);
                Debug.DebugBreak();
                return null;
            }
        }

        public static SqlCeDataReader ExecuteReader(SqlCeCommand cmd, ref SqlCeConnection conn)
        {
            conn = new SqlCeConnection(DatabaseConnectionString);
            try
            {
                cmd.Connection = conn;
                conn.Open();
                return cmd.ExecuteReader();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Stop, MessageBoxResult.OK, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                logger.Error(ex.InnerException, "Message: {0}, StackTrace: {1}, TargetSite: {2}, Data: {3}, Source: {4}", ex.Message, ex.StackTrace, ex.TargetSite, ex.Data, ex.Source);
                Debug.DebugBreak();
                return null;
            }
        }
    }
}
