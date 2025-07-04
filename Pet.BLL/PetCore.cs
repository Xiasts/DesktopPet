using System.Drawing;

namespace Pet.BLL
{
    /// <summary>
    /// 宠物核心控制器 - 状态模式的上下文类
    /// 负责管理宠物的当前状态和状态切换
    /// </summary>
    public class PetCore
    {
        private IPetState _currentState;
        private FallState _fallState; // 保持下落状态的引用以检查完成状态

        /// <summary>
        /// 宠物在屏幕上的位置
        /// </summary>
        public Point Position { get; set; }

        /// <summary>
        /// 屏幕边界大小
        /// </summary>
        public Size ScreenBounds { get; set; }

        /// <summary>
        /// 吸附检测距离（像素）
        /// </summary>
        private const int ATTACH_DISTANCE = 50;

        /// <summary>
        /// 吸附动画速度
        /// </summary>
        private const int ATTACH_SPEED = 8;

        /// <summary>
        /// 吸附时距离边框的像素距离
        /// </summary>
        private const int ATTACH_EDGE_OFFSET = 0;

        public PetCore()
        {
            // 初始状态为待机状态
            _currentState = new IdleState();

            // 订阅日程提醒事件（观察者模式）
            ScheduleManager.Instance.OnReminderDue += HandleReminder;
        }

        /// <summary>
        /// 让外部（UI层）可以获取当前需要显示的图片
        /// </summary>
        /// <returns>当前状态的图片</returns>
        public Image GetCurrentImage()
        {
            return _currentState?.GetImage();
        }

        /// <summary>
        /// 每次Timer触发时调用，驱动状态更新
        /// </summary>
        public void Update()
        {
            // 检查日程提醒
            ScheduleManager.Instance.CheckReminders();

            // 更新当前状态
            _currentState?.Update(this); // 将自身传递给状态

            // 在状态更新后检查屏幕边缘吸附（但不在拖拽、下落和吸附状态时检查）
            if (!IsInState<DragState>() && !IsInState<FallState>() && !IsInState<AttachState>())
            {
                CheckScreenAttachment();
            }
        }

        /// <summary>
        /// 切换状态的方法 - 为未来功能扩展预留
        /// </summary>
        /// <param name="newState">新的状态</param>
        public void SetState(IPetState newState)
        {
            if (newState != null)
            {
                _currentState = newState;
            }
        }

        /// <summary>
        /// 获取当前状态类型（用于调试和状态判断）
        /// </summary>
        /// <returns>当前状态的类型名称</returns>
        public string GetCurrentStateType()
        {
            return _currentState?.GetType().Name ?? "Unknown";
        }

        /// <summary>
        /// 开始拖拽 - 切换到拖拽状态
        /// </summary>
        public void StartDrag()
        {
            SetState(new DragState());
        }

        /// <summary>
        /// 结束拖拽 - 检查吸附或切换到下落状态
        /// </summary>
        public void EndDrag()
        {
            // 先检查是否应该吸附到边缘
            int imageWidth = 64; // 默认宽度
            int imageHeight = 64; // 默认高度
            var currentImage = GetCurrentImage();
            if (currentImage != null)
            {
                imageWidth = currentImage.Width;
                imageHeight = currentImage.Height;
            }

            // 检测顶部吸附（优先级最高）
            if (Position.Y <= ATTACH_DISTANCE)
            {
                Position = new Point(Position.X, -imageHeight / 3);
                SetState(new AttachState(AttachState.AttachDirection.Top));
                return;
            }
            // 检测左边缘吸附
            else if (Position.X <= ATTACH_DISTANCE)
            {
                Position = new Point(-imageWidth / 3, Position.Y);
                SetState(new AttachState(AttachState.AttachDirection.Left));
                return;
            }
            // 检测右边缘吸附
            else if (Position.X >= ScreenBounds.Width - imageWidth - ATTACH_DISTANCE)
            {
                Position = new Point(ScreenBounds.Width - imageWidth * 2 / 3, Position.Y);
                SetState(new AttachState(AttachState.AttachDirection.Right));
                return;
            }

            // 如果没有吸附，则进入下落状态
            _fallState = new FallState();
            SetState(_fallState);
        }

