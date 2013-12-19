using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data.Linq;

namespace BazyDanych
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        static private string CiagPolaczenia = "Data Source=(local);"
               + "Initial Catalog=ReklamaDB;"
               + "Persist Security Info=False;"
               + "User ID=ReklamaDBUser;Password=MaSeŁkOhAsEłKo";

        private void logowanieToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            DataClasses1DataContext db = new DataClasses1DataContext(CiagPolaczenia);
            var query =
                from kl in db.Klients
                where kl.Nazwa == "Zenon"
                select kl;
            dataGridView1.DataSource = query;
            db.Connection.Close();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (DataClasses1DataContext db = new DataClasses1DataContext(CiagPolaczenia))
            {
                var query =
                    from zl in db.Zlecenies
                    from pr in db.Pracowniks
                    from kl in db.Klients
                    from rek in db.Reklamas
                    where zl.ReklamaID==rek.ReklamaID
                    where zl.KlientID==kl.KlientID
                    where zl.PracownikID==pr.PracownikID
                    select new { zl.ZlecenieID, Pracownik=pr.Imie+" "+pr.Nazwisko, NazwaKlienta=kl.Nazwa ,WymiaryReklamy=rek.Szerokosc+"x"+rek.Wysokosc, zl.TerminRozpoczecia, zl.TerminZakonczenia, zl.StanZlecenia };
                dataGridView2.DataSource = query;
                db.Connection.Close();
            }
        }
    }
}
