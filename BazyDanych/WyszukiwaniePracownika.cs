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
    public partial class WyszukiwaniePracownika : Form
    {
        static private string CiagPolaczenia = "Data Source=(local);"
               + "Initial Catalog=ReklamaDB;"
               + "Persist Security Info=False;"
               + "User ID=ReklamaDBUser;Password=MaSeŁkOhAsEłKo";

        public WyszukiwaniePracownika()
        {
            InitializeComponent();

            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;

            UstawListeImion();
            UstawListeNazwisk();
            UstawListeAdresow();
            UstawListeStanowisk();

        }

        private void button1_Click(object sender, EventArgs e) // wyszukiwanie po id_pracownika 
        {
            SqlConnection db = new SqlConnection(CiagPolaczenia);
            try
            {                
                db.Open();

                SqlCommand cmd = new SqlCommand("ZnajdzPracownikaPoId", db);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.Int, 5, ParameterDirection.Output, false, 0, 50, "PracownikID", DataRowVersion.Default, null));
                cmd.Parameters.Add(new SqlParameter("@imie", SqlDbType.VarChar, 20, ParameterDirection.Output, false, 0, 50, "Imie", DataRowVersion.Default, null));
                cmd.Parameters.Add(new SqlParameter("@nazwisko", SqlDbType.VarChar, 50, ParameterDirection.Output, false, 0, 50, "Nazwisko", DataRowVersion.Default, null));
                cmd.Parameters.Add(new SqlParameter("@adres", SqlDbType.VarChar, -1, ParameterDirection.Output, false, 0, 50, "Adres", DataRowVersion.Default, null));
                cmd.Parameters.Add(new SqlParameter("@stanowisko", SqlDbType.VarChar, 30, ParameterDirection.Output, false, 0, 50, "Stanowisko", DataRowVersion.Default, null));

                cmd.Parameters.Add(new SqlParameter("@pracownikID", numericUpDown1.Value));

                cmd.ExecuteNonQuery();

                DataTable set = new DataTable();
                set.Columns.Add("PracownikID", typeof(int));
                set.Columns.Add("Imie", typeof(string));
                set.Columns.Add("Nazwisko", typeof(string));
                set.Columns.Add("Adres", typeof(string));
                set.Columns.Add("Stanowisko", typeof(string));

                set.Rows.Add(cmd.Parameters[0].Value, cmd.Parameters[1].Value, cmd.Parameters[2].Value, cmd.Parameters[3].Value, cmd.Parameters[4].Value);
                
                dataGridView1.DataSource = set.DefaultView;                

            }
            catch (Exception exc)
            {
                MessageBox.Show("błąd!\n" + exc.Message, "Komunikat", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                db.Close();
            }

        }

        private void button2_Click(object sender, EventArgs e) // wyszukiwanie po innych parametrach 
        {
            SqlConnection db = new SqlConnection(CiagPolaczenia);
            try
            {
                db.Open();

                string query = @"select * from Pracownik
                                    where Imie like @imie and Nazwisko like @nazwisko and
                                    Adres like @adres and Stanowisko like @stanowisko";
                SqlDataAdapter adapter = new SqlDataAdapter(query, db);
                adapter.SelectCommand.Parameters.AddWithValue("@imie", comboBox1.Text == "" ? "%" : "%" + comboBox1.Text + "%");
                adapter.SelectCommand.Parameters.AddWithValue("@nazwisko", comboBox2.Text == "" ? "%" : "%"+comboBox2.Text+"%");
                adapter.SelectCommand.Parameters.AddWithValue("@adres", comboBox3.Text == "" ? "%" : "%" + comboBox3.Text + "%");
                adapter.SelectCommand.Parameters.AddWithValue("@stanowisko", comboBox4.Text == "" ? "%" : "%" + comboBox4.Text + "%");

                DataSet set = new DataSet();
                adapter.Fill(set, "Pracownik");

                dataGridView1.DataSource = set.Tables[0].DefaultView;

            }
            catch (Exception exc)
            {
                MessageBox.Show("błąd!\n" + exc.Message, "Komunikat", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                db.Close();
            }

        }

        private void button3_Click(object sender, EventArgs e) // resetuj wyszukiwanie 
        {
            dataGridView1.DataSource = null;
            comboBox1.SelectedIndex = 0;
            comboBox1.Text = "";
            comboBox2.SelectedIndex = 0;
            comboBox2.Text = "";
            comboBox3.SelectedIndex = 0;
            comboBox3.Text = "";
            comboBox4.SelectedIndex = 0;
            comboBox4.Text = "";

            

        }

        private void button4_Click(object sender, EventArgs e) // zamknij forme 
        {
            this.Close();
        }

        private void UstawListeImion()
        {
            using (SqlConnection db = new SqlConnection(CiagPolaczenia))
            {
                string query = "select distinct(Imie) from Pracownik";
                SqlDataAdapter dataAdapter = new SqlDataAdapter(query, db);
                DataSet dataSet = new DataSet();
                dataAdapter.Fill(dataSet, "Pracownik");
                List<string> lista = new List<string>();
                lista.Add("");
                foreach (DataRow row in dataSet.Tables["Pracownik"].Rows)
                {
                    lista.Add(row[0].ToString());
                }
                comboBox1.DataSource = lista;
            }
        }

        private void UstawListeNazwisk()
        {
            using (SqlConnection db = new SqlConnection(CiagPolaczenia))
            {
                string query = "select distinct(Nazwisko) from Pracownik";
                SqlDataAdapter dataAdapter = new SqlDataAdapter(query, db);
                DataSet dataSet = new DataSet();
                dataAdapter.Fill(dataSet, "Pracownik");
                List<string> lista = new List<string>();
                lista.Add("");
                foreach (DataRow row in dataSet.Tables["Pracownik"].Rows)
                {
                    lista.Add(row[0].ToString());
                }
                comboBox2.DataSource = lista;
            }
        }

        private void UstawListeAdresow()
        {
            using (SqlConnection db = new SqlConnection(CiagPolaczenia))
            {
                string query = "select distinct(Adres) from Pracownik";
                SqlDataAdapter dataAdapter = new SqlDataAdapter(query, db);
                DataSet dataSet = new DataSet();
                dataAdapter.Fill(dataSet, "Pracownik");
                List<string> lista = new List<string>();
                lista.Add("");
                foreach (DataRow row in dataSet.Tables["Pracownik"].Rows)
                {
                    lista.Add(row[0].ToString());
                }
                comboBox3.DataSource = lista;
            }
        }

        private void UstawListeStanowisk()
        {
            using (SqlConnection db = new SqlConnection(CiagPolaczenia))
            {
                string query = "select distinct(Stanowisko) from Pracownik";
                SqlDataAdapter dataAdapter = new SqlDataAdapter(query, db);
                DataSet dataSet = new DataSet();
                dataAdapter.Fill(dataSet, "Pracownik");
                List<string> lista = new List<string>();
                lista.Add("");
                foreach (DataRow row in dataSet.Tables["Pracownik"].Rows)
                {
                    lista.Add(row[0].ToString());
                }
                comboBox4.DataSource = lista;
            }
        }
    }
}
