using MySql.Data.MySqlClient;
using System;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class SupplierEditForm : Form
    {
        private string _supplierId; // 改为字符串类型
        private string _connectionString;

        public SupplierEditForm(string supplierId, string connectionString)
        {
            InitializeComponent();
            _supplierId = supplierId;
            _connectionString = connectionString;

            if (!string.IsNullOrEmpty(_supplierId))
            {
                this.Text = "编辑供应商";
                LoadSupplierData();
            }
            else
            {
                this.Text = "新增供应商";
            }

            // 确保按钮事件正确绑定
            btnOK.Click += btnOK_Click;
            btnCancel.Click += btnCancel_Click;
        }

        private void LoadSupplierData()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = "SELECT name, contact_name, phone, address " +
                                   "FROM suppliers WHERE id = @id";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", _supplierId); // 直接使用字符串

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            txtName.Text = reader["name"].ToString();
                            txtContact.Text = reader["contact_name"].ToString();
                            txtPhone.Text = reader["phone"].ToString();
                            txtAddress.Text = reader["address"].ToString();
                        }
                        else
                        {
                            MessageBox.Show("未找到该供应商数据", "错误",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            this.DialogResult = DialogResult.Cancel;
                            this.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载数据失败：{ex.Message}", "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            // 验证输入
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("请输入供应商名称", "提示",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtName.Focus();
                return;
            }

            try
            {
                using (MySqlConnection conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();
                    if (!string.IsNullOrEmpty(_supplierId))
                    {
                        // 编辑模式
                        string updateQuery = @"UPDATE suppliers SET 
                                              name = @name, 
                                              contact_name = @contact, 
                                              phone = @phone, 
                                              address = @address 
                                              WHERE id = @id";
                        MySqlCommand cmd = new MySqlCommand(updateQuery, conn);
                        cmd.Parameters.AddWithValue("@name", txtName.Text.Trim());
                        cmd.Parameters.AddWithValue("@contact", txtContact.Text.Trim());
                        cmd.Parameters.AddWithValue("@phone", txtPhone.Text.Trim());
                        cmd.Parameters.AddWithValue("@address", txtAddress.Text.Trim());
                        cmd.Parameters.AddWithValue("@id", _supplierId); // 使用字符串ID
                        cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        // 新增模式
                        // 根据您的数据库结构，生成合适的ID
                        // 这里假设id是varchar类型，可以使用UUID或自定义格式
                        string newId = Guid.NewGuid().ToString(); // 仍然使用GUID格式，但作为字符串存储

                        string insertQuery = @"INSERT INTO suppliers (id, name, contact_name, phone, address, active) 
                                              VALUES (@id, @name, @contact, @phone, @address, 1)";
                        MySqlCommand cmd = new MySqlCommand(insertQuery, conn);
                        cmd.Parameters.AddWithValue("@id", newId);
                        cmd.Parameters.AddWithValue("@name", txtName.Text.Trim());
                        cmd.Parameters.AddWithValue("@contact", txtContact.Text.Trim());
                        cmd.Parameters.AddWithValue("@phone", txtPhone.Text.Trim());
                        cmd.Parameters.AddWithValue("@address", txtAddress.Text.Trim());
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("操作成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存失败：{ex.Message}", "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void label2_Click(object sender, EventArgs e)
        {
            // 无需处理
        }

        private void label4_Click(object sender, EventArgs e)
        {
            // 无需处理
        }
    }
}