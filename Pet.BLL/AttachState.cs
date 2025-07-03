using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Pet.Common;

namespace Pet.BLL
{
    /// <summary>
    /// 吸附状态 - 当宠物吸附到屏幕边缘时的状态
    /// </summary>
    public class AttachState : IPetState
    {
        public enum AttachDirection
        {
            Left,
            Right,
            Top
        }

        private List<Image> _attachLeftFrames = new List<Image>();
        private List<Image> _attachRightFrames = new List<Image>();
        private List<Image> _attachTopFrames = new List<Image>();
        private int _currentFrameIndex = 0;
        private AttachDirection _attachDirection;
        private int _attachDuration = 0;
        private int _maxAttachDuration;

        private Point _fixedPosition; // 固定位置
        private int _animationTimer = 0; // 动画帧计时器
        private const int ANIMATION_SPEED = 20; // 每20帧更新一次动画（约0.66秒一帧，吸附状态较慢）

        public AttachState(AttachDirection direction)
        {
            _attachDirection = direction;
            LoadAnimationFrames();
            // 随机吸附时间，让行为更自然 (基于30FPS: 8-20秒)
            _maxAttachDuration = SharedRandom.Next(240, 600);
        }

        // 保持向后兼容的构造函数
        public AttachState(bool attachToLeft) : this(attachToLeft ? AttachDirection.Left : AttachDirection.Right)
        {
        }

        /// <summary>
        /// 加载吸附动画的图片帧
        /// </summary>
        private void LoadAnimationFrames()
        {
            try
            {
                string resourcesPath = GetResourcesPath();
                
                if (resourcesPath != null)
                {
                    // 加载左侧吸附动画帧（爬墙向左）
                    string[] leftAttachImages = { "Climb_Wall_left_1.png", "Climb_Wall_left_2.png", "Climb_Wall_left_3.png" };
                    foreach (string imageName in leftAttachImages)
                    {
                        string imagePath = Path.Combine(resourcesPath, imageName);
                        if (File.Exists(imagePath))
                        {
                            _attachLeftFrames.Add(Image.FromFile(imagePath));
                        }
                    }

                    // 加载顶部吸附动画帧（爬墙向上）
                    string[] topAttachImages = { "Climb_Wall_up_1.png", "Climb_Wall_up_2.png" };
                    foreach (string imageName in topAttachImages)
                    {
                        string imagePath = Path.Combine(resourcesPath, imageName);
                        if (File.Exists(imagePath))
                        {
                            _attachTopFrames.Add(Image.FromFile(imagePath));
                        }
                    }

                    // 如果没有找到新的吸附图片，使用旧的图片作为备用
                    if (_attachLeftFrames.Count == 0)
                    {
                        LoadFrames(_attachLeftFrames, new[] { 28, 29, 30, 31 }, resourcesPath);
                    }

                    if (_attachTopFrames.Count == 0)
                    {
                        LoadFrames(_attachTopFrames, new[] { 32, 33, 34, 35 }, resourcesPath);
                    }

                    // 创建右侧吸附帧（左右翻转）
                    CreateFlippedFrames();
                }

                // 如果还是没有找到任何图片，创建默认图片
                if (_attachLeftFrames.Count == 0)
                {
                    CreateDefaultFrames();
                }
            }
            catch (Exception)
            {
                CreateDefaultFrames();
            }
        }

        private void LoadFrames(List<Image> frameList, int[] frameNumbers, string resourcesPath)
        {
            foreach (int frameNum in frameNumbers)
            {
                string imagePath = Path.Combine(resourcesPath, $"shime{frameNum}.png");
                if (File.Exists(imagePath))
                {
                    frameList.Add(Image.FromFile(imagePath));
                }
            }
        }

        /// <summary>
        /// 创建右侧吸附帧（通过翻转左侧帧）
        /// </summary>
        private void CreateFlippedFrames()
        {
            foreach (Image leftFrame in _attachLeftFrames)
            {
                Bitmap flippedFrame = new Bitmap(leftFrame.Width, leftFrame.Height);
                using (Graphics g = Graphics.FromImage(flippedFrame))
                {
                    // 设置高质量渲染
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

                    // 水平翻转图片
                    g.ScaleTransform(-1, 1);
                    g.TranslateTransform(-leftFrame.Width, 0);
                    g.DrawImage(leftFrame, 0, 0, leftFrame.Width, leftFrame.Height);
                }
                _attachRightFrames.Add(flippedFrame);
            }
        }

