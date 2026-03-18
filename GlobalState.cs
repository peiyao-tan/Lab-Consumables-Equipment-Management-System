// GlobalState.cs
using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace WindowsFormsApp1
{
    public static class GlobalState
    {
        public static string CurrentUserId { get; set; }
        public static string CurrentFullName { get; set; }

        private static HashSet<string> _userRoles = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// 登录成功后调用，加载用户角色
        /// </summary>
        public static void LoadUserRoles(string userId, string connectionString)
        {
            _userRoles.Clear();
            CurrentUserId = userId;

            try
            {
                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    // 加载 full_name
                    using (var cmd = new MySqlCommand("SELECT full_name FROM users WHERE id = @UserId", conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        CurrentFullName = cmd.ExecuteScalar()?.ToString() ?? "";
                    }

                    // 加载所有 role_id
                    using (var cmd = new MySqlCommand("SELECT role_id FROM user_roles WHERE user_id = @UserId", conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                _userRoles.Add(reader["role_id"].ToString());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"加载用户角色失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 判断当前用户是否为仓库管理员
        /// </summary>
        public static bool IsWarehouseAdmin
        {
            get
            {
                // ✅ 请根据你数据库中真实的 role_id 修改！
                return _userRoles.Contains("209fd9b5-9b78-11f0-963b-38f3ab79e8fa") ||
                       _userRoles.Contains("209fd44f-9b78-11f0-963b-38f3ab79e8fa") ||
                       _userRoles.Contains("209fd9b5-9b78-11f0-963b-38f3ab79e8fa");
            }
        }
    }
}