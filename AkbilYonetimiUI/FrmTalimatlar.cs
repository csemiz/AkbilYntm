using AkbilYntmIsKatmani;
using AkbilYntmVeriKatmani;
using System.Collections;

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

        private void TalimatlariDataGrideGetir(bool tumunuGoster = false, string akbilNo=null)
        {
            try
            {
                string kosullar = $"KullaniciId={GenelIslemler.GirisYapanKullaniciID}";
                if (cmbBoxAkbiller.SelectedIndex>=0)
                {
                    akbilNo = cmbBoxAkbiller.Text;
                }
                if (!string.IsNullOrEmpty(akbilNo))
                {
                    kosullar += $" and Akbil like '%{akbilNo}%' ";
                }

                if (tumunuGoster)//tumunuGoster==true
                {
                    dataGridViewTalimatlar.DataSource = VeriTabaniIslemleri.VeriGetir("KullanicininTalimatlari", kosullar:kosullar);
                }
                else
                {
                    kosullar += " and YuklendiMi=0 ";
                    dataGridViewTalimatlar.DataSource = VeriTabaniIslemleri.VeriGetir("KullanicininTalimatlari", kosullar:kosullar);
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
            BekleyenTalimatSayisiniGetir();
            TalimatlariDataGrideGetir(checkBoxTumunuGoster.Checked);
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
                kolonlar.Add("YuklenecekTutar", textBoxYuklenecekTutar.Text.Trim().Replace(",", "."));
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
                        MessageBox.Show($"DİKKAT {item.Cells["Akbil"].Value} {item.Cells["YuklenecekTutar"].Value} liralık yüklemesi yapılmıştır. YÜKLENEN TALİMAT İPTAL EDİLEMEZ/SİLİNEMEZ! \nİşlemlerinize devam etmek için tamama basınız.");
                        continue;
                    }//if bitti.

                    sayac += VeriTabaniIslemleri.VeriSil("Talimatlar", $"Id={item.Cells["Id"].Value}");

                }//foreach bitti.

                MessageBox.Show($"Seçtiniğiz {sayac} talimat sistemden silinmiştir.");
                TalimatlariDataGrideGetir();
                BekleyenTalimatSayisiniGetir();

            }
            catch (Exception hata)
            {

                MessageBox.Show("Beklenmedik bir hata oluştu ! " + hata.Message); ;
            }
        }

        private void talimatiYukleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                int sayac = 0;
                foreach (DataGridViewRow item in dataGridViewTalimatlar.SelectedRows)
                {
                    //talimatlar tablosunu guncellemek
                    if ((bool)item.Cells["YuklendiMi"].Value)
                    {
                        continue;
                    }

                    Hashtable talimatkolonlar = new Hashtable();
                    talimatkolonlar.Add("YuklendiMi", 1);
                    talimatkolonlar.Add("YuklenmeTarih", $"'{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}'");

                    string talimatGuncelle = VeriTabaniIslemleri.VeriGuncellemeCumlesiOlustur("Talimatlar", talimatkolonlar, $"Id={item.Cells["Id"].Value}");

                    if (VeriTabaniIslemleri.KomutIsle(talimatGuncelle) > 0)
                    {
                        //akbilin mevcut bakiyesini ogren
                        decimal bakiye = Convert.ToDecimal(VeriTabaniIslemleri.VeriOku("Akbiller", new string[] { "Bakiye" }, $"AkbilNo='{item.Cells["Akbil"].Value.ToString()?.Substring(0, 16)}'")["Bakiye"]);


                        //var sonuc = veriTabaniIslemleri.VeriOku("Akbiller", new string[] { "Bakiye" },
                        //    $"AkbilNo='{item.Cells["Akbil"].Value.ToString()?.Substring(0, 15)}'");
                        //decimal bakiye = (decimal)sonuc["Bakiye"]; //Yukarıdaki ile ayni islem


                        //akbil bakiyesini güncellemek
                        Hashtable akbilkolon = new Hashtable();
                        var sonbakiye = (bakiye + (decimal)item.Cells["YuklenecekTutar"].Value).ToString().Replace(",", ".");

                        akbilkolon.Add("Bakiye", sonbakiye);

                        string akbilGuncelle = VeriTabaniIslemleri.VeriGuncellemeCumlesiOlustur("Akbiller", akbilkolon, $"AkbilNo='{item.Cells["Akbil"].Value.ToString()?.Substring(0, 16)}'");

                        sayac += VeriTabaniIslemleri.KomutIsle(akbilGuncelle);
                    }


                }//foreach bitti.
                MessageBox.Show($"{sayac} adet talimat akbile yüklendi ! ");
                TalimatlariDataGrideGetir();
                BekleyenTalimatSayisiniGetir();
            }
            catch (Exception hata)
            {

                MessageBox.Show("Beklenmedik bir hata oluştu" + hata.Message); ;
            }
        }
    }
}
