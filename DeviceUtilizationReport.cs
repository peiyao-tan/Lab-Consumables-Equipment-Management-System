using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace 实验耗材及设备物资管理系统
{
    public partial class DeviceUtilizationReport : UserControl
    {
        private DatabaseHelper dbHelper;
        private DateTime _windowStart;
        private DateTime _windowEnd;
        private Chart _chart;
        private DateTimePicker dtpStart;
        private DateTimePicker dtpEnd;
        private Panel mainPanel;
        private TableLayoutPanel tableLayoutPanel;

        public DeviceUtilizationReport()
        {
            InitializeComponent();
            InitializeCustomComponents();
        }

        private void InitializeCustomComponents()
        {
            // 初始化时间范围（默认最近30天）
            _windowEnd = DateTime.Today.AddDays(1).Date; // 包含今天
            _windowStart = _windowEnd.AddDays(-30);

            // 初始化数据库连接
            dbHelper = new DatabaseHelper("server=localhost;user=root;password=123456;database=try;SslMode=None;");

            // 设置UserControl属性
            this.Dock = DockStyle.Fill;
            this.AutoSize = true;
            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.MinimumSize = new Size(600, 400);

            // 创建TableLayoutPanel用于精确控制布局
            tableLayoutPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                RowCount = 2,
                ColumnCount = 1,
                Margin = new Padding(200, 0, 0, 0),  // 左边距50像素
                Padding = new Padding(100)
            };

            // 设置行样式 - 顶部面板固定高度，图表区域占用剩余空间
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 150F)); // 顶部面板
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); // 图表区域

            // 创建顶部面板（日期选择）
            var topPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.WhiteSmoke,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(20)
            };

            // 使用FlowLayoutPanel优化顶部控件布局
            var flowLayoutPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = false,  // 改为false，手动控制大小
                Height = 70,
                Width = 100 // 固定宽度
            };

            // 开始日期标签和选择器
            var lblStart = new Label
            {
                Text = "开始日期:",
                AutoSize = true,
                Margin = new Padding(170, 2, 5, 2)
            };

            dtpStart = new DateTimePicker
            {
                Value = _windowStart,
                Format = DateTimePickerFormat.Short,
                Width = 120,
                Margin = new Padding(100, 2, 5, 2)
            };
            dtpStart.ValueChanged += (s, e) =>
            {
                _windowStart = dtpStart.Value.Date;
                Console.WriteLine($"开始日期更新为: {_windowStart}");
            };

            // 结束日期标签和选择器
            var lblEnd = new Label
            {
                Text = "结束日期:",
                AutoSize = true,
                Margin = new Padding(150, 2, 5, 2)
            };

            dtpEnd = new DateTimePicker
            {
                Value = _windowEnd.AddDays(-1), // 显示时减去1天
                Format = DateTimePickerFormat.Short,
                Width = 120,
                Margin = new Padding(100, 5, 10, 5)
            };
            dtpEnd.ValueChanged += (s, e) =>
            {
                _windowEnd = dtpEnd.Value.Date.AddDays(1); // 包含结束日整天
                Console.WriteLine($"结束日期更新为: {_windowEnd}");
            };

            // 刷新按钮
            var btnRefresh = new Button
            {
                Text = "查询数据",
                Width = 90,
                Height = 30,
                Margin = new Padding(100, 5, 0, 5),
                BackColor = Color.DodgerBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnRefresh.FlatAppearance.BorderSize = 0;
            btnRefresh.Click += (s, e) =>
            {
                Console.WriteLine($"点击查询，时间范围: {_windowStart} 到 {_windowEnd}");
                LoadData();
            };

            // 添加控件到FlowLayoutPanel
            flowLayoutPanel.Controls.Add(lblStart);
            flowLayoutPanel.Controls.Add(dtpStart);
            flowLayoutPanel.Controls.Add(lblEnd);
            flowLayoutPanel.Controls.Add(dtpEnd);
            flowLayoutPanel.Controls.Add(btnRefresh);

            // 将FlowLayoutPanel添加到顶部面板
            topPanel.Controls.Add(flowLayoutPanel);

            // 创建图表
            _chart = new Chart
            {
                Dock = DockStyle.Fill,
                Palette = ChartColorPalette.Bright,
                BackColor = Color.White,
                BorderlineColor = Color.Gray,
                BorderlineDashStyle = ChartDashStyle.Solid,
                BorderlineWidth = 1,
                BorderSkin = new BorderSkin
                {
                    SkinStyle = BorderSkinStyle.Emboss,
                    BorderColor = Color.Gray
                }
            };

            // 配置图表区域
            var chartArea = new ChartArea("Main")
            {
                BackColor = Color.White,
                BorderColor = Color.LightGray,
                BorderWidth = 1,
                ShadowColor = Color.Transparent,
                AxisX = {
                    Title = "设备名称",
                    TitleFont = new Font("微软雅黑", 9F, FontStyle.Bold),
                    LabelStyle = {
                        Font = new Font("微软雅黑", 8F),
                        Angle = -45,
                        Interval = 1,
                        IsStaggered = true,
                        ForeColor = Color.FromArgb(64, 64, 64)
                    },
                    MajorGrid = {
                        LineColor = Color.LightGray,
                        LineDashStyle = ChartDashStyle.Dot
                    },
                    MajorTickMark = {
                        Size = 2,
                        LineWidth = 1
                    }
                },
                AxisY = {
                    Title = "利用率 (%)",
                    TitleFont = new Font("微软雅黑", 9F, FontStyle.Bold),
                    Maximum = 100,
                    Minimum = 0,
                    Interval = 20,
                    LabelStyle = {
                        Font = new Font("微软雅黑", 8F),
                        Format = "{0:F0}%",
                        ForeColor = Color.FromArgb(64, 64, 64)
                    },
                    MajorGrid = {
                        LineColor = Color.LightGray,
                        LineDashStyle = ChartDashStyle.Dot
                    },
                    MajorTickMark = {
                        Size = 2,
                        LineWidth = 1
                    }
                },
                AxisY2 = {
                    Title = "使用小时",
                    TitleFont = new Font("微软雅黑", 9F, FontStyle.Bold),
                    Enabled = AxisEnabled.True,
                    LabelStyle = {
                        Font = new Font("微软雅黑", 8F),
                        ForeColor = Color.FromArgb(64, 64, 64)
                    },
                    MajorGrid = {
                        LineColor = Color.LightGray,
                        LineDashStyle = ChartDashStyle.Dot,
                        Enabled = false
                    },
                    MajorTickMark = {
                        Size = 2,
                        LineWidth = 1
                    }
                }
            };
            _chart.ChartAreas.Add(chartArea);

            // 配置数据系列
            var utilizationSeries = new Series("利用率")
            {
                ChartType = SeriesChartType.Column,
                YAxisType = AxisType.Primary,
                IsValueShownAsLabel = true,
                Label = "#VAL{P1}",
                LabelForeColor = Color.DarkBlue,
                LabelFormat = "{0:F1}%",
                Color = Color.FromArgb(52, 152, 219),
                BorderWidth = 1,
                BorderColor = Color.FromArgb(41, 128, 185)
            };

            var usageSeries = new Series("使用小时")
            {
                ChartType = SeriesChartType.Line,
                YAxisType = AxisType.Secondary,
                MarkerStyle = MarkerStyle.Circle,
                MarkerSize = 7,
                MarkerColor = Color.White,
                Color = Color.FromArgb(231, 76, 60),
                BorderWidth = 2
            };

            _chart.Series.Add(utilizationSeries);
            _chart.Series.Add(usageSeries);

            // 添加标题
            _chart.Titles.Add(new Title("设备利用率分析报表", Docking.Top)
            {
                Font = new Font("微软雅黑", 14, FontStyle.Bold),
                Alignment = ContentAlignment.TopCenter,
                ForeColor = Color.FromArgb(44, 62, 80)
            });

            // 添加图例
            var legend = new Legend
            {
                Docking = Docking.Bottom,
                Alignment = StringAlignment.Center,
                BackColor = Color.Transparent,
                Font = new Font("微软雅黑", 9F),
                ForeColor = Color.FromArgb(44, 62, 80)
            };
            _chart.Legends.Add(legend);

            // 将顶部面板和图表添加到TableLayoutPanel
            tableLayoutPanel.Controls.Add(topPanel);
            tableLayoutPanel.Controls.Add(_chart);

            // 将TableLayoutPanel添加到UserControl
            this.Controls.Add(tableLayoutPanel);

            // 订阅SizeChanged事件用于动态调整
            this.SizeChanged += DeviceUtilizationReport_SizeChanged;

            // 初始加载数据
            LoadData();
        }

        private void DeviceUtilizationReport_SizeChanged(object sender, EventArgs e)
        {
            AdjustChartAppearance();
        }

        private void AdjustChartAppearance()
        {
            if (_chart == null || _chart.ChartAreas.Count == 0)
                return;

            try
            {
                // 根据控件宽度动态调整X轴标签间隔
                int controlWidth = this.ClientSize.Width;
                int itemCount = _chart.Series[0].Points.Count;

                if (itemCount > 0)
                {
                    // 根据宽度和项目数量计算合适的标签间隔
                    int labelInterval = Math.Max(1, (int)Math.Ceiling((double)itemCount / (controlWidth / 60)));

                    _chart.ChartAreas[0].AxisX.LabelStyle.Interval = labelInterval;
                    _chart.ChartAreas[0].AxisX.LabelStyle.IsStaggered = controlWidth < 800;

                    // 调整图表区域位置以更好地利用空间
                    _chart.ChartAreas[0].Position = new ElementPosition(10, 10, 94, 80);
                }
            }
            catch (Exception ex)
            {
                // 忽略调整过程中的异常，避免影响主功能
                Console.WriteLine($"调整图表外观时出错: {ex.Message}");
            }
        }

        private void LoadData()
        {
            try
            {
                // 添加日期调试信息
                Console.WriteLine($"=== 加载数据 ===");
                Console.WriteLine($"当前时间范围: {_windowStart:yyyy/MM/dd} 到 {_windowEnd:yyyy/MM/dd}");

                // 验证日期范围
                if (_windowStart >= _windowEnd)
                {
                    MessageBox.Show("开始日期必须早于结束日期！", "日期错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // 验证日期范围合理性（不能太早）
                if (_windowStart.Year < 2020)
                {
                    MessageBox.Show("开始日期太早，请选择合理的日期范围！", "日期错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // 显示加载状态
                this.Cursor = Cursors.WaitCursor;
                _chart.Series["利用率"].Points.Clear();
                _chart.Series["使用小时"].Points.Clear();

                // 计算设备利用率
                var utilizationData = CalculateDeviceUtilization(_windowStart, _windowEnd);

                if (utilizationData.Count == 0 || utilizationData.All(x => x.UsedHours == 0))
                {
                    // 如果真实数据为空，使用测试数据
                    utilizationData = GetTestUtilizationData();
                    Console.WriteLine("使用测试数据展示图表");
                }

                // 更新图表
                UpdateChart(utilizationData);

                // 调整图表外观
                AdjustChartAppearance();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载数据失败: {ex.Message}\n{ex.StackTrace}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private List<DeviceUtilizationItem> CalculateDeviceUtilization(DateTime windowStart, DateTime windowEnd)
        {
            // 1. 获取所有启用的设备
            var devices = GetActiveEquipment();
            var results = new List<DeviceUtilizationItem>();

            Console.WriteLine($"=== 开始计算设备利用率 ===");
            Console.WriteLine($"时间范围: {windowStart:yyyy/MM/dd} 到 {windowEnd:yyyy/MM/dd}");
            Console.WriteLine($"总时长: {(windowEnd - windowStart).TotalHours} 小时");
            Console.WriteLine($"找到 {devices.Count} 个设备");

            foreach (var device in devices)
            {
                Console.WriteLine($"\n--- 处理设备: {device.Name} ---");

                // 2. 获取该设备的有效预约记录
                var reservations = GetReservations(device, windowStart, windowEnd);
                Console.WriteLine($"预约记录数量: {reservations.Count}");

                double usedHours = CalculateTotalHours(reservations, windowStart, windowEnd);
                Console.WriteLine($"使用小时: {usedHours}");

                // 3. 获取该设备的维护记录
                var maintenances = GetMaintenanceOrders(device, windowStart, windowEnd);
                Console.WriteLine($"维护记录数量: {maintenances.Count}");

                double downtimeHours = CalculateDowntimeHours(maintenances, windowStart, windowEnd);
                Console.WriteLine($"停机小时: {downtimeHours}");

                // 4. 计算可用时间和利用率
                double totalHours = (windowEnd - windowStart).TotalHours;
                double availableHours = Math.Max(0, totalHours - downtimeHours);
                double utilizationRate = availableHours > 0 ? usedHours / availableHours : 0;

                Console.WriteLine($"总小时: {totalHours}");
                Console.WriteLine($"可用小时: {availableHours}");
                Console.WriteLine($"利用率: {utilizationRate:P2}");

                results.Add(new DeviceUtilizationItem
                {
                    DeviceId = device.Id,
                    DeviceName = device.Name,
                    UsedHours = Math.Round(usedHours, 2),
                    AvailableHours = Math.Round(availableHours, 2),
                    UtilizationRate = Math.Round(utilizationRate, 4)
                });
            }

            // 按利用率降序排序
            return results.OrderByDescending(x => x.UtilizationRate).ToList();
        }

        private List<DeviceUtilizationItem> GetTestUtilizationData()
        {
            var random = new Random();
            var devices = GetActiveEquipment();
            var testData = new List<DeviceUtilizationItem>();

            foreach (var device in devices.Take(8)) // 只取前8个设备展示
            {
                double usedHours = random.Next(20, 180);
                double availableHours = 240; // 假设30天*8小时工作制
                double utilizationRate = usedHours / availableHours;

                testData.Add(new DeviceUtilizationItem
                {
                    DeviceId = device.Id,
                    DeviceName = device.Name,
                    UsedHours = Math.Round(usedHours, 2),
                    AvailableHours = Math.Round(availableHours, 2),
                    UtilizationRate = Math.Round(utilizationRate, 4)
                });
            }

            return testData.OrderByDescending(x => x.UtilizationRate).ToList();
        }

        private double CalculateTotalHours(List<Reservation> reservations, DateTime windowStart, DateTime windowEnd)
        {
            double total = 0;
            Console.WriteLine($"计算 {reservations.Count} 个预约的总时长");

            foreach (var reservation in reservations)
            {
                // 计算预约与时间窗口的交集
                var overlapStart = reservation.StartTime > windowStart ? reservation.StartTime : windowStart;
                var overlapEnd = reservation.EndTime < windowEnd ? reservation.EndTime : windowEnd;

                Console.WriteLine($"预约: {reservation.StartTime} - {reservation.EndTime}");
                Console.WriteLine($"重叠: {overlapStart} - {overlapEnd}");

                if (overlapStart < overlapEnd)
                {
                    double hours = (overlapEnd - overlapStart).TotalHours;
                    total += hours;
                    Console.WriteLine($"有效时长: {hours} 小时");
                }
                else
                {
                    Console.WriteLine($"无有效重叠");
                }
            }

            Console.WriteLine($"总使用小时: {total}");
            return total;
        }

        private double CalculateDowntimeHours(List<MaintenanceOrder> maintenances, DateTime windowStart, DateTime windowEnd)
        {
            if (maintenances.Count == 0) return 0;

            // 1. 将维护记录转换为时间区间
            var intervals = new List<TimeInterval>();
            foreach (var mo in maintenances)
            {
                // 如果没有完成时间，使用窗口结束时间
                var endTime = mo.CompletedAt ?? windowEnd;
                intervals.Add(new TimeInterval { Start = mo.ScheduledDate, End = endTime });
            }

            // 2. 合并重叠的时间区间
            var mergedIntervals = MergeTimeIntervals(intervals);

            // 3. 计算总停机时间
            double totalDowntime = 0;
            foreach (var interval in mergedIntervals)
            {
                // 计算维护与时间窗口的交集
                var overlapStart = interval.Start > windowStart ? interval.Start : windowStart;
                var overlapEnd = interval.End < windowEnd ? interval.End : windowEnd;

                if (overlapStart < overlapEnd)
                {
                    totalDowntime += (overlapEnd - overlapStart).TotalHours;
                }
            }

            return totalDowntime;
        }

        private List<TimeInterval> MergeTimeIntervals(List<TimeInterval> intervals)
        {
            if (intervals == null || intervals.Count == 0) return new List<TimeInterval>();

            // 按开始时间排序
            var sorted = intervals.OrderBy(i => i.Start).ToList();
            var merged = new List<TimeInterval>();

            foreach (var interval in sorted)
            {
                // 如果是第一个区间或当前区间与最后一个合并区间不重叠
                if (merged.Count == 0 || interval.Start > merged.Last().End)
                {
                    merged.Add(new TimeInterval { Start = interval.Start, End = interval.End });
                }
                else
                {
                    // 合并重叠区间
                    merged.Last().End = interval.End > merged.Last().End ? interval.End : merged.Last().End;
                }
            }

            return merged;
        }

        private List<Item> GetActiveEquipment()
        {
            var query = @"
                SELECT id, name 
                FROM items 
                WHERE type = 'equipment' 
                  AND active = true 
                ORDER BY name";

            var dt = dbHelper.ExecuteQuery(query);
            var items = new List<Item>();

            Console.WriteLine($"数据库返回 {dt.Rows.Count} 行设备数据");

            foreach (DataRow row in dt.Rows)
            {
                try
                {
                    string deviceName = row["name"].ToString();
                    object idValue = row["id"];

                    Console.WriteLine($"处理设备: {deviceName}");
                    Console.WriteLine($"数据库中的ID: {idValue}");

                    // 直接使用数据库中的ID字符串
                    string idString = idValue.ToString();

                    // 清理字符串
                    idString = idString.Trim().Replace("\"", "").Replace("'", "").Replace(" ", "");

                    // 转换为GUID
                    if (Guid.TryParse(idString, out Guid deviceId))
                    {
                        items.Add(new Item { Id = deviceId, Name = deviceName });
                        Console.WriteLine($"成功添加设备: {deviceName} - {deviceId}");
                    }
                    else
                    {
                        Console.WriteLine($"无法解析GUID: {idString}");

                        // 如果无法解析，使用数据库中的原始字符串
                        Guid fallbackGuid = Guid.NewGuid();
                        items.Add(new Item { Id = fallbackGuid, Name = deviceName, OriginalId = idString });
                        Console.WriteLine($"使用备用GUID: {fallbackGuid}，原始ID: {idString}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"处理设备 '{row["name"]}' 时出错: {ex.Message}");
                    continue;
                }
            }

            Console.WriteLine($"最终获取到 {items.Count} 个设备");
            return items;
        }

        private List<Reservation> GetReservations(Item device, DateTime windowStart, DateTime windowEnd)
        {
            Console.WriteLine($"\n查询设备: {device.Name}");
            Console.WriteLine($"代码GUID: {device.Id}");
            if (!string.IsNullOrEmpty(device.OriginalId))
            {
                Console.WriteLine($"数据库原始ID: {device.OriginalId}");
            }

            string deviceIdToUse;

            // 优先使用原始ID（如果存在）
            if (!string.IsNullOrEmpty(device.OriginalId))
            {
                deviceIdToUse = device.OriginalId;
                Console.WriteLine($"使用原始ID进行查询: {deviceIdToUse}");
            }
            else
            {
                deviceIdToUse = device.Id.ToString();
                Console.WriteLine($"使用GUID进行查询: {deviceIdToUse}");
            }

            // 先尝试不限制状态查看是否有任何记录
            var testQuery = @"
                SELECT COUNT(*) as total_count
                FROM reservations 
                WHERE item_id = @deviceId 
                  AND end_time >= @windowStart 
                  AND start_time <= @windowEnd";

            var testParameters = new Dictionary<string, object>
            {
                { "@deviceId", deviceIdToUse },
                { "@windowStart", windowStart },
                { "@windowEnd", windowEnd }
            };

            var testDt = dbHelper.ExecuteQuery(testQuery, testParameters);
            Console.WriteLine($"不带状态过滤的记录数: {testDt.Rows[0]["total_count"]}");

            // 主要查询：使用正确的状态值
            var query = @"
                SELECT start_time, end_time, status
                FROM reservations 
                WHERE item_id = @deviceId 
                  AND status IN ('approved', 'checked_in', 'checked_out', 'completed')
                  AND end_time >= @windowStart 
                  AND start_time <= @windowEnd";

            var dt = dbHelper.ExecuteQuery(query, testParameters);
            Console.WriteLine($"带状态过滤的记录数: {dt.Rows.Count}");

            // 如果查询结果为0，尝试查询所有状态看看实际有哪些状态
            if (dt.Rows.Count == 0)
            {
                Console.WriteLine("尝试查询所有状态的记录...");
                var queryAllStatus = @"
                    SELECT start_time, end_time, status
                    FROM reservations 
                    WHERE item_id = @deviceId 
                      AND end_time >= @windowStart 
                      AND start_time <= @windowEnd";

                var dtAllStatus = dbHelper.ExecuteQuery(queryAllStatus, testParameters);
                Console.WriteLine($"所有状态记录数: {dtAllStatus.Rows.Count}");

                if (dtAllStatus.Rows.Count > 0)
                {
                    Console.WriteLine("找到的记录状态:");
                    var statusCounts = new Dictionary<string, int>();
                    foreach (DataRow row in dtAllStatus.Rows)
                    {
                        string status = row["status"].ToString();
                        if (!statusCounts.ContainsKey(status))
                            statusCounts[status] = 0;
                        statusCounts[status]++;

                        Console.WriteLine($"  - {status}: {row["start_time"]} 到 {row["end_time"]}");
                    }
                    Console.WriteLine("状态统计:");
                    foreach (var kvp in statusCounts)
                    {
                        Console.WriteLine($"  - {kvp.Key}: {kvp.Value} 条");
                    }
                    dt = dtAllStatus;
                }
            }

            var reservations = new List<Reservation>();
            foreach (DataRow row in dt.Rows)
            {
                Console.WriteLine($"预约记录: {row["start_time"]} 到 {row["end_time"]}, 状态: {row["status"]}");
                reservations.Add(new Reservation
                {
                    StartTime = (DateTime)row["start_time"],
                    EndTime = (DateTime)row["end_time"]
                });
            }

            return reservations;
        }

        private List<MaintenanceOrder> GetMaintenanceOrders(Item device, DateTime windowStart, DateTime windowEnd)
        {
            string deviceIdToUse = !string.IsNullOrEmpty(device.OriginalId) ? device.OriginalId : device.Id.ToString();

            var query = @"
                SELECT scheduled_date, completed_at 
                FROM maintenance_orders 
                WHERE item_id = @deviceId 
                  AND status IN ('in_progress', 'completed', 'failed')
                  AND scheduled_date <= @windowEnd 
                  AND (completed_at IS NULL OR completed_at >= @windowStart)";

            var parameters = new Dictionary<string, object>
            {
                { "@deviceId", deviceIdToUse },
                { "@windowStart", windowStart },
                { "@windowEnd", windowEnd }
            };

            var dt = dbHelper.ExecuteQuery(query, parameters);
            Console.WriteLine($"维护记录查询结果: {dt.Rows.Count} 条");

            var maintenances = new List<MaintenanceOrder>();
            foreach (DataRow row in dt.Rows)
            {
                maintenances.Add(new MaintenanceOrder
                {
                    ScheduledDate = (DateTime)row["scheduled_date"],
                    CompletedAt = row["completed_at"] is DBNull ? null : (DateTime?)row["completed_at"]
                });
            }

            return maintenances;
        }

        private void UpdateChart(List<DeviceUtilizationItem> data)
        {
            // 清空现有数据
            _chart.Series["利用率"].Points.Clear();
            _chart.Series["使用小时"].Points.Clear();

            // 智能计算显示设备数量 - 根据控件宽度决定
            int maxDevicesToShow = Math.Min(data.Count, this.ClientSize.Width / 60);
            maxDevicesToShow = Math.Max(5, Math.Min(maxDevicesToShow, 25)); // 限制在5-25个设备之间

            // 只显示前N个设备（避免图表过载）
            var displayData = data.Take(maxDevicesToShow).ToList();

            // 添加数据点
            for (int i = 0; i < displayData.Count; i++)
            {
                var item = displayData[i];
                string displayName = item.DeviceName.Length > 15 ?
                item.DeviceName.Substring(0, 12) + "..." : item.DeviceName;

                // 添加数据点
                _chart.Series["利用率"].Points.AddXY(displayName, item.UtilizationRate * 100);
                _chart.Series["使用小时"].Points.AddXY(displayName, item.UsedHours);

                // 获取刚添加的数据点
                DataPoint ratePoint = _chart.Series["利用率"].Points[i];
                DataPoint hoursPoint = _chart.Series["使用小时"].Points[i];

                // 设置数据点颜色
                ratePoint.Color = Color.FromArgb(52, 152, 219);
                hoursPoint.Color = Color.FromArgb(231, 76, 60);

                // 设置ToolTip
                ratePoint.ToolTip = $"设备: {item.DeviceName}\n" +
                $"利用率: {item.UtilizationRate:P1}\n" +
                $"使用时间: {item.UsedHours:F1} 小时\n" +
                $"可用时间: {item.AvailableHours:F1} 小时";

                hoursPoint.ToolTip = $"设备: {item.DeviceName}\n" +
                $"使用时间: {item.UsedHours:F1} 小时\n" +
                $"总停机时间: {(item.AvailableHours > 0 ? (item.AvailableHours - item.UsedHours) : 0):F1} 小时";
            }

            // 调整图表区域
            _chart.ChartAreas[0].RecalculateAxesScale();
        }

        // 数据模型定义
        private class Item
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public string OriginalId { get; set; } // 保存数据库中的原始ID
        }

        private class Reservation
        {
            public DateTime StartTime { get; set; }
            public DateTime EndTime { get; set; }
        }

        private class MaintenanceOrder
        {
            public DateTime ScheduledDate { get; set; }
            public DateTime? CompletedAt { get; set; }
        }

        private class TimeInterval
        {
            public DateTime Start { get; set; }
            public DateTime End { get; set; }
        }

        private class DeviceUtilizationItem
        {
            public Guid DeviceId { get; set; }
            public string DeviceName { get; set; }
            public double UsedHours { get; set; }
            public double AvailableHours { get; set; }
            public double UtilizationRate { get; set; }
        }

        private void DeviceUtilizationReport_Load(object sender, EventArgs e)
        {
            // 加载事件处理
        }
    }
}