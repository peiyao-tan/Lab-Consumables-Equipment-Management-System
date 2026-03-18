using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Windows.Forms.DataVisualization.Charting;

namespace WindowsFormsApp1
{
    public partial class M9_ABC_AnalysisReport : UserControl
    {
        public M9_ABC_AnalysisReport()
        {
            InitializeComponent();
            InitializeControls();
            LoadDefaultData();
        }

        private void InitializeControls()
        {
            // 设置默认日期范围（最近30天）
            dtpEndDate.Value = DateTime.Now;
            dtpStartDate.Value = DateTime.Now.AddDays(-30);

            // 设置工具提示和右键菜单
            SetupToolTips();
            SetupContextMenu();
        }

        private void LoadDefaultData()
        {
            // 默认加载最近30天的数据
            LoadABCAnalysisData(dtpStartDate.Value, dtpEndDate.Value);
        }

        private void BtnQuery_Click(object sender, EventArgs e)
        {
            try
            {
                LoadABCAnalysisData(dtpStartDate.Value, dtpEndDate.Value);
                MessageBox.Show("ABC分析数据加载完成！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"查询数据时出错：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadABCAnalysisData(DateTime startDate, DateTime endDate)
        {
            try
            {
                // 根据实际调整连接字符串（host/port/user/password/database）
                string connectionString = "server=localhost;port=3306;user id=root;password=123456;database=try;SslMode=none;";

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"
WITH consumption AS (
SELECT st.item_id,
SUM(st.qty) AS consumed_qty
FROM stock_transactions st
WHERE st.tx_time BETWEEN @StartDate AND @EndDate
AND st.tx_type IN ('issue','borrow_out')
GROUP BY st.item_id
),
po_weighted AS (
SELECT poi.item_id,
SUM( (CASE WHEN IFNULL(poi.qty_received,0) > 0 THEN poi.qty_received ELSE poi.qty_ordered END) * IFNULL(poi.unit_price,0) ) AS total_amt,
SUM( CASE WHEN IFNULL(poi.qty_received,0) > 0 THEN poi.qty_received ELSE poi.qty_ordered END ) AS total_qty
FROM purchase_order_items poi
JOIN purchase_orders po ON poi.po_id = po.id
WHERE po.status IN ('received','partially_received','closed')
GROUP BY poi.item_id
),
last_price AS (
SELECT item_id, unit_price FROM (
SELECT poi.item_id,
IFNULL(poi.unit_price,0) AS unit_price,
ROW_NUMBER() OVER (PARTITION BY poi.item_id ORDER BY po.created_at DESC, poi.id DESC) rn
FROM purchase_order_items poi
JOIN purchase_orders po ON poi.po_id = po.id
) t
WHERE rn = 1
)
SELECT
i.id AS item_id,
i.code AS 物品编码,
i.name AS 物品名称,
i.category AS 分类,
IFNULL(c.consumed_qty,0) AS 消耗数量,
ROUND( COALESCE( po_weighted.total_amt / NULLIF(po_weighted.total_qty,0), last_price.unit_price, 0 ), 2) AS 平均单价,
ROUND( IFNULL(c.consumed_qty,0) * COALESCE( po_weighted.total_amt / NULLIF(po_weighted.total_qty,0), last_price.unit_price, 0 ), 2) AS 消耗金额
FROM items i
LEFT JOIN consumption c ON c.item_id = i.id
LEFT JOIN po_weighted ON po_weighted.item_id = i.id
LEFT JOIN last_price ON last_price.item_id = i.id
WHERE i.active = 1
AND IFNULL(c.consumed_qty,0) > 0
ORDER BY 消耗金额 DESC;
";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        // 包含结束当天的全部记录
                        command.Parameters.Add("@StartDate", MySqlDbType.DateTime).Value = startDate;
                        command.Parameters.Add("@EndDate", MySqlDbType.DateTime).Value = endDate.AddDays(1).AddSeconds(-1);

                        MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                        DataTable rawTable = new DataTable();
                        adapter.Fill(rawTable);

                        if (rawTable.Rows.Count > 0)
                        {
                            ProcessABCClassification(rawTable);
                        }
                        else
                        {
                            LoadMockData();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载数据失败：{ex.Message}，将使用模拟数据演示。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                LoadMockData();
            }
        }

        private void ProcessABCClassification(DataTable rawTable)
        {
            // 计算总金额时更健壮地处理 DBNull / 不同类型
            decimal totalAmount = 0m;
            foreach (DataRow r in rawTable.Rows)
            {
                object val = r["消耗金额"];
                if (val == DBNull.Value || val == null) continue;
                try
                {
                    totalAmount += Convert.ToDecimal(val);
                }
                catch
                {
                    // 如果数据库返回 double 等，尝试转换
                    try { totalAmount += Convert.ToDecimal(Convert.ToDouble(val)); }
                    catch { }
                }
            }

            if (totalAmount == 0)
            {
                LoadMockData();
                return;
            }

            DataTable resultTable = new DataTable();
            resultTable.Columns.Add("物品编码", typeof(string));
            resultTable.Columns.Add("物品名称", typeof(string));
            resultTable.Columns.Add("分类", typeof(string));
            resultTable.Columns.Add("消耗数量", typeof(decimal));
            resultTable.Columns.Add("平均单价", typeof(decimal));
            resultTable.Columns.Add("消耗金额", typeof(decimal));
            resultTable.Columns.Add("金额占比", typeof(decimal));
            resultTable.Columns.Add("累计占比", typeof(decimal));
            resultTable.Columns.Add("ABC分类", typeof(string));
            resultTable.Columns.Add("排名", typeof(int));

            decimal cumulative = 0m;
            int rank = 1;

            foreach (DataRow row in rawTable.Rows)
            {
                decimal amount = 0m;
                object oAmount = row["消耗金额"];
                if (oAmount != DBNull.Value && oAmount != null)
                {
                    try { amount = Convert.ToDecimal(oAmount); }
                    catch { amount = Convert.ToDecimal(Convert.ToDouble(oAmount)); }
                }

                decimal qty = 0m;
                object oQty = row["消耗数量"];
                if (oQty != DBNull.Value && oQty != null)
                {
                    try { qty = Convert.ToDecimal(oQty); }
                    catch { qty = Convert.ToDecimal(Convert.ToDouble(oQty)); }
                }

                decimal avgPrice = 0m;
                object oAvg = row["平均单价"];
                if (oAvg != DBNull.Value && oAvg != null)
                {
                    try { avgPrice = Convert.ToDecimal(oAvg); }
                    catch { avgPrice = Convert.ToDecimal(Convert.ToDouble(oAvg)); }
                }

                decimal ratio = Math.Round(amount / totalAmount * 100m, 1);
                cumulative += ratio;
                decimal cumRatio = Math.Round(cumulative, 1);

                string abcClass = cumRatio <= 80m ? "A" :
                                  cumRatio <= 95m ? "B" : "C";

                resultTable.Rows.Add(
                    row["物品编码"] == DBNull.Value ? "" : row["物品编码"].ToString(),
                    row["物品名称"] == DBNull.Value ? "" : row["物品名称"].ToString(),
                    row["分类"] == DBNull.Value ? "" : row["分类"].ToString(),
                    qty,
                    avgPrice,
                    amount,
                    ratio,
                    cumRatio,
                    abcClass,
                    rank++
                );
            }

            dgvABCAnalysis.DataSource = resultTable;
            GenerateChart(resultTable);
        }

        private void LoadMockData()
        {
            DataTable mockData = new DataTable();
            mockData.Columns.Add("物品编码", typeof(string));
            mockData.Columns.Add("物品名称", typeof(string));
            mockData.Columns.Add("分类", typeof(string));
            mockData.Columns.Add("消耗数量", typeof(decimal));
            mockData.Columns.Add("平均单价", typeof(decimal));
            mockData.Columns.Add("消耗金额", typeof(decimal));
            mockData.Columns.Add("金额占比", typeof(decimal));
            mockData.Columns.Add("累计占比", typeof(decimal));
            mockData.Columns.Add("ABC分类", typeof(string));
            mockData.Columns.Add("排名", typeof(int));

            mockData.Rows.Add("ITEM001", "实验室试剂A", "化学试剂", 150.50m, 25.80m, 3882.90m, 22.5m, 22.5m, "A", 1);
            mockData.Rows.Add("ITEM002", "防护手套", "防护用品", 200.00m, 8.50m, 1700.00m, 9.8m, 32.3m, "A", 2);
            mockData.Rows.Add("ITEM003", "培养皿", "实验器材", 500.00m, 2.80m, 1400.00m, 8.1m, 40.4m, "B", 3);
            mockData.Rows.Add("ITEM004", "移液管", "实验器材", 300.00m, 3.50m, 1050.00m, 6.1m, 46.5m, "B", 4);
            mockData.Rows.Add("ITEM005", "试管", "实验器材", 800.00m, 1.20m, 960.00m, 5.5m, 52.0m, "B", 5);
            mockData.Rows.Add("ITEM006", "离心管", "实验器材", 600.00m, 1.50m, 900.00m, 5.2m, 57.2m, "C", 6);
            mockData.Rows.Add("ITEM007", "滤纸", "耗材", 1000.00m, 0.80m, 800.00m, 4.6m, 61.8m, "C", 7);
            mockData.Rows.Add("ITEM008", "标签纸", "办公用品", 2000.00m, 0.35m, 700.00m, 4.0m, 65.8m, "C", 8);
            mockData.Rows.Add("ITEM009", "记号笔", "办公用品", 150.00m, 4.20m, 630.00m, 3.6m, 69.4m, "C", 9);
            mockData.Rows.Add("ITEM010", "实验服", "防护用品", 30.00m, 20.00m, 600.00m, 3.5m, 72.9m, "C", 10);

            dgvABCAnalysis.DataSource = mockData;
            GenerateChart(mockData);
        }

        private void GenerateChart(DataTable dataTable)
        {
            try
            {
                chartABC.Series.Clear();
                chartABC.Titles.Clear();
                chartABC.ChartAreas.Clear();

                ChartArea chartArea = new ChartArea("ChartArea1");
                chartArea.AxisX.LabelStyle.Angle = -45;
                chartArea.AxisX.Interval = 1;
                chartArea.AxisX.MajorGrid.Enabled = false;
                chartArea.AxisY.MajorGrid.LineColor = Color.LightGray;
                chartArea.AxisY.Title = "消耗金额 (元)";
                chartArea.AxisY2.Maximum = 100;
                chartArea.AxisY2.Minimum = 0;
                chartArea.AxisY2.Title = "累计占比 (%)";
                chartArea.AxisY2.MajorGrid.Enabled = false;
                chartABC.ChartAreas.Add(chartArea);

                chartABC.Titles.Add("ABC分类分析 - 帕累托图");
                chartABC.Titles[0].Font = new Font("微软雅黑", 12, FontStyle.Bold);

                Series barSeries = new Series("消耗金额")
                {
                    ChartType = SeriesChartType.Column,
                    YAxisType = AxisType.Primary
                };

                Series lineSeries = new Series("累计占比")
                {
                    ChartType = SeriesChartType.Line,
                    YAxisType = AxisType.Secondary,
                    Color = Color.Red,
                    BorderWidth = 3,
                    MarkerStyle = MarkerStyle.Circle,
                    MarkerSize = 8,
                    MarkerColor = Color.DarkRed
                };

                foreach (DataRow row in dataTable.Rows)
                {
                    string itemName = row["物品名称"].ToString();
                    decimal consumptionAmount = Convert.ToDecimal(row["消耗金额"]);
                    decimal cumulativePercentage = Convert.ToDecimal(row["累计占比"]);
                    string abcClass = row["ABC分类"].ToString();

                    DataPoint barPoint = new DataPoint { AxisLabel = itemName };
                    barPoint.YValues = new double[] { (double)consumptionAmount };

                    if (abcClass == "A") barPoint.Color = Color.OrangeRed;
                    else if (abcClass == "B") barPoint.Color = Color.Goldenrod;
                    else if (abcClass == "C") barPoint.Color = Color.SteelBlue;

                    barSeries.Points.Add(barPoint);

                    DataPoint linePoint = new DataPoint { AxisLabel = itemName };
                    linePoint.YValues = new double[] { (double)cumulativePercentage };
                    lineSeries.Points.Add(linePoint);
                }

                chartABC.Series.Add(barSeries);
                chartABC.Series.Add(lineSeries);

                chartABC.Legends.Clear();
                Legend legend = new Legend { Docking = Docking.Bottom };
                chartABC.Legends.Add(legend);

                chartABC.Invalidate();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"生成图表时出错：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DgvABCAnalysis_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            foreach (DataGridViewRow row in dgvABCAnalysis.Rows)
            {
                if (!row.IsNewRow)
                {
                    string abcClass = row.Cells["ABC分类"]?.Value?.ToString();

                    if (abcClass == "A")
                    {
                        row.DefaultCellStyle.BackColor = Color.LightCoral;
                        row.DefaultCellStyle.ForeColor = Color.DarkRed;
                    }
                    else if (abcClass == "B")
                    {
                        row.DefaultCellStyle.BackColor = Color.LightYellow;
                        row.DefaultCellStyle.ForeColor = Color.DarkGoldenrod;
                    }
                    else if (abcClass == "C")
                    {
                        row.DefaultCellStyle.BackColor = Color.LightGreen;
                        row.DefaultCellStyle.ForeColor = Color.DarkGreen;
                    }

                    // 设置数字格式
                    FormatCell(row, "消耗数量", "N2");
                    FormatCell(row, "平均单价", "N2");
                    FormatCell(row, "消耗金额", "N2");
                    FormatCell(row, "金额占比", "N1");
                    FormatCell(row, "累计占比", "N1");
                }
            }
        }

        private void FormatCell(DataGridViewRow row, string columnName, string format)
        {
            if (row.Cells[columnName] != null && row.Cells[columnName].Value != null)
                row.Cells[columnName].Style.Format = format;
        }

        private void DgvABCAnalysis_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && !dgvABCAnalysis.Rows[e.RowIndex].IsNewRow)
            {
                DataGridViewRow selectedRow = dgvABCAnalysis.Rows[e.RowIndex];

                string itemName = selectedRow.Cells["物品名称"].Value?.ToString();
                decimal amount = Convert.ToDecimal(selectedRow.Cells["消耗金额"].Value);
                string abcClass = selectedRow.Cells["ABC分类"].Value?.ToString();

                string details = $"物品名称：{itemName}\n" +
                                $"物品编码：{selectedRow.Cells["物品编码"].Value}\n" +
                                $"消耗数量：{selectedRow.Cells["消耗数量"].Value:N2}\n" +
                                $"平均单价：{selectedRow.Cells["平均单价"].Value:N2} 元\n" +
                                $"消耗金额：{amount:N2} 元\n" +
                                $"ABC分类：{abcClass}\n" +
                                $"管理建议：{GetManagementAdvice(abcClass)}";

                MessageBox.Show(details, "物品详情", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private string GetManagementAdvice(string abcClass)
        {
            switch (abcClass)
            {
                case "A":
                    return "重点管理：严格控制库存，定期盘点，重点关注";
                case "B":
                    return "一般管理：常规监控，定期订货";
                case "C":
                    return "简化管理：大批量订货，简化管理流程";
                default:
                    return "普通管理";
            }
        }

        private void ExportToExcel()
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Excel文件|*.xlsx";
                saveFileDialog.Title = "导出ABC分析报表";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    MessageBox.Show("报表导出功能开发中...", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"导出报表时出错：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetupToolTips()
        {
            ToolTip toolTip = new ToolTip
            {
                AutoPopDelay = 5000,
                InitialDelay = 1000,
                ReshowDelay = 500
            };

            toolTip.SetToolTip(btnQuery, "查询并分析耗材ABC分类");
            toolTip.SetToolTip(dtpStartDate, "选择分析的开始日期");
            toolTip.SetToolTip(dtpEndDate, "选择分析的结束日期");
        }

        private void SetupContextMenu()
        {
            ContextMenuStrip contextMenu = new ContextMenuStrip();

            contextMenu.Items.Add("导出报表", null, (s, e) => ExportToExcel());
            contextMenu.Items.Add("刷新数据", null, (s, e) => LoadDefaultData());
            contextMenu.Items.Add("打印报表", null, (s, e) => PrintReport());

            dgvABCAnalysis.ContextMenuStrip = contextMenu;
        }

        private void PrintReport()
        {
            try
            {
                MessageBox.Show("打印功能开发中...", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"打印报表时出错：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void RefreshData()
        {
            LoadDefaultData();
        }

        public DataRow GetSelectedItem()
        {
            if (dgvABCAnalysis.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dgvABCAnalysis.SelectedRows[0];
                if (!selectedRow.IsNewRow)
                {
                    DataTable dataSource = dgvABCAnalysis.DataSource as DataTable;
                    if (dataSource != null && selectedRow.Index < dataSource.Rows.Count)
                    {
                        return dataSource.Rows[selectedRow.Index];
                    }
                }
            }
            return null;
        }

        public DateTime GetStartDate() => dtpStartDate.Value;

        public DateTime GetEndDate() => dtpEndDate.Value;

        private void dgvABCAnalysis_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}