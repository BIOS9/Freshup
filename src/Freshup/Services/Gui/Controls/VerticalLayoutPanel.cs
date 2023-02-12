using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Freshup.Services.Gui.Controls
{
    public partial class VerticalLayoutPanel : FlowLayoutPanel
    {
        public VerticalLayoutPanel()
        {
            InitializeComponent();

            // Disable horizontal scroll
            HorizontalScroll.Maximum = 0;
            AutoScroll = false; // Winforms is weird yo
            VerticalScroll.Visible = false;
            AutoScroll = true;
        }

        private void organise()
        {
            foreach (Control c in Controls)
            {
                if (VerticalScroll.Visible)
                    c.Width = Width - SystemInformation.VerticalScrollBarWidth;
                else
                    c.Width = Width;
            }
        }

        private void VerticalLayoutPanel_ControlAdded(object sender, ControlEventArgs e)
        {
            organise();
        }

        private void VerticalLayoutPanel_ControlRemoved(object sender, ControlEventArgs e)
        {
            organise();
        }

        private void VerticalLayoutPanel_Resize(object sender, EventArgs e)
        {
            organise();
        }
    }
}
