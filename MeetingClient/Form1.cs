using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MeetingClient
{
    public partial class Form1 : Form
    {
        ChatClient client = new ChatClient();
        string username;

        public Form1()
        {
            InitializeComponent();
            client.OnMessageReceived = DisplayMessage;
        }

        // JOIN ROOM
        void JoinRoom()
        {
            username = txtName.Text;
            string ip = txtIP.Text;

            client.ConnectToServer(ip, 8888);
            client.SendMessage("JOIN:" + username);
        }

        // SEND MESSAGE
        void SendMessage()
        {
            string msg = txtMessage.Text;

            client.SendMessage("MSG:" + username + ": " + msg);
            txtMessage.Clear();
        }

        // HIỂN THỊ TIN NHẮN
        void DisplayMessage(string msg)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(DisplayMessage), msg);
                return;
            }

            if (msg.StartsWith("USERS:"))
            {
                UpdateUserList(msg.Substring(6));
            }
            else
            {
                txtChat.AppendText(msg + Environment.NewLine);
            }
        }

        // UPDATE USER LIST
        void UpdateUserList(string users)
        {
            lstUsers.Items.Clear();

            string[] list = users.Split(',');

            foreach (var u in list)
            {
                lstUsers.Items.Add(u);
            }
        }

        // BUTTON CLICK
        private void btnJoin_Click(object sender, EventArgs e)
        {
            JoinRoom();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            SendMessage();
        }

        // FIX lỗi Designer
        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}