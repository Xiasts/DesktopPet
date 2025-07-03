using System;
using System.Drawing;
using System.Windows.Forms;
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

        public PetForm()
        {
            InitializeComponent();
            _petCore = new PetCore(); // 创建BLL核心实例

            // 初始化位置和屏幕边界
            _petCore.Position = this.Location;
            _petCore.ScreenBounds = Screen.PrimaryScreen.Bounds.Size;

            InitializeContextMenu(); // 初始化右键菜单
            InitializeBubble(); // 初始化气泡窗体
            InitializeScheduleReminder(); // 初始化日程提醒
            InitializeDialogService(); // 初始化对话服务
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

            // 添加关于菜单项
            ToolStripMenuItem aboutItem = new ToolStripMenuItem("关于");
            aboutItem.Click += AboutItem_Click;
            _contextMenu.Items.Add(aboutItem);

            // 添加退出菜单项
            ToolStripMenuItem exitItem = new ToolStripMenuItem("退出");
            exitItem.Click += ExitItem_Click;
            _contextMenu.Items.Add(exitItem);

            // 将右键菜单绑定到PictureBox
            picPet.ContextMenuStrip = _contextMenu;
        }

        private void animationTimer_Tick(object sender, EventArgs e)
        {
            // 1. 更新BLL中的状态
            _petCore.Update();

            // 2. 从BLL获取当前要显示的图片
            Image petImage = _petCore.GetCurrentImage();

            // 3. 将图片显示在UI上
            picPet.Image = petImage;

            // 4. 让窗体大小自适应图片大小
            this.ClientSize = picPet.Size;

            // 5. 同步位置（从BLL获取更新后的位置）
            if (this.Location != _petCore.Position)
            {
                this.Location = _petCore.Position;
            }
        }

        private void picPet_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _isDragging = true;
                _startPoint = new Point(e.X, e.Y);

                // 如果当前是吸附状态，先脱离吸附
                _petCore.DetachFromEdge();

                // 切换到拖拽状态
                _petCore.StartDrag();
            }
        }

        private void picPet_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragging)
            {
                Point p = PointToScreen(e.Location);
                Point newLocation = new Point(p.X - _startPoint.X, p.Y - _startPoint.Y);
                Location = newLocation;

                // 同步位置到PetCore
                _petCore.Position = newLocation;
            }
        }

        private void picPet_MouseUp(object sender, MouseEventArgs e)
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
        /// 异步处理用户对话
        /// </summary>
        /// <param name="userInput">用户输入</param>
        private async void ProcessChatAsync(string userInput)
        {
            try
            {
                // 显示"思考中"的气泡，3秒后自动消失
                ShowBubble("皮卡皮卡... 🤔\n(思考中)", 3000);

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

            _bubbleForm.Location = new Point(bubbleX, bubbleY);
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
    }
}
