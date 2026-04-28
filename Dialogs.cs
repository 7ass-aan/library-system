using System;
using System.Drawing;
using System.Windows.Forms;

namespace WinFormsApp1
{
    // ===================== ADD BOOK DIALOG =====================
    public class AddBookForm : Form
    {
        public string BookTitle { get; private set; }
        public string Author { get; private set; }
        public BookCategory Category { get; private set; }
        public int Year { get; private set; }
        public int Copies { get; private set; }

        private TextBox txtTitle, txtAuthor, txtYear, txtCopies;
        private ComboBox cmbCategory;

        public AddBookForm()
        {
            this.Text = "إضافة كتاب جديد";
            this.Size = new Size(400, 370);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.White;
            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;

            int y = 20, lw = 120, tw = 210, lx = 20, tx = 150;

            void AddField(string label, Control ctrl)
            {
                this.Controls.Add(new Label { Text = label, Location = new Point(lx, y + 3), Width = lw, Font = new Font("Segoe UI", 10f) });
                ctrl.Location = new Point(tx, y);
                ctrl.Width = tw;
                ctrl.Font = new Font("Segoe UI", 10f);
                this.Controls.Add(ctrl);
                y += 45;
            }

            txtTitle = new TextBox();
            txtAuthor = new TextBox();
            txtYear = new TextBox { Text = DateTime.Now.Year.ToString() };
            txtCopies = new TextBox { Text = "1" };
            cmbCategory = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
            cmbCategory.Items.AddRange(new[] { "علوم", "أدب", "تاريخ", "تكنولوجيا", "دين", "طب", "فنون", "أخرى" });
            cmbCategory.SelectedIndex = 0;

            AddField("عنوان الكتاب:", txtTitle);
            AddField("اسم المؤلف:", txtAuthor);
            AddField("التصنيف:", cmbCategory);
            AddField("سنة النشر:", txtYear);
            AddField("عدد النسخ:", txtCopies);

            var btnOk = new Button
            {
                Text = "✅ حفظ",
                Location = new Point(20, y + 10),
                Size = new Size(120, 38),
                BackColor = Color.FromArgb(16, 185, 129),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                DialogResult = DialogResult.None,
                FlatAppearance = { BorderSize = 0 }
            };

            var btnCancel = new Button
            {
                Text = "إلغاء",
                Location = new Point(155, y + 10),
                Size = new Size(100, 38),
                BackColor = Color.FromArgb(239, 68, 68),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                DialogResult = DialogResult.Cancel
            };

            btnOk.Click += (s, e) =>
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(txtTitle.Text)) throw new Exception("أدخل عنوان الكتاب");
                    if (string.IsNullOrWhiteSpace(txtAuthor.Text)) throw new Exception("أدخل اسم المؤلف");
                    if (!int.TryParse(txtYear.Text, out int yr) || yr < 1000 || yr > DateTime.Now.Year)
                        throw new Exception("سنة النشر غير صحيحة");
                    if (!int.TryParse(txtCopies.Text, out int cp) || cp <= 0)
                        throw new Exception("عدد النسخ يجب أن يكون أكبر من صفر");

                    BookTitle = txtTitle.Text.Trim();
                    Author = txtAuthor.Text.Trim();
                    Category = (BookCategory)cmbCategory.SelectedIndex;
                    Year = yr;
                    Copies = cp;
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            };

            this.Controls.AddRange(new Control[] { btnOk, btnCancel });
        }
    }

    // ===================== ADD MEMBER DIALOG =====================
    public class AddMemberForm : Form
    {
        public string MemberName { get; private set; }
        public string Phone { get; private set; }
        public MemberType MType { get; private set; }

        private TextBox txtName, txtPhone;
        private ComboBox cmbType;

        public AddMemberForm()
        {
            this.Text = "إضافة عضو جديد";
            this.Size = new Size(380, 270);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.White;
            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;

            int y = 20, tx = 140, tw = 200;

            void AddField(string label, Control ctrl)
            {
                this.Controls.Add(new Label { Text = label, Location = new Point(15, y + 3), Width = 115, Font = new Font("Segoe UI", 10f) });
                ctrl.Location = new Point(tx, y);
                ctrl.Width = tw;
                ctrl.Font = new Font("Segoe UI", 10f);
                this.Controls.Add(ctrl);
                y += 45;
            }

            txtName = new TextBox();
            txtPhone = new TextBox();
            cmbType = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
            cmbType.Items.AddRange(new[] { "طالب (3 كتب)", "معلم (5 كتب)", "عادي (2 كتاب)" });
            cmbType.SelectedIndex = 0;

            AddField("اسم العضو:", txtName);
            AddField("رقم الهاتف:", txtPhone);
            AddField("نوع العضوية:", cmbType);

            var btnOk = new Button
            {
                Text = "✅ حفظ",
                Location = new Point(15, y + 10),
                Size = new Size(120, 38),
                BackColor = Color.FromArgb(16, 185, 129),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 }
            };

            var btnCancel = new Button
            {
                Text = "إلغاء",
                Location = new Point(145, y + 10),
                Size = new Size(100, 38),
                BackColor = Color.FromArgb(239, 68, 68),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                DialogResult = DialogResult.Cancel
            };

            btnOk.Click += (s, e) =>
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(txtName.Text)) throw new Exception("أدخل اسم العضو");
                    if (string.IsNullOrWhiteSpace(txtPhone.Text)) throw new Exception("أدخل رقم الهاتف");

                    MemberName = txtName.Text.Trim();
                    Phone = txtPhone.Text.Trim();
                    MType = (MemberType)cmbType.SelectedIndex;
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            };

            this.Controls.AddRange(new Control[] { btnOk, btnCancel });
        }
    }
}
