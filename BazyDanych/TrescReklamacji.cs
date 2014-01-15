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
        int IDzlec;
        int IDklient;
        int IDprac;
        static private string CiagPolaczenia = "Data Source=(local);"
               + "Initial Catalog=ReklamaDB;"
               + "Persist Security Info=False;"
               + "User ID=ReklamaDBUser;Password=MaSeŁkOhAsEłKo";
        public TrescReklamacji(int idZ, int idK, int idP)
        {
            IDzlec = idZ;
            IDklient = idK;
            IDprac = idP;
            forma = Form1.ActiveForm as Form1;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {          
            using (DataClasses1DataContext db = new DataClasses1DataContext(CiagPolaczenia))
            {
                Reklamacja z = new Reklamacja
                {
                    DataWystawienia = DateTime.Today,
                    Tresc = textBox1.Text,
                    ZlecenieID = IDzlec,
                    KlientID = IDklient,
                    PracownikID = IDprac
                };
                db.Reklamacjas.InsertOnSubmit(z);
                try
                {
                    db.SubmitChanges();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    // Make some adjustments.
                    // ...
                    // Try again.
                    db.SubmitChanges();
                }              
            }

            this.Close();
        }
    }
}
