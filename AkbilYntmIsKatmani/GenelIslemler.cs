using System.Security.Cryptography;
using System.Text;

namespace AkbilYntmIsKatmani
{
    public class GenelIslemler
    {
        public static string SinifSQLBaglantiCumlesi { get; set; } = @"Server=DESKTOP-E30TBPJ\MSSQLSERVER01;Database=AKBILUYGULAMADB;Trusted_Connection=True; TrustServerCertificate=True;";

        public static string EvSQLBaglantiCumlesi { get; set; } = "";
        public static int GirisYapanKullaniciID { get; set; } 
        public static string GirisYapanKullaniciEmail { get; set; }
        public static string GirisYapanKullaniciAdSoyad { get; set; }

        public static string MD5Encryption(string encryptionText)
        {

            // We have created an instance of the MD5CryptoServiceProvider class.
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            //We converted the data as a parameter to a byte array.
            byte[] array = Encoding.UTF8.GetBytes(encryptionText);
            //We have calculated the hash of the array.
            array = md5.ComputeHash(array);
            //We created a StringBuilder object to store hashed data.
            StringBuilder sb = new StringBuilder();//Fazladan string deger olusturmadan string degerleri birlestirmek icin kullanilir(string builder)//
            //We have converted each byte from string into string type.

            foreach (byte ba in array)
            {
                sb.Append(ba.ToString("x2").ToLower());
            }

            //We returned the hexadecimal string.
            return sb.ToString();
        }

    }
}

