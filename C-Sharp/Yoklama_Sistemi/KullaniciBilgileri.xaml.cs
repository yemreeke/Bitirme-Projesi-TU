using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Yoklama_Sistemi
{
    /// <summary>
    /// KullaniciBilgileri.xaml etkileşim mantığı
    /// </summary>
    public partial class KullaniciBilgileri : Window
    {
        public KullaniciBilgileri(string ad, string soyad, string numara, Uri uri)
        {
            InitializeComponent();
            profilFotografi.Source = new BitmapImage(uri);
            labelAd.Content = ad;
            labelSoyad.Content = soyad;
            labelNumara.Content = numara;
        }
    }
}
