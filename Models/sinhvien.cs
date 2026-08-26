using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace web_qlsinhvien.Models
{
    public class SinhVien
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("masv")]
        public string Masv { get; set; } = string.Empty;

        [BsonElement("hoten")]
        public string Hoten { get; set; } = string.Empty;

        [BsonElement("tuoi")]
        public int Tuoi { get; set; }

        [BsonElement("phai")]
        public string Phai { get; set; } = string.Empty;

        [BsonElement("malop")]
        public string Malop { get; set; } = string.Empty;

        [BsonElement("ngoaingu")]
        public List<string> Ngoaingu { get; set; } = new List<string>();

        [BsonElement("monhoc")]
        public List<MonHoc> Monhoc { get; set; } = new List<MonHoc>();
    }

    public class MonHoc
    {
        [BsonElement("mamon")]
        public string Mamon { get; set; } = string.Empty;

        [BsonElement("tenmon")]
        public string Tenmon { get; set; } = string.Empty;

        [BsonElement("diem")]
        public double Diem { get; set; }
    }
}