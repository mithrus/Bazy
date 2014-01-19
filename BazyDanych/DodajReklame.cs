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
    public partial class DodajReklame : Form
    {
        public Form1 forma;
        static private string CiagPolaczenia = "Data Source=(local);"
              + "Initial Catalog=ReklamaDB;"
              + "Persist Security Info=False;"
              + "User ID=ReklamaDBUser;Password=MaSeŁkOhAsEłKo";
        public DodajReklame()
        {
            forma = Form1.ActiveForm as Form1;
            InitializeComponent();

            using (DataClasses1DataContext db = new DataClasses1DataContext(CiagPolaczenia))
            {
                //Branze
                var query =
                    from br in db.Branzas
                    select br;
                List<Branza> l1 = new List<Branza>(query);
                for (int i = 0; i < l1.Count; i++)
                {
                    comboBox1.Items.Add(l1[i].BranzaID+" "+l1[i].Nazwa);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

            using (DataClasses1DataContext db = new DataClasses1DataContext(CiagPolaczenia))
            {
                Reklama z = new Reklama
                {
                    Opis=textBox1.Text,
                    Szerokosc=Int32.Parse(textBox2.Text),
                    Wysokosc=Int32.Parse(textBox3.Text)               
                };
                db.Reklamas.InsertOnSubmit(z);
                try
                {
                    db.SubmitChanges();
                }
                catch (System.Data.SqlClient.SqlException ex)
                {
                    //System.Data.SqlClient.SqlException
                    MessageBox.Show(ex.Message, "error");
                    //Console.WriteLine(ex);
                    //db.SubmitChanges();
                }

                var q = from r in db.Reklamas
                        select r;

                List<Reklama> rr = new List<Reklama>(q);
                ReklamaZBranzy rzb = new ReklamaZBranzy
                {
                    BranzaID=Int32.Parse(comboBox1.SelectedItem.ToString().ElementAt(0).ToString()),
                    ReklamaID=rr.Last().ReklamaID
                };
                db.ReklamaZBranzies.InsertOnSubmit(rzb);
                try
                {
                    db.SubmitChanges();
                }
                catch (System.Data.SqlClient.SqlException ex)
                {
                    //System.Data.SqlClient.SqlException
                    MessageBox.Show(ex.Message, "error");
                    //Console.WriteLine(ex);
                    //db.SubmitChanges();
                }
            }

        }
    }
}
