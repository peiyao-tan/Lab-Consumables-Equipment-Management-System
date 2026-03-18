using System;
using System.Data;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace WindowsFormsApp1
{
    public partial class EditItemForm : Form
    {
        private MySqlConnection connection;
        private string itemId; // 改为string类型

        private readonly Regex _itemCodeRegex = new Regex(@"^[A-Z]{3,5}-\d{3}$", RegexOptions.Compiled);

        public EditItemForm(string id) // 改为string类型
        {
            InitializeComponent();
            itemId = id;
            InitializeDatabaseConnection();
            LoadCurrentItemData();
            SetupEventHandlers();
        }

        private void InitializeDatabaseConnection()
        {
            string connStr = "server=localhost;database=try;uid=root;pwd=123456;";
            connection = new MySqlConnection(connStr);
        }

        private void SetupEventHandlers()
        {
            btnSave.Click += btnSave_Click;
            btnCancel.Click += btnCancel_Click;

            txtItemCode.KeyPress += (s, e) => {
                if (!char.IsLetterOrDigit(e.KeyChar) && !char.IsControl(e.KeyChar) && e.KeyChar != '-')
                    e.Handled = true;
            };

            txtItemCode.TextChanged += (s, e) => {
                var tb = s as TextBox;
                if (tb == null || string.IsNullOrEmpty(tb.Text)) return;
                string cleaned = Regex.Replace(tb.Text, @"[^A-Za-z0-9\-]", "");
                if (tb.Text != cleaned)
                {
                    tb.Text = cleaned.ToUpper();
                    tb.SelectionStart = tb.Text.Length;
                }
            };

            txtMinStock.KeyPress += (s, e) => {
                if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
                    e.Handled = true;
            };
        }

        private void LoadCurrentItemData()
        {
            try
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();

                string sql = @"SELECT code, name, type, unit, min_stock, supplier_id, active 
                               FROM items WHERE id = @id";
                MySqlCommand cmd = new MySqlCommand(sql, connection);
                cmd.Parameters.AddWithValue("@id", itemId); // 使用string类型的itemId

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        txtItemCode.Text = reader["code"].ToString();
                        txtName.Text = reader["name"].ToString();
                        txtUnit.Text = reader["unit"].ToString();
                        txtMinStock.Text = reader["min_stock"].ToString();
                        chkActive.Checked = Convert.ToBoolean(reader["active"]);

                        string type = reader["type"].ToString();
                        cboType.SelectedItem = cboType.Items.Contains(type) ? type : null;

                        object supplierIdObj = reader["supplier_id"];
                        if (supplierIdObj != DBNull.Value)
                        {
                            string supplierId = supplierIdObj.ToString();
                            LoadSupplierNameById(supplierId);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载物资数据失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
        }

        private void LoadSupplierNameById(string supplierId)
        {
            try
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();

                string sql = "SELECT name FROM suppliers WHERE id = @id";
                MySqlCommand cmd = new MySqlCommand(sql, connection);
                cmd.Parameters.AddWithValue("@id", supplierId);

                object result = cmd.ExecuteScalar();
                if (result != null)
                {
                    string supplierName = result.ToString();
                    cboSupplier.SelectedItem = cboSupplier.Items.Contains(supplierName)
                        ? supplierName
                        : null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载供应商失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateAllInputs()) return;

            try
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();

                // 检查物资编号是否已存在
                if (IsItemCodeExists(txtItemCode.Text.Trim(), itemId))
                {
                    MessageBox.Show("物资编号已存在，请使用其他编号！", "输入错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtItemCode.Focus();
                    return;
                }

                string supplierId = GetSupplierIdByName(cboSupplier.SelectedItem?.ToString());
                if (string.IsNullOrEmpty(supplierId))
                {
                    MessageBox.Show("所选供应商不存在，请重新选择！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string sql = @"UPDATE items 
                               SET code = @code, name = @name, type = @type, unit = @unit, 
                                   min_stock = @min_stock, supplier_id = @supplier_id, 
                                   active = @active 
                               WHERE id = @id";

                MySqlCommand cmd = new MySqlCommand(sql, connection);
                cmd.Parameters.AddWithValue("@code", txtItemCode.Text.Trim());
                cmd.Parameters.AddWithValue("@name", txtName.Text.Trim());
                cmd.Parameters.AddWithValue("@type", cboType.SelectedItem?.ToString());
                cmd.Parameters.AddWithValue("@unit", txtUnit.Text.Trim());
                cmd.Parameters.AddWithValue("@min_stock", Convert.ToInt32(txtMinStock.Text.Trim()));
                cmd.Parameters.AddWithValue("@supplier_id", supplierId);
                cmd.Parameters.AddWithValue("@active", chkActive.Checked);
                cmd.Parameters.AddWithValue("@id", itemId);

                int affectedRows = cmd.ExecuteNonQuery();
                if (affectedRows > 0)
                {
                    MessageBox.Show("物资编辑成功！", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("未检测到数据变更，或物资ID不存在。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
        }

        // 修改这个方法：第二个参数改为string类型
        private bool IsItemCodeExists(string itemCode, string excludeId)
        {
            try
            {
                string sql = "SELECT COUNT(*) FROM items WHERE code = @code AND id != @excludeId";
                MySqlCommand cmd = new MySqlCommand(sql, connection);
                cmd.Parameters.AddWithValue("@code", itemCode);
                cmd.Parameters.AddWithValue("@excludeId", excludeId); // 使用string类型

                int count = Convert.ToInt32(cmd.ExecuteScalar());
                return count > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"检查物资编号失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return true;
            }
        }

        private string GetSupplierIdByName(string supplierName)
        {
            try
            {
                string sql = "SELECT id FROM suppliers WHERE name = @name";
                MySqlCommand cmd = new MySqlCommand(sql, connection);
                cmd.Parameters.AddWithValue("@name", supplierName);

                object result = cmd.ExecuteScalar();
                return result?.ToString() ?? "";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"查询供应商ID失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return "";
            }
        }

        private bool ValidateAllInputs()
        {
            if (string.IsNullOrWhiteSpace(txtItemCode.Text.Trim()))
            {
                MessageBox.Show("请输入物资编号！", "输入错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtItemCode.Focus();
                return false;
            }
            if (!_itemCodeRegex.IsMatch(txtItemCode.Text.Trim()))
            {
                MessageBox.Show("物资编号格式错误！需为「前缀-三位数字」（如 CHEM-001）。", "输入错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtItemCode.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtName.Text.Trim()))
            {
                MessageBox.Show("请输入物资名称！", "输入错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtName.Focus();
                return false;
            }

            if (cboType.SelectedItem == null)
            {
                MessageBox.Show("请选择物资类型！", "输入错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cboType.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtUnit.Text.Trim()))
            {
                MessageBox.Show("请输入计量单位！", "输入错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtUnit.Focus();
                return false;
            }

            if (!int.TryParse(txtMinStock.Text.Trim(), out int minStock) || minStock < 0)
            {
                MessageBox.Show("最小库存必须是非负整数！", "输入错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMinStock.Focus();
                return false;
            }

            if (cboSupplier.SelectedItem == null)
            {
                MessageBox.Show("请选择供应商！", "输入错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cboSupplier.Focus();
                return false;
            }

            return true;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}