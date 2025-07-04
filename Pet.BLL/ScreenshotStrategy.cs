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

        public event Action<string, int> OnActionMessage;

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

                // 通过事件显示成功提示
                OnActionMessage?.Invoke($"皮卡！截图已保存到桌面！\n{fileName}", 5000);
            }
            catch (Exception ex)
            {
                OnActionMessage?.Invoke($"皮卡...截图失败：{ex.Message}", 4000);
            }
        }
    }
}
