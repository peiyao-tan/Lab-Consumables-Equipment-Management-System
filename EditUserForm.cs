using MySql.Data.MySqlClient;
using System;
using System.Drawing;
using System.Security.Cryptography;
using System.Windows.Forms;
using BCrypt.Net;

namespace 实验耗材及设备物资管理系统
{
    public partial class EditUserForm : Form
    {
        private string connectionString;
        private string userId;
        private TextBox txtUsername;
        private TextBox txtFullName;
        private TextBox txtEmail;
        private TextBox txtPhone;
        private ComboBox cmbStatus;
        private string actorId;

        public EditUserForm(string connString, string id, string actorId)
        {
            connectionString = connString;
            userId = id;
            this.actorId = actorId;
            InitializeForm();
            LoadUserInfo();
        }

        private void InitializeForm()
        {
            this.Text = "编辑用户";
            this.Size = new Size(400, 350);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // 创建面板布局
            Panel mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20)
            };
            this.Controls.Add(mainPanel);

            // 用户ID（只读）
            Label lblUserId = new Label
            {
                Text = "用户ID:",
                Location = new Point(20, 10),
                Size = new Size(80, 20),
                Font = new Font("微软雅黑", 9)
            };
            mainPanel.Controls.Add(lblUserId);

            TextBox txtUserId = new TextBox
            {
                Location = new Point(110, 10),
                Size = new Size(220, 25),
                Font = new Font("微软雅黑", 9),
                ReadOnly = true,
                Text = userId
            };
            mainPanel.Controls.Add(txtUserId);

            // 用户名
            Label lblUsername = new Label
            {
                Text = "用户名:",
                Location = new Point(20, 50),
                Size = new Size(80, 20),
                Font = new Font("微软雅黑", 9)
            };
            mainPanel.Controls.Add(lblUsername);

            txtUsername = new TextBox
            {
                Location = new Point(110, 50),
                Size = new Size(220, 25),
                Font = new Font("微软雅黑", 9)
            };
            mainPanel.Controls.Add(txtUsername);

            // 姓名
            Label lblFullName = new Label
            {
                Text = "姓名:",
                Location = new Point(20, 90),
                Size = new Size(80, 20),
                Font = new Font("微软雅黑", 9)
            };
            mainPanel.Controls.Add(lblFullName);

            txtFullName = new TextBox
            {
                Location = new Point(110, 90),
                Size = new Size(220, 25),
                Font = new Font("微软雅黑", 9)
            };
            mainPanel.Controls.Add(txtFullName);

            // 邮箱
            Label lblEmail = new Label
            {
                Text = "邮箱:",
                Location = new Point(20, 130),
                Size = new Size(80, 20),
                Font = new Font("微软雅黑", 9)
            };
            mainPanel.Controls.Add(lblEmail);

            txtEmail = new TextBox
            {
                Location = new Point(110, 130),
                Size = new Size(220, 25),
                Font = new Font("微软雅黑", 9)
            };
            mainPanel.Controls.Add(txtEmail);

            // 电话
            Label lblPhone = new Label
            {
                Text = "电话:",
                Location = new Point(20, 170),
                Size = new Size(80, 20),
                Font = new Font("微软雅黑", 9)
            };
            mainPanel.Controls.Add(lblPhone);

            txtPhone = new TextBox
            {
                Location = new Point(110, 170),
                Size = new Size(220, 25),
                Font = new Font("微软雅黑", 9)
            };
            mainPanel.Controls.Add(txtPhone);

            // 状态
            Label lblStatus = new Label
            {
                Text = "状态:",
                Location = new Point(20, 210),
                Size = new Size(80, 20),
                Font = new Font("微软雅黑", 9)
            };
            mainPanel.Controls.Add(lblStatus);

