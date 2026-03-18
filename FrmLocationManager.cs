using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace WindowsFormsApp1
{
    public partial class FrmLocationManager : UserControl
    {
        // ⚠️ 替换为你的数据库连接字符串
        private string connectionString = "server=localhost;user=root;password=123456;database=try;";

        private string editingId = ""; // 用字符串存储当前编辑的ID

        public FrmLocationManager()
        {
            InitializeComponent();
            InitializeUI();
            LoadLocationTree(); // 加载树形结构时不再解析Guid
        }

        private void InitializeUI()
        {
            // 初始化库位类型下拉框（根据实际业务调整）
            cmbType.Items.AddRange(new string[] { "warehouse", "zone", "rack", "bin" });
            cmbType.SelectedIndex = 0;

            // 绑定事件
            tvLocations.AfterSelect += tvLocations_AfterSelect;
            btnSave.Click += btnSave_Click;
            btnNew.Click += btnNew_Click;
            btnDelete.Click += btnDelete_Click;
        }

        // 数据模型：所有ID用string，不依赖Guid
        public class Location
        {
            public string Id { get; set; }          // 数据库ID（字符串格式）
            public string Code { get; set; }        // 库位编码
            public string Name { get; set; }        // 库位名称
            public string Type { get; set; }        // 库位类型（warehouse/zone等）
            public string ParentId { get; set; }    // 上级ID（可为空字符串）
            public string TemperatureRange { get; set; } // 温控范围
            public string HazardClass { get; set; } // 危化分类
            public bool Active { get; set; }        // 是否启用
        }

        // 获取所有库位（不解析Guid，直接用字符串）
        private List<Location> GetAllLocations()
        {
            var list = new List<Location>();
            string sql = "SELECT id, code, name, type, parent_id, temperature_range, hazard_class, active FROM locations ORDER BY code";
            using (var conn = new MySqlConnection(connectionString))
            using (var cmd = new MySqlCommand(sql, conn))
            {
                try
                {
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new Location
                            {
                                // 直接读取字符串，不做Guid转换
                                Id = reader["id"].ToString(),
                                Code = reader["code"].ToString(),
                                Name = reader["name"].ToString(),
                                Type = reader["type"].ToString(),
                                // 处理空值：数据库NULL转为null字符串
                                ParentId = reader["parent_id"] == DBNull.Value ? null : reader["parent_id"].ToString(),
                                TemperatureRange = reader["temperature_range"] == DBNull.Value ? null : reader["temperature_range"].ToString(),
                                HazardClass = reader["hazard_class"] == DBNull.Value ? null : reader["hazard_class"].ToString(),
                                Active = Convert.ToBoolean(reader["active"])
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"加载库位失败：{ex.Message}", "错误");
                }
            }
            return list;
        }

        // 根据ID获取单个库位
        private Location GetLocationById(string id)
        {
            string sql = "SELECT id, code, name, type, parent_id, temperature_range, hazard_class, active FROM locations WHERE id = @id";
            using (var conn = new MySqlConnection(connectionString))
            using (var cmd = new MySqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                try
                {
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Location
                            {
                                Id = reader["id"].ToString(),
                                Code = reader["code"].ToString(),
                                Name = reader["name"].ToString(),
                                Type = reader["type"].ToString(),
                                ParentId = reader["parent_id"] == DBNull.Value ? null : reader["parent_id"].ToString(),
                                TemperatureRange = reader["temperature_range"] == DBNull.Value ? null : reader["temperature_range"].ToString(),
                                HazardClass = reader["hazard_class"] == DBNull.Value ? null : reader["hazard_class"].ToString(),
                                Active = Convert.ToBoolean(reader["active"])
                            };
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"获取库位详情失败：{ex.Message}", "错误");
                }
            }
            return null;
        }

        // 检查编码是否已存在（排除当前编辑的ID）
        private bool IsCodeExists(string code, string excludeId)
        {
            string sql = "SELECT COUNT(1) FROM locations WHERE code = @code";
            if (!string.IsNullOrEmpty(excludeId))
                sql += " AND id != @excludeId";

            using (var conn = new MySqlConnection(connectionString))
            using (var cmd = new MySqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@code", code);
                if (!string.IsNullOrEmpty(excludeId))
                    cmd.Parameters.AddWithValue("@excludeId", excludeId);
                try
                {
                    conn.Open();
                    return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"检查编码失败：{ex.Message}", "错误");
                    return true; // 失败时默认视为已存在，避免重复
                }
            }
        }

        // 保存库位（新增或修改）
        private void SaveLocation(Location loc)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    using (var tran = conn.BeginTransaction())
                    {
                        string sql;
                        if (string.IsNullOrEmpty(loc.Id))
                        {
                            // 新增：生成唯一ID（用Guid字符串，避免冲突但不解析）
                            loc.Id = Guid.NewGuid().ToString();
                            sql = @"
                                INSERT INTO locations (id, code, name, type, parent_id, temperature_range, hazard_class, active)
                                VALUES (@id, @code, @name, @type, @parentId, @tempRange, @hazardClass, @active)";
                        }
                        else
                        {
                            // 修改：更新现有记录
                            sql = @"
                                UPDATE locations 
                                SET code = @code, name = @name, type = @type, parent_id = @parentId, 
                                    temperature_range = @tempRange, hazard_class = @hazardClass, active = @active
                                WHERE id = @id";
                        }

                        using (var cmd = new MySqlCommand(sql, conn, tran))
                        {
                            cmd.Parameters.AddWithValue("@id", loc.Id);
                            cmd.Parameters.AddWithValue("@code", loc.Code);
                            cmd.Parameters.AddWithValue("@name", loc.Name);
                            cmd.Parameters.AddWithValue("@type", loc.Type);
                            // 处理空值：null字符串转为数据库NULL
                            cmd.Parameters.AddWithValue("@parentId", string.IsNullOrEmpty(loc.ParentId) ? (object)DBNull.Value : loc.ParentId);
                            cmd.Parameters.AddWithValue("@tempRange", string.IsNullOrEmpty(loc.TemperatureRange) ? (object)DBNull.Value : loc.TemperatureRange);
                            cmd.Parameters.AddWithValue("@hazardClass", string.IsNullOrEmpty(loc.HazardClass) ? (object)DBNull.Value : loc.HazardClass);
                            cmd.Parameters.AddWithValue("@active", loc.Active);

                            cmd.ExecuteNonQuery();
                            tran.Commit();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"保存失败：{ex.Message}", "错误");
                }
            }
        }

        // 检查是否有子节点
        private bool HasChildren(string parentId)
        {
            string sql = "SELECT COUNT(1) FROM locations WHERE parent_id = @parentId";
            using (var conn = new MySqlConnection(connectionString))
            using (var cmd = new MySqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@parentId", parentId);
                try
                {
                    conn.Open();
                    return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"检查子节点失败：{ex.Message}", "错误");
                    return true; // 失败时默认视为有子节点，避免误删
                }
            }
        }

        // 检查库位是否被使用
        private bool IsLocationInUse(string locationId)
        {
            // 假设关联表stock_batches的current_location_id字段引用库位ID
            string sql = "SELECT COUNT(1) FROM stock_batches WHERE current_location_id = @id";
            using (var conn = new MySqlConnection(connectionString))
            using (var cmd = new MySqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@id", locationId);
                try
                {
                    conn.Open();
                    return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"检查使用状态失败：{ex.Message}", "错误");
                    return true; // 失败时默认视为被使用，避免误删
                }
            }
        }

        // 删除库位
        private void DeleteLocation(string id)
        {
            string sql = "DELETE FROM locations WHERE id = @id";
            using (var conn = new MySqlConnection(connectionString))
            using (var cmd = new MySqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"删除失败：{ex.Message}", "错误");
                }
            }
        }

        // 加载树形结构（用string ID关联）
        private void LoadLocationTree()
        {
            tvLocations.Nodes.Clear();
            var allLocations = GetAllLocations();
            if (allLocations.Count == 0) return;

            // 用字典存储ID与Location的映射（string为键）
            var locationDict = allLocations.ToDictionary(x => x.Id);

            // 先加载根节点（无上级的库位）
            foreach (var loc in allLocations.Where(x => string.IsNullOrEmpty(x.ParentId)))
            {
                var node = new TreeNode(loc.Name) { Tag = loc.Id }; // Tag存储string ID
                BuildTreeRecursive(node, locationDict);
                tvLocations.Nodes.Add(node);
            }
            tvLocations.ExpandAll();
        }

        // 递归构建子节点
        private void BuildTreeRecursive(TreeNode parentNode, Dictionary<string, Location> dict)
        {
            // 查找当前节点的子节点（ParentId等于当前节点的ID）
            var children = dict.Values.Where(x => x.ParentId == parentNode.Tag.ToString()).ToList();
            foreach (var child in children)
            {
                var node = new TreeNode(child.Name) { Tag = child.Id };
                parentNode.Nodes.Add(node);
                BuildTreeRecursive(node, dict); // 递归加载孙子节点
            }
        }

        // 加载上级库位下拉框（排除当前编辑的ID，避免自关联）
        private void LoadParentComboBox(string excludeId = null)
        {
            var allLocations = GetAllLocations();
            var filtered = allLocations.Where(x => x.Id != excludeId).ToList();

            cmbParent.DisplayMember = "Name"; // 显示名称
            cmbParent.ValueMember = "Id";     // 实际值为ID（string）
            cmbParent.DataSource = filtered;
        }

        // 清空表单
        private void ClearForm()
        {
            editingId = "";
            txtCode.Clear();
            txtName.Clear();
            cmbType.SelectedIndex = 0;
            cmbParent.SelectedIndex = -1;
            txtTempRange.Clear();
            txtHazardClass.Clear();
            chkActive.Checked = true;
        }

        // 树形控件选择事件：加载选中库位的详情
        private void tvLocations_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node?.Tag == null) return;

            string id = e.Node.Tag.ToString(); // 获取string类型的ID
            var loc = GetLocationById(id);
            if (loc == null) return;

            // 填充表单
            editingId = id;
            txtCode.Text = loc.Code;
            txtName.Text = loc.Name;
            cmbType.SelectedItem = loc.Type;
            chkActive.Checked = loc.Active;
            txtTempRange.Text = loc.TemperatureRange ?? "";
            txtHazardClass.Text = loc.HazardClass ?? "";

            // 加载上级下拉框（排除当前ID）
            LoadParentComboBox(excludeId: id);

            // 选中上级ID（如果有）
            if (!string.IsNullOrEmpty(loc.ParentId))
            {
                cmbParent.SelectedValue = loc.ParentId;
            }
            else
            {
                cmbParent.SelectedIndex = -1;
            }
        }

        // 保存按钮点击事件
        private void btnSave_Click(object sender, EventArgs e)
        {
            // 校验必填项
            if (string.IsNullOrWhiteSpace(txtCode.Text.Trim()))
            {
                MessageBox.Show("请输入库位编码！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCode.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(txtName.Text.Trim()))
            {
                MessageBox.Show("请输入库位名称！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtName.Focus();
                return;
            }

            // 校验编码唯一性
            if (IsCodeExists(txtCode.Text.Trim(), editingId))
            {
                MessageBox.Show("该编码已存在，请更换！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCode.Focus();
                return;
            }

            // 校验类型
            string type = cmbType.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(type))
            {
                MessageBox.Show("请选择库位类型！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 处理上级ID
            string parentId = null;
            if (cmbParent.SelectedValue != null)
            {
                parentId = cmbParent.SelectedValue.ToString();
            }

            // 业务规则：bin类型必须有上级
            if (type == "bin" && string.IsNullOrEmpty(parentId))
            {
                MessageBox.Show("库位（bin）必须指定上级货架！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 构建实体
            var location = new Location
            {
                Id = editingId, // 编辑时复用ID，新增时为空（由SaveLocation生成）
                Code = txtCode.Text.Trim(),
                Name = txtName.Text.Trim(),
                Type = type,
                ParentId = parentId,
                TemperatureRange = txtTempRange.Text.Trim(),
                HazardClass = txtHazardClass.Text.Trim(),
                Active = chkActive.Checked
            };

            // 保存并刷新
            SaveLocation(location);
            LoadLocationTree();
            ClearForm();
            MessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // 新建按钮点击事件
        private void btnNew_Click(object sender, EventArgs e)
        {
            ClearForm();
            LoadParentComboBox(); // 加载所有可用上级库位
        }

        // 删除按钮点击事件
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(editingId))
            {
                MessageBox.Show("请先选择要删除的库位！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 校验是否有子节点
            if (HasChildren(editingId))
            {
                MessageBox.Show("该库位包含子节点，无法删除！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 校验是否被使用
            if (IsLocationInUse(editingId))
            {
                MessageBox.Show("该库位正在被库存使用，无法删除！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 确认删除
            if (MessageBox.Show("确定要删除该库位吗？删除后不可恢复！", "确认", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                DeleteLocation(editingId);
                LoadLocationTree();
                ClearForm();
                MessageBox.Show("删除成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}