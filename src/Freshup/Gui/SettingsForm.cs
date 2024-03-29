﻿using Freshup.Services.Gui;
using Freshup.TicketApp.FreshdeskTicketApp;
using Freshup.TicketApp;

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
        exitMenuItem.Click += ExitMenuItem_Click;
        aboutMenuItem.Click += AboutMenuItem_Click;
        notifyIcon1.Icon = Properties.Resources.AppleCan;
    }

    private bool TryInitTicketApp(out string error)
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

            error = string.Empty;
            return true;
        }
        catch (ArgumentException ex)
        {
            error = ex.Message;
            return false;
        }
    }

    private void SettingsMenuItem_Click(object? sender, EventArgs e)
    {
        _firstHide = false;
        Show();
        ShowInTaskbar = true;
        WindowState = FormWindowState.Normal;
    }

    private void AboutMenuItem_Click(object? sender, EventArgs e)
    {
        string verStr = Program.CurrentVersion?.ToString() ?? "Unknown";
        MessageBox.Show($"Freshup version {verStr}", "About Freshup", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void ExitMenuItem_Click(object? sender, EventArgs e)
    {
        notifyIcon1.Visible = false;
        Environment.Exit(0);
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
        
        if(TryInitTicketApp(out string error))
        {
            Hide();
        }
        else
        {
            MessageBox.Show(error, "Invalid Settings", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    private void SettingsForm_Activated(object sender, EventArgs e)
    {
        if (_firstHide)
        {
            if(TryInitTicketApp(out string error))
                Hide();
            Opacity = 1;
        }
    }

    private void _ticketApp_ExceptionThrown(object sender, Exception ex)
    {
        if(MessageBox.Show("Freshup exception: " + ex.Message + "\n\nClick yes to copy the stack trace to your clipboard.", "Fresup", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Error) == DialogResult.Yes)
        {
            Clipboard.SetText(ex.StackTrace);
        } 
    }

    private void _ticketApp_NewTicket(object sender, ITicket ticket)
    {
        Invoke(() => _notificationForm.Notify(ticket));
    }

    private void cancelButton_Click(object sender, EventArgs e)
    {
        Hide();
    }

    private void contextMenuStrip1_Opening(object sender, System.ComponentModel.CancelEventArgs e)
    {

    }
}