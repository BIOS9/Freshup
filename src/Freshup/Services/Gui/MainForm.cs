using System.Windows.Forms;
using Freshup.Services.Gui;
using Microsoft.Extensions.Logging;

namespace Freshup.Services.Testing;

public partial class MainForm : Form
{
    private readonly ILogger<MainForm> _logger;
    private readonly ITicketApp _ticketApp;
    private readonly NotificationForm _notificationForm;

    public MainForm(ILogger<MainForm> logger, ITicketApp ticketApp)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _ticketApp = ticketApp ?? throw new ArgumentNullException(nameof(ticketApp));
        _notificationForm = new NotificationForm();
        InitializeComponent();
    }

    private void testNotificationButton_Click(object sender, EventArgs e)
    {

    }
}