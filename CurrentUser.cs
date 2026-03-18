using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 实验耗材及设备物资管理系统
{
    public static class CurrentUser
    {
        public static string Id { get; private set; }      // 数据库中的用户ID（如：U001 或 整数ID）
        public static string Username { get; private set; } // 用户名

        /// <summary>
        /// 登录时设置当前用户信息
        /// </summary>
        public static void Login(string id, string username)
        {
            Id = id;
            Username = username;
        }

        /// <summary>
        /// 判断是否已登录
        /// </summary>
        public static bool IsLoggedIn => Id != null;

        /// <summary>
        /// 获取当前用户ID（可用于 actorId）
        /// </summary>
        public static string GetId()
        {
            return Id ?? "Unknown";
        }

        /// <summary>
        /// 获取当前用户名
        /// </summary>
        public static string GetUsername()
        {
            return Username ?? "Unknown";
        }
    }
}