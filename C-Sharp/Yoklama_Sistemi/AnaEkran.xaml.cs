using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using System.Collections;
using System.IO.Ports;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Threading;
/*
 
Arduino ile kart okuyup kart id sini bilgisayara yollayacaktır.
Bilgisayara gelen kart id tanımlı kartlarda var ise arduinoya giriş başarılı uyarısı yollanacaktır.
Giriş başarılı olunca servo motor ve yeşil led çalşacaktır.
Eğer bilgisayara gelen mesaj tanımsız ise kırmızı led yanacaktır.
Kullanıma göre ekran da eklenebilir giriş başarılı kart okutunuz vb bir işlem
 
*/

namespace Yoklama_Sistemi
{
    public partial class AnaEkran : Window
    {
        bool ogrencilerİceAktarildiMi = false;
        ArrayList ogrenciListesi = new ArrayList();
        SerialPort serial;
        Ogrenci eklenecekOgrenci = null;
        string dizin;
        bool portBagliMi;
        DispatcherTimer timer = new DispatcherTimer();
        string RadioButtonDurum;
        public AnaEkran()
        {
            InitializeComponent();
            dizin = Directory.GetCurrentDirectory();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.IsEnabled = true;
            timer.Tick += OgrenciGirisKontrol;
            string[] portlar = SerialPort.GetPortNames();
            foreach (string port in portlar)
            {
                comboBoxPort.Items.Add(port);
            }
            portBagliMi = false;
        }
        private void portBaglantiButon_Click(object sender, RoutedEventArgs e)
        {
            string portText;
            try
            {
                portText = comboBoxPort.SelectedItem.ToString();
            }
            catch (Exception)
            {
                MessageBox.Show("Port Seçiniz!");
                return;
                throw;
            }
            if (TextBoxDersKodu.Text != "")
            {
                portBagliMi = !portBagliMi;
                if (portBagliMi) //Port Bağlandı
                {
                    if (PortAc(portText))
                    {
                        TextBoxDersKodu.IsEnabled = false;
                        btnSistemBaslat.IsEnabled = true;
                        btnSistemDurdur.IsEnabled = false;
                        lblComPortBilgi.Content = portText + " PORTUNA BAĞLANDI.";
                        lblComPortBilgi.Foreground = Brushes.Green;
                        btnPortBaglan.Content = "Bağlantıyı Kes";
                    }
                }
                else // Bağlantı kapatıldı
                {
                    BtnYoklamaAlKaydet.IsEnabled = false;
                    PortKapat();
                    TextBoxDersKodu.IsEnabled = true;
                    btnSistemBaslat.IsEnabled = false;
                    btnSistemDurdur.IsEnabled = false;
                    lblComPortBilgi.Content = "COM PORT BAĞLI DEĞİL";
                    lblComPortBilgi.Foreground = Brushes.Red;
                    btnPortBaglan.Content = "Bağlan";
                    Sifirla();

                }
            }
            else
            {
                MessageBox.Show("Lütfen Ders Kodu Giriniz!");
            }
        }
        public bool PortAc(string portNo)
        {
            try
            {
                serial = new SerialPort(portNo);
                serial.BaudRate = 9600;
                serial.Open();
                return true;
            }
            catch
            {
                MessageBox.Show("Porta Bağlanılamadı.");
                return false;
            }
        }
        public void PortKapat()
        {
            if (serial != null && serial.IsOpen)
            {
                while (!(serial.BytesToRead == 0 && serial.BytesToWrite == 0))
                {
                    serial.DiscardInBuffer();
                    serial.DiscardOutBuffer();
                }
                serial.Close();
                //MessageBox.Show("Port Kapatıldı.");
            }
        }
        public void Sifirla()
        {
            ogrencilerİceAktarildiMi = false;
            listBoxOgrKayitli.Items.Clear();
            ogrenciListesi.Clear();
            eklenecekOgrenci = null;
            listBoxOgrGiris.Items.Clear();
            ImageSonOkuma.Source = new BitmapImage(new Uri(dizin + @"\Profil Pictures\default.png"));
            labelSonOkumaAd.Content = "";
            labelSonOkumaSoyad.Content = "";
            labelSonOkumaNo.Content = "";
            labelSonOkumaTarih.Content = "";
        }
        private void BtnOgrenciİceAktar_Click(object sender, RoutedEventArgs e)
        {
            if (ogrencilerİceAktarildiMi == false)
            {
                ogrencilerİceAktarildiMi = true;
                OgrencileriOku();
                // MessageBox.Show("Öğrenciler İçe Aktarıldı.");
            }
            else
            {
                MessageBox.Show("Öğrenciler Zaten İçe Aktarıldı.");
            }
        }
        public void OgrencileriOku()
        {
            // CSV Dosyasından Öğrencilerin bilgilerini okuyup.
            // Ogrenci classından nesne oluşturarak ogrenciListesine aktarıyoruz.
            string url = Directory.GetCurrentDirectory() + @"\Ogrenciler.csv";
            StreamReader sr = new StreamReader(url, Encoding.Default);
            string line = sr.ReadLine();
            string kartId, ad, soyad;
            string numara;
            while (true)
            {
                line = sr.ReadLine();
                if (line == null)
                {
                    break;
                }
                var list = line.Split(';');
                try
                {
                    kartId = list[0].Replace("\"", "");
                    ad = list[1].Replace("\"", "");
                    soyad = list[2].Replace("\"", "");
                    numara = list[3].Replace("\"", "");
                }
                catch (Exception)
                {
                    MessageBox.Show("CSV Dosyası Hatali!");
                    return;
                    throw;
                }
                ogrenciListesi.Add(new Ogrenci(kartId, ad, soyad, numara));
            }
            sr.Close();
            foreach (Ogrenci ogrenci in ogrenciListesi)
            {
                listBoxOgrKayitli.Items.Add(new ProfilBilgileri(ogrenci));
            }
        }
        private void btnSistemBaslat_Click(object sender, RoutedEventArgs e)
        {
            if (ogrencilerİceAktarildiMi)
            {
                serial.DataReceived += VeriGeldi;
                btnPortBaglan.IsEnabled = false;
                btnSistemDurdur.IsEnabled = true;
                btnSistemBaslat.IsEnabled = false;
                lblSistemDurumu.Content = "AKTİF";
                lblSistemDurumu.Foreground = Brushes.Green;
                BtnYoklamaAlKaydet.IsEnabled = false;
                Thread.Sleep(250);
                serial.Write("7"); //Sistem başlatıldı anlamında
                Thread.Sleep(250);
            }
            else
            {
                MessageBox.Show("Lütfen Önce Öğrencileri içeriye Aktarınız.");
            }
        }
        private void btnSistemDurdur_Click(object sender, RoutedEventArgs e)
        {
            serial.DataReceived -= VeriGeldi;
            btnPortBaglan.IsEnabled = true;
            btnSistemDurdur.IsEnabled = false;
            btnSistemBaslat.IsEnabled = true;
            lblSistemDurumu.Content = "PASİF";
            lblSistemDurumu.Foreground = Brushes.Red;
            BtnYoklamaAlKaydet.IsEnabled = true;
            Thread.Sleep(250);
            serial.Write("8"); //Sistem Durdur anlamında
            Thread.Sleep(250);
        }
        
