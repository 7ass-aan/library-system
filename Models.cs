using System;
using System.Collections.Generic;

namespace WinFormsApp1
{
    // ===================== ENUM =====================
    public enum BookCategory
    {
        Science,
        Literature,
        History,
        Technology,
        Religion,
        Medicine,
        Arts,
        Other
    }

    public enum MemberType
    {
        Student,
        Teacher,
        Regular
    }

    public enum BorrowStatus
    {
        Borrowed,
        Returned,
        Overdue
    }

    // ===================== BASE CLASS =====================
    public abstract class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }

        public Person(int id, string name, string phone)
        {
            Id = id;
            Name = name;
            Phone = phone;
        }

        // Polymorphism - abstract method
        public abstract string GetInfo();

        public override string ToString()
        {
            return $"{Id} | {Name} | {Phone}";
        }
    }

    // ===================== DERIVED CLASS: Member =====================
    public class Member : Person
    {
        public MemberType Type { get; set; }
        public DateTime JoinDate { get; set; }
        public int MaxBooks => Type == MemberType.Teacher ? 5 : Type == MemberType.Student ? 3 : 2;

        public Member(int id, string name, string phone, MemberType type)
            : base(id, name, phone)
        {
            Type = type;
            JoinDate = DateTime.Now;
        }

        public override string GetInfo()
        {
            return $"[عضو] {Name} - النوع: {GetArabicType()} - الهاتف: {Phone}";
        }

        public string GetArabicType()
        {
            switch (Type)
            {
                case MemberType.Student: return "طالب";
                case MemberType.Teacher: return "معلم";
                default: return "عادي";
            }
        }

        public string ToFileString()
        {
            return $"{Id},{Name},{Phone},{Type},{JoinDate:yyyy-MM-dd}";
        }

        public static Member FromFileString(string line)
        {
            var parts = line.Split(',');
            var m = new Member(
                int.Parse(parts[0]),
                parts[1],
                parts[2],
                (MemberType)Enum.Parse(typeof(MemberType), parts[3])
            );
            m.JoinDate = DateTime.Parse(parts[4]);
            return m;
        }
    }

    // ===================== DERIVED CLASS: Librarian =====================
    public class Librarian : Person
    {
        public string EmployeeCode { get; set; }

        public Librarian(int id, string name, string phone, string empCode)
            : base(id, name, phone)
        {
            EmployeeCode = empCode;
        }

        public override string GetInfo()
        {
            return $"[أمين مكتبة] {Name} - كود: {EmployeeCode}";
        }
    }

    // ===================== Book CLASS =====================
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public BookCategory Category { get; set; }
        public int Year { get; set; }
        public bool IsAvailable { get; set; }
        public int TotalCopies { get; set; }
        public int AvailableCopies { get; set; }

        public Book(int id, string title, string author, BookCategory category, int year, int copies = 1)
        {
            Id = id;
            Title = title;
            Author = author;
            Category = category;
            Year = year;
            TotalCopies = copies;
            AvailableCopies = copies;
            IsAvailable = copies > 0;
        }

        public string GetCategoryArabic()
        {
            switch (Category)
            {
                case BookCategory.Science: return "علوم";
                case BookCategory.Literature: return "أدب";
                case BookCategory.History: return "تاريخ";
                case BookCategory.Technology: return "تكنولوجيا";
                case BookCategory.Religion: return "دين";
                case BookCategory.Medicine: return "طب";
                case BookCategory.Arts: return "فنون";
                default: return "أخرى";
            }
        }

        public string ToFileString()
        {
            return $"{Id},{Title},{Author},{Category},{Year},{TotalCopies},{AvailableCopies}";
        }

        public static Book FromFileString(string line)
        {
            var parts = line.Split(',');
            return new Book(
                int.Parse(parts[0]),
                parts[1],
                parts[2],
                (BookCategory)Enum.Parse(typeof(BookCategory), parts[3]),
                int.Parse(parts[4]),
                int.Parse(parts[5])
            )
            { AvailableCopies = int.Parse(parts[6]), IsAvailable = int.Parse(parts[6]) > 0 };
        }

        public override string ToString()
        {
            return $"{Id} | {Title} | {Author} | {GetCategoryArabic()} | {(IsAvailable ? "متاح" : "غير متاح")}";
        }
    }

    // ===================== BorrowRecord CLASS =====================
    public class BorrowRecord
    {
        public int RecordId { get; set; }
        public int BookId { get; set; }
        public string BookTitle { get; set; }
        public int MemberId { get; set; }
        public string MemberName { get; set; }
        public DateTime BorrowDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public BorrowStatus Status { get; set; }

        public BorrowRecord(int recId, int bookId, string bookTitle, int memberId, string memberName)
        {
            RecordId = recId;
            BookId = bookId;
            BookTitle = bookTitle;
            MemberId = memberId;
            MemberName = memberName;
            BorrowDate = DateTime.Now;
            DueDate = DateTime.Now.AddDays(14);
            Status = BorrowStatus.Borrowed;
        }

        public double FineAmount()
        {
            if (Status == BorrowStatus.Returned) return 0;
            if (DateTime.Now > DueDate)
            {
                return (DateTime.Now - DueDate).Days * 1.0; // 1 pound per day
            }
            return 0;
        }

        public string ToFileString()
        {
            string ret = ReturnDate.HasValue ? ReturnDate.Value.ToString("yyyy-MM-dd") : "NULL";
            return $"{RecordId},{BookId},{BookTitle},{MemberId},{MemberName},{BorrowDate:yyyy-MM-dd},{DueDate:yyyy-MM-dd},{ret},{Status}";
        }

        public static BorrowRecord FromFileString(string line)
        {
            var parts = line.Split(',');
            var rec = new BorrowRecord(int.Parse(parts[0]), int.Parse(parts[1]), parts[2], int.Parse(parts[3]), parts[4]);
            rec.BorrowDate = DateTime.Parse(parts[5]);
            rec.DueDate = DateTime.Parse(parts[6]);
            rec.ReturnDate = parts[7] == "NULL" ? (DateTime?)null : DateTime.Parse(parts[7]);
            rec.Status = (BorrowStatus)Enum.Parse(typeof(BorrowStatus), parts[8]);
            return rec;
        }
    }
}
