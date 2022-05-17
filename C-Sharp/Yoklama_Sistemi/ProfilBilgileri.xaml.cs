using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.IO;
namespace Yoklama_Sistemi
{
    /// <summary>
    /// ProfilBilgileri.xaml etkileşim mantığı
    /// </summary>
    public partial class ProfilBilgileri : UserControl
    {
        private Ogrenci ogr;
        private Uri uri;
        public ProfilBilgileri(Ogrenci ogrenci)
        {
            InitializeComponent();
            ogr = ogrenci;
            string dizin = Directory.GetCurrentDirectory();
            labelAd.Content = ogr.getAd() + " " + ogr.getSoyad();
            uri = new Uri(dizin + @"\Profil Pictures\" + ogr.getNo()+ ".jpg");
            try
            {
                resim.Source = new BitmapImage(uri);
            }
            catch (Exception ) {
                uri = new Uri(dizin + @"\Profil Pictures\default.png");
                resim.Source = new BitmapImage(uri);
            }
        }
        public Ogrenci GetOgrenci()
        {
            return ogr;
        }
        public string getAd()
        {
            return ogr.getAd();
        }
        public string getSoyad()
        {
            return ogr.getSoyad();
        }
        public string getNo()
        {
            return ogr.getNo();
        }
        public string getGirisTarihi()
        {
            return ogr.getGirisTarihi();
        }
        public string getCikisTarihi()
        {
            return ogr.getCikisTarihi();
        }
        private void UserControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            KullaniciBilgileri pencere = new KullaniciBilgileri(ogr.getAd(), ogr.getSoyad(), ogr.getNo(), uri);
            pencere.ShowDialog();
        }
    }
}
