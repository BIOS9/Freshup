using Freshup.Services.Gui.Controls;
using Freshup.Services.TicketApp;
using System.Media;

namespace Freshup.Services.Gui
{
    public partial class NotificationForm : Form
    {
        private bool _allowShow = false;


        public NotificationForm()
        {
            InitializeComponent();
            ticketPanel.ControlRemoved += TicketPanel_ControlRemoved;
        }

        public void Notify(ITicket ticket)
        {
            _allowShow = true;
            SetLocation(Screen.PrimaryScreen);
            Show();
            WindowState = FormWindowState.Normal;

            ticketPanel.Controls.Add(new TicketSummary(ticket));

            using SoundPlayer soundPlayer = new SoundPlayer(Properties.Resources.DefaultNotificationSound);
            soundPlayer.Play();
        }

        private void SetLocation(Screen notificationScreen)
        {
            Location = notificationScreen.WorkingArea.Location + notificationScreen.WorkingArea.Size - Size;
        }

        #region EVENT_HANDLERS

        private void NotificationForm_Load(object sender, EventArgs e)
        { 

        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            if(!_allowShow) this.Hide();
        }

        private void NotificationForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Hide();
            ticketPanel.Controls.Clear();
        }

        private void closeButton1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void NotificationForm_Move(object sender, EventArgs e)
        {
            SetLocation(Screen.PrimaryScreen);
        }

        private void NotificationForm_Resize(object sender, EventArgs e)
        {
            SetLocation(Screen.PrimaryScreen);
        }

        private void TicketPanel_ControlRemoved(object? sender, ControlEventArgs e)
        {
            if(ticketPanel.Controls.Count == 0)
                Close();
        }

        #endregion
    }
}
