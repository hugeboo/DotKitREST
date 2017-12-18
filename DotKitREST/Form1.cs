using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DotKitREST
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            HttpTest.SendGET();
            HttpTest.SendPOST();
            HttpsTest.SendGET();
            HttpsTest.SendPOST();
            HttpsTest.SendPOST_AuthFailed();
        }
    }
}
