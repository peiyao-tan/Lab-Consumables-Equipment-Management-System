namespace WindowsFormsApp1
{
    partial class M4M5ReportForm
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabM4 = new System.Windows.Forms.TabPage();
            this.dgvM4 = new System.Windows.Forms.DataGridView();
            this.tabM5 = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.dgvM5 = new System.Windows.Forms.DataGridView();
            this.chartM5 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.mySqlConnection1 = new MySql.Data.MySqlClient.MySqlConnection();
            this.tabControl1.SuspendLayout();
            this.tabM4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvM4)).BeginInit();
            this.tabM5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvM5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartM5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabM4);
            this.tabControl1.Controls.Add(this.tabM5);
            this.tabControl1.Location = new System.Drawing.Point(329, 79);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1320, 681);
            this.tabControl1.TabIndex = 0;
            // 
            // tabM4
            // 
            this.tabM4.Controls.Add(this.dgvM4);
            this.tabM4.Location = new System.Drawing.Point(4, 25);
            this.tabM4.Name = "tabM4";
            this.tabM4.Padding = new System.Windows.Forms.Padding(3);
            this.tabM4.Size = new System.Drawing.Size(1159, 603);
            this.tabM4.TabIndex = 0;
            this.tabM4.Text = "M4: 近效期与过期风险";
            this.tabM4.UseVisualStyleBackColor = true;
            // 
            // dgvM4
            // 
            this.dgvM4.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvM4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvM4.Location = new System.Drawing.Point(3, 3);
            this.dgvM4.Name = "dgvM4";
            this.dgvM4.RowHeadersWidth = 51;
            this.dgvM4.RowTemplate.Height = 27;
            this.dgvM4.Size = new System.Drawing.Size(1153, 597);
            this.dgvM4.TabIndex = 0;
            // 
            // tabM5
            // 
            this.tabM5.Controls.Add(this.splitContainer1);
            this.tabM5.Location = new System.Drawing.Point(4, 25);
            this.tabM5.Name = "tabM5";
            this.tabM5.Padding = new System.Windows.Forms.Padding(3);
            this.tabM5.Size = new System.Drawing.Size(1312, 652);
            this.tabM5.TabIndex = 1;
            this.tabM5.Text = "M5: 安全库存与缺货预警";
            this.tabM5.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(3, 3);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.dgvM5);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.chartM5);
            this.splitContainer1.Panel2.Controls.Add(this.chart1);
            this.splitContainer1.Size = new System.Drawing.Size(1306, 646);
            this.splitContainer1.SplitterDistance = 434;
            this.splitContainer1.TabIndex = 2;
            // 
            // dgvM5
            // 
            this.dgvM5.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvM5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvM5.Location = new System.Drawing.Point(0, 0);
            this.dgvM5.Name = "dgvM5";
            this.dgvM5.RowHeadersWidth = 51;
            this.dgvM5.RowTemplate.Height = 27;
            this.dgvM5.Size = new System.Drawing.Size(434, 646);
            this.dgvM5.TabIndex = 0;
            // 
            // chartM5
            // 
            chartArea1.Name = "ChartArea1";
            this.chartM5.ChartAreas.Add(chartArea1);
            this.chartM5.Dock = System.Windows.Forms.DockStyle.Fill;
            legend1.Name = "Legend1";
            this.chartM5.Legends.Add(legend1);
            this.chartM5.Location = new System.Drawing.Point(0, 0);
            this.chartM5.Name = "chartM5";
            series1.ChartArea = "ChartArea1";
            series1.Legend = "Legend1";
            series1.Name = "物资缺货量";
            this.chartM5.Series.Add(series1);
            this.chartM5.Size = new System.Drawing.Size(868, 646);
            this.chartM5.TabIndex = 1;
            this.chartM5.Text = "chart2";
            this.chartM5.Click += new System.EventHandler(this.chartM5_Click);
            // 
            // chart1
            // 
            this.chart1.Location = new System.Drawing.Point(245, 259);
            this.chart1.Name = "chart1";
            this.chart1.Size = new System.Drawing.Size(300, 300);
            this.chart1.TabIndex = 0;
            this.chart1.Text = "chart1";
            // 
            // M4M5ReportForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabControl1);
            this.Name = "M4M5ReportForm";
            this.Size = new System.Drawing.Size(1675, 760);
            this.tabControl1.ResumeLayout(false);
            this.tabM4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvM4)).EndInit();
            this.tabM5.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvM5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartM5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabM4;
        private System.Windows.Forms.TabPage tabM5;
        private MySql.Data.MySqlClient.MySqlConnection mySqlConnection1;
        private System.Windows.Forms.DataGridView dgvM4;
        private System.Windows.Forms.DataGridView dgvM5;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartM5;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
    }
}
