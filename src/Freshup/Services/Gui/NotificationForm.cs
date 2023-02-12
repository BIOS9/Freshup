﻿using Freshup.Services.Gui.Controls;
using Freshup.Services.TicketApp;
using System.Media;

namespace Freshup.Services.Gui
{
    public partial class NotificationForm : Form
    {
        public NotificationForm()
        {
            InitializeComponent();
        }

        public void notify(ITicket ticket)
        {
            SetLocation(Screen.PrimaryScreen);
            Show();

            ticketPanel.Controls.Add(new TicketSummary(ticket));

            using SoundPlayer soundPlayer = new SoundPlayer(Resources.DefaultNotificationSound);
            soundPlayer.Play();
        }

        private void NotificationForm_Load(object sender, EventArgs e)
        {

        }

        private void NotificationForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void SetLocation(Screen notificationScreen)
        {
            Location = notificationScreen.WorkingArea.Location + notificationScreen.WorkingArea.Size - Size;
        }

        private void closeButton1_Click(object sender, EventArgs e)
        {
            Close();
            ticketPanel.Controls.Clear();
        }

        private void NotificationForm_Move(object sender, EventArgs e)
        {
            SetLocation(Screen.PrimaryScreen);
        }

        private void NotificationForm_Resize(object sender, EventArgs e)
        {
            SetLocation(Screen.PrimaryScreen);
        }
    }
}
