using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace 实验耗材及设备物资管理系统
{
    public partial class SupplierPerformanceControl : UserControl
    {
        private DatabaseHelper dbHelper;
        private string currentLab = "";

        public SupplierPerformanceControl()
        {
            InitializeComponent();

            // 在构造中进行轻量级配置（事件绑定、图表基础配置）
            HookEvents();
            ConfigureChartAppearance();
        }

        /// <summary>
        /// 由外部调用以传入数据库访问对象和当前实验室ID
        /// </summary>
        public void Initialize(DatabaseHelper dbHelper, string labId)
        {
            this.dbHelper = dbHelper;
            this.currentLab = labId;

            // 设置默认日期范围（过去12个月）
            try
            {
                dtpStartDate.Value = DateTime.Now.AddMonths(-12);
                dtpEndDate.Value = DateTime.Now;
            }
            catch { }

            // 首次加载数据
            LoadSupplierPerformanceData();
        }

        private void HookEvents()
        {
            // 绑定按钮事件（Designer 已创建控件，这里绑定事件处理）
            btnApply.Click -= BtnApply_Click;
            btnApply.Click += BtnApply_Click;

            btnExport.Click -= BtnExport_Click;
            btnExport.Click += BtnExport_Click;

            // 如果chart控件需要在窗体大小变化时重绘，可绑定 Resize 事件
            this.Resize += (s, e) => chartSupplierPerformance?.Invalidate();
        }

        private void ConfigureChartAppearance()
        {
            if (chartSupplierPerformance == null) return;

            // 清空并初始化
            chartSupplierPerformance.Titles.Clear();
            chartSupplierPerformance.Series.Clear();
            chartSupplierPerformance.ChartAreas.Clear();
            chartSupplierPerformance.Legends.Clear();

            chartSupplierPerformance.BackColor = Color.WhiteSmoke;
            chartSupplierPerformance.BorderlineColor = Color.Gray;
            chartSupplierPerformance.BorderlineDashStyle = ChartDashStyle.Solid;
            chartSupplierPerformance.BorderlineWidth = 1;

            chartSupplierPerformance.Titles.Add("供应商绩效分析");
            chartSupplierPerformance.Titles[0].Font = new Font("Microsoft YaHei", 12, FontStyle.Bold);
            chartSupplierPerformance.Titles[0].Alignment = ContentAlignment.TopCenter;

            ChartArea area = new ChartArea("MainArea");
            area.BackColor = Color.White;

            // X轴
            area.AxisX.Title = "供应商";
            area.AxisX.TitleFont = new Font("Microsoft YaHei", 9, FontStyle.Bold);
            area.AxisX.LabelStyle.Font = new Font("Microsoft YaHei", 8);
            area.AxisX.LabelStyle.Angle = -45;
            area.AxisX.Interval = 1;
            area.AxisX.MajorGrid.LineColor = Color.LightGray;

            // 主 Y 轴（按时率）
            area.AxisY.Title = "按时交付率 (%)";
            area.AxisY.TitleFont = new Font("Microsoft YaHei", 9, FontStyle.Bold);
            area.AxisY.LabelStyle.Format = "P0";
            area.AxisY.Minimum = 0;
            area.AxisY.Maximum = 1; // 100%

            // 次 Y 轴（延迟天数）
            area.AxisY2.Title = "平均延迟天数 (天)";
            area.AxisY2.TitleFont = new Font("Microsoft YaHei", 9, FontStyle.Bold);
            area.AxisY2.LabelStyle.Format = "N1";
            area.AxisY2.Minimum = 0;

            area.Area3DStyle.Enable3D = false;

            chartSupplierPerformance.ChartAreas.Add(area);

            // 按时率（柱）
            Series s1 = new Series("按时率")
            {
                ChartType = SeriesChartType.Column,
                YAxisType = AxisType.Primary,
                IsValueShownAsLabel = true,
                LabelFormat = "P0",
                Font = new Font("Microsoft YaHei", 8),
                Color = Color.FromArgb(54, 128, 220)
            };
            s1["DrawingStyle"] = "Emboss";

            // 平均延迟天数（线）
            Series s2 = new Series("平均延迟天数")
            {
                ChartType = SeriesChartType.Line,
                YAxisType = AxisType.Secondary,
                IsValueShownAsLabel = true,
                LabelFormat = "N1",
                Font = new Font("Microsoft YaHei", 8),
                Color = Color.Red,
                BorderWidth = 3,
                MarkerStyle = MarkerStyle.Circle,
                MarkerSize = 7,
                MarkerColor = Color.Red,
                MarkerBorderColor = Color.White
            };

            chartSupplierPerformance.Series.Add(s1);
            chartSupplierPerformance.Series.Add(s2);

            Legend legend = new Legend { Docking = Docking.Bottom, Font = new Font("Microsoft YaHei", 9) };
            chartSupplierPerformance.Legends.Add(legend);

            lstSupplierDetails.View = View.Details;
            lstSupplierDetails.Columns.Add("供应商名称", 150);
            lstSupplierDetails.Columns.Add("总订单数", 80);
            lstSupplierDetails.Columns.Add("按时订单数", 80);
            lstSupplierDetails.Columns.Add("按时率", 80);
            lstSupplierDetails.Columns.Add("平均延迟天数", 100);

        }

        private void BtnApply_Click(object sender, EventArgs e)
        {
            LoadSupplierPerformanceData();
        }

        private void BtnExport_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "PNG图像|*.png|JPEG图像|*.jpg|BMP图像|*.bmp|GIF图像|*.gif",
                    Title = "导出供应商绩效报表",
                    FileName = $"SupplierPerformance_{DateTime.Now:yyyyMMddHHmmss}"
                };

                if (saveFileDialog.ShowDialog() != DialogResult.OK) return;

                ChartImageFormat format = ChartImageFormat.Png;
                string ext = System.IO.Path.GetExtension(saveFileDialog.FileName).ToLower();
                switch (ext)
                {
                    case ".jpg":
                    case ".jpeg":
                        format = ChartImageFormat.Jpeg;
                        break;
                    case ".bmp":
                        format = ChartImageFormat.Bmp;
                        break;
                    case ".gif":
                        format = ChartImageFormat.Gif;
                        break;
                }

                chartSupplierPerformance.SaveImage(saveFileDialog.FileName, format);
                MessageBox.Show("报表已成功导出！", "导出成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"导出失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadSupplierPerformanceData()
        {
            if (dbHelper == null)
            {
                MessageBox.Show("数据库未初始化（dbHelper 为空）。请先调用 Initialize(dbHelper, labId)。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Cursor = Cursors.WaitCursor;
            try
            {
                DateTime startDate = dtpStartDate.Value;
                DateTime endDate = dtpEndDate.Value.AddDays(1).AddSeconds(-1);

                string query = @"
SELECT 
  s.name AS supplier_name,
  COUNT(p.id) AS total_orders,
  SUM(CASE WHEN st.first_receipt_time <= p.expected_date THEN 1 ELSE 0 END) AS on_time_orders,
  AVG(CASE WHEN st.first_receipt_time > p.expected_date THEN DATEDIFF(st.first_receipt_time, p.expected_date) ELSE 0 END) AS avg_delay_days
FROM purchase_orders p
JOIN suppliers s ON p.supplier_id = s.id
JOIN (
  SELECT ref_doc_id, MIN(tx_time) AS first_receipt_time
  FROM stock_transactions
  WHERE tx_type = 'receipt' AND ref_doc_type = 'PO'
  GROUP BY ref_doc_id
) st ON p.id = st.ref_doc_id
WHERE p.status IN ('received', 'closed')
  AND p.created_at BETWEEN @startDate AND @endDate
GROUP BY s.id, s.name
HAVING COUNT(p.id) > 0
ORDER BY total_orders DESC;
";

                var parameters = new Dictionary<string, object>
                {
                    { "@startDate", startDate },
                    { "@endDate", endDate }
                };

                DataTable dt = dbHelper.ExecuteQuery(query, parameters);

                // 若无数据，清空并提示
                if (dt == null || dt.Rows.Count == 0)
                {
                    MessageBox.Show("没有找到符合条件的供应商绩效数据。请尝试调整时间范围。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearVisualization();
                    return;
                }

                // 填充图表与统计
                FillChartAndStats(dt);

                // 更新明细列表
                UpdateSupplierDetails(dt);

                chartSupplierPerformance.Invalidate();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载供应商绩效数据失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void ClearVisualization()
        {
            if (chartSupplierPerformance != null)
            {
                chartSupplierPerformance.Series["按时率"].Points.Clear();
                chartSupplierPerformance.Series["平均延迟天数"].Points.Clear();
            }
            lblTotalSuppliers.Text = "供应商数量: 0";
            lblAvgOnTimeRate.Text = "平均按时率: 0%";
            lblAvgDelayDays.Text = "平均延迟天数: 0.0 天";
            lstSupplierDetails.Items.Clear();
        }

        private void FillChartAndStats(DataTable dt)
        {
            var sOnTime = chartSupplierPerformance.Series["按时率"];
            var sDelay = chartSupplierPerformance.Series["平均延迟天数"];
            sOnTime.Points.Clear();
            sDelay.Points.Clear();

            double totalOnTimeRate = 0;
            double totalDelayDays = 0;
            int validSuppliers = 0;

            foreach (DataRow row in dt.Rows)
            {
                string supplierName = Convert.ToString(row["supplier_name"]);
                int totalOrders = Convert.ToInt32(row["total_orders"]);
                int onTimeOrders = Convert.ToInt32(row["on_time_orders"]);
                double avgDelayDays = 0;
                if (row["avg_delay_days"] != DBNull.Value) avgDelayDays = Convert.ToDouble(row["avg_delay_days"]);

                if (totalOrders <= 0) continue;

                double onTimeRate = (double)onTimeOrders / totalOrders;
                avgDelayDays = Math.Max(0, avgDelayDays);

                DataPoint colPoint = new DataPoint { AxisLabel = supplierName, YValues = new double[] { onTimeRate } };
                sOnTime.Points.Add(colPoint);

                DataPoint linePoint = new DataPoint { AxisLabel = supplierName, YValues = new double[] { avgDelayDays } };
                sDelay.Points.Add(linePoint);

                totalOnTimeRate += onTimeRate;
                totalDelayDays += avgDelayDays;
                validSuppliers++;
            }

            if (validSuppliers > 0)
            {
                lblTotalSuppliers.Text = $"供应商数量: {validSuppliers}";
                lblAvgOnTimeRate.Text = $"平均按时率: {(totalOnTimeRate / validSuppliers).ToString("P1")}";
                lblAvgDelayDays.Text = $"平均延迟天数: {(totalDelayDays / validSuppliers).ToString("N1")} 天";
            }
            else
            {
                lblTotalSuppliers.Text = "供应商数量: 0";
                lblAvgOnTimeRate.Text = "平均按时率: 0%";
                lblAvgDelayDays.Text = "平均延迟天数: 0.0 天";
            }

            // 控制 X 轴 标签间隔避免重叠
            int points = chartSupplierPerformance.Series["按时率"].Points.Count;
            if (points > 10)
            {
                chartSupplierPerformance.ChartAreas["MainArea"].AxisX.LabelStyle.Interval = Math.Ceiling((double)points / 10);
            }
            else
            {
                chartSupplierPerformance.ChartAreas["MainArea"].AxisX.LabelStyle.Interval = 1;
            }
        }

        private void UpdateSupplierDetails(DataTable dt)
        {
            lstSupplierDetails.Items.Clear();

            foreach (DataRow row in dt.Rows)
            {
                string supplierName = Convert.ToString(row["supplier_name"]);
                int totalOrders = Convert.ToInt32(row["total_orders"]);
                int onTimeOrders = Convert.ToInt32(row["on_time_orders"]);
                double avgDelayDays = row["avg_delay_days"] == DBNull.Value ? 0d : Convert.ToDouble(row["avg_delay_days"]);

                double onTimeRate = totalOrders > 0 ? (double)onTimeOrders / totalOrders : 0;

                ListViewItem item = new ListViewItem(supplierName);
                item.SubItems.Add(totalOrders.ToString());
                item.SubItems.Add(onTimeOrders.ToString());
                item.SubItems.Add(onTimeRate.ToString("P1"));
                item.SubItems.Add(avgDelayDays.ToString("N1"));

                if (onTimeRate >= 0.95) item.BackColor = Color.LightGreen;
                else if (onTimeRate >= 0.85) item.BackColor = Color.LightYellow;
                else item.BackColor = Color.LightPink;

                lstSupplierDetails.Items.Add(item);
            }
        }

        private void mainPanel_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}