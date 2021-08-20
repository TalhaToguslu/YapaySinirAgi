using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;
using System.IO;
using ExcelDataReader;
using ExcelApp = Microsoft.Office.Interop.Excel;
using Microsoft.Office.Interop;
using System.Collections;
using System.Numerics;
using Rationals;

namespace YapaySinirAgi
{
    public partial class Form1 : Form
    {       

        List<double> XA1 = new List<double>();
        List<double> XA2 = new List<double>();
        List<double> XA3 = new List<double>();
        List<double> XA4 = new List<double>();
        List<double> XA5 = new List<double>();
        List<double> XA6 = new List<double>();
        List<double> XA7 = new List<double>();
        List<double> XA8 = new List<double>();
        List<double> YA1 = new List<double>();
        List<double> gizliKatman = new List<double>();
        List<double> dentritGG = new List<double>();// Giriş - Gizli
        List<double> dentritGC = new List<double>();// Giriş - Çıkış
        List<double> ciktiKatmani = new List<double>();

        private int gizliKatmanHucreSayisi = 0;
        private int girisHucreSayisi = 8;
        private int satir = 0;
        private int epoch = 0;
        private double ogrenmeKatsayisi = 0.01f;
        private double momentum = 0.02f;
        private double hataHesabi = 0;
        private double dagitilacakHata = 0;
        private double bias = 0;
        private double mape = 0;
        private double satirSayisi = 0;

        List<double> agirlikDegisimMiktari = new List<double>();
        List<double> araKatmanDagitilacakHataHesabi = new List<double>();
        List<double> girisAraKatmanArasiAgirlik = new List<double>();
        List<double> hataDegerleri = new List<double>();
        Random rand = new Random();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        //Veriler normalize edildi
        private List<double> NormalizeEt(List<double> list)
        {
            double min = 0, max = 0;
            for (int i = 0; i < list.Count; i++)
            {

                if (i == 0)
                {
                    min = list[i];
                    max = list[i];
                }

                if (list[i] < min)
                {
                    min = list[i];
                }

                if (list[i] > max)
                {
                    max = list[i];
                }
            }

            for (int i = 0; i < list.Count; i++)
            {
                list[i] = (list[i] - min) / (max - min);
            }
            return list;
        }

        private double RandomDeger()
        {
            return rand.NextDouble() * (1 - 0) + 0;
        }

        private void GirisAraDentritUret()
        {
            for (int i = 0; i < girisHucreSayisi * gizliKatmanHucreSayisi; i++)
            {
                dentritGG.Add(RandomDeger());
            }
        }

        private double SigmoidFonksiyonu(double total)
        {
            return (1 / (1 + Math.Exp(-total)));
        }

        private void AraCiktiDentritUret()
        {
            for (int i = 0; i < gizliKatmanHucreSayisi; i++)
            {
                dentritGC.Add(RandomDeger());
            }
        }

        private void VeriSetiOku()
        {
            //Dosyanın Adresi
            string filePath = @"enerji-verimliliği-veri-seti.xls";

            //Dosya İzinleri
            FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read);

            IExcelDataReader excelReader;

            int counter = 0;

            //Dosya Excel Mi Değil mi?
            if (Path.GetExtension(filePath).ToUpper() == ".XLS")
            {
                excelReader = ExcelReaderFactory.CreateBinaryReader(stream);
            }
            else
            {
                excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            }

