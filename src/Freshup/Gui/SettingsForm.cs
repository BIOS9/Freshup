using System.Windows.Forms;
using Freshup.Services.Gui;
using Freshup.Services.TicketApp;
using Microsoft.Extensions.Logging;

namespace Freshup.Services.Testing;

public partial class SettingsForm : Form
{
    public SettingsForm()
    {
        InitializeComponent();
    }

    private void SettingsForm_Load(object sender, EventArgs e)
    {
        domainTextBox.Text = Properties.Settings.Default.FreshdeskDomain;
        apiKeyTextBox.Text = Properties.Settings.Default.FreshdeskApiKey;
    }

    private void saveButton_Click(object sender, EventArgs e)
    {
        Properties.Settings.Default.FreshdeskDomain = domainTextBox.Text;
        Properties.Settings.Default.FreshdeskApiKey = apiKeyTextBox.Text;
        Properties.Settings.Default.Save();
        DialogResult = DialogResult.OK;
        Close();
    }
}