using MySql.Data.MySqlClient;
using System;

namespace 实验耗材及设备物资管理系统
{
    public class LogAudit
    {
        private string _connectionString;

        public LogAudit(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// 获取客户端IP地址
        /// </summary>
        private string GetClientIPAddress()
        {
            try
            {
                string hostName = System.Net.Dns.GetHostName();
                var addresses = System.Net.Dns.GetHostAddresses(hostName);

                foreach (var ip in addresses)
                {
                    if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        return ip.ToString();
                    }
                }

                // 如果没有找到IPv4地址，返回本地回环地址
                return "127.0.0.1";
            }
            catch (Exception)
            {
                return "Unknown";
            }
        }

        /// <summary>
        /// 记录审计日志
        /// </summary>
        public void LogAction(string actorId, string action, string objectType, string objectId)
        {
            try
            {
                string query = @"
                    INSERT INTO audit_logs (id, actor_id, action, object_type, object_id, timestamp, ip)
                    VALUES (@id, @actorId, @action, @objectType, @objectId, NOW(), @ip)";

                using (MySqlConnection conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", Guid.NewGuid().ToString());
                    cmd.Parameters.AddWithValue("@actorId", actorId);
                    cmd.Parameters.AddWithValue("@action", action);
                    cmd.Parameters.AddWithValue("@objectType", objectType);
                    cmd.Parameters.AddWithValue("@objectId", objectId);
                    cmd.Parameters.AddWithValue("@ip", GetClientIPAddress());

                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                // 审计日志记录失败不应该影响主业务流程
                System.Diagnostics.Debug.WriteLine($"审计日志记录失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 重载方法：记录审计日志（包含前后数据快照）
        /// </summary>
        public void LogAction(string actorId, string action, string objectType, string objectId,
                            string beforeJson, string afterJson)
        {
            try
            {
                string query = @"
                    INSERT INTO audit_logs (id, actor_id, action, object_type, object_id, timestamp, before_json, after_json, ip)
                    VALUES (@id, @actorId, @action, @objectType, @objectId, NOW(), @beforeJson, @afterJson, @ip)";

                using (MySqlConnection conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", Guid.NewGuid().ToString());
                    cmd.Parameters.AddWithValue("@actorId", actorId);
                    cmd.Parameters.AddWithValue("@action", action);
                    cmd.Parameters.AddWithValue("@objectType", objectType);
                    cmd.Parameters.AddWithValue("@objectId", objectId);
                    cmd.Parameters.AddWithValue("@beforeJson", string.IsNullOrEmpty(beforeJson) ? (object)DBNull.Value : beforeJson);
                    cmd.Parameters.AddWithValue("@afterJson", string.IsNullOrEmpty(afterJson) ? (object)DBNull.Value : afterJson);
                    cmd.Parameters.AddWithValue("@ip", GetClientIPAddress());

                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                // 审计日志记录失败不应该影响主业务流程
                System.Diagnostics.Debug.WriteLine($"审计日志记录失败: {ex.Message}");
            }
        }
    }
}