            //Verilerin Okunması
            while (excelReader.Read())
            {
                if (counter == 0)
                {
                    satir = rand.Next(2, 769);
                }

                counter++;
                //ilk satır başlık olduğu için 2.satırdan okumaya başlıyorum.
                if (counter == satir)
                {
                    XA1.Add(excelReader.GetDouble(0));
                    XA2.Add(excelReader.GetDouble(1));
                    XA3.Add(excelReader.GetDouble(2));
                    XA4.Add(excelReader.GetDouble(3));
                    XA5.Add(excelReader.GetDouble(4));
                    XA6.Add(excelReader.GetDouble(5));
                    XA7.Add(excelReader.GetDouble(6));
                    XA8.Add(excelReader.GetDouble(7));
                    YA1.Add(excelReader.GetDouble(8));
                    break;
                }

            }
            excelReader.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            try
            {
                gizliKatmanHucreSayisi = Convert.ToInt32(textBox1.Text);

                if (2 <= gizliKatmanHucreSayisi && gizliKatmanHucreSayisi <= 20)
                {

                    while (epoch < 100)
                    {

                        if (epoch > 0)
                        {
                            XA1.Clear();
                            XA2.Clear();
                            XA3.Clear();
                            XA4.Clear();
                            XA5.Clear();
                            XA6.Clear();
                            XA7.Clear();
                            XA8.Clear();
                            YA1.Clear();
                        }

                        SatirSayisi();
                        
                        //Verinin %70'i
                        satirSayisi = satirSayisi*(7/10f);

                        for (int i = 0; i < satirSayisi; i++)//Veri setinin %70'i
                        {
                            VeriSetiOku();
                        }

                        //Normalize edildi.
                        XA1 = NormalizeEt(XA1);
                        XA2 = NormalizeEt(XA2);
                        XA3 = NormalizeEt(XA3);
                        XA4 = NormalizeEt(XA4);
                        XA5 = NormalizeEt(XA5);
                        XA6 = NormalizeEt(XA6);
                        XA7 = NormalizeEt(XA7);
                        XA8 = NormalizeEt(XA8);
                        YA1 = NormalizeEt(YA1);

                        if (epoch == 0)
                        {
                            //Giriş - gizli arası Dentrit Üretme
                            GirisAraDentritUret();

                            //Gizli - Çıktı katmanı arası dentrit
                            AraCiktiDentritUret();
                        }

                        Öğrenme();

                        epoch++;

                    }

                }
                else
                {
                    MessageBox.Show("Gizli Katman Hücre Sayısı 2 ve 20 arasında olmalıdır.");
                }

            }
            catch (Exception)
            {
                MessageBox.Show("Gizli Katman Hücre Sayısı 2 ve 20 arasında girin.");
            }                       
          
        }

