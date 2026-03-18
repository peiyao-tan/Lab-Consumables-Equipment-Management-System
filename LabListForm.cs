using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using 实验耗材及设备物资管理系统;

namespace WindowsFormsApp1
{
    public partial class LabListForm : UserControl
    {
        private DatabaseHelper dbHelper;
        private DataTable labTable;

        // 构造函数：接收数据库助手
        public LabListForm(DatabaseHelper helper)
        {
            InitializeComponent();
            dbHelper = helper;

            // 绑定“新建实验室”和“查看成员”按钮事件（核心：解决按钮无响应）
            this.btnNewLab.Click += BtnNewLab_Click;
            this.btnViewMembers.Click += BtnViewMembers_Click;

            // 初始化表格并加载数据
            InitializeLabGrid();
            LoadLabData();
        }

        // 初始化表格（复用原逻辑，绑定到Designer的dgvLabs）
        private void InitializeLabGrid()
        {
            // 直接使用Designer中的dgvLabs，无需重新创建
            this.dgvLabs.ReadOnly = true;
            this.dgvLabs.AllowUserToAddRows = false;
            this.dgvLabs.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvLabs.DoubleClick += DgvLabs_DoubleClick; // 保留双击事件
        }

        // 加载实验室数据（修正：新增code字段，匹配labs表结构）
        private void LoadLabData()
        {
            try
            {
                string query = @"
                    SELECT 
                        id, 
                        code, 
                        name, 
                        department_id, 
                        location_desc 
                    FROM labs 
                    ";

                labTable = dbHelper.ExecuteQuery(query);
                this.dgvLabs.DataSource = labTable;

                // 设置列名显示
                this.dgvLabs.Columns["id"].Visible = false; // 隐藏UUID主键
                this.dgvLabs.Columns["code"].HeaderText = "实验室编码";
                this.dgvLabs.Columns["name"].HeaderText = "实验室名称";
                this.dgvLabs.Columns["department_id"].HeaderText = "所属部门ID";
                this.dgvLabs.Columns["location_desc"].HeaderText = "物理位置";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载实验室数据失败: {ex.Message}", "错误");
            }
        }

        // 1. “新建实验室”按钮事件
        private void BtnNewLab_Click(object sender, EventArgs e)
        {
            // 新建实验室弹窗：输入编码、名称、部门ID、位置
            var newLabForm = new NewLabForm(dbHelper);
            if (newLabForm.ShowDialog() == DialogResult.OK)
            {
                LoadLabData(); // 刷新实验室列表
            }
        }

        // 2. “查看成员”按钮事件（与双击逻辑一致）
        private void BtnViewMembers_Click(object sender, EventArgs e)
        {
            OpenLabMembersForm();
        }

        // 3. 双击表格打开成员管理（复用逻辑）
        private void DgvLabs_DoubleClick(object sender, EventArgs e)
        {
            OpenLabMembersForm();
        }

        // 通用：打开成员管理窗体（提取重复逻辑）
        private void OpenLabMembersForm()
        {
            if (this.dgvLabs.SelectedRows.Count == 0)
            {
                MessageBox.Show("请先选择一个实验室", "提示");
                return;
            }

            var selectedRow = this.dgvLabs.SelectedRows[0];
            string labId = selectedRow.Cells["id"].Value.ToString();
            string labName = selectedRow.Cells["name"].Value.ToString();
            string labCode = selectedRow.Cells["code"].Value.ToString(); // 传递编码

            // 打开成员管理窗体（传递labId、labName、labCode）
            var memberForm = new LabMembersForm(dbHelper, labId, labName, labCode);
            memberForm.ShowDialog();
        }
    }

    // 新增：新建实验室弹窗（NewLabForm）
    public class NewLabForm : Form
    {
        private DatabaseHelper dbHelper;
        private TextBox txtCode;
        private TextBox txtName;
        private TextBox txtDeptId;
        private TextBox txtLocation;
        private Button btnSave;
        private Button btnCancel;

