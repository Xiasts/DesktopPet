using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace Pet.BLL
{
    /// <summary>
    /// 下落状态 - 当松开鼠标后宠物下落时的状态
    /// </summary>
    public class FallState : IPetState
    {
        private List<Image> _animationFrames = new List<Image>();
        private int _currentFrameIndex = 0;
        private int _gravity = 8; // 每次下落的像素数（重力加速度）
        private int _velocity = 0; // 当前下落速度

        // 动画速度控制
        private int _animationTimer = 0;
        private const int ANIMATION_SPEED = 4; // 下落动画可以快一点，每4个Tick更新一次

        public FallState()
        {
            LoadAnimationFrames();
        }

        /// <summary>
        /// 加载下落动画的图片帧
        /// </summary>
        private void LoadAnimationFrames()
        {
            try
            {
                // 获取正确的Resources路径
                string resourcesPath = GetResourcesPath();

                if (resourcesPath != null)
                {
                    // 使用下落/躺下相关的动画图片
                    string[] fallImages = { "LieDown_Sad_1.png", "LieDown_Sad_2.png", "LieDown_Sad_3.png" };

                    foreach (string imageName in fallImages)
                    {
                        string imagePath = Path.Combine(resourcesPath, imageName);
                        if (File.Exists(imagePath))
                        {
                            _animationFrames.Add(Image.FromFile(imagePath));
                        }
                    }

                    // 如果没有找到新的下落图片，使用旧的图片作为备用
                    if (_animationFrames.Count == 0)
                    {
                        int[] backupFrames = { 13, 14, 15, 16 };
                        foreach (int frameNum in backupFrames)
                        {
                            string imagePath = Path.Combine(resourcesPath, $"shime{frameNum}.png");
                            if (File.Exists(imagePath))
                            {
                                _animationFrames.Add(Image.FromFile(imagePath));
                            }
                        }
                    }

                    // 如果还是没有，至少加载一帧
                    if (_animationFrames.Count == 0)
                    {
                        string imagePath = Path.Combine(resourcesPath, "shime1.png");
                        if (File.Exists(imagePath))
                        {
                            _animationFrames.Add(Image.FromFile(imagePath));
                        }
                    }
                }

                // 如果还是没有找到任何图片，创建默认图片
                if (_animationFrames.Count == 0)
                {
                    // 创建默认下落图片
                    Bitmap defaultImage = new Bitmap(64, 64);
                    using (Graphics g = Graphics.FromImage(defaultImage))
                    {
                        g.FillEllipse(Brushes.LightGreen, 0, 0, 64, 64);
                        g.DrawString("Fall", SystemFonts.DefaultFont, Brushes.Black, 15, 25);
                    }
                    _animationFrames.Add(defaultImage);
                }
            }
            catch (Exception)
            {
                // 创建错误提示图片
                Bitmap errorImage = new Bitmap(64, 64);
                using (Graphics g = Graphics.FromImage(errorImage))
                {
                    g.FillEllipse(Brushes.Yellow, 0, 0, 64, 64);
                    g.DrawString("Fall", SystemFonts.DefaultFont, Brushes.Black, 15, 25);
                }
                _animationFrames.Add(errorImage);
            }
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

        public Image GetImage()
        {
            if (_animationFrames.Count > 0)
            {
                return _animationFrames[_currentFrameIndex];
            }
            return null;
        }

        public void Update(PetCore core)
        {
            // 控制动画帧更新频率
            _animationTimer++;
            if (_animationFrames.Count > 1 && _animationTimer >= ANIMATION_SPEED)
            {
                _animationTimer = 0;
                _currentFrameIndex++;
                if (_currentFrameIndex >= _animationFrames.Count)
                {
                    _currentFrameIndex = 0;
                }
            }

            // 获取当前图片的尺寸
            int imageHeight = 64; // 默认高度
            int imageWidth = 64;  // 默认宽度
            if (_animationFrames.Count > 0 && _animationFrames[_currentFrameIndex] != null)
            {
                imageHeight = _animationFrames[_currentFrameIndex].Height;
                imageWidth = _animationFrames[_currentFrameIndex].Width;
            }

            // 在下落过程中检查边缘吸附
            const int ATTACH_DISTANCE = 50;

            // 检测顶部吸附（虽然下落时不太可能，但为了完整性）
            if (core.Position.Y <= ATTACH_DISTANCE)
            {
                core.Position = new Point(core.Position.X, -imageHeight / 3);
                core.SetState(new AttachState(AttachState.AttachDirection.Top));
                return;
            }
            // 检测左边缘吸附
            else if (core.Position.X <= ATTACH_DISTANCE)
            {
                core.Position = new Point(-imageWidth / 3, core.Position.Y);
                core.SetState(new AttachState(AttachState.AttachDirection.Left));
                return;
            }
            // 检测右边缘吸附
            else if (core.Position.X >= core.ScreenBounds.Width - imageWidth - ATTACH_DISTANCE)
            {
                core.Position = new Point(core.ScreenBounds.Width - imageWidth * 2 / 3, core.Position.Y);
                core.SetState(new AttachState(AttachState.AttachDirection.Right));
                return;
            }

            // 应用重力，增加下落速度
            _velocity += _gravity;

            // 计算新位置
            int newY = core.Position.Y + _velocity;

            // 检查是否到达屏幕底部
            int groundLevel = core.ScreenBounds.Height - imageHeight - 50; // 留一点边距
            if (newY >= groundLevel)
            {
                newY = groundLevel; // 贴在地面上
                core.Position = new Point(core.Position.X, newY);
                core.SetState(new IdleState()); // 落地，切换回待机状态
                return;
            }

            // 更新位置
            core.Position = new Point(core.Position.X, newY);
        }
    }
}
