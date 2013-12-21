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

        private void logowanieToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            

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
            }
            else if (KlientEdit == true)
            {
                dataGridView3.ReadOnly = true ;
                button6.Text = "Edytuj";
                KlientEdit = false;
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
        //=======
       
    }
}
