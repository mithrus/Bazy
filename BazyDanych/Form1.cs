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
        bool KlientEdit = false;
        public int a;// zmienna pomocnicza przy edytowaniu danych klienta
        //public string reklamacjaTresc=null;
            

        private void logowanieToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            

        }

        private void button2_Click(object sender, EventArgs e)
        {
            button8.Enabled = true;
            button9.Enabled = true;
            using (DataClasses1DataContext db = new DataClasses1DataContext(CiagPolaczenia))
            {
                var subquery =
                    from lok in db.Lokalizacjas                  
                    from rWl in db.ReklamaWLokalizacjis
                    where rWl.LokalizacjaID == lok.LokalizacjaID                  
                    select new {rWl.ReklamaID,lok.Opis, lok.Szerokosc,lok.Wysokosc, lok.WolneMiejsce};

                var query =
                    from zl in db.Zlecenies
                    from pr in db.Pracowniks
                    from kl in db.Klients
                    from rek in db.Reklamas
                    from lok in subquery
                    where zl.ReklamaID==rek.ReklamaID
                    where zl.KlientID==kl.KlientID
                    where zl.PracownikID==pr.PracownikID
                    where rek.ReklamaID==lok.ReklamaID
                    select new { zl.ZlecenieID, Pracownik=pr.Imie+" "+pr.Nazwisko, NazwaKlienta=kl.Nazwa ,OpisReklamy=rek.Opis,WymiaryReklamy=rek.Szerokosc+"x"+rek.Wysokosc,Lokalizacja="Opis: "+lok.Opis+", Wymiary: "+lok.Szerokosc+"x"+lok.Wysokosc+", Wolna powierzchnia: "+lok.WolneMiejsce, zl.TerminRozpoczecia, zl.TerminZakonczenia, zl.StanZlecenia };
                dataGridView2.DataSource = query;             
                db.Connection.Close();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            using (DataClasses1DataContext db = new DataClasses1DataContext(CiagPolaczenia))
            {
                var query =
                    from kl in db.Klients
                    select kl;
                dataGridView3.DataSource = query;
                db.Connection.Close();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            DodajKlienta nowyKl = new DodajKlienta();
            nowyKl.FormClosed += nowyKl_FormClosed;
            nowyKl.Show(); 
        }

        private void nowyKl_FormClosed(object sender, FormClosedEventArgs e)//akcja zamknięcia okna 
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            using (DataClasses1DataContext db = new DataClasses1DataContext(CiagPolaczenia))
            {
                for (int i = 0; i < dataGridView3.RowCount; i++)
                {
                    var x = dataGridView3[0, i];
                    if (x.Selected == true)
                    {
                        var del =
                        from row in db.Klients
                        where row.KlientID == (int)x.Value
                        select row;

                        foreach (var detail in del)
                        {
                            db.Klients.DeleteOnSubmit(detail);
                        }
                    }
                }
                try
                {
                    db.SubmitChanges();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    // Provide for exceptions.
                }


                var query =
                    from kl in db.Klients
                    select kl;
                dataGridView3.DataSource = query;
                db.Connection.Close();
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {           
            if (KlientEdit == false)
            {
                dataGridView3.ReadOnly = false;
                var x = dataGridView3.SelectedCells;
                a = x[0].RowIndex;
                KlientEdit = true;
                button6.Text = "Zatwierdź zmiany";
                dataGridView3.GridColor = Color.Red;
            }
            else if (KlientEdit == true)
            {
                dataGridView3.ReadOnly = true ;
                button6.Text = "Edytuj";
                KlientEdit = false;
                dataGridView3.GridColor = SystemColors.ControlDark;//Color.ControlDark;
                using (DataClasses1DataContext db = new DataClasses1DataContext(CiagPolaczenia))
                {
                    var query =
                    from aa in db.Klients
                    where aa.KlientID == (int)dataGridView3[0, a].Value
                    select aa;

                    foreach (Klient ob in query)
                    {
                        ob.NIP = dataGridView3[1, a].Value.ToString();
                        ob.Nazwa = dataGridView3[2, a].Value.ToString();
                        ob.Adres = dataGridView3[3, a].Value.ToString();
                    }


                    try
                    {
                        db.SubmitChanges();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);

                    }
                }
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            DodajZlecenie noweZl = new DodajZlecenie();
            //noweZl.FormClosed += noweZl_FormClosed;
            noweZl.Show();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            using (DataClasses1DataContext db = new DataClasses1DataContext(CiagPolaczenia))
            {             
                for (int i = 0; i < dataGridView2.RowCount; i++)
                {
                    var x = dataGridView2[0, i];
                    if (x.Selected == true)
                    {
                        //usunięcie z tabeli Zlecenie
                        var del =
                        from row in db.Zlecenies
                        where row.ZlecenieID == (int)x.Value
                        select row;
                        List<Zlecenie> temp= new List<Zlecenie>(del);
                        
                        foreach (var detail in del)
                        {
                            db.Zlecenies.DeleteOnSubmit(detail);
                        }

                        //usuwanie z tabeli ReklamaWLokalizacji                   

                        var del2 =
                        from row in db.ReklamaWLokalizacjis
                        where row.ReklamaID ==temp.First().ReklamaID
                        select row;

                        List<ReklamaWLokalizacji> temp2= new List<ReklamaWLokalizacji>(del2);
                        foreach (var detail in del2)
                        {
                            db.ReklamaWLokalizacjis.DeleteOnSubmit(detail);
                        }
                        

                        //update w tabeli Lokalizacja
                        var update =
                        from l in db.Lokalizacjas
                        where l.LokalizacjaID == temp2.First().LokalizacjaID
                        select l;

                        var rek =
                        from r in db.Reklamas
                        where r.ReklamaID == temp2.First().ReklamaID
                        select r;
                        List<Reklama> temp3 = new List<Reklama>(rek);

                        foreach (Lokalizacja up in update)
                        {
                            up.WolneMiejsce = up.WolneMiejsce + temp3.First().Szerokosc*temp3.First().Wysokosc;
                        } 

                        temp.Clear();
                        temp2.Clear();
                        temp3.Clear();
                    }
                }
                try
                {
                    db.SubmitChanges();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    // Provide for exceptions.
                }


                //var query =
                //    from zl in db.Zlecenies
                //    select zl;
                //dataGridView1.DataSource = query;
                db.Connection.Close();
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            groupBox1.Visible = true;
            comboBox1.Items.Clear();
            comboBox1.Items.Add("Przyjęte");
            comboBox1.Items.Add("W trakcie realizacji");
            comboBox1.Items.Add("Reklamacja");
            comboBox1.Items.Add("Zrealizowane");

        }

        private void button10_Click(object sender, EventArgs e)
        {
            groupBox1.Visible = false;
            var x = dataGridView2.SelectedCells;
            int b = x[0].RowIndex;

            using (DataClasses1DataContext db = new DataClasses1DataContext(CiagPolaczenia))
            {               
                var query =
                from aa in db.Zlecenies
                where aa.ZlecenieID == (int)dataGridView2[0, b].Value
                select aa;

                foreach (Zlecenie up in query)
                {
                    up.TerminZakonczenia = dateTimePicker1.Value;
                    up.StanZlecenia = comboBox1.SelectedItem.ToString() ;
                }


                try
                {
                    db.SubmitChanges();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);

                }
            }

            using (DataClasses1DataContext db = new DataClasses1DataContext(CiagPolaczenia))
            {
                var subquery =
                    from lok in db.Lokalizacjas
                    from rWl in db.ReklamaWLokalizacjis
                    where rWl.LokalizacjaID == lok.LokalizacjaID
                    select new { rWl.ReklamaID, lok.Opis, lok.Szerokosc, lok.Wysokosc, lok.WolneMiejsce };

                var query =
                    from zl in db.Zlecenies
                    from pr in db.Pracowniks
                    from kl in db.Klients
                    from rek in db.Reklamas
                    from lok in subquery
                    where zl.ReklamaID == rek.ReklamaID
                    where zl.KlientID == kl.KlientID
                    where zl.PracownikID == pr.PracownikID
                    where rek.ReklamaID == lok.ReklamaID
                    select new { zl.ZlecenieID, Pracownik = pr.Imie + " " + pr.Nazwisko, NazwaKlienta = kl.Nazwa, OpisReklamy = rek.Opis, WymiaryReklamy = rek.Szerokosc + "x" + rek.Wysokosc, Lokalizacja = "Opis: " + lok.Opis + ", Wymiary: " + lok.Szerokosc + "x" + lok.Wysokosc + ", Wolna powierzchnia: " + lok.WolneMiejsce, zl.TerminRozpoczecia, zl.TerminZakonczenia, zl.StanZlecenia };
                dataGridView2.DataSource = query;
                db.Connection.Close();
            }

            if (comboBox1.SelectedItem.ToString() == "Reklamacja")
            {                               
                using (DataClasses1DataContext db = new DataClasses1DataContext(CiagPolaczenia))
                {
                    var query =
                        from zl in db.Zlecenies
                        from pr in db.Pracowniks
                        from kl in db.Klients
                        from rek in db.Reklamas
                        where zl.ReklamaID == rek.ReklamaID
                        where zl.KlientID == kl.KlientID
                        where zl.PracownikID == pr.PracownikID
                        where zl.ZlecenieID == (int)dataGridView2[0, b].Value
                        select new { zl.ZlecenieID, pr.PracownikID, kl.KlientID };

                    List<ReklamacjaZlecenia> rz = new List<ReklamacjaZlecenia>();
                    if (query.Any())
                    {
                        foreach (var q in query)
                        {
                            rz.Add(new ReklamacjaZlecenia { ZlecenieID = q.ZlecenieID, PracownikID = q.PracownikID, KlientID = q.KlientID });
                        }
                    }

                    TrescReklamacji reklamacja = new TrescReklamacji(rz.First().ZlecenieID, rz.First().KlientID, rz.First().PracownikID);
                    reklamacja.Show();
                    rz.Clear();
                    //Reklamacja z = new Reklamacja
                    //{
                    //    DataWystawienia = DateTime.Today,
                    //    Tresc = reklamacjaTresc,
                    //    ZlecenieID = rz.First().ZlecenieID,
                    //    KlientID = rz.First().KlientID,
                    //    PracownikID = rz.First().PracownikID
                    //};
                    //db.Reklamacjas.InsertOnSubmit(z);
                    //try
                    //{
                    //    db.SubmitChanges();
                    //}
                    //catch (Exception ex)
                    //{
                    //    Console.WriteLine(ex);
                    //        // Make some adjustments.
                    //        // ...
                    //        // Try again.
                    //    db.SubmitChanges();
                    //}                                       
                    //rz.Clear();
                }
            }
        }


        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            if (dateTimePicker1.Value < DateTime.Today)
            {
                dateTimePicker1.Value = DateTime.Today;
                MessageBox.Show("Wybrana data jest niepoprawna, spróbuj ponownie.", "Błąd");
            }
        }
        //=======
       
    }
}
