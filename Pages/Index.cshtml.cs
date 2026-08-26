using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MongoDB.Driver;
using web_qlsinhvien.Models;
using web_qlsinhvien.Services;

namespace web_qlsinhvien.Pages
{
    public class IndexModel : PageModel
    {
        private readonly MongoDbService _dbService;

        public IndexModel(MongoDbService dbService)
        {
            _dbService = dbService;
        }

        public List<SinhVien> DanhSachSinhVien { get; set; } = new List<SinhVien>();

        // Thuộc tính để liên kết dữ liệu Form thêm mới sinh viên (sửa lỗi CS1061)
        [BindProperty]
        public SinhVien NewSinhVien { get; set; } = new SinhVien();

        [BindProperty(SupportsGet = true)]
        public string? SearchMasv { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? FilterLop { get; set; }

        public async Task OnGetAsync()
        {
            var filterBuilder = Builders<SinhVien>.Filter;
            var filter = filterBuilder.Empty;

            if (!string.IsNullOrEmpty(SearchMasv))
            {
                filter &= filterBuilder.Eq(x => x.Masv, SearchMasv.Trim());
            }

            if (!string.IsNullOrEmpty(FilterLop))
            {
                filter &= filterBuilder.Eq(x => x.Malop, FilterLop.Trim());
            }

            DanhSachSinhVien = await _dbService.SinhVienCollection.Find(filter).ToListAsync();
        }

        // Xử lý thêm mới sinh viên
        public async Task<IActionResult> OnPostAddAsync()
        {
            try
            {
                // Loại bỏ các môn học hoặc ngoại ngữ trống nếu có
                if (NewSinhVien.Ngoaingu != null)
                {
                    NewSinhVien.Ngoaingu = NewSinhVien.Ngoaingu.Where(n => !string.IsNullOrWhiteSpace(n)).ToList();
                }

                if (NewSinhVien.Monhoc != null)
                {
                    NewSinhVien.Monhoc = NewSinhVien.Monhoc.Where(m => !string.IsNullOrWhiteSpace(m.Mamon)).ToList();
                }

                await _dbService.SinhVienCollection.InsertOneAsync(NewSinhVien);
                return RedirectToPage();
            }
            catch (MongoWriteException ex) when (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
            {
                ModelState.AddModelError(string.Empty, "Mã sinh viên này đã tồn tại trong hệ thống!");
                await OnGetAsync();
                return Page();
            }
        }

        // Xử lý xóa 1 sinh viên
        public async Task<IActionResult> OnPostDeleteAsync(string masv)
        {
            await _dbService.SinhVienCollection.DeleteOneAsync(x => x.Masv == masv);
            return RedirectToPage();
        }
        // Thêm hàm này vào trong class IndexModel:
        public async Task<IActionResult> OnPostDeleteClassAsync(string malop)
        {
            if (!string.IsNullOrEmpty(malop))
            {
                // Thực hiện xóa toàn bộ sinh viên có mã lớp tương ứng (deleteMany)
                await _dbService.SinhVienCollection.DeleteManyAsync(x => x.Malop == malop);
            }
            return RedirectToPage("/Index");
        }
        [BindProperty]
        public SinhVien EditSinhVien { get; set; } = new SinhVien();

        public async Task<IActionResult> OnPostEditAsync()
        {
            // Lọc các giá trị trống
            if (EditSinhVien.Ngoaingu != null)
            {
                EditSinhVien.Ngoaingu = EditSinhVien.Ngoaingu.Where(n => !string.IsNullOrWhiteSpace(n)).ToList();
            }

            if (EditSinhVien.Monhoc != null)
            {
                EditSinhVien.Monhoc = EditSinhVien.Monhoc.Where(m => !string.IsNullOrWhiteSpace(m.Mamon)).ToList();
            }

            // Cập nhật Document với $set theo masv
            var filter = Builders<SinhVien>.Filter.Eq(x => x.Masv, EditSinhVien.Masv);
            var update = Builders<SinhVien>.Update
                .Set(x => x.Hoten, EditSinhVien.Hoten)
                .Set(x => x.Tuoi, EditSinhVien.Tuoi)
                .Set(x => x.Phai, EditSinhVien.Phai)
                .Set(x => x.Malop, EditSinhVien.Malop)
                .Set(x => x.Ngoaingu, EditSinhVien.Ngoaingu)
                .Set(x => x.Monhoc, EditSinhVien.Monhoc);

            await _dbService.SinhVienCollection.UpdateOneAsync(filter, update);
            return RedirectToPage();
        }
    }
}