        public void VeriGeldi(object sender, SerialDataReceivedEventArgs e)
        {
            
            var serialCihaz = sender as SerialPort;
            string veri = serialCihaz.ReadLine();
            veri = veri.Replace("\n", "").Replace("\r", "");
            if (veri.Length > 5)
            {
                if (veri[0] == '_' && veri[veri.Length - 1] == '_')
                {
                    Ogrenci tempOgrenci = null;
                    string tanimliOgrKontrol = "Tanımsız";
                    string id = "";
                    for (int i = 1; i < veri.Length - 1; i++)
                    {
                        id += veri[i];
                    }
                    foreach (Ogrenci ogr in ogrenciListesi)
                    {
                        if (ogr.getKartId() == id && ogr.getGirisDurum() == false)
                        {
                            tempOgrenci = ogr;
                            tanimliOgrKontrol = "İlk Giriş"; //Öğrenci Giriş Yaptı
                            if(RadioButtonDurum== "Sadece Giriş")
                            {
                                bool varMi = false;
                                foreach(ProfilBilgileri ogr1 in listBoxOgrGiris.Items)
                                {
                                    if(ogr1.GetOgrenci().getKartId() == ogr.getKartId())
                                    {
                                        varMi = true;
                                    }
                                }
                                if(varMi == false)
                                {
                                    eklenecekOgrenci = ogr;
                                }
                            }
                        }
                        if (ogr.getKartId() == id && ogr.getGirisDurum() == true)
                        {
                            tempOgrenci = ogr;
                            tanimliOgrKontrol = "Çok Defa Giriş"; // Öğrenci Giriş Yaptı Zaten
                        }
                    }
                    ArduinoVeriYolla(tanimliOgrKontrol,tempOgrenci);
                }
            }
        }
        
