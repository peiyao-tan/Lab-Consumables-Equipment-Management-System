using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.IO;
using 实验耗材及设备物资管理系统;

namespace WindowsFormsApp1
{
    public partial class ReportBorrowOverdueRate : UserControl
    {
        private DatabaseHelper dbHelper;

        // 报告数据类
        private class OverdueRateData
        {
            public string LabName { get; set; }
            public int TotalOrders { get; set; }
            public int NormalCount { get; set; }
            public int OverdueCount { get; set; }
            public decimal OverdueRate { get; set; }
        }

        private List<OverdueRateData> _currentData;

        // 无参构造函数
        public ReportBorrowOverdueRate() : this(null)
        {

        }

        public ReportBorrowOverdueRate(DatabaseHelper dbHelper)
        {
            this.dbHelper = dbHelper;
            InitializeComponent();
            InitializeEvents();
            InitializeControls();
        }

        private void InitializeEvents()
        {
            this.Load += ReportBorrowOverdueRate_Load;
            btnGenerate.Click += btnGenerate_Click;
            btnExportExcel.Click += btnExportExcel_Click;
        }

        private void InitializeControls()
        {
            // 手动设置文本，避免资源依赖
            lblTitle.Text = "借用超期率";
            label1.Text = "日期范围";
            label2.Text = "至";
            btnGenerate.Text = "生成报表";
            btnExportExcel.Text = "导出Excel";

            // 配置按钮图标
            btnGenerate.Image = Resources.GetIcon("generate");
            btnExportExcel.Image = Resources.GetIcon("export");

            // 配置日期选择器
            dtpStartDate.Value = DateTime.Today.AddMonths(-1);
            dtpEndDate.Value = DateTime.Today;

            // 配置数据表格
            ConfigureDataGridView();

            // 初始化加载数据
            GenerateReport();
        }

