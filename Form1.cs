using System;
using System.Windows.Forms;

/**
 *   Geri Yayılımlı Yapay Sinir Ağı Örneği
 *   Hüseyin Atasoy
 *   03/04/2011
 *   www.atasoyweb.net
 **/

namespace YapaySinirAgi
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private float ogrenmeKatsayisi = 0.9f;
        private float momentum = 0.5f;

        private float noron1, noron2;

        private float noron3, noron4;
        private float delta3, delta4;

        private float noron5;
        private float delta5;

        private float dendrit31, dendrit32, dendrit3b;
        private float degisim31, degisim32, degisim3b;
        private float dendrit41, dendrit42, dendrit4b;
        private float degisim41, degisim42, degisim4b;

        private float dendrit53, dendrit54, dendrit5b;
        private float degisim53, degisim54, degisim5b;

        private Random rastgeleUretec = new Random();

        private float rastgele()
        {
            return (float)rastgeleUretec.Next(1, 999) / 1000.0f; // 0.001 ve 0.999 arasında rastgele sayılar.
        }

        private float aktivasyon(float x) // Sigmoid fonksiyonu kullandık.
        {
            return (float)(1.0f / (1.0f + Math.Exp(-x)));
        }

        private void agiKur()
        {
            // Dendritlere rastgele değerler veriyoruz ve değişimleri sıfırlıyoruz:
            dendrit31 = rastgele();
            _d31.Text = dendrit31.ToString();
            dendrit32 = rastgele();
            _d32.Text = dendrit32.ToString();
            dendrit3b = rastgele();
            _d3b.Text = dendrit3b.ToString();
            dendrit41 = rastgele();
            _d41.Text = dendrit41.ToString();
            dendrit42 = rastgele();
            _d42.Text = dendrit42.ToString();
            dendrit4b = rastgele();
            _d4b.Text = dendrit4b.ToString();
            degisim31 = 0;
            degisim32 = 0;
            degisim3b = 0;
            degisim41 = 0;
            degisim42 = 0;
            degisim4b = 0;

            dendrit53 = rastgele();
            _d53.Text = dendrit53.ToString();
            dendrit54 = rastgele();
            _d54.Text = dendrit54.ToString();
            dendrit5b = rastgele();
            _d5b.Text = dendrit5b.ToString();
            degisim53 = 0;
            degisim54 = 0;
            degisim5b = 0;
        }

        private float cikisHesapla(int giris1, int giris2) // İleri besleme
        {
            noron1 = (float)giris1;
            noron2 = (float)giris2;
            lblN1.Text = noron1.ToString();
            lblN2.Text = noron2.ToString();

            noron3 = aktivasyon(noron1 * dendrit31 + noron2 * dendrit32 + 1 * dendrit3b);
            noron4 = aktivasyon(noron1 * dendrit41 + noron2 * dendrit42 + 1 * dendrit4b);
            lblN3.Text = noron3.ToString();
            lblN4.Text = noron4.ToString();

            noron5 = aktivasyon(noron3 * dendrit53 + noron4 * dendrit54 + 1 * dendrit5b);
            lblN5.Text = noron5.ToString();

            return noron5;
        }

        private void egit(int giris1, int giris2, int cikis) //Farkların hesaplanması ve geri yayılım
        {
            cikisHesapla(giris1, giris2);
            Application.DoEvents();

            delta5 = (cikis - noron5);
            delta5 *= noron5 * (1 - noron5); // işlem tekrarı yapmamak için
            delta3 = dendrit53 * delta5;
            delta3 *= noron3 * (1 - noron3);
            delta4 =  dendrit54 * delta5;
            delta4 *= noron4 * (1 - noron4);

            // Ağırlık değişimlerini hesaplayıp değişimlere momentum katarak ağırlıkları güncelliyoruz.
            degisim31 = ogrenmeKatsayisi * delta3 * noron1 + momentum * degisim31;
            dendrit31 += degisim31;
            _d31.Text = dendrit31.ToString();
            degisim32 = ogrenmeKatsayisi * delta3 * noron2 + momentum * degisim32;
            dendrit32 += degisim32;
            _d32.Text = dendrit32.ToString();
            degisim3b = ogrenmeKatsayisi * delta3 * 1 + momentum * degisim3b;
            dendrit3b += degisim3b;
            _d3b.Text = dendrit3b.ToString();
            degisim41 = ogrenmeKatsayisi * delta4 * noron1 + momentum * degisim41;
            dendrit41 += degisim41;
            _d41.Text = dendrit41.ToString();
            degisim42 = ogrenmeKatsayisi * delta4 * noron2 + momentum * degisim42;
            dendrit42 += degisim42;
            _d42.Text = dendrit42.ToString();
            degisim4b = ogrenmeKatsayisi * delta4 * 1 + momentum * degisim4b;
            dendrit4b += degisim4b;
            _d4b.Text = dendrit4b.ToString();

            degisim53 = ogrenmeKatsayisi * delta5 * noron3 + momentum * degisim53;
            dendrit53 += degisim53;
            _d53.Text = dendrit53.ToString();
            degisim54 = ogrenmeKatsayisi * delta5 * noron4 + momentum * degisim54;
            dendrit54 += degisim54;
            _d54.Text = dendrit54.ToString();
            degisim5b = ogrenmeKatsayisi * delta5 * 1 + momentum * degisim5b;
            dendrit5b += degisim5b;
            _d5b.Text = dendrit5b.ToString();
        }

        private float karelerOrtalamasi(int hedef)
        {
            return (float)Math.Pow(hedef - noron5, 2) / 2.0f; // Sadece 1 tane çıkışımız var.
        }

        private void tumCikislarIcinHatalar()
        {
            cikisHesapla(0, 0);
            lblKO1.Text = karelerOrtalamasi(0).ToString();
            cikisHesapla(0, 1);
            lblKO2.Text = karelerOrtalamasi(1).ToString();
            cikisHesapla(1, 0);
            lblKO3.Text = karelerOrtalamasi(1).ToString();
            cikisHesapla(1, 1);
            lblKO4.Text = karelerOrtalamasi(0).ToString();
        }

        private bool degerleriAl(ref int g1, ref int g2, ref int c)
        {
            try
            {
                g1 = Convert.ToInt32(txtGiris1.Text);
                g2 = Convert.ToInt32(txtGiris2.Text);
                c = Convert.ToInt32(txtCikis.Text);
                if ((g1 != 0 && g1 != 1) || (g2 != 0 && g2 != 1) || (c != 0 && c != 1)) throw new Exception();
            }
            catch
            {
                MessageBox.Show(null, "Giriş ve çıkış verileri için sadece 0 veya 1 değerleri girilmelidir!", "Hata!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtGiris1.Text = "0";
                txtGiris2.Text = "0";
                txtCikis.Text = "0";
                return false;
            }
            return true;
        }

        private void egit_Click(object sender, EventArgs e) // Bir tek giriş için eğit
        {
            int g1 = 0, g2 = 0, c = 0;
            if (!degerleriAl(ref g1, ref g2, ref c)) return;

            egit(g1, g2, c);
            if (g1 == 0 && g2 == 0) lblKO1.Text = karelerOrtalamasi(c).ToString();
            else if (g1 == 0) lblKO2.Text = karelerOrtalamasi(c).ToString();
            else if (g2 == 0) lblKO3.Text = karelerOrtalamasi(c).ToString();
            else lblKO4.Text = karelerOrtalamasi(c).ToString();
        }

        private void TumuyleEgit_Click(object sender, EventArgs e) // Olası tüm girişler için eğit
        {
            kacDefa.Enabled = false;
            TumuyleEgit.Enabled = false;
            kacDefa.Minimum = 0;
            int kac = (int)kacDefa.Value;
            for (int i = 0; i < kac; i++)
            {
                kacDefa.Value--;
                egit(0, 0, 0);
                lblKO1.Text = karelerOrtalamasi(0).ToString();
                egit(0, 1, 1);
                lblKO2.Text = karelerOrtalamasi(1).ToString();
                egit(1, 0, 1);
                lblKO3.Text = karelerOrtalamasi(1).ToString();
                egit(1, 1, 0);
                lblKO4.Text = karelerOrtalamasi(0).ToString();
            }
            kacDefa.Value = kac;
            kacDefa.Minimum = 10;
            kacDefa.Enabled = true;
            TumuyleEgit.Enabled = true;
            
            tumCikislarIcinHatalar();
        }

        private void hatirla_Click(object sender, EventArgs e)
        {
            int g1 = 0, g2 = 0, c = 0;
            if (!degerleriAl(ref g1, ref g2, ref c)) return;

            cikisHesapla(g1, g2);
            txtCikis.Text = Math.Round(Convert.ToDouble(lblN5.Text)).ToString();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            agiKur();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            agiKur();
        }
    }
}