        private void ArduinoVeriYolla(string kontrol,Ogrenci ogrenci)
        {
            // kontrol değişkeni
            // Tanımsız
            // İlk Giris
            // Çok Defa Giriş

            //Yollanan Veri 
            // 1 Tanımsız Kart.
            // 2 Giriş Başarılı.
            // 3 Çıkış Yapılamaz.
            // 4 Giriş Yapılamaz.
            // 5 Çıkış Başarılı.
            // 6 Giriş Veya Çıkış Yapılamaz.
            // 7 Sistemi Aç
            // 8 Sistemi Kapat
            
            Thread.Sleep(250);
            if(RadioButtonDurum=="Sadece Giriş")
            {
                if (kontrol == "Tanımsız")
                {
                    serial.Write("1"); // 1 Tanımsız Kart.
                }
                else if(kontrol =="İlk Giriş")
                {
                    serial.Write("2");// 2 Giriş Başarılı.
                }
                else if (kontrol =="Çok Defa Giriş")
                {
                    serial.Write("3");// 3 Çıkış Yapılamaz.
                }
            }
            else if(RadioButtonDurum == "Sadece Çıkış")
            {
                if (kontrol == "Tanımsız")
                {
                    serial.Write("1");// 1 Tanımsız Kart.
                }
                else if (kontrol == "İlk Giriş")
                {
                    serial.Write("4");// 4 Giriş Yapılamaz.
                }
                else if (kontrol == "Çok Defa Giriş")
                {
                    string tarih = DateTime.Now.ToString("dd.MM.yyyy HH:mm");

                    serial.Write("5"); // 5 Çıkış Başarılı.
                    ogrenci.setGirisDurum(false);
                    ogrenci.setCikisTarihi(tarih);
                }
            }
            else if(RadioButtonDurum == "Hiç Bir Şey")
            {
                serial.Write("6");// 6 Giriş Veya Çıkış Yapılamaz.
            }
            Thread.Sleep(250);
        }
        private void OgrenciGirisKontrol(object sender, EventArgs e)
        {
            if (eklenecekOgrenci != null) //Öğrenci tanımlı ise
            {
                string tarih = DateTime.Now.ToString("dd.MM.yyyy HH:mm");
                eklenecekOgrenci.setGirisDurum(true); // Öğrenci Giriş Yaptı
                eklenecekOgrenci.setGirisTarihi(tarih);
                ProfilBilgileri profil = new ProfilBilgileri(eklenecekOgrenci);
                listBoxOgrGiris.Items.Add(profil);
                labelSonOkumaTarih.Content = tarih;
                labelSonOkumaAd.Content = eklenecekOgrenci.getAd();
                labelSonOkumaSoyad.Content = eklenecekOgrenci.getSoyad();
                labelSonOkumaNo.Content = eklenecekOgrenci.getNo();
                Uri uri = new Uri(dizin + @"\Profil Pictures\" + eklenecekOgrenci.getNo() + ".jpg");
                try
                {
                    ImageSonOkuma.Source = new BitmapImage(uri);
                }
                catch (Exception)
                {
                    uri = new Uri(dizin + @"\Profil Pictures\default.png");
                    ImageSonOkuma.Source = new BitmapImage(uri);
                }
                eklenecekOgrenci = null;
            }
        }
        private void BtnYoklamaAlKaydet_Click(object sender, RoutedEventArgs e)
        {
            //if(listBoxOgrGiris.Items.Count != 0)
            //{
                string veri = "Toplam Ogrenci Sayisi:;;;" + listBoxOgrKayitli.Items.Count;
                veri += "\n";
                veri += "Mevcut Ogrenci Sayisi:;;;" + (listBoxOgrGiris.Items.Count);
                veri += "\n";
                veri += "\n";
                veri += "Ad;Soyad;No;Giris Tarihi;Cikis Tarihi\n";
                foreach (ProfilBilgileri pb in listBoxOgrGiris.Items)
                {
                    veri += pb.getAd() + ";" + pb.getSoyad() + ";" + pb.getNo() + ";" + pb.getGirisTarihi() + ";" + pb.getCikisTarihi() + "\n";
                }
                string tarih = DateTime.Now.ToString("dd.MM.yyyy");
                string konum = Directory.GetCurrentDirectory() + @"\Yoklamalar\";
                if (!Directory.Exists(konum))
                {
                    Directory.CreateDirectory(konum);
                }
                string dosya = TextBoxDersKodu.Text + "_" + tarih + ".csv";
                string fullKonum = konum + dosya;
                try
                {
                    File.WriteAllText(fullKonum, veri, Encoding.Default);
                    MessageBox.Show(dosya + " Başarıyla Kaydedilmiştir.");
                }
                catch (Exception)
                {
                    MessageBox.Show(dosya + " Kaydedilemedi. Açıksa kapatınız.");
                }
            //}
            // else
            // {
            //     MessageBox.Show("Derse Giriş Yapan Öğrenci Bulunmamaktadır.");
            // }

        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            string veri = "Toplam Ogrenci Sayisi:;;;" + listBoxOgrKayitli.Items.Count;
            veri += "\n";
            veri += "Mevcut Ogrenci Sayisi:;;;" + (listBoxOgrGiris.Items.Count);
            veri += "\n";
            veri += "\n";
            veri += "Ad;Soyad;No;Giris Tarihi\n";
            foreach (ProfilBilgileri pb in listBoxOgrGiris.Items)
            {
                veri += pb.getAd() + ";" + pb.getSoyad() + ";" + pb.getNo() + ";" + pb.getGirisTarihi() + "\n";
            }
            string tarih = DateTime.Now.ToString("dd.MM.yyyy");
            string konum = Directory.GetCurrentDirectory() +@"\Yoklamalar\";
            if (!Directory.Exists(konum))
            {
                Directory.CreateDirectory(konum);
            }
            string dosya = TextBoxDersKodu.Text + "_" + tarih + ".csv";
            string fullKonum = konum + dosya;
            try
            {
                File.WriteAllText(fullKonum, veri, Encoding.Default);
                MessageBox.Show(dosya + " Başarıyla Kaydedilmiştir.");
            }
            catch (Exception)
            {
                MessageBox.Show(dosya + " Kaydedilemedi. Açıksa kapatınız.");
            }
        }

        private void RB_SadeceCikis_Checked(object sender, RoutedEventArgs e)
        {
            RadioButtonDurum = "Sadece Çıkış";
        }
        private void RB_HicBirSey_Checked(object sender, RoutedEventArgs e)
        {
            RadioButtonDurum = "Hiç Bir Şey";
        }
        private void RB_SadeceGiris_Checked(object sender, RoutedEventArgs e)
        {
            RadioButtonDurum = "Sadece Giriş";
        }
    }
}
