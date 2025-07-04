using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace Pet.BLL
{
    /// <summary>
    /// 截图策略 - 双击时截取全屏并保存到桌面
    /// </summary>
    public class ScreenshotStrategy : IDoubleClickActionStrategy
    {
        public string Name => "截取全屏";

        public void Execute()
        {
            try
            {
                // 获取屏幕尺寸
                Rectangle bounds = Screen.PrimaryScreen.Bounds;
                Bitmap screenshot = new Bitmap(bounds.Width, bounds.Height);

                using (Graphics g = Graphics.FromImage(screenshot))
                {
                    g.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);
                }

                // 保存到桌面
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string fileName = $"截图_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                string filePath = Path.Combine(desktopPath, fileName);
                screenshot.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);

                // 释放资源
                screenshot.Dispose();

                // 显示成功提示
                MessageBox.Show($"截图已保存到桌面！\n{filePath}", "截图成功", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"截图失败：{ex.Message}", "错误", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
