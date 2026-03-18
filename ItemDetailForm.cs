using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace 实验耗材及设备物资管理系统
{
    public partial class ItemDetailForm : Form
    {
        private DatabaseHelper dbHelper;
        private string currentLab;
        private string itemId = null; // 如果为null则是新建，否则是编辑
        private bool isChemical = false;

        // 表单控件
        private Label itemCodeLabel;
        private TextBox itemCodeTextBox;
        private Label itemNameLabel;
        private TextBox itemNameTextBox;
        private Label itemTypeLabel;
        private ComboBox itemTypeComboBox;
        private Label categoryLabel;
        private ComboBox categoryComboBox;
        private Label unitLabel;
        private TextBox unitTextBox;
        private Label supplierLabel;
        private ComboBox supplierComboBox;
        private Label msdsUrlLabel;
        private TextBox msdsUrlTextBox;
        private Label descriptionLabel;
        private TextBox descriptionTextBox;
        private Button saveButton;
        private Button cancelButton;

        public ItemDetailForm(DatabaseHelper helper, string labName, string existingItemId = null)
        {
            InitializeComponent();
            dbHelper = helper;
            currentLab = labName;
            itemId = existingItemId;

            // 初始化界面
            InitializeItemDetailForm();

            // 如果是编辑模式，加载现有数据
            if (!string.IsNullOrEmpty(itemId))
            {
                LoadItemData();
            }
        }

        private void InitializeItemDetailForm()
        {
            this.Text = string.IsNullOrEmpty(itemId) ? "新建物资" : "编辑物资";
            this.Size = new Size(600, 500);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // 创建主面板
            Panel mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20),
                BackColor = Color.FromArgb(245, 246, 250)
            };
            this.Controls.Add(mainPanel);

            // 创建表单布局
            TableLayoutPanel formLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ColumnCount = 2,
                RowCount = 8,
                Padding = new Padding(10),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.Single
            };
            mainPanel.Controls.Add(formLayout);

            // 添加标签和文本框
            itemCodeLabel = new Label { Text = "物资编号：", Anchor = AnchorStyles.Right };
            itemCodeTextBox = new TextBox { Width = 250 };
            if (!string.IsNullOrEmpty(itemId)) itemCodeTextBox.ReadOnly = true;

            itemNameLabel = new Label { Text = "物资名称：", Anchor = AnchorStyles.Right };
            itemNameTextBox = new TextBox { Width = 250 };

            itemTypeLabel = new Label { Text = "物资类型：", Anchor = AnchorStyles.Right };
            itemTypeComboBox = new ComboBox { Width = 250, DropDownStyle = ComboBoxStyle.DropDownList };
            itemTypeComboBox.Items.AddRange(new string[] { "耗材", "设备", "化学品" });
            itemTypeComboBox.SelectedIndexChanged += ItemTypeComboBox_SelectedIndexChanged;

            categoryLabel = new Label { Text = "分类：", Anchor = AnchorStyles.Right };
            categoryComboBox = new ComboBox { Width = 250, DropDownStyle = ComboBoxStyle.DropDownList };
            categoryComboBox.Items.AddRange(new string[] { "玻璃器皿", "塑料耗材", "电子设备", "化学试剂", "其他" });

            unitLabel = new Label { Text = "单位：", Anchor = AnchorStyles.Right };
            unitTextBox = new TextBox { Width = 250 };

            supplierLabel = new Label { Text = "供应商：", Anchor = AnchorStyles.Right };
            supplierComboBox = new ComboBox { Width = 250, DropDownStyle = ComboBoxStyle.DropDownList };
            LoadSuppliers();

            msdsUrlLabel = new Label { Text = "MSDS URL：", Anchor = AnchorStyles.Right };
            msdsUrlTextBox = new TextBox { Width = 250 };
            msdsUrlLabel.Visible = false;
            msdsUrlTextBox.Visible = false;

            descriptionLabel = new Label { Text = "描述：", Anchor = AnchorStyles.Right | AnchorStyles.Top };
            descriptionTextBox = new TextBox { Width = 250, Height = 80, Multiline = true, ScrollBars = ScrollBars.Vertical };

            // 添加到表单布局
            formLayout.Controls.Add(itemCodeLabel, 0, 0);
            formLayout.Controls.Add(itemCodeTextBox, 1, 0);
            formLayout.Controls.Add(itemNameLabel, 0, 1);
            formLayout.Controls.Add(itemNameTextBox, 1, 1);
            formLayout.Controls.Add(itemTypeLabel, 0, 2);
            formLayout.Controls.Add(itemTypeComboBox, 1, 2);
            formLayout.Controls.Add(categoryLabel, 0, 3);
            formLayout.Controls.Add(categoryComboBox, 1, 3);
            formLayout.Controls.Add(unitLabel, 0, 4);
            formLayout.Controls.Add(unitTextBox, 1, 4);
            formLayout.Controls.Add(supplierLabel, 0, 5);
            formLayout.Controls.Add(supplierComboBox, 1, 5);
            formLayout.Controls.Add(msdsUrlLabel, 0, 6);
            formLayout.Controls.Add(msdsUrlTextBox, 1, 6);
            formLayout.Controls.Add(descriptionLabel, 0, 7);
            formLayout.Controls.Add(descriptionTextBox, 1, 7);

            // 设置行高
            for (int i = 0; i < 7; i++)
            {
                formLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            }
            formLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 100));

            // 创建按钮面板
            Panel buttonPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 60,
                Padding = new Padding(10)
            };
            mainPanel.Controls.Add(buttonPanel);

            // 添加按钮
            saveButton = new Button
            {
                Text = "保存",
                Width = 80,
                Height = 35,
                Location = new Point(buttonPanel.Width - 180, 10),
                BackColor = Color.FromArgb(0, 120, 212),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            saveButton.FlatAppearance.BorderSize = 0;
            saveButton.Click += SaveButton_Click;

            cancelButton = new Button
            {
                Text = "取消",
                Width = 80,
                Height = 35,
                Location = new Point(buttonPanel.Width - 90, 10),
                BackColor = Color.Gainsboro,
                FlatStyle = FlatStyle.Flat
            };
            cancelButton.FlatAppearance.BorderSize = 0;
            cancelButton.Click += CancelButton_Click;

            buttonPanel.Controls.Add(saveButton);
            buttonPanel.Controls.Add(cancelButton);
        }

        private void LoadSuppliers()
        {
            try
            {
                string query = "SELECT id, name FROM suppliers WHERE is_active = 1";
                DataTable dt = dbHelper.ExecuteQuery(query);
                if (dt != null && dt.Rows.Count > 0)
                {
                    supplierComboBox.DisplayMember = "name";
                    supplierComboBox.ValueMember = "id";
                    supplierComboBox.DataSource = dt;
                }
                else
                {
                    supplierComboBox.Items.Add("无供应商数据");
                    supplierComboBox.SelectedIndex = 0;
                    supplierComboBox.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("加载供应商数据失败: " + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ItemTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            isChemical = (itemTypeComboBox.SelectedItem?.ToString() == "化学品");
            msdsUrlLabel.Visible = isChemical;
            msdsUrlTextBox.Visible = isChemical;
        }

        private void LoadItemData()
        {
            try
            {
                string query = "SELECT * FROM items WHERE id = @itemId";
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters.Add("@itemId", itemId);
                DataTable dt = dbHelper.ExecuteQuery(query, parameters);

                if (dt != null && dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    itemCodeTextBox.Text = row["code"].ToString();
                    itemNameTextBox.Text = row["name"].ToString();
                    itemTypeComboBox.SelectedItem = row["type"].ToString();
                    categoryComboBox.SelectedItem = row["category"].ToString();
                    unitTextBox.Text = row["unit"].ToString();
                    descriptionTextBox.Text = row["description"].ToString();

                    // 设置供应商
                    if (row["supplier_id"] != DBNull.Value)
                    {
                        string supplierId = row["supplier_id"].ToString();
                        for (int i = 0; i < supplierComboBox.Items.Count; i++)
                        {
                            DataRowView item = supplierComboBox.Items[i] as DataRowView;
                            if (item != null && item["id"].ToString() == supplierId)
                            {
                                supplierComboBox.SelectedIndex = i;
                                break;
                            }
                        }
                    }

                    // 设置MSDS URL
                    if (row["msds_url"] != DBNull.Value)
                    {
                        msdsUrlTextBox.Text = row["msds_url"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("加载物资数据失败: " + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            try
            {
                // 合规校验
                if (!ValidateForm())
                {
                    return;
                }

                // 开始事务
                dbHelper.BeginTransaction();

                try
                {
                    string itemCode = itemCodeTextBox.Text.Trim();
                    string itemName = itemNameTextBox.Text.Trim();
                    string itemType = itemTypeComboBox.SelectedItem?.ToString();
                    string category = categoryComboBox.SelectedItem?.ToString();
                    string unit = unitTextBox.Text.Trim();
                    string description = descriptionTextBox.Text.Trim();
                    string msdsUrl = msdsUrlTextBox.Text.Trim();
                    string supplierId = supplierComboBox.SelectedItem is DataRowView ? 
                        (supplierComboBox.SelectedItem as DataRowView)["id"].ToString() : null;

                    if (string.IsNullOrEmpty(itemId))
                    {
                        // 新建物资
                        string insertQuery = @"INSERT INTO items (code, name, type, category, unit, description, 
                            msds_url, supplier_id, created_by, created_at, updated_at, is_active) 
                            VALUES (@code, @name, @type, @category, @unit, @description, 
                            @msdsUrl, @supplierId, @createdBy, NOW(), NOW(), 1)";

                        Dictionary<string, object> parameters = new Dictionary<string, object>();
                        parameters.Add("@code", itemCode);
                        parameters.Add("@name", itemName);
                        parameters.Add("@type", itemType);
                        parameters.Add("@category", category);
                        parameters.Add("@unit", unit);
                        parameters.Add("@description", description);
                        if (isChemical)
                            parameters.Add("@msdsUrl", msdsUrl);
                        else
                            parameters.Add("@msdsUrl", DBNull.Value);

                        if (string.IsNullOrEmpty(supplierId))
                            parameters.Add("@supplierId", DBNull.Value);
                        else
                            parameters.Add("@supplierId", supplierId);
                        parameters.Add("@createdBy", "当前用户"); // 实际应用中应获取当前登录用户

                        dbHelper.ExecuteNonQuery(insertQuery, parameters);

                        // 记录审计日志
                        WriteAuditLog(itemCode, "CREATE", "新建物资：" + itemName);
                    }
                    else
                    {
                        // 编辑物资
                        string updateQuery = @"UPDATE items SET name = @name, type = @type, category = @category, 
                            unit = @unit, description = @description, msds_url = @msdsUrl, 
                            supplier_id = @supplierId, updated_at = NOW() WHERE id = @itemId";

                        Dictionary<string, object> parameters = new Dictionary<string, object>();
                        parameters.Add("@name", itemName);
                        parameters.Add("@type", itemType);
                        parameters.Add("@category", category);
                        parameters.Add("@unit", unit);
                        parameters.Add("@description", description);
                        if (isChemical)
                            parameters.Add("@msdsUrl", msdsUrl);
                        else
                            parameters.Add("@msdsUrl", DBNull.Value);

                        if (string.IsNullOrEmpty(supplierId))
                            parameters.Add("@supplierId", DBNull.Value);
                        else
                            parameters.Add("@supplierId", supplierId);
                        parameters.Add("@itemId", itemId);

                        dbHelper.ExecuteNonQuery(updateQuery, parameters);

                        // 记录审计日志
                        WriteAuditLog(itemCode, "UPDATE", "编辑物资：" + itemName);
                    }

                    // 提交事务
                    dbHelper.CommitTransaction();
                    MessageBox.Show("保存成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                catch (Exception ex)
                {
                    // 回滚事务
                    dbHelper.RollbackTransaction();
                    MessageBox.Show("保存失败: " + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("操作失败: " + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(itemCodeTextBox.Text))
            {
                MessageBox.Show("请输入物资编号", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                itemCodeTextBox.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(itemNameTextBox.Text))
            {
                MessageBox.Show("请输入物资名称", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                itemNameTextBox.Focus();
                return false;
            }

            if (itemTypeComboBox.SelectedIndex == -1)
            {
                MessageBox.Show("请选择物资类型", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                itemTypeComboBox.Focus();
                return false;
            }

            if (categoryComboBox.SelectedIndex == -1)
            {
                MessageBox.Show("请选择分类", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                categoryComboBox.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(unitTextBox.Text))
            {
                MessageBox.Show("请输入单位", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                unitTextBox.Focus();
                return false;
            }

            // 合规校验：化学试剂必须填写MSDS URL
            if (isChemical && string.IsNullOrWhiteSpace(msdsUrlTextBox.Text))
            {
                MessageBox.Show("化学试剂必须填写MSDS URL", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                msdsUrlTextBox.Focus();
                return false;
            }

            // 检查物资编号是否重复
            if (string.IsNullOrEmpty(itemId) && IsCodeDuplicate(itemCodeTextBox.Text.Trim()))
            {
                MessageBox.Show("物资编号已存在", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                itemCodeTextBox.Focus();
                return false;
            }

            return true;
        }

        private bool IsCodeDuplicate(string code)
        {
            try
            {
                string query = "SELECT COUNT(*) FROM items WHERE code = @code";
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters.Add("@code", code);
                int count = Convert.ToInt32(dbHelper.ExecuteScalar(query, parameters));
                return count > 0;
            }
            catch (Exception)
            {
                return false;
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

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
