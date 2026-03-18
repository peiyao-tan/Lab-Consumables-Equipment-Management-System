using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using WindowsFormsApp1;

namespace 实验耗材及设备物资管理系统
{
    public partial class Form1 : Form
    {
        // 数据库连接字符串
        private string connectionString = "server=localhost;user=root;password=123456;database=try;SslMode=None;";

        // 控件声明
        private CheckBox checkBoxShowPassword;
        private ComboBox comboBoxRoles;
        private Panel panelContainer;
        private PictureBox pictureBoxUser;
        private PictureBox pictureBoxPassword;
        private PictureBox pictureBoxRole;

        // 颜色定义
        private Color primaryColor = Color.FromArgb(41, 128, 185);
        private Color backgroundColor = Color.FromArgb(245, 246, 250);
        private Color textColor = Color.FromArgb(52, 73, 94);

        // 角色定义字典（根据图片恢复）
        private Dictionary<string, string> roleCodes = new Dictionary<string, string>
        {
            { "系统管理员", "ADMIN" },
            { "实验管理员", "LAB_ADMIN" },
            { "仓库管理员", "WAREHOUSE_MANAGER" },
            { "实验人员", "LAB_USER" },
            { "审批人", "APPROVER" },
            { "安全合规员", "COMPLIANCE_OFFICER" }
        };

        public Form1()
        {
            InitializeComponent();
            ApplyStyling();
            InitializeCustomComponents();
            LoadRoles();
         

        }

        private void ApplyStyling()
        {
            this.Text = "用户登录系统";
            this.Size = new Size(450, 500);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = backgroundColor;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Padding = new Padding(20);
        }

        private void InitializeCustomComponents()
        {
            // 创建主容器
            panelContainer = new Panel();
            panelContainer.Size = new Size(380, 420);
            panelContainer.Location = new Point(10, 10);
            panelContainer.BackColor = Color.White;
            panelContainer.BorderStyle = BorderStyle.None;
            this.Controls.Add(panelContainer);

       

            // 添加标题
            Label titleLabel = new Label();
            titleLabel.Text = "用户登录";
            titleLabel.Font = new Font("微软雅黑", 18, FontStyle.Bold);
            titleLabel.ForeColor = primaryColor;
            titleLabel.Size = new Size(200, 40);
            titleLabel.Location = new Point(90, 30);
            titleLabel.TextAlign = ContentAlignment.MiddleCenter;
            panelContainer.Controls.Add(titleLabel);

            // 用户名图标
            pictureBoxUser = new PictureBox();
            pictureBoxUser.Size = new Size(24, 24);
            pictureBoxUser.Location = new Point(40, 100);
            pictureBoxUser.Image = CreateUserIcon();
            pictureBoxUser.SizeMode = PictureBoxSizeMode.Zoom;
            panelContainer.Controls.Add(pictureBoxUser);

            // 用户名文本框 - 使用水印提示
            textBox1.Location = new Point(80, 100);
            textBox1.Size = new Size(240, 35);
            textBox1.Font = new Font("微软雅黑", 10);
            textBox1.BorderStyle = BorderStyle.FixedSingle;
            textBox1.BackColor = Color.White;
            textBox1.ForeColor = Color.Gray;
            textBox1.Text = "请输入用户名"; // 水印提示
            textBox1.Enter += TextBox1_Enter;
            textBox1.Leave += TextBox1_Leave;
            panelContainer.Controls.Add(textBox1);

            // 密码图标
            pictureBoxPassword = new PictureBox();
            pictureBoxPassword.Size = new Size(24, 24);
            pictureBoxPassword.Location = new Point(40, 150);
            pictureBoxPassword.Image = CreatePasswordIcon();
            pictureBoxPassword.SizeMode = PictureBoxSizeMode.Zoom;
            panelContainer.Controls.Add(pictureBoxPassword);

            // 密码文本框 - 使用水印提示
            textBox2.Location = new Point(80, 150);
            textBox2.Size = new Size(240, 35);
            textBox2.Font = new Font("微软雅黑", 10);
            textBox2.PasswordChar = '\0'; // 初始不显示密码字符
            textBox2.Text = "请输入密码"; // 水印提示
            textBox2.ForeColor = Color.Gray;
            textBox2.Enter += TextBox2_Enter;
            textBox2.Leave += TextBox2_Leave;
            panelContainer.Controls.Add(textBox2);

            // 显示密码复选框
            checkBoxShowPassword = new CheckBox();
            checkBoxShowPassword.Text = "显示密码";
            checkBoxShowPassword.Location = new Point(80, 190);
            checkBoxShowPassword.Size = new Size(100, 20);
            checkBoxShowPassword.Font = new Font("微软雅黑", 9);
            checkBoxShowPassword.ForeColor = textColor;
            checkBoxShowPassword.FlatStyle = FlatStyle.Flat;
            checkBoxShowPassword.CheckedChanged += CheckBoxShowPassword_CheckedChanged;
            panelContainer.Controls.Add(checkBoxShowPassword);

            // 角色图标（根据图片恢复）
            pictureBoxRole = new PictureBox();
            pictureBoxRole.Size = new Size(24, 24);
            pictureBoxRole.Location = new Point(40, 230);
            pictureBoxRole.Image = CreateRoleIcon();
            pictureBoxRole.SizeMode = PictureBoxSizeMode.Zoom;
            panelContainer.Controls.Add(pictureBoxRole);

            // 角色下拉框（根据图片恢复）
            comboBoxRoles = new ComboBox();
            comboBoxRoles.Location = new Point(80, 230);
            comboBoxRoles.Size = new Size(240, 30);
            comboBoxRoles.Font = new Font("微软雅黑", 9);
            comboBoxRoles.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxRoles.FlatStyle = FlatStyle.Flat;
            panelContainer.Controls.Add(comboBoxRoles);

            // 登录按钮
            button1.Text = "登录";
            button1.Location = new Point(110, 280);
            button1.Size = new Size(120, 40);
            button1.Font = new Font("微软雅黑", 11, FontStyle.Bold);
            button1.BackColor = primaryColor;
            button1.ForeColor = Color.White;
            button1.FlatStyle = FlatStyle.Flat;
            button1.FlatAppearance.BorderSize = 0;
            panelContainer.Controls.Add(button1);

            // 底部版权信息（根据图片更新）
            Label footerLabel = new Label();
            footerLabel.Text = "© 2025 实验耗材及设备物资管理系统 | 版本 1.0";
            footerLabel.Font = new Font("微软雅黑", 8);
            footerLabel.ForeColor = Color.Gray;
            footerLabel.Size = new Size(280, 20);
            footerLabel.Location = new Point(50, 340);
            footerLabel.TextAlign = ContentAlignment.MiddleCenter;
            panelContainer.Controls.Add(footerLabel);
        }



