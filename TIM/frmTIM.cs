/*
 * TIM - A meeting cost calculator
 * Copyright 2010 Michael Farrell <http://micolous.id.au/>.
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace TIM
{
    public partial class frmTIM : Form
    {
        private bool Running = false;
        private TimeSpan LastTimer = new TimeSpan(0);
        private DateTime LastStartedAt;
        private long HourlyRate;
        private long People;

        public frmTIM()
        {
            InitializeComponent();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (Running)
            {
                Calculate();
            }
        }

        private void btnStartStop_Click(object sender, EventArgs e)
        {
            if (Running)
            {
                // stop running
                Running = false;
                btnStartStop.Text = "&Start";
                LastTimer += (DateTime.Now - LastStartedAt);
                LastStartedAt = DateTime.Now;
            }
            else
            {
                // start running
                Running = true;
                btnStartStop.Text = "&Stop";
                LastStartedAt = DateTime.Now;
            }

            /* allow changing values in real-time
            btnReset.Enabled = !Running;
            txtHourlyRate.Enabled = !Running;
            txtPeople.Enabled = !Running;*/
            timer1.Enabled = Running;
            Calculate();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            LastTimer = new TimeSpan(0);
            LastStartedAt = DateTime.Now;
            Calculate();

        }

        void Calculate()
        {
            // this gets validated in other methods.
            HourlyRate = long.Parse(txtHourlyRate.Text);
            People = long.Parse(txtPeople.Text);

            TimeSpan meetingTime = LastTimer + (Running?(DateTime.Now - LastStartedAt):new TimeSpan());
            lblTime.Text = string.Format("{0:00}h {1:00}m {2:00}s", Math.Floor(meetingTime.TotalHours), meetingTime.Minutes, meetingTime.Seconds);
            lblCost.Text = string.Format("{0:n}", (meetingTime.TotalHours * (double)People * (double)HourlyRate));
        }

        private void txtHourlyRate_TextChanged(object sender, EventArgs e)
        {
            TextBox t = (TextBox)sender;
            try
            {
                string newv = long.Parse(t.Text).ToString();
                if (t.Text != newv)
                {
                    // shift selection appropriately
                    int newss = t.SelectionStart - (t.Text.Length - newv.Length);
                    t.Text = newv;
                    t.SelectionStart = newss;
                }
                Calculate();
            }
            catch (Exception)
            {
                t.Text = "0";
                t.SelectionStart = 1;
            }
        }

        private void txtHourlyRate_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !("0123456789\x08".Contains(e.KeyChar.ToString()));
        }
    }
}
