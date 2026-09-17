using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Threading;

namespace StajProje
{
    public partial class Form1 : Form
    {
        private DataTable dataTable = new DataTable();
        private int currentIndex = 0;
        private string connectionString = @"Data Source=BEGUMSS\SQLEXPRESS;Initial Catalog=master;Integrated Security=True;TrustServerCertificate=True";

        // ZAMANLAYICI VE KONTROL DEĞİŞKENLERİ
        private string eskiKod = "";
        private System.Windows.Forms.Timer rpaZamanlayici;
        private bool buHaftaGonderildi = false;

        public Form1()
        {
            InitializeComponent();

            this.Load -= Form1_Load;
            this.Load += Form1_Load;

            // KESİN BAĞLANTILAR
            toolStripButton14.Click -= ToolStripButton14_Click_EnGeri;
            toolStripButton14.Click += ToolStripButton14_Click_EnGeri;

            toolStripButton15.Click -= ToolStripButton15_Click_Geri;
            toolStripButton15.Click += ToolStripButton15_Click_Geri;

            toolStripButton16.Click -= ToolStripButton16_Click_Ileri;
            toolStripButton16.Click += ToolStripButton16_Click_Ileri;

            toolStripButton17.Click -= ToolStripButton17_Click_EnIleri;
            toolStripButton17.Click += ToolStripButton17_Click_EnIleri;

            toolStripButton13.Click -= ToolStripButton13_Click_Arama;
            toolStripButton13.Click += ToolStripButton13_Click_Arama;

            toolStripButton9.Click -= toolStripButton9_Click;
            toolStripButton9.Click += toolStripButton9_Click;

            toolStripButton12.Click -= toolStripButton12_Click;
            toolStripButton12.Click += toolStripButton12_Click;

            toolStripButton8.Click -= toolStripButton8_Click;
            toolStripButton8.Click += toolStripButton8_Click;

            toolStripButton10.Click -= toolStripButton10_Click;
            toolStripButton10.Click += toolStripButton10_Click;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            VerileriYukle();

            // Enter tuşu ile kutular arası geçiş ayarı
            foreach (Control ctrl in this.Controls)
            {
                if (ctrl is TextBox txt)
                {
                    txt.KeyDown += TextBox_KeyDown_EnterGecis;
                }
                else if (ctrl is Panel || ctrl is GroupBox)
                {
                    foreach (Control subCtrl in ctrl.Controls)
                    {
                        if (subCtrl is TextBox subTxt)
                        {
                            subTxt.KeyDown += TextBox_KeyDown_EnterGecis;
                        }
                    }
                }
            }

            // ==========================================
            // OTOMATİK CUMA 17:00 ZAMANLAYICISI (TIMER)
            // ==========================================
            rpaZamanlayici = new System.Windows.Forms.Timer();
            rpaZamanlayici.Interval = 60000; // Her 60 saniyede (1 dakikada) bir saati kontrol eder
            rpaZamanlayici.Tick += RpaZamanlayici_Tick;
            rpaZamanlayici.Start();
        }

        private void TextBox_KeyDown_EnterGecis(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == System.Windows.Forms.Keys.Enter)
            {
                e.SuppressKeyPress = true;
                this.SelectNextControl((Control)sender, true, true, true, true);
            }
        }

