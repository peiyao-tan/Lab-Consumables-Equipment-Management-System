using MySql.Data.MySqlClient;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace 实验耗材及设备物资管理系统
{
    public partial class DeleteUserForm : Form
    {
        private string connectionString;
        private string userId;
        private string username;
        private string fullName;
        private string actorId;

        // 颜色定义，保持与主界面一致
        private Color primaryColor = Color.FromArgb(41, 128, 185);
        private Color warningColor = Color.FromArgb(231, 76, 60);
        private Color successColor = Color.FromArgb(39, 174, 96);
        private Color textColor = Color.FromArgb(52, 73, 94);

        public DeleteUserForm(string connString, string id, string uname, string fname, string actorId)
        {
            connectionString = connString;
            userId = id;
            this.actorId = actorId;
            username = uname;
            fullName = fname;

            InitializeComponents();
            this.Text = "删除用户确认";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Size = new Size(400, 300);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
        }

        private void InitializeComponents()
        {
            this.SuspendLayout();

            // 创建警告图标
            PictureBox warningIcon = new PictureBox
            {
                Image = SystemIcons.Warning.ToBitmap(),
                Size = new Size(48, 48),
                Location = new Point(30, 30),
                SizeMode = PictureBoxSizeMode.StretchImage
            };
            this.Controls.Add(warningIcon);

            // 创建提示文本 - 更新说明文字，反映locked状态
            Label confirmLabel = new Label
            {
                Text = $"您确定要删除用户【{fullName}（{username}）】吗？\n\n" +
                       "注意：\n" +
                       "1. 如果该用户存在审计记录，将被锁定(locked)而非删除\n" +
                       "2. 锁定的用户可以在后续重新启用\n" +
                       "3. 无审计记录的用户将被永久删除",
                Font = new Font("微软雅黑", 9),
                ForeColor = textColor,
                Location = new Point(90, 30),
                Size = new Size(270, 150),
                TextAlign = ContentAlignment.TopLeft,
                AutoSize = false
            };
            this.Controls.Add(confirmLabel);

            // 创建用户ID显示
            Label idLabel = new Label
            {
                Text = $"用户ID: {userId}",
                Font = new Font("微软雅黑", 8.5f),
                ForeColor = Color.Gray,
                Location = new Point(30, 200),
                Size = new Size(200, 20),
                TextAlign = ContentAlignment.MiddleLeft
            };
            this.Controls.Add(idLabel);

            // 创建按钮面板
            Panel buttonPanel = new Panel
            {
                Location = new Point(30, 230),
                Size = new Size(340, 40),
                BackColor = Color.Transparent
            };
            this.Controls.Add(buttonPanel);

            // 取消按钮
            Button cancelButton = new Button
            {
                Text = "取消",
                Location = new Point(230, 5),
                Size = new Size(90, 30),
                BackColor = SystemColors.Control,
                ForeColor = textColor,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 1 }
            };
            cancelButton.Click += CancelButton_Click;
            buttonPanel.Controls.Add(cancelButton);

            // 确认按钮
            Button confirmButton = new Button
            {
                Text = "确认删除",
                Location = new Point(130, 5),
                Size = new Size(90, 30),
                BackColor = warningColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 }
            };
            confirmButton.Click += ConfirmButton_Click;
            buttonPanel.Controls.Add(confirmButton);

            this.ResumeLayout(false);
        }

        private void ConfirmButton_Click(object sender, EventArgs e)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    using (MySqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // 扩展的检查：检查该用户在所有相关表中是否存在记录
                            string checkRelatedRecordsSql = @"
                        SELECT (
                            -- 1. 审批记录：用户作为审批人
                            (SELECT COUNT(*) FROM approvals WHERE approver_id = @userId) +
                            -- 2. 审计日志：用户作为操作者
                            (SELECT COUNT(*) FROM audit_logs WHERE actor_id = @userId) +
                            -- 3. 部门管理：用户作为部门经理
                            (SELECT COUNT(*) FROM departments WHERE manager_id = @userId) +
                            -- 4. 实验室成员：用户作为实验室成员
                            (SELECT COUNT(*) FROM lab_memberships WHERE user_id = @userId) +
                            -- 5. 实验室管理：用户作为实验室管理员
                            (SELECT COUNT(*) FROM labs WHERE manager_id = @userId) +
                            -- 6. 采购订单：用户作为申请人
                            (SELECT COUNT(*) FROM purchase_orders WHERE requester_id = @userId) +
                            -- 7. 库存交易：用户作为操作员
                            (SELECT COUNT(*) FROM stock_transactions WHERE operator_id = @userId) +
                            -- 8. 用户角色：用户拥有的角色
                            (SELECT COUNT(*) FROM user_roles WHERE user_id = @userId) +
                            -- 9. 用户角色分配：用户分配的角色（assigned_by）
                            (SELECT COUNT(*) FROM user_roles WHERE assigned_by = @userId) +
                            -- 10. 审计日志：用户作为被操作对象（额外保护）
                            (SELECT COUNT(*) FROM audit_logs WHERE object_type = 'users' AND object_id = @userId)
                        ) as total_related_records";

                            MySqlCommand checkCmd = new MySqlCommand(checkRelatedRecordsSql, connection, transaction);
                            checkCmd.Parameters.AddWithValue("@userId", userId);
                            int relatedRecordsCount = Convert.ToInt32(checkCmd.ExecuteScalar());

                            string resultMessage = "";

                            if (relatedRecordsCount > 0)
                            {
                                // 存在关联记录：执行锁定操作，设置状态为locked
                                string lockSql = "UPDATE users SET status = 'locked', updated_at = NOW() WHERE id = @userId";
                                MySqlCommand lockCmd = new MySqlCommand(lockSql, connection, transaction);
                                lockCmd.Parameters.AddWithValue("@userId", userId);
                                lockCmd.ExecuteNonQuery();

                                // 获取详细的关联信息用于提示
                                string detailSql = @"
                            SELECT '审批记录(审批人)' as type, COUNT(*) as count FROM approvals WHERE approver_id = @userId
                            UNION ALL
                            SELECT '审计记录(操作者)' as type, COUNT(*) as count FROM audit_logs WHERE actor_id = @userId
                            UNION ALL
                            SELECT '部门管理记录' as type, COUNT(*) as count FROM departments WHERE manager_id = @userId
                            UNION ALL
                            SELECT '实验室成员记录' as type, COUNT(*) as count FROM lab_memberships WHERE user_id = @userId
                            UNION ALL
                            SELECT '实验室管理记录' as type, COUNT(*) as count FROM labs WHERE manager_id = @userId
                            UNION ALL
                            SELECT '采购订单记录' as type, COUNT(*) as count FROM purchase_orders WHERE requester_id = @userId
                            UNION ALL
                            SELECT '库存交易记录' as type, COUNT(*) as count FROM stock_transactions WHERE operator_id = @userId
                            UNION ALL
                            SELECT '用户角色记录' as type, COUNT(*) as count FROM user_roles WHERE user_id = @userId
                            UNION ALL
                            SELECT '角色分配记录' as type, COUNT(*) as count FROM user_roles WHERE assigned_by = @userId
                            UNION ALL
                            SELECT '审计记录(被操作)' as type, COUNT(*) as count FROM audit_logs WHERE object_type = 'users' AND object_id = @userId";

                                MySqlCommand detailCmd = new MySqlCommand(detailSql, connection, transaction);
                                detailCmd.Parameters.AddWithValue("@userId", userId);

                                using (var reader = detailCmd.ExecuteReader())
                                {
                                    string detailMessage = "存在以下关联记录：\n\n";
                                    bool hasRecords = false;

                                    while (reader.Read())
                                    {
                                        int count = Convert.ToInt32(reader["count"]);
                                        if (count > 0)
                                        {
                                            detailMessage += $"{reader["type"]}: {count}条\n";
                                            hasRecords = true;
                                        }
                                    }

                                    if (hasRecords)
                                    {
                                        resultMessage = $"用户【{fullName}】{detailMessage}\n用户已被锁定(locked)，无法删除。";
                                    }
                                    else
                                    {
                                        resultMessage = $"用户【{fullName}】存在关联记录，已被锁定(locked)";
                                    }
                                }
                            }
                            else
                            {
                      
                                string deleteUserSql = "DELETE FROM users WHERE id = @userId";
                                MySqlCommand deleteUserCmd = new MySqlCommand(deleteUserSql, connection, transaction);
                                deleteUserCmd.Parameters.AddWithValue("@userId", userId);
                                int affectedRows = deleteUserCmd.ExecuteNonQuery();

                                if (affectedRows > 0)
                                {
                                    resultMessage = $"用户【{fullName}】已成功删除";
                                }
                                else
                                {
                                    resultMessage = $"未找到用户【{fullName}】，操作失败";
                                    transaction.Rollback();
                                    MessageBox.Show(resultMessage, "操作失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return;
                                }
                            }

                            transaction.Commit();
                            LogAudit logger = new LogAudit(connectionString);
                            logger.LogAction(actorId, relatedRecordsCount > 0 ? "LOCK" : "DELETE", "users", userId);
                            MessageBox.Show(resultMessage, "操作成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            this.DialogResult = DialogResult.OK;
                            this.Close();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw new Exception($"操作执行失败：{ex.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"数据库操作异常：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // DeleteUserForm
            // 
            this.ClientSize = new System.Drawing.Size(278, 244);
            this.Name = "DeleteUserForm";
            this.Load += new System.EventHandler(this.DeleteUserForm_Load);
            this.ResumeLayout(false);

        }

        private void DeleteUserForm_Load(object sender, EventArgs e)
        {

        }
    }
}