using Freshup.Services.Testing;
using Microsoft.Extensions.Logging;

namespace Freshup.Services.Gui
{
    internal class Gui
    {
        private readonly ILogger<SettingsForm> _logger;
        private readonly ITicketApp _ticketApp;
        private readonly NotificationForm _notificationForm;

        public Gui(ILogger<SettingsForm> logger, ITicketApp ticketApp)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _ticketApp = ticketApp ?? throw new ArgumentNullException(nameof(ticketApp));
            _ticketApp.NewTicket += _ticketApp_NewTicket;
            _notificationForm = new NotificationForm();
        }

        public void Run()
        {
            Application.Run(_notificationForm);
        }

        private void _ticketApp_NewTicket(object sender, TicketApp.ITicket ticket)
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
    }
}
