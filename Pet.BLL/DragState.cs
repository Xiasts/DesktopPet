using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace Pet.BLL
{
    /// <summary>
    /// 拖拽状态 - 当用户拖拽宠物时的状态
    /// </summary>
    public class DragState : IPetState
    {
        private List<Image> _animationFrames = new List<Image>();
        private int _currentFrameIndex = 0;

        // 动画速度控制
        private int _animationTimer = 0;
        private const int ANIMATION_SPEED = 5; // 每5个Tick更新一帧 (5 * 33ms ≈ 165ms一帧)

        public DragState()
        {
            LoadAnimationFrames();
        }

        /// <summary>
        /// 加载拖拽动画的图片帧
        /// </summary>
        private void LoadAnimationFrames()
        {
            try
            {
                // 获取正确的Resources路径
                string resourcesPath = GetResourcesPath();

                if (resourcesPath != null)
                {
                    // 使用被拖拽的动画图片
                    string[] dragImages = { "Being_carried_1.png", "Being_carried_2.png", "Being_carried_3.png",
                                          "Being_carried_4.png", "Being_carried_5.png", "Being_carried_6.png" };

                    foreach (string imageName in dragImages)
                    {
                        string imagePath = Path.Combine(resourcesPath, imageName);
                        if (File.Exists(imagePath))
                        {
                            _animationFrames.Add(Image.FromFile(imagePath));
                        }
                    }

                    // 如果没有找到新的拖拽图片，使用旧的图片作为备用
                    if (_animationFrames.Count == 0)
                    {
                        int[] backupFrames = { 9, 10, 11, 12 };
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
                    // 创建默认拖拽图片
                    Bitmap defaultImage = new Bitmap(64, 64);
                    using (Graphics g = Graphics.FromImage(defaultImage))
                    {
                        g.FillEllipse(Brushes.LightBlue, 0, 0, 64, 64);
                        g.DrawString("Drag", SystemFonts.DefaultFont, Brushes.Black, 15, 25);
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
                    g.FillEllipse(Brushes.Orange, 0, 0, 64, 64);
                    g.DrawString("Drag", SystemFonts.DefaultFont, Brushes.Black, 15, 25);
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
                _animationTimer = 0; // 重置计时器
                _currentFrameIndex++;
                if (_currentFrameIndex >= _animationFrames.Count)
                {
                    _currentFrameIndex = 0;
                }
            }
        }
    }
}
