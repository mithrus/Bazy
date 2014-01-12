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
    public partial class DodajZlecenie : Form
    {
        public Form1 forma;
        static private string CiagPolaczenia = "Data Source=(local);"
              + "Initial Catalog=ReklamaDB;"
              + "Persist Security Info=False;"
              + "User ID=ReklamaDBUser;Password=MaSeŁkOhAsEłKo";
        List<Reklama> l3;
        List<Lokalizacja> l4 = null;//lokalizacja
        List<ReklamyDoWyswietlenia> rekWlok; //reklamy w danej lokalizacji do wyświetlenia      
        //List<Reklama> rek;//reklamy do wyświetlenia w danej lokalizacji
        float skala;
        int x, y,w,h;
        bool czyWyswietlac = false; //zmienna pomocnicza ustawiana na true jesli ma byc wyswietlana reklama(zielona) do dodania

        public DodajZlecenie()
        {
            forma = Form1.ActiveForm as Form1;
            InitializeComponent();

            dateTimePicker1.CustomFormat = "yyyy/dd/MM";
            dateTimePicker1.Format = DateTimePickerFormat.Custom;
            comboBox4.Enabled = false;
           

            using (DataClasses1DataContext db = new DataClasses1DataContext(CiagPolaczenia))
            {
                //Pracownicy
                var query =
                    from pr in db.Pracowniks
                    select pr;
                List<Pracownik> l1 = new List<Pracownik>(query);
                for (int i = 0; i < l1.Count; i++)
                {
                    comboBox1.Items.Add(l1[i].PracownikID+" "+l1[i].Imie + " " + l1[i].Nazwisko);
                }
                //Klienci
                var query2 =
                    from kl in db.Klients
                    select kl;
                List<Klient> l2 = new List<Klient>(query2);
                for (int i = 0; i < l2.Count; i++)
                {
                    comboBox2.Items.Add(l2[i].KlientID+" "+l2[i].Nazwa);// + " " + l2[i].Adres);
                }

                //Reklamy
                var query3 =
                   from re in db.Reklamas
                   select re;
                l3 = new List<Reklama>(query3);
                for (int i = 0; i < l3.Count; i++)
                {
                    comboBox3.Items.Add(l3[i].ReklamaID+" "+l3[i].Opis + " " + l3[i].Szerokosc+"x"+l3[i].Wysokosc);
                }
                //Zespol
                //var query
               
                //typ powierzchni
                comboBox5.Items.Add("Słup ogłoszeniowy");
                comboBox5.Items.Add("Tramwaj");
                comboBox5.Items.Add("Budynek");
                comboBox5.Items.Add("Bilobard");
                comboBox5.Items.Add("Ogrodzenie");

                

                    db.Connection.Close();
            }

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //char x = comboBox1.SelectedItem.ToString().ElementAt(0);
           // DateTime t;
            
            using (DataClasses1DataContext db = new DataClasses1DataContext(CiagPolaczenia))
            {
                
                Zlecenie z = new Zlecenie
                {
                    PracownikID=int.Parse(comboBox1.SelectedItem.ToString().ElementAt(0).ToString()),
                    KlientID=int.Parse(comboBox2.SelectedItem.ToString().ElementAt(0).ToString()),
                    ReklamaID=int.Parse(comboBox3.SelectedItem.ToString().ElementAt(0).ToString()),
                    TerminRozpoczecia=DateTime.Today,//dateTimePicker1.Value,
                    TerminZakonczenia=dateTimePicker1.Value,
                    StanZlecenia="Przyjeto"
                };
                db.Zlecenies.InsertOnSubmit(z);

                ReklamaWLokalizacji rwl = new ReklamaWLokalizacji
                {
                    ReklamaID = l3[comboBox3.SelectedIndex].ReklamaID,
                    LokalizacjaID = l4[comboBox4.SelectedIndex].LokalizacjaID,
                    Wspolrzedna_X=x,
                    Wspolrzedna_Y=y
                };
                db.ReklamaWLokalizacjis.InsertOnSubmit(rwl);

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
            //wyswietlenie wynikow w tabeli
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
                    select new { zl.ZlecenieID, Pracownik = pr.Imie + " " + pr.Nazwisko, NazwaKlienta = kl.Nazwa, OpisReklamy = rek.Opis, WymiaryReklamy = rek.Szerokosc + "x" + rek.Wysokosc, zl.TerminRozpoczecia, zl.TerminZakonczenia, zl.StanZlecenia };
                forma.dataGridView2.DataSource = query;
                db.Connection.Close();
            }

            this.Close();
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            panel1.BackColor = Color.White;
            panel1.Visible = true;
            panel1.Height = 550;
            panel1.Width = 550;

            int idx = comboBox4.SelectedIndex;
            if (l4[idx].Szerokosc >= l4[idx].Wysokosc)
            {
                skala = (float)panel1.Width / (float)l4[idx].Szerokosc;
                panel1.Height = (int)(skala * (float)l4[idx].Wysokosc);
            }
            else
            {
                skala = (float)panel1.Width / (float)l4[idx].Wysokosc;
            }

            if (czyWyswietlac == true)
            {
                w = (int)((float)l3[comboBox3.SelectedIndex].Szerokosc * skala);
                h = (int)((float)l3[comboBox3.SelectedIndex].Wysokosc * skala);
            }

            //wybieranie reklam w danej lokalizajci
            using (DataClasses1DataContext db = new DataClasses1DataContext(CiagPolaczenia))
            {                
                var query =
                    from rwl in db.ReklamaWLokalizacjis
                    from r in db.Reklamas
                    where rwl.LokalizacjaID == l4[comboBox4.SelectedIndex].LokalizacjaID
                    where r.ReklamaID == rwl.ReklamaID
                    select new {r.ReklamaID, r.Szerokosc,r.Wysokosc,rwl.Wspolrzedna_X,rwl.Wspolrzedna_Y };       
      
               rekWlok = new List<ReklamyDoWyswietlenia>();
               if (query.Any())
               {
                   foreach (var q in query)
                   {
                       rekWlok.Add(new ReklamyDoWyswietlenia { ReklamaID = q.ReklamaID, Szerokosc = (int)q.Szerokosc, Wysokosc = (int)q.Wysokosc, Wspolrzedna_X = (int)q.Wspolrzedna_X, Wspolrzedna_Y = (int)q.Wspolrzedna_Y });
                   }
               }
                db.Connection.Close();
            }

            panel1.Invalidate();
        }

        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox4.Items.Clear();
            comboBox4.Text = "Wybierz";
            using (DataClasses1DataContext db = new DataClasses1DataContext(CiagPolaczenia))
            {
                //Lokalizacja
                if (comboBox5.SelectedIndex > -1)
                {                  
                    
                    if (comboBox5.SelectedIndex == 0)//slupOgloszeniowy
                    {
                        comboBox4.Enabled = true;
                        var query4 =
                             from lok in db.Lokalizacjas
                             from sl in db.SlupOgloszeniowies
                             where lok.LokalizacjaID == sl.LokalizacjaID
                             select lok;
                        l4 = new List<Lokalizacja>(query4);
                    }
                    if (comboBox5.SelectedIndex == 1)//tramwaj
                    {
                        comboBox4.Enabled = true;
                        var query4 =
                             from lok in db.Lokalizacjas
                             from tr in db.Tramwajs
                             where lok.LokalizacjaID == tr.LokalizacjaID
                             select lok;
                        l4 = new List<Lokalizacja>(query4);
                    }
                    if (comboBox5.SelectedIndex == 2)//budynek
                    {
                        comboBox4.Enabled = true;
                        var query4 =
                             from lok in db.Lokalizacjas
                             from bud in db.Budyneks
                             where lok.LokalizacjaID == bud.LokalizacjaID
                             select lok;
                        l4 = new List<Lokalizacja>(query4);
                    }
                    if (comboBox5.SelectedIndex == 3)//Bilboard
                    {
                        comboBox4.Enabled = true;
                        var query4 =
                             from lok in db.Lokalizacjas
                             from bil in db.Bilboards
                             where lok.LokalizacjaID == bil.LokalizacjaID
                             select lok;
                        l4 = new List<Lokalizacja>(query4);
                    }
                    if (comboBox5.SelectedIndex == 4)//Ogrodzenie
                    {
                        comboBox4.Enabled = true;
                        var query4 =
                             from lok in db.Lokalizacjas
                             from og in db.Ogordzenies
                             where lok.LokalizacjaID == og.LokalizacjaID
                             select lok;
                        l4 = new List<Lokalizacja>(query4);
                    }
                    for (int i = 0; i < l4.Count; i++)
                    {
                       comboBox4.Items.Add(l4[i].LokalizacjaID + " " + l4[i].Opis);//+ " " + l4[i].Szerokosc + "x" + l4[i].Wysokosc);
                    }
                    
                }
                else
                    comboBox4.Enabled = false;

                db.Connection.Close();
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            //Graphics g = e.Graphics;   
            using (Font font1 = new Font("Arial", 12, FontStyle.Bold, GraphicsUnit.Point))
            {
                Graphics gr = panel1.CreateGraphics();
               
                //wyswietlanie reklam z w danej lokalizacji
                SolidBrush lightBlue = new SolidBrush(Color.LightBlue);
                if (rekWlok != null)
                {
                    for (int i = 0; i < rekWlok.Count; i++)
                    {
                        Rectangle adv = new Rectangle(rekWlok.ElementAt(i).Wspolrzedna_X, rekWlok.ElementAt(i).Wspolrzedna_Y, (int)((float)rekWlok.ElementAt(i).Szerokosc * skala), (int)((float)rekWlok.ElementAt(i).Wysokosc * skala));
                        gr.DrawRectangle(new Pen(new SolidBrush(Color.LightBlue), 1), adv);
                        gr.FillRectangle(lightBlue, adv);
                        gr.DrawString(rekWlok.ElementAt(i).ReklamaID.ToString(), font1, Brushes.Black, adv);
                    }
                }

                //wyświetlanie zielonego prostokąta
                if (czyWyswietlac == true)
                {
                    Rectangle c;
                    if (x + w > panel1.Width)
                        x = panel1.Width - w;
                    if (y + h > panel1.Height)
                        y = panel1.Height - h;
                    if (x < 0)
                        x = 0;
                    if (y < 0)
                        y = 0;
                    c = new Rectangle(x, y, w, h);
                    SolidBrush green = new SolidBrush(Color.Green);
                    gr.FillRectangle(green, c);
                    gr.DrawString(l3[comboBox3.SelectedIndex].ReklamaID.ToString(), font1, Brushes.Black, c);
                    gr.DrawRectangle(new Pen(new SolidBrush(Color.Green), 1), c);
                }

            }
             
        }

        private void panel1_MouseCaptureChanged(object sender, EventArgs e)
        {
            // MouseEventArgs m = ;
            Point p = panel1.PointToClient(Cursor.Position);//new Point(m.X, m.Y);
            x = p.X;
            y = p.Y;
            panel1.Invalidate();

        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            czyWyswietlac = true;
        }
      

        //private void panel1_MouseClick(object sender, MouseEventArgs e)
        //{
        //    Point p = new Point(e.X, e.Y);
        //    x = p.X;
        //    y = p.Y;
        //    panel1.Invalidate();
        //    label7.Text = x.ToString();
        //    label8.Text = y.ToString();
        //}

        //=========
    }
}
