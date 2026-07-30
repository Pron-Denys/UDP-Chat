using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace UDP_Chat
{
    using System.Net;
    using UserName;
    public partial class Form2 : Form
    {
        UserName? User;
        public Form2(object user)
        {
            InitializeComponent();
            User = user as UserName;
        }

        private void Click_OK(object sender, EventArgs e)
        {
            if (textBox1.Text != "")
            {
                User?.name = textBox1.Text;
            }
        }

        private void Cancel(object sender, EventArgs e)
        {
            User?.name = null;
        }

        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (textBox1.Text != "")
                User?.name = textBox1.Text;
            else
                User?.name = null;
        }
    }
}
