using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace 实验耗材及设备物资管理系统
{
    public class DatabaseHelper : IDisposable
    {
        private string connectionString;
        private MySqlConnection currentConnection;
        private MySqlTransaction currentTransaction;
        private bool _disposed = false;

        // 只读连接字符串属性（代码二独有）
        public string ConnectionString => connectionString;

        public DatabaseHelper(string connectionString)
        {
            this.connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        #region 事务相关方法
        public void BeginTransaction()
        {
            if (currentConnection != null && currentConnection.State == ConnectionState.Open)
            {
                throw new InvalidOperationException("事务已经存在");
            }

            currentConnection = new MySqlConnection(connectionString);
            currentConnection.Open();
            currentTransaction = currentConnection.BeginTransaction();
        }

        public void CommitTransaction()
        {
            if (currentTransaction == null)
            {
                throw new InvalidOperationException("没有活动的事务可以提交");
            }

            currentTransaction.Commit();
            CleanupTransaction();
        }

        public void RollbackTransaction()
        {
            if (currentTransaction == null)
            {
                throw new InvalidOperationException("没有活动的事务可以回滚");
            }

            currentTransaction.Rollback();
            CleanupTransaction();
        }

        private void CleanupTransaction()
        {
            if (currentTransaction != null)
            {
                currentTransaction.Dispose();
                currentTransaction = null;
            }

            if (currentConnection != null)
            {
                currentConnection.Close();
                currentConnection.Dispose();
                currentConnection = null;
            }
        }
        #endregion

        #region 事务内执行方法
        public DataTable ExecuteQueryInTransaction(string query, params MySqlParameter[] parameters)
        {
            if (currentTransaction == null)
                throw new InvalidOperationException("没有活动的事务");

            using (var cmd = new MySqlCommand(query, currentConnection, currentTransaction))
            {
                if (parameters != null && parameters.Length > 0)
                    cmd.Parameters.AddRange(parameters);

                using (var adapter = new MySqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    return dt;
                }
            }
        }

        public int ExecuteNonQueryInTransaction(string query, params MySqlParameter[] parameters)
        {
            if (currentTransaction == null)
                throw new InvalidOperationException("没有活动的事务");

            using (var cmd = new MySqlCommand(query, currentConnection, currentTransaction))
            {
                if (parameters != null && parameters.Length > 0)
                    cmd.Parameters.AddRange(parameters);

                return cmd.ExecuteNonQuery();
            }
        }

        public object ExecuteScalarInTransaction(string query, params MySqlParameter[] parameters)
        {
            if (currentTransaction == null)
                throw new InvalidOperationException("没有活动的事务");

            using (var cmd = new MySqlCommand(query, currentConnection, currentTransaction))
            {
                if (parameters != null && parameters.Length > 0)
                    cmd.Parameters.AddRange(parameters);

                return cmd.ExecuteScalar();
            }
        }
        #endregion

        #region 普通执行方法（MySqlParameter数组）
        public DataTable ExecuteQuery(string query, params MySqlParameter[] parameters)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new MySqlCommand(query, conn))
                {
                    if (parameters != null && parameters.Length > 0)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }

                    using (var adapter = new MySqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        return dt;
                    }
                }
            }
        }

        public object ExecuteScalar(string query, params MySqlParameter[] parameters)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new MySqlCommand(query, conn))
                {
                    if (parameters != null && parameters.Length > 0)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }

                    return cmd.ExecuteScalar();
                }
            }
        }

        public int ExecuteNonQuery(string query, params MySqlParameter[] parameters)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new MySqlCommand(query, conn))
                {
                    if (parameters != null && parameters.Length > 0)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }

                    return cmd.ExecuteNonQuery();
                }
            }
        }
        #endregion

        #region 普通执行方法（字典参数）
        public DataTable ExecuteQuery(string query, Dictionary<string, object> parameters = null)
        {
            var paramArray = parameters?.Select(kvp => new MySqlParameter(kvp.Key, kvp.Value)).ToArray();
            return ExecuteQuery(query, paramArray);
        }

        public object ExecuteScalar(string query, Dictionary<string, object> parameters = null)
        {
            var paramArray = parameters?.Select(kvp => new MySqlParameter(kvp.Key, kvp.Value)).ToArray();
            return ExecuteScalar(query, paramArray);
        }

        public int ExecuteNonQuery(string query, Dictionary<string, object> parameters = null)
        {
            var paramArray = parameters?.Select(kvp => new MySqlParameter(kvp.Key, kvp.Value)).ToArray();
            return ExecuteNonQuery(query, paramArray);
        }
        #endregion

        #region 批量事务执行方法
        public bool ExecuteTransaction(List<(string query, Dictionary<string, object> parameters)> commands)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        foreach (var (query, parameters) in commands)
                        {
                            using (var cmd = new MySqlCommand(query, conn, transaction))
                            {
                                AddParameters(cmd, parameters); // 使用代码二的参数添加方法
                                cmd.ExecuteNonQuery();
                            }
                        }
                        transaction.Commit();
                        return true;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public int ExecuteNonQuery(string query, Dictionary<string, object> parameters, MySqlTransaction transaction)
        {
            var paramArray = parameters?.Select(kvp => new MySqlParameter(kvp.Key, kvp.Value)).ToArray();
            return ExecuteNonQuery(query, paramArray, transaction);
        }

        public int ExecuteNonQuery(string query, MySqlParameter[] parameters, MySqlTransaction transaction)
        {
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));

            using (var cmd = new MySqlCommand(query, transaction.Connection, transaction))
            {
                if (parameters != null && parameters.Length > 0)
                {
                    cmd.Parameters.AddRange(parameters);
                }
                return cmd.ExecuteNonQuery();
            }
        }

        public bool ExecuteTransaction(List<(string query, MySqlParameter[] parameters)> commands)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        foreach (var (query, parameters) in commands)
                        {
                            using (var cmd = new MySqlCommand(query, conn, transaction))
                            {
                                if (parameters != null && parameters.Length > 0)
                                {
                                    cmd.Parameters.AddRange(parameters);
                                }
                                cmd.ExecuteNonQuery();
                            }
                        }
                        transaction.Commit();
                        return true;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
        #endregion

        #region 代码二独有方法
        // 辅助方法：添加参数（处理GUID和DBNull，代码二独有）
        private void AddParameters(MySqlCommand cmd, Dictionary<string, object> parameters)
        {
            if (parameters == null) return;

            foreach (var p in parameters)
            {
                // 对于 GUID 参数，确保转换为字符串
                if (p.Key == "@labId" || p.Key == "@userId")
                {
                    if (p.Value is Guid guidValue)
                    {
                        cmd.Parameters.AddWithValue(p.Key, guidValue.ToString());
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue(p.Key, p.Value != null ? p.Value : DBNull.Value);
                    }
                }
                else
                {
                    cmd.Parameters.AddWithValue(p.Key, p.Value != null ? p.Value : DBNull.Value);
                }
            }
        }

        // 优化GetConnection方法：确保连接打开后返回（代码二独有）
        public MySqlConnection GetConnection()
        {
            if (currentConnection == null)
            {
                currentConnection = new MySqlConnection(connectionString);
            }
            if (currentConnection.State != ConnectionState.Open)
            {
                currentConnection.Open();
            }
            return currentConnection;
        }

        // 完善测试连接方法：添加异常输出（代码二独有）
        public bool TestConnection()
        {
            try
            {
                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"数据库连接测试失败: {ex.Message}");
                return false;
            }
        }
        #endregion

        #region IDisposable接口实现（代码二独有）
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // 释放托管资源
                    currentConnection?.Dispose();
                    currentTransaction?.Dispose();
                }
                _disposed = true;
            }
        }

        // 析构函数：防止未手动调用Dispose
        ~DatabaseHelper()
        {
            Dispose(false);
        }
        #endregion
    }
}