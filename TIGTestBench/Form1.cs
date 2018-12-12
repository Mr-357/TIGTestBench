using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TIG.AV.Karte;
//using _16022;

namespace TIGTestBench
{
    public partial class Form1 : Form
    {
        private Spil spil = new Spil();
        private Stopwatch time = new Stopwatch();
        private List<Karta> Talon = new List<Karta>();
        private List<Karta> Spil = new List<Karta>();
        private IIgra bot1;
        private IIgra bot2;
        Type loaded;
        public Form1()
        {
            InitializeComponent();
            for (int i = 2; i <= 10; i++)
            {
                comboBox1.Items.Add(i.ToString());
            }
            comboBox1.Items.Add("A");
            comboBox1.Items.Add("J");
            comboBox1.Items.Add("Q");
            comboBox1.Items.Add("K");
            comboBox2.Items.Add(Boja.Pik);
            comboBox2.Items.Add(Boja.Tref);
            comboBox2.Items.Add(Boja.Karo);
            comboBox2.Items.Add(Boja.Herz);
            spil = new Spil();
            spil.Promesaj();
        }
        private Type loadDLL()
        {
            Assembly assembly;
            OpenFileDialog open = new OpenFileDialog();
            open.ShowDialog();
            string f = open.FileName;
            if (f != null && f != "")
            {
                assembly = Assembly.LoadFile(f);
                foreach (Type mytype in assembly.GetTypes()
                .Where(mytype => mytype.GetInterfaces().Contains(typeof(IIgra))))
                {
                   loaded= mytype;
                }
            }
            return null;
            }


        private string ListaKarataToString(List<Karta> k)
        {
            string ret = "";
            foreach (Karta karta in k)
            {
                ret += karta.Boja + karta.Broj + " ";
            }
            return ret;
        }
        private void btnTest_Click(object sender, EventArgs e)
        {
            
            if (loaded == null)
                throw new Exception("nema bot(ova)");

            List < Karta > r = new List<Karta>();
             for (int i = 0; i <= 5; i++)
             {
                Karta k = spil.Karte.Last();
                spil.Karte.Remove(k);
                r.Add(k);
             }
            bot1.SetRuka(r);
            label5.Text = ListaKarataToString(r);
            Karta top;
            List<Karta> t = new List<Karta>();
            if (checkBox2.Checked)
            {
                //dodaj proveru da izbaci warning ako forcujes kartu koja je vec prosla
                top = new Karta();
                top.Boja = (Boja)comboBox2.SelectedItem;
                top.Broj = comboBox1.SelectedItem.ToString();
            }
            else
            {
                top = spil.Karte.Last();
                spil.Karte.Remove(top);
            }
            label6.Text = top.Boja.ToString() + top.Broj.ToString();
            t.Add(top);
            bot1.Bacenekarte(t, top.Boja, 6);
            time.Start();
            timer1.Start();
  
            bot1.BeginBestMove();

            if (checkBox1.Checked)
            {
                timer1.Stop();
                ShowResults();
            }
        }


        private void timer1_Tick(object sender, EventArgs e)
        {

            timer1.Stop();
            ShowResults();
        }

        private void ShowResults()
        {
            bot1.EndBestMove();
            time.Stop();
            string potez = "";
            potez += bot1.BestMove.Tip.ToString() + "\r\n";
            foreach (Karta k in bot1.BestMove.Karte)
            {
                potez += k.Broj;
                potez += k.Boja;
                potez += "\r\n";
            }
            string totaltime;
            if (checkBox1.Checked)
            {
                totaltime = time.ElapsedMilliseconds.ToString();
            }
            else
            {
                totaltime = (time.ElapsedMilliseconds - timer1.Interval).ToString() ; // tehnicki vreme za zvanje endbestmove-a,treba da bude 0 ili <0 kad bi C# stopwatch radio kako treba
            }
            MessageBox.Show("zavrsio za" + totaltime + "ms \r\n potez: \r\n" + potez);
            time.Reset();
        }

        private void botToolStripMenuItem_Click(object sender, EventArgs e)
        {
            loadDLL();
            bot1 = (IIgra)Activator.CreateInstance(loaded);
        }
    }
}
