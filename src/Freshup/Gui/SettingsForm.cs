using System.Windows.Forms;
using Freshup.Services.Gui;
using Freshup.Services.TicketApp;
using Microsoft.Extensions.Logging;

namespace Freshup.Services.Testing;

public partial class SettingsForm : Form
{
    private readonly ILogger<SettingsForm> _logger;
    private readonly ITicketApp _ticketApp;
    private readonly NotificationForm _notificationForm;

    public SettingsForm(ILogger<SettingsForm> logger, ITicketApp ticketApp)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _ticketApp = ticketApp ?? throw new ArgumentNullException(nameof(ticketApp));
        _notificationForm = new NotificationForm();
        InitializeComponent();
    }

    private void testNotificationButton_Click(object sender, EventArgs e)
    {
        _notificationForm.Notify(new TestTicket());
    }

    private class TestTicket : ITicket
    {
        public string? Subject => "Test Ticket Subject";
        public string? Description => "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";
        public string? SenderEmail => "bobby.fischer@email.com";
        public string? SenderName => "Bobby Fischer";
        public Uri? Link => null;
    }

    private void SettingsForm_Load(object sender, EventArgs e)
    {

    }
}