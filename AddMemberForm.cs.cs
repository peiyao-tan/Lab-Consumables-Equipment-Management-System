using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using 实验耗材及设备物资管理系统;

namespace WindowsFormsApp1
{
    public partial class AddMemberForm : Form
    {
        private DatabaseHelper dbHelper;
        private string labId;

        public AddMemberForm(DatabaseHelper helper, string labId)
        {
            InitializeComponent();
            this.dbHelper = helper;
            this.labId = labId;
            // 初始化角色下拉框（匹配表6的enum值：member/manager/owner）
            cboRole.Items.AddRange(new[] { "member", "manager", "owner" });
            cboRole.SelectedIndex = 0;
            LoadAvailableUsers();
        }

        private void LoadAvailableUsers()
        {
            try
            {
                string query = @"
            SELECT u.id, u.username, u.full_name 
            FROM users u
            LEFT JOIN lab_memberships lm ON u.id = lm.user_id AND lm.lab_id = @labId AND lm.active = true
            WHERE u.status = 'active' 
            AND lm.user_id IS NULL";

                var parameters = new Dictionary<string, object> { { "@labId", labId } };
                DataTable dt = dbHelper.ExecuteQuery(query, parameters);

                // 检查是否有数据
                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("没有可添加的用户，所有用户都已加入该实验室", "提示");
                    cboUsers.DataSource = null;
                    return;
                }

                cboUsers.DataSource = dt;
                cboUsers.DisplayMember = "username";
                cboUsers.ValueMember = "id";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载用户列表失败: {ex.Message}", "错误");
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (cboUsers.SelectedItem == null || cboUsers.SelectedValue == null)
            {
                MessageBox.Show("请选择要添加的用户", "提示");
                return;
            }

            if (string.IsNullOrEmpty(cboRole.Text))
            {
                MessageBox.Show("请选择实验室内角色", "提示");
                return;
            }

            try
            {
                string userId = cboUsers.SelectedValue.ToString();
                string roleInLab = cboRole.Text;
                string joinDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                string query = @"
            INSERT INTO lab_memberships (lab_id, user_id, role_in_lab, active)
            VALUES (@labId, @userId, @roleInLab, true)";

                var parameters = new Dictionary<string, object>
        {
            { "@labId", labId },
            { "@userId", userId },
            { "@roleInLab", roleInLab }
        };

                int rowsAffected = dbHelper.ExecuteNonQuery(query, parameters);

                if (rowsAffected > 0)
                {
                    MessageBox.Show("成员添加成功", "成功");
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("添加失败，请重试", "错误");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"添加失败: {ex.Message}", "错误");
            }
        }

        // “取消”按钮点击事件
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}