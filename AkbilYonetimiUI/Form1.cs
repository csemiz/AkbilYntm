using AkbilYntmIsKatmani;
using AkbilYntmVeriKatmani;
using System.Text;

namespace AkbilYonetimiUI
{
    public partial class Form1 : Form
    {
        public string Email { get; set; }//kayýt ol formunda kayýt olan kullanýcýnýn emaili buraya gelsin
        IveriTabaniIslemleri veriTabaniIslemleri = new SQLVeriTabaniIslemleri();

        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            if (Email != null)
            {
                txtEmail.Text = Email;
            }
            txtEmail.TabIndex = 1;
            txtSifre.TabIndex = 2;
            checkBoxHatirla.TabIndex = 3;
            btnGirisYap.TabIndex = 4;
            btnKayitOl.TabIndex = 5;

        }
        private void btnKayitOl_Click(object sender, EventArgs e)
        {
            //Bu formu gizleyeceðiz.
            //Kayýt ol formunu açacaðýz.
            this.Hide();
            FrmKayitOl frm = new FrmKayitOl();
            frm.Show();

        }

        private void txtEmail_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnGirisYap_Click(object sender, EventArgs e)
        {
            GirisYap();
        }

        private void GirisYap()
        {
            try
            {
                //1)Email ve sifre textboxlarý dolu mu?
                if (string.IsNullOrEmpty(txtEmail.Text) || string.IsNullOrEmpty(txtSifre.Text))
                {
                    MessageBox.Show("Bilgilerinizi eksiksiz giriniz", "UYARI", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }
                //2)Girdiði email ve þifre veritabanýnda mevcut mu?
                //select*from Kullanicilar where Email='' ans Sifre=''

                string[] istedigimKolonlar = new string[] { "Id", "Ad", "Soyad" };
                string kosullar = string.Empty;
                StringBuilder sb = new StringBuilder();
                sb.Append($"Email='{txtEmail.Text.Trim()}'");
                sb.Append(" and ");
                sb.Append($"Parola='{GenelIslemler.MD5Encryption(txtSifre.Text.Trim())}'");
                kosullar = sb.ToString();

                var sonuc = veriTabaniIslemleri.VeriOku("Kullanicilar", istedigimKolonlar, kosullar);

                if (sonuc.Count==0)
                {
                    MessageBox.Show("Email ya da þifre yanlýþ ! Tekrar deneyiniz ! ");
                }
                else
                {
                    GenelIslemler.GirisYapanKullaniciID = (int)sonuc["Id"];
                    GenelIslemler.GirisYapanKullaniciAdSoyad = $"{sonuc["Ad"]} {sonuc["Soyad"]}";
                    MessageBox.Show($"Hoþgeldiniz....{GenelIslemler.GirisYapanKullaniciAdSoyad}");

                    //BENÝ HATIRLA YAZILACAK
                    this.Hide();
                    FrmAnasayfa frmanasayfa = new FrmAnasayfa();
                    frmanasayfa.Show();

                }

            }
            catch (Exception hata)
            {
                //Dipnot: Exceptionlar asla kullanýcýya gösterilemez.Exceptionlar loglanýr, yazýlýmcýya iletilir.Biz öðrenmek için mbox ýn içine yazdýk.

                MessageBox.Show("Beklenmedik bir sorun oluþtu!" + hata.Message);
            }
        }

        private void checkBoxHatirla_CheckedChanged(object sender, EventArgs e)
        {
            BeniHatirla();
        }

        private void BeniHatirla()
        {

        }

        private void txtSifre_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))//basilan tus enter ise giris yapilacak
            {
                GirisYap();
            }
        }
    }
}