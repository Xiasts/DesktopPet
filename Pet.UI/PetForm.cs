using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Pet.BLL;

namespace Pet.UI
{
    public partial class PetForm : Form
    {
        private PetCore _petCore;
        private bool _isDragging = false;
        private Point _startPoint;
        private ContextMenuStrip _contextMenu;
        private BubbleForm _bubbleForm;
        private IDialogService _dialogService;
        private IDoubleClickActionStrategy _currentDoubleClickStrategy;
        private List<IDoubleClickActionStrategy> _availableStrategies;

        #region Win32 API for Layered Windows

        // Win32 常量
        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_LAYERED = 0x80000;
        private const int LWA_ALPHA = 0x2;
        private const int ULW_ALPHA = 0x2;

        // Win32 结构体
        [StructLayout(LayoutKind.Sequential)]
        private struct Win32Point
        {
            public int X;
            public int Y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct Win32Size
        {
            public int Width;
            public int Height;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct BLENDFUNCTION
        {
            public byte BlendOp;
            public byte BlendFlags;
            public byte SourceConstantAlpha;
            public byte AlphaFormat;
        }

        // Win32 API 函数声明
        [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
        private static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll", ExactSpelling = true)]
        private static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        private static extern IntPtr CreateCompatibleDC(IntPtr hDC);

        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        private static extern bool DeleteDC(IntPtr hdc);

        [DllImport("gdi32.dll", ExactSpelling = true)]
        private static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);

        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        private static extern bool DeleteObject(IntPtr hObject);

        [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
        private static extern bool UpdateLayeredWindow(IntPtr hwnd, IntPtr hdcDst, ref Win32Point pptDst, ref Win32Size psize, IntPtr hdcSrc, ref Win32Point pptSrc, int crKey, ref BLENDFUNCTION pblend, int dwFlags);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        #endregion

        /// <summary>
        /// 重写CreateParams属性，添加分层窗口支持
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                // 添加 WS_EX_LAYERED 扩展样式
                cp.ExStyle |= WS_EX_LAYERED;
                return cp;
            }
        }

        public PetForm()
        {
            InitializeComponent();
            _petCore = new PetCore(); // 创建BLL核心实例

            // 初始化位置和屏幕边界
            _petCore.Position = this.Location;
            _petCore.ScreenBounds = Screen.PrimaryScreen.Bounds.Size;

            InitializeDoubleClickStrategies(); // 初始化双击策略
            InitializeContextMenu(); // 初始化右键菜单
            InitializeBubble(); // 初始化气泡窗体
            InitializeScheduleReminder(); // 初始化日程提醒
            InitializeDialogService(); // 初始化对话服务
        }

