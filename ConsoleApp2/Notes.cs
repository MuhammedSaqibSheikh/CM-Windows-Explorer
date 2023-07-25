using System;
using System.Windows.Forms;
using TRIM.SDK;

namespace ConsoleApp2
{
    public partial class Notes : Form
    {
        public Notes()
        {
            InitializeComponent();
        }

        Program p = new Program();

        public virtual string ClsNumber { get; set; }

        private void Notes_Load(object sender, EventArgs e)
        {
            //load notes from CM
            p.ConnectDb();
            TrimMainObjectSearch cls = new TrimMainObjectSearch(p.db, BaseObjectTypes.Classification);
            cls.SetSearchString("number:" + ClsNumber);
            foreach (Classification clsRec in cls)
            {
                txtnotes.Text = clsRec.Notes;
            }
        }

        public void LoadEditor(String name, String number)
        {
            this.Text = name + " - " + number + " - Notes";
            ClsNumber = number;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //Save notes
            TrimMainObjectSearch cls = new TrimMainObjectSearch(p.db, BaseObjectTypes.Classification);
            cls.SetSearchString("number:" + ClsNumber);
            foreach (Classification clsRec in cls)
            {
                clsRec.Notes = txtnotes.Text;
                clsRec.Save();
            }
            this.Close();
        }

        private void btnclear_Click(object sender, EventArgs e)
        {
            txtnotes.Text = "";
        }

        private void btnstamp_Click(object sender, EventArgs e)
        {
            txtnotes.Text += "\t" + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
        }

        private void btncancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}