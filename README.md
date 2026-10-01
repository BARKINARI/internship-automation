# Stajyer Yönetim ve Başvuru Sistemi

Bu proje; staj başvurularının toplanması, değerlendirilmesi, kabul edilen öğrencilerin kullanıcı hesaplarının otomatik oluşturulması, stajyerlere proje atanması, takip/notlandırma yapılması ve öğrenci-yönetici arasında proje bazlı mesajlaşmanın yürütülmesini sağlayan kurumsal bir web platformudur.

---

## 🛠️ Mimari ve Kullanılan Teknolojiler

- **Backend:** .NET 8 Web API
- **Frontend / Arayüz:** .NET 8 MVC (Razor Views) & SB Admin 2 Dashboard
- **Mimari:** Clean Architecture (Domain, Application, Persistence, Core)
- **Tasarım Kalıpları:** CQRS (MediatR), Repository Pattern, Unit of Work
- **Veritabanı & ORM:** PostgreSQL & Entity Framework Core (Code-First)
- **Güvenlik & Yetkilendirme:** JWT (JSON Web Token), Hashing (HMACSHA512), Dinamik Modüler Rol ve Yetki Sistemi
- **Yardımcı Servisler:** Serilog (File & Database Logging), MailKit (E-Posta Servisi), AutoMapper, FluentValidation

---

## 📁 Katman Yapısı

```
StajSistemi/
├── Core/            # Çapraz kesen fonksiyonlar (Güvenlik, Loglama, Mail, İstisna Yönetimi)
├── Domain/          # Varlıklar (Entities) ve Enum tanımları
├── Application/     # İş kuralları, CQRS komut ve sorguları (Commands, Queries, DTOs)
├── Persistence/     # EF Core DbContext, Migration'lar ve Repository implementasyonları
├── WebAPI/          # RESTful API servisleri ve uç noktalar
└── WebUI/           # Öğrenci başvuru ve Admin yönetim arayüzü (MVC)
```

---

## Kurulum Kılavuzu (Deployment Guide)

### 1. Gereksinimler
- [.NET 8.0 SDK / Runtime](https://dotnet.microsoft.com/download/dotnet/8.0)
- [PostgreSQL](https://www.postgresql.org/) (Sürüm 13+)

---

### 2. Yapılandırma (`appsettings.json` / Ortam Değişkenleri)

Canlı ortama alırken `WebAPI/appsettings.json` (veya `appsettings.Production.json`) dosyasındaki aşağıdaki alanları canlı sunucu bilgilerinize göre güncelleyiniz:

#### a) Veritabanı Bağlantısı:
```json
"ConnectionStrings": {
  "StajSistemi": "Host=localhost;Port=5432;Database=StajSistemiDb;Username=postgres;Password=VERITABANI_SIFRENIZ;"
}
```

#### b) JWT Güvenlik Anahtarı:
```json
"TokenOptions": {
  "Audience": "YOUR_AUDIENCE_OR_DOMAIN",
  "Issuer": "YOUR_ISSUER_OR_DOMAIN",
  "AccessTokenExpiration": 60,
  "RefreshTokenTTL": 7,
  "SecurityKey": "EN_AZ_32_KARAKTER_UZUNLUGUNDA_GIZLI_GUVENLIK_ANAHTARINIZ"
}
```

#### c) E-Posta Servisi (SMTP):
Stajyerlere onay ve şifre e-postalarının gidebilmesi için:
```json
"MailSettings": {
  "Server": "smtp.gmail.com",
  "Port": 587,
  "SenderFullName": "Şirketiniz",
  "SenderEmail": "staj@sirketiniz.com",
  "UserName": "staj@sirketiniz.com",
  "Password": "SMTP_VEYA_UYGULAMA_SIFRENIZ",
  "AuthenticationRequired": true
}
```

#### d) WebUI API Adresi:
`WebUI/appsettings.json` içerisinde WebAPI'nin canlı sunucudaki adresi belirtilmelidir:
```json
"ApiSettings": {
  "BaseUrl": "https://api.sirketiniz.com" // veya yerel/sunucu portu
}
```

---

### 3. Veritabanı Migration'larını Çalıştırma

Terminal veya komut satırında çözüm kök dizinindeyken aşağıdaki komutu çalıştırarak PostgreSQL veritabanını tabloları ve varsayılan seed verileriyle birlikte oluşturunuz:

```bash
dotnet ef database update --project Persistence --startup-project WebAPI
```

*(Eğer `dotnet-ef` yüklü değilse: `dotnet tool install --global dotnet-ef` komutu ile yükleyebilirsiniz.)*

---

### 4. Varsayılan Yönetici (Super Admin) Girişi

Migration tamamlandığında sistemde hazır bir yönetici hesabı oluşturulur:
- **E-Posta:** `admin@ae.com`
- **Şifre:** `Admin123*`

> **Not:** Canlıya alındıktan sonra güvenlik nedeniyle bu şifrenin değiştirilmesi önerilir.

