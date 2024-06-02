using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Data.OleDb;
using System.Net.Configuration;
using notBul;
/*personelin maasını hesaplayan ve net maası ekranda görüntüleyen program kodlarını yazınız

istenilenler: maas hesaplama işlemi dll tarafından yapılıp form üzerine geriye bilgi 
gönderecek brüt maaş ile kesintiler textboxa girip hesapla butonuna tıklandıgı zaman 
hesaplama islemi gerceklesecek her hesaplanan personelin ad soyad tckn brüt maası 
kesintiler ve net ücret buton tıklamasıyla textboxlardan veri tabanına kaydedilecek
buton tıklamasıyla veri tabanındaki tüm veriler adı maas olan xml dosyasına gönderilecek 
buton tıklamasıyla xml de kayıtlı olan veriler datagridview üzerinde görüntülenecek yine 
buton tıklamasıyla xmldek, veriler listviewdan alan baslıklarıyla birlikte görüntülensin 
buton tıklamasıyla xml dosyasındaki veriler listbox icinde görüntülensin
*/
namespace FinSonsoru
{
    public partial class Form1 : Form
    {
        OleDbConnection baglanti = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0; Data Source="+Application.StartupPath+"/acc1.mdb");
        OleDbCommand kmt;
        BindingSource bs1 = new BindingSource();

        public Form1()
        {
            InitializeComponent();
        }
        public void ortHesapla()
        {
            Class1 ortBul = new Class1();
            textBox8.Text = Convert.ToString(ortBul.sonuc(Convert.ToDouble(textBox6.Text), Convert.ToDouble(textBox7.Text)));
        }

        private void button6_Click(object sender, EventArgs e)
        {
            ortHesapla();
        }
        private void vericek()
        {
            DataSet ds = new DataSet(); 
            OleDbDataAdapter da = new OleDbDataAdapter("select*from bilgilerim",baglanti);
            da.Fill(ds,"asd");
            bs1.DataSource = ds;
            bs1.DataMember = ds.Tables[0].TableName;
            dataGridView1.DataSource = bs1;
        }
        private void vericek2()
        {
            vericek();
            dataGridView1.DataSource = bs1;
            textBox1.DataBindings.Add("text", bs1, "id");
            textBox2.DataBindings.Add("text", bs1, "adi");
            textBox3.DataBindings.Add("text", bs1, "soyadi");
            textBox4.DataBindings.Add("text", bs1, "tc");
            textBox5.DataBindings.Add("text", bs1, "dersad");
            textBox6.DataBindings.Add("text", bs1, "vize");
            textBox7.DataBindings.Add("text", bs1, "final");
            textBox8.DataBindings.Add("text", bs1, "ortalama");
        }


        private void button1_Click(object sender, EventArgs e)
        {
            baglanti.Open();
            kmt = new OleDbCommand("Insert into bilgilerim values(@a1,@a2,@a3,@a4,@a5,@a6,@a7,@a8)",baglanti);
            kmt.Parameters.AddWithValue("@a1", textBox1.Text);
            kmt.Parameters.AddWithValue("@a2", textBox2.Text);
            kmt.Parameters.AddWithValue("@a3", textBox3.Text);
            kmt.Parameters.AddWithValue("@a4", textBox4.Text);
            kmt.Parameters.AddWithValue("@a5", textBox5.Text);
            kmt.Parameters.AddWithValue("@a6", textBox6.Text);
            kmt.Parameters.AddWithValue("@a7", textBox7.Text);
            kmt.Parameters.AddWithValue("@a8", textBox8.Text);

            ortHesapla();
            kmt.ExecuteNonQuery();
            baglanti.Close();
            MessageBox.Show("KAYIT BAŞARILI");
            vericek();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            vericek();
            vericek2();
            list();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            XmlDocument dos = new XmlDocument();
            dos.Load(Application.StartupPath + "\\kayit.xml");
            for (int i = 0; i < dataGridView1.Rows.Count-1; i++)
            {
                XmlElement ogrenci = dos.CreateElement("ogrenci");
                ogrenci.SetAttribute("id", dataGridView1.Rows[i].Cells[0].Value.ToString());

                XmlNode adi = dos.CreateNode(XmlNodeType.Element, "adi", "");
                adi.InnerText = dataGridView1.Rows[i].Cells[1].Value.ToString();
                ogrenci.AppendChild(adi);

                XmlNode soyadi = dos.CreateNode(XmlNodeType.Element, "soyadi", "");
                soyadi.InnerText = dataGridView1.Rows[i].Cells[2].Value.ToString();
                ogrenci.AppendChild(soyadi);


                XmlNode tc= dos.CreateNode(XmlNodeType.Element, "tc", "");
                tc.InnerText = dataGridView1.Rows[i].Cells[3].Value.ToString();
                ogrenci.AppendChild(tc);

                XmlNode dersad= dos.CreateNode(XmlNodeType.Element, "dersad", "");
                dersad.InnerText = dataGridView1.Rows[i].Cells[4].Value.ToString();
                ogrenci.AppendChild(dersad);

                XmlNode vize = dos.CreateNode(XmlNodeType.Element, "vize", "");
                vize.InnerText = dataGridView1.Rows[i].Cells[5].Value.ToString();
                ogrenci.AppendChild(vize);
                
                XmlNode final = dos.CreateNode(XmlNodeType.Element, "final", "");
                final.InnerText = dataGridView1.Rows[i].Cells[6].Value.ToString();
                ogrenci.AppendChild(final);

                XmlNode ortalama = dos.CreateNode(XmlNodeType.Element, "ortalama", "");
                ortalama.InnerText = dataGridView1.Rows[i].Cells[7].Value.ToString();
                ogrenci.AppendChild(ortalama);

                dos.DocumentElement.AppendChild(ogrenci);
                dos.Save(Application.StartupPath + "\\kayit.xml");
            }
            MessageBox.Show("KAYITLAR XML'E TAŞINDI");
        }

