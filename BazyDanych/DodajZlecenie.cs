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

        public DodajZlecenie()
        {
            forma = Form1.ActiveForm as Form1;
            InitializeComponent();

            dateTimePicker1.CustomFormat = "yyyy/dd/MM";
            dateTimePicker1.Format = DateTimePickerFormat.Custom;

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
                List<Reklama> l3 = new List<Reklama>(query3);
                for (int i = 0; i < l3.Count; i++)
                {
                    comboBox3.Items.Add(l3[i].ReklamaID+" "+l3[i].Opis + " " + l3[i].Szerokosc+"x"+l3[i].Wysokosc);
                }

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

                try
                {
                    db.SubmitChanges();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    db.SubmitChanges();
                }
            }
        }

        //=========
    }
}
