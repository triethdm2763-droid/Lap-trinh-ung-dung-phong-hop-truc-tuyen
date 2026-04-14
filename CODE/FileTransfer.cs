using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace Network_Programming.Features
{
    public static class FileTransfer
    {
        /// <summary>
        /// Gửi file qua TCP connection
        /// </summary>
        /// <param name="client">TcpClient để gửi file</param>
        /// <param name="filePath">Đường dẫn file cần gửi</param>
        /// <param name="targetUsername">Username người nhận (null = public)</param>
        /// <param name="senderUsername">Username người gửi</param>
        public static void SendFile(TcpClient client, string filePath, string? targetUsername, string senderUsername)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    Console.WriteLine("File không tồn tại!");
                    return;
                }

                var fileInfo = new FileInfo(filePath);
                string fileName = fileInfo.Name;

                // Đọc file và encode base64
                byte[] fileBytes = File.ReadAllBytes(filePath);
                string base64Data = Convert.ToBase64String(fileBytes);

                // Gửi lệnh FILE với data base64
                var writer = new StreamWriter(client.GetStream()) { AutoFlush = true };
                string command = targetUsername == null
                    ? $"FILE|{fileName}|{base64Data}|{senderUsername}"
                    : $"FILE_PRIVATE|{targetUsername}|{fileName}|{base64Data}|{senderUsername}";

                writer.WriteLine(command);

                Console.WriteLine($"Đã gửi file: {fileName}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi gửi file: {ex.Message}");
            }
        }

        /// <summary>
        /// Nhận file từ TCP connection
        /// </summary>
        /// <param name="client">TcpClient để nhận file</param>
        /// <param name="fileName">Tên file</param>
        /// <param name="fileSize">Kích thước file</param>
        /// <param name="savePath">Đường dẫn lưu file</param>
        public static void ReceiveFile(TcpClient client, string fileName, string base64Data, string savePath)
        {
            try
            {
                // Decode base64
                byte[] fileBytes = Convert.FromBase64String(base64Data);

                // Tạo thư mục nếu chưa có
                Directory.CreateDirectory(Path.GetDirectoryName(savePath)!);

                // Ghi file
                File.WriteAllBytes(savePath, fileBytes);

                Console.WriteLine($"Đã nhận file: {fileName} -> {savePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi nhận file: {ex.Message}");
            }
        }
    }
}