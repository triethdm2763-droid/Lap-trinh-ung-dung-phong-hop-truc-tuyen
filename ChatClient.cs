using System;
using System.Windows.Forms;

namespace MeetingClient
{
    public partial class Form1 : Form
    {
        ChatClient client = new ChatClient();

        public Form1()
        {
            InitializeComponent();

            client.OnMessageReceived = DisplayMessage;
        }

        private void btnJoin_Click(object sender, EventArgs e)
        {
            client.ConnectToServer("127.0.0.1", 5000);

            string name = txtName.Text;
            string room = txtRoom.Text;

            client.SendMessage($"JOIN|{name}|{room}");

            MessageBox.Show("Joined!");
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            client.SendMessage(txtMessage.Text);
            txtMessage.Clear();
        }

        void DisplayMessage(string msg)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(DisplayMessage), msg);
                return;
            }

            rtbChat.AppendText(msg + "\n");
        }
    }
}