        // 用户名文本框获得焦点事件
        private void TextBox1_Enter(object sender, EventArgs e)
        {
            if (textBox1.Text == "请输入用户名")
            {
                textBox1.Text = "";
                textBox1.ForeColor = textColor;
            }
        }

        // 用户名文本框失去焦点事件
        private void TextBox1_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                textBox1.Text = "请输入用户名";
                textBox1.ForeColor = Color.Gray;
            }
        }

        // 密码文本框获得焦点事件
        private void TextBox2_Enter(object sender, EventArgs e)
        {
            if (textBox2.Text == "请输入密码")
            {
                textBox2.Text = "";
                textBox2.ForeColor = textColor;
                textBox2.PasswordChar = '●';
            }
        }

        // 密码文本框失去焦点事件
        private void TextBox2_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox2.Text))
            {
                textBox2.Text = "请输入密码";
                textBox2.ForeColor = Color.Gray;
                textBox2.PasswordChar = '\0';
            }
        }

        // 显示/隐藏密码切换
        private void CheckBoxShowPassword_CheckedChanged(object sender, EventArgs e)
        {
            if (textBox2.Text != "请输入密码")
            {
                textBox2.PasswordChar = checkBoxShowPassword.Checked ? '\0' : '●';
            }
        }

        // 创建用户图标
        private Bitmap CreateUserIcon()
        {
            Bitmap bmp = new Bitmap(24, 24);
            using (Graphics g = Graphics.FromImage(bmp))
            using (SolidBrush brush = new SolidBrush(primaryColor))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.FillEllipse(brush, 2, 2, 20, 20);
            }
            return bmp;
        }

        // 创建密码图标
        private Bitmap CreatePasswordIcon()
        {
            Bitmap bmp = new Bitmap(24, 24);
            using (Graphics g = Graphics.FromImage(bmp))
            using (SolidBrush brush = new SolidBrush(primaryColor))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.FillRectangle(brush, 7, 10, 10, 10);
                g.FillEllipse(brush, 9, 6, 6, 6);
            }
            return bmp;
        }

        // 创建角色图标（根据图片恢复）
        private Bitmap CreateRoleIcon()
        {
            Bitmap bmp = new Bitmap(24, 24);
            using (Graphics g = Graphics.FromImage(bmp))
            using (SolidBrush brush = new SolidBrush(primaryColor))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.FillEllipse(brush, 8, 4, 8, 8);
                g.FillRectangle(brush, 10, 12, 4, 8);
            }
            return bmp;
        }

        // 加载角色列表（根据图片恢复）
        private void LoadRoles()
        {
            try
            {
                comboBoxRoles.Items.Clear();
                comboBoxRoles.Items.Add("系统管理员");
                comboBoxRoles.Items.Add("实验管理员");
                comboBoxRoles.Items.Add("仓库管理员");
                comboBoxRoles.Items.Add("实验人员");
                comboBoxRoles.Items.Add("审批人");
                comboBoxRoles.Items.Add("安全合规员");

                if (comboBoxRoles.Items.Count > 0)
                {
                    comboBoxRoles.SelectedIndex = 0; // 默认选择第一个角色
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载角色列表失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 登录按钮点击事件


        private void button1_Click(object sender, EventArgs e)
        {
            string username = textBox1.Text.Trim();
            string password = textBox2.Text.Trim();
            string selectedRole = comboBoxRoles.SelectedItem?.ToString();

            // 详细的字符级调试
            Console.WriteLine($"=== 登录尝试 ===");
            Console.WriteLine($"输入用户名: '{username}'");
            Console.WriteLine($"用户名长度: {username.Length}");
            Console.WriteLine($"输入密码: '{password}'");
            Console.WriteLine($"密码长度: {password.Length}");

            // 显示每个字符的ASCII码
            Console.WriteLine($"用户名ASCII: {string.Join(",", username.Select(c => (int)c))}");
            Console.WriteLine($"密码ASCII: {string.Join(",", password.Select(c => (int)c))}");

            // 检查是否为水印文本
            if (username == "请输入用户名" || password == "请输入密码")
            {
                MessageBox.Show("请输入有效的用户名和密码！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 使用新方法登录并获取用户信息
            var (success, userId, dbUsername) = LoginAndGetUserInfo(username, password);

            Console.WriteLine($"登录结果 - 成功: {success}, 用户ID: {userId}, 用户名: {dbUsername}");

            if (success)
            {
                // 设置当前用户
                CurrentUser.Login(userId, dbUsername);

                // 更新最后登录时间
                UpdateLastLoginTime(dbUsername);

                // 验证角色匹配
                var userRoles = GetUserRoles(dbUsername);
                Console.WriteLine($"用户角色: {string.Join(", ", userRoles)}");

                if (!userRoles.Contains(selectedRole))
                {
                    string availableRoles = string.Join("、", userRoles);
                    MessageBox.Show($"角色不匹配！\n您可登录的角色为：{availableRoles}\n请选择正确的角色。",
                        "角色验证失败", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // 打开对应角色界面
                OpenRoleSpecificForm(dbUsername, selectedRole);
                this.Hide();
            }
            else
            {
                MessageBox.Show("用户名或密码错误！", "登录失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 验证用户登录
        private (bool success, string userId, string username) LoginAndGetUserInfo(string inputUsername, string inputPassword)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    // 首先检查用户是否存在（不限制状态）
                    string checkUserSql = @"SELECT u.id, u.username, u.password_hash, u.status
                           FROM users u 
                           WHERE u.username = @username";

                    MySqlCommand checkCmd = new MySqlCommand(checkUserSql, conn);
                    checkCmd.Parameters.AddWithValue("@username", inputUsername);

                    using (MySqlDataReader reader = checkCmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string userId = reader["id"].ToString();
                            string username = reader["username"].ToString();
                            string status = reader["status"].ToString();
                            string storedHash = reader["password_hash"].ToString();

                            Console.WriteLine($"=== 用户存在 ===");
                            Console.WriteLine($"用户ID: {userId}");
                            Console.WriteLine($"用户名: {username}");
                            Console.WriteLine($"状态: {status}");
                            Console.WriteLine($"哈希长度: {storedHash.Length}");
                            Console.WriteLine($"哈希内容: {storedHash}");

                            reader.Close(); // 先关闭reader

                            // 检查状态是否为 active
                            if (status != "active")
                            {
                                Console.WriteLine($"用户状态不是 active，当前状态: {status}");
                                return (false, null, null);
                            }

                            // 验证密码
                            Console.WriteLine($"开始验证密码...");
                            bool isPasswordValid = false;
                            try
                            {
                                isPasswordValid = BCrypt.Net.BCrypt.Verify(inputPassword, storedHash);
                                Console.WriteLine($"BCrypt 验证结果: {isPasswordValid}");
                            }
                            catch (Exception bcryptEx)
                            {
                                Console.WriteLine($"BCrypt 验证异常: {bcryptEx.Message}");
                                Console.WriteLine($"异常类型: {bcryptEx.GetType()}");
                            }

                            return (isPasswordValid, userId, username);
                        }
                        else
                        {
                            Console.WriteLine($"用户 '{inputUsername}' 在数据库中不存在");
                            return (false, null, null);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"数据库异常: {ex.Message}");
                    Console.WriteLine($"异常类型: {ex.GetType()}");
                    MessageBox.Show($"登录出错：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return (false, null, null);
                }
            }
        }

        // 获取用户角色
        private List<string> GetUserRoles(string username)
        {
            var roles = new List<string>();
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string sql = @"SELECT r.name FROM users u 
                           JOIN user_roles ur ON u.id = ur.user_id 
                           JOIN roles r ON ur.role_id = r.id 
                           WHERE u.username = @username";

                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@username", username);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            roles.Add(reader["name"].ToString());
                        }
                    }
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show($"获取用户角色失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            return roles;
        }

        // 根据角色打开对应的功能界面
        private void OpenRoleSpecificForm(string username, string userRole)
        {
            switch (userRole)
            {
                case "系统管理员":
                    MessageBox.Show($"欢迎回来，{username}（系统管理员）！\n\n系统管理员功能：\n- 系统配置\n- 用户与角色\n- 权限策略",
                        "登录成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    系统管理员 systemAdminForm = new 系统管理员(username);
                    systemAdminForm.Show();
                    break;

                case "实验管理员":
                    MessageBox.Show($"欢迎回来，{username}（实验管理员）！\n\n实验管理员功能：\n- 物资与设备台账管理\n- 库存与库位管理\n- 审批配置\n- 报表生成",
                        "登录成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    实验管理员 labAdminForm = new 实验管理员(username);
                    labAdminForm.Show();
                    break;

                case "仓库管理员":
                    MessageBox.Show($"欢迎回来，{username}（仓库管理员）！\n\n仓库管理员功能：\n- 入库管理\n- 出库管理\n- 移库操作\n- 盘点管理",
                        "登录成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    仓库管理员 warehouseForm = new 仓库管理员(username);
                    warehouseForm.Show();
                    break;

                case "实验人员":
                    MessageBox.Show($"欢迎回来，{username}（实验人员）！\n\n实验人员功能：\n- 提交借用申请\n- 提交预约申请\n- 提交领用申请\n- 执行归还操作",
                        "登录成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    实验人员 labUserForm = new 实验人员(username);
                    labUserForm.Show();
                    break;

                case "审批人":
                    MessageBox.Show($"欢迎回来，{username}（审批人）！\n\n审批人功能：\n- 审批采购申请\n- 审批借用申请\n- 审批预约申请\n- 审批维护申请",
                        "登录成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    // 实例化审批人窗口（使用之前写的“实验耗材审批人”类，传入当前用户名）
                    审批人 approverForm = new 审批人(username);
                    approverForm.Show(); // 显示审批人窗口
                    break;

                case "安全合规员":
                    MessageBox.Show($"欢迎回来，{username}（安全合规员）！\n\n安全合规员功能：\n- 危化品管理\n- 检定校准合规\n- 审计追踪\n- 合规检查",
         "登录成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    // 显示安全合规员窗口
                    安全合规员 complianceForm = new 安全合规员(username);
                    complianceForm.Show();
                    break;

                default:
                    MessageBox.Show($"欢迎回来，{username}！", "登录成功",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
            }

            // 隐藏登录窗体（可选）
            this.Hide();

        }

        private void UpdateLastLoginTime(string username)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string sql = "UPDATE users SET last_login_at = @loginTime, updated_at = @loginTime WHERE username = @username";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@loginTime",
                        DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.ExecuteNonQuery();
                }
                catch (MySqlException ex)
                {
                    System.Diagnostics.Debug.WriteLine($"更新登录时间失败: {ex.Message}");
                }
            }
        }

        // 空的事件处理方法
        private void label1_Click(object sender, EventArgs e) { }
        private void textBox1_TextChanged(object sender, EventArgs e) { }
        private void textBox2_TextChanged(object sender, EventArgs e) { }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}