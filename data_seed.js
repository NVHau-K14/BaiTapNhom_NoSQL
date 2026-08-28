// 1. Chọn database (nếu chưa có sẽ tự tạo mới)
db = db.getSiblingDB('qlsinhvien_db');

// Xóa dữ liệu cũ trong collection sinhvien để tránh trùng lặp khi chạy lại nhiều lần
db.sv.drop();

// 2. Định nghĩa các tập dữ liệu
const ho = ["Nguyễn", "Trần", "Lê", "Phạm", "Vũ", "Võ", "Đặng", "Bùi", "Đỗ", "Hồ", "Ngô", "Dương", "Lý"];
// Phân loại tên để ghép cho tự nhiên
const dem_nam = ["Văn", "Hoàng", "Minh", "Quốc", "Thanh", "Anh", "Đức", "Nhất"];
const dem_nu = ["Thị", "Ngọc", "Kim", "Bích", "Thanh", "Anh", "Hoàng", "Minh"];
const ten_nam = ["An", "Bình", "Cường", "Dũng", "Giang", "Hải", "Khánh", "Lâm", "Nam", "Quân", "Sơn", "Tùng", "Vinh"];
const ten_nu = ["An", "Bình", "Em", "Giang", "Hương", "Khánh", "Mai", "Oanh", "Phương", "Thảo", "Uyên", "Yến"];

const dsLop = ["L01", "L02", "L03", "L04", "L05"];
const ngoaiNguKhac = ["Tiếng Nhật", "Tiếng Pháp", "Tiếng Đức", "Tiếng Trung", "Tiếng Hàn"];
const monHocPool = [
    { mamon: "MH001", tenmon: "Internet of Things" },
    { mamon: "MH002", tenmon: "Dữ liệu NoSQL" },
    { mamon: "MH003", tenmon: "Nhập môn Big Data" },
    { mamon: "MH004", tenmon: "Lập trình di động" },
    { mamon: "MH005", tenmon: "Quản trị hệ thống mạng" },
    { mamon: "MH006", tenmon: "Deep learning" },
    { mamon: "MH007", tenmon: "Lập trình web" },
    { mamon: "MH008", tenmon: "Trí tuệ nhân tạo" },
    { mamon: "MH009", tenmon: "Bảo mật máy tính" },
    { mamon: "MH010", tenmon: "Lập trình hướng đối tượng" }
];

// Hàm hỗ trợ random
function randomInt(min, max) { return Math.floor(Math.random() * (max - min + 1)) + min; }
function randomItem(arr) { return arr[randomInt(0, arr.length - 1)]; }
function randomDiem() { return Math.round((Math.random() * 10) * 10) / 10; }

let dsSinhVien = [];
let tenDaDung = new Set();

// 3. Sinh dữ liệu cho 30 sinh viên
for (let i = 1; i <= 30; i++) {
    // Nửa đầu là Nam (15), nửa sau là Nữ (15)
    let isNam = i <= 15;
    let phai = isNam ? "Nam" : "Nữ";

    // Random họ tên không trùng lặp
    let hoten = "";
    do {
        let h = randomItem(ho);
        let d = isNam ? randomItem(dem_nam) : randomItem(dem_nu);
        let t = isNam ? randomItem(ten_nam) : randomItem(ten_nu);
        hoten = h + " " + d + " " + t;
    } while (tenDaDung.has(hoten));
    tenDaDung.add(hoten);

    // Tuổi từ 18 - 21
    let tuoi = randomInt(18, 21);

    // Mã sinh viên tăng dần (SV001 -> SV030)
    let masv = "SV" + String(i).padStart(3, '0');

    // Chia đều vào 5 lớp (mỗi lớp 6 sinh viên, đảm bảo nam nữ phân bố đều)
    let malop = dsLop[(i - 1) % 5];

    // Xử lý ngoại ngữ: Mặc định có Tiếng Anh, random thêm 0 hoặc 1, 2 ngôn ngữ khác
    let ngoaingu = ["Tiếng Anh"];
    let soNgoaiNguThem = randomInt(0, 2);
    let nnguKhacShuffle = [...ngoaiNguKhac].sort(() => 0.5 - Math.random());
    for (let j = 0; j < soNgoaiNguThem; j++) {
        ngoaingu.push(nnguKhacShuffle[j]);
    }

    // Xử lý môn học: Đúng 2 môn, điểm random
    let monHocShuffle = [...monHocPool].sort(() => 0.5 - Math.random());
    let monhoc = [
        { mamon: monHocShuffle[0].mamon, tenmon: monHocShuffle[0].tenmon, diem: randomDiem() },
        { mamon: monHocShuffle[1].mamon, tenmon: monHocShuffle[1].tenmon, diem: randomDiem() }
    ];

    // Push vào mảng
    dsSinhVien.push({
        masv: masv,
        hoten: hoten,
        tuoi: tuoi,
        phai: phai,
        malop: malop,
        ngoaingu: ngoaingu,
        monhoc: monhoc
    });
}

// 4. Insert toàn bộ vào Collection
db.sinhvien.insertMany(dsSinhVien);

// 5. Khởi tạo Index theo như yêu cầu của đề tài
db.sv.createIndex({ masv: 1 }, { unique: true });
db.sv.createIndex({ malop: 1, hoten: 1 });

print("Đã tạo thành công 30 sinh viên và đánh Index cho collection!");
