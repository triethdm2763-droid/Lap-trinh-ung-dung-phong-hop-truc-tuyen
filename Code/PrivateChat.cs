using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Network_Programming.Models;

namespace Network_Programming.Features
{
    /// <summary>
    /// Xử lý tính năng chat riêng tư (private chat)
    /// 
    /// Cách sử dụng:
    /// - Từ client hoặc host chat mode, gõ: private <tên_user> <tin_nhắn>
    /// - Ví dụ: private Alice Hello Alice!
    /// 
    /// Giao diện: Không có UI riêng, chỉ hiển thị tin nhắn với prefix [PRIVATE]
    /// Logic: Gửi tin nhắn chỉ cho người gửi và người nhận
    /// Cách vào: Gõ lệnh trực tiếp trong chat
    /// Cách thoát: Không cần thoát, chỉ gõ tin nhắn bình thường để quay lại chat công khai
    /// 
    /// Từ khóa khởi tạo: PRIVATE|<target>|<message>
    /// </summary>
    public static class PrivateChat
    {
        /// <summary>
        /// Gửi tin nhắn private cho user cụ thể
        /// </summary>
        /// <param name="sender">Người gửi</param>
        /// <param name="targetUsername">Tên user nhận</param>
        /// <param name="message">Nội dung tin nhắn</param>
        /// <param name="roomUsers">Danh sách user trong phòng</param>
        public static void SendPrivate(User sender, string targetUsername, string message, List<User> roomUsers)
        {
            var target = roomUsers.FirstOrDefault(u => u.Username == targetUsername);
            if (target == null) return;

            // Thêm thời gian Việt Nam
            TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime vietnamTime = TimeZoneInfo.ConvertTime(DateTime.Now, vietnamTimeZone);
            string timeStr = vietnamTime.ToString("HH:mm:ss");

            string formatted = $"{timeStr} : [PRIVATE] {sender.Username}: {message}";

            Send(target, formatted);
            Send(sender, formatted);
        }

        private static void Send(User user, string msg)
        {
            try
            {   
                if (user.Client == null)
                    return;
                var writer = new StreamWriter(user.Client.GetStream())
                {
                    AutoFlush = true
                };

                writer.WriteLine(msg);
            }
            catch { }
        }
    }
}
