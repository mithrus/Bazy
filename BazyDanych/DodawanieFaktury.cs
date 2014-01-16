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
    public partial class DodawanieFaktury : Form
    {
        static private string CiagPolaczenia = "Data Source=(local);"
               + "Initial Catalog=ReklamaDB;"
               + "Persist Security Info=False;"
               + "User ID=ReklamaDBUser;Password=MaSeŁkOhAsEłKo";
        public Form1 forma;
        public DodawanieFaktury()
        {
            forma = Form1.ActiveForm as Form1;
            InitializeComponent();

            using (DataClasses1DataContext db = new DataClasses1DataContext(CiagPolaczenia))
            {
                //Pracownicy
                var query =
                    from pr in db.Pracowniks
                    select pr;
                List<Pracownik> l1 = new List<Pracownik>(query);
                for (int i = 0; i < l1.Count; i++)
                {
                    comboBox3.Items.Add(l1[i].PracownikID + " " + l1[i].Imie + " " + l1[i].Nazwisko);
                }
                //Klienci
                var query2 =
                    from kl in db.Klients
                    select kl;
                List<Klient> l2 = new List<Klient>(query2);
                for (int i = 0; i < l2.Count; i++)
                {
                    comboBox2.Items.Add(l2[i].KlientID + " " + l2[i].Nazwa);// + " " + l2[i].Adres);
                }
            }

            comboBox1.Items.Add("Gotówka");
             comboBox1.Items.Add("Gotówka i przelew");
             comboBox1.Items.Add("Przelew");
             comboBox1.Items.Add("Karta");       
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (DataClasses1DataContext db = new DataClasses1DataContext(CiagPolaczenia))
            {

                Faktura k = new Faktura
                {
                    NumerFaktury = textBox1.Text,
                    DataWystawienia = dateTimePicker1.Value,
                    DataSprzedazy = dateTimePicker2.Value,
                    TerminPlatnosci=dateTimePicker3.Value,
                    SposobPlatnosci= comboBox1.SelectedItem.ToString(),
                    Zaplacona=false,
                    KlientID = int.Parse(comboBox2.SelectedItem.ToString().ElementAt(0).ToString()),
                    PracownikID = int.Parse(comboBox3.SelectedItem.ToString().ElementAt(0).ToString()),
                    Kwota = int.Parse(textBox2.Text)
                };
                db.Fakturas.InsertOnSubmit(k);

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
            }
        }
    }
}
