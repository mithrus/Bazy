﻿using System;
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
using System.IO;

namespace BazyDanych
{
    public partial class Form1 : Form
    {
        static private string CiagPolaczenia = "Data Source=(local);"
               + "Initial Catalog=ReklamaDB;"
               + "Persist Security Info=False;"
               + "User ID=ReklamaDBUser;Password=MaSeŁkOhAsEłKo";
      
        public Form1()
        {
            InitializeComponent();


            //dla zakładki Pracownicy
            dataGridView4.SelectionChanged += dataGridView4_SelectionChanged;
            button14.Enabled = false;            

        }
        string SciezkaKopiiZapasowej = "";
        
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
            dataGridView2.ClearSelection();
            button8.Enabled = true;
            button9.Enabled = true;
            button16PS.Enabled = true;
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

        private void button11_Click(object sender, EventArgs e) // wyświetl pracowników 
        {
            WyswietlPracownikow();
        }

        public void WyswietlPracownikow() // funkcja wyświetlania pracowników 
        {
            using (SqlConnection db = new SqlConnection(CiagPolaczenia))
            {
                string query = "select * from Pracownik";
                SqlDataAdapter dataAdapter = new SqlDataAdapter(query, db);
                DataSet dataSet = new DataSet();
                dataAdapter.Fill(dataSet, "Pracownik");
                dataGridView4.DataSource = dataSet.Tables["Pracownik"].DefaultView;
                dataGridView4.AllowUserToAddRows = false;
            }
        }

        private void button12_Click(object sender, EventArgs e) // wyszukaj pracownika 
        {

        }

        private void button13_Click(object sender, EventArgs e) // dodaj pracownika 
        {
            Form dodaj = new DodajPracownika();
            dodaj.Show();
        }

        private void button14_Click(object sender, EventArgs e) // usuń pracownika 
        {
            if (dataGridView4.SelectedRows.Count > 0)
            {//zaznaczono wiersze
                if (MessageBox.Show("Czy jesteś pewien, że chcesz usunąć zaznaczonych pracowników!", "Ostrzeżenie", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.Yes)
                {// user jest pewien, że chce usunąć pracowników
                    try
                    {
                        using (SqlConnection db = new SqlConnection(CiagPolaczenia))
                        {
                            int usunieto = 0;
                            foreach (DataGridViewRow row in dataGridView4.SelectedRows)
                            {
                                db.Open();
                                SqlCommand cmd = db.CreateCommand();
                                cmd.CommandText = "delete Pracownik where Imie=@imie and Nazwisko=@nazwisko";
                                string p1 = row.Cells[1].Value.ToString();
                                string p2 = row.Cells[2].Value.ToString();
                                cmd.Parameters.Add(new SqlParameter("@imie", p1));
                                cmd.Parameters.Add(new SqlParameter("@nazwisko", p2));

                                usunieto += cmd.ExecuteNonQuery();
                                db.Close();
                            }
                            MessageBox.Show("Pomyślnie usunięto " + usunieto + " pracownik(ów)!", "Komunikat", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        WyswietlPracownikow();
                    }
                    catch (SqlException)
                    {
                        MessageBox.Show("Pracownik, którego chcesz usunąć, jest kierownikiem jakiegoś zespołu", "Komunikat", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }




            }
            else
            {
                MessageBox.Show("Nie wybrano żadnych pracowników!", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button15_Click(object sender, EventArgs e) // modyfikuj pracownika 
        {

        }

        void dataGridView4_SelectionChanged(object sender, EventArgs e) // zmiana zaznaczenia w Pracownicy 
        {
            if (dataGridView4.SelectedRows.Count > 0)
                button14.Enabled = true;
            else
                button14.Enabled = false;
            //throw new NotImplementedException();
        }

        private void button16PS_Click(object sender, EventArgs e)
        {
            WyszukiwanieZlec szukaj = new WyszukiwanieZlec();
            szukaj.Show();
        }

        private void RBKopiaWybrane_CheckedChanged(object sender, EventArgs e)
        {
            //string dbname = "ReklamaDB";
            if (((RadioButton)sender).Checked == true)
            {
                try
                {
                    using (SqlConnection Polaczenie = new SqlConnection(CiagPolaczenia))
                    {
                        string sqlQuery = "SELECT * FROM information_schema.tables";
                        SqlDataAdapter dataAdapter = new SqlDataAdapter(sqlQuery, Polaczenie);
                        DataSet dataSet = new DataSet();
                        dataAdapter.Fill(dataSet);
                        CLBKopiaTabele.Items.Clear();
                        foreach (DataRow r in dataSet.Tables[0].Rows)
                        {
                            CLBKopiaTabele.Items.Add(r[2], false);
                        }
                        if (dataSet.Tables[0].Rows.Count > 0)
                            CLBKopiaTabele.Enabled = true;
                    }
                }
                catch (Exception ee)
                {
                    MessageBox.Show(ee.Message);
                }
            }
        }

        private void BKopiaWybierzKatalog_Click(object sender, EventArgs e)
        {
            FBDKopiaWybierz.ShowDialog();
            TBKopiaSciezkaKatalog.Text = FBDKopiaWybierz.SelectedPath;
            //SciezkaKopiiZapasowej = FBDKopiaWybierz.SelectedPath;
        }

        private void BKopiaUtworz_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(SciezkaKopiiZapasowej))
            {
            }
            else
            {
                MessageBox.Show("Wprowadzona ścieżka nie istnieje! Spróbj wybrać inną.");
            }
        }

        private void TBKopiaSciezkaKatalog_TextChanged(object sender, EventArgs e)
        {
            SciezkaKopiiZapasowej = TBKopiaSciezkaKatalog.Text;
            if (Directory.Exists(SciezkaKopiiZapasowej))
            {
                FBDKopiaWybierz.SelectedPath = SciezkaKopiiZapasowej;
            }
        }

        private void button16PS2_Click(object sender, EventArgs e)
        {           
            using (DataClasses1DataContext db = new DataClasses1DataContext(CiagPolaczenia))
            {
                var q = db.WyswietlReklamacje();
                dataGridView5PS.DataSource = q;               
            }
        }

        private void button16PS3_Click(object sender, EventArgs e)
        {
            using (DataClasses1DataContext db = new DataClasses1DataContext(CiagPolaczenia))
            {
                for (int i = 0; i < dataGridView5PS.RowCount; i++)
                {
                    var x = dataGridView5PS[0, i];
                    var idZlec = dataGridView5PS[5, i];
                    if (x.Selected == true)
                    {
                        var del =
                        from row in db.Reklamacjas
                        where row.ReklamacjaID == (int)x.Value
                        select row;

                        foreach (var detail in del)
                        {
                            db.Reklamacjas.DeleteOnSubmit(detail);
                        }

                        db.AktualizacjaDoZlecenia(Convert.ToInt32(idZlec.Value.ToString()));
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
            }
        }

        private void button16PS4_Click(object sender, EventArgs e)
        {
            using (DataClasses1DataContext db = new DataClasses1DataContext(CiagPolaczenia))
            {              
               var q= db.WyszukajReklamacjePoIDz(Convert.ToInt32(textBox1PS.Text));
               dataGridView5PS.DataSource = q;
            }
        }

        private void button16PS5_Click(object sender, EventArgs e)
        {
            NajgorszyPrac worstPrac = new NajgorszyPrac();       
            worstPrac.Show(); 
        }

       




        //=======
       
    }
}
