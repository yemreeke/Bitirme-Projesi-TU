using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yoklama_Sistemi
{
    public class Ogrenci
    {
        private string kartId;
        private string ad;
        private string soyad;
        private string no;
        private bool girisDurum;
        private bool cikisDurum;
        private string GirisTarihi;
        private string CikisTarihi;
        public Ogrenci(string kartId, string ad, string soyad, string no)
        {
            this.girisDurum = false;
            this.kartId = kartId;
            this.ad = ad;
            this.soyad = soyad;
            this.no = no;
        }
        public void setGirisTarihi(string tarih)
        {
            this.GirisTarihi = tarih;
        }
        public void setCikisTarihi(string tarih)
        {
            this.CikisTarihi = tarih;
        }
        public void setKartId(string kartId)
        {
            this.kartId = kartId;
        }
        public void setAd(string ad)
        {
            this.ad = ad;
        }
        public void setSoyad(string soyad)
        {
            this.soyad = soyad;
        }
        public void setNo(string no)
        {
            this.no = no;
        }
        public void setGirisDurum(bool girisDurum)
        {
            this.girisDurum = girisDurum;
        }
        public void setCikisDurum(bool cikisDurum)
        {
            this.cikisDurum = cikisDurum;
        }
        public bool getGirisDurum()
        {
            return girisDurum;
        }
        public bool getCikisDurum()
        {
            return cikisDurum;
        }
        public string getGirisTarihi()
        {
            return GirisTarihi;
        }
        public string getCikisTarihi()
        {
            return CikisTarihi;
        }
        public string getKartId()
        {
            return kartId;
        }
        public string getAd()
        {
            return ad;
        }
        public string getSoyad()
        {
            return soyad;
        }
        public string getNo()
        {
            return no;
        }
        public override string ToString()
        {
            return "ID:" + kartId + " Ad:" + ad + " Soyad:" + soyad + " No:" + no.ToString();
        }
    }
}
