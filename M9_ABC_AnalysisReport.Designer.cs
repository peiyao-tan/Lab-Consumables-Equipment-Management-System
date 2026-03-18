namespace WindowsFormsApp1
{
    partial class M9_ABC_AnalysisReport
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
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.mainTableLayout = new System.Windows.Forms.TableLayoutPanel();
            this.criteriaPanel = new System.Windows.Forms.Panel();
            this.btnQuery = new System.Windows.Forms.Button();
            this.dtpEndDate = new System.Windows.Forms.DateTimePicker();
            this.lblEndDate = new System.Windows.Forms.Label();
            this.dtpStartDate = new System.Windows.Forms.DateTimePicker();
            this.lblStartDate = new System.Windows.Forms.Label();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.dgvABCAnalysis = new System.Windows.Forms.DataGridView();
            this.chartABC = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.mainTableLayout.SuspendLayout();
            this.criteriaPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvABCAnalysis)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartABC)).BeginInit();
            this.SuspendLayout();
            // 
            // mainTableLayout
            // 
            this.mainTableLayout.ColumnCount = 1;
            this.mainTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainTableLayout.Controls.Add(this.splitContainer, 0, 1);
            this.mainTableLayout.Controls.Add(this.criteriaPanel, 0, 0);
            this.mainTableLayout.Location = new System.Drawing.Point(366, 0);
            this.mainTableLayout.Name = "mainTableLayout";
            this.mainTableLayout.RowCount = 2;
            this.mainTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 180F));
            this.mainTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainTableLayout.Size = new System.Drawing.Size(1229, 751);
            this.mainTableLayout.TabIndex = 0;
            // 
            // criteriaPanel
            // 
            this.criteriaPanel.BackColor = System.Drawing.Color.White;
            this.criteriaPanel.Controls.Add(this.btnQuery);
            this.criteriaPanel.Controls.Add(this.dtpEndDate);
            this.criteriaPanel.Controls.Add(this.lblEndDate);
            this.criteriaPanel.Controls.Add(this.dtpStartDate);
            this.criteriaPanel.Controls.Add(this.lblStartDate);
            this.criteriaPanel.Location = new System.Drawing.Point(3, 3);
            this.criteriaPanel.Name = "criteriaPanel";
            this.criteriaPanel.Size = new System.Drawing.Size(1223, 174);
            this.criteriaPanel.TabIndex = 0;
            // 
            // btnQuery
            // 
            this.btnQuery.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(123)))), ((int)(((byte)(255)))));
            this.btnQuery.FlatAppearance.BorderSize = 0;
            this.btnQuery.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnQuery.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnQuery.ForeColor = System.Drawing.Color.White;
            this.btnQuery.Location = new System.Drawing.Point(1113, 120);
            this.btnQuery.Name = "btnQuery";
            this.btnQuery.Size = new System.Drawing.Size(90, 28);
            this.btnQuery.TabIndex = 4;
            this.btnQuery.Text = "查询分析";
            this.btnQuery.UseVisualStyleBackColor = false;
            // 
            // dtpEndDate
            // 
            this.dtpEndDate.CustomFormat = "yyyy-MM-dd";
            this.dtpEndDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpEndDate.Location = new System.Drawing.Point(961, 123);
            this.dtpEndDate.Name = "dtpEndDate";
            this.dtpEndDate.Size = new System.Drawing.Size(120, 25);
            this.dtpEndDate.TabIndex = 3;
            // 
            // lblEndDate
            // 
            this.lblEndDate.AutoSize = true;
            this.lblEndDate.Location = new System.Drawing.Point(870, 130);
            this.lblEndDate.Name = "lblEndDate";
            this.lblEndDate.Size = new System.Drawing.Size(75, 15);
            this.lblEndDate.TabIndex = 2;
            this.lblEndDate.Text = "结束日期:";
            // 
            // dtpStartDate
            // 
            this.dtpStartDate.CustomFormat = "yyyy-MM-dd";
            this.dtpStartDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpStartDate.Location = new System.Drawing.Point(733, 123);
            this.dtpStartDate.Name = "dtpStartDate";
            this.dtpStartDate.Size = new System.Drawing.Size(120, 25);
            this.dtpStartDate.TabIndex = 1;
            // 
            // lblStartDate
            // 
            this.lblStartDate.AutoSize = true;
            this.lblStartDate.Location = new System.Drawing.Point(640, 130);
            this.lblStartDate.Name = "lblStartDate";
            this.lblStartDate.Size = new System.Drawing.Size(75, 15);
            this.lblStartDate.TabIndex = 0;
            this.lblStartDate.Text = "开始日期:";
            // 
            // splitContainer
            // 
            this.splitContainer.Location = new System.Drawing.Point(3, 183);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.dgvABCAnalysis);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.chartABC);
            this.splitContainer.Size = new System.Drawing.Size(1223, 517);
            this.splitContainer.SplitterDistance = 489;
            this.splitContainer.TabIndex = 1;
            // 
            // dgvABCAnalysis
            // 
            this.dgvABCAnalysis.AllowUserToAddRows = false;
            this.dgvABCAnalysis.AllowUserToDeleteRows = false;
            this.dgvABCAnalysis.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvABCAnalysis.BackgroundColor = System.Drawing.Color.White;
            this.dgvABCAnalysis.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvABCAnalysis.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvABCAnalysis.Location = new System.Drawing.Point(0, 0);
            this.dgvABCAnalysis.Name = "dgvABCAnalysis";
            this.dgvABCAnalysis.ReadOnly = true;
            this.dgvABCAnalysis.RowHeadersVisible = false;
            this.dgvABCAnalysis.RowHeadersWidth = 51;
            this.dgvABCAnalysis.RowTemplate.Height = 27;
            this.dgvABCAnalysis.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvABCAnalysis.Size = new System.Drawing.Size(489, 517);
            this.dgvABCAnalysis.TabIndex = 0;
            this.dgvABCAnalysis.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvABCAnalysis_CellContentClick);
            // 
            // chartABC
            // 
            chartArea1.AxisX.Interval = 1D;
            chartArea1.AxisX.LabelStyle.Angle = -45;
            chartArea1.AxisX.LabelStyle.Font = new System.Drawing.Font("微软雅黑", 8F);
            chartArea1.AxisX.MajorGrid.Enabled = false;
            chartArea1.AxisY.MajorGrid.LineColor = System.Drawing.Color.LightGray;
            chartArea1.AxisY.Title = "消耗金额 (元)";
            chartArea1.AxisY2.MajorGrid.Enabled = false;
            chartArea1.AxisY2.Maximum = 100D;
            chartArea1.AxisY2.Minimum = 0D;
            chartArea1.AxisY2.Title = "累计占比 (%)";
            chartArea1.Name = "ChartArea1";
            this.chartABC.ChartAreas.Add(chartArea1);
            this.chartABC.Dock = System.Windows.Forms.DockStyle.Fill;
            legend1.Name = "Legend1";
            this.chartABC.Legends.Add(legend1);
            this.chartABC.Location = new System.Drawing.Point(0, 0);
            this.chartABC.Name = "chartABC";
            series1.ChartArea = "ChartArea1";
            series1.Legend = "Legend1";
            series1.Name = "消耗金额";
            series2.ChartArea = "ChartArea1";
            series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series2.Legend = "Legend1";
            series2.Name = "累计占比";
            series2.YAxisType = System.Windows.Forms.DataVisualization.Charting.AxisType.Secondary;
            this.chartABC.Series.Add(series1);
            this.chartABC.Series.Add(series2);
            this.chartABC.Size = new System.Drawing.Size(730, 517);
            this.chartABC.TabIndex = 0;
            this.chartABC.Text = "chart1";
            // 
            // M9_ABC_AnalysisReport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mainTableLayout);
            this.Name = "M9_ABC_AnalysisReport";
            this.Size = new System.Drawing.Size(1598, 847);
            this.mainTableLayout.ResumeLayout(false);
            this.criteriaPanel.ResumeLayout(false);
            this.criteriaPanel.PerformLayout();
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvABCAnalysis)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartABC)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel mainTableLayout;
        private System.Windows.Forms.Panel criteriaPanel;
        private System.Windows.Forms.Button btnQuery;
        private System.Windows.Forms.DateTimePicker dtpEndDate;
        private System.Windows.Forms.Label lblEndDate;
        private System.Windows.Forms.DateTimePicker dtpStartDate;
        private System.Windows.Forms.Label lblStartDate;
        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.DataGridView dgvABCAnalysis;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartABC;
    }
}