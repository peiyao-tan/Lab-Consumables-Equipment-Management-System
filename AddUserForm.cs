using MySql.Data.MySqlClient;
using System;
using System.Drawing;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using BCrypt.Net;

namespace 实验耗材及设备物资管理系统
{
    public partial class AddUserForm : Form
    {
        private string connectionString;
        private TextBox txtUsername;
        private TextBox txtPassword;       // 新增密码输入框
        private TextBox txtConfirmPassword;// 新增确认密码输入框
        private TextBox txtFullName;
        private TextBox txtEmail;
        private TextBox txtPhone;
        private ComboBox cmbStatus;
        private string actorId;

        /// <summary>
        /// 获取新创建用户的ID（用于审计日志）
        /// </summary>
        public string NewlyCreatedUserId { get; private set; }

        // 构造函数，接收数据库连接字符串
        public AddUserForm(string connString, string actorId)
        {

            connectionString = connString;
            this.actorId = actorId;
            InitializeForm();
        }

        private void InitializeForm()
        {
            this.Text = "添加新用户";
            this.Size = new Size(400, 430);  // 增加窗口高度以容纳新字段
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

            // 用户名
            Label lblUsername = new Label
            {
                Text = "用户名:",
                Location = new Point(20, 20),
                Size = new Size(80, 20),
                Font = new Font("微软雅黑", 9)
            };
            mainPanel.Controls.Add(lblUsername);

            txtUsername = new TextBox
            {
                Location = new Point(110, 20),
                Size = new Size(220, 25),
                Font = new Font("微软雅黑", 9)
            };
            mainPanel.Controls.Add(txtUsername);

            // 密码（新增）
            Label lblPassword = new Label
            {
                Text = "密码:",
                Location = new Point(20, 60),
                Size = new Size(80, 20),
                Font = new Font("微软雅黑", 9)
            };
            mainPanel.Controls.Add(lblPassword);

            txtPassword = new TextBox
            {
                Location = new Point(110, 60),
                Size = new Size(220, 25),
                Font = new Font("微软雅黑", 9),
                PasswordChar = '*'  // 密码显示为*
            };
            mainPanel.Controls.Add(txtPassword);

            // 确认密码（新增）
            Label lblConfirmPassword = new Label
            {
                Text = "确认密码:",
                Location = new Point(20, 100),
                Size = new Size(80, 20),
                Font = new Font("微软雅黑", 9)
            };
            mainPanel.Controls.Add(lblConfirmPassword);

            txtConfirmPassword = new TextBox
            {
                Location = new Point(110, 100),
                Size = new Size(220, 25),
                Font = new Font("微软雅黑", 9),
                PasswordChar = '*'  // 密码显示为*
            };
            mainPanel.Controls.Add(txtConfirmPassword);

            // 姓名
            Label lblFullName = new Label
            {
                Text = "姓名:",
                Location = new Point(20, 140),
                Size = new Size(80, 20),
                Font = new Font("微软雅黑", 9)
            };
            mainPanel.Controls.Add(lblFullName);

            txtFullName = new TextBox
            {
                Location = new Point(110, 140),
                Size = new Size(220, 25),
                Font = new Font("微软雅黑", 9)
            };
            mainPanel.Controls.Add(txtFullName);

            // 邮箱
            Label lblEmail = new Label
            {
                Text = "邮箱:",
                Location = new Point(20, 180),
                Size = new Size(80, 20),
                Font = new Font("微软雅黑", 9)
            };
            mainPanel.Controls.Add(lblEmail);

            txtEmail = new TextBox
            {
                Location = new Point(110, 180),
                Size = new Size(220, 25),
                Font = new Font("微软雅黑", 9)
            };
            mainPanel.Controls.Add(txtEmail);

            // 电话
            Label lblPhone = new Label
            {
                Text = "电话:",
                Location = new Point(20, 220),
                Size = new Size(80, 20),
                Font = new Font("微软雅黑", 9)
            };
            mainPanel.Controls.Add(lblPhone);

            txtPhone = new TextBox
            {
                Location = new Point(110, 220),
                Size = new Size(220, 25),
                Font = new Font("微软雅黑", 9)
            };
            mainPanel.Controls.Add(txtPhone);

            // 状态
            Label lblStatus = new Label
            {
                Text = "状态:",
                Location = new Point(20, 260),
                Size = new Size(80, 20),
                Font = new Font("微软雅黑", 9)
            };
            mainPanel.Controls.Add(lblStatus);

            cmbStatus = new ComboBox
            {
                Location = new Point(110, 260),
                Size = new Size(220, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("微软雅黑", 9)
            };
            cmbStatus.Items.AddRange(new object[] { "启用", "禁用" });
            cmbStatus.SelectedIndex = 0; // 默认启用
            mainPanel.Controls.Add(cmbStatus);

            // 按钮区域（调整位置）
            Panel btnPanel = new Panel
            {
                Location = new Point(20, 320),
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

        // 密码哈希方法 - 使用 BCrypt
        // 密码哈希方法 - 使用 BCrypt
        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
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

            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("请输入密码", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPassword.Focus();
                return;
            }

            if (txtPassword.Text != txtConfirmPassword.Text)
            {
                MessageBox.Show("两次输入的密码不一致", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtConfirmPassword.Focus();
                return;
            }

            // 密码强度验证
            if (txtPassword.Text.Length < 6)
            {
                MessageBox.Show("密码长度不能少于6位", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPassword.Focus();
                return;
            }

            try
            {
                // 准备插入数据
                string username = txtUsername.Text.Trim();
                string passwordHash = HashPassword(txtPassword.Text);
                string fullName = txtFullName.Text.Trim();
                string email = txtEmail.Text.Trim();
                string phone = txtPhone.Text.Trim();
                int status = cmbStatus.SelectedIndex == 0 ? 1 : 0;

                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    // 检查用户名是否已存在
                    string checkQuery = "SELECT COUNT(*) FROM users WHERE username = @username";
                    MySqlCommand checkCmd = new MySqlCommand(checkQuery, conn);
                    checkCmd.Parameters.AddWithValue("@username", username);
                    int exists = Convert.ToInt32(checkCmd.ExecuteScalar());

                    if (exists > 0)
                    {
                        MessageBox.Show("该用户名已存在，请更换", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // 插入新用户
                    string insertQuery = @"
                INSERT INTO users (username, password_hash, full_name, email, phone, status, created_at)
                VALUES (@username, @passwordHash, @fullName, @email, @phone, @status, NOW())";
                    MySqlCommand cmd = new MySqlCommand(insertQuery, conn);
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@passwordHash", passwordHash);
                    cmd.Parameters.AddWithValue("@fullName", fullName);
                    cmd.Parameters.AddWithValue("@email", email);
                    cmd.Parameters.AddWithValue("@phone", phone);
                    cmd.Parameters.AddWithValue("@status", status);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        // 通过用户名查询获取新用户的ID
                        string getUserIdQuery = "SELECT id FROM users WHERE username = @username";
                        MySqlCommand getUserIdCmd = new MySqlCommand(getUserIdQuery, conn);
                        getUserIdCmd.Parameters.AddWithValue("@username", username);
                        object userIdResult = getUserIdCmd.ExecuteScalar();

                        if (userIdResult != null && userIdResult != DBNull.Value)
                        {
                            string newUserId = userIdResult.ToString();
                            NewlyCreatedUserId = newUserId;

                            // 记录审计日志
                            LogAudit logger = new LogAudit(connectionString);
                            logger.LogAction(actorId, "CREATE", "users", newUserId);

                            MessageBox.Show("用户添加成功！", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            this.DialogResult = DialogResult.OK;
                            this.Close();
                        }
                        else
                        {
                            throw new Exception("无法获取新用户ID");
                        }
                    }
                    else
                    {
                        throw new Exception("用户插入失败，影响行数为0");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"添加用户时出错: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            // AddUserForm
            // 
            this.ClientSize = new System.Drawing.Size(278, 244);
            this.Name = "AddUserForm";
            this.Load += new System.EventHandler(this.AddUserForm_Load);
            this.ResumeLayout(false);

        }

        private void AddUserForm_Load(object sender, EventArgs e)
        {

        }
    }


}
