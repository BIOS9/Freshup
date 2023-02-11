using Freshup.Services.TicketApp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Freshup.Services.Gui
{
    public partial class TicketSummary : UserControl
    {
        private readonly ITicket _ticket;

        public TicketSummary(ITicket ticket)
        {
            _ticket = ticket;
            InitializeComponent();
        }

        private void TicketSummary_Load(object sender, EventArgs e)
        {
            subjectLabel.Text = _ticket.Subject;
        }
    }
}