        private void VerileriYukle()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT Kod, Ad, Adres, Semt, Sehir, Ulke, PostaKodu, Telefon, Fax, VerDar, VerNo, Yetkili1, Yetkili2, EpostaAdr, WebAdr FROM dbo.AdresAna ORDER BY Ad ASC";
                using (SqlDataAdapter adapter = new SqlDataAdapter(query, connection))
                {
                    try
                    {
                        dataTable.Clear();
                        adapter.Fill(dataTable);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Veri çekme hatası: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            if (dataTable.Rows.Count > 0)
            {
                currentIndex = 0;
                KaydiEkranaYaz(currentIndex);
            }
        }

        private void KaydiEkranaYaz(int index)
        {
            if (dataTable.Rows.Count > 0 && index >= 0 && index < dataTable.Rows.Count)
            {
                DataRow row = dataTable.Rows[index];

                textBox1.Text = row["Kod"]?.ToString();
                eskiKod = textBox1.Text;

                textBox2.Text = row["Ad"]?.ToString();
                textBox3.Text = row["Adres"]?.ToString();
                textBox4.Text = row["Semt"]?.ToString();
                textBox5.Text = row["Sehir"]?.ToString();
                textBox6.Text = row["Ulke"]?.ToString();
                textBox7.Text = row["PostaKodu"]?.ToString();
                textBox8.Text = row["Telefon"]?.ToString();
                textBox9.Text = row["Fax"]?.ToString();
                textBox10.Text = row["VerDar"]?.ToString();
                textBox11.Text = row["VerNo"]?.ToString();
                textBox16.Text = row["EpostaAdr"]?.ToString();
                textBox17.Text = row["WebAdr"]?.ToString();

                string yetkili1 = row["Yetkili1"]?.ToString();
                if (!string.IsNullOrEmpty(yetkili1) && yetkili1.Contains(" - "))
                {
                    string[] parcalar = yetkili1.Split(new string[] { " - " }, StringSplitOptions.None);
                    textBox12.Text = parcalar[0];
                    textBox14.Text = parcalar.Length > 1 ? parcalar[1] : "";
                }
                else
                {
                    textBox12.Text = yetkili1;
                    textBox14.Text = "";
                }

                string yetkili2 = row["Yetkili2"]?.ToString();
                if (!string.IsNullOrEmpty(yetkili2) && yetkili2.Contains(" - "))
                {
                    string[] parcalar = yetkili2.Split(new string[] { " - " }, StringSplitOptions.None);
                    textBox13.Text = parcalar[0];
                    textBox15.Text = parcalar.Length > 1 ? parcalar[1] : "";
                }
                else
                {
                    textBox13.Text = yetkili2;
                    textBox15.Text = "";
                }
            }
        }

        // ==========================================
        // NAVİGASYON TUŞLARI (OKLAR)
        // ==========================================

        private void ToolStripButton14_Click_EnGeri(object sender, EventArgs e)
        {
            if (dataTable.Rows.Count > 0)
            {
                currentIndex = 0;
                KaydiEkranaYaz(currentIndex);
            }
            else MessageBox.Show("Gösterilecek kayıt bulunamadı!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void ToolStripButton15_Click_Geri(object sender, EventArgs e)
        {
            if (dataTable.Rows.Count > 0)
            {
                currentIndex--;
                if (currentIndex < 0)
                {
                    currentIndex = 0;
                    MessageBox.Show("Zaten ilk kayıttasınız!", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                KaydiEkranaYaz(currentIndex);
            }
            else MessageBox.Show("Gösterilecek kayıt bulunamadı!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void ToolStripButton16_Click_Ileri(object sender, EventArgs e)
        {
            if (dataTable.Rows.Count > 0)
            {
                currentIndex++;
                if (currentIndex >= dataTable.Rows.Count)
                {
                    currentIndex = dataTable.Rows.Count - 1;
                    MessageBox.Show("Zaten en son kayıttasınız!", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                KaydiEkranaYaz(currentIndex);
            }
            else MessageBox.Show("Gösterilecek kayıt bulunamadı!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void ToolStripButton17_Click_EnIleri(object sender, EventArgs e)
        {
            if (dataTable.Rows.Count > 0)
            {
                currentIndex = dataTable.Rows.Count - 1;
                KaydiEkranaYaz(currentIndex);
            }
            else MessageBox.Show("Gösterilecek kayıt bulunamadı!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        // ==========================================
        // CANLI ARAMA EKRANI (BÜYÜTEÇ)
        // ==========================================
        private void ToolStripButton13_Click_Arama(object sender, EventArgs e)
        {
            Form aramaFormu = new Form();
            aramaFormu.Text = "Kayıt Arama ve Filtreleme - MEGA İŞ ÇÖZÜMLERİ";
            aramaFormu.Size = new Size(1100, 600);
            aramaFormu.StartPosition = FormStartPosition.CenterScreen;

            Panel ustPanel = new Panel();
            ustPanel.Height = 55;
            ustPanel.Dock = DockStyle.Top;
            ustPanel.BackColor = Color.WhiteSmoke;

            Label lblAra = new Label();
            lblAra.Text = "🔍 Firma Adı veya Kod Ara:";
            lblAra.AutoSize = true;
            lblAra.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblAra.ForeColor = Color.DarkSlateGray;
            lblAra.Location = new Point(15, 17);

            TextBox txtAra = new TextBox();
            txtAra.Size = new Size(350, 26);
            txtAra.Location = new Point(200, 14);
            txtAra.Font = new Font("Segoe UI", 9.5F);

            ustPanel.Controls.Add(lblAra);
            ustPanel.Controls.Add(txtAra);

            DataGridView dgv = new DataGridView();
            dgv.Dock = DockStyle.Fill;
            dgv.ReadOnly = true;
            dgv.AllowUserToAddRows = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            Panel altPanel = new Panel();
            altPanel.Height = 40;
            altPanel.Dock = DockStyle.Bottom;
            altPanel.BackColor = Color.WhiteSmoke;

            Label lblSayac = new Label();
            lblSayac.AutoSize = true;
            lblSayac.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblSayac.ForeColor = Color.FromArgb(47, 79, 79);
            lblSayac.Location = new Point(15, 10);

            void AramaListesiniYukle(string arananKelime = "")
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT Kod, Ad, Adres, Semt, Sehir, Ulke, PostaKodu, Telefon, Fax, VerDar, VerNo, Yetkili1, Yetkili2, EpostaAdr, WebAdr FROM dbo.AdresAna ";
                    if (!string.IsNullOrWhiteSpace(arananKelime))
                    {
                        query += "WHERE Kod LIKE @aranan OR Ad LIKE @aranan ";
                    }
                    query += "ORDER BY Ad ASC";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        if (!string.IsNullOrWhiteSpace(arananKelime))
                        {
                            cmd.Parameters.AddWithValue("@aranan", "%" + arananKelime + "%");
                        }

                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            try
                            {
                                adapter.Fill(dt);
                                dgv.DataSource = dt;
                                lblSayac.Text = "Listelenen Kayıt Sayısı: " + dt.Rows.Count;
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Arama sırasında hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
            }

            AramaListesiniYukle();

            txtAra.TextChanged += (s, ev) =>
            {
                AramaListesiniYukle(txtAra.Text.Trim());
            };

            dgv.CellDoubleClick += (s, ev) =>
            {
                if (ev.RowIndex >= 0 && dgv.Rows[ev.RowIndex].Cells["Kod"].Value != null)
                {
                    string secilenKod = dgv.Rows[ev.RowIndex].Cells["Kod"].Value.ToString();

                    for (int i = 0; i < dataTable.Rows.Count; i++)
                    {
                        if (dataTable.Rows[i]["Kod"]?.ToString() == secilenKod)
                        {
                            currentIndex = i;
                            KaydiEkranaYaz(currentIndex);
                            break;
                        }
                    }
                    aramaFormu.Close();
                }
            };

            altPanel.Controls.Add(lblSayac);
            aramaFormu.Controls.Add(dgv);
            aramaFormu.Controls.Add(ustPanel);
            aramaFormu.Controls.Add(altPanel);
            aramaFormu.ShowDialog();
        }

        // ==========================================
        // YENİ MERKEZİ RAPOR GÖSTERİM METODU (AÇILIR MENÜ İÇİN)
        // ==========================================
        private void RaporEkraniAc(int raporTuru)
        {
            Form raporFormu = new Form();
            raporFormu.Text = raporTuru == 1 ? "Özet Adres Raporu - MEGA İŞ ÇÖZÜMLERİ" : "Detaylı Adres Raporu - MEGA İŞ ÇÖZÜMLERİ";
            raporFormu.Size = new Size(1100, 600);
            raporFormu.StartPosition = FormStartPosition.CenterScreen;

            DataGridView dgv = new DataGridView();
            dgv.Dock = DockStyle.Fill;
            dgv.ReadOnly = true;
            dgv.AllowUserToAddRows = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dgv.CellDoubleClick += (s, ev) =>
            {
                if (ev.RowIndex >= 0 && dgv.Rows[ev.RowIndex].Cells["Kod"].Value != null)
                {
                    string secilenKod = dgv.Rows[ev.RowIndex].Cells["Kod"].Value.ToString();
                    for (int i = 0; i < dataTable.Rows.Count; i++)
                    {
                        if (dataTable.Rows[i]["Kod"]?.ToString() == secilenKod)
                        {
                            currentIndex = i;
                            KaydiEkranaYaz(currentIndex);
                            break;
                        }
                    }
                    raporFormu.Close();
                }
            };

            Panel altPanel = new Panel();
            altPanel.Height = 55;
            altPanel.Dock = DockStyle.Bottom;
            altPanel.BackColor = Color.WhiteSmoke;

            Button btnPdfIndir = new Button();
            btnPdfIndir.Text = "📥 Ekranda Görünen Raporu PDF Olarak İndir";
            btnPdfIndir.Size = new Size(320, 38);
            btnPdfIndir.Location = new Point(15, 8);
            btnPdfIndir.BackColor = Color.FromArgb(47, 79, 79);
            btnPdfIndir.ForeColor = Color.White;
            btnPdfIndir.FlatStyle = FlatStyle.Flat;
            btnPdfIndir.FlatAppearance.BorderSize = 0;
            btnPdfIndir.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnPdfIndir.Cursor = Cursors.Hand;

            Label lblSayac = new Label();
            lblSayac.AutoSize = true;
            lblSayac.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblSayac.ForeColor = Color.DarkSlateGray;
            lblSayac.Location = new Point(350, 17);

            altPanel.Controls.Add(btnPdfIndir);
            altPanel.Controls.Add(lblSayac);

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = raporTuru == 1
                    ? "SELECT Kod, Ad, Adres, Semt, Sehir, Telefon, EpostaAdr FROM dbo.AdresAna ORDER BY Ad ASC"
                    : "SELECT Kod, Ad, Adres, Semt, Sehir, Ulke, PostaKodu, Telefon, Fax, VerDar, VerNo, Yetkili1, Yetkili2, EpostaAdr, WebAdr FROM dbo.AdresAna ORDER BY Ad ASC";

                using (SqlDataAdapter adapter = new SqlDataAdapter(query, connection))
                {
                    DataTable dt = new DataTable();
                    try
                    {
                        adapter.Fill(dt);
                        dgv.DataSource = dt;
                        lblSayac.Text = "Listelenen Kayıt Sayısı: " + dt.Rows.Count;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Rapor yüklenirken hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            btnPdfIndir.Click += (s, ev) =>
            {
                if (dgv.Rows.Count == 0)
                {
                    MessageBox.Show("İndirilecek kayıt bulunamadı!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "PDF Belgesi (*.pdf)|*.pdf";
                sfd.FileName = (raporTuru == 1 ? "Adres_Ozet_Raporu_" : "Adres_Detayli_Raporu_") + DateTime.Now.ToString("yyyyMMdd");
                sfd.Title = raporTuru == 1 ? "Rapor 1 - Özet PDF Kaydet" : "Rapor 2 - Tüm Detaylar PDF Kaydet";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    System.Drawing.Printing.PrintDocument pd = new System.Drawing.Printing.PrintDocument();
                    pd.DefaultPageSettings.Landscape = true;
                    pd.PrinterSettings.PrinterName = "Microsoft Print to PDF";
                    pd.PrinterSettings.PrintToFile = true;
                    pd.PrinterSettings.PrintFileName = sfd.FileName;

                    pd.PrintPage += (senderPrint, ePrint) =>
                    {
                        Graphics g = ePrint.Graphics;
                        Font baslikFont = new Font("Arial", 14, FontStyle.Bold);
                        Font altBaslikFont = new Font("Arial", 9, FontStyle.Regular);

                        if (raporTuru == 1)
                        {
                            Font tabloBaslikFont = new Font("Arial", 7, FontStyle.Bold);
                            Font icerikFont = new Font("Arial", 6, FontStyle.Regular);
                            g.DrawString("MEGA İŞ ÇÖZÜMLERİ - ADRES ÖZET RAPORU (RAPOR 1)", baslikFont, Brushes.DarkGreen, 20, 30);
                            g.DrawString("Rapor Tarihi: " + DateTime.Now.ToString("dd.MM.yyyy HH:mm"), altBaslikFont, Brushes.Black, 20, 55);
                            int y = 85; int cellHeight = 26;
                            string[] hedefKolonlar = { "Kod", "Ad", "Adres", "Semt", "Sehir", "Telefon", "EpostaAdr" };
                            int[] sutunGenislik = { 55, 230, 240, 95, 95, 115, 235 };
                            int x = 20;
                            for (int i = 0; i < hedefKolonlar.Length; i++) { g.FillRectangle(Brushes.DarkSlateGray, x, y, sutunGenislik[i], cellHeight); g.DrawRectangle(Pens.Black, x, y, sutunGenislik[i], cellHeight); g.DrawString(hedefKolonlar[i], tabloBaslikFont, Brushes.White, x + 4, y + 8); x += sutunGenislik[i]; }
                            y += cellHeight;
                            foreach (DataGridViewRow row in dgv.Rows) { x = 20; for (int i = 0; i < hedefKolonlar.Length; i++) { string veri = row.Cells[hedefKolonlar[i]].Value?.ToString() ?? ""; if (veri.Length > 44 && (i == 1 || i == 2)) veri = veri.Substring(0, 41) + "..."; g.DrawRectangle(Pens.Black, x, y, sutunGenislik[i], cellHeight); g.DrawString(veri, icerikFont, Brushes.Black, x + 4, y + 8); x += sutunGenislik[i]; } y += cellHeight; if (y > ePrint.MarginBounds.Bottom) break; }
                        }
                        else
                        {
                            Font tabloBaslikFont = new Font("Arial", 6, FontStyle.Bold);
                            Font icerikFont = new Font("Arial", 5, FontStyle.Regular);
                            g.DrawString("MEGA İŞ ÇÖZÜMLERİ - DETAYLI ADRES RAPORU (RAPOR 2)", baslikFont, Brushes.DarkGreen, 20, 30);
                            g.DrawString("Rapor Tarihi: " + DateTime.Now.ToString("dd.MM.yyyy HH:mm"), altBaslikFont, Brushes.Black, 20, 55);
                            int y = 85; int cellHeight = 25;
                            string[] kolAdlari = { "Kod", "Ad", "Adres", "Semt", "Sehir", "Telefon", "EpostaAdr", "Ulke", "PostaKodu", "Fax", "VerDar", "VerNo", "WebAdr", "Yetkili1", "Yetkili2" };
                            int[] sutunGenislik = { 45, 125, 105, 45, 45, 60, 95, 30, 52, 50, 50, 45, 115, 85, 85 };
                            int x = 20;
                            for (int i = 0; i < kolAdlari.Length; i++) { g.FillRectangle(Brushes.DarkSlateGray, x, y, sutunGenislik[i], cellHeight); g.DrawRectangle(Pens.Black, x, y, sutunGenislik[i], cellHeight); g.DrawString(kolAdlari[i], tabloBaslikFont, Brushes.White, x + 2, y + 8); x += sutunGenislik[i]; }
                            y += cellHeight;
                            foreach (DataGridViewRow row in dgv.Rows) { x = 20; for (int i = 0; i < kolAdlari.Length; i++) { string veri = ""; if (dgv.Columns.Contains(kolAdlari[i])) { veri = row.Cells[kolAdlari[i]].Value?.ToString() ?? ""; } int maxKarakter = (int)(sutunGenislik[i] / 3.4); if (veri.Length > maxKarakter) veri = veri.Substring(0, maxKarakter - 3) + "..."; g.DrawRectangle(Pens.Black, x, y, sutunGenislik[i], cellHeight); g.DrawString(veri, icerikFont, Brushes.Black, x + 2, y + 8); x += sutunGenislik[i]; } y += cellHeight; if (y > ePrint.MarginBounds.Bottom) break; }
                        }
                    };
                    try { pd.Print(); MessageBox.Show(raporTuru == 1 ? "Rapor 1 (Özet PDF) başarıyla kaydedildi!" : "Rapor 2 (Detaylı PDF) başarıyla kaydedildi!", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information); }
                    catch (Exception ex) { MessageBox.Show("PDF Hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                }
            };

            raporFormu.Controls.Add(dgv);
            raporFormu.Controls.Add(altPanel);
            raporFormu.ShowDialog();
        }

        // ==========================================
        // KAYDET (INSERT) - AKILLI WEB KONTROLLÜ
        // ==========================================
        private void toolStripButton12_Click(object sender, EventArgs e)
        {
            // --- 1. SİTE CANLILIK KONTROLÜ (Eğer web adresi girilmişse çalışır) ---
            string url = textBox17.Text.Trim();

            if (!string.IsNullOrWhiteSpace(url))
            {
                if (!url.StartsWith("http://") && !url.StartsWith("https://"))
                {
                    url = "https://" + url;
                }

                bool siteAktifMi = false;

                ChromeOptions ayarlar = new ChromeOptions();
                ayarlar.AddArgument("--headless"); // Ekran açılmadan arka planda hayalet gibi çalışır
                ayarlar.AddArgument("--disable-gpu");

                using (IWebDriver driver = new ChromeDriver(ayarlar))
                {
                    try
                    {
                        // Maksimum 7 saniye beklesin, açılmazsa ölü kabul etsin
                        driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(7);
                        driver.Navigate().GoToUrl(url);
                        siteAktifMi = true; // Site açıldı, sorun yok!
                    }
                    catch (Exception)
                    {
                        siteAktifMi = false; // Hata verdi veya süre doldu, site ölü!
                    }
                    finally
                    {
                        driver.Quit(); // Tarayıcıyı ram'de bırakmamak için kesinlikle kapatıyoruz
                    }
                }

                // Eğer site kapalıysa kullanıcıya onayı soruyoruz
                if (!siteAktifMi)
                {
                    DialogResult cevap = MessageBox.Show($"⚠️ Dikkat! Girdiğiniz web sitesi ({url}) şu an kapalı, çökmüş veya hatalı yazılmış görünüyor.\n\nEmin misiniz, bu firmayı yine de veritabanına ekleyelim mi?", "Kırık Bağlantı Uyarısı", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                    if (cevap == DialogResult.No)
                    {
                        return; // Kullanıcı "Hayır" derse SQL kaydetme işlemini tamamen iptal et!
                    }
                }
            }

            // --- 2. MEVCUT KAYDETME ---
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO dbo.AdresAna (Kod, Ad, Adres, Semt, Sehir, Ulke, PostaKodu, Telefon, Fax, VerDar, VerNo, Yetkili1, Yetkili2, EpostaAdr, WebAdr) " +
                               "VALUES (@Kod, @Ad, @Adres, @Semt, @Sehir, @Ulke, @PostaKodu, @Telefon, @Fax, @VerDar, @VerNo, @Yetkili1, @Yetkili2, @EpostaAdr, @WebAdr)";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Kod", textBox1.Text);
                    command.Parameters.AddWithValue("@Ad", textBox2.Text);
                    command.Parameters.AddWithValue("@Adres", textBox3.Text);
                    command.Parameters.AddWithValue("@Semt", textBox4.Text);
                    command.Parameters.AddWithValue("@Sehir", textBox5.Text);
                    command.Parameters.AddWithValue("@Ulke", textBox6.Text);
                    command.Parameters.AddWithValue("@PostaKodu", textBox7.Text);
                    command.Parameters.AddWithValue("@Telefon", textBox8.Text);
                    command.Parameters.AddWithValue("@Fax", textBox9.Text);
                    command.Parameters.AddWithValue("@VerDar", textBox10.Text);
                    command.Parameters.AddWithValue("@VerNo", textBox11.Text);
                    command.Parameters.AddWithValue("@EpostaAdr", textBox16.Text);
                    command.Parameters.AddWithValue("@WebAdr", textBox17.Text);

                    string yetkili1Bilgi = textBox12.Text + (string.IsNullOrEmpty(textBox14.Text) ? "" : " - " + textBox14.Text);
                    command.Parameters.AddWithValue("@Yetkili1", yetkili1Bilgi);

                    string yetkili2Bilgi = textBox13.Text + (string.IsNullOrEmpty(textBox15.Text) ? "" : " - " + textBox15.Text);
                    command.Parameters.AddWithValue("@Yetkili2", yetkili2Bilgi);

                    try
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                        MessageBox.Show("Kayıt başarıyla eklendi!", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        VerileriYukle();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        // ==========================================
        // GÜNCELLE
        // ==========================================
        private void toolStripButton10_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Lütfen Kod alanını boş bırakmayın!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(eskiKod))
            {
                eskiKod = textBox1.Text;
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "UPDATE dbo.AdresAna SET Kod=@YeniKod, Ad=@Ad, Adres=@Adres, Semt=@Semt, Sehir=@Sehir, Ulke=@Ulke, " +
                               "PostaKodu=@PostaKodu, Telefon=@Telefon, Fax=@Fax, VerDar=@VerDar, VerNo=@VerNo, " +
                               "Yetkili1=@Yetkili1, Yetkili2=@Yetkili2, EpostaAdr=@EpostaAdr, WebAdr=@WebAdr WHERE Kod=@EskiKod";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@YeniKod", textBox1.Text);
                    command.Parameters.AddWithValue("@EskiKod", eskiKod);
                    command.Parameters.AddWithValue("@Ad", textBox2.Text);
                    command.Parameters.AddWithValue("@Adres", textBox3.Text);
                    command.Parameters.AddWithValue("@Semt", textBox4.Text);
                    command.Parameters.AddWithValue("@Sehir", textBox5.Text);
                    command.Parameters.AddWithValue("@Ulke", textBox6.Text);
                    command.Parameters.AddWithValue("@PostaKodu", textBox7.Text);
                    command.Parameters.AddWithValue("@Telefon", textBox8.Text);
                    command.Parameters.AddWithValue("@Fax", textBox9.Text);
                    command.Parameters.AddWithValue("@VerDar", textBox10.Text);
                    command.Parameters.AddWithValue("@VerNo", textBox11.Text);
                    command.Parameters.AddWithValue("@EpostaAdr", textBox16.Text);
                    command.Parameters.AddWithValue("@WebAdr", textBox17.Text);

                    string yetkili1Bilgi = textBox12.Text + (string.IsNullOrEmpty(textBox14.Text) ? "" : " - " + textBox14.Text);
                    command.Parameters.AddWithValue("@Yetkili1", yetkili1Bilgi);

                    string yetkili2Bilgi = textBox13.Text + (string.IsNullOrEmpty(textBox15.Text) ? "" : " - " + textBox15.Text);
                    command.Parameters.AddWithValue("@Yetkili2", yetkili2Bilgi);

                    try
                    {
                        connection.Open();
                        int etkilenen = command.ExecuteNonQuery();

                        if (etkilenen > 0)
                        {
                            MessageBox.Show("Kayıt başarıyla güncellendi!", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            eskiKod = textBox1.Text;
                            VerileriYukle();
                        }
                        else
                        {
                            MessageBox.Show("Güncellenecek kayıt veritabanında bulunamadı.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Güncelleme sırasında hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        // ==========================================
        // SİLME
        // ==========================================
        private void toolStripButton9_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Lütfen silmek için önce ekrana bir kayıt getirin!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult cevap = MessageBox.Show($"'{textBox2.Text}' (Kod: {textBox1.Text}) firmasını silmek istediğinize emin misiniz?", "Kayıt Silme Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (cevap == DialogResult.Yes)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "DELETE FROM dbo.AdresAna WHERE Kod = @Kod";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Kod", textBox1.Text);

                        try
                        {
                            connection.Open();
                            int etkilenenSatir = command.ExecuteNonQuery();

                            if (etkilenenSatir > 0)
                            {
                                MessageBox.Show("Kayıt başarıyla silindi!", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                VerileriYukle();

                                if (dataTable.Rows.Count == 0)
                                {
                                    toolStripButton8_Click(null, null);
                                }
                            }
                            else
                            {
                                MessageBox.Show("Kayıt silinemedi. Zaten silinmiş veya bulunamamış olabilir.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Silme sırasında SQL hatası oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
        }

        // ==========================================
        // TEMİZLE
        // ==========================================
        private void toolStripButton8_Click(object sender, EventArgs e)
        {
            VerileriYukle();

            foreach (Control ctrl in this.Controls)
            {
                if (ctrl is TextBox)
                {
                    ctrl.Text = string.Empty;
                }
                else if (ctrl is Panel || ctrl is GroupBox)
                {
                    foreach (Control subCtrl in ctrl.Controls)
                    {
                        if (subCtrl is TextBox)
                        {
                            subCtrl.Text = string.Empty;
                        }
                    }
                }
            }

            eskiKod = "";
            textBox1.Focus();
        }

        private void toolStripButton16_Click_1(object sender, EventArgs e) { }
        private void textBox1_TextChanged(object sender, EventArgs e) { }
        private void textBox3_TextChanged(object sender, EventArgs e) { }
        private void label7_Click(object sender, EventArgs e) { }
        private void label12_Click(object sender, EventArgs e) { }
        private void statusStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e) { }
        private void adresToolStripMenuItem_Click(object sender, EventArgs e) { }
        private void groupBox1_Enter(object sender, EventArgs e) { }
        private void label12_Click_1(object sender, EventArgs e) { }
        private void toolStripButton15_Click(object sender, EventArgs e) { }

        // ==========================================
        // ZAMANLAYICI (HER DAKİKA SAATİ KONTROL EDER)
        // ==========================================
        private void RpaZamanlayici_Tick(object sender, EventArgs e)
        {
            DateTime suAn = DateTime.Now;

            // Gün Cuma (Friday) ve Saat 17:00 ise
            if (suAn.DayOfWeek == DayOfWeek.Friday && suAn.Hour == 17 && suAn.Minute == 0)
            {
                if (!buHaftaGonderildi)
                {
                    RPABotunuCalistir();
                    buHaftaGonderildi = true;
                    Application.Exit(); // İşini bitiren bot dükkanı kapatır
                }
            }
            else
            {
                // Saat 17:00'ı geçtikten sonra kilidi aç
                buHaftaGonderildi = false;
            }
        }

        // ==========================================
        // TAMAMEN SESSİZ RPA HAYALET BOTU 
        // ==========================================
        private void RPABotunuCalistir()
        {
            try
            {
                DataTable dtBot = new DataTable();
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT Kod, Ad, Adres, Semt, Sehir, Ulke, PostaKodu, Telefon, Fax, VerDar, VerNo, Yetkili1, Yetkili2, EpostaAdr, WebAdr FROM dbo.AdresAna ORDER BY Ad ASC";
                    using (SqlDataAdapter adapter = new SqlDataAdapter(query, connection))
                    {
                        adapter.Fill(dtBot);
                    }
                }

                string dosyaAdi = "Haftalik_Firma_Raporu_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".pdf";
                string pdfYolu = System.IO.Path.Combine(System.IO.Path.GetTempPath(), dosyaAdi);

                System.Drawing.Printing.PrintDocument pd = new System.Drawing.Printing.PrintDocument();
                pd.DefaultPageSettings.Landscape = true;
                pd.PrinterSettings.PrinterName = "Microsoft Print to PDF";
                pd.PrinterSettings.PrintToFile = true;
                pd.PrinterSettings.PrintFileName = pdfYolu;

                pd.PrintPage += (senderPrint, ePrint) =>
                {
                    Graphics g = ePrint.Graphics;
                    Font baslikFont = new Font("Arial", 14, FontStyle.Bold);
                    Font altBaslikFont = new Font("Arial", 9, FontStyle.Regular);
                    Font tabloBaslikFont = new Font("Arial", 6, FontStyle.Bold);
                    Font icerikFont = new Font("Arial", 5, FontStyle.Regular);

                    g.DrawString("MEGA İŞ ÇÖZÜMLERİ - HAFTALIK OTOMATİK ADRES RAPORU (RPA)", baslikFont, Brushes.DarkGreen, 20, 30);
                    g.DrawString("Rapor Tarihi: " + DateTime.Now.ToString("dd.MM.yyyy HH:mm") + " | Üreten: RPA Bot", altBaslikFont, Brushes.Black, 20, 55);

                    int y = 85;
                    int cellHeight = 25;
                    string[] kolAdlari = { "Kod", "Ad", "Adres", "Semt", "Sehir", "Telefon", "EpostaAdr", "Ulke", "PostaKodu", "Fax", "VerDar", "VerNo", "WebAdr", "Yetkili1", "Yetkili2" };
                    int[] sutunGenislik = { 45, 125, 105, 45, 45, 60, 95, 30, 52, 50, 50, 45, 115, 85, 85 };

                    int x = 20;
                    for (int i = 0; i < kolAdlari.Length; i++)
                    {
                        g.FillRectangle(Brushes.DarkSlateGray, x, y, sutunGenislik[i], cellHeight);
                        g.DrawRectangle(Pens.Black, x, y, sutunGenislik[i], cellHeight);
                        g.DrawString(kolAdlari[i], tabloBaslikFont, Brushes.White, x + 2, y + 8);
                        x += sutunGenislik[i];
                    }
                    y += cellHeight;

                    foreach (DataRow row in dtBot.Rows)
                    {
                        x = 20;
                        for (int i = 0; i < kolAdlari.Length; i++)
                        {
                            string veri = row[kolAdlari[i]]?.ToString() ?? "";
                            int maxKarakter = (int)(sutunGenislik[i] / 3.4);
                            if (veri.Length > maxKarakter) veri = veri.Substring(0, maxKarakter - 3) + "...";

                            g.DrawRectangle(Pens.Black, x, y, sutunGenislik[i], cellHeight);
                            g.DrawString(veri, icerikFont, Brushes.Black, x + 2, y + 8);
                            x += sutunGenislik[i];
                        }
                        y += cellHeight;
                        if (y > ePrint.MarginBounds.Bottom) break;
                    }
                };

                pd.Print();

                using (System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage())
                {
                    mail.From = new System.Net.Mail.MailAddress("begumkayaa1@gmail.com", "Mega İş RPA Botu");
                    mail.To.Add("begumkayaa1@gmail.com");
                    mail.Subject = "Haftalık Güncel Firma Adres Listesi";
                    mail.Body = "Merhaba,\n\nİyi çalışmalar dilerim. Sistemdeki güncel firma adres kayıtlarının detaylı raporu ektedir.\n\nSaygılarımla,\nMega İş Çözümleri RPA Botu";

                    using (System.Net.Mail.Attachment ekDosya = new System.Net.Mail.Attachment(pdfYolu))
                    {
                        mail.Attachments.Add(ekDosya);

                        using (System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient("smtp.gmail.com", 587))
                        {
                            smtp.Credentials = new System.Net.NetworkCredential("begumkayaa1@gmail.com", "nrmcvjaepujgwpap");
                            smtp.EnableSsl = true;
                            smtp.Send(mail);
                        }
                    }
                }

                if (System.IO.File.Exists(pdfYolu))
                {
                    System.IO.File.Delete(pdfYolu);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("RPA Botu arka planda çalışırken bir hata oluştu:\n\n" + ex.Message, "RPA Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ==========================================
        // AÇILIR MENÜ RAPOR BUTONLARI
        // ==========================================
        private void rapor1ÖzetTabloToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RaporEkraniAc(1);
        }

        private void rapor2DetaylıTabloToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RaporEkraniAc(2);
        }

        // ==========================================
        // TXT İLE OTOMATİK VERİ YÜKLEME 
        // ==========================================
        private void tXTİleVeriYükleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Metin Dosyaları (*.txt)|*.txt";
            ofd.Title = "SQL Veritabanına Aktarılacak TXT Dosyasını Seçin";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                int basariliSayisi = 0;
                int zatenVarSayisi = 0;
                try
                {
                    string[] satirlar = File.ReadAllLines(ofd.FileName);

                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        foreach (string satir in satirlar)
                        {
                            if (string.IsNullOrWhiteSpace(satir)) continue;

                            string[] parcalar = satir.Split(new char[] { ',', ';' });
                            if (parcalar.Length >= 2)
                            {
                                try
                                {
                                    string GuvenliKes(string metin, int maxBoyut)
                                    {
                                        string temiz = metin?.Trim() ?? "";
                                        if (temiz.Length > maxBoyut) temiz = temiz.Substring(0, maxBoyut);
                                        return temiz;
                                    }

                                    string kod = GuvenliKes(parcalar[0], 10);
                                    if (string.IsNullOrEmpty(kod)) continue;

                                    string ad = GuvenliKes(parcalar[1], 50);
                                    string adres = GuvenliKes(parcalar.Length > 2 ? parcalar[2] : "", 100);
                                    string semt = GuvenliKes(parcalar.Length > 3 ? parcalar[3] : "", 50);
                                    string sehir = GuvenliKes(parcalar.Length > 4 ? parcalar[4] : "", 50);
                                    string ulke = GuvenliKes(parcalar.Length > 5 ? parcalar[5] : "", 50);
                                    string postaKodu = GuvenliKes(parcalar.Length > 6 ? parcalar[6] : "", 20);
                                    string telefon = GuvenliKes(parcalar.Length > 7 ? parcalar[7] : "", 30);
                                    string fax = GuvenliKes(parcalar.Length > 8 ? parcalar[8] : "", 30);
                                    string verDar = GuvenliKes(parcalar.Length > 9 ? parcalar[9] : "", 50);
                                    string verNo = GuvenliKes(parcalar.Length > 10 ? parcalar[10] : "", 30);
                                    string epostaAdr = GuvenliKes(parcalar.Length > 11 ? parcalar[11] : "", 50);
                                    string webAdr = GuvenliKes(parcalar.Length > 12 ? parcalar[12] : "", 50);
                                    string yetkili1 = GuvenliKes(parcalar.Length > 13 ? parcalar[13] : "", 50);
                                    string yetkili2 = GuvenliKes(parcalar.Length > 14 ? parcalar[14] : "", 50);

                                    string checkQuery = "SELECT COUNT(*) FROM dbo.AdresAna WHERE Kod = @Kod";
                                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, connection))
                                    {
                                        checkCmd.Parameters.AddWithValue("@Kod", kod);
                                        int mevcut = (int)checkCmd.ExecuteScalar();

                                        if (mevcut > 0)
                                        {
                                            zatenVarSayisi++;
                                        }
                                        else
                                        {
                                            string query = "INSERT INTO dbo.AdresAna (Kod, Ad, Adres, Semt, Sehir, Ulke, PostaKodu, Telefon, Fax, VerDar, VerNo, EpostaAdr, WebAdr, Yetkili1, Yetkili2) " +
                                                           "VALUES (@Kod, @Ad, @Adres, @Semt, @Sehir, @Ulke, @PostaKodu, @Telefon, @Fax, @VerDar, @VerNo, @EpostaAdr, @WebAdr, @Yetkili1, @Yetkili2)";

                                            using (SqlCommand cmd = new SqlCommand(query, connection))
                                            {
                                                cmd.Parameters.AddWithValue("@Kod", kod);
                                                cmd.Parameters.AddWithValue("@Ad", string.IsNullOrEmpty(ad) ? (object)DBNull.Value : ad);
                                                cmd.Parameters.AddWithValue("@Adres", string.IsNullOrEmpty(adres) ? (object)DBNull.Value : adres);
                                                cmd.Parameters.AddWithValue("@Semt", string.IsNullOrEmpty(semt) ? (object)DBNull.Value : semt);
                                                cmd.Parameters.AddWithValue("@Sehir", string.IsNullOrEmpty(sehir) ? (object)DBNull.Value : sehir);
                                                cmd.Parameters.AddWithValue("@Ulke", string.IsNullOrEmpty(ulke) ? (object)DBNull.Value : ulke);
                                                cmd.Parameters.AddWithValue("@PostaKodu", string.IsNullOrEmpty(postaKodu) ? (object)DBNull.Value : postaKodu);
                                                cmd.Parameters.AddWithValue("@Telefon", string.IsNullOrEmpty(telefon) ? (object)DBNull.Value : telefon);
                                                cmd.Parameters.AddWithValue("@Fax", string.IsNullOrEmpty(fax) ? (object)DBNull.Value : fax);
                                                cmd.Parameters.AddWithValue("@VerDar", string.IsNullOrEmpty(verDar) ? (object)DBNull.Value : verDar);
                                                cmd.Parameters.AddWithValue("@VerNo", string.IsNullOrEmpty(verNo) ? (object)DBNull.Value : verNo);
                                                cmd.Parameters.AddWithValue("@EpostaAdr", string.IsNullOrEmpty(epostaAdr) ? (object)DBNull.Value : epostaAdr);
                                                cmd.Parameters.AddWithValue("@WebAdr", string.IsNullOrEmpty(webAdr) ? (object)DBNull.Value : webAdr);
                                                cmd.Parameters.AddWithValue("@Yetkili1", string.IsNullOrEmpty(yetkili1) ? (object)DBNull.Value : yetkili1);
                                                cmd.Parameters.AddWithValue("@Yetkili2", string.IsNullOrEmpty(yetkili2) ? (object)DBNull.Value : yetkili2);

                                                if (cmd.ExecuteNonQuery() > 0)
                                                {
                                                    basariliSayisi++;
                                                }
                                            }
                                        }
                                    }
                                }
                                catch (Exception exItem)
                                {
                                    MessageBox.Show($"Bir kayıt eklenirken SQL'e takıldı!\n\nSebep: {exItem.Message}\n\nAtlanan Firma Kodu: {parcalar[0]}", "Dedektif Yakaladı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                }
                            }
                        }
                    }

                    string mesaj = $"TXT dosyası başarıyla okundu!\n\n" +
                                   $"✅ Yeni Eklenen Kayıt: {basariliSayisi}\n" +
                                   $"⚠️ Zaten Kayıtlı Olup Atlanan: {zatenVarSayisi}";

                    MessageBox.Show(mesaj, "Veri Aktarım Raporu", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    VerileriYukle();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("TXT okuma hatası: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // ==========================================
        // JSON İLE OTOMATİK VERİ YÜKLEME
        // ==========================================
        private void jSONİleVeriYükleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "JSON Dosyaları (*.json)|*.json";
            ofd.Title = "SQL Veritabanına Aktarılacak JSON Dosyasını Seçin";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                int basariliSayisi = 0;
                int zatenVarSayisi = 0;
                try
                {
                    string jsonIcerik = File.ReadAllText(ofd.FileName);

                    List<string> bloklar = new List<string>();
                    int basIndex = 0;
                    while ((basIndex = jsonIcerik.IndexOf("{", basIndex)) != -1)
                    {
                        int bitIndex = jsonIcerik.IndexOf("}", basIndex);
                        if (bitIndex != -1)
                        {
                            bloklar.Add(jsonIcerik.Substring(basIndex, bitIndex - basIndex + 1));
                            basIndex = bitIndex + 1;
                        }
                        else break;
                    }

                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        foreach (string blok in bloklar)
                        {
                            if (!blok.Contains("Kod")) continue;

                            try
                            {
                                string GetJsonDeger(string anahtar, int maxKarakter)
                                {
                                    try
                                    {
                                        int arananIndex = blok.IndexOf("\"" + anahtar + "\"");
                                        if (arananIndex == -1) return "";
                                        int ikiNokta = blok.IndexOf(":", arananIndex);
                                        int tirnakBas = blok.IndexOf("\"", ikiNokta);
                                        int tirnakBit = blok.IndexOf("\"", tirnakBas + 1);
                                        if (tirnakBas != -1 && tirnakBit != -1)
                                        {
                                            string deger = blok.Substring(tirnakBas + 1, tirnakBit - tirnakBas - 1).Trim();
                                            if (deger.Length > maxKarakter) deger = deger.Substring(0, maxKarakter);
                                            return deger;
                                        }
                                    }
                                    catch { }
                                    return "";
                                }

                                string kod = GetJsonDeger("Kod", 20);
                                if (string.IsNullOrEmpty(kod)) continue;

                                string ad = GetJsonDeger("Ad", 50);
                                string adres = GetJsonDeger("Adres", 100);
                                string semt = GetJsonDeger("Semt", 50);
                                string sehir = GetJsonDeger("Sehir", 50);
                                string ulke = GetJsonDeger("Ulke", 50);
                                string postaKodu = GetJsonDeger("PostaKodu", 20);
                                string telefon = GetJsonDeger("Telefon", 30);
                                string fax = GetJsonDeger("Fax", 30);
                                string verDar = GetJsonDeger("VerDar", 50);
                                string verNo = GetJsonDeger("VerNo", 30);
                                string epostaAdr = GetJsonDeger("EpostaAdr", 50);
                                string webAdr = GetJsonDeger("WebAdr", 50);
                                string yetkili1 = GetJsonDeger("Yetkili1", 50);
                                string yetkili2 = GetJsonDeger("Yetkili2", 50);

                                string checkQuery = "SELECT COUNT(*) FROM dbo.AdresAna WHERE Kod = @Kod";
                                using (SqlCommand checkCmd = new SqlCommand(checkQuery, connection))
                                {
                                    checkCmd.Parameters.AddWithValue("@Kod", kod);
                                    int mevcut = (int)checkCmd.ExecuteScalar();

                                    if (mevcut > 0)
                                    {
                                        zatenVarSayisi++;
                                    }
                                    else
                                    {
                                        string query = "INSERT INTO dbo.AdresAna (Kod, Ad, Adres, Semt, Sehir, Ulke, PostaKodu, Telefon, Fax, VerDar, VerNo, EpostaAdr, WebAdr, Yetkili1, Yetkili2) " +
                                                       "VALUES (@Kod, @Ad, @Adres, @Semt, @Sehir, @Ulke, @PostaKodu, @Telefon, @Fax, @VerDar, @VerNo, @EpostaAdr, @WebAdr, @Yetkili1, @Yetkili2)";

                                        using (SqlCommand cmd = new SqlCommand(query, connection))
                                        {
                                            cmd.Parameters.AddWithValue("@Kod", kod);
                                            cmd.Parameters.AddWithValue("@Ad", string.IsNullOrEmpty(ad) ? (object)DBNull.Value : ad);
                                            cmd.Parameters.AddWithValue("@Adres", string.IsNullOrEmpty(adres) ? (object)DBNull.Value : adres);
                                            cmd.Parameters.AddWithValue("@Semt", string.IsNullOrEmpty(semt) ? (object)DBNull.Value : semt);
                                            cmd.Parameters.AddWithValue("@Sehir", string.IsNullOrEmpty(sehir) ? (object)DBNull.Value : sehir);
                                            cmd.Parameters.AddWithValue("@Ulke", string.IsNullOrEmpty(ulke) ? (object)DBNull.Value : ulke);
                                            cmd.Parameters.AddWithValue("@PostaKodu", string.IsNullOrEmpty(postaKodu) ? (object)DBNull.Value : postaKodu);
                                            cmd.Parameters.AddWithValue("@Telefon", string.IsNullOrEmpty(telefon) ? (object)DBNull.Value : telefon);
                                            cmd.Parameters.AddWithValue("@Fax", string.IsNullOrEmpty(fax) ? (object)DBNull.Value : fax);
                                            cmd.Parameters.AddWithValue("@VerDar", string.IsNullOrEmpty(verDar) ? (object)DBNull.Value : verDar);
                                            cmd.Parameters.AddWithValue("@VerNo", string.IsNullOrEmpty(verNo) ? (object)DBNull.Value : verNo);
                                            cmd.Parameters.AddWithValue("@EpostaAdr", string.IsNullOrEmpty(epostaAdr) ? (object)DBNull.Value : epostaAdr);
                                            cmd.Parameters.AddWithValue("@WebAdr", string.IsNullOrEmpty(webAdr) ? (object)DBNull.Value : webAdr);
                                            cmd.Parameters.AddWithValue("@Yetkili1", string.IsNullOrEmpty(yetkili1) ? (object)DBNull.Value : yetkili1);
                                            cmd.Parameters.AddWithValue("@Yetkili2", string.IsNullOrEmpty(yetkili2) ? (object)DBNull.Value : yetkili2);

                                            if (cmd.ExecuteNonQuery() > 0)
                                            {
                                                basariliSayisi++;
                                            }
                                        }
                                    }
                                }
                            }
                            catch (Exception exItem)
                            {
                                System.Diagnostics.Debug.WriteLine("JSON Blok Hatası: " + exItem.Message);
                            }
                        }
                    }

                    string mesaj = $"JSON dosyası başarıyla okundu!\n\n" +
                                   $"✅ Yeni Eklenen Kayıt: {basariliSayisi}\n" +
                                   $"⚠️ Zaten Kayıtlı Olup Atlanan: {zatenVarSayisi}";

                    MessageBox.Show(mesaj, "Veri Aktarım Raporu", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    VerileriYukle();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("JSON okuma hatası: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // ==========================================
        // YENİ EKLENEN: DÖVİZ KURLARI ARŞİV EKRANI
        // ==========================================
        private void DovizGecmisiEkraniAc()
        {
            Form dovizFormu = new Form();
            dovizFormu.Text = "TCMB Güncel ve Geçmiş Kurlar Arşivi - MEGA İŞ ÇÖZÜMLERİ";
            dovizFormu.Size = new Size(950, 480);
            dovizFormu.StartPosition = FormStartPosition.CenterScreen;

            Panel ustPanel = new Panel();
            ustPanel.Height = 50;
            ustPanel.Dock = DockStyle.Top;
            ustPanel.BackColor = Color.WhiteSmoke;

            Label lblBaslik = new Label();
            lblBaslik.Text = "🏦 Türkiye Cumhuriyet Merkez Bankası (TCMB) Döviz Arşivi";
            lblBaslik.AutoSize = true;
            lblBaslik.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblBaslik.ForeColor = Color.DarkSlateGray;
            lblBaslik.Location = new Point(15, 14);

            ustPanel.Controls.Add(lblBaslik);

            DataGridView dgv = new DataGridView();
            dgv.Dock = DockStyle.Fill;
            dgv.ReadOnly = true;
            dgv.AllowUserToAddRows = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT Tarih AS [Çekim Tarihi], DolarKuru AS [ABD Doları ($)], EuroKuru AS [Euro (€)], Durum AS [Kur Türü / Açıklama] FROM dbo.DovizGecmisi ORDER BY ID DESC";
                using (SqlDataAdapter adapter = new SqlDataAdapter(query, connection))
                {
                    DataTable dt = new DataTable();
                    try
                    {
                        adapter.Fill(dt);
                        dgv.DataSource = dt;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Döviz arşivi yüklenirken hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            dovizFormu.Controls.Add(dgv);
            dovizFormu.Controls.Add(ustPanel);
            dovizFormu.ShowDialog();
        }

        // ==========================================
        // AKILLI VE VERİTABANI ENTEGRELİ RPA DÖVİZ BOTU
        // ==========================================
        private void button1_Click(object sender, EventArgs e)
        {
            ChromeOptions ayarlar = new ChromeOptions();
            // Fethi Bey tarayıcıyı hiç görmesin, bot tamamen arka planda sessiz çalışsın istersen şu alt satırın başındaki // işaretini kaldırabilirsin:
            // ayarlar.AddArgument("--headless");

            using (IWebDriver driver = new ChromeDriver(ayarlar))
            {
                try
                {
                    // 1. Merkez Bankası'nın kurlar sayfasına git
                    driver.Navigate().GoToUrl("https://www.tcmb.gov.tr/kurlar/kurlar_tr.html");

                    // Sitenin ilk açılış hızı için 2 saniye bekle
                    Thread.Sleep(2000);

                    // 2. İlk hamle: Bugünün kurunu göstermek için "Göster" butonuna tıkla
                    IWebElement gosterButonu = driver.FindElement(By.XPath("//*[normalize-space(text())='Göster' or normalize-space(@value)='Göster']"));
                    IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                    js.ExecuteScript("arguments[0].click();", gosterButonu);

                    // Tablonun yüklenmesi için 3 saniye bekle
                    Thread.Sleep(3000);

                    // 3. AKILLI BOT KONTROLÜ: Tablo geldi mi yoksa saat 15:30'dan önce mi / tatil mi?
                    var dolarElementleri = driver.FindElements(By.XPath("//td[contains(text(), 'ABD DOLARI')]/following-sibling::td[1]"));

                    string aciklamaMesaji = "Güncel Bugünün Kuru";

                    // Eğer bugün için tablo açılmadıysa (15:30 öncesi veya hafta sonu/tatil durumu varsa):
                    if (dolarElementleri.Count == 0)
                    {
                        // 4. ŞOV KISMI: Merkez Bankası takviminde, bugünün solunda duran bir önceki (en son açıklanan) aktif kırmızı gün butonunu bul!
                        var enSonAciklananGun = driver.FindElements(By.XPath("//*[contains(@class, 'red') or contains(@class, 'active')]/preceding-sibling::*[1]"));

                        if (enSonAciklananGun.Count > 0)
                        {
                            js.ExecuteScript("arguments[0].click();", enSonAciklananGun[0]);
                        }
                        else
                        {
                            // Alternatif garanti XPath: Günler listesindeki sondan bir önceki öğeye tıkla
                            var oncekiGun = driver.FindElement(By.XPath("//ul[contains(@class, 'days') or contains(@class, 'date')]/li[last()-1] | //div[contains(@class, 'date-list')]//*[last()-1]"));
                            js.ExecuteScript("arguments[0].click();", oncekiGun);
                        }

                        Thread.Sleep(1500);

                        // Önceki günü seçtikten sonra tekrar "Göster" butonuna basıyoruz
                        gosterButonu = driver.FindElement(By.XPath("//*[normalize-space(text())='Göster' or normalize-space(@value)='Göster']"));
                        js.ExecuteScript("arguments[0].click();", gosterButonu);

                        Thread.Sleep(3000);

                        // Elementleri yeni açılan geçmiş gün tablosundan tekrar alıyoruz
                        dolarElementleri = driver.FindElements(By.XPath("//td[contains(text(), 'ABD DOLARI')]/following-sibling::td[1]"));
                        aciklamaMesaji = "En Son Açıklanan Geçmiş İş Günü Kuru (15:30 Öncesi / Tatil Modu)";
                    }

                    // 5. Veriyi çek, veritabanına kaydet ve tablo ekranını aç
                    if (dolarElementleri.Count > 0)
                    {
                        string dolarKuru = dolarElementleri[0].Text;

                        IWebElement euroElementi = driver.FindElement(By.XPath("//td[contains(text(), 'EURO')]/following-sibling::td[1]"));
                        string euroKuru = euroElementi.Text;

                        driver.Quit(); // Tarayıcıyı kapat

                        // --- VERİTABANINA KAYDETME İŞLEMİ ---
                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            string sql = "INSERT INTO dbo.DovizGecmisi (Tarih, DolarKuru, EuroKuru, Durum) VALUES (@Tarih, @Dolar, @Euro, @Durum)";
                            using (SqlCommand komut = new SqlCommand(sql, connection))
                            {
                                komut.Parameters.AddWithValue("@Tarih", DateTime.Now);
                                komut.Parameters.AddWithValue("@Dolar", dolarKuru);
                                komut.Parameters.AddWithValue("@Euro", euroKuru);
                                komut.Parameters.AddWithValue("@Durum", aciklamaMesaji);

                                connection.Open();
                                komut.ExecuteNonQuery();
                            }
                        }

                        MessageBox.Show($"✅ Yeni kur verisi başarıyla çekildi ve veritabanına arşivlendi!\n\n" +
                                        $"📌 Durum: {aciklamaMesaji}\n\n" +
                                        $"💵 ABD Doları: {dolarKuru} ₺\n" +
                                        $"💶 Euro: {euroKuru} ₺",
                                        "TCMB Akıllı Kur ve Arşivleme", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // --- ÇEKİLEN TÜM GEÇMİŞ KURLARI TABLO OLARAK EKRANDA GÖSTER ---
                        DovizGecmisiEkraniAc();
                    }
                    else
                    {
                        driver.Quit();
                        MessageBox.Show("⚠️ Merkez Bankası sunucularından güncel veya geçmiş kur tablosu alınamadı. Lütfen internet bağlantınızı kontrol edin.", "Bağlantı Uyarısı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                catch (Exception ex)
                {
                    driver.Quit();
                    MessageBox.Show("RPA Botu çalışırken bir hata oluştu:\n\n" + ex.Message, "RPA Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void tCMBKurArşiviToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // İstenildiği an geçmiş kur arşivini ekrana getirir:
            DovizGecmisiEkraniAc();
        }
    }
}