            cmbStatus = new ComboBox
            {
                Location = new Point(110, 210),
                Size = new Size(220, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("微软雅黑", 9)
            };
            cmbStatus.Items.AddRange(new object[] { "启用", "禁用" });
            mainPanel.Controls.Add(cmbStatus);

            // 按钮区域
            Panel btnPanel = new Panel
            {
                Location = new Point(20, 260),
                Size = new Size(330, 40)
            };
            mainPanel.Controls.Add(btnPanel);

            // 确定按钮
            Button btnOK = new Button
            {
                Text = "确定",
                Location = new Point(80, 5),
                Size = new Size(90, 30),
                BackColor = Color.FromArgb(39, 174, 96),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 }
            };
            btnOK.Click += BtnOK_Click;
            btnPanel.Controls.Add(btnOK);

            // 取消按钮
            Button btnCancel = new Button
            {
                Text = "取消",
                Location = new Point(190, 5),
                Size = new Size(90, 30),
                BackColor = Color.FromArgb(231, 76, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 }
            };
            btnCancel.Click += BtnCancel_Click;
            btnPanel.Controls.Add(btnCancel);
        }

        // 加载用户信息到表单
        private void LoadUserInfo()
        {
            try
            {
                string query = @"
            SELECT username, full_name, email, phone, status 
            FROM users 
            WHERE id = @id";

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@id", userId);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            txtUsername.Text = reader["username"].ToString();
                            txtFullName.Text = reader["full_name"].ToString();
                            txtEmail.Text = reader["email"].ToString();
                            txtPhone.Text = reader["phone"].ToString();

                            // 修正：正确处理状态值
                            string status = reader["status"].ToString();
                            // 将数据库的 "active"/"locked" 映射到界面的 "启用"/"禁用"
                            cmbStatus.SelectedIndex = status == "active" ? 0 : 1;
                        }
                        else
                        {
                            MessageBox.Show("未找到用户信息", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            this.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载用户信息失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            // 验证输入
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                MessageBox.Show("请输入用户名", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtUsername.Focus();
                return;
            }

            try
            {
                // 准备更新数据
                string username = txtUsername.Text.Trim();
                string fullName = txtFullName.Text.Trim();
                string email = txtEmail.Text.Trim();
                string phone = txtPhone.Text.Trim();

                // 修正：使用正确的状态值映射
                string status = cmbStatus.SelectedIndex == 0 ? "active" : "locked";

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    // 检查用户名是否已存在（排除当前用户）
                    string checkQuery = @"
                SELECT COUNT(*) FROM users 
                WHERE username = @username AND id != @id";

                    MySqlCommand checkCmd = new MySqlCommand(checkQuery, connection);
                    checkCmd.Parameters.AddWithValue("@username", username);
                    checkCmd.Parameters.AddWithValue("@id", userId);
                    int exists = Convert.ToInt32(checkCmd.ExecuteScalar());

                    if (exists > 0)
                    {
                        MessageBox.Show("该用户名已存在，请更换", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // 构建更新SQL（不再包含密码字段）
                    string updateQuery = @"
                    UPDATE users SET 
                        username = @username,
                        full_name = @fullName,
                        email = @email,
                        phone = @phone,
                        status = @status,
                        updated_at = NOW()
                    WHERE id = @id";

                    MySqlCommand cmd = new MySqlCommand(updateQuery, connection);
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@fullName", fullName);
                    cmd.Parameters.AddWithValue("@email", email);
                    cmd.Parameters.AddWithValue("@phone", phone);
                    cmd.Parameters.AddWithValue("@status", status);
                    cmd.Parameters.AddWithValue("@id", userId);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("用户信息更新成功！", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LogAudit logger = new LogAudit(connectionString);
                        logger.LogAction(actorId, "UPDATE", "users", userId);
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("用户信息更新失败，请重试", "失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"更新用户信息时出错: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // EditUserForm
            // 
            this.ClientSize = new System.Drawing.Size(278, 244);
            this.Name = "EditUserForm";
            this.Load += new System.EventHandler(this.EditUserForm_Load);
            this.ResumeLayout(false);

        }

        private void EditUserForm_Load(object sender, EventArgs e)
        {

        }
    }
}