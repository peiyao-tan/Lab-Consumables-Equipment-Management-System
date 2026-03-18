namespace WindowsFormsApp1
{
    partial class LabListForm
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
            this.dgvLabs = new System.Windows.Forms.DataGridView();
            this.btnNewLab = new System.Windows.Forms.Button();
            this.btnViewMembers = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLabs)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvLabs
            // 
            this.dgvLabs.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvLabs.Location = new System.Drawing.Point(371, 138);
            this.dgvLabs.Name = "dgvLabs";
            this.dgvLabs.RowHeadersWidth = 51;
            this.dgvLabs.RowTemplate.Height = 27;
            this.dgvLabs.Size = new System.Drawing.Size(1063, 660);
            this.dgvLabs.TabIndex = 0;
            // 
            // btnNewLab
            // 
            this.btnNewLab.Location = new System.Drawing.Point(656, 100);
            this.btnNewLab.Name = "btnNewLab";
            this.btnNewLab.Size = new System.Drawing.Size(112, 32);
            this.btnNewLab.TabIndex = 1;
            this.btnNewLab.Text = "新建实验室";
            this.btnNewLab.UseVisualStyleBackColor = true;
            // 
            // btnViewMembers
            // 
            this.btnViewMembers.Location = new System.Drawing.Point(826, 100);
            this.btnViewMembers.Name = "btnViewMembers";
            this.btnViewMembers.Size = new System.Drawing.Size(112, 32);
            this.btnViewMembers.TabIndex = 2;
            this.btnViewMembers.Text = "查看成员";
            this.btnViewMembers.UseVisualStyleBackColor = true;
            // 
            // LabListForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnViewMembers);
            this.Controls.Add(this.btnNewLab);
            this.Controls.Add(this.dgvLabs);
            this.Name = "LabListForm";
            this.Size = new System.Drawing.Size(1669, 827);
            ((System.ComponentModel.ISupportInitialize)(this.dgvLabs)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvLabs;
        private System.Windows.Forms.Button btnNewLab;
        private System.Windows.Forms.Button btnViewMembers;
    }
}
