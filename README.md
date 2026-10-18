# 🏢 Kurumsal Adres ve Raporlama Yönetim Sistemi Masaüstü Uygulaması

**Kurumsal Adres Yönetim Masaüstü Uygulaması**, kurumların geniş adres verilerini tek bir merkezden güvenle yönetmesini, veri tabloları üzerinden hızlı arama/gezinme yapmasını ve Selenium altyapısı ile TCMB döviz kurlarının otomatik olarak çekilmesini sağlayan C# / WinForms tabanlı bir masaüstü yazılımıdır.

> 🔗 Bu projenin [web](https://github.com/begummkayaa/kurumsal-adres-yonetim-web) ve [mobil](https://github.com/begummkayaa/kurumsal-adres-yonetim-mobil) sürümleri de bulunmaktadır. Üç sürüm aynı SQL Server veritabanını kullanır.

---

## 📑 İçindekiler
1. [Proje Vizyonu ve Özellikler](#-proje-vizyonu-ve-özellikler)
2. [Sistem Mimarisi](#-sistem-mimarisi)
3. [Kullanılan Teknolojiler](#-kullanılan-teknolojiler)
4. [Kurulum ve Çalıştırma](#-kurulum-ve-çalıştırma)
5. [Kullanım Senaryosu](#-kullanım-senaryosu)
6. [Geliştirici ve Proje Hakkında](#-geliştirici-ve-proje-hakkında)
7. [Lisans](#-lisans)

---

## ✨ Proje Vizyonu ve Özellikler

Kurumsal ölçekte adres verilerinin manuel yönetimi; zaman kaybına, hatalı veri girişlerine ve operasyonel aksaklıklara yol açar. Bu proje, masaüstü hızı ve web otomasyon gücünü birleştirerek verimliliği maksimuma çıkarmayı hedefler.

- 🗄️ **Güçlü Veritabanı Entegrasyonu:** SQL Server tabanlı mimarisi sayesinde binlerce kaydı yüksek performansla ilişkisel olarak saklar ve sorgular.
- 🤖 **Selenium Web Otomasyonu:** Entegre `OpenQA.Selenium` sürücüleri ile TCMB'nin web sitesinden güncel dolar ve euro kurlarını arka planda otomatik olarak çeker ve veritabanına kaydeder.
- ⏱️ **Zamanlayıcı (Timer) Desteği:** Belirli periyotlarda çalışan kontrol mekanizmalarıyla döviz kurlarının düzenli aralıklarla otomatik olarak güncellenmesini sağlar.
- ⚡ **Kolay Navigasyon ve Dinamik Veri Seti:** `DataTable` mimarisi ve özelleştirilmiş ToolStrip menüleri sayesinde kayıtlar arasında hızlıca gezinme (İleri/Geri) imkanı sunar.

---

## ⚙️ Sistem Mimarisi

Uygulamanın çalışma prensibi üç ana katmana dayanır:

1. **Veri Yönetim Katmanı (Data Layer):** SQL Server bağlantısı (`Microsoft.Data.SqlClient`) üzerinden dinamik `DataTable` nesneleri oluşturulur. Veriler bellekte optimize bir şekilde işlenir.
2. **Otomasyon Katmanı (Automation Layer):** Chrome WebDriver tetiklenerek TCMB web sitesindeki güncel döviz kurları okunur ve veritabanındaki kur arşivine kaydedilir.
3. **Kullanıcı Arayüzü (UI Layer):** Windows Forms mimarisi üzerinde yapılandırılmış ToolStrip butonları, Timer denetimleri ve özelleştirilmiş form bileşenleriyle kullanıcıya akıcı bir deneyim sunulur.

---

## 🛠️ Kullanılan Teknolojiler

- **Dil & Çerçeve:** C# (.NET / Windows Forms)
- **Veritabanı:** Microsoft SQL Server (`SqlClient`)
- **Web Otomasyonu:** Selenium WebDriver (`OpenQA.Selenium`, `OpenQA.Selenium.Chrome`)
- **Geliştirme Ortamı:** Visual Studio

---

## 🚀 Kurulum ve Çalıştırma

Projeyi yerel makinenizde çalıştırmak için aşağıdaki adımları sırasıyla izleyin:

### 1. Repoyu Klonlayın
```bash
git clone https://github.com/begummkayaa/kurumsal-adres-yonetim-masaustu.git
cd kurumsal-adres-yonetim-masaustu
```

### 2. Projeyi Visual Studio ile Açın
StajProje.slnx veya StajProje.csproj dosyasına çift tıklayarak projeyi Visual Studio'da açın.

### 3. Bağımlılıkları (NuGet Paketlerini) Geri Yükleyin
Visual Studio otomatik yüklemezse Package Manager Console üzerinden veya Solution -> Restore NuGet Packages adımıyla paketleri indirin:
```bash
Microsoft.Data.SqlClient
Selenium.WebDriver
Selenium.WebDriver.ChromeDriver
```

### 4. Veritabanı Bağlantısını Düzenleyin
Form1.cs içerisindeki connectionString değişkenini kendi SQL Server adresinize göre güncelleyin:
```bash
private string connectionString = @"Data Source=YOUR_SERVER_NAME;Initial Catalog=master;Integrated Security=True;TrustServerCertificate=True";
```

### 5. Uygulamayı Başlatın
Visual Studio üzerinden F5 tuşuna basarak veya Start butonuna tıklayarak uygulamayı derleyip çalıştırabilirsiniz.

## 💡 Kullanım Senaryosu
- Uygulama açıldığında veritabanı bağlantısı kurulur ve adres verileri DataTable içerisine yüklenir.

- Üst menüde yer alan İleri ve Geri butonları ile kayıtlar arasında hızlıca geçiş yapılabilir.

- Kur çekme işlemi başlatıldığında Selenium arka planda Chrome tarayıcısını açarak TCMB'den güncel döviz kurlarını okur ve kur arşivine kaydeder.

## 🏢 Geliştirici ve Proje Hakkında
Bu proje, kurumsal adres verilerinin yönetimini ve TCMB döviz kuru otomasyonunu tek bir masaüstü arayüzünde birleştirmek amacıyla Begüm Kaya tarafından geliştirilmiştir.

## 📄 Lisans
Bu proje MIT Lisansı altında lisanslanmıştır. Daha fazla bilgi için LICENSE dosyasına göz atabilirsiniz.
