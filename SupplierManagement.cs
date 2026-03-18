using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows.Forms;
using 实验耗材及设备物资管理系统;

namespace WindowsFormsApp1
{
    public partial class SupplierManagement : UserControl
    {
        private string connectionString;
        private string selectedSupplierId = null; // 从Guid?改为string类型
        private DatabaseHelper dbHelper;
        private string currentLab;

        public SupplierManagement()
        {
            InitializeComponent();
            connectionString = "server=localhost;user=root;password=123456;database=try;SslMode=None;";
            InitializeDataGridView();
            LoadSuppliers();

        }

        public SupplierManagement(DatabaseHelper helper, string labId)
        {
            InitializeComponent();
            dbHelper = helper;
            currentLab = labId;
            connectionString = dbHelper.ConnectionString;
            InitializeDataGridView();
            LoadSuppliers();

        }



        private void InitializeDataGridView()
        {
            dgvSuppliers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvSuppliers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvSuppliers.MultiSelect = false;

            dgvSuppliers.SelectionChanged += (sender, e) =>
            {
                if (dgvSuppliers.SelectedRows.Count > 0)
                {
                    // 直接获取字符串ID，不再进行Guid转换
                    selectedSupplierId = dgvSuppliers.SelectedRows[0].Cells["ID"].Value?.ToString();
                }
                else
                {
                    selectedSupplierId = null;
                }
            };
        }

        private void LoadSuppliers(string searchKeyword = "")
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT id AS 'ID', " +
                                   "name AS '供应商名称', " +
                                   "contact_name AS '联系人', " +
                                   "phone AS '联系电话', " +
                                   "address AS '地址', " +
                                   "CASE WHEN active = 1 THEN '启用' ELSE '禁用' END AS '状态' " +
                                   "FROM suppliers WHERE 1=1";

                    if (!string.IsNullOrEmpty(searchKeyword))
                    {
                        query += " AND (name LIKE @keyword OR contact_name LIKE @keyword)";
                    }

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    if (!string.IsNullOrEmpty(searchKeyword))
                    {
                        cmd.Parameters.AddWithValue("@keyword", $"%{searchKeyword}%");
                    }

                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dgvSuppliers.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载数据失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 搜索按钮事件
        private void btnSearch_Click(object sender, EventArgs e)
        {
            LoadSuppliers(textBox1.Text.Trim());
        }

        // 新增按钮事件
        private void btnAdd_Click(object sender, EventArgs e)
        {
            SupplierEditForm editForm = new SupplierEditForm(null, connectionString);
            if (editForm.ShowDialog() == DialogResult.OK)
            {
                LoadSuppliers(textBox1.Text.Trim());
            }
        }

        // 编辑按钮事件
        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedSupplierId))
            {
                MessageBox.Show("请先选择要编辑的供应商", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            SupplierEditForm editForm = new SupplierEditForm(selectedSupplierId, connectionString);
            if (editForm.ShowDialog() == DialogResult.OK)
            {
                LoadSuppliers(textBox1.Text.Trim());
            }
        }

        // 删除按钮事件
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedSupplierId))
            {
                MessageBox.Show("请先选择要删除的供应商", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (MessageBox.Show("确定要删除该供应商吗？删除后不可恢复！", "确认删除",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                try
                {
                    using (MySqlConnection conn = new MySqlConnection(connectionString))
                    {
                        conn.Open();
                        string query = "UPDATE suppliers SET active = false WHERE id = @id";
                        MySqlCommand cmd = new MySqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@id", selectedSupplierId); // 直接使用字符串ID
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("删除成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadSuppliers(textBox1.Text.Trim());
                        }
                        else
                        {
                            MessageBox.Show("删除失败，未找到该供应商", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"删除失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            // 可选：实现实时搜索
            // LoadSuppliers(textBox1.Text.Trim());
        }

        private void SupplierManagement_Load(object sender, EventArgs e)
        {

        }
    }
}