        public NewLabForm(DatabaseHelper helper)
        {
            dbHelper = helper;
            InitializeComponent();
            this.Text = "新建实验室";
            this.Size = new Size(400, 300);
        }

        private void InitializeComponent()
        {
            // 布局控件
            var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 5 };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70));

            // 添加控件
            layout.Controls.Add(new Label { Text = "实验室编码*" }, 0, 0);
            txtCode = new TextBox { Dock = DockStyle.Fill };
            layout.Controls.Add(txtCode, 1, 0);

            layout.Controls.Add(new Label { Text = "实验室名称*" }, 0, 1);
            txtName = new TextBox { Dock = DockStyle.Fill };
            layout.Controls.Add(txtName, 1, 1);

            layout.Controls.Add(new Label { Text = "所属部门ID*" }, 0, 2);
            txtDeptId = new TextBox { Dock = DockStyle.Fill };
            layout.Controls.Add(txtDeptId, 1, 2);

            layout.Controls.Add(new Label { Text = "物理位置" }, 0, 3);
            txtLocation = new TextBox { Dock = DockStyle.Fill };
            layout.Controls.Add(txtLocation, 1, 3);

            // 按钮行
            var btnPanel = new Panel { Dock = DockStyle.Fill };
            btnSave = new Button { Text = "保存", Left = 100, Top = 10, Width = 75 };
            btnSave.Click += BtnSave_Click;
            btnCancel = new Button { Text = "取消", Left = 200, Top = 10, Width = 75 };
            btnCancel.Click += (s, e) => this.Close();
            btnPanel.Controls.Add(btnSave);
            btnPanel.Controls.Add(btnCancel);
            layout.Controls.Add(btnPanel, 0, 4);
            layout.SetColumnSpan(btnPanel, 2);

            this.Controls.Add(layout);
        }

        // 保存新建实验室（生成UUID主键，校验编码唯一性）
        private void BtnSave_Click(object sender, EventArgs e)
        {
            // 校验必填字段
            if (string.IsNullOrEmpty(txtCode.Text.Trim()))
            {
                MessageBox.Show("请输入实验室编码", "提示");
                return;
            }
            if (string.IsNullOrEmpty(txtName.Text.Trim()))
            {
                MessageBox.Show("请输入实验室名称", "提示");
                return;
            }
            if (string.IsNullOrEmpty(txtDeptId.Text.Trim()))
            {
                MessageBox.Show("请输入所属部门ID", "提示");
                return;
            }

            try
            {
                // 1. 校验编码唯一性
                string checkCodeQuery = "SELECT COUNT(*) FROM labs WHERE code = @code";
                var codeParam = new Dictionary<string, object> { { "@code", txtCode.Text.Trim() } };
                int codeCount = Convert.ToInt32(dbHelper.ExecuteScalar(checkCodeQuery, codeParam));
                if (codeCount > 0)
                {
                    MessageBox.Show("实验室编码已存在，请修改", "错误");
                    return;
                }

                // 2. 插入实验室数据（id为UUID）
                string labId = Guid.NewGuid().ToString(); // 生成UUID
                string insertQuery = @"
                    INSERT INTO labs (id, code, name, department_id, location_desc)
                    VALUES (@id, @code, @name, @deptId, @location)";

                var parameters = new Dictionary<string, object>
                {
                    { "@id", labId },
                    { "@code", txtCode.Text.Trim() },
                    { "@name", txtName.Text.Trim() },
                    { "@deptId", txtDeptId.Text.Trim() },
                    { "@location", txtLocation.Text.Trim() ?? "" }
                };

                dbHelper.ExecuteNonQuery(insertQuery, parameters);
                MessageBox.Show("实验室创建成功", "成功");
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"创建失败: {ex.Message}", "错误");
            }
        }
    }
}