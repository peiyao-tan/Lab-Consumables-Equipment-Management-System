using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting; 
using 实验耗材及设备物资管理系统;

namespace WindowsFormsApp1
{
    public partial class M4M5ReportForm : UserControl
    {
        private DatabaseHelper dbHelper;

        // 构造函数：接收 dbHelper
        public M4M5ReportForm(DatabaseHelper db)
        {
            InitializeComponent(); // ✅ 现在这是设计器生成的，不会冲突
            dbHelper = db;
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                LoadM4Data();
                LoadM5Data();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载报表失败:\n{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadM4Data()
        {
            string sql = @"
                SELECT 
                    item_code AS '物资编码',
                    item_name AS '物资名称',
                    category AS '分类',
                    total_available_qty AS '总可用库存',
                    near_expiry_qty AS '近效期数量',
                    expired_qty AS '过期数量',
                    (near_expiry_qty + expired_qty) AS '风险总量'
                FROM inventory_risk_view
                WHERE near_expiry_qty > 0 OR expired_qty > 0
                ORDER BY expired_qty DESC, near_expiry_qty DESC";

            DataTable dt = dbHelper.ExecuteQuery(sql);
            dgvM4.DataSource = dt;

            // 高亮过期行
            foreach (DataGridViewRow row in dgvM4.Rows)
            {
                if (row.Cells["过期数量"].Value != null &&
                    int.TryParse(row.Cells["过期数量"].Value.ToString(), out int expired) &&
                    expired > 0)
                {
                    row.DefaultCellStyle.BackColor = Color.LightCoral;
                }
            }
        }

        private void LoadM5Data()
        {
            string sql = @"
                SELECT 
                    item_code AS '物资编码',
                    item_name AS '物资名称',
                    category AS '分类',
                    total_available_qty AS '当前库存',
                    reorder_point AS '订货点',
                    min_stock AS '安全库存',
                    (reorder_point - total_available_qty) AS '缺货量'
                FROM inventory_risk_view
                WHERE total_available_qty < reorder_point
                ORDER BY `缺货量` DESC";

            DataTable dt = dbHelper.ExecuteQuery(sql);
            dgvM5.DataSource = dt;

            // 清空图表并重绘
            chartM5.Series[0].Points.Clear();
            foreach (DataRow row in dt.Rows)
            {
                string name = row["物资名称"].ToString();
                if (double.TryParse(row["缺货量"].ToString(), out double shortage) && shortage > 0)
                {
                    chartM5.Series[0].Points.AddXY(name, shortage);
                }
            }

            // 优化显示
            chartM5.ChartAreas[0].AxisX.LabelStyle.Angle = -45;
            chartM5.ChartAreas[0].AxisX.Interval = 1;
        }

        private void chartM5_Click(object sender, EventArgs e)
        {

        }
    }
}