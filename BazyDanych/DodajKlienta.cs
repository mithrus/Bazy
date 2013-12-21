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
    public partial class DodajKlienta : Form
    {
        public Form1 forma;
        static private string CiagPolaczenia = "Data Source=(local);"
               + "Initial Catalog=ReklamaDB;"
               + "Persist Security Info=False;"
               + "User ID=ReklamaDBUser;Password=MaSeŁkOhAsEłKo";

        public DodajKlienta()
        {
            forma = Form1.ActiveForm as Form1;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (DataClasses1DataContext db = new DataClasses1DataContext(CiagPolaczenia))
            {

                Klient k = new Klient
                {
                    NIP=textBox1.Text,
                    Nazwa=textBox2.Text,
                    Adres=textBox3.Text
                };
                db.Klients.InsertOnSubmit(k);

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
                //return k.KlientID;

                var query =
                    from kl in db.Klients
                    select kl;
                
                forma.dataGridView3.DataSource = query;
                db.Connection.Close();

                textBox1.Clear();
                textBox2.Clear();
                textBox3.Clear();
            }
        }
    }
}
