"<img width="750" height="286" alt="image" src="https://github.com/user-attachments/assets/a8425a57-45d7-4aba-b607-f61dd568ffc4" />
# AuthService 🔐

Modern, güvenli ve ölçeklenebilir bir kimlik doğrulama ve yetkilendirme servisi. Multi-tenant mimarisi ile birden fazla müşteriyi destekler.

## 📋 İçindekiler

- [Özellikler](#özellikler)
- [Teknolojiler](#teknolojiler)
- [Mimari](#mimari)
- [Kurulum](#kurulum)
- [Yapılandırma](#yapılandırma)
- [API Endpoints](#api-endpoints)
- [Güvenlik](#güvenlik)
- [Geliştirme](#geliştirme)

## ✨ Özellikler

### Kimlik Doğrulama ve Yetkilendirme
- **JWT Token Tabanlı Kimlik Doğrulama**: Güvenli, stateless authentication
- **Refresh Token Desteği**: Otomatik token yenileme mekanizması
- **Multi-Device Yönetimi**: Web ve mobil cihazlar için farklı token stratejileri
- **Role-Based Access Control (RBAC)**: Rol bazlı yetkilendirme sistemi
- **Policy-Based Authorization**: Özel yetkilendirme politikaları (SystemAdmin, AuthServiceOnly)

### Multi-Tenancy
- **Tenant İzolasyonu**: Her müşteri için ayrı veri izolasyonu
- **Domain Bazlı Tenant Yönetimi**: Tenant'lar domain ile tanımlanır
- **System Tenant Desteği**: Yönetim için özel sistem tenant'ı

### Cihaz ve Güvenlik Yönetimi
- **Client Type Detection**: Web/Mobile client otomatik algılama
- **Device ID Tracking**: Cihaz bazlı güvenlik kontrolü
- **IP ve User Agent Takibi**: Gelişmiş güvenlik logging
- **Cookie-Based Refresh Token**: Web uygulamaları için güvenli refresh token yönetimi

### Loglama ve İzleme
- **Serilog Integration**: Yapılandırılmış loglama
- **Seq Integration**: Merkezi log toplama ve analiz
- **Global Exception Handling**: Merkezi hata yönetimi

## 🛠 Teknolojiler

- **.NET 9**: En son framework özellikleri
- **ASP.NET Core Web API**: RESTful API geliştirme
- **Entity Framework Core**: ORM ve veritabanı yönetimi
- **PostgreSQL**: İlişkisel veritabanı
- **ASP.NET Core Identity**: Kimlik yönetimi framework'ü
- **JWT (JSON Web Tokens)**: Token bazlı kimlik doğrulama
- **Serilog + Seq**: Yapılandırılmış loglama
- **API Versioning**: Versiyon yönetimi (v1.0)


## 🚀 Kurulum

### Gereksinimler

- .NET 9 SDK
- PostgreSQL 13+
- (Opsiyonel) Docker
- (Opsiyonel) Seq Server

### Adımlar

1. **Repository'yi klonlayın** : " git clone https://github.com/gorkemkayas/AuthService.git cd AuthService "
2. **Bağımlılıkları yükleyin** : " dotnet restore "
3. **Veritabanı bağlantısını yapılandırın** :`appsettings.json` dosyasında PostgreSQL bağlantı dizesini güncelleyin: "{ "ConnectionStrings": { "AuthServiceDb": "Host=localhost;Database=authservice;Username=postgres;Password=yourpassword" } }"
4. **JWT Secret'ı yapılandırın** : " { "Jwt": { "Secret": "your-super-secret-key-at-least-32-characters-long" } } " 
5. **Veritabanı migration'larını çalıştırın** : " cd AuthService.API dotnet ef database update "
6. **Uygulamayı çalıştırın** : " dotnet run "
   **Not**: İlk çalıştırmada otomatik olarak bir "Default Tenant" ve "SuperAdmin" kullanıcısı oluşturulur.
### CORS Yapılandırması

**Development:**
- `http://localhost:3000`
- `https://localhost:3000`

**Production:**
- `*.kayas.dev` domain'leri
- `https://kayas.dev`

### Seq Loglama (Opsiyonel)
{ "Serilog": { "MinimumLevel": "Information", "WriteTo": [ { "Name": "Seq", "Args": { "serverUrl": "http://localhost:5341" } } ] } }


## 📡 API Endpoints

### Authentication (`/api/v1/auth`)

| Method | Endpoint | Açıklama | Auth |
|--------|----------|----------|------|
| POST | `/login` | Kullanıcı girişi | ❌ |
| POST | `/register` | Yeni kullanıcı kaydı | ❌ |
| POST | `/refresh` | Token yenileme | ❌ |

### Users Management (`/api/v1/admin/tenants/{tenantId}/users`)

| Method | Endpoint | Açıklama | Auth |
|--------|----------|----------|------|
| GET | `/` | Tenant kullanıcıları listele | SystemAdmin |
| POST | `/` | Yeni kullanıcı oluştur | SystemAdmin |
| GET | `/{userId}/roles` | Kullanıcı rollerini getir | SystemAdmin |
| POST | `/{userId}/roles` | Kullanıcı rollerini güncelle | SystemAdmin |


## 🔒 Güvenlik

### Token Stratejileri

**Web Uygulamaları:**
- Refresh token HttpOnly cookie'de saklanır
- Access token client-side'da tutulur
- CORS koruması aktif

**Mobil Uygulamalar:**
- Refresh token response body'de döner
- Device ID ile ek güvenlik katmanı
- Token başına device binding

### Authorization Policies

1. **SystemAdmin**: 
   - Token type: "system"
   - Role: "SuperAdmin"
   - Tam yönetim yetkisi

2. **AuthServiceOnly**:
   - Audience: "AuthService"
   - Servis-içi işlemler için

### Middleware'ler

- **GlobalExceptionHandlingMiddleware**: Merkezi hata yönetimi
- **ClientTypeMiddleware**: Client type detection (Web/Mobile)
- Authentication & Authorization middlewares

## 🧪 Geliştirme

### Database Migration Oluşturma


## 🤝 Katkıda Bulunma

1. Fork yapın
2. Feature branch oluşturun (`git checkout -b feature/amazing-feature`)
3. Değişikliklerinizi commit edin (`git commit -m 'feat: Add amazing feature'`)
4. Branch'inizi push edin (`git push origin feature/amazing-feature`)
5. Pull Request oluşturun

## 📄 Lisans

Bu proje MIT lisansı altında lisanslanmıştır.

## 👤 Geliştirici

**Görkem Kayas**
- GitHub: [@gorkemkayas](https://github.com/gorkemkayas)
- Website: [kayas.dev](https://kayas.dev)

## 🔗 İlgili Projeler

- **TenantApi**: Multi-tenant uygulama servisi
- Diğer projeler [GitHub profilini](https://github.com/gorkemkayas) ziyaret edin

---

⭐ Projeyi beğendiyseniz yıldız vermeyi unutmayın!