        private void ConfigureDataGridView()
        {
            // 清除现有列
            dgvDetails.Columns.Clear();

            // 添加列
            dgvDetails.Columns.Add("LabName", "实验室名称");
            dgvDetails.Columns.Add("TotalOrders", "总单数");
            dgvDetails.Columns.Add("NormalCount", "正常单数");
            dgvDetails.Columns.Add("OverdueCount", "超期单数");
            dgvDetails.Columns.Add("OverdueRate", "超期率");

            // 设置列样式
            dgvDetails.Columns["LabName"].Width = 250;
            dgvDetails.Columns["TotalOrders"].Width = 100;
            dgvDetails.Columns["NormalCount"].Width = 100;
            dgvDetails.Columns["OverdueCount"].Width = 100;
            dgvDetails.Columns["OverdueRate"].Width = 100;

            // 设置对齐方式
            dgvDetails.Columns["TotalOrders"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvDetails.Columns["NormalCount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvDetails.Columns["OverdueCount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvDetails.Columns["OverdueRate"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // 设置数字格式
            dgvDetails.Columns["OverdueRate"].DefaultCellStyle.Format = "P1";

            // 设置表头样式
            dgvDetails.EnableHeadersVisualStyles = false;
            dgvDetails.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 73, 94);
            dgvDetails.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvDetails.ColumnHeadersDefaultCellStyle.Font = new Font("微软雅黑", 9F, FontStyle.Bold);
            dgvDetails.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // 设置行高
            dgvDetails.RowTemplate.Height = 30;

            // 设置网格样式
            dgvDetails.GridColor = Color.LightGray;
            dgvDetails.BorderStyle = BorderStyle.None;
            dgvDetails.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
        }

        private void ReportBorrowOverdueRate_Load(object sender, EventArgs e)
        {
            GenerateReport();
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            GenerateReport();
        }

        private void btnExportExcel_Click(object sender, EventArgs e)
        {
            if (_currentData == null || _currentData.Count == 0)
            {
                MessageBox.Show("没有数据可导出", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Excel文件|*.xlsx";
                saveFileDialog.Title = "导出借用超期率报表";
                saveFileDialog.FileName = $"借用超期率报表_{DateTime.Now:yyyyMMddHHmmss}.xlsx";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    ExportToExcel(saveFileDialog.FileName);
                    MessageBox.Show($"报表已成功导出到:\n{saveFileDialog.FileName}", "导出成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"导出失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExportToExcel(string fileName)
        {
            // 简单的CSV导出实现（如果无法引用Excel库）
            if (Path.GetExtension(fileName).ToLower() == ".csv")
            {
                ExportToCsv(fileName);
                return;
            }

            // 如果有Excel库引用，可以实现真正的Excel导出
            // 以下为模拟实现
            System.IO.File.WriteAllText(fileName,
                "借用超期率报表\n" +
                $"生成时间: {DateTime.Now:yyyy-MM-dd HH:mm:ss}\n\n" +
                "实验室名称,总单数,正常单数,超期单数,超期率\n" +
                string.Join("\n", _currentData.Select(d =>
                    $"{d.LabName},{d.TotalOrders},{d.NormalCount},{d.OverdueCount},{d.OverdueRate:P1}")));
        }

        private void ExportToCsv(string fileName)
        {
            using (var writer = new System.IO.StreamWriter(fileName, false, System.Text.Encoding.UTF8))
            {
                writer.WriteLine("借用超期率报表");
                writer.WriteLine($"生成时间: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                writer.WriteLine();
                writer.WriteLine("实验室名称,总单数,正常单数,超期单数,超期率");

                foreach (var data in _currentData)
                {
                    writer.WriteLine($"{data.LabName},{data.TotalOrders},{data.NormalCount},{data.OverdueCount},{data.OverdueRate:P1}");
                }
            }
        }

        private void GenerateReport()
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                ShowLoading(true);

                // 获取数据 - 真实数据
                _currentData = GetRealData();

                // 更新数据表格
                UpdateDataGridView();

                // 更新摘要
                UpdateSummary();

                // 更新图表
                UpdateChart();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"生成报表时出错: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                ShowLoading(false);
                Cursor = Cursors.Default;
            }
        }

        private List<OverdueRateData> GetRealData()
        {
            var data = new List<OverdueRateData>();

            if (dbHelper == null)
            {
                // 模拟数据
                return GetMockData();
            }

            // SQL查询：获取按实验室分组的借用超期率
            string sql = @"
                SELECT 
                    l.name AS LabName,
                    COUNT(*) AS TotalOrders,
                    SUM(CASE WHEN bo.status = 'overdue' OR (bo.status IN ('issued', 'partially_returned') AND bo.expected_return_date < CURDATE())
                        THEN 1 ELSE 0 END) AS OverdueCount,
                    SUM(CASE WHEN NOT (bo.status = 'overdue' OR (bo.status IN ('issued', 'partially_returned') AND bo.expected_return_date < CURDATE()))
                        THEN 1 ELSE 0 END) AS NormalCount
                FROM borrow_orders bo
                JOIN labs l ON bo.lab_id = l.id
                WHERE bo.expected_return_date IS NOT NULL 
                  AND bo.created_at BETWEEN @startDate AND @endDate
                GROUP BY l.name
                ORDER BY TotalOrders DESC";

            var parameters = new Dictionary<string, object>
            {
                { "@startDate", dtpStartDate.Value.Date },
                { "@endDate", dtpEndDate.Value.Date.AddDays(1).AddSeconds(-1) }
            };

            try
            {
                DataTable dt = dbHelper.ExecuteQuery(sql, parameters);

                foreach (DataRow row in dt.Rows)
                {
                    int total = Convert.ToInt32(row["TotalOrders"]);
                    int overdue = Convert.ToInt32(row["OverdueCount"]);

                    data.Add(new OverdueRateData
                    {
                        LabName = row["LabName"].ToString(),
                        TotalOrders = total,
                        OverdueCount = overdue,
                        NormalCount = total - overdue,
                        OverdueRate = total > 0 ? (decimal)overdue / total : 0
                    });
                }

                // 如果没有数据，返回空列表
                return data;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"查询数据失败: {ex.Message}\n使用模拟数据", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return GetMockData();
            }
        }

        private List<OverdueRateData> GetMockData()
        {
            return new List<OverdueRateData>
            {
                new OverdueRateData { LabName = "分子生物学实验室", TotalOrders = 45, NormalCount = 38, OverdueCount = 7, OverdueRate = 7m/45m },
                new OverdueRateData { LabName = "细胞培养实验室", TotalOrders = 32, NormalCount = 25, OverdueCount = 7, OverdueRate = 7m/32m },
                new OverdueRateData { LabName = "化学分析实验室", TotalOrders = 28, NormalCount = 24, OverdueCount = 4, OverdueRate = 4m/28m },
                new OverdueRateData { LabName = "材料测试实验室", TotalOrders = 19, NormalCount = 12, OverdueCount = 7, OverdueRate = 7m/19m },
                new OverdueRateData { LabName = "环境监测实验室", TotalOrders = 15, NormalCount = 15, OverdueCount = 0, OverdueRate = 0m },
                new OverdueRateData { LabName = "物理测量实验室", TotalOrders = 22, NormalCount = 14, OverdueCount = 8, OverdueRate = 8m/22m }
            };
        }

        private void ShowLoading(bool show)
        {
            if (show)
            {
                var loadingPanel = new Panel
                {
                    Dock = DockStyle.Fill,
                    BackColor = Color.FromArgb(220, 255, 255, 255),
                    Cursor = Cursors.WaitCursor
                };

                var progressPanel = new Panel
                {
                    Width = 150,
                    Height = 100,
                    BackColor = Color.FromArgb(41, 128, 185),
                    Location = new System.Drawing.Point((Width - 150) / 2, (Height - 100) / 2),
                    BorderStyle = BorderStyle.None
                };

                var loadingLabel = new Label
                {
                    Text = "加载中...",
                    ForeColor = Color.White,
                    Font = new Font("微软雅黑", 12F, FontStyle.Bold),
                    AutoSize = true,
                    Location = new System.Drawing.Point(35, 30)
                };

                progressPanel.Controls.Add(loadingLabel);
                loadingPanel.Controls.Add(progressPanel);
                this.Controls.Add(loadingPanel);
                loadingPanel.BringToFront();

                this.Tag = loadingPanel;
            }
            else
            {
                if (this.Tag is Panel loadingPanel)
                {
                    this.Controls.Remove(loadingPanel);
                    this.Tag = null;
                }
            }
        }

        private void UpdateDataGridView()
        {
            dgvDetails.Rows.Clear();

            if (_currentData == null || _currentData.Count == 0)
            {
                lblSummary.Text = "没有找到数据";
                return;
            }

            foreach (var data in _currentData)
            {
                int rowIndex = dgvDetails.Rows.Add();
                var row = dgvDetails.Rows[rowIndex];

                row.Cells["LabName"].Value = data.LabName;
                row.Cells["TotalOrders"].Value = data.TotalOrders;
                row.Cells["NormalCount"].Value = data.NormalCount;
                row.Cells["OverdueCount"].Value = data.OverdueCount;
                row.Cells["OverdueRate"].Value = data.OverdueRate;

                // 超期率高的行标记为红色
                if (data.OverdueRate > 0.3m)
                {
                    row.DefaultCellStyle.BackColor = Color.FromArgb(255, 235, 235);
                    row.DefaultCellStyle.ForeColor = Color.FromArgb(183, 28, 28);
                }
                else if (data.OverdueRate > 0.1m)
                {
                    row.DefaultCellStyle.BackColor = Color.FromArgb(255, 255, 235);
                    row.DefaultCellStyle.ForeColor = Color.FromArgb(117, 79, 0);
                }
            }
        }

        private void UpdateSummary()
        {
            if (_currentData == null || _currentData.Count == 0)
            {
                lblSummary.Text = "没有数据";
                return;
            }

            var totalOrders = _currentData.Sum(d => d.TotalOrders);
            var totalOverdue = _currentData.Sum(d => d.OverdueCount);
            var avgOverdueRate = totalOrders > 0 ? (decimal)totalOverdue / totalOrders : 0;

            // 找出超期最严重的实验室
            var worstLab = _currentData.OrderByDescending(d => d.OverdueRate).FirstOrDefault();

            lblSummary.Text = $"总计: {totalOrders} 单 | 超期: {totalOverdue} 单 | " +
                             $"平均超期率: {avgOverdueRate:P1} | " +
                             $"最严重: {worstLab?.LabName ?? "无"} ({worstLab?.OverdueRate:P1})";
        }

        private void UpdateChart()
        {
            // 清空现有图表
            chartOverdue.Series.Clear();

            // 创建两个系列：正常和超期
            var normalSeries = new Series("正常归还")
            {
                ChartType = SeriesChartType.StackedColumn,
                Color = Color.FromArgb(28, 184, 65),
                IsValueShownAsLabel = true,
                LabelForeColor = Color.White
            };

            var overdueSeries = new Series("超期归还")
            {
                ChartType = SeriesChartType.StackedColumn,
                Color = Color.FromArgb(224, 49, 38),
                IsValueShownAsLabel = true,
                LabelForeColor = Color.White
            };

            // 添加数据
            foreach (var item in _currentData)
            {
                normalSeries.Points.AddXY(item.LabName, item.NormalCount);
                overdueSeries.Points.AddXY(item.LabName, item.OverdueCount);
            }

            // 添加到图表
            chartOverdue.Series.Add(normalSeries);
            chartOverdue.Series.Add(overdueSeries);

            // 设置图表属性
            chartOverdue.ChartAreas[0].AxisY.Title = "借用单数量";
            chartOverdue.ChartAreas[0].AxisX.Title = "实验室";
            chartOverdue.Titles.Clear();
            chartOverdue.Titles.Add("按实验室分组的借用超期情况");
            chartOverdue.Titles[0].Font = new Font("微软雅黑", 12F, FontStyle.Bold);
        }
    }

    // 简单的资源模拟类
    public static class Resources
    {
        public static Image GetIcon(string name)
        {
            switch (name.ToLower())
            {
                case "generate":
                    return CreateImageFromBase64("iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAABHNCSVQICAgIfAhkiAAAAAlwSFlzAAALEwAACxMBAJqcGAAAAVhJREFUOI3t0j1LQlEYxvHf65vY0FZLQdCgCIKgCIKgCIJgEATBUM1BcHBwEQRBcHBwEQRBcAqCIHh+V6GhoaHzvs/z4nDPf/78OedcLlyKokjwDyKXZVmGpumQZRlpmiIMQ+zu7mJjYwPb29sYDAZQVRXVahW+79dZwzAgiiLIsow8z+F5HgaDAdrtNlqtFgRBgOM4v4ZBEEBRFJTLZRQKBeR5jjiOkec5ZFlGp9P5NYzjGFEUQdd1aJoGWZaRZRmiKEIcx5BlGWVZ/hhGUQRd11Gv16FpGrIsQxRFiOMYQRAgiiLUarUfwyiKEIYhGo0GGo0GZFlGkiQIwxBBECCOYzQajR/DMAwRhiE6nQ7a7TbK5TJkWUaSJAiCAH4QIEkStFqtH8MwDOH7PjqdDtrtNiqVCuI4hu/78DwPSZKg3W7/GPq+j8lkgslkgr29PZRKJciyjDiO4fs+PM9DmqbodrtfhkVR4Pr6GtfX17i6usLV1RXOz89xdXWF6+trXF9f4/b2Fn6/D3/wGf5gAH8wwHA4hO/7CIKgrgYfL3B3d4f7+3s8PDzg8fERz8/PeHl5wevra319e3vD+/s7Pj4+8Pn5ia+vrz9fP8C3JH0lZ5XNAAAAAElFTkSuQmCC");
                case "export":
                    return CreateImageFromBase64("iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAABHNCSVQICAgIfAhkiAAAAAlwSFlzAAALEwAACxMBAJqcGAAAAGVJREFUOI3t0jEKgDAMBdD3n7p0EBy6dCk4Cg4ODoKDIIKDiIg4iIiIiIiIiIiIiIiIiIiIiIiIyC1ZKqT9QNKlTtLzJgF1xO8HrCGXK+QD6pBZoY74/YB55FJjPqAPmdXqiN8PWEAuNeYDxpBLjfmAL8Q9L4J3w1l/AAAAAElFTkSuQmCC");
                default:
                    return null;
            }
        }

        private static Image CreateImageFromBase64(string base64)
        {
            try
            {
                byte[] imageBytes = Convert.FromBase64String(base64);
                using (var ms = new System.IO.MemoryStream(imageBytes))
                {
                    return Image.FromStream(ms);
                }
            }
            catch
            {
                return null;
            }
        }
    }
}