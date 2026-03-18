namespace WindowsFormsApp1
{
    partial class ReportMonthlyUsageTrend
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.usageChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            ((System.ComponentModel.ISupportInitialize)(this.usageChart)).BeginInit();
            this.SuspendLayout();
            // 
            // usageChart
            // 
            chartArea1.Name = "ChartArea1";
            this.usageChart.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.usageChart.Legends.Add(legend1);
            this.usageChart.Location = new System.Drawing.Point(393, 225);
            this.usageChart.Name = "usageChart";
            series1.ChartArea = "ChartArea1";
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            this.usageChart.Series.Add(series1);
            this.usageChart.Size = new System.Drawing.Size(840, 413);
            this.usageChart.TabIndex = 0;
            this.usageChart.Text = "chart1";
            // 
            // ReportMonthlyUsageTrend
            // 
            this.Controls.Add(this.usageChart);
            this.Name = "ReportMonthlyUsageTrend";
            this.Size = new System.Drawing.Size(1233, 641);
            ((System.ComponentModel.ISupportInitialize)(this.usageChart)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart usageChart;
    }
}