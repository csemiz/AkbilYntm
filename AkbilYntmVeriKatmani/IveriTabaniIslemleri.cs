using System.Collections;
using System.Data;

namespace AkbilYntmVeriKatmani
{
    public interface IveriTabaniIslemleri
    {
        //CRUD: Create Read Update Delete
        string BaglantiCumlesi { get; set; }
        DataTable VeriGetir(string tabloAdi, string kolonlar = "*", string kosullar = null);

        int VeriSil(string tabloAdi, string? kosullar = null);

        int KomutIsle(string eklemeyadaGuncellemeCumlesi);//executenonquery

        string VeriEklemeCumlesiOlustur(string tabloAdi, Dictionary<string, object> kolonlar);
        string VeriGuncellemeCumlesiOlustur(string tabloAdi, Hashtable kolonlar, string? kosullar = null);
        Hashtable VeriOku(string tabloAdi, string[] kolonlar, string? kosullar = null);
    }
}
