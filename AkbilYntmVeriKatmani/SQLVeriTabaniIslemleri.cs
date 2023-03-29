using AkbilYntmIsKatmani;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace AkbilYntmVeriKatmani
{
    public class SQLVeriTabaniIslemleri : IveriTabaniIslemleri
    {
        public string BaglantiCumlesi { get; set; }

        private SqlConnection baglanti;
        private SqlCommand komut;
        public SQLVeriTabaniIslemleri()
        {
            BaglantiCumlesi = GenelIslemler.SinifSQLBaglantiCumlesi;//buranin icine gotodefinition yapip kendi pc adini yaz
            baglanti = new SqlConnection(BaglantiCumlesi);
            komut = new SqlCommand();
            komut.Connection = baglanti;
        }
        public SQLVeriTabaniIslemleri(string baglantiCumle)
        {//burada baglanti cumlesini parametreye atadik iki turlu kullanilabilir
            BaglantiCumlesi = baglantiCumle;
            baglanti = new SqlConnection(BaglantiCumlesi);
            komut = new SqlCommand();
            komut.Connection = baglanti;
        }
        private void BaglantiyiAc()
        {
            baglanti.ConnectionString = BaglantiCumlesi;
            if (baglanti.State != ConnectionState.Open)
            {
                baglanti.Open();
            }
        }
        public int KomutIsle(string eklemeyadaGuncellemeCumlesi)
        {
            try
            {
                using (baglanti)
                {
                    komut.CommandType = CommandType.Text;
                    komut.CommandText = eklemeyadaGuncellemeCumlesi;
                    BaglantiyiAc();
                    int EtkilenenSatirSayisi = komut.ExecuteNonQuery();
                    return EtkilenenSatirSayisi;

                    //return komut.ExecuteNonQuery();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public string VeriEklemeCumlesiOlustur(string tabloAdi, Dictionary<string, object> kolonlar)
        {
            try
            {
                //insert into tabloAdi (kolonlar) values (degerler)
                string sorgu = string.Empty;
                string sutunlar = string.Empty;
                string degerler = string.Empty;

                foreach (var item in kolonlar)
                {
                    sutunlar += $"{item.Key},";
                    degerler += $"{item.Value},";
                }
                //En sondaki vırgulden kurtulmamiz icin TRIM kullanalim
                sutunlar = sutunlar.TrimEnd(',');
                degerler = degerler.TrimEnd(',');

                sorgu = $" insert into {tabloAdi} ({sutunlar}) values ({degerler}) ";
                return sorgu;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public DataTable VeriGetir(string tabloAdi, string kolonlar = "*", string kosullar = null)
        {
            try
            {
                using (baglanti)
                {
                    string sorgu = $"select {kolonlar} from {tabloAdi} ";
                    if (!string.IsNullOrEmpty(kosullar))
                    {
                        sorgu += $" where {kosullar}";
                    }
                    komut.CommandText = sorgu;
                    SqlDataAdapter adp = new SqlDataAdapter(komut);
                    BaglantiyiAc();
                    DataTable dt = new DataTable();
                    adp.Fill(dt);
                    return dt;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public string VeriGuncellemeCumlesiOlustur(string tabloAdi, Hashtable kolonlar, string? kosullar = null)
        {
            try
            {
                //update TabloAdi set col1=deger1,... where kosul
                string sorgu = string.Empty, setler = string.Empty;
                foreach (var item in kolonlar.Keys)
                {
                    setler += $"{item}={kolonlar[item]}, ";
                }
                setler = setler.Trim().TrimEnd(','); //todo deneyecegiz...

                sorgu += $"update {tabloAdi} set {setler} ";
                if (!string.IsNullOrEmpty(kosullar))
                {
                    sorgu += $" where {kosullar}";
                }
                return sorgu;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public Hashtable VeriOku(string tabloAdi, string[] kolonlar, string? kosullar = null)
        {
            try
            {
                Hashtable sonuc = new Hashtable();
                string sutunlar = string.Empty;
                //kolonlara , ekleyecegiz
                StringBuilder sb = new StringBuilder();
                foreach (var item in kolonlar)
                {
                    sb.Append($"{item} ,");
                }
                sutunlar = sb.ToString().TrimEnd(',');


                string sorgu = $"select {sutunlar} from {tabloAdi} ";
                if (!string.IsNullOrEmpty(kosullar))
                {
                    sorgu += $" where {kosullar}";
                }
                using (baglanti)
                {
                    komut.CommandText = sorgu;
                    BaglantiyiAc();
                    SqlDataReader okuyucu = komut.ExecuteReader();
                    if (okuyucu.HasRows)
                    {
                        while (okuyucu.Read())
                        {
                            //for (int i = 0; i < kolonlar.Length; i++)
                            //{
                            //    sonuc.Add(kolonlar[i], okuyucu[kolonlar[i]]);

                            //}

                            foreach (var item in kolonlar)
                            {
                                sonuc.Add(item, okuyucu[item]);//birden cokakbilgetirdiginde??

                            }

                        }//while bitti.
                    }
                }

                return sonuc;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public int VeriSil(string tabloAdi, string? kosullar = null)
        {
            try
            {
                using (baglanti)
                {
                    string sorgu = $" delete from {tabloAdi} ";
                    if (!string.IsNullOrEmpty(kosullar))
                    {
                        sorgu += $" where {kosullar}";
                    }
                    komut.CommandText = sorgu;
                    BaglantiyiAc();
                    int silinenSatirSayisi = komut.ExecuteNonQuery();
                    return silinenSatirSayisi;

                }
            }
            catch (Exception)
            {

                throw;
            }
        }

  

        
    }
}
