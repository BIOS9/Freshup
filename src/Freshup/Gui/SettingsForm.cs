using Freshup.Services.Gui;
using Freshup.Services.TicketApp;
using Freshup.Services.TicketApp.FreshdeskTicketApp;

namespace Freshup.Services.Testing;

public partial class SettingsForm : Form
{
    private ITicketApp? _ticketApp;
    private bool _firstHide = true;
    private NotificationForm _notificationForm = new ();

    public SettingsForm()
    {
        InitializeComponent();
        settingsMenuItem.Click += SettingsMenuItem_Click;
        notifyIcon1.Icon = Properties.Resources.AppleCan;
    }

    public void LoadTicketApp()
    {
        while (true)
        {
            try
            {
                if (_ticketApp != null)
                {
                    _ticketApp.NewTicket -= _ticketApp_NewTicket;
                    _ticketApp.ExceptionThrown -= _ticketApp_ExceptionThrown;
                    _ticketApp.Stop();
                    _ticketApp.Dispose();
                    _ticketApp = null;
                }

                _ticketApp = new FreshdeskTicketApp(
                   Properties.Settings.Default.FreshdeskDomain,
                   Properties.Settings.Default.FreshdeskApiKey,
                   Properties.Settings.Default.FreshdeskPollInterval);
                _ticketApp.NewTicket += _ticketApp_NewTicket;
                _ticketApp.ExceptionThrown += _ticketApp_ExceptionThrown;
                _ticketApp.Start();

                break;
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Invalid Settings", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                using (var settingsForm = new SettingsForm())
                {
                    _firstHide = false;
                    Show();
                }
            }
        }
    }

    private void SettingsMenuItem_Click(object? sender, EventArgs e)
    {
        _firstHide = false;
        Show();
        ShowInTaskbar = true;
        WindowState = FormWindowState.Normal;
    }

    private void SettingsForm_Load(object sender, EventArgs e)
    {
        domainTextBox.Text = Properties.Settings.Default.FreshdeskDomain;
        apiKeyTextBox.Text = Properties.Settings.Default.FreshdeskApiKey;

        LoadTicketApp();
    }

    private void saveButton_Click(object sender, EventArgs e)
    {
        Properties.Settings.Default.FreshdeskDomain = domainTextBox.Text;
        Properties.Settings.Default.FreshdeskApiKey = apiKeyTextBox.Text;
        Properties.Settings.Default.Save();
        DialogResult = DialogResult.OK;
        Close();
    }

    private void SettingsForm_Activated(object sender, EventArgs e)
    {
        if (_firstHide)
        {
            Hide();
            Opacity = 1;
        }
    }

    private void _ticketApp_ExceptionThrown(object sender, Exception ex)
    {
        MessageBox.Show("Freshup exception: " + ex.Message, "Fresup", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    private void _ticketApp_NewTicket(object sender, ITicket ticket)
    {
        Invoke(() => _notificationForm.Notify(ticket));
    }
}