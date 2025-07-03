using System;
using System.Drawing;
using System.Windows.Forms;

namespace Pet.UI
{
    /// <summary>
    /// 自定义气泡窗体 - 用于显示对话和提醒
    /// </summary>
    public partial class BubbleForm : Form
    {
        private Label lblMessage;

        public BubbleForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 初始化组件
        /// </summary>
        private void InitializeComponent()
        {
            this.lblMessage = new Label();
            this.SuspendLayout();

            // 
            // lblMessage
            // 
            this.lblMessage.AutoSize = false;
            this.lblMessage.Dock = DockStyle.Fill;
            this.lblMessage.Font = new Font("微软雅黑", 10F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(134)));
            this.lblMessage.Location = new Point(0, 0);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new Size(200, 50);
            this.lblMessage.TabIndex = 0;
            this.lblMessage.Text = "";
            this.lblMessage.TextAlign = ContentAlignment.MiddleLeft;

            // 
            // BubbleForm
            // 
            this.AutoScaleDimensions = new SizeF(6F, 12F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = Color.White;
            this.ClientSize = new Size(200, 50);
            this.Controls.Add(this.lblMessage);
            this.FormBorderStyle = FormBorderStyle.None;
            this.Name = "BubbleForm";
            this.Padding = new Padding(10);
            this.ShowInTaskbar = false;
            this.StartPosition = FormStartPosition.Manual;
            this.TopMost = true;
            this.Text = "BubbleForm";

            // 添加圆角和阴影效果
            this.Paint += BubbleForm_Paint;

            this.ResumeLayout(false);
        }

        /// <summary>
        /// 自定义绘制气泡外观
        /// </summary>
        private void BubbleForm_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // 绘制圆角矩形背景
            Rectangle rect = new Rectangle(0, 0, this.Width - 1, this.Height - 1);
            using (Brush brush = new SolidBrush(Color.FromArgb(240, 248, 255))) // 淡蓝色背景
            {
                g.FillRoundedRectangle(brush, rect, 10);
            }

            // 绘制边框
            using (Pen pen = new Pen(Color.FromArgb(173, 216, 230), 2)) // 淡蓝色边框
            {
                g.DrawRoundedRectangle(pen, rect, 10);
            }
        }

        /// <summary>
        /// 设置消息内容，并自动调整窗体大小
        /// </summary>
        /// <param name="message">要显示的消息</param>
        /// <param name="maxWidth">气泡的最大宽度</param>
        public void SetMessage(string message, int maxWidth = 250)
        {
            lblMessage.Text = message;

            // 使用 TextRenderer.MeasureText 来精确测量文本需要的尺寸
            Size textSize = TextRenderer.MeasureText(message, lblMessage.Font, 
                new Size(maxWidth, 0), // 限制最大宽度，高度不限
                TextFormatFlags.WordBreak); // 允许自动换行

            // 加上Padding，计算窗体最终尺寸
            this.Width = Math.Max(textSize.Width + this.Padding.Horizontal, 100); // 最小宽度100
            this.Height = Math.Max(textSize.Height + this.Padding.Vertical, 40);  // 最小高度40
        }
    }

    /// <summary>
    /// Graphics扩展方法，用于绘制圆角矩形
    /// </summary>
    public static class GraphicsExtensions
    {
        public static void FillRoundedRectangle(this Graphics graphics, Brush brush, Rectangle bounds, int cornerRadius)
        {
            if (graphics == null) throw new ArgumentNullException("graphics");
            if (brush == null) throw new ArgumentNullException("brush");

            using (System.Drawing.Drawing2D.GraphicsPath path = CreateRoundedRectanglePath(bounds, cornerRadius))
            {
                graphics.FillPath(brush, path);
            }
        }

        public static void DrawRoundedRectangle(this Graphics graphics, Pen pen, Rectangle bounds, int cornerRadius)
        {
            if (graphics == null) throw new ArgumentNullException("graphics");
            if (pen == null) throw new ArgumentNullException("pen");

            using (System.Drawing.Drawing2D.GraphicsPath path = CreateRoundedRectanglePath(bounds, cornerRadius))
            {
                graphics.DrawPath(pen, path);
            }
        }

        private static System.Drawing.Drawing2D.GraphicsPath CreateRoundedRectanglePath(Rectangle bounds, int cornerRadius)
        {
            int diameter = cornerRadius * 2;
            Size size = new Size(diameter, diameter);
            Rectangle arc = new Rectangle(bounds.Location, size);
            System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();

            if (cornerRadius == 0)
            {
                path.AddRectangle(bounds);
                return path;
            }

            // 左上角
            path.AddArc(arc, 180, 90);

            // 右上角
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);

            // 右下角
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);

            // 左下角
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
            return path;
        }
    }
}
