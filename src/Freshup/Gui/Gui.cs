﻿using Freshup.Services.Testing;
using Freshup.Services.TicketApp;
using Freshup.Services.TicketApp.FreshdeskTicketApp;
using Microsoft.Extensions.Logging;

namespace Freshup.Services.Gui
{
    internal class Gui : ApplicationContext
    {
        private readonly NotifyIcon _trayIcon;
        private readonly ITicketApp _ticketApp;
        private readonly NotificationForm _notificationForm;

        public Gui()
        {
            ContextMenuStrip contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add(new ToolStripMenuItem("Exit", null, new EventHandler(Exit)));
            _trayIcon = new NotifyIcon()
            {
                Icon = Properties.Resources.AppleCan,
                ContextMenuStrip = contextMenu,
                Visible = true
            };

            //_ticketApp = new FreshdeskTicketApp(
            //    Properties.Settings.Default.FreshdeskDomain,
            //    Properties.Settings.Default.FreshdeskApiKey,
            //    Properties.Settings.Default.FreshdeskPollInterval);
            //_ticketApp.NewTicket += _ticketApp_NewTicket;
            //_notificationForm = new NotificationForm();
        }

        public void Run()
        {
            
        }

        private void _ticketApp_NewTicket(object sender, ITicket ticket)
        {
            if (_notificationForm.InvokeRequired)
            {
                _notificationForm.Invoke(() => _notificationForm.Notify(ticket));
            }
            else
            {
                _notificationForm.Notify(ticket);
            }
        }

        private void Exit(object sender, EventArgs e)
        {
            // Hide tray icon, otherwise it will remain shown until user mouses over it
            _trayIcon.Visible = false;
            Application.Exit();
        }
    }
}
