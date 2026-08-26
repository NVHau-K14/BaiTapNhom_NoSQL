using MongoDB.Driver;
using web_qlsinhvien.Models;

namespace web_qlsinhvien.Services
{
    public class MongoDbService
    {
        private readonly MongoClient _client;
        private readonly IMongoDatabase _database;

        public IMongoCollection<SinhVien> SinhVienCollection { get; }

        public MongoDbService(IConfiguration configuration)
        {
            // 1. Đọc cấu hình từ appsettings.json
            var connectionString = configuration.GetConnectionString("MongoDb") ?? "mongodb://localhost:27017";
            var databaseName = configuration["DatabaseName"] ?? "qlsinhvien_db";

            // 2. Khởi tạo kết nối duy nhất (Singleton)
            _client = new MongoClient(connectionString);
            _database = _client.GetDatabase(databaseName);

            // 3. Lấy collection 'sv'
            SinhVienCollection = _database.GetCollection<SinhVien>("sv");

            // 4. Tự động khởi tạo Index theo yêu cầu mục II.6
            InitIndexes();
        }

        private void InitIndexes()
        {
            try
            {
                // Unique Index cho masv (ngăn chặn trùng lặp mã sinh viên)
                var indexMasv = new CreateIndexModel<SinhVien>(
                    Builders<SinhVien>.IndexKeys.Ascending(x => x.Masv),
                    new CreateIndexOptions { Unique = true }
                );

                // Compound Index cho cặp { malop: 1, hoten: 1 }
                var indexCompound = new CreateIndexModel<SinhVien>(
                    Builders<SinhVien>.IndexKeys.Ascending(x => x.Malop).Ascending(x => x.Hoten)
                );

                SinhVienCollection.Indexes.CreateMany(new[] { indexMasv, indexCompound });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khởi tạo Index: {ex.Message}");
            }
        }
    }
}