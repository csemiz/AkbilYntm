using AkbilYntmIsKatmani;
using AkbilYntmVeriKatmani;

namespace AkbilYonetimiUI
{
    public partial class FrmTalimatlar : Form
    {
        IveriTabaniIslemleri VeriTabaniIslemleri = new SQLVeriTabaniIslemleri(GenelIslemler.SinifSQLBaglantiCumlesi);
        public FrmTalimatlar()
        {
            InitializeComponent();
        }

        private void FrmTalimatlar_Load(object sender, EventArgs e)
        {
            //Comboboxa akbilleri getir
            ComboBoxaKullanicininAkbilleriniGetir();

            cmbBoxAkbiller.SelectedIndex = -1;
            cmbBoxAkbiller.Text = "Akbil seçiniz...";
            // cmbBoxAkbiller.DropDownStyle = ComboBoxStyle.DropDownList;
            groupBoxYukleme.Enabled = false;

            dataGridViewTalimatlar.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            TalimatlariDataGrideGetir();
            dataGridViewTalimatlar.ContextMenuStrip = contextMenuStrip1;

            checkBoxTumunuGoster.Checked = false;
            BekleyenTalimatSayisiniGetir();
            timerBekleyenTalimat.Interval = 1000;
            timerBekleyenTalimat.Enabled = true;
            

        }

        private void BekleyenTalimatSayisiniGetir()
        {
            try
            {
                lblBekleyenTalimat.Text = VeriTabaniIslemleri.VeriGetir("KullanicininTalimatlari", kosullar: $"KullaniciId={GenelIslemler.GirisYapanKullaniciID} and YuklendiMi=0").Rows.Count.ToString();
            }
            catch (Exception hata)
            {

                MessageBox.Show("Beklenmedik bir hata oluştu !" + hata.Message); ;
            }
        }

        private void TalimatlariDataGrideGetir(bool tumunuGoster = false)
        {
            try
            {
                if (tumunuGoster)//tumunuGoster==true
                {
                    dataGridViewTalimatlar.DataSource = VeriTabaniIslemleri.VeriGetir("KullanicininTalimatlari", kosullar: $"KullaniciId={GenelIslemler.GirisYapanKullaniciID}");
                }
                else
                {
                    dataGridViewTalimatlar.DataSource = VeriTabaniIslemleri.VeriGetir("KullanicininTalimatlari", kosullar: $"KullaniciId={GenelIslemler.GirisYapanKullaniciID} and YuklendiMi=0");
                }
                dataGridViewTalimatlar.Columns["Id"].Visible = false;
                dataGridViewTalimatlar.Columns["Akbil"].Width = 200;
                dataGridViewTalimatlar.Columns["YuklendiMi"].HeaderText = "Talimat Yüklendi mi?";
                dataGridViewTalimatlar.Columns["YuklendiMi"].Width = 150;
                //istediğiniz diger kolonlara da ayarlama yapabilirsiniz.

            }
            catch (Exception hata)
            {
                MessageBox.Show("Taliamtlar getirilirken hata oluştu " + hata.Message);
            }
        }

        private void ComboBoxaKullanicininAkbilleriniGetir()
        {
            try
            {
                cmbBoxAkbiller.DataSource = VeriTabaniIslemleri.VeriGetir("Akbiller", kosullar: $"AkbilSahibiId={GenelIslemler.GirisYapanKullaniciID}");
                cmbBoxAkbiller.DisplayMember = "AkbilNo";
                cmbBoxAkbiller.ValueMember = "AkbilNo";//Genellikle benzersiz bilgi atanir. Orn:MissingPrimaryKeyException key kolonu

            }
            catch (Exception hata)
            {

                MessageBox.Show("Beklenmedik bir hata oluştu ! " + hata.Message); ;
            }
        }