        private void kayitAçToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bs1.AddNew();
        }
        private XmlElement CreateElement(XmlDocument xmlDoc, string elementName, string value)
        {
            XmlElement element = xmlDoc.CreateElement(elementName);
            element.InnerText = value;
            return element;
        }
        public void list()
        {
            listView1.View = View.Details;
            listView1.GridLines = true;
            listView1.FullRowSelect = true;
            listView1.Columns.Add("id", 65);
            listView1.Columns.Add("adi", 65);
            listView1.Columns.Add("soyadi", 65);
            listView1.Columns.Add("tc", 65);
            listView1.Columns.Add("dersad", 65);
            listView1.Columns.Add("vize", 65);
            listView1.Columns.Add("final", 65);
            listView1.Columns.Add("ortalama", 65);
        }


        private void button3_Click(object sender, EventArgs e)
        {
            XmlDocument dos = new XmlDocument();
            dos.Load(Application.StartupPath + "\\kayit.xml");

            XmlNodeList b = dos.SelectNodes("/kayitlar/ogrenci");
            listView1.View = View.Details;

            foreach (XmlNode a in b) 
            {
                string id = a.Attributes["id"].Value;
                string adi = a["adi"].InnerText;
                string soyadi = a["soyadi"].InnerText;
                string tc = a["tc"].InnerText;
                string dersad = a["dersad"].InnerText;
                string vize = a["vize"].InnerText;
                string final = a["final"].InnerText;
                string ortalama = a["ortalama"].InnerText;

                ListViewItem item= new ListViewItem(id);

                item.SubItems.Add(adi);
                item.SubItems.Add(soyadi);
                item.SubItems.Add(tc);
                item.SubItems.Add(dersad);
                item.SubItems.Add(vize);
                item.SubItems.Add(final);
                item.SubItems.Add(ortalama);
                listView1.Items.Add(item);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(Application.StartupPath + "\\kayit.xml");

            DataSet ds = new DataSet();

            ds.ReadXml(new XmlNodeReader(doc));

            string[] kolonlar = { "id", "adi", "soyadi", "tc", "dersad", "vize", "final", "ortalama" };

            DataTable dt = new DataTable();

            foreach (var kolon in kolonlar)
            {
                dt.Columns.Add(kolon, typeof(string));
            }

            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                DataRow newRow = dt.NewRow();
                foreach (var kolon in kolonlar)
                {
                    newRow[kolon] = dr[kolon];
                }
                dt.Rows.Add(newRow);
            }
            dataGridView2.DataSource = dt;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            XmlDocument dos  = new XmlDocument();
            dos.Load(Application.StartupPath + "\\kayit.xml");
            XmlNodeList liste = dos.SelectNodes(@"/kayitlar/ogrenci");
            foreach (XmlNode node in liste) 
            {
                listBox1.Items.Add("id:"+node.Attributes["id"].Value);
                listBox1.Items.Add("adi:" + node["adi"].InnerText);
                listBox1.Items.Add("soyadi:" + node["soyadi"].InnerText);
                listBox1.Items.Add("tc:" + node["tc"].InnerText);
                listBox1.Items.Add("ders adı:" + node["dersad"].InnerText);
                listBox1.Items.Add("vize:"+node["vize"].InnerText);
                listBox1.Items.Add("final"+node["final"].InnerText);
                listBox1.Items.Add("ortalama"+node["ortalama"].InnerText);
                listBox1.Items.Add("----------");
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            XmlDocument dos = new XmlDocument();
            dos.Load(Application.StartupPath + "//kayit.xml");
            XmlNodeList liste = dos.SelectNodes("//kayitlar//ogrenci[id"+textBox1.Text+"]");
            foreach(XmlNode node in liste)
            {
                node.ParentNode.RemoveChild(node);
            }
            dos.Save(Application.StartupPath + "//kayit.xml");
        }
    }
}