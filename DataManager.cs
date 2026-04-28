using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WinFormsApp1
{
    public class DataManager
    {
        // ===================== Array of Objects =====================
        private List<Book> books = new List<Book>();
        private List<Member> members = new List<Member>();
        private List<BorrowRecord> records = new List<BorrowRecord>();

        private readonly string booksFile = "books.txt";
        private readonly string membersFile = "members.txt";
        private readonly string recordsFile = "records.txt";

        private int nextBookId = 1;
        private int nextMemberId = 1;
        private int nextRecordId = 1;

        public DataManager()
        {
            LoadAll();
        }

        // ==================== FILE HANDLING ====================
        public void SaveAll()
        {
            try
            {
                File.WriteAllLines(booksFile, books.Select(b => b.ToFileString()));
                File.WriteAllLines(membersFile, members.Select(m => m.ToFileString()));
                File.WriteAllLines(recordsFile, records.Select(r => r.ToFileString()));
            }
            catch (Exception ex)
            {
                throw new Exception("خطأ في حفظ البيانات: " + ex.Message);
            }
        }

        public void LoadAll()
        {
            try
            {
                if (File.Exists(booksFile))
                {
                    books = File.ReadAllLines(booksFile)
                        .Where(l => !string.IsNullOrWhiteSpace(l))
                        .Select(Book.FromFileString).ToList();
                    nextBookId = books.Any() ? books.Max(b => b.Id) + 1 : 1;
                }

                if (File.Exists(membersFile))
                {
                    members = File.ReadAllLines(membersFile)
                        .Where(l => !string.IsNullOrWhiteSpace(l))
                        .Select(Member.FromFileString).ToList();
                    nextMemberId = members.Any() ? members.Max(m => m.Id) + 1 : 1;
                }

                if (File.Exists(recordsFile))
                {
                    records = File.ReadAllLines(recordsFile)
                        .Where(l => !string.IsNullOrWhiteSpace(l))
                        .Select(BorrowRecord.FromFileString).ToList();
                    nextRecordId = records.Any() ? records.Max(r => r.RecordId) + 1 : 1;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("خطأ في تحميل البيانات: " + ex.Message);
            }
        }

        // ==================== BOOKS ====================
        public void AddBook(string title, string author, BookCategory cat, int year, int copies)
        {
            if (string.IsNullOrWhiteSpace(title)) throw new Exception("اسم الكتاب مطلوب");
            if (string.IsNullOrWhiteSpace(author)) throw new Exception("اسم المؤلف مطلوب");
            if (copies <= 0) throw new Exception("عدد النسخ يجب أن يكون أكبر من صفر");

            books.Add(new Book(nextBookId++, title, author, cat, year, copies));
            SaveAll();
        }

        public void DeleteBook(int id)
        {
            var book = books.FirstOrDefault(b => b.Id == id);
            if (book == null) throw new Exception("الكتاب غير موجود");
            if (records.Any(r => r.BookId == id && r.Status == BorrowStatus.Borrowed))
                throw new Exception("لا يمكن حذف كتاب مستعار حالياً");
            books.Remove(book);
            SaveAll();
        }

        public List<Book> GetAllBooks() => books.ToList();

        public List<Book> SearchBooks(string query)
        {
            query = query.ToLower();
            return books.Where(b =>
                b.Title.ToLower().Contains(query) ||
                b.Author.ToLower().Contains(query)).ToList();
        }

        public Book GetBookById(int id) => books.FirstOrDefault(b => b.Id == id);

        // ==================== MEMBERS ====================
        public void AddMember(string name, string phone, MemberType type)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new Exception("اسم العضو مطلوب");
            if (string.IsNullOrWhiteSpace(phone)) throw new Exception("رقم الهاتف مطلوب");

            members.Add(new Member(nextMemberId++, name, phone, type));
            SaveAll();
        }

        public void DeleteMember(int id)
        {
            var m = members.FirstOrDefault(x => x.Id == id);
            if (m == null) throw new Exception("العضو غير موجود");
            if (records.Any(r => r.MemberId == id && r.Status == BorrowStatus.Borrowed))
                throw new Exception("لا يمكن حذف عضو لديه كتب مستعارة");
            members.Remove(m);
            SaveAll();
        }

        public List<Member> GetAllMembers() => members.ToList();

        public List<Member> SearchMembers(string query)
        {
            query = query.ToLower();
            return members.Where(m =>
                m.Name.ToLower().Contains(query) ||
                m.Phone.Contains(query)).ToList();
        }

        public Member GetMemberById(int id) => members.FirstOrDefault(m => m.Id == id);

        // ==================== BORROW / RETURN ====================
        public void BorrowBook(int bookId, int memberId)
        {
            var book = GetBookById(bookId);
            if (book == null) throw new Exception("الكتاب غير موجود");
            if (book.AvailableCopies <= 0) throw new Exception("لا توجد نسخ متاحة من هذا الكتاب");

            var member = GetMemberById(memberId);
            if (member == null) throw new Exception("العضو غير موجود");

            int currentBorrowed = records.Count(r => r.MemberId == memberId && r.Status == BorrowStatus.Borrowed);
            if (currentBorrowed >= member.MaxBooks)
                throw new Exception($"وصل العضو للحد الأقصى ({member.MaxBooks} كتب)");

            if (records.Any(r => r.BookId == bookId && r.MemberId == memberId && r.Status == BorrowStatus.Borrowed))
                throw new Exception("العضو استعار هذا الكتاب بالفعل");

            book.AvailableCopies--;
            book.IsAvailable = book.AvailableCopies > 0;

            records.Add(new BorrowRecord(nextRecordId++, bookId, book.Title, memberId, member.Name));
            SaveAll();
        }

        public void ReturnBook(int recordId)
        {
            var rec = records.FirstOrDefault(r => r.RecordId == recordId && r.Status == BorrowStatus.Borrowed);
            if (rec == null) throw new Exception("سجل الاستعارة غير موجود أو تم الإرجاع مسبقاً");

            rec.ReturnDate = DateTime.Now;
            rec.Status = BorrowStatus.Returned;

            var book = GetBookById(rec.BookId);
            if (book != null)
            {
                book.AvailableCopies++;
                book.IsAvailable = true;
            }
            SaveAll();
        }

        public List<BorrowRecord> GetAllRecords() => records.ToList();

        public List<BorrowRecord> GetActiveBorrows() =>
            records.Where(r => r.Status == BorrowStatus.Borrowed).ToList();

        public List<BorrowRecord> GetMemberBorrows(int memberId) =>
            records.Where(r => r.MemberId == memberId).ToList();

        // ==================== STATS ====================
        public int TotalBooks => books.Count;
        public int TotalMembers => members.Count;
        public int ActiveBorrows => records.Count(r => r.Status == BorrowStatus.Borrowed);
        public int OverdueBooks => records.Count(r => r.Status == BorrowStatus.Borrowed && DateTime.Now > r.DueDate);
    }
}
