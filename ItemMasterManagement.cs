using MySql.Data.MySqlClient;
using OfficeOpenXml;
using System;
using System.Data;
using System.IO;
using System.Windows.Forms;
using 实验耗材及设备物资管理系统;

namespace WindowsFormsApp1
{
    public partial class ItemMasterManagement : UserControl
    {
        private MySqlConnection connection;
        private LogAudit _logAudit;
        public ItemMasterManagement()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            InitializeComponent();
            InitializeDatabaseConnection();
            LoadItemTypeData();
            LoadItemData();


            // 绑定事件
            btn_Search.Click += btn_Search_Click;
            btnNewItem.Click += btnNewItem_Click;
            btnImportItems.Click += btnImportItems_Click;
            btnExportItems.Click += btnExportItems_Click;
            btnEditItem.Click += btnEditItem_Click;
            chk_Active.CheckedChanged += chk_Active_CheckedChanged;

            dgv_ItemList.CellDoubleClick += dgv_ItemList_CellDoubleClick;
        }

        private void InitializeDatabaseConnection()
        {
            string connectionString = "server=localhost;database=try;uid=root;pwd=123456;";
            connection = new MySqlConnection(connectionString);
        }

        private void LoadItemTypeData()
        {
            cbo_ItemType.Items.Clear();
            cbo_ItemType.Items.Add("全部");
            cbo_ItemType.Items.Add("耗材");
            cbo_ItemType.Items.Add("设备");
            cbo_ItemType.Items.Add("化学品");
            cbo_ItemType.SelectedIndex = 0;
        }

        private void LoadItemData(string keyword = "", string itemType = "全部")
        {
            try
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();

                string sql = @"
                    SELECT 
                        i.id AS ID,
                        i.code AS 物资编号,
                        i.name AS 物资名称,
                        i.type AS 类型,
                        i.unit AS 单位,
                        i.min_stock AS 最小库存,
                        s.name AS 供应商,
                        CASE WHEN i.active = 1 THEN '启用' ELSE '停用' END AS 状态
                    FROM items i
                    LEFT JOIN suppliers s ON i.supplier_id = s.id
                    WHERE 1=1";

                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    sql += " AND (i.name LIKE @keyword OR i.code LIKE @keyword)";
                }
                if (itemType != "全部")
                {
                    sql += " AND i.type = @type";
                }

                using (MySqlCommand cmd = new MySqlCommand(sql, connection))
                {
                    if (!string.IsNullOrWhiteSpace(keyword))
                        cmd.Parameters.AddWithValue("@keyword", $"%{keyword}%");
                    if (itemType != "全部")
                        cmd.Parameters.AddWithValue("@type", itemType);

                    DataTable dt = new DataTable();
                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }

                    dgv_ItemList.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载数据失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
        }

        private void btn_Search_Click(object sender, EventArgs e)
        {
            string keyword = txt_SearchKeyword.Text.Trim();
            string itemType = cbo_ItemType.SelectedItem?.ToString() ?? "全部";
            LoadItemData(keyword, itemType);
        }

        private void btnNewItem_Click(object sender, EventArgs e)
        {
            using (var form = new NewItemForm())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadItemData();
                }
            }
        }

        private void btnEditItem_Click(object sender, EventArgs e)
        {
            if (dgv_ItemList.SelectedRows.Count == 0)
            {
                MessageBox.Show("请先选择一条记录！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var row = dgv_ItemList.SelectedRows[0];

            // 直接获取GUID字符串
            string itemId = row.Cells["ID"].Value?.ToString();

            if (string.IsNullOrEmpty(itemId))
            {
                MessageBox.Show("无法获取物资ID！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 调试：显示获取到的ID
            Console.WriteLine($"获取到的物资ID: {itemId}");

            // 打开编辑窗体（传入 string 类型的GUID）
            using (var editForm = new EditItemForm(itemId))
            {
                if (editForm.ShowDialog() == DialogResult.OK)
                {
                    LoadItemData();
                }
            }
        }

        private void dgv_ItemList_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                btnEditItem.PerformClick();
            }
        }

        private void btnImportItems_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Filter = "Excel 文件|*.xlsx;*.xls",
                Title = "选择  要导入的物资文件"
            };

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                MessageBox.Show("批量导入功能将在后续版本实现。\n\n文件路径：" + ofd.FileName, "提示");
            }
        }

        private void btnExportItems_Click(object sender, EventArgs e)
        {
            if (dgv_ItemList.Rows.Count == 0)
            {
                MessageBox.Show("没有数据可导出！", "提示");
                return;
            }

            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "Excel 工作簿|*.xlsx",
                FileName = $"物资主数据_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
            };

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (var package = new ExcelPackage())
                    {
                        var worksheet = package.Workbook.Worksheets.Add("物资列表");

                        for (int col = 0; col < dgv_ItemList.Columns.Count; col++)
                        {
                            worksheet.Cells[1, col + 1].Value = dgv_ItemList.Columns[col].HeaderText;
                        }

                        for (int row = 0; row < dgv_ItemList.Rows.Count; row++)
                        {
                            var gridRow = dgv_ItemList.Rows[row];
                            if (gridRow.IsNewRow) continue;

                            for (int col = 0; col < dgv_ItemList.Columns.Count; col++)
                            {
                                worksheet.Cells[row + 2, col + 1].Value = gridRow.Cells[col].Value?.ToString();
                            }
                        }

                        worksheet.Cells.AutoFitColumns();
                        FileInfo file = new FileInfo(sfd.FileName);
                        package.SaveAs(file);

                        MessageBox.Show("导出成功！", "成功");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"导出失败：{ex.Message}", "错误");
                }
            }
        }

        private void chk_Active_CheckedChanged(object sender, EventArgs e)
        {
            string keyword = txt_SearchKeyword.Text.Trim();
            string itemType = cbo_ItemType.SelectedItem?.ToString() ?? "全部";
            LoadItemData(keyword, itemType);
        }

        private void ItemMasterManagement_Load(object sender, EventArgs e)
        {

        }
    }
}