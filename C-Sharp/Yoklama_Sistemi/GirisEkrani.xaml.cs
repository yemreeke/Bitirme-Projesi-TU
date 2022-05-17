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
using System.Windows.Shapes;
using System.Net;
using System.Management;
namespace Yoklama_Sistemi
{
    /// <summary>
    /// GirisEkrani.xaml etkileşim mantığı
    /// </summary>
    public partial class GirisEkrani : Window
    {
        public GirisEkrani()
        {
            InitializeComponent();
        }

        private void BtnKopyala_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(hwidTextBox.Text);

        }

        private void BtnGirisYap_Click(object sender, RoutedEventArgs e)
        {
            if (hwidTextBox.Text != "")
            {
                string data;
                using (var webClient = new WebClient())
                {
                    webClient.Headers["Accept-Encoding"] = "utf-8";
                    data = webClient.DownloadString("https://yemreeke.com/hwid.txt");
                }
                // 2 adet hwid olunca hata veriyor.
                string[] hwids = data.Split('\n');
                bool kontrol = false;
                for (int i = 0; i < hwids.Length; i++)
                {
                    hwids[i] = hwids[i].Replace("\n", "").Replace("\r", "");
                    if (hwids[i] == hwidTextBox.Text)
                    {
                        kontrol = true;
                    }
                }
                if (kontrol)
                {
                    MessageBox.Show("Giriş Başarılı.", "Giriş Başarılı.");
                    var newWindow = new AnaEkran();
                    newWindow.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Giriş Başarısız.", "Giriş Başarısız.");
                }

            }
            else
            {
                MessageBox.Show("Lütfen Hwid Öğreniniz.", "Lütfen Hwid Öğreniniz.");
            }
        }

        private void BtnHwidOgren_Click(object sender, RoutedEventArgs e)
        {
            //Sistemin Hwid Bilgisini Öğreniyoruz.
            var mbs = new ManagementObjectSearcher("Select ProcessorId From Win32_processor");
            ManagementObjectCollection mbsList = mbs.Get();
            string id = "";
            foreach (ManagementObject mo in mbsList)
            {
                id = mo["ProcessorId"].ToString();
                break;
            }
            hwidTextBox.Text = id;
        }
    }
}
