using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public class RoundedPanel : Panel
    {
        private int radius = 8;

        public RoundedPanel() : this(8) { }

        public RoundedPanel(int radius)
        {
            this.radius = radius;
            this.DoubleBuffered = true;
        }

        public int Radius
        {
            get { return radius; }
            set { radius = value; this.Invalidate(); }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            using (GraphicsPath path = new GraphicsPath())
            using (Pen borderPen = new Pen(Color.FromArgb(240, 240, 240), 1))
            {
                // 绘制圆角矩形
                path.AddArc(0, 0, radius, radius, 180, 90);
                path.AddArc(Width - radius, 0, radius, radius, 270, 90);
                path.AddArc(Width - radius, Height - radius, radius, radius, 0, 90);
                path.AddArc(0, Height - radius, radius, radius, 90, 90);
                path.CloseFigure();

                this.Region = new Region(path);

                // 绘制边框
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.DrawPath(borderPen, path);
            }
        }
    }
}