namespace 实验耗材及设备物资管理系统
{
    partial class SupplierPerformanceControl
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        // 布局容器与控件声明（Designer 负责）
        private System.Windows.Forms.Panel mainPanel;
        private System.Windows.Forms.Panel datePanel;
        private System.Windows.Forms.Label lblStartDate;
        private System.Windows.Forms.DateTimePicker dtpStartDate;
        private System.Windows.Forms.Label lblEndDate;
        private System.Windows.Forms.DateTimePicker dtpEndDate;
        private System.Windows.Forms.Button btnApply;

        private System.Windows.Forms.Panel chartPanel;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartSupplierPerformance;

        private System.Windows.Forms.Panel statsPanel;
        private System.Windows.Forms.Label lblTotalSuppliers;
        private System.Windows.Forms.Label lblAvgOnTimeRate;
        private System.Windows.Forms.Label lblAvgDelayDays;
        private System.Windows.Forms.Button btnExport;

        private System.Windows.Forms.Panel detailsPanel;
        private System.Windows.Forms.Label lblDetailsTitle;
        private System.Windows.Forms.ListView lstSupplierDetails;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary> 
        /// 设计器支持所需的方法 - 不要用代码编辑器修改
        /// </summary>
        private void InitializeComponent()
        {
            this.mainPanel = new System.Windows.Forms.Panel();
            this.detailsPanel = new System.Windows.Forms.Panel();
            this.lblDetailsTitle = new System.Windows.Forms.Label();
            this.lstSupplierDetails = new System.Windows.Forms.ListView();
            this.statsPanel = new System.Windows.Forms.Panel();
            this.lblTotalSuppliers = new System.Windows.Forms.Label();
            this.lblAvgOnTimeRate = new System.Windows.Forms.Label();
            this.lblAvgDelayDays = new System.Windows.Forms.Label();
            this.btnExport = new System.Windows.Forms.Button();
            this.chartPanel = new System.Windows.Forms.Panel();
            this.chartSupplierPerformance = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.datePanel = new System.Windows.Forms.Panel();
            this.lblStartDate = new System.Windows.Forms.Label();
            this.dtpStartDate = new System.Windows.Forms.DateTimePicker();
            this.lblEndDate = new System.Windows.Forms.Label();
            this.dtpEndDate = new System.Windows.Forms.DateTimePicker();
            this.btnApply = new System.Windows.Forms.Button();
            this.mainPanel.SuspendLayout();
            this.detailsPanel.SuspendLayout();
            this.statsPanel.SuspendLayout();
            this.chartPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartSupplierPerformance)).BeginInit();
            this.datePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainPanel
            // 
            this.mainPanel.AutoScroll = true;
            this.mainPanel.Controls.Add(this.detailsPanel);
            this.mainPanel.Controls.Add(this.statsPanel);
            this.mainPanel.Controls.Add(this.chartPanel);
            this.mainPanel.Controls.Add(this.datePanel);
            this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainPanel.Location = new System.Drawing.Point(0, 0);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Size = new System.Drawing.Size(1000, 820);
            this.mainPanel.TabIndex = 0;
            this.mainPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.mainPanel_Paint);
            // 
            // detailsPanel
            // 
            this.detailsPanel.BackColor = System.Drawing.Color.White;
            this.detailsPanel.Controls.Add(this.lblDetailsTitle);
            this.detailsPanel.Controls.Add(this.lstSupplierDetails);
            this.detailsPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.detailsPanel.Location = new System.Drawing.Point(0, 557);
            this.detailsPanel.Name = "detailsPanel";
            this.detailsPanel.Padding = new System.Windows.Forms.Padding(8);
            this.detailsPanel.Size = new System.Drawing.Size(979, 302);
            this.detailsPanel.TabIndex = 0;
            // 
            // lblDetailsTitle
            // 
            this.lblDetailsTitle.AutoSize = true;
            this.lblDetailsTitle.Location = new System.Drawing.Point(530, 8);
            this.lblDetailsTitle.Name = "lblDetailsTitle";
            this.lblDetailsTitle.Size = new System.Drawing.Size(142, 15);
            this.lblDetailsTitle.TabIndex = 0;
            this.lblDetailsTitle.Text = "供应商详细绩效数据";
            // 
            // lstSupplierDetails
            // 
            this.lstSupplierDetails.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lstSupplierDetails.FullRowSelect = true;
            this.lstSupplierDetails.GridLines = true;
            this.lstSupplierDetails.HideSelection = false;
            this.lstSupplierDetails.Location = new System.Drawing.Point(269, 32);
            this.lstSupplierDetails.Name = "lstSupplierDetails";
            this.lstSupplierDetails.Size = new System.Drawing.Size(721, 265);
            this.lstSupplierDetails.TabIndex = 1;
            this.lstSupplierDetails.UseCompatibleStateImageBehavior = false;
            this.lstSupplierDetails.View = System.Windows.Forms.View.Details;
            // 
            // statsPanel
            // 
            this.statsPanel.BackColor = System.Drawing.Color.WhiteSmoke;
            this.statsPanel.Controls.Add(this.lblTotalSuppliers);
            this.statsPanel.Controls.Add(this.lblAvgOnTimeRate);
            this.statsPanel.Controls.Add(this.lblAvgDelayDays);
            this.statsPanel.Controls.Add(this.btnExport);
            this.statsPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.statsPanel.Location = new System.Drawing.Point(0, 503);
            this.statsPanel.Name = "statsPanel";
            this.statsPanel.Padding = new System.Windows.Forms.Padding(8);
            this.statsPanel.Size = new System.Drawing.Size(979, 54);
            this.statsPanel.TabIndex = 1;
            // 
            // lblTotalSuppliers
            // 
            this.lblTotalSuppliers.AutoSize = true;
            this.lblTotalSuppliers.Location = new System.Drawing.Point(420, 26);
            this.lblTotalSuppliers.Name = "lblTotalSuppliers";
            this.lblTotalSuppliers.Size = new System.Drawing.Size(106, 15);
            this.lblTotalSuppliers.TabIndex = 0;
            this.lblTotalSuppliers.Text = "供应商数量: 0";
            // 
            // lblAvgOnTimeRate
            // 
            this.lblAvgOnTimeRate.AutoSize = true;
            this.lblAvgOnTimeRate.Location = new System.Drawing.Point(570, 26);
            this.lblAvgOnTimeRate.Name = "lblAvgOnTimeRate";
            this.lblAvgOnTimeRate.Size = new System.Drawing.Size(114, 15);
            this.lblAvgOnTimeRate.TabIndex = 1;
            this.lblAvgOnTimeRate.Text = "平均按时率: 0%";
            // 
            // lblAvgDelayDays
            // 
            this.lblAvgDelayDays.AutoSize = true;
            this.lblAvgDelayDays.Location = new System.Drawing.Point(707, 26);
            this.lblAvgDelayDays.Name = "lblAvgDelayDays";
            this.lblAvgDelayDays.Size = new System.Drawing.Size(160, 15);
            this.lblAvgDelayDays.TabIndex = 2;
            this.lblAvgDelayDays.Text = "平均延迟天数: 0.0 天";
            // 
            // btnExport
            // 
            this.btnExport.Location = new System.Drawing.Point(873, 16);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(100, 32);
            this.btnExport.TabIndex = 3;
            this.btnExport.Text = "导出图表";
            // 
            // chartPanel
            // 
            this.chartPanel.BackColor = System.Drawing.Color.White;
            this.chartPanel.Controls.Add(this.chartSupplierPerformance);
            this.chartPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.chartPanel.Location = new System.Drawing.Point(0, 123);
            this.chartPanel.Name = "chartPanel";
            this.chartPanel.Padding = new System.Windows.Forms.Padding(8);
            this.chartPanel.Size = new System.Drawing.Size(979, 380);
            this.chartPanel.TabIndex = 2;
            // 
            // chartSupplierPerformance
            // 
            this.chartSupplierPerformance.Location = new System.Drawing.Point(269, 0);
            this.chartSupplierPerformance.Name = "chartSupplierPerformance";
            this.chartSupplierPerformance.Size = new System.Drawing.Size(710, 380);
            this.chartSupplierPerformance.TabIndex = 0;
            // 
            // datePanel
            // 
            this.datePanel.BackColor = System.Drawing.Color.White;
            this.datePanel.Controls.Add(this.lblStartDate);
            this.datePanel.Controls.Add(this.dtpStartDate);
            this.datePanel.Controls.Add(this.lblEndDate);
            this.datePanel.Controls.Add(this.dtpEndDate);
            this.datePanel.Controls.Add(this.btnApply);
            this.datePanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.datePanel.Location = new System.Drawing.Point(0, 0);
            this.datePanel.Name = "datePanel";
            this.datePanel.Padding = new System.Windows.Forms.Padding(8);
            this.datePanel.Size = new System.Drawing.Size(979, 123);
            this.datePanel.TabIndex = 3;
            // 
            // lblStartDate
            // 
            this.lblStartDate.AutoSize = true;
            this.lblStartDate.Location = new System.Drawing.Point(451, 100);
            this.lblStartDate.Name = "lblStartDate";
            this.lblStartDate.Size = new System.Drawing.Size(75, 15);
            this.lblStartDate.TabIndex = 0;
            this.lblStartDate.Text = "开始日期:";
            // 
            // dtpStartDate
            // 
            this.dtpStartDate.CustomFormat = "yyyy-MM-dd";
            this.dtpStartDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpStartDate.Location = new System.Drawing.Point(544, 93);
            this.dtpStartDate.Name = "dtpStartDate";
            this.dtpStartDate.Size = new System.Drawing.Size(140, 25);
            this.dtpStartDate.TabIndex = 1;
            // 
            // lblEndDate
            // 
            this.lblEndDate.AutoSize = true;
            this.lblEndDate.Location = new System.Drawing.Point(690, 100);
            this.lblEndDate.Name = "lblEndDate";
            this.lblEndDate.Size = new System.Drawing.Size(75, 15);
            this.lblEndDate.TabIndex = 2;
            this.lblEndDate.Text = "结束日期:";
            // 
            // dtpEndDate
            // 
            this.dtpEndDate.CustomFormat = "yyyy-MM-dd";
            this.dtpEndDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpEndDate.Location = new System.Drawing.Point(771, 93);
            this.dtpEndDate.Name = "dtpEndDate";
            this.dtpEndDate.Size = new System.Drawing.Size(140, 25);
            this.dtpEndDate.TabIndex = 3;
            // 
            // btnApply
            // 
            this.btnApply.Location = new System.Drawing.Point(917, 90);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(80, 28);
            this.btnApply.TabIndex = 4;
            this.btnApply.Text = "应用";
            // 
            // SupplierPerformanceControl
            // 
            this.Controls.Add(this.mainPanel);
            this.Name = "SupplierPerformanceControl";
            this.Size = new System.Drawing.Size(1000, 820);
            this.mainPanel.ResumeLayout(false);
            this.detailsPanel.ResumeLayout(false);
            this.detailsPanel.PerformLayout();
            this.statsPanel.ResumeLayout(false);
            this.statsPanel.PerformLayout();
            this.chartPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chartSupplierPerformance)).EndInit();
            this.datePanel.ResumeLayout(false);
            this.datePanel.PerformLayout();
            this.ResumeLayout(false);

        }
    }
}