        private void CreateDefaultFrames()
        {
            // 创建默认的左侧吸附图片
            Bitmap leftImage = new Bitmap(64, 64);
            using (Graphics g = Graphics.FromImage(leftImage))
            {
                g.FillEllipse(Brushes.LightSalmon, 0, 0, 64, 64);
                g.DrawString("◀", new Font("Arial", 20), Brushes.Black, 15, 20);
            }
            _attachLeftFrames.Add(leftImage);

            // 创建默认的右侧吸附图片
            Bitmap rightImage = new Bitmap(64, 64);
            using (Graphics g = Graphics.FromImage(rightImage))
            {
                g.FillEllipse(Brushes.LightSalmon, 0, 0, 64, 64);
                g.DrawString("▶", new Font("Arial", 20), Brushes.Black, 15, 20);
            }
            _attachRightFrames.Add(rightImage);

            // 创建默认的顶部吸附图片
            Bitmap topImage = new Bitmap(64, 64);
            using (Graphics g = Graphics.FromImage(topImage))
            {
                g.FillEllipse(Brushes.LightBlue, 0, 0, 64, 64);
                g.DrawString("▲", new Font("Arial", 20), Brushes.Black, 20, 15);
            }
            _attachTopFrames.Add(topImage);
        }

        private string GetResourcesPath()
        {
            string[] possiblePaths = {
                Path.Combine(Directory.GetCurrentDirectory(), "Resources"),
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources"),
                Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "Resources")
            };
            
            foreach (string path in possiblePaths)
            {
                if (Directory.Exists(path))
                {
                    return path;
                }
            }
            return null;
        }

        public void Update(PetCore core)
        {
            // 第一次进入吸附状态时，记录固定位置
            if (_attachDuration == 0)
            {
                _fixedPosition = core.Position;
            }

            _attachDuration++;

            // 强制保持固定位置（防止任何位置变化）
            core.Position = _fixedPosition;

            // 吸附时间到了，切换回待机状态
            if (_attachDuration > _maxAttachDuration)
            {
                core.SetState(new IdleState());
                return;
            }

            // 控制动画帧更新频率
            _animationTimer++;
            if (_animationTimer >= ANIMATION_SPEED)
            {
                _animationTimer = 0;

                List<Image> currentFrames;
                switch (_attachDirection)
                {
                    case AttachDirection.Left:
                        currentFrames = _attachLeftFrames;
                        break;
                    case AttachDirection.Right:
                        currentFrames = _attachRightFrames;
                        break;
                    case AttachDirection.Top:
                        currentFrames = _attachTopFrames;
                        break;
                    default:
                        currentFrames = _attachLeftFrames;
                        break;
                }

                if (currentFrames.Count > 0)
                {
                    _currentFrameIndex++;
                    if (_currentFrameIndex >= currentFrames.Count)
                    {
                        _currentFrameIndex = 0;
                    }
                }
            }
        }

        public Image GetImage()
        {
            List<Image> currentFrames;
            switch (_attachDirection)
            {
                case AttachDirection.Left:
                    currentFrames = _attachLeftFrames;
                    break;
                case AttachDirection.Right:
                    currentFrames = _attachRightFrames;
                    break;
                case AttachDirection.Top:
                    currentFrames = _attachTopFrames;
                    break;
                default:
                    currentFrames = _attachLeftFrames;
                    break;
            }

            if (currentFrames.Count == 0) return null;
            return currentFrames[_currentFrameIndex % currentFrames.Count];
        }

        /// <summary>
        /// 检查是否吸附到左侧
        /// </summary>
        public bool IsAttachedToLeft => _attachDirection == AttachDirection.Left;

        /// <summary>
        /// 检查是否吸附到右侧
        /// </summary>
        public bool IsAttachedToRight => _attachDirection == AttachDirection.Right;

        /// <summary>
        /// 检查是否吸附到顶部
        /// </summary>
        public bool IsAttachedToTop => _attachDirection == AttachDirection.Top;

        /// <summary>
        /// 获取吸附方向
        /// </summary>
        public AttachDirection Direction => _attachDirection;
    }
}
