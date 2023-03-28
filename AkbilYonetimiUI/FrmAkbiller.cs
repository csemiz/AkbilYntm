using AkbilYntmIsKatmani;
using AkbilYntmVeriKatmani;

namespace AkbilYonetimiUI
{
    public partial class FrmAkbiller : Form
    {
        IveriTabaniIslemleri veriTabaniIslemleri = new SQLVeriTabaniIslemleri();
        public FrmAkbiller()
        {
            InitializeComponent();
        }

        private void btnKaydet_Click(object sender, EventArgs e)
        {
            try
            {
                //kontroller
                if (cmbBoxAkbilTipleri.SelectedIndex < 0)
                {
                    MessageBox.Show("Lutfen ekleyeceginiz akbilin türünü seciniz !");
                    return;
                }
                if (maskedTextBoxAkbilNo.Text.Length < 16)
                {
                    MessageBox.Show("Akbil No 16 haneli olmak zorundadır!");
                    return;
                }
                Dictionary<string, object> yeniAkbilBilgileri = new Dictionary<string, object>();
                yeniAkbilBilgileri.Add("AkbilNo", $"'{maskedTextBoxAkbilNo.Text}'");
                yeniAkbilBilgileri.Add("Bakiye", 0);
                yeniAkbilBilgileri.Add("AkbilTipi", $"'{cmbBoxAkbilTipleri.SelectedItem}'");
                yeniAkbilBilgileri.Add("EklenmeTarihi", $"'{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}'");
                yeniAkbilBilgileri.Add("VizelendigiTarih", "null");
                yeniAkbilBilgileri.Add("AkbilSahibiId", GenelIslemler.GirisYapanKullaniciID);


                string insertCumle = veriTabaniIslemleri.VeriEklemeCumlesiOlustur("Akbiller", yeniAkbilBilgileri);
                int sonuc = veriTabaniIslemleri.KomutIsle(insertCumle);
                if (sonuc > 0)
                {
                    MessageBox.Show("Akbil eklendi");
                    DataGridViewiDoldur();
                    maskedTextBoxAkbilNo.Clear();
                    cmbBoxAkbilTipleri.SelectedIndex = -1;
                    cmbBoxAkbilTipleri.Text = "Akbil Tipi Seçiniz";
                }
                else
                {
                    MessageBox.Show("Akbil eklenemedi ! ");

                }
            }

            catch (Exception hata)
            {

                MessageBox.Show("Beklenmedik bir hata oluştu !" + hata.Message);
            }
        }

        private void FrmAkbiller_Load(object sender, EventArgs e)
        {
            cmbBoxAkbilTipleri.Text = "Akbil tipi seçiniz...";
            cmbBoxAkbilTipleri.SelectedIndex = -1;

            DataGridViewiDoldur();
        }

        private void DataGridViewiDoldur()
        {
            try
            {
                dataGridViewAkbiller.DataSource = veriTabaniIslemleri.VeriGetir("Akbiller", kosullar: $"AkbilSahibiId={GenelIslemler.GirisYapanKullaniciID}");

                //bazi kolonlar gizlensin
                dataGridViewAkbiller.Columns["AkbilSahibiId"].Visible = false;
                dataGridViewAkbiller.Columns["VizelendigiTarih"].HeaderText = "Vizelendiği Tarih";
                dataGridViewAkbiller.Columns["VizelendigiTarih"].Width = 200;

            }
            catch (Exception hata)
            {
                MessageBox.Show("Akbilleri listeleyemedim !" + hata.Message);
            }
        }

        private void dataGridViewAkbiller_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void çIKIŞYAPToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}
