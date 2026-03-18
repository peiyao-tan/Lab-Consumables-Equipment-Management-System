using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using 实验耗材及设备物资管理系统;

namespace WindowsFormsApp1
{
    public partial class ReportMonthlyUsageTrend : UserControl
    {
        private DatabaseHelper _dbHelper;

        public ReportMonthlyUsageTrend(DatabaseHelper dbHelper)
        {
            if (dbHelper == null)
                throw new ArgumentNullException(nameof(dbHelper));

            InitializeComponent();
            _dbHelper = dbHelper;
            LoadDataAndRenderChart();
        }

        private void LoadDataAndRenderChart()
        {
            try
            {
                // 修正后的SQL查询
                string sql = @"SELECT CONCAT(YEAR(st.tx_time), '-', LPAD(MONTH(st.tx_time), 2, '0')) AS month, 
                      COALESCE(i.category, '未分类') AS category, 
                      SUM(st.qty) AS total_qty
               FROM stock_transactions st
               LEFT JOIN items i ON st.item_id = i.id
               WHERE st.tx_time >= DATE_SUB(CURDATE(), INTERVAL 12 MONTH) 
                 AND i.category IS NOT NULL 
                 AND i.category != ''
               GROUP BY month, i.category
               ORDER BY month, i.category;";

                DataTable dt = _dbHelper.ExecuteQuery(sql);
                RenderChart(dt);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"加载月度趋势报表失败：{ex.Message}",
                    "错误",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void RenderChart(DataTable dt)
        {
            usageChart.Series.Clear();
            usageChart.ChartAreas.Clear();
            usageChart.Titles.Clear();
            usageChart.Legends.Clear();

            // 创建图表区域
            ChartArea chartArea = new ChartArea();
            usageChart.ChartAreas.Add(chartArea);

            // 创建图例
            Legend legend = new Legend();
            usageChart.Legends.Add(legend);

            if (dt.Rows.Count == 0)
            {
                usageChart.Titles.Add("月度借用/领用数量趋势（无数据）");
                return;
            }

            // 添加标题
            usageChart.Titles.Add("月度借用/领用数量趋势");

            // 获取所有唯一分类（category）
            var categories = dt.AsEnumerable()
                               .Select(r => r.Field<string>("category") ?? "未分类")
                               .Distinct()
                               .OrderBy(c => c)
                               .ToList();

            // 获取所有唯一月份（按时间顺序排序）
            var months = dt.AsEnumerable()
                           .Select(r => r.Field<string>("month"))
                           .Distinct()
                           .OrderBy(m =>
                           {
                               var parts = m.Split('-');
                               return parts.Length == 2 ? int.Parse(parts[0]) * 100 + int.Parse(parts[1]) : 0;
                           })
                           .ToList();

            // 为每个分类创建一个折线系列
            foreach (string category in categories)
            {
                var series = new Series(category)
                {
                    ChartType = SeriesChartType.Line,
                    MarkerStyle = MarkerStyle.Circle,
                    MarkerSize = 8,
                    BorderWidth = 3,
                    MarkerColor = GetSeriesColor(categories.IndexOf(category) % 10)
                };

                // 为每个月份添加数据点（缺失值补 0）
                foreach (string month in months)
                {
                    var row = dt.AsEnumerable()
                                .FirstOrDefault(r =>
                                    r.Field<string>("month") == month &&
                                    (r.Field<string>("category") ?? "未分类") == category);

                    double qty = row != null ? Convert.ToDouble(row["total_qty"]) : 0;
                    series.Points.AddXY(month, qty);
                }

                usageChart.Series.Add(series);
            }

            // 配置图表区域
            ChartArea area = usageChart.ChartAreas[0];
            area.AxisX.Title = "月份";
            area.AxisY.Title = "数量";

            // 确保 X 轴显示所有月份标签
            area.AxisX.Interval = 1;
            area.AxisX.LabelStyle.Angle = -45;
            area.AxisX.MajorGrid.Enabled = false;
            area.AxisY.MajorGrid.Enabled = true;

            // 设置 Y 轴从 0 开始
            area.AxisY.Minimum = 0;

            // 自动调整 Y 轴最大值
            double maxY = usageChart.Series.Max(s => s.Points.Max(p => p.YValues[0]));
            area.AxisY.Maximum = maxY * 1.1; // 留出10%的边距

            // 启用数据点标签
            foreach (Series series in usageChart.Series)
            {
                series.IsValueShownAsLabel = maxY < 100; // 数量较小时显示标签
            }
        }

        private System.Drawing.Color GetSeriesColor(int index)
        {
            System.Drawing.Color[] colors = new System.Drawing.Color[]
            {
                System.Drawing.Color.Red,
                System.Drawing.Color.Blue,
                System.Drawing.Color.Green,
                System.Drawing.Color.Orange,
                System.Drawing.Color.Purple,
                System.Drawing.Color.Brown,
                System.Drawing.Color.Pink,
                System.Drawing.Color.Teal,
                System.Drawing.Color.Magenta,
                System.Drawing.Color.Gray
            };

            return colors[index % colors.Length];
        }
    }
}