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
    public partial class WyszukiwanieZlec : Form
    {
        public Form1 forma;
        List<string> stan = new List<string>();
       // List<Lokalizacja> lokal = new List<Lokalizacja>();
        List<Zlecenie> zlecen = new List<Zlecenie>();
        List<int> zlecID = new List<int>();
        List<Klient> l1;//lista klientow
        List<int> kliID = new List<int>();
        static private string CiagPolaczenia = "Data Source=(local);"
              + "Initial Catalog=ReklamaDB;"
              + "Persist Security Info=False;"
              + "User ID=ReklamaDBUser;Password=MaSeŁkOhAsEłKo";
        public WyszukiwanieZlec()
        {
            forma = Form1.ActiveForm as Form1;
            InitializeComponent();
            using (DataClasses1DataContext db = new DataClasses1DataContext(CiagPolaczenia))
            {
                //Klienci
                var query2 =
                    from kl in db.Klients
                    select kl;
                l1 = new List<Klient>(query2);
                comboBox1.Items.Add("Wszyscy");
                for (int i = 0; i < l1.Count; i++)
                {
                    comboBox1.Items.Add(l1[i].KlientID + " " + l1[i].Nazwa);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {           
            zlecID.Clear();        
            for (int i = 0; i < zlecen.Count; i++)
            {
                zlecID.Add(zlecen.ElementAt(i).ZlecenieID);
            }
                using (DataClasses1DataContext db = new DataClasses1DataContext(CiagPolaczenia))
                {
                    var temp = db.Zlecenies
                      .Where(zl => stan.Contains(zl.StanZlecenia))
                      .Where(x=> zlecID.Contains(x.ZlecenieID))
                      .Where(k => kliID.Contains(k.KlientID))
                      .OrderBy(zl => zl.ZlecenieID);

                    var subquery =
                    from lok in db.Lokalizacjas
                    from rWl in db.ReklamaWLokalizacjis
                    where rWl.LokalizacjaID == lok.LokalizacjaID
                    select new { rWl.ReklamaID, lok.Opis, lok.Szerokosc, lok.Wysokosc, lok.WolneMiejsce };

                    var query =
                        //from t in temp
                        from zl in temp
                        from pr in db.Pracowniks
                        from kl in db.Klients
                        from rek in db.Reklamas
                        from lok in subquery
                       // where zl.ZlecenieID==t.ZlecenieID
                        where zl.ReklamaID == rek.ReklamaID
                        where zl.KlientID == kl.KlientID
                        where zl.PracownikID == pr.PracownikID
                        where rek.ReklamaID == lok.ReklamaID
                        select new { zl.ZlecenieID, Pracownik = pr.Imie + " " + pr.Nazwisko, NazwaKlienta = kl.Nazwa, OpisReklamy = rek.Opis, WymiaryReklamy = rek.Szerokosc + "x" + rek.Wysokosc, Lokalizacja = "Opis: " + lok.Opis + ", Wymiary: " + lok.Szerokosc + "x" + lok.Wysokosc + ", Wolna powierzchnia: " + lok.WolneMiejsce, zl.TerminRozpoczecia, zl.TerminZakonczenia, zl.StanZlecenia };


                    forma.dataGridView2.DataSource = query;
                }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
                stan.Add(checkBox1.Text);
            else
                stan.Remove(checkBox1.Text);
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked == true)
                stan.Add(checkBox2.Text);
            else
                stan.Remove(checkBox2.Text);
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked == true)
                stan.Add(checkBox3.Text);
            else
                stan.Remove(checkBox3.Text);
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox4.Checked == true)
                stan.Add(checkBox4.Text);
            else
                stan.Remove(checkBox4.Text);
        }      

        private void timer1_Tick(object sender, EventArgs e)
        {
            using (DataClasses1DataContext db = new DataClasses1DataContext(CiagPolaczenia))
            {
                zlecen.Clear();
                if (checkBox5.Checked == true)
                {
                    var query2 =
                    from lok in db.Lokalizacjas
                    from rwl in db.ReklamaWLokalizacjis
                    from z in db.Zlecenies
                    from sl in db.SlupOgloszeniowies
                    where lok.LokalizacjaID == sl.LokalizacjaID
                    where lok.LokalizacjaID == rwl.LokalizacjaID
                    where rwl.ReklamaID == z.ReklamaID
                    select z;
                    zlecen.AddRange(query2);
                }
                if (checkBox6.Checked == true)
                {
                    var query2 =
                        from lok in db.Lokalizacjas
                        from rwl in db.ReklamaWLokalizacjis
                        from z in db.Zlecenies
                        from tr in db.Tramwajs
                        where lok.LokalizacjaID == tr.LokalizacjaID
                        where lok.LokalizacjaID == rwl.LokalizacjaID
                        where rwl.ReklamaID == z.ReklamaID
                        select z;
                    zlecen.AddRange(query2);
                }
                if (checkBox7.Checked == true)
                {
                    var query2 =
                        from lok in db.Lokalizacjas
                        from rwl in db.ReklamaWLokalizacjis
                        from z in db.Zlecenies
                        from bud in db.Budyneks
                        where lok.LokalizacjaID == bud.LokalizacjaID
                        where lok.LokalizacjaID == rwl.LokalizacjaID
                        where rwl.ReklamaID == z.ReklamaID
                        select z;
                    zlecen.AddRange(query2);
                }
                if (checkBox8.Checked == true)
                {
                    var query2 =
                        from lok in db.Lokalizacjas
                        from rwl in db.ReklamaWLokalizacjis
                        from z in db.Zlecenies
                        from bil in db.Bilboards
                        where lok.LokalizacjaID == bil.LokalizacjaID
                        where lok.LokalizacjaID == rwl.LokalizacjaID
                        where rwl.ReklamaID == z.ReklamaID
                        select z;
                    zlecen.AddRange(query2);
                }
                if (checkBox9.Checked == true)
                {
                    var query2 =
                        from lok in db.Lokalizacjas
                        from rwl in db.ReklamaWLokalizacjis
                        from z in db.Zlecenies
                        from ogr in db.Ogrodzenies
                        where lok.LokalizacjaID == ogr.LokalizacjaID
                        where lok.LokalizacjaID == rwl.LokalizacjaID
                        where rwl.ReklamaID == z.ReklamaID
                        select z;
                    zlecen.AddRange(query2);
                }
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            kliID.Clear();

            if (comboBox1.SelectedIndex == 0)
            {
                for (int i = 0; i < l1.Count; i++)
                {
                    kliID.Add(l1.ElementAt(i).KlientID);
                }
            }
            for (int i = 0; i < l1.Count; i++)
            {
                if (comboBox1.SelectedIndex == i + 1)
                {
                    kliID.Add(l1.ElementAt(i).KlientID);
                }
            }
        }

      
    }
}
