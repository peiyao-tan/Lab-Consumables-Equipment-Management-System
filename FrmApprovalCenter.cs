using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class FrmApprovalCenter : UserControl
    {
        private string _currentUserId;
        private List<ApprovalTask> _pendingTasks = new List<ApprovalTask>();
        
        private string _connectionString = "server=localhost;user=root;database=try;port=3306;password=123456";

        public FrmApprovalCenter(string currentUserId)
        {
            InitializeComponent();
            _currentUserId = currentUserId;

            // 绑定按钮事件
            if (btnApprove != null) btnApprove.Click += btnApprove_Click;
            if (btnReject != null) btnReject.Click += btnReject_Click;
            if (btnRefresh != null) btnRefresh.Click += btnRefresh_Click;

            // 移除：构造函数中不加载数据，避免控件未初始化完成
        }

        private void FrmApprovalCenter_Load(object sender, EventArgs e)
        {
            SetupDataGridViewColumns(); // 先初始化列
            LoadPendingApprovals();    // 后加载数据
        }

        private void LoadPendingApprovals()
        {
            try
            {
                _pendingTasks.Clear();

                string sql = @"
SELECT 
    a.id AS approvals_id, 
    a.biz_type,
    a.biz_id,
    a.step_no,
    a.approver_id,
    a.status,
    a.comment,
    a.acted_at,
    -- 业务创建时间（根据业务类型从不同表获取）
    CASE a.biz_type
        WHEN 'PO' THEN po.created_at
        WHEN 'BR' THEN bo.created_at
        WHEN 'RS' THEN r.start_time
        WHEN 'MO' THEN COALESCE(mo.completed_at, mo.scheduled_date)
        WHEN 'SC' THEN sc.started_at
        ELSE NOW()
    END AS biz_created_time,
    -- 业务类型中文描述
    CASE a.biz_type
        WHEN 'PO' THEN '采购申请'
        WHEN 'BR' THEN '物资借用'
        WHEN 'RS' THEN '设备预约'
        WHEN 'MO' THEN '设备维护'
        WHEN 'SC' THEN '库存盘点'
        ELSE a.biz_type
    END AS biz_type_name,
    -- 文档编号
    CASE a.biz_type
        WHEN 'PO' THEN po.po_no
        WHEN 'BR' THEN bo.borrow_no
        WHEN 'RS' THEN CONCAT('RS-', r.id)
        WHEN 'MO' THEN mo.mo_no
        WHEN 'SC' THEN sc.count_no
        ELSE CONCAT('UNK-', a.biz_id)
    END AS doc_no,
    -- 申请人信息
    CASE a.biz_type
        WHEN 'PO' THEN u_po.full_name
        WHEN 'BR' THEN u_br.full_name
        WHEN 'RS' THEN u_rs.full_name
        WHEN 'MO' THEN '系统' -- 3. 修正：MO无直接申请人，直接显示系统
        WHEN 'SC' THEN u_sc.full_name
        ELSE '未知申请人'
    END AS requester_name
FROM approvals a
LEFT JOIN purchase_orders po ON a.biz_type = 'PO' AND a.biz_id = po.id
LEFT JOIN borrow_orders bo ON a.biz_type = 'BR' AND a.biz_id = bo.id
LEFT JOIN reservations r ON a.biz_type = 'RS' AND a.biz_id = r.id
LEFT JOIN maintenance_orders mo ON a.biz_type = 'MO' AND a.biz_id = mo.id
LEFT JOIN stock_counts sc ON a.biz_type = 'SC' AND a.biz_id = sc.id
LEFT JOIN users u_po ON po.requester_id = u_po.id
LEFT JOIN users u_br ON bo.requester_id = u_br.id
LEFT JOIN users u_rs ON r.requester_id = u_rs.id
-- 移除：错误的MO与用户关联语句
LEFT JOIN users u_sc ON sc.initiator_id = u_sc.id
WHERE a.approver_id = @approver_id 
  AND a.status = 'pending'
ORDER BY biz_created_time DESC, a.step_no"; // 优化：按创建时间倒序，最新任务在前

                using (MySqlConnection conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        // 确保参数类型与数据库UUID字段匹配（避免字符串格式问题）
                        cmd.Parameters.AddWithValue("@approver_id", _currentUserId);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // 4. 增强空值处理：UUID字段强制转换为字符串
                                string approvalId = reader["approvals_id"]?.ToString() ?? string.Empty;

                                if (!string.IsNullOrEmpty(approvalId))
                                {
                                    var task = new ApprovalTask
                                    {
                                        Id = approvalId,
                                        BizType = reader["biz_type"]?.ToString() ?? string.Empty,
                                        BizId = reader["biz_id"]?.ToString() ?? string.Empty,
                                        StepNo = reader["step_no"] != DBNull.Value ? Convert.ToInt32(reader["step_no"]) : 1,
                                        Status = reader["status"]?.ToString() ?? string.Empty,
                                        Comment = reader["comment"]?.ToString() ?? string.Empty,
                                        DocNo = reader["doc_no"]?.ToString() ?? "N/A",
                                        RequesterName = reader["requester_name"]?.ToString() ?? "未知申请人",
                                        // 日期字段空值处理
                                        CreatedAt = reader["biz_created_time"] != DBNull.Value
                                            ? Convert.ToDateTime(reader["biz_created_time"])
                                            : DateTime.Now,
                                        BizTypeName = reader["biz_type_name"]?.ToString() ?? "未知业务"
                                    };

                                    _pendingTasks.Add(task);
                                }
                            }
                        }
                    }
                }

                // 调试提示：显示查询到的任务数量
                MessageBox.Show($"查询到 {_pendingTasks.Count} 条待审批任务", "调试信息",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                BindDataToGridView();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载审批数据失败：{ex.Message}\n\nSQL异常请检查查询语句和表结构。", "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BindDataToGridView()
        {
            try
            {
                if (dataGridView1 == null)
                {
                    MessageBox.Show("DataGridView控件未找到，请检查控件名称是否正确", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                dataGridView1.AutoGenerateColumns = false;
                dataGridView1.DataSource = null;
                dataGridView1.DataSource = _pendingTasks;

                // 强制刷新数据显示
                dataGridView1.Refresh();

                UpdateButtonState();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"数据绑定失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetupDataGridViewColumns()
        {
            if (dataGridView1 == null)
            {
                MessageBox.Show("DataGridView控件未初始化", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (dataGridView1.Columns.Count == 0)
            {
                dataGridView1.Columns.Clear();

                // 单据编号列
                dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "colDocNo",
                    HeaderText = "单据编号",
                    DataPropertyName = "DocNo",
                    Width = 120,
                    ReadOnly = true
                });

                // 业务类型列
                dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "colBizType",
                    HeaderText = "业务类型",
                    DataPropertyName = "BizTypeName",
                    Width = 100,
                    ReadOnly = true
                });

                // 申请人列
                dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "colRequester",
                    HeaderText = "申请人",
                    DataPropertyName = "RequesterName",
                    Width = 100,
                    ReadOnly = true
                });

                // 审批步骤列
                dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "colStep",
                    HeaderText = "审批步骤",
                    DataPropertyName = "StepNoDisplay",
                    Width = 80,
                    ReadOnly = true
                });

                // 申请时间列
                dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "colCreatedAt",
                    HeaderText = "申请时间",
                    DataPropertyName = "CreatedAtDisplay",
                    Width = 150,
                    ReadOnly = true
                });

                // 设置日期格式
                dataGridView1.Columns["colCreatedAt"].DefaultCellStyle.Format = "yyyy-MM-dd HH:mm";
                // 启用行选择模式
                dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                // 绑定选择变更事件
                dataGridView1.SelectionChanged += dataGridView1_SelectionChanged;
            }
        }

        private void UpdateButtonState()
        {
            bool hasSelection = dataGridView1?.SelectedRows.Count > 0;
            if (btnApprove != null) btnApprove.Enabled = hasSelection;
            if (btnReject != null) btnReject.Enabled = hasSelection;
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            UpdateButtonState();
        }

        private void btnApprove_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("请先选择一条待审批记录。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var selectedRow = dataGridView1.SelectedRows[0];
            if (selectedRow.DataBoundItem is ApprovalTask task)
            {
                try
                {
                    DialogResult confirm = MessageBox.Show(
                        $"确定要通过【{task.BizTypeName}】申请（单据号：{task.DocNo}）吗？",
                        "确认审批",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (confirm != DialogResult.Yes) return;

                    UpdateApprovalStatus(task.Id, "approved", "");
                    AdvanceMainDocumentStatus(task.BizType, task.BizId);
                    MessageBox.Show("审批通过成功！", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadPendingApprovals();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"审批失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnReject_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("请先选择一条待审批记录。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var selectedRow = dataGridView1.SelectedRows[0];
            if (selectedRow.DataBoundItem is ApprovalTask task)
            {
                string comment = ShowRejectReasonDialog();
                if (string.IsNullOrEmpty(comment)) return;

                try
                {
                    UpdateApprovalStatus(task.Id, "rejected", comment);
                    RejectMainDocument(task.BizType, task.BizId);
                    MessageBox.Show("已驳回！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadPendingApprovals();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"驳回失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private string ShowRejectReasonDialog()
        {
            using (Form form = new Form())
            {
                form.Text = "驳回原因";
                form.Width = 400;
                form.Height = 200;
                form.StartPosition = FormStartPosition.CenterParent;
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                form.MaximizeBox = false;
                form.MinimizeBox = false;

                Label label = new Label { Text = "请输入驳回原因：", Location = new Point(10, 10), Width = 200 };
                TextBox textBox = new TextBox { Location = new Point(10, 35), Width = 360, Height = 60, Multiline = true };
                Button okBtn = new Button { Text = "确定", Location = new Point(150, 110), DialogResult = DialogResult.OK };
                Button cancelBtn = new Button { Text = "取消", Location = new Point(250, 110), DialogResult = DialogResult.Cancel };

                form.Controls.AddRange(new Control[] { label, textBox, okBtn, cancelBtn });
                form.AcceptButton = okBtn;
                form.CancelButton = cancelBtn;

                return form.ShowDialog() == DialogResult.OK ? textBox.Text.Trim() : null;
            }
        }

        private void UpdateApprovalStatus(string approvalId, string status, string comment)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                string sql = @"UPDATE approvals 
                              SET status = @status, 
                                  comment = @comment, 
                                  acted_at = NOW()
                              WHERE id = @id";

                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@status", status);
                    cmd.Parameters.AddWithValue("@comment", comment ?? "");
                    cmd.Parameters.AddWithValue("@id", approvalId);
                    int affected = cmd.ExecuteNonQuery();
                    if (affected == 0)
                        throw new Exception("审批记录未找到或已被处理。");
                }
            }
        }

        private void AdvanceMainDocumentStatus(string bizType, string bizId)
        {
            string table = GetTableNameByBizType(bizType);
            string status = GetApprovedStatusByBizType(bizType);
            ExecuteDocumentStatusUpdate(table, bizId, status);
        }

        private void RejectMainDocument(string bizType, string bizId)
        {
            string table = GetTableNameByBizType(bizType);
            string status = GetRejectedStatusByBizType(bizType);
            ExecuteDocumentStatusUpdate(table, bizId, status);
        }

        private string GetTableNameByBizType(string bizType)
        {
            switch (bizType)
            {
                case "PO": return "purchase_orders";
                case "BR": return "borrow_orders";
                case "RS": return "reservations";
                case "MO": return "maintenance_orders";
                case "SC": return "stock_counts";
                default:
                    throw new ArgumentException($"不支持的业务类型：{bizType}");
            }
        }

        private string GetApprovedStatusByBizType(string bizType)
        {
            // 优化：根据不同业务类型返回对应的审批通过状态（匹配数据库设计）
            switch (bizType)
            {
                case "PO": return "approved";
                case "BR": return "approved";
                case "RS": return "approved";
                case "MO": return "approved";
                case "SC": return "approved";
                default: return "approved";
            }
        }

        private string GetRejectedStatusByBizType(string bizType)
        {
            switch (bizType)
            {
                case "SC": return "cancelled";
                default: return "rejected";
            }
        }

        private void ExecuteDocumentStatusUpdate(string table, string id, string status)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                string sql = $@"UPDATE {table} SET status = @status WHERE id = @id";
                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@status", status);
                    cmd.Parameters.AddWithValue("@id", id);
                    int affected = cmd.ExecuteNonQuery();
                    if (affected == 0)
                        throw new Exception($"业务单据（{table} - {id}）未找到或已被修改。");
                }
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadPendingApprovals();
        }
    }

    public class ApprovalTask
    {
        public string Id { get; set; }
        public string BizType { get; set; }
        public string BizId { get; set; }
        public int StepNo { get; set; }
        public string Status { get; set; }
        public string Comment { get; set; }
        public string DocNo { get; set; }
        public string RequesterName { get; set; }
        public DateTime CreatedAt { get; set; }
        public string BizTypeName { get; set; }

        // 计算属性：步骤显示
        public string StepNoDisplay => $"步骤{StepNo}";

        // 计算属性：创建时间格式化显示
        public string CreatedAtDisplay => CreatedAt.ToString("yyyy-MM-dd HH:mm");

        // 计算属性：业务类型描述（冗余，可保留）
        public string Description
        {
            get
            {
                switch (BizType)
                {
                    case "PO": return "采购申请";
                    case "BR": return "物资借用";
                    case "RS": return "设备预约";
                    case "MO": return "设备维护";
                    case "SC": return "库存盘点";
                    default: return "未知业务";
                }
            }
        }
    }
}