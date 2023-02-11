using System.Windows.Forms;
using Microsoft.Extensions.Logging;

namespace Freshup.Services.Testing;

public partial class MainForm : Form
{
    private readonly ILogger<Testing> _logger;
    private readonly ITicketApp _ticketApp;
    
    public MainForm(ILogger<Testing> logger, ITicketApp ticketApp)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _ticketApp = ticketApp ?? throw new ArgumentNullException(nameof(ticketApp));
        InitializeComponent();
    }
}