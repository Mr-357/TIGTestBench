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
        private Stopwatch time = new Stopwatch();
        private List<Karta> Talon = new List<Karta>();
        private List<Karta> Spil = new List<Karta>();
        public Form1()
        {
            InitializeComponent();
            //  loadDLL();
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
        }
        private Type loadDLL()
        {
            Assembly assembly;

            //foreach (string f in Directory.GetFiles("./", "*.dll"))
            //{
            //  if (f != "./TIG.AV.Karte.dll" )
            //   {
            OpenFileDialog open = new OpenFileDialog();
            open.ShowDialog();
            string f = open.FileName;
            if (f != null && f != "")
            {
                assembly = Assembly.LoadFile(f);

                //   }

                //  }
                foreach (Type mytype in assembly.GetTypes()
                    .Where(mytype => mytype.GetInterfaces().Contains(typeof(IIgra))))
                {
                    return mytype;
                }
            }
          //  if(assembly.GetTypes().Where(p =>            
            return null;
            }
          
       

        private void btnTest_Click(object sender, EventArgs e)
        {
          //  IIgra igra = new _16022.MakaoBot();
            IIgra igra = (IIgra)Activator.CreateInstance(loadDLL());
            List < Karta > r = new List<Karta>();
            for (int i = 1; i <= 6; i++)
            {
                Karta k = new Karta();
                k.Boja = (Boja)(i % 4 + 1) ;
                k.Broj = i.ToString();
                r.Add(k);
            }
            igra.SetRuka(r);
            List<Karta> t = new List<Karta>();
            Karta top = new Karta();
            top.Boja = (Boja)comboBox2.SelectedItem;
            top.Broj = comboBox1.SelectedText;
            t.Add(top);
            igra.Bacenekarte(t, Boja.Unknown, 6);
            time.Start();
            igra.BeginBestMove();
            time.Stop();
            string potez = "";
            potez += igra.BestMove.Tip.ToString()+"\r\n";
            foreach (Karta k in igra.BestMove.Karte)
            {
                potez += k.Broj;
                potez += k.Boja;
                potez += "\r\n";
            }
            MessageBox.Show("zavrsio za"+time.ElapsedMilliseconds.ToString()+"ms \r\n potez: \r\n"+potez);
        }
    }
}
