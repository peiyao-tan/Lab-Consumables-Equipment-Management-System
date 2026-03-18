using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace 实验耗材及设备物资管理系统
{
    public partial class ItemsMasterControl : UserControl
    {
        private string connectionString;
        private DatabaseHelper dbHelper;
        private string currentLab;

        // 表格和搜索控件
        private DataGridView itemsDataGridView;
        private TextBox searchTextBox;
        private ComboBox itemTypeComboBox;
        private Button searchButton;
        private Button addButton;
        private Button editButton;
        private Button deleteButton;

        // 界面颜色配置，与实验管理员界面保持一致
        private readonly Color primaryColor = Color.FromArgb(41, 128, 185);
        private readonly Color backgroundColor = Color.FromArgb(245, 246, 250);
        private readonly Color textColor = Color.FromArgb(52, 73, 94);

        public ItemsMasterControl(string connString, string labName)
        {
            InitializeComponent();
            connectionString = connString;
            currentLab = labName;
            dbHelper = new DatabaseHelper(connectionString);
            InitializeItemsMasterInterface();
        }

        private void InitializeItemsMasterInterface()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = backgroundColor;
            this.Padding = new Padding(10);

            // 创建顶部搜索和操作区域
            Panel topPanel = new Panel
            {
                Height = 80,
                Dock = DockStyle.Top,
                BackColor = Color.White,
                Padding = new Padding(10)
            };
            this.Controls.Add(topPanel);

            // 搜索标签
            Label searchLabel = new Label
            {
                Text = "搜索:",
                Font = new Font("微软雅黑", 9),
                ForeColor = textColor,
                AutoSize = true,
                Location = new Point(10, 15)
            };
            topPanel.Controls.Add(searchLabel);

            // 搜索文本框
            searchTextBox = new TextBox
            {
                Width = 200,
                Location = new Point(50, 10),
                Font = new Font("微软雅黑", 9),
                BorderStyle = BorderStyle.FixedSingle
            };
            topPanel.Controls.Add(searchTextBox);

            // 类型下拉框
            Label typeLabel = new Label
            {
                Text = "类型:",
                Font = new Font("微软雅黑", 9),
                ForeColor = textColor,
                AutoSize = true,
                Location = new Point(260, 15)
            };
            topPanel.Controls.Add(typeLabel);

            itemTypeComboBox = new ComboBox
            {
                Width = 120,
                Location = new Point(300, 10),
                Font = new Font("微软雅黑", 9),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            itemTypeComboBox.Items.AddRange(new object[] { "全部", "实验设备", "化学试剂", "玻璃器皿", "其他耗材" });
            itemTypeComboBox.SelectedIndex = 0;
            topPanel.Controls.Add(itemTypeComboBox);

            // 搜索按钮
            searchButton = new Button
            {
                Text = "搜索",
                Width = 80,
                Height = 30,
                Location = new Point(430, 10),
                Font = new Font("微软雅黑", 9),
                BackColor = primaryColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            searchButton.FlatAppearance.BorderSize = 0;
            searchButton.Click += SearchButton_Click;
            topPanel.Controls.Add(searchButton);

            // 添加按钮
            addButton = new Button
            {
                Text = "添加",
                Width = 80,
                Height = 30,
                Location = new Point(520, 10),
                Font = new Font("微软雅黑", 9),
                BackColor = Color.Green,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            addButton.FlatAppearance.BorderSize = 0;
            addButton.Click += AddButton_Click;
            topPanel.Controls.Add(addButton);

            // 编辑按钮
            editButton = new Button
            {
                Text = "编辑",
                Width = 80,
                Height = 30,
                Location = new Point(610, 10),
                Font = new Font("微软雅黑", 9),
                BackColor = Color.Orange,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            editButton.FlatAppearance.BorderSize = 0;
            editButton.Click += EditButton_Click;
            topPanel.Controls.Add(editButton);

            // 删除按钮
            deleteButton = new Button
            {
                Text = "删除",
                Width = 80,
                Height = 30,
                Location = new Point(700, 10),
                Font = new Font("微软雅黑", 9),
                BackColor = Color.Red,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            deleteButton.FlatAppearance.BorderSize = 0;
            deleteButton.Click += DeleteButton_Click;
            topPanel.Controls.Add(deleteButton);

            // 创建表格区域
            Panel tablePanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(10)
            };
            this.Controls.Add(tablePanel);

            // 创建数据网格视图
            itemsDataGridView = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false,
                BorderStyle = BorderStyle.None,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                GridColor = Color.LightGray
            };

            // 设置表格样式
            itemsDataGridView.EnableHeadersVisualStyles = false;
            itemsDataGridView.ColumnHeadersDefaultCellStyle.Font = new Font("微软雅黑", 9, FontStyle.Bold);
            itemsDataGridView.ColumnHeadersDefaultCellStyle.BackColor = primaryColor;
            itemsDataGridView.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            itemsDataGridView.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            itemsDataGridView.DefaultCellStyle.Font = new Font("微软雅黑", 9);
            itemsDataGridView.DefaultCellStyle.ForeColor = textColor;
            itemsDataGridView.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(249, 249, 249);

            tablePanel.Controls.Add(itemsDataGridView);

            // 加载数据
            LoadItemsData();
        }

        private void LoadItemsData()
        {
            try
            {
                // 从数据库加载数据
                string query = "SELECT i.code AS 编号, i.name AS 名称, i.type AS 类型, i.unit AS 单位, " +
                               "0 AS 库存数量, 0 AS 预警数量, '' AS 备注, i.active AS 启用状态 " +
                               "FROM items i " +
                               "WHERE i.active = 1";

                DataTable dt = dbHelper.ExecuteQuery(query);

                // 为了确保表格能正常显示，即使数据库中没有数据，也创建基本结构
                if (dt.Rows.Count == 0)
                {
                    dt.Columns.Add("编号", typeof(string));
                    dt.Columns.Add("名称", typeof(string));
                    dt.Columns.Add("类型", typeof(string));
                    dt.Columns.Add("单位", typeof(string));
                    dt.Columns.Add("库存数量", typeof(int));
                    dt.Columns.Add("预警数量", typeof(int));
                    dt.Columns.Add("备注", typeof(string));
                    dt.Columns.Add("启用状态", typeof(bool));
                }

                itemsDataGridView.DataSource = dt;
            }
            catch (Exception ex)
            {
                // 捕获异常时，使用示例数据作为备用
                try
                {
                    DataTable dt = new DataTable();
                    dt.Columns.Add("编号", typeof(string));
                    dt.Columns.Add("名称", typeof(string));
                    dt.Columns.Add("类型", typeof(string));
                    dt.Columns.Add("单位", typeof(string));
                    dt.Columns.Add("库存数量", typeof(int));
                    dt.Columns.Add("预警数量", typeof(int));
                    dt.Columns.Add("备注", typeof(string));
                    dt.Columns.Add("启用状态", typeof(bool));

                    // 示例数据
                    dt.Rows.Add("ITEM001", "微型离心机", "实验设备", "台", 10, 2, "用于样本离心", true);
                    dt.Rows.Add("ITEM002", "烧杯", "玻璃器皿", "个", 100, 20, "500ml", true);
                    dt.Rows.Add("ITEM003", "乙醇", "化学试剂", "瓶", 50, 10, "95%纯度", true);
                    dt.Rows.Add("ITEM004", "移液器", "实验设备", "支", 15, 3, "100-1000ul", true);
                    dt.Rows.Add("ITEM005", "滤纸", "其他耗材", "包", 20, 5, "定性滤纸", true);

                    itemsDataGridView.DataSource = dt;
                }
                catch { }

                MessageBox.Show("加载数据失败: " + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {
            try
            {
                // 实现搜索功能
                string searchText = searchTextBox.Text.Trim().ToLower();
                string itemType = itemTypeComboBox.SelectedItem.ToString();

                string query = "SELECT i.code AS 编号, i.name AS 名称, i.type AS 类型, i.unit AS 单位, " +
                               "0 AS 库存数量, 0 AS 预警数量, '' AS 备注, i.active AS 启用状态 " +
                               "FROM items i " +
                               "WHERE i.active = 1 ";

                Dictionary<string, object> parameters = new Dictionary<string, object>();

                // 添加搜索条件
                if (!string.IsNullOrEmpty(searchText))
                {
                    query += "AND (LOWER(i.code) LIKE @searchText OR LOWER(i.name) LIKE @searchText) ";
                    parameters.Add("@searchText", "%" + searchText + "%");
                }

                // 添加类型条件
                if (itemType != "全部")
                {
                    query += "AND i.type = @type ";
                    parameters.Add("@type", itemType);
                }



                DataTable dt = dbHelper.ExecuteQuery(query, parameters);
                itemsDataGridView.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("搜索失败: " + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            try
            {
                // 创建并显示新建物资对话框
                using (ItemDetailForm form = new ItemDetailForm(dbHelper, currentLab))
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        // 保存成功后刷新数据
                        LoadItemsData();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("添加失败: " + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void EditButton_Click(object sender, EventArgs e)
        {
            try
            {
                // 实现编辑功能
                if (itemsDataGridView.SelectedRows.Count > 0)
                {
                    string itemId = itemsDataGridView.SelectedRows[0].Cells["编号"].Value.ToString();

                    // 创建并显示编辑物资对话框
                    using (ItemDetailForm form = new ItemDetailForm(dbHelper, currentLab, itemId))
                    {
                        if (form.ShowDialog() == DialogResult.OK)
                        {
                            // 保存成功后刷新数据
                            LoadItemsData();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("请先选择要编辑的物品", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("编辑失败: " + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            try
            {
                // 实现删除功能
                if (itemsDataGridView.SelectedRows.Count > 0)
                {
                    string itemId = itemsDataGridView.SelectedRows[0].Cells["编号"].Value.ToString();
                    string itemName = itemsDataGridView.SelectedRows[0].Cells["名称"].Value.ToString();

                    if (MessageBox.Show($"确定要停用'{itemName}'吗？\n停用后该物资将不会显示在列表中，但相关数据会保留。", "确认停用", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        // 开始事务
                        dbHelper.BeginTransaction();

                        try
                        {
                            // 实际上执行停用操作而非物理删除
                            string query = "UPDATE items SET active = 0, updated_at = NOW() WHERE code = @code";
                            Dictionary<string, object> parameters = new Dictionary<string, object> { { "@code", itemId } };
                            dbHelper.ExecuteNonQuery(query, parameters);

                            // 写入审计日志
                            WriteAuditLog(itemId, "DELETE", $"停用物资: {itemName}");

                            // 提交事务
                            dbHelper.CommitTransaction();

                            MessageBox.Show("停用成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // 刷新数据
                            LoadItemsData();
                        }
                        catch (Exception ex)
                        {
                            // 回滚事务
                            dbHelper.RollbackTransaction();
                            MessageBox.Show("停用失败: " + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("请先选择要停用的物品", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("操作失败: " + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void WriteAuditLog(string itemCode, string action, string description)
        {
            try
            {
                string query = "INSERT INTO audit_logs (entity_type, entity_id, action, description, created_by, created_at) VALUES ('item', @itemCode, @action, @description, @createdBy, NOW())";
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters.Add("@itemCode", itemCode);
                parameters.Add("@action", action);
                parameters.Add("@description", description);
                parameters.Add("@createdBy", "当前用户"); // 实际应用中应获取当前登录用户
                dbHelper.ExecuteNonQuery(query, parameters);
            }
            catch (Exception ex)
            {
                // 审计日志记录失败不应影响主业务流程
                Console.WriteLine("记录审计日志失败: " + ex.Message);
            }
        }

        public void RefreshData()
        {
            LoadItemsData();
        }

        // 当选择的实验室改变时更新数据
        public void UpdateLab(string labName)
        {
            currentLab = labName;
            LoadItemsData();
        }
    }
}