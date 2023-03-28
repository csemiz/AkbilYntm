using AkbilYntmIsKatmani;
using AkbilYntmVeriKatmani;
using Microsoft.Identity.Client;

namespace AkbilYonetimiUI
{
    public partial class FrmKayitOl : Form
    {
        IveriTabaniIslemleri veriTabaniIslemleri = new SQLVeriTabaniIslemleri(GenelIslemler.SinifSQLBaglantiCumlesi);
        public FrmKayitOl()
        {
            InitializeComponent();//Formu İnşa etmek
        }

        private void FrmKayitOl_Load(object sender, EventArgs e)
        {
            #region Ayarlar
            textBoxSifre.PasswordChar = '*';
            dateTimePicker1.MaxDate = new DateTime(2016, 1, 1);//Girilecek tarihi ayarladık
            dateTimePicker1.Value = new DateTime(2016, 1, 1);
            dateTimePicker1.Format = DateTimePickerFormat.Short;
            #endregion
        }

        private void btnKayitOl_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (var item in Controls)
                {
                    if (item is TextBox && string.IsNullOrEmpty(((TextBox)item).Text))
                    {
                        MessageBox.Show("Zorunlu alanlari doldurunuz.");
                        return;
                    }
                }
                Dictionary<string, object> kolonlar = new Dictionary<string, object>();
                kolonlar.Add("Ad", $"'{textBoxIsim.Text.Trim()}'");
                kolonlar.Add("Soyad", $"'{textBoxSoyisim.Text.Trim()}'");
                kolonlar.Add("Email", $"'{textBoxEmail.Text.Trim()}'");
                kolonlar.Add("EklenmeTarihi", $"'{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}'");
                kolonlar.Add("DogumTarihi", $"'{dateTimePicker1.Value.ToString("yyyyMMdd")}'");
                kolonlar.Add("Parola", $"'{GenelIslemler.MD5Encryption(textBoxSifre.Text.Trim())}'");

                string insertCumle = veriTabaniIslemleri.VeriEklemeCumlesiOlustur("Kullanicilar", kolonlar);
                int sonuc = veriTabaniIslemleri.KomutIsle(insertCumle);
                if (sonuc > 0)
                {
                    MessageBox.Show("Kayıt oluşturuldu");
                   DialogResult cevap= MessageBox.Show("Giriş sayfasına yönlendirmemizi ister misiniz?","SORU",MessageBoxButtons.YesNo,MessageBoxIcon.Question);

                    if (cevap==DialogResult.Yes)
                    {
                        //temizlik

                        //Girişe git
                        Form1 frmg = new Form1();
                        frmg.Email = textBoxEmail.Text.Trim();

                        foreach (Form item in Application.OpenForms)
                        {
                            item.Hide();
                        }
                        frmg.Show();
                    }

                }
                else
                {
                    MessageBox.Show("Kayıt EKLENEMEDİ !");
                }
            }
            catch (Exception ex)
            {
                //ex log.txt'ye yazılacak (loglama) 
                MessageBox.Show("Beklenmedik bir hata oluştu! Lütfen tekrar deneyiniz !" +
                     ex.Message); ;
            }
        }

        private void GirisFomunaGit()
        {
            Form1 frmG = new Form1();
            frmG.Email = textBoxEmail.Text.Trim();
            this.Hide();
            frmG.Show();
        }

        private void FrmKayitOl_FormClosed(object sender, FormClosedEventArgs e)
        {
            GirisFomunaGit();
        }

        private void btnGirisYap_Click(object sender, EventArgs e)
        {
            GirisFomunaGit();
        }
    }
}






