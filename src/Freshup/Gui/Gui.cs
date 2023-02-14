using Freshup.Services.Testing;
using Freshup.Services.TicketApp;
using Freshup.Services.TicketApp.FreshdeskTicketApp;
using Microsoft.Extensions.Logging;

namespace Freshup.Services.Gui
{
    internal class Gui : ApplicationContext
    {
        private readonly NotifyIcon _trayIcon;
        private ITicketApp? _ticketApp;

        public Gui()
        {
            LoadTicketApp();
            ContextMenuStrip contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add(new ToolStripMenuItem("Exit", null, new EventHandler(Exit)));
            _trayIcon = new NotifyIcon()
            {
                Icon = Properties.Resources.AppleCan,
                ContextMenuStrip = contextMenu,
                Visible = true
            };

            //_ticketApp.NewTicket += _ticketApp_NewTicket;
            //_notificationForm = new NotificationForm();
        }

        public void LoadTicketApp()
        {
            bool firstRun = true;
            while (true)
            {
                try
                {
                    if(_ticketApp != null)
                    {
                        _ticketApp.NewTicket -= _ticketApp_NewTicket;
                        _ticketApp.Stop();
                        _ticketApp.Dispose();
                        _ticketApp = null;
                    }
                   
                    _ticketApp = new FreshdeskTicketApp(
                       Properties.Settings.Default.FreshdeskDomain,
                       Properties.Settings.Default.FreshdeskApiKey,
                       Properties.Settings.Default.FreshdeskPollInterval);
                    _ticketApp.NewTicket += _ticketApp_NewTicket;
                    _ticketApp.Start();

                    break;
                } 
                catch(ArgumentException ex)
                {
                    if(!firstRun)
                        MessageBox.Show(ex.Message, "Invalid Settings", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    using (var settingsForm = new SettingsForm())
                        settingsForm.ShowDialog();
                }
                firstRun = false;
            }
        }

        private void _ticketApp_NewTicket(object sender, ITicket ticket)
        {
            //if (_notificationForm.InvokeRequired)
            //{
            //    _notificationForm.Invoke(() => _notificationForm.Notify(ticket));
            //}
            //else
            //{
            //    _notificationForm.Notify(ticket);
            //}
        }

        private void Exit(object sender, EventArgs e)
        {
            // Hide tray icon, otherwise it will remain shown until user mouses over it
            _trayIcon.Visible = false;
            Application.Exit();
        }
    }
}
