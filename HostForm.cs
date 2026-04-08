using System;
using System.Drawing;
using System.Windows.Forms;

namespace MeetingRoomGUI
{
    public class HostForm : Form
    {
        Label lblTitle, lblRoomID, lblPassword, lblUsers, lblLink, lblParticipants, lblChat;
        TextBox txtRoomID, txtPassword, txtMessage;
        ListBox lstUsers;
        RichTextBox rtbChat;
        Button btnCreate, btnJoin, btnSend, btnKick, btnClose;
        string serverIP = "127.0.0.1";
        int port = 5000;

        public HostForm()
        {
            //===== FORM =====
            this.Text = "Meeting Room";
            this.Size = new Size(700, 500);

            //===== TITLE =====
            lblTitle = new Label();
            lblTitle.Text = "ONLINE MEETING ROOM";
            lblTitle.Location = new Point(200, 20);
            lblTitle.AutoSize = true;
            this.Controls.Add(lblTitle);

            //==== ROOM ID =====
            lblRoomID = new Label();
            lblRoomID.Text = "Room ID: ";
            lblRoomID.Location = new Point(50, 70);
            this.Controls.Add(lblRoomID);

            txtRoomID = new TextBox();
            txtRoomID.Location = new Point(120, 70);
            txtRoomID.Size = new Size(150, 25);
            this.Controls.Add(txtRoomID);

            //==== PASSWORD =====
            lblPassword = new Label();
            lblPassword.Text = "Password: ";
            lblPassword.Location = new Point(300, 70);
            this.Controls.Add(lblPassword);

            txtPassword = new TextBox();
            txtPassword.Location = new Point(380, 70);
            txtPassword.Size = new Size(150, 25);
            txtPassword.PasswordChar = '*';
            this.Controls.Add(txtPassword);

            // ===== CREATE =====
            btnCreate = new Button();
            btnCreate.Text = "Create";
            btnCreate.Location = new Point(550, 65);
            btnCreate.Click += BtnCreate_Click;
            this.Controls.Add(btnCreate);

            // ===== JOIN =====
            btnJoin = new Button();
            btnJoin.Text = "Join Room";
            btnJoin.Location = new Point(550, 100);
            btnJoin.Click += BtnJoin_Click;
            this.Controls.Add(btnJoin);

            //===== LINK =====
            lblLink = new Label();
            lblLink.Text = "Link: ";
            lblLink.Location = new Point(50, 110);
            lblLink.Size = new Size(500, 20);
            this.Controls.Add(lblLink);

             //===== USERS ===== 
            lblUsers = new Label();
            lblUsers.Location = new Point(300, 110);
            lblUsers.Text = "Users: 0/100";
            this.Controls.Add(lblUsers);

            //===== PARTICIPANTS TITLE =====
            lblParticipants = new Label();
            lblParticipants.Text = "Participants";
            lblParticipants.Location = new Point(50, 130);
            lblParticipants.AutoSize = true;
            this.Controls.Add(lblParticipants);
            
            //===== USER LIST =====
            lstUsers = new ListBox();
            lstUsers.Location = new Point(50, 150);
            lstUsers.Size = new Size(150, 200);
            this.Controls.Add(lstUsers);

            AddUser("You(Host)");

            //===== CHAT TITLE =====
            lblChat = new Label();
            lblChat.Text = "Chat";
            lblChat.Location = new Point(250, 130);
            lblChat.AutoSize = true;
            this.Controls.Add(lblChat);

            //===== CHAT =====
            rtbChat = new RichTextBox();
            rtbChat.Location = new Point(250, 150);
            rtbChat.Size = new Size(300, 200);
            this.Controls.Add(rtbChat);

            //===== INPUT =====
            txtMessage = new TextBox();
            txtMessage.Location = new Point(50, 380);
            txtMessage.Size = new Size(350, 25);
            this.Controls.Add(txtMessage);

            //===== SEND =====
            btnSend = new Button();
            btnSend.Text = "Send";
            btnSend.Location = new Point(420, 380);
            this.Controls.Add(btnSend);

            //===== KICK =====
            btnKick = new Button();
            btnKick.Text = "Kick";
            btnKick.Location = new Point(50, 420);
            btnKick.Click += BtnKick_Click;
            this.Controls.Add(btnKick);

            //===== CLOSE =====
            btnClose = new Button();
            btnClose.Text = "Close Room";
            btnClose.Location = new Point(150, 420);
            btnClose.Click += BtnClose_Click;
            this.Controls.Add(btnClose);

        }

        // ===== CREATE ROOM =====
        private void BtnCreate_Click(object sender, EventArgs e)
        {
            string roomID = txtRoomID.Text;
            if (roomID == "")
            {
                MessageBox.Show("Nhap Room ID truoc!");
                return;
            }
            string link = serverIP + ":" + port + "?room=" + roomID;
            lblLink.Text = "Link: " + link;
            Clipboard.SetText(link);
            MessageBox.Show("Room created!\nDa copy Link!");
        }

        // ===== JOIN ROOM =====
        private void BtnJoin_Click(object sender, EventArgs e)
        {
            AddUser("User mới");
        }
      
        //===== KICK =====
        private void BtnKick_Click(object sender, EventArgs e)
        {
            if (lstUsers.SelectedItem != null)
            {
                string user = lstUsers.SelectedItem.ToString();
                MessageBox.Show("Kick " + user);
            }
            else
            {
                MessageBox.Show("Chon user truoc! ");
            }
        }

        //===== CLOSE =====
        private void BtnClose_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Room Closed");
        }

        //===== ADD USER =====
        public void AddUser(string name)
        {
            lstUsers.Items.Add(name);
            lblUsers.Text = "Users: " + lstUsers.Items.Count + "/100";
        }
    }
}