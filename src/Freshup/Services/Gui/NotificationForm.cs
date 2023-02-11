using System.Media;

namespace Freshup.Services.Gui
{
    public partial class NotificationForm : Form
    {
        public NotificationForm()
        {
            InitializeComponent();
        }

        public void notify()
        {
            Show();

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
    }
}
