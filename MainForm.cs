using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public class MainForm : Form
    {
        private DataManager db;
        private Panel sidePanel;
        private Panel contentPanel;
        private Label lblTitle;
        private Button btnDashboard, btnBooks, btnMembers, btnBorrow, btnReturn, btnReports;
        private Button activeBtn;

        public MainForm()
        {
            db = new DataManager();
            InitializeLayout();
            ShowDashboard();
        }

        // ===================== LAYOUT =====================
        private void InitializeLayout()
        {
            this.Text = "نظام إدارة المكتبة";
            this.Size = new Size(1100, 680);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(245, 247, 250);
            this.Font = new Font("Segoe UI", 9.5f);
            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.MinimumSize = new Size(950, 600);

            // Side Panel
            sidePanel = new Panel
            {
                Dock = DockStyle.Right,
                Width = 210,
                BackColor = Color.FromArgb(30, 41, 59)
            };

            // Logo area
            var logoPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = Color.FromArgb(15, 23, 42)
            };
            var logoLabel = new Label
            {
                Text = "📚 مكتبتي",
                Font = new Font("Segoe UI", 16f, FontStyle.Bold),
                ForeColor = Color.White,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };
            logoPanel.Controls.Add(logoLabel);
            sidePanel.Controls.Add(logoPanel);

            // Nav Buttons
            btnDashboard = CreateNavBtn("🏠  الرئيسية", 80);
            btnBooks = CreateNavBtn("📖  الكتب", 130);
            btnMembers = CreateNavBtn("👥  الأعضاء", 180);
            btnBorrow = CreateNavBtn("📤  استعارة كتاب", 230);
            btnReturn = CreateNavBtn("📥  إرجاع كتاب", 280);
            btnReports = CreateNavBtn("📊  التقارير", 330);

            btnDashboard.Click += (s, e) => { SetActive(btnDashboard); ShowDashboard(); };
            btnBooks.Click += (s, e) => { SetActive(btnBooks); ShowBooks(); };
            btnMembers.Click += (s, e) => { SetActive(btnMembers); ShowMembers(); };
            btnBorrow.Click += (s, e) => { SetActive(btnBorrow); ShowBorrow(); };
            btnReturn.Click += (s, e) => { SetActive(btnReturn); ShowReturn(); };
            btnReports.Click += (s, e) => { SetActive(btnReports); ShowReports(); };

            sidePanel.Controls.AddRange(new Control[] { btnDashboard, btnBooks, btnMembers, btnBorrow, btnReturn, btnReports });

            // Content Panel
            contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(25),
                BackColor = Color.FromArgb(245, 247, 250)
            };

            this.Controls.Add(contentPanel);
            this.Controls.Add(sidePanel);
            SetActive(btnDashboard);
        }

        private Button CreateNavBtn(string text, int top)
        {
            return new Button
            {
                Text = text,
                Location = new Point(0, top),
                Size = new Size(210, 45),
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.FromArgb(148, 163, 184),
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleRight,
                Padding = new Padding(15, 0, 15, 0),
                Font = new Font("Segoe UI", 10.5f),
                Cursor = Cursors.Hand,
                FlatAppearance = { BorderSize = 0, MouseOverBackColor = Color.FromArgb(51, 65, 85) }
            };
        }

        private void SetActive(Button btn)
        {
            if (activeBtn != null)
            {
                activeBtn.BackColor = Color.Transparent;
                activeBtn.ForeColor = Color.FromArgb(148, 163, 184);
            }
            activeBtn = btn;
            btn.BackColor = Color.FromArgb(59, 130, 246);
            btn.ForeColor = Color.White;
        }

        private void ClearContent()
        {
            contentPanel.Controls.Clear();
        }

        private Label MakeTitle(string text)
        {
            return new Label
            {
                Text = text,
                Font = new Font("Segoe UI", 18f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 41, 59),
                AutoSize = true,
                Location = new Point(0, 0)
            };
        }

        private Panel MakeCard(int x, int y, int w, int h, Color bg)
        {
            var p = new Panel
            {
                Location = new Point(x, y),
                Size = new Size(w, h),
                BackColor = bg
            };
            p.Paint += (s, e) =>
            {
                var g = e.Graphics;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                using var brush = new SolidBrush(Color.FromArgb(20, Color.Black));
                g.FillRectangle(brush, 0, h - 4, w, 4);
            };
            return p;
        }

        // ===================== DASHBOARD =====================
        private void ShowDashboard()
        {
            ClearContent();
            contentPanel.Controls.Add(MakeTitle("🏠 لوحة التحكم"));

            int cardW = 190, cardH = 110, topY = 50, gap = 20;
            var cards = new[]
            {
                new { label = "إجمالي الكتب", value = db.TotalBooks.ToString(), color = Color.FromArgb(59,130,246), icon = "📚" },
                new { label = "الأعضاء", value = db.TotalMembers.ToString(), color = Color.FromArgb(16,185,129), icon = "👥" },
                new { label = "مستعار حالياً", value = db.ActiveBorrows.ToString(), color = Color.FromArgb(245,158,11), icon = "📤" },
                new { label = "متأخر الإرجاع", value = db.OverdueBooks.ToString(), color = Color.FromArgb(239,68,68), icon = "⚠️" },
            };

            for (int i = 0; i < cards.Length; i++)
            {
                var c = cards[i];
                var card = new Panel
                {
                    Location = new Point(i * (cardW + gap), topY),
                    Size = new Size(cardW, cardH),
                    BackColor = c.color
                };
                card.Controls.Add(new Label { Text = c.icon + "  " + c.label, Location = new Point(10, 15), AutoSize = true, ForeColor = Color.FromArgb(220, 255, 255), Font = new Font("Segoe UI", 9f) });
                card.Controls.Add(new Label { Text = c.value, Location = new Point(10, 45), AutoSize = true, ForeColor = Color.White, Font = new Font("Segoe UI", 28f, FontStyle.Bold) });
                contentPanel.Controls.Add(card);
            }

            // Recent borrows table
            var lbl = new Label { Text = "📋 آخر الاستعارات", Location = new Point(0, 185), AutoSize = true, Font = new Font("Segoe UI", 13f, FontStyle.Bold), ForeColor = Color.FromArgb(30, 41, 59) };
            contentPanel.Controls.Add(lbl);

            var grid = CreateGrid(new Point(0, 215), new Size(contentPanel.Width - 50, contentPanel.Height - 240));
            grid.Columns.Add("RecId", "رقم");
            grid.Columns.Add("Book", "الكتاب");
            grid.Columns.Add("Member", "العضو");
            grid.Columns.Add("Borrow", "تاريخ الاستعارة");
            grid.Columns.Add("Due", "تاريخ الإرجاع");
            grid.Columns.Add("Status", "الحالة");
            foreach (var r in db.GetActiveBorrows().Take(10))
            {
                grid.Rows.Add(r.RecordId, r.BookTitle, r.MemberName,
                    r.BorrowDate.ToString("yyyy/MM/dd"),
                    r.DueDate.ToString("yyyy/MM/dd"),
                    DateTime.Now > r.DueDate ? "⚠️ متأخر" : "✅ في الموعد");
            }
            contentPanel.Controls.Add(grid);
        }

        // ===================== BOOKS =====================
        private void ShowBooks()
        {
            ClearContent();
            contentPanel.Controls.Add(MakeTitle("📚 إدارة الكتب"));

            // Search
            var searchBox = new TextBox { Location = new Point(0, 50), Width = 280, Height = 30, Font = new Font("Segoe UI", 10f), PlaceholderText = "🔍 ابحث عن كتاب..." };
            var btnSearch = StyledBtn("بحث", 295, 50, Color.FromArgb(59, 130, 246));
            var btnAdd = StyledBtn("+ إضافة كتاب", 380, 50, Color.FromArgb(16, 185, 129));
            var btnDel = StyledBtn("🗑 حذف", 510, 50, Color.FromArgb(239, 68, 68));

            var grid = CreateGrid(new Point(0, 100), new Size(contentPanel.Width - 50, contentPanel.Height - 130));
            grid.Columns.Add("Id", "#");
            grid.Columns.Add("Title", "عنوان الكتاب");
            grid.Columns.Add("Author", "المؤلف");
            grid.Columns.Add("Cat", "التصنيف");
            grid.Columns.Add("Year", "السنة");
            grid.Columns.Add("Copies", "النسخ الكلية");
            grid.Columns.Add("Avail", "المتاحة");

            Action loadBooks = () =>
            {
                grid.Rows.Clear();
                var list = string.IsNullOrWhiteSpace(searchBox.Text) ? db.GetAllBooks() : db.SearchBooks(searchBox.Text);
                foreach (var b in list)
                    grid.Rows.Add(b.Id, b.Title, b.Author, b.GetCategoryArabic(), b.Year, b.TotalCopies, b.AvailableCopies);
            };
            loadBooks();

            btnSearch.Click += (s, e) => loadBooks();
            searchBox.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) loadBooks(); };

            btnAdd.Click += (s, e) =>
            {
                using var dlg = new AddBookForm();
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    try { db.AddBook(dlg.BookTitle, dlg.Author, dlg.Category, dlg.Year, dlg.Copies); loadBooks(); }
                    catch (Exception ex) { MessageBox.Show(ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
                }
            };

            btnDel.Click += (s, e) =>
            {
                if (grid.SelectedRows.Count == 0) { MessageBox.Show("اختار كتاباً أولاً"); return; }
                int id = (int)grid.SelectedRows[0].Cells["Id"].Value;
                if (MessageBox.Show("هل تريد حذف هذا الكتاب؟", "تأكيد", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    try { db.DeleteBook(id); loadBooks(); }
                    catch (Exception ex) { MessageBox.Show(ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
                }
            };

            contentPanel.Controls.AddRange(new Control[] { searchBox, btnSearch, btnAdd, btnDel, grid });
        }

        // ===================== MEMBERS =====================
        private void ShowMembers()
        {
            ClearContent();
            contentPanel.Controls.Add(MakeTitle("👥 إدارة الأعضاء"));

            var searchBox = new TextBox { Location = new Point(0, 50), Width = 280, Font = new Font("Segoe UI", 10f), PlaceholderText = "🔍 ابحث عن عضو..." };
            var btnSearch = StyledBtn("بحث", 295, 50, Color.FromArgb(59, 130, 246));
            var btnAdd = StyledBtn("+ إضافة عضو", 380, 50, Color.FromArgb(16, 185, 129));
            var btnDel = StyledBtn("🗑 حذف", 510, 50, Color.FromArgb(239, 68, 68));

            var grid = CreateGrid(new Point(0, 100), new Size(contentPanel.Width - 50, contentPanel.Height - 130));
            grid.Columns.Add("Id", "#");
            grid.Columns.Add("Name", "الاسم");
            grid.Columns.Add("Phone", "الهاتف");
            grid.Columns.Add("Type", "النوع");
            grid.Columns.Add("Join", "تاريخ الانضمام");
            grid.Columns.Add("Max", "الحد الأقصى");

            Action load = () =>
            {
                grid.Rows.Clear();
                var list = string.IsNullOrWhiteSpace(searchBox.Text) ? db.GetAllMembers() : db.SearchMembers(searchBox.Text);
                foreach (var m in list)
                    grid.Rows.Add(m.Id, m.Name, m.Phone, m.GetArabicType(), m.JoinDate.ToString("yyyy/MM/dd"), m.MaxBooks);
            };
            load();

            btnSearch.Click += (s, e) => load();
            searchBox.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) load(); };

            btnAdd.Click += (s, e) =>
            {
                using var dlg = new AddMemberForm();
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    try { db.AddMember(dlg.MemberName, dlg.Phone, dlg.MType); load(); }
                    catch (Exception ex) { MessageBox.Show(ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
                }
            };

            btnDel.Click += (s, e) =>
            {
                if (grid.SelectedRows.Count == 0) { MessageBox.Show("اختار عضواً أولاً"); return; }
                int id = (int)grid.SelectedRows[0].Cells["Id"].Value;
                if (MessageBox.Show("هل تريد حذف هذا العضو؟", "تأكيد", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    try { db.DeleteMember(id); load(); }
                    catch (Exception ex) { MessageBox.Show(ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
                }
            };

            contentPanel.Controls.AddRange(new Control[] { searchBox, btnSearch, btnAdd, btnDel, grid });
        }

        // ===================== BORROW =====================
        private void ShowBorrow()
        {
            ClearContent();
            contentPanel.Controls.Add(MakeTitle("📤 استعارة كتاب"));

            int lx = 0, ly = 60;
            void AddRow(string label, Control ctrl, ref int y)
            {
                contentPanel.Controls.Add(new Label { Text = label, Location = new Point(lx, y + 3), AutoSize = true, Font = new Font("Segoe UI", 10f) });
                ctrl.Location = new Point(150, y);
                ctrl.Width = 300;
                contentPanel.Controls.Add(ctrl);
                y += 45;
            }

            var txtBookId = new TextBox { Font = new Font("Segoe UI", 10f) };
            var txtMemberId = new TextBox { Font = new Font("Segoe UI", 10f) };
            var lblBookInfo = new Label { Location = new Point(0, 0), AutoSize = true, ForeColor = Color.FromArgb(59, 130, 246), Font = new Font("Segoe UI", 9.5f) };
            var lblMemberInfo = new Label { AutoSize = true, ForeColor = Color.FromArgb(16, 185, 129), Font = new Font("Segoe UI", 9.5f) };

            AddRow("رقم الكتاب:", txtBookId, ref ly);
            lblBookInfo.Location = new Point(150, ly - 20);
            contentPanel.Controls.Add(lblBookInfo);

            AddRow("رقم العضو:", txtMemberId, ref ly);
            lblMemberInfo.Location = new Point(150, ly - 20);
            contentPanel.Controls.Add(lblMemberInfo);

            txtBookId.TextChanged += (s, e) =>
            {
                if (int.TryParse(txtBookId.Text, out int bid))
                {
                    var b = db.GetBookById(bid);
                    lblBookInfo.Text = b != null ? $"✅ {b.Title} - {b.Author} ({b.AvailableCopies} متاح)" : "❌ كتاب غير موجود";
                }
                else lblBookInfo.Text = "";
            };

            txtMemberId.TextChanged += (s, e) =>
            {
                if (int.TryParse(txtMemberId.Text, out int mid))
                {
                    var m = db.GetMemberById(mid);
                    lblMemberInfo.Text = m != null ? $"✅ {m.Name} - {m.GetArabicType()} (حد: {m.MaxBooks})" : "❌ عضو غير موجود";
                }
                else lblMemberInfo.Text = "";
            };

            var btnBorrowNow = StyledBtn("✅ تأكيد الاستعارة", 0, ly + 10, Color.FromArgb(16, 185, 129));
            btnBorrowNow.Width = 200;
            btnBorrowNow.Click += (s, e) =>
            {
                try
                {
                    if (!int.TryParse(txtBookId.Text, out int bId)) throw new Exception("رقم الكتاب غير صحيح");
                    if (!int.TryParse(txtMemberId.Text, out int mId)) throw new Exception("رقم العضو غير صحيح");
                    db.BorrowBook(bId, mId);
                    MessageBox.Show("✅ تم تسجيل الاستعارة بنجاح!\nتاريخ الإرجاع: " + DateTime.Now.AddDays(14).ToString("yyyy/MM/dd"), "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtBookId.Clear(); txtMemberId.Clear(); lblBookInfo.Text = ""; lblMemberInfo.Text = "";
                }
                catch (Exception ex) { MessageBox.Show(ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
            };
            contentPanel.Controls.Add(btnBorrowNow);

            // Available books quick view
            var lbl = new Label { Text = "الكتب المتاحة:", Location = new Point(0, ly + 70), AutoSize = true, Font = new Font("Segoe UI", 12f, FontStyle.Bold), ForeColor = Color.FromArgb(30, 41, 59) };
            contentPanel.Controls.Add(lbl);

            var grid = CreateGrid(new Point(0, ly + 100), new Size(contentPanel.Width - 50, contentPanel.Height - ly - 120));
            grid.Columns.Add("Id", "#");
            grid.Columns.Add("Title", "الكتاب");
            grid.Columns.Add("Author", "المؤلف");
            grid.Columns.Add("Avail", "المتاحة");
            foreach (var b in db.GetAllBooks().Where(x => x.AvailableCopies > 0))
                grid.Rows.Add(b.Id, b.Title, b.Author, b.AvailableCopies);
            contentPanel.Controls.Add(grid);
        }

        // ===================== RETURN =====================
        private void ShowReturn()
        {
            ClearContent();
            contentPanel.Controls.Add(MakeTitle("📥 إرجاع كتاب"));

            var lbl = new Label { Text = "اختار سجل الاستعارة وانقر إرجاع:", Location = new Point(0, 50), AutoSize = true, Font = new Font("Segoe UI", 11f), ForeColor = Color.FromArgb(71, 85, 105) };
            contentPanel.Controls.Add(lbl);

            var btnReturn = StyledBtn("📥 إرجاع المحدد", 0, 80, Color.FromArgb(245, 158, 11));
            contentPanel.Controls.Add(btnReturn);

            var grid = CreateGrid(new Point(0, 125), new Size(contentPanel.Width - 50, contentPanel.Height - 155));
            grid.Columns.Add("RecId", "رقم السجل");
            grid.Columns.Add("Book", "الكتاب");
            grid.Columns.Add("Member", "العضو");
            grid.Columns.Add("BorrowDate", "تاريخ الاستعارة");
            grid.Columns.Add("DueDate", "موعد الإرجاع");
            grid.Columns.Add("Fine", "الغرامة (جنيه)");
            grid.Columns.Add("Late", "الحالة");

            Action load = () =>
            {
                grid.Rows.Clear();
                foreach (var r in db.GetActiveBorrows())
                {
                    bool late = DateTime.Now > r.DueDate;
                    grid.Rows.Add(r.RecordId, r.BookTitle, r.MemberName,
                        r.BorrowDate.ToString("yyyy/MM/dd"), r.DueDate.ToString("yyyy/MM/dd"),
                        r.FineAmount().ToString("F0"),
                        late ? "⚠️ متأخر" : "✅ في الموعد");
                    if (late) grid.Rows[grid.Rows.Count - 1].DefaultCellStyle.BackColor = Color.FromArgb(255, 235, 235);
                }
            };
            load();

            btnReturn.Click += (s, e) =>
            {
                if (grid.SelectedRows.Count == 0) { MessageBox.Show("اختار سجلاً أولاً"); return; }
                int recId = (int)grid.SelectedRows[0].Cells["RecId"].Value;
                double fine = double.Parse(grid.SelectedRows[0].Cells["Fine"].Value.ToString());
                string msg = fine > 0 ? $"يوجد غرامة تأخير: {fine} جنيه\n" : "";
                if (MessageBox.Show(msg + "تأكيد الإرجاع؟", "تأكيد", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    try { db.ReturnBook(recId); load(); MessageBox.Show("✅ تم الإرجاع بنجاح!"); }
                    catch (Exception ex) { MessageBox.Show(ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
                }
            };
            contentPanel.Controls.Add(grid);
        }

        // ===================== REPORTS =====================
        private void ShowReports()
        {
            ClearContent();
            contentPanel.Controls.Add(MakeTitle("📊 التقارير"));

            // Stats cards
            int cx = 0, cy = 50, cw = 200, ch = 80, gap = 15;
            var stats = new[]
            {
                new { t = "إجمالي الكتب", v = db.TotalBooks, c = Color.FromArgb(59,130,246) },
                new { t = "الأعضاء", v = db.TotalMembers, c = Color.FromArgb(16,185,129) },
                new { t = "نشط حالياً", v = db.ActiveBorrows, c = Color.FromArgb(245,158,11) },
                new { t = "متأخر", v = db.OverdueBooks, c = Color.FromArgb(239,68,68) },
            };
            foreach (var s in stats)
            {
                var card = new Panel { Location = new Point(cx, cy), Size = new Size(cw, ch), BackColor = s.c };
                card.Controls.Add(new Label { Text = s.t, Location = new Point(8, 8), AutoSize = true, ForeColor = Color.White, Font = new Font("Segoe UI", 9f) });
                card.Controls.Add(new Label { Text = s.v.ToString(), Location = new Point(8, 30), AutoSize = true, ForeColor = Color.White, Font = new Font("Segoe UI", 20f, FontStyle.Bold) });
                contentPanel.Controls.Add(card);
                cx += cw + gap;
            }

            var tab = new TabControl { Location = new Point(0, 150), Size = new Size(contentPanel.Width - 30, contentPanel.Height - 170) };

            // Tab 1: All borrow history
            var tabHistory = new TabPage("📋 سجل الاستعارات");
            var g1 = CreateGrid(new Point(5, 5), new Size(tab.Width - 20, tab.Height - 50));
            g1.Columns.Add("R", "#"); g1.Columns.Add("Bk", "الكتاب"); g1.Columns.Add("Mb", "العضو");
            g1.Columns.Add("Bd", "استعار"); g1.Columns.Add("Dd", "موعد"); g1.Columns.Add("Rd", "أُرجع"); g1.Columns.Add("St", "الحالة");
            foreach (var r in db.GetAllRecords())
                g1.Rows.Add(r.RecordId, r.BookTitle, r.MemberName, r.BorrowDate.ToString("yyyy/MM/dd"),
                    r.DueDate.ToString("yyyy/MM/dd"),
                    r.ReturnDate.HasValue ? r.ReturnDate.Value.ToString("yyyy/MM/dd") : "-",
                    r.Status == BorrowStatus.Returned ? "✅ مُرجع" : DateTime.Now > r.DueDate ? "⚠️ متأخر" : "📤 مستعار");
            tabHistory.Controls.Add(g1);

            // Tab 2: overdue
            var tabOverdue = new TabPage("⚠️ المتأخرون");
            var g2 = CreateGrid(new Point(5, 5), new Size(tab.Width - 20, tab.Height - 50));
            g2.Columns.Add("R", "#"); g2.Columns.Add("Bk", "الكتاب"); g2.Columns.Add("Mb", "العضو");
            g2.Columns.Add("Dd", "كان المفروض يرجع"); g2.Columns.Add("Delay", "تأخر (يوم)"); g2.Columns.Add("Fine", "الغرامة");
            foreach (var r in db.GetActiveBorrows().Where(r => DateTime.Now > r.DueDate))
                g2.Rows.Add(r.RecordId, r.BookTitle, r.MemberName, r.DueDate.ToString("yyyy/MM/dd"),
                    (DateTime.Now - r.DueDate).Days, r.FineAmount() + " جنيه");
            tabOverdue.Controls.Add(g2);

            tab.TabPages.Add(tabHistory);
            tab.TabPages.Add(tabOverdue);
            contentPanel.Controls.Add(tab);
        }

        // ===================== HELPERS =====================
        private DataGridView CreateGrid(Point loc, Size sz)
        {
            var g = new DataGridView
            {
                Location = loc,
                Size = sz,
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                Font = new Font("Segoe UI", 9.5f),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                GridColor = Color.FromArgb(226, 232, 240),
                RightToLeft = RightToLeft.Yes,
            };
            g.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(30, 41, 59);
            g.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            g.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            g.ColumnHeadersHeight = 36;
            g.RowTemplate.Height = 32;
            g.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);
            return g;
        }

        private Button StyledBtn(string text, int x, int y, Color bg)
        {
            return new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(130, 35),
                BackColor = bg,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                Cursor = Cursors.Hand,
                FlatAppearance = { BorderSize = 0 }
            };
        }
    }
}
