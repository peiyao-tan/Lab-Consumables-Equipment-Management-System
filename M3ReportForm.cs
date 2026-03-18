using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using 实验耗材及设备物资管理系统;

namespace WindowsFormsApp1
{
    public partial class M3ReportForm : UserControl
    {
        private DatabaseHelper dbHelper;
        private DataTable originalData;

        // 带参数构造函数（运行时使用）
        public M3ReportForm(DatabaseHelper db)
        {
            dbHelper = db ?? throw new ArgumentNullException(nameof(db));
            InitializeComponent();
            LoadData();
        }

        // 无参构造函数（设计器兼容）
        public M3ReportForm() : this(null) { }

        private void LoadData()
        {
            if (dbHelper == null) return;

            try
            {
                string sql = @"
WITH consumption AS (
    SELECT 
        st.item_id,
        SUM(CASE 
            WHEN st.tx_type = 'ISSUE' THEN st.qty
            WHEN st.tx_type = 'RETURN' THEN -st.qty
            ELSE 0 
        END) AS net_consumption
    FROM stock_transactions st
    WHERE st.tx_time >= DATE_SUB(CURDATE(), INTERVAL 180 DAY)
      AND st.tx_type IN ('ISSUE', 'RETURN')
    GROUP BY st.item_id
),
opening_stock AS (
    SELECT 
        sb.item_id,
        SUM(sb.qty_on_hand) AS opening_qty
    FROM stock_batches sb
    WHERE sb.mfg_date < DATE_SUB(CURDATE(), INTERVAL 180 DAY)
      AND sb.status = 'available'
    GROUP BY sb.item_id
),
closing_stock AS (
    SELECT 
        sb.item_id,
        SUM(sb.qty_on_hand) AS closing_qty
    FROM stock_batches sb
    WHERE sb.status = 'available'
    GROUP BY sb.item_id
)
SELECT 
    i.code AS item_code,
    i.name AS item_name,
    COALESCE(i.category, '未分类') AS category,
    COALESCE(c.net_consumption, 0) AS annual_consumption,
    (COALESCE(o.opening_qty, 0) + COALESCE(cl.closing_qty, 0)) / 2.0 AS avg_stock,
    180 AS period_days
FROM items i
LEFT JOIN consumption c ON i.id = c.item_id
LEFT JOIN opening_stock o ON i.id = o.item_id
LEFT JOIN closing_stock cl ON i.id = cl.item_id
WHERE i.active = 1
  AND (COALESCE(c.net_consumption, 0) > 0 OR COALESCE(o.opening_qty, 0) > 0 OR COALESCE(cl.closing_qty, 0) > 0);";

                originalData = dbHelper.ExecuteQuery(sql);

                // 初始化类别下拉框
                var categories = originalData.AsEnumerable()
                    .Select(r => r.Field<string>("category"))
                    .Where(c => !string.IsNullOrWhiteSpace(c))
                    .Distinct()
                    .OrderBy(c => c)
                    .ToList();

                cmbCategory.Items.Clear();
                cmbCategory.Items.Add("【全部类别】");
                cmbCategory.Items.AddRange(categories.ToArray());
                cmbCategory.SelectedIndex = 0;

                FilterAndDisplayData(null);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载 M3 报表失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FilterAndDisplayData(string selectedCategory)
        {
            if (originalData == null || originalData.Rows.Count == 0) return;

            var filteredRows = originalData.AsEnumerable();
            if (!string.IsNullOrEmpty(selectedCategory) && selectedCategory != "【全部类别】")
            {
                filteredRows = filteredRows.Where(r => r.Field<string>("category") == selectedCategory);
            }

            var tableData = new DataTable();
            tableData.Columns.Add("物资编码", typeof(string));
            tableData.Columns.Add("物资名称", typeof(string));
            tableData.Columns.Add("类别", typeof(string));
            tableData.Columns.Add("年消耗量", typeof(double));
            tableData.Columns.Add("平均库存", typeof(double));
            tableData.Columns.Add("周转率", typeof(double));
            tableData.Columns.Add("周转天数", typeof(double));

            var chartPoints = new List<(string name, string category, double turnover, double days)>();

            foreach (var row in filteredRows)
            {
                // 安全转换数值字段
                double annualConsumption = 0;
                double avgStock = 0;
                int periodDays = 180;

                try
                {
                    object obj = row["annual_consumption"];
                    annualConsumption = obj != DBNull.Value ? Convert.ToDouble(obj) : 0;

                    obj = row["avg_stock"];
                    avgStock = obj != DBNull.Value ? Convert.ToDouble(obj) : 0;

                    obj = row["period_days"];
                    periodDays = obj != DBNull.Value ? Convert.ToInt32(obj) : 180;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"数据类型转换失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    continue; // 跳过当前行
                }

                // 计算周转率和天数
                double turnover = avgStock > 0 ? annualConsumption / avgStock : 0;
                double days = turnover > 0 ? (double)periodDays / turnover : 999;

                string code = row.Field<string>("item_code") ?? "";
                string name = row.Field<string>("item_name") ?? "未知";
                string category = row.Field<string>("category") ?? "未分类";

                chartPoints.Add((name, category, turnover, days));

                tableData.Rows.Add(
                    code,
                    name,
                    category,
                    Math.Round(annualConsumption, 2),
                    Math.Round(avgStock, 2),
                    Math.Round(turnover, 2),
                    Math.Round(days, 0)
                );
            }

            // 绑定表格
            dataGridView1.DataSource = tableData;
            FormatDataGridView();

            // 绘制图表
            RenderChart(chartPoints, selectedCategory);
        }

        private void FormatDataGridView()
        {
            foreach (DataGridViewColumn col in dataGridView1.Columns)
            {
                if (col.Name == "年消耗量" || col.Name == "平均库存")
                    col.DefaultCellStyle.Format = "N2";
                else if (col.Name == "周转率")
                    col.DefaultCellStyle.Format = "F2";
                else if (col.Name == "周转天数")
                    col.DefaultCellStyle.Format = "N0";
            }
        }

        private void RenderChart(List<(string name, string category, double turnover, double days)> points, string selectedCategory)
        {
            chartTurnover.Series.Clear();
            chartTurnover.ChartAreas.Clear();
            chartTurnover.Legends.Clear();

            var area = new ChartArea("Main")
            {
                AxisX = { Title = "周转率", Minimum = 0, Interval = 0.5 },
                AxisY = { Title = "周转天数", Minimum = 0, Interval = 50 }
            };
            chartTurnover.ChartAreas.Add(area);

            if (string.IsNullOrEmpty(selectedCategory) || selectedCategory == "【全部类别】")
            {
                var categories = points.Select(p => p.category).Distinct().ToList();
                var colorMap = GetCategoryColors(categories);

                foreach (var cat in categories)
                {
                    var series = new Series(cat)
                    {
                        ChartType = SeriesChartType.Point,
                        MarkerSize = 8,
                        MarkerStyle = MarkerStyle.Circle,
                        Color = colorMap[cat]
                    };

                    foreach (var p in points.Where(x => x.category == cat))
                    {
                        series.Points.Add(new DataPoint(p.turnover, p.days)
                        {
                            Label = p.name,
                            ToolTip = $"{p.name}\n周转率: {p.turnover:F2}\n天数: {p.days:F0}"
                        });
                    }
                    chartTurnover.Series.Add(series);
                }
            }
            else
            {
                var series = new Series(selectedCategory)
                {
                    ChartType = SeriesChartType.Point,
                    MarkerSize = 8,
                    MarkerStyle = MarkerStyle.Circle,
                    Color = Color.SteelBlue
                };

                foreach (var p in points)
                {
                    series.Points.Add(new DataPoint(p.turnover, p.days)
                    {
                        Label = p.name,
                        ToolTip = $"{p.name}\n周转率: {p.turnover:F2}\n天数: {p.days:F0}"
                    });
                }
                chartTurnover.Series.Add(series);
            }

            chartTurnover.Legends.Add(new Legend());
        }

        private Dictionary<string, Color> GetCategoryColors(List<string> categories)
        {
            var colors = new[]
            {
                Color.Red, Color.Blue, Color.Green, Color.Purple, Color.Orange,
                Color.Brown, Color.Cyan, Color.Magenta, Color.Gray, Color.Maroon,
                Color.Olive, Color.Navy, Color.Teal, Color.Gold, Color.Indigo
            };
            return categories.Select((cat, i) => new { cat, color = colors[i % colors.Length] })
                             .ToDictionary(x => x.cat, x => x.color);
        }

        private void cmbCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selected = cmbCategory.SelectedItem?.ToString();
            if (selected == "【全部类别】") selected = null;
            FilterAndDisplayData(selected);
        }

        private void M3ReportForm_Load(object sender, EventArgs e)
        {

        }
    }
}