using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class NewItemForm : Form
    {
        private MySqlConnection connection;
        // 定义供应商数据结构
        private class SupplierItem
        {
            public string Id { get; set; } // 存储UUID
            public string Name { get; set; } // 显示名称
            public override string ToString() => Name; // ComboBox显示名称
        }

        public NewItemForm()
        {
            InitializeComponent();
            InitializeDatabaseConnection();
            SetupInputValidation();
            chkActive.Checked = true; // 默认启用

            // 关键修复：加载供应商数据
            LoadSuppliers();
        }

        private void InitializeDatabaseConnection()
        {
            string connectionString = "server=localhost;database=try;uid=root;pwd=123456;";
            connection = new MySqlConnection(connectionString);
        }

        // ✅ 新增方法：从数据库加载供应商
        private void LoadSuppliers()
        {
            cboSupplier.Items.Clear();
            // 添加提示项
            cboSupplier.Items.Add(new SupplierItem { Name = "-- 请选择供应商 --", Id = "" });

            try
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();

                // 只加载有效的UUID供应商
                string sql = @"
                    SELECT id, name 
                    FROM suppliers 
                    WHERE active = 1
                      AND id REGEXP '^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$'
                    ORDER BY name";

                using (var cmd = new MySqlCommand(sql, connection))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            cboSupplier.Items.Add(new SupplierItem
                            {
                                Id = reader["id"].ToString().Trim(),
                                Name = reader["name"].ToString().Trim()
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载供应商失败：{ex.Message}", "数据库错误",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
                // 添加测试数据（备用）
                cboSupplier.Items.Add(new SupplierItem { Name = "试剂公司", Id = "0364269a-34ec-4315-afe4-d6385df9385a" });
                cboSupplier.Items.Add(new SupplierItem { Name = "仪器公司", Id = "d9c6c293-d88a-4e84-a030-110263ddd546" });
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            cboSupplier.SelectedIndex = 0; // 默认选中提示项
        }

        private void SetupInputValidation()
        {
            txtMinStock.KeyPress += TextBox_OnlyDigits;
            txtMinStock.TextChanged += TextBox_CleanupNonDigits;
        }

        private void TextBox_OnlyDigits(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void TextBox_CleanupNonDigits(object sender, EventArgs e)
        {
            var textBox = sender as TextBox;
            if (string.IsNullOrEmpty(textBox.Text)) return;

            string cleaned = Regex.Replace(textBox.Text, @"[^\d]", "");
            if (textBox.Text != cleaned)
            {
                textBox.Text = cleaned;
                textBox.SelectionStart = textBox.Text.Length;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // 验证必填项
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("请输入物资名称！", "输入错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtName.Focus();
                return;
            }

            if (cboSupplier.SelectedIndex <= 0) // 未选择有效供应商
            {
                MessageBox.Show("请选择有效的供应商！", "输入错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cboSupplier.Focus();
                return;
            }

            // ✅ 修复1：生成标准UUID（36字符带连字符）
            string id = Guid.NewGuid().ToString(); // 默认格式就是带连字符
            string code = Guid.NewGuid().ToString("N").Substring(0, 16).ToUpper();

            try
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();

                // ✅ 修复2：获取真实的供应商UUID
                SupplierItem selectedSupplier = cboSupplier.SelectedItem as SupplierItem;
                string supplierId = selectedSupplier.Id;

                string sql = @"
                    INSERT INTO items (id, code, name, type, unit, min_stock, supplier_id, active)
                    VALUES (@id, @code, @name, @type, @unit, @min_stock, @supplier_id, @active)";

                using (var cmd = new MySqlCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@code", code);
                    cmd.Parameters.AddWithValue("@name", txtName.Text.Trim());
                    cmd.Parameters.AddWithValue("@type", cboType.SelectedItem?.ToString() ?? "");
                    cmd.Parameters.AddWithValue("@unit", txtUnit.Text.Trim());
                    cmd.Parameters.AddWithValue("@min_stock", int.Parse(txtMinStock.Text));
                    cmd.Parameters.AddWithValue("@supplier_id", supplierId); // 传入UUID字符串
                    cmd.Parameters.AddWithValue("@active", chkActive.Checked ? 1 : 0);

                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("物资新增成功！", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                // ✅ 修复3：添加详细错误诊断
                string errorDetail = ex.Message;
                if (ex.InnerException != null)
                    errorDetail += $"\n内部错误: {ex.InnerException.Message}";

                MessageBox.Show($"保存失败：{errorDetail}", "数据库错误",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);

                // 记录详细错误（开发用）
                Console.WriteLine($"SQL Error: {ex.StackTrace}");
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
        }

        

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}