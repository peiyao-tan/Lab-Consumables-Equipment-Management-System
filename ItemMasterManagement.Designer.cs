namespace WindowsFormsApp1
{
    partial class ItemMasterManagement
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
            this.mySqlConnection1 = new MySql.Data.MySqlClient.MySqlConnection();
            this.txt_SearchKeyword = new System.Windows.Forms.TextBox();
            this.cbo_ItemType = new System.Windows.Forms.ComboBox();
            this.btn_Search = new System.Windows.Forms.Button();
            this.btnEditItem = new System.Windows.Forms.Button();
            this.dgv_ItemList = new System.Windows.Forms.DataGridView();
            this.pnl_SearchArea = new System.Windows.Forms.Panel();
            this.pnl_EditArea = new System.Windows.Forms.Panel();
            this.chk_Active = new System.Windows.Forms.CheckBox();
            this.btnExportItems = new System.Windows.Forms.Button();
            this.btnImportItems = new System.Windows.Forms.Button();
            this.btnNewItem = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_ItemList)).BeginInit();
            this.pnl_SearchArea.SuspendLayout();
            this.pnl_EditArea.SuspendLayout();
            this.SuspendLayout();
            // 
            // txt_SearchKeyword
            // 
            this.txt_SearchKeyword.Location = new System.Drawing.Point(18, 59);
            this.txt_SearchKeyword.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txt_SearchKeyword.Name = "txt_SearchKeyword";
            this.txt_SearchKeyword.Size = new System.Drawing.Size(112, 28);
            this.txt_SearchKeyword.TabIndex = 0;
            // 
            // cbo_ItemType
            // 
            this.cbo_ItemType.FormattingEnabled = true;
            this.cbo_ItemType.Location = new System.Drawing.Point(137, 59);
            this.cbo_ItemType.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cbo_ItemType.Name = "cbo_ItemType";
            this.cbo_ItemType.Size = new System.Drawing.Size(136, 26);
            this.cbo_ItemType.TabIndex = 1;
            this.cbo_ItemType.Text = "物资类型";
            // 
            // btn_Search
            // 
            this.btn_Search.Location = new System.Drawing.Point(282, 59);
            this.btn_Search.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btn_Search.Name = "btn_Search";
            this.btn_Search.Size = new System.Drawing.Size(84, 30);
            this.btn_Search.TabIndex = 2;
            this.btn_Search.Text = "搜索";
            this.btn_Search.UseVisualStyleBackColor = true;
            // 
            // btnEditItem
            // 
            this.btnEditItem.Location = new System.Drawing.Point(423, 83);
            this.btnEditItem.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnEditItem.Name = "btnEditItem";
            this.btnEditItem.Size = new System.Drawing.Size(124, 31);
            this.btnEditItem.TabIndex = 4;
            this.btnEditItem.Text = "编辑物资";
            this.btnEditItem.UseVisualStyleBackColor = true;
            // 
            // dgv_ItemList
            // 
            this.dgv_ItemList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_ItemList.Location = new System.Drawing.Point(406, 248);
            this.dgv_ItemList.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.dgv_ItemList.Name = "dgv_ItemList";
            this.dgv_ItemList.RowHeadersWidth = 51;
            this.dgv_ItemList.RowTemplate.Height = 27;
            this.dgv_ItemList.Size = new System.Drawing.Size(1539, 605);
            this.dgv_ItemList.TabIndex = 9;
            // 
            // pnl_SearchArea
            // 
            this.pnl_SearchArea.Controls.Add(this.btn_Search);
            this.pnl_SearchArea.Controls.Add(this.txt_SearchKeyword);
            this.pnl_SearchArea.Controls.Add(this.cbo_ItemType);
            this.pnl_SearchArea.Location = new System.Drawing.Point(407, 109);
            this.pnl_SearchArea.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.pnl_SearchArea.Name = "pnl_SearchArea";
            this.pnl_SearchArea.Size = new System.Drawing.Size(385, 114);
            this.pnl_SearchArea.TabIndex = 10;
            // 
            // pnl_EditArea
            // 
            this.pnl_EditArea.Controls.Add(this.btnEditItem);
            this.pnl_EditArea.Controls.Add(this.chk_Active);
            this.pnl_EditArea.Controls.Add(this.btnExportItems);
            this.pnl_EditArea.Controls.Add(this.btnImportItems);
            this.pnl_EditArea.Controls.Add(this.btnNewItem);
            this.pnl_EditArea.Location = new System.Drawing.Point(819, 85);
            this.pnl_EditArea.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.pnl_EditArea.Name = "pnl_EditArea";
            this.pnl_EditArea.Size = new System.Drawing.Size(702, 156);
            this.pnl_EditArea.TabIndex = 11;
            // 
            // chk_Active
            // 
            this.chk_Active.AutoSize = true;
            this.chk_Active.Location = new System.Drawing.Point(577, 90);
            this.chk_Active.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.chk_Active.Name = "chk_Active";
            this.chk_Active.Size = new System.Drawing.Size(70, 22);
            this.chk_Active.TabIndex = 12;
            this.chk_Active.Text = "启用";
            this.chk_Active.UseVisualStyleBackColor = true;
            // 
            // btnExportItems
            // 
            this.btnExportItems.Location = new System.Drawing.Point(292, 82);
            this.btnExportItems.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnExportItems.Name = "btnExportItems";
            this.btnExportItems.Size = new System.Drawing.Size(124, 30);
            this.btnExportItems.TabIndex = 11;
            this.btnExportItems.Text = "导出excel";
            this.btnExportItems.UseVisualStyleBackColor = true;
            // 
            // btnImportItems
            // 
            this.btnImportItems.Location = new System.Drawing.Point(169, 82);
            this.btnImportItems.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnImportItems.Name = "btnImportItems";
            this.btnImportItems.Size = new System.Drawing.Size(117, 31);
            this.btnImportItems.TabIndex = 10;
            this.btnImportItems.Text = "批量导入";
            this.btnImportItems.UseVisualStyleBackColor = true;
            // 
            // btnNewItem
            // 
            this.btnNewItem.Location = new System.Drawing.Point(38, 83);
            this.btnNewItem.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnNewItem.Name = "btnNewItem";
            this.btnNewItem.Size = new System.Drawing.Size(124, 31);
            this.btnNewItem.TabIndex = 9;
            this.btnNewItem.Text = "新增物资";
            this.btnNewItem.UseVisualStyleBackColor = true;
            // 
            // ItemMasterManagement
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnl_EditArea);
            this.Controls.Add(this.pnl_SearchArea);
            this.Controls.Add(this.dgv_ItemList);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "ItemMasterManagement";
            this.Size = new System.Drawing.Size(1904, 908);
            this.Load += new System.EventHandler(this.ItemMasterManagement_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgv_ItemList)).EndInit();
            this.pnl_SearchArea.ResumeLayout(false);
            this.pnl_SearchArea.PerformLayout();
            this.pnl_EditArea.ResumeLayout(false);
            this.pnl_EditArea.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private MySql.Data.MySqlClient.MySqlConnection mySqlConnection1;
        private System.Windows.Forms.TextBox txt_SearchKeyword;
        private System.Windows.Forms.ComboBox cbo_ItemType;
        private System.Windows.Forms.Button btn_Search;
        private System.Windows.Forms.Button btnEditItem;
        private System.Windows.Forms.DataGridView dgv_ItemList;
        private System.Windows.Forms.Panel pnl_SearchArea;
        private System.Windows.Forms.Panel pnl_EditArea;
        private System.Windows.Forms.CheckBox chk_Active;
        private System.Windows.Forms.Button btnExportItems;
        private System.Windows.Forms.Button btnImportItems;
        private System.Windows.Forms.Button btnNewItem;
    }
}