        /// <summary>
        /// 切换到待机状态
        /// </summary>
        public void SetIdle()
        {
            SetState(new IdleState());
        }

        /// <summary>
        /// 检查当前是否为指定状态
        /// </summary>
        /// <typeparam name="T">状态类型</typeparam>
        /// <returns>如果是指定状态返回true</returns>
        public bool IsInState<T>() where T : IPetState
        {
            return _currentState is T;
        }

        /// <summary>
        /// 触发喂食状态 - 从右键菜单调用
        /// </summary>
        public void TriggerEating()
        {
            // 只有在非拖拽状态时才能触发
            if (!IsInState<DragState>())
            {
                SetState(new CookieState());
            }
        }

        /// <summary>
        /// 触发玩耍状态 - 从右键菜单调用
        /// </summary>
        public void TriggerPlaying()
        {
            // 只有在非拖拽状态时才能触发
            if (!IsInState<DragState>())
            {
                SetState(new PlayState());
            }
        }

        /// <summary>
        /// 触发放电状态 - 从右键菜单调用
        /// </summary>
        public void TriggerThunderShock()
        {
            // 只有在非拖拽状态时才能触发
            if (!IsInState<DragState>())
            {
                SetState(new ThunderShockState());
            }
        }

        /// <summary>
        /// 处理日程提醒事件
        /// </summary>
        /// <param name="schedule">到期的日程</param>
        private void HandleReminder(Pet.Model.Schedule schedule)
        {
            // 这里可以切换到提醒状态，或者触发其他行为
            // 暂时先让宠物回到待机状态并可能显示提醒
            if (!IsInState<IdleState>())
            {
                SetIdle();
            }

            // 可以在这里添加更多提醒逻辑，比如：
            // - 切换到特殊的提醒状态
            // - 播放提醒音效
            // - 显示提醒气泡等
        }

        /// <summary>
        /// 检测是否应该吸附到屏幕边缘
        /// </summary>
        public void CheckScreenAttachment()
        {
            // 只有在待机状态时才检测吸附，吸附状态下不再检测
            if (!IsInState<IdleState>())
                return;

            int imageWidth = 64; // 默认宽度
            int imageHeight = 64; // 默认高度
            var currentImage = GetCurrentImage();
            if (currentImage != null)
            {
                imageWidth = currentImage.Width;
                imageHeight = currentImage.Height;
            }

            // 检测顶部吸附（优先级最高）
            if (Position.Y <= ATTACH_DISTANCE)
            {
                // 吸附到顶部，图片1/3在屏幕外
                Position = new Point(Position.X, -imageHeight / 3);
                SetState(new AttachState(AttachState.AttachDirection.Top));
            }
            // 检测左边缘吸附
            else if (Position.X <= ATTACH_DISTANCE)
            {
                // 吸附到左边缘，图片1/3在屏幕外
                Position = new Point(-imageWidth / 3, Position.Y);
                SetState(new AttachState(AttachState.AttachDirection.Left));
            }
            // 检测右边缘吸附
            else if (Position.X >= ScreenBounds.Width - imageWidth - ATTACH_DISTANCE)
            {
                // 吸附到右边缘，图片1/3在屏幕外
                Position = new Point(ScreenBounds.Width - imageWidth * 2 / 3, Position.Y);
                SetState(new AttachState(AttachState.AttachDirection.Right));
            }
        }

        /// <summary>
        /// 从吸附状态脱离
        /// </summary>
        public void DetachFromEdge()
        {
            if (IsInState<AttachState>())
            {
                SetState(new IdleState());
            }
        }
    }
}
