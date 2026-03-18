namespace 实验耗材及设备物资管理系统
{
    partial class DeviceUtilizationReport
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
            if (disposing)
            {
                components?.Dispose();
                dbHelper?.Dispose();
                _chart?.Dispose();
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
            this.SuspendLayout();
            // 
            // DeviceUtilizationReport
            // 
            this.Name = "DeviceUtilizationReport";
            this.Size = new System.Drawing.Size(1459, 837);
            this.Load += new System.EventHandler(this.DeviceUtilizationReport_Load);
            this.ResumeLayout(false);

        }

        #endregion
    }
}