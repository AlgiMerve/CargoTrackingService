# .NET Core Web API Projesi

Bu proje, .NET Core kullanılarak geliştirilmiş iki adet Web API'yi içermektedir: Document API ve PTT API.

## Proje Gereksinimleri

* .NET Core SDK 8.0
* Docker
* Docker Compose
* PostgreSQL
* RabbitMQ

## Proje Yapısı

* **DocumentApi:** Belge CRUD işlemlerini ve RabbitMQ entegrasyonunu yöneten API.
* **PttApi:** PTT takip sitesinden kargo durumunu çeken ve Document API'yi güncelleyen API.

## Kurulum ve Çalıştırma

1.  **Docker ve Docker Compose'u Kurun:**
    * Docker ve Docker Compose'un bilgisayarınızda kurulu olduğundan emin olun.
2.  **Veritabanını Yapılandırın:**
    * PostgreSQL'i kurun ve bir veritabanı oluşturun (örneğin, `documentsdb`).
    * Veritabanı bağlantı bilgilerini `DocumentApi/appsettings.json` dosyasına ekleyin.
3.  **RabbitMQ'yu Yapılandırın:**
    * RabbitMQ'yu kurun ve çalıştırın.
    * RabbitMQ bağlantı bilgilerini `PttApi/appsettings.json` dosyasına ekleyin.
4.  **Docker Compose'u Çalıştırın:**
    * Proje kök dizininde `docker-compose up --build` komutunu çalıştırın.
5.  **API'leri Test Edin:**
    * `DocumentApi` API'sine `http://localhost:5000` adresinden erişebilirsiniz.
    * `PttApi` API'sine `http://localhost:5001` adresinden erişebilirsiniz.
    * Hangfire Dashboard'a `http://localhost:5001/hangfire` adresinden erişebilirsiniz.

## API Endpoint'leri

### Document API

* `GET /Documents`: Tüm belgeleri listeler.
* `GET /Documents/{id}`: Belirli bir belgeyi getirir.
* `POST /Documents`: Yeni bir belge oluşturur.
* `PUT /Documents/{id}`: Belirli bir belgeyi günceller.
* `DELETE /Documents/{id}`: Belirli bir belgeyi siler.
* `PUT /Documents/UpdateCargoStatus`: Kargo durumunu günceller.

### PTT API

* `GET /Ptt/CheckCargoStatus`: Kargo durumu kontrol işini başlatır.

## Örnek Takip Numaraları

* `2710327123358`: Teslim edildi
* `2710327123359`: Geçersiz takip numarası
* `2710256387760`: Teslim edilmedi