        private void cmbBoxAkbiller_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbBoxAkbiller.SelectedIndex >= 0)
            {
                textBoxYuklenecekTutar.Clear();
                groupBoxYukleme.Enabled = true;
            }
            else
            {
                textBoxYuklenecekTutar.Clear();
                groupBoxYukleme.Enabled = false;
            }

        }

        private void btnTalimatKaydet_Click(object sender, EventArgs e)
        {
            try
            {
                if (cmbBoxAkbiller.SelectedIndex < 0)
                {
                    MessageBox.Show("Akbil seçmeden talimat seçilemez ! ");
                    return;
                }
                if (string.IsNullOrEmpty(textBoxYuklenecekTutar.Text))
                {
                    MessageBox.Show("Yükleme için giriş miktarı zorunludur ! ! ");
                    return;
                }
                if (!decimal.TryParse(textBoxYuklenecekTutar.Text.Trim(), out decimal tutar))
                {
                    MessageBox.Show("Yükleme miktarı girişi uygun formatta olmalıdır ! ");
                    return;
                }
                Dictionary<string, object> kolonlar = new Dictionary<string, object>();
                kolonlar.Add("EklenmeTarihi", $"'{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}'");
                kolonlar.Add("AkbilId", $"'{cmbBoxAkbiller.SelectedValue}'");
                kolonlar.Add("YuklenecekTutar", tutar);
                kolonlar.Add("YuklendiMi", "0");
                kolonlar.Add("YuklenmeTarih", "null");
                string talimatinsert = VeriTabaniIslemleri.VeriEklemeCumlesiOlustur("Talimatlar", kolonlar);
                int sonuc = VeriTabaniIslemleri.KomutIsle(talimatinsert);
                if (sonuc > 0)
                {
                    MessageBox.Show("Talimat Kaydedildi...");
                    textBoxYuklenecekTutar.Clear();
                    cmbBoxAkbiller.SelectedIndex = -1;
                    cmbBoxAkbiller.Text = "Akbil seciniz...";
                    groupBoxYukleme.Enabled = false;
                    TalimatlariDataGrideGetir(checkBoxTumunuGoster.Checked);
                    BekleyenTalimatSayisiniGetir();
                }
                else
                {
                    MessageBox.Show("Talimat kaydedilemedi !");
                }

            }
            catch (Exception hata)
            {

                MessageBox.Show("Talimat kaydedilemedi !" + hata.Message); ;
            }
        }

        private void checkBoxTumunuGoster_CheckedChanged(object sender, EventArgs e)
        {
            TalimatlariDataGrideGetir(checkBoxTumunuGoster.Checked);
        }

        private void anaMenuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmAnasayfa frma = new FrmAnasayfa();
            this.Hide();
            frma.Show();
        }

        private void cikisYapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Güle güle çıkış yapıldı... ");
            GenelIslemler.GirisYapanKullaniciAdSoyad = string.Empty;
            GenelIslemler.GirisYapanKullaniciID = 0;

            foreach (Form item in Application.OpenForms)
            {
                if (item.Name != "Form1")
                {
                    item.Hide();
                }
            }

            Application.OpenForms["Form1"].Show();
        }

        private void timerBekleyenTalimat_Tick(object sender, EventArgs e)
        {
            if (lblBekleyenTalimat.Text != "0")
            {
                if (DateTime.Now.Second % 2 == 0)
                {
                    lblBekleyenTalimat.Font = new Font("Segoe UI", 40);
                    lblBekleyenTalimat.ForeColor = Color.Cyan;
                }
                else
                {
                    lblBekleyenTalimat.Font = new Font("Segoe UI", 25);
                    lblBekleyenTalimat.ForeColor = Color.Red;
                }
            }
        }

        private void talimatiIptalEtToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                int sayac = 0;
                foreach (DataGridViewRow item in dataGridViewTalimatlar.SelectedRows)
                {
                    //Yuklenmis bir talimat iptal edilemez/silinemez.
                    if ((bool)item.Cells["YuklendiMi"].Value)
                    {
                        MessageBox.Show($"DİKKAT {item.Cells["Akbil"]} akbilin {item.Cells["Yüklenecek Tutar"]} liralık yüklemesi yapılmıştır. YÜKLENEN TALİMAT İPTAL EDİLEMEZ/SİLİNEMEZ! \nİşlemlerinize devam etmek için tamama basınız.");
                        continue;   
                    }//if bitti.

                    sayac += VeriTabaniIslemleri.VeriSil("Talimatlar", $"Id={item.Cells["Id"].Value}");

                }//foreach bitti.

                MessageBox.Show($"Seçtiniğiz {sayac} adet talimat sistemden silinmiştir.");
                TalimatlariDataGrideGetir();
                BekleyenTalimatSayisiniGetir();

            }
            catch (Exception hata)
            {

                MessageBox.Show("Beklenmedik bir hata oluştu ! " + hata.Message); ;
            }
        }
    }
}
