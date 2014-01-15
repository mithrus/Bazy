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
    public partial class DodajPracownika : Form
    {
        public Form1 forma;
        static private string CiagPolaczenia = "Data Source=(local);"
              + "Initial Catalog=ReklamaDB;"
              + "Persist Security Info=False;"
              + "User ID=ReklamaDBUser;Password=MaSeŁkOhAsEłKo";
        private bool poprawneDane = false;

        public DodajPracownika() // konstruktor klasy 
        {
            forma = Form1.ActiveForm as Form1;
            InitializeComponent();

            textBox1.BackColor = Color.Red;
            textBox2.BackColor = Color.Red;
            textBox3.BackColor = Color.Red;

            textBox1.TextChanged += textBox1_TextChanged;
            textBox2.TextChanged += textBox2_TextChanged;
            textBox3.TextChanged += textBox3_TextChanged;

            UstawListeStanowisk();

        }

        void textBox3_TextChanged(object sender, EventArgs e) // pole wymagane 
        {
            if (textBox3.TextLength > 0)
            {
                textBox3.BackColor = Color.White;
                poprawneDane = true;
            }
            else
            {
                textBox3.BackColor = Color.Red;
                poprawneDane = false;
            }
        }

        void textBox2_TextChanged(object sender, EventArgs e) // pole wymagane 
        {
            if (textBox2.TextLength > 0)
            {
                textBox2.BackColor = Color.White;
                poprawneDane = true;
            }
            else
            {
                textBox2.BackColor = Color.Red;
                poprawneDane = false;
            }
        }

        void textBox1_TextChanged(object sender, EventArgs e) // pole wymagane 
        {
            if (textBox1.TextLength > 0)
            {
                textBox1.BackColor = Color.White;
                poprawneDane = true;
            }
            else
            {
                textBox1.BackColor = Color.Red;
                poprawneDane = false;
            }
        }

        private void button1_Click(object sender, EventArgs e) // dodanie pracownika 
        {
            try
            {
                //sprawdzenie poprawności danych
                if (poprawneDane)
                {
                    int dodano = 0; //ile wierszy dodano
                    using (SqlConnection db = new SqlConnection(CiagPolaczenia))
                    {
                        db.Open();
                        SqlCommand cmd = db.CreateCommand();
                        cmd.CommandText = "insert into Pracownik values ( @imie, @nazwisko, @adres, @stanowisko )";
                        cmd.Parameters.Add(new SqlParameter("@imie", textBox1.Text));
                        cmd.Parameters.Add(new SqlParameter("@nazwisko", textBox2.Text));
                        cmd.Parameters.Add(new SqlParameter("@adres", textBox3.Text + ", " + textBox4.Text + " " + textBox5.Text + "/" + textBox6.Text));
                        cmd.Parameters.Add(new SqlParameter("@stanowisko", comboBox1.Text));
                        dodano = cmd.ExecuteNonQuery();
                        db.Close();
                    }
                    if (dodano == 1)
                    {
                        this.DialogResult = System.Windows.Forms.DialogResult.OK;
                        forma.WyswietlPracownikow();
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Nie można utworzyć nowego pracownika!", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }//dane poprawne
            }
            catch (SqlException exc)
            {
                MessageBox.Show(exc.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UstawListeStanowisk() // ściąga stanowiska użyte w bazie i dodaje je do edytowalnego comboBoxa 
        {
            using (SqlConnection db = new SqlConnection(CiagPolaczenia))
            {
                string query = "select distinct(Stanowisko) from Pracownik";
                SqlDataAdapter dataAdapter = new SqlDataAdapter(query, db);
                DataSet dataSet = new DataSet();
                dataAdapter.Fill(dataSet, "Pracownik");
                List<string> lista = new List<string>();
                foreach (DataRow row in dataSet.Tables["Pracownik"].Rows)
                {
                    lista.Add(row[0].ToString());
                }
                comboBox1.DataSource = lista;
            }
        }

        private void button2_Click(object sender, EventArgs e) // anuluj 
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }


    }
}
