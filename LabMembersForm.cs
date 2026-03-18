using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using 实验耗材及设备物资管理系统;

namespace WindowsFormsApp1
{
    public partial class LabMembersForm : Form
    {
        private DatabaseHelper dbHelper;
        private string labId;
        private string labName;
        private string labCode; // 新增：显示实验室编码

        // 构造函数：新增labCode参数（从LabListForm传递）
        public LabMembersForm(DatabaseHelper helper, string labId, string labName, string labCode)
        {
            InitializeComponent();
            dbHelper = helper;
            this.labId = labId;
            this.labName = labName;
            this.labCode = labCode;
            this.Text = $"实验室成员管理 - {labName}（编码：{labCode}）";

            // 绑定Designer中“添加成员”按钮的点击事件
            this.btnAddMember.Click += BtnAddMember_Click;
            // 绑定“返回”按钮事件
            this.btnBack.Click += (s, e) => this.Close();

            // 显示当前实验室信息
            this.lblLabName.Text = $"当前实验室：{labName}（编码：{labCode}）";
            LoadLabMembers();
        }

        // 加载实验室成员
        private void LoadLabMembers()
        {
            try
            {
                string query = @"
            SELECT 
                u.id, 
                u.username, 
                u.full_name, 
                lm.role_in_lab AS lab_role                     
            FROM users u
            JOIN lab_memberships lm 
                ON u.id = lm.user_id
            WHERE lm.lab_id = @labId AND lm.active = true"; // 确保查询活跃成员

                var parameters = new Dictionary<string, object> { { "@labId", labId } };
                DataTable dt = dbHelper.ExecuteQuery(query, parameters);

                // 绑定到Designer中的dataGridView1
                this.dataGridView1.DataSource = dt;

                // 设置所有列的中文显示
                if (this.dataGridView1.Columns.Count > 0)
                {
                    this.dataGridView1.Columns["username"].HeaderText = "用户名";
                    this.dataGridView1.Columns["full_name"].HeaderText = "姓名";
                    this.dataGridView1.Columns["lab_role"].HeaderText = "实验室内角色";

                    // 隐藏id列（通常不需要显示给用户）
                    this.dataGridView1.Columns["id"].Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载成员数据失败: {ex.Message}", "错误");
            }
        }

        // 打开添加成员窗体（保持逻辑，传递labId）
        private void BtnAddMember_Click(object sender, EventArgs e)
        {
            var addForm = new AddMemberForm(dbHelper, labId);
            if (addForm.ShowDialog() == DialogResult.OK)
            {
                LoadLabMembers(); // 刷新成员列表
            }
        }
    }
}