        /// <summary>
        /// 使用分层窗口API在指定位置更新窗体图像，支持Alpha透明
        /// </summary>
        /// <param name="bitmap">带有Alpha通道的图像</param>
        /// <param name="targetLocation">窗体要显示的目标屏幕坐标</param>
        public void SetBitmap(Bitmap bitmap, System.Drawing.Point targetLocation)
        {
            if (bitmap.PixelFormat != System.Drawing.Imaging.PixelFormat.Format32bppArgb)
            {
                // 如果不是32位带Alpha通道的格式，需要转换
                var convertedBitmap = new Bitmap(bitmap.Width, bitmap.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                using (Graphics g = Graphics.FromImage(convertedBitmap))
                {
                    g.DrawImage(bitmap, 0, 0);
                }
                bitmap = convertedBitmap;
            }

            // 1. 获取屏幕DC和内存DC
            IntPtr screenDc = GetDC(IntPtr.Zero);
            IntPtr memDc = CreateCompatibleDC(screenDc);
            IntPtr hBitmap = IntPtr.Zero;
            IntPtr oldBitmap = IntPtr.Zero;

            try
            {
                // 2. 将我们的Bitmap选入内存DC
                hBitmap = bitmap.GetHbitmap(Color.FromArgb(0)); // 从Bitmap创建GDI句柄
                oldBitmap = SelectObject(memDc, hBitmap);

                // 3. 定义窗口的混合函数和参数
                Win32Size size = new Win32Size { Width = bitmap.Width, Height = bitmap.Height };
                Win32Point pointSource = new Win32Point { X = 0, Y = 0 };
                Win32Point topPos = new Win32Point { X = targetLocation.X, Y = targetLocation.Y };
                BLENDFUNCTION blend = new BLENDFUNCTION();
                blend.BlendOp = 0x00; // AC_SRC_OVER
                blend.BlendFlags = 0;
                blend.SourceConstantAlpha = 255; // 完全不透明
                blend.AlphaFormat = 0x01; // AC_SRC_ALPHA，表示使用源图像的Alpha通道

                // 4. 调用核心API函数，更新窗口
                UpdateLayeredWindow(this.Handle, screenDc, ref topPos, ref size, memDc, ref pointSource, 0, ref blend, ULW_ALPHA);
            }
            finally
            {
                // 5. 释放所有GDI资源
                ReleaseDC(IntPtr.Zero, screenDc);
                if (hBitmap != IntPtr.Zero)
                {
                    SelectObject(memDc, oldBitmap);
                    DeleteObject(hBitmap);
                }
                DeleteDC(memDc);
            }
        }

        /// <summary>
        /// 初始化右键菜单
        /// </summary>
        private void InitializeContextMenu()
        {
            _contextMenu = new ContextMenuStrip();

            // 添加设置菜单项
            ToolStripMenuItem settingsItem = new ToolStripMenuItem("日程设置");
            settingsItem.Click += SettingsItem_Click;
            _contextMenu.Items.Add(settingsItem);

            // 添加对话菜单项
            ToolStripMenuItem chatItem = new ToolStripMenuItem("和我聊天");
            chatItem.Click += ChatItem_Click;
            _contextMenu.Items.Add(chatItem);

            // 添加分隔线
            _contextMenu.Items.Add(new ToolStripSeparator());

            // 添加互动菜单项
            ToolStripMenuItem feedItem = new ToolStripMenuItem("喂食");
            feedItem.Click += FeedItem_Click;
            _contextMenu.Items.Add(feedItem);

            ToolStripMenuItem playItem = new ToolStripMenuItem("玩耍");
            playItem.Click += PlayItem_Click;
            _contextMenu.Items.Add(playItem);

            ToolStripMenuItem thunderShockItem = new ToolStripMenuItem("放电");
            thunderShockItem.Click += ThunderShockItem_Click;
            _contextMenu.Items.Add(thunderShockItem);

            // 添加分隔线
            _contextMenu.Items.Add(new ToolStripSeparator());

            // 创建"双击功能"子菜单
            ToolStripMenuItem doubleClickMenu = new ToolStripMenuItem("双击功能");

            foreach (var strategy in _availableStrategies)
            {
                ToolStripMenuItem menuItem = new ToolStripMenuItem(strategy.Name);
                menuItem.Tag = strategy; // 将策略实例存放在Tag中
                menuItem.Click += DoubleClickStrategy_Click;

                // 默认选中的策略打上勾
                if (strategy == _currentDoubleClickStrategy)
                {
                    menuItem.Checked = true;
                }

                doubleClickMenu.DropDownItems.Add(menuItem);
            }

            _contextMenu.Items.Add(doubleClickMenu);

            // 添加分隔线
            _contextMenu.Items.Add(new ToolStripSeparator());

            // 添加关于菜单项
            ToolStripMenuItem aboutItem = new ToolStripMenuItem("关于");
            aboutItem.Click += AboutItem_Click;
            _contextMenu.Items.Add(aboutItem);

            // 添加退出菜单项
            ToolStripMenuItem exitItem = new ToolStripMenuItem("退出");
            exitItem.Click += ExitItem_Click;
            _contextMenu.Items.Add(exitItem);

            // 将右键菜单绑定到窗体
            this.ContextMenuStrip = _contextMenu;
        }

        private void animationTimer_Tick(object sender, EventArgs e)
        {
            // 1. 更新业务逻辑层，BLL会根据当前状态计算出新的位置
            _petCore.Update();

            // 2. 从BLL获取当前要显示的图片
            Image petImage = _petCore.GetCurrentImage();

            if (petImage != null)
            {
                // 3. 让窗体自身的物理位置与BLL保持一致
                if (this.Location != _petCore.Position)
                {
                    this.Location = _petCore.Position;
                }

                // 4. 更新窗体大小以适应图片
                if (this.ClientSize != petImage.Size)
                {
                    this.ClientSize = petImage.Size;
                }

                // 5. 调用新的绘制方法，使用BLL提供的最终位置和图片进行绘制
                SetBitmap(new Bitmap(petImage), _petCore.Position);
            }
        }

        private void PetForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _isDragging = true;
                _startPoint = new System.Drawing.Point(e.X, e.Y);

                // 如果当前是吸附状态，先脱离吸附
                _petCore.DetachFromEdge();

                // 切换到拖拽状态
                _petCore.StartDrag();
            }
        }

