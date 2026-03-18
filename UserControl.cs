using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace AutoWindowsSize
{
    class AutoAdaptUserControlSize
    {
        double originalWidth;
        double originalHeight;
        double scaleX;
        double scaleY;
        Dictionary<string, string> ControlsInfo = new Dictionary<string, string>();

        private UserControl _userControl;
        Panel ContainerPanel = new Panel();

        public AutoAdaptUserControlSize(UserControl userControl)
        {
            _userControl = userControl;

            // 创建容器面板
            _userControl.Controls.Add(ContainerPanel);
            ContainerPanel.BorderStyle = BorderStyle.None;
            ContainerPanel.Dock = DockStyle.Fill;
            ContainerPanel.BackColor = Color.Transparent;

            // 将用户控件中的所有控件移动到容器面板
            MoveControlsToContainer();

            // 保存初始大小信息
            InitControlsInfo(ContainerPanel);
        }

        private void MoveControlsToContainer()
        {
            List<Control> controlsToMove = new List<Control>();

            // 收集需要移动的控件（排除容器面板自身）
            foreach (Control control in _userControl.Controls)
            {
                if (control != ContainerPanel)
                {
                    controlsToMove.Add(control);
                }
            }

            // 移动控件到容器
            foreach (Control control in controlsToMove)
            {
                ContainerPanel.Controls.Add(control);
            }
        }

        public void InitControlsInfo(Control ctrlContainer)
        {
            if (ctrlContainer.Parent == _userControl)
            {
                originalWidth = Convert.ToDouble(ctrlContainer.Width);
                originalHeight = Convert.ToDouble(ctrlContainer.Height);
            }

            foreach (Control item in ctrlContainer.Controls)
            {
                if (item.Name.Trim() != "")
                {
                    ControlsInfo.Add(item.Name,
                        (item.Left + item.Width / 2) + "," +
                        (item.Top + item.Height / 2) + "," +
                        item.Width + "," +
                        item.Height + "," +
                        item.Font.Size);
                }

                if ((item as UserControl) == null && item.Controls.Count > 0)
                {
                    InitControlsInfo(item);
                }
            }
        }

        public void ControlSizeChanged()
        {
            try
            {
                if (ControlsInfo.Count > 0 && _userControl.Width > 0 && _userControl.Height > 0)
                {
                    ControlsZoomScale(ContainerPanel);
                    ControlsChange(ContainerPanel);
                }
            }
            catch (Exception ex)
            {
                // 可记录日志
                System.Diagnostics.Debug.WriteLine("自适应缩放错误: " + ex.Message);
            }
        }

        private void ControlsZoomScale(Control ctrlContainer)
        {
            scaleX = (Convert.ToDouble(ctrlContainer.Width) / originalWidth);
            scaleY = (Convert.ToDouble(ctrlContainer.Height) / originalHeight);
        }

        private void ControlsChange(Control ctrlContainer)
        {
            double[] pos = new double[5];
            foreach (Control item in ctrlContainer.Controls)
            {
                if (item.Name.Trim() != "")
                {
                    if ((item as UserControl) == null && item.Controls.Count > 0)
                    {
                        ControlsChange(item);
                    }

                    if (ControlsInfo.ContainsKey(item.Name))
                    {
                        string[] strs = ControlsInfo[item.Name].Split(',');

                        for (int i = 0; i < 5; i++)
                        {
                            pos[i] = Convert.ToDouble(strs[i]);
                        }

                        double itemWidth = pos[2] * scaleX;
                        double itemHeight = pos[3] * scaleY;
                        item.Left = Convert.ToInt32(pos[0] * scaleX - itemWidth / 2);
                        item.Top = Convert.ToInt32(pos[1] * scaleY - itemHeight / 2);
                        item.Width = Convert.ToInt32(itemWidth);
                        item.Height = Convert.ToInt32(itemHeight);

                        if (float.Parse((pos[4] * Math.Min(scaleX, scaleY)).ToString()) > 0)
                        {
                            item.Font = new Font(item.Font.Name,
                                float.Parse((pos[4] * Math.Min(scaleX, scaleY)).ToString()));
                        }
                    }
                }
            }
        }
    }
}