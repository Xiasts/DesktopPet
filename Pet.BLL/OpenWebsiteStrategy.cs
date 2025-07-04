using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace Pet.BLL
{
    /// <summary>
    /// 打开网站策略 - 双击时打开指定网站
    /// </summary>
    public class OpenWebsiteStrategy : IDoubleClickActionStrategy
    {
        private readonly string _url;
        private readonly string _displayName;

        public string Name => _displayName ?? $"打开网站 ({_url})";

        public event Action<string, int> OnActionMessage;

        public OpenWebsiteStrategy(string url = "https://www.bilibili.com", string displayName = null)
        {
            _url = url;
            _displayName = displayName;
        }

        public void Execute()
        {
            try
            {
                // 使用默认浏览器打开网站
                Process.Start(new ProcessStartInfo
                {
                    FileName = _url,
                    UseShellExecute = true
                });
                OnActionMessage?.Invoke($"皮卡！正在打开 {_displayName ?? _url}", 3000);
            }
            catch (Exception ex)
            {
                OnActionMessage?.Invoke($"皮卡...无法打开网站：{ex.Message}", 4000);
            }
        }
    }
}