        private void Öğrenme()
        {
            double total;
            hataHesabi = 0;
            mape = 0;
            //Verilerin Tek Tek Alınması
            for (int i = 0; i < XA1.Count; i++)
            {
                //Toplam Fonksiyonu               
                total = 0;
                for (int j = 0; j < dentritGG.Count; j += 8)
                {                    
                    total += XA1[i] * dentritGG[j];
                    total += XA2[i] * dentritGG[j + 1];
                    total += XA3[i] * dentritGG[j + 2];
                    total += XA4[i] * dentritGG[j + 3];
                    total += XA5[i] * dentritGG[j + 4];
                    total += XA6[i] * dentritGG[j + 5];
                    total += XA7[i] * dentritGG[j + 6];
                    total += XA8[i] * dentritGG[j + 7];
                    bias = RandomDeger();
                    total += bias;
                    //Sigmoid Fonksiyonu
                    gizliKatman.Add(SigmoidFonksiyonu(total));
                    total = 0;
                }

                //Çıkış Katmanının Bulunması
                total = 0;
                for (int j = 0; j < gizliKatman.Count; j++)
                {
                    total += gizliKatman[j] * dentritGC[j];
                }

                ciktiKatmani.Add(SigmoidFonksiyonu(total));

                //MSE 
                hataHesabi = Math.Pow((YA1[i] - ciktiKatmani[ciktiKatmani.Count - 1]), 2)/XA1.Count;
                hataDegerleri.Add(hataHesabi);

                //MAPE
                mape += Math.Abs(YA1[i] - ciktiKatmani[ciktiKatmani.Count - 1]) / YA1[i];
                mape = (mape / XA1.Count) * 100;

                if (mape > 3)
                {
                    label3.Text = "İterasyon Sayısı : "+(i-1);
                    MessageBox.Show("Mape Değeri 3%'ü geçti.");
                    mape = Math.Round(mape*100,3);
                    epoch = 100;
                    break;
                }

                //Çıktı katmanı için dağılatacak hataların hesaplanması
                dagitilacakHata = ciktiKatmani[ciktiKatmani.Count - 1] * (1 - ciktiKatmani[ciktiKatmani.Count - 1]) * hataHesabi;

                //Ağırlıkların Değişim miktarını hesaplama
                for (int j = 0; j < gizliKatman.Count; j++)
                {
                    agirlikDegisimMiktari.Add(dagitilacakHata * ogrenmeKatsayisi * gizliKatman[j] + momentum * i);
                }

                //Ara katmanda dağıtılacak hata hesabı
                for (int j = 0; j < gizliKatman.Count; j++)
                {
                    araKatmanDagitilacakHataHesabi.Add(gizliKatman[j] * ((1 - gizliKatman[j]) * (dagitilacakHata * dentritGC[j])));
                }

                
                //Gizli - çıktı katman arası ağırlıkların güncellenmesi
                for (int j = 0; j < gizliKatman.Count; j++)
                {
                    dentritGC[j] = dentritGC[j] - araKatmanDagitilacakHataHesabi[j];
                }

                //Giriş - Ara katman Arası ağırlık hesabı
                for (int j = 0; j < araKatmanDagitilacakHataHesabi.Count; j++)
                {
                    girisAraKatmanArasiAgirlik.Add(-1 * araKatmanDagitilacakHataHesabi[j] * ogrenmeKatsayisi * XA1[i] + momentum * i);
                    girisAraKatmanArasiAgirlik.Add(-1 * araKatmanDagitilacakHataHesabi[j] * ogrenmeKatsayisi * XA2[i] + momentum * i);
                    girisAraKatmanArasiAgirlik.Add(-1 * araKatmanDagitilacakHataHesabi[j] * ogrenmeKatsayisi * XA3[i] + momentum * i);
                    girisAraKatmanArasiAgirlik.Add(-1 * araKatmanDagitilacakHataHesabi[j] * ogrenmeKatsayisi * XA4[i] + momentum * i);
                    girisAraKatmanArasiAgirlik.Add(-1 * araKatmanDagitilacakHataHesabi[j] * ogrenmeKatsayisi * XA5[i] + momentum * i);
                    girisAraKatmanArasiAgirlik.Add(-1 * araKatmanDagitilacakHataHesabi[j] * ogrenmeKatsayisi * XA6[i] + momentum * i);
                    girisAraKatmanArasiAgirlik.Add(-1 * araKatmanDagitilacakHataHesabi[j] * ogrenmeKatsayisi * XA7[i] + momentum * i);
                    girisAraKatmanArasiAgirlik.Add(-1 * araKatmanDagitilacakHataHesabi[j] * ogrenmeKatsayisi * XA8[i] + momentum * i);
                }

                //Yeni Ağırlıkların Hesaplanması
                for (int j = 0; j < dentritGG.Count; j++)
                {
                    dentritGG[j] = dentritGG[j] + girisAraKatmanArasiAgirlik[j];
                }

                //Listelerin Düzenlemesi
                gizliKatman.Clear();
                agirlikDegisimMiktari.Clear();
                araKatmanDagitilacakHataHesabi.Clear();
                girisAraKatmanArasiAgirlik.Clear();

                //Grafiğe Yazılması İşlemi
                for (int k = 0; k < i; k++)
                {
                    chart1.Series["Hata D. - İterasyon S."].Points.AddXY(i, hataDegerleri[k]);
                }
            }
        }

        private void SatirSayisi()
        {

            //Dosyanın Adresi
            string filePath = @"enerji-verimliliği-veri-seti.xls";

            //Dosya İzinleri
            FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read);

            IExcelDataReader excelReader;

            //Dosya Excel Mi Değil mi?
            if (Path.GetExtension(filePath).ToUpper() == ".XLS")
            {
                excelReader = ExcelReaderFactory.CreateBinaryReader(stream);
            }
            else
            {
                excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            }

            //Verilerin Okunması
            while (excelReader.Read())
            {
                satirSayisi++;
            }
            excelReader.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Restart();
        }
    }
}

