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
    public partial class NajgorszyPrac : Form
    {
        public Form1 forma;
        static private string CiagPolaczenia = "Data Source=(local);"
               + "Initial Catalog=ReklamaDB;"
               + "Persist Security Info=False;"
               + "User ID=ReklamaDBUser;Password=MaSeŁkOhAsEłKo";
        public NajgorszyPrac()
        {
            forma = Form1.ActiveForm as Form1;
            InitializeComponent();

            using (DataClasses1DataContext db = new DataClasses1DataContext(CiagPolaczenia))
            {
                int? max=0;
                var q = db.WyszukajNajgorszegoPrac(ref max);
                dataGridView1.DataSource = q;

            }

        }
    }
}