        private void PetForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragging)
            {
                System.Drawing.Point p = PointToScreen(e.Location);
                System.Drawing.Point newLocation = new System.Drawing.Point(p.X - _startPoint.X, p.Y - _startPoint.Y);

                // 关键：在拖拽时，UI的位置是主导，需要立即更新BLL
                this.Location = newLocation;
                _petCore.Position = newLocation;
            }
        }

        private void PetForm_MouseUp(object sender, MouseEventArgs e)
        {
            if (_isDragging)
            {
                _isDragging = false;

                // 切换到下落状态
                _petCore.EndDrag();
            }
        }

        /// <summary>
        /// 设置菜单项点击事件
        /// </summary>
        private void SettingsItem_Click(object sender, EventArgs e)
        {
            var settingsForm = new SettingsForm();
            settingsForm.ShowDialog(this);
        }

        /// <summary>
        /// 对话菜单项点击事件
        /// </summary>
        private void ChatItem_Click(object sender, EventArgs e)
        {
            // 显示自定义对话输入窗体
            using (var chatForm = new ChatForm())
            {
                if (chatForm.ShowDialog(this) == DialogResult.OK)
                {
                    // 异步处理对话
                    ProcessChatAsync(chatForm.UserInput);
                }
            }
        }

        /// <summary>
        /// 喂食菜单项点击事件
        /// </summary>
        private void FeedItem_Click(object sender, EventArgs e)
        {
            _petCore.TriggerEating();
        }

        /// <summary>
        /// 玩耍菜单项点击事件
        /// </summary>
        private void PlayItem_Click(object sender, EventArgs e)
        {
            _petCore.TriggerPlaying();
        }

        /// <summary>
        /// 放电菜单项点击事件
        /// </summary>
        private void ThunderShockItem_Click(object sender, EventArgs e)
        {
            _petCore.TriggerThunderShock();
        }

        /// <summary>
        /// 退出菜单项点击事件
        /// </summary>
        private void ExitItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        /// <summary>
        /// 关于菜单项点击事件
        /// </summary>
        private void AboutItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("桌面宠物 v2.0\n\n使用多种设计模式实现的皮卡丘桌面宠物\n\n功能：\n- 拖拽移动\n- 物理下落\n- 随机行走\n- 日程提醒\n- 状态切换动画\n\n设计模式：\n- 状态模式\n- 单例模式\n- 观察者模式",
                          "关于", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// 初始化气泡窗体
        /// </summary>
        private void InitializeBubble()
        {
            _bubbleForm = new BubbleForm();
        }

        /// <summary>
        /// 初始化日程提醒功能
        /// </summary>
        private void InitializeScheduleReminder()
        {
            // 订阅日程提醒事件
            ScheduleManager.Instance.OnReminderDue += (schedule) => {
                ShowBubble($"⏰ 叮！\n{schedule.Content}");
            };
        }

        /// <summary>
        /// 初始化对话服务
        /// </summary>
        private void InitializeDialogService()
        {
            // 使用DeepSeek AI服务
            _dialogService = new DeepSeekAdapter("sk-dff97f772b8149e8aa894562d4ff16b8");

            // 如果DeepSeek不可用，则回退到模拟服务
            if (!_dialogService.IsAvailable)
            {
                _dialogService = new MockDialogService();
            }
        }

        /// <summary>
        /// 处理聊天请求（从双击策略触发）
        /// </summary>
        private void HandleChatRequest()
        {
            // 显示自定义对话输入窗体
            using (var chatForm = new ChatForm())
            {
                if (chatForm.ShowDialog(this) == DialogResult.OK)
                {
                    // 异步处理对话
                    ProcessChatAsync(chatForm.UserInput);
                }
            }
        }

        /// <summary>
        /// 异步处理用户对话
        /// </summary>
        /// <param name="userInput">用户输入</param>
        private async void ProcessChatAsync(string userInput)
        {
            try
            {
                // 显示"思考中"的气泡，3秒后自动消失
                ShowBubble("皮卡皮卡... 🤔\n(思考中)", 3000);

                // 在思考时触发放电动作
                _petCore.TriggerThunderShock();

                // 获取AI回复
                string response = await _dialogService.GetResponseAsync(userInput);

                // 显示AI回复，5秒后自动消失
                ShowBubble(response, 5000);
            }
            catch (Exception ex)
            {
                // 错误处理，5秒后自动消失
                ShowBubble($"皮卡皮卡... 😵\n出现了一些问题：{ex.Message}", 5000);
            }
        }

        /// <summary>
        /// 在宠物旁边显示一个消息气泡
        /// </summary>
        /// <param name="message">要显示的消息</param>
        /// <param name="durationMilliseconds">显示时长（毫秒），0表示一直显示</param>
        private void ShowBubble(string message, int durationMilliseconds = 5000)
        {
            // 确保在UI线程上执行
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => ShowBubble(message, durationMilliseconds)));
                return;
            }

            _bubbleForm.SetMessage(message);

            // 计算气泡的位置（在宠物左上方）
            int bubbleX = this.Location.X - _bubbleForm.Width + 30; // 稍微向右偏移一点
            int bubbleY = this.Location.Y - _bubbleForm.Height;

            // 防止气泡超出屏幕左侧
            if (bubbleX < 0)
            {
                bubbleX = this.Location.X + this.Width - 30; // 如果左边放不下，就放到右边
            }

            // 防止气泡超出屏幕顶部
            if (bubbleY < 0)
            {
                bubbleY = this.Location.Y + this.Height + 10; // 如果上面放不下，就放到下面
            }

            _bubbleForm.Location = new System.Drawing.Point(bubbleX, bubbleY);
            _bubbleForm.Show();
            this.BringToFront(); // 确保宠物在气泡前面（可选）

            // 如果设置了显示时长，则在时长结束后自动关闭
            if (durationMilliseconds > 0)
            {
                var closeTimer = new Timer();
                closeTimer.Interval = durationMilliseconds;
                closeTimer.Tick += (s, e) =>
                {
                    _bubbleForm.Hide();
                    closeTimer.Stop();
                    closeTimer.Dispose();
                };
                closeTimer.Start();
            }
        }

        /// <summary>
        /// 隐藏气泡
        /// </summary>
        private void HideBubble()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(HideBubble));
                return;
            }
            _bubbleForm.Hide();
        }

        /// <summary>
        /// 窗体关闭时清理资源
        /// </summary>
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _bubbleForm?.Close();
            _bubbleForm?.Dispose();
            base.OnFormClosed(e);
        }

        /// <summary>
        /// 初始化双击策略
        /// </summary>
        private void InitializeDoubleClickStrategies()
        {
            _availableStrategies = new List<IDoubleClickActionStrategy>
            {
                new NoActionStrategy(),
                new ScreenshotStrategy(),
                new OpenWebsiteStrategy("https://github.com/", "打开GitHub"),
                new OpenWebsiteStrategy("https://www.bilibili.com/", "打开哔哩哔哩"),
                new OpenWebsiteStrategy("https://www.baidu.com/", "打开百度"),
                new WeatherReporterStrategy(), // 添加天气查询策略
                new ChatStrategy() // 添加聊天策略
                // 未来可以添加更多策略...
            };

            // 为所有策略订阅消息事件
            foreach (var strategy in _availableStrategies)
            {
                strategy.OnActionMessage += (message, duration) => ShowBubble(message, duration);

                // 如果是聊天策略，订阅聊天请求事件
                if (strategy is ChatStrategy chatStrategy)
                {
                    chatStrategy.OnChatRequested += HandleChatRequest;
                }
            }

            // 默认选择"无操作"
            _currentDoubleClickStrategy = _availableStrategies[0];
        }

        /// <summary>
        /// 双击事件处理
        /// </summary>
        private void PetForm_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            // 执行当前配置的双击策略
            _currentDoubleClickStrategy?.Execute();
        }

        /// <summary>
        /// 双击策略菜单项点击事件处理
        /// </summary>
        private void DoubleClickStrategy_Click(object sender, EventArgs e)
        {
            // 取消所有菜单项的选中状态
            var clickedItem = sender as ToolStripMenuItem;
            if (clickedItem?.OwnerItem is ToolStripMenuItem parentMenu)
            {
                foreach (ToolStripMenuItem item in parentMenu.DropDownItems)
                {
                    item.Checked = false;
                }
            }

            // 设置新的策略
            if (clickedItem != null)
            {
                clickedItem.Checked = true;
                _currentDoubleClickStrategy = clickedItem.Tag as IDoubleClickActionStrategy;
            }
        }
    }
}
