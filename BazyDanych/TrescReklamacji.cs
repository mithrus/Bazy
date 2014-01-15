using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BazyDanych
{
    public partial class TrescReklamacji : Form
    {
        public Form1 forma;
        public TrescReklamacji()
        {
            forma = Form1.ActiveForm as Form1;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            forma.pobierzTrescReklamacji(textBox1.Text);
            this.Close();
        }
    }
}
