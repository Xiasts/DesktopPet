using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Pet.Common;

namespace Pet.BLL
{
    /// <summary>
    /// 玩耍状态 - 宠物玩耍的动画状态
    /// </summary>
    public class PlayState : IPetState
    {
        private List<Image> _animationFrames = new List<Image>();
        private int _currentFrameIndex = 0;
        private int _stateDuration = 0;
        private int _maxStateDuration;
        private int _animationTimer = 0; // 动画帧计时器
        private const int ANIMATION_SPEED = 12; // 每12帧更新一次动画（约0.4秒一帧，玩耍动作适中）


        public PlayState()
        {
            LoadAnimationFrames();
            // 随机玩耍时间，3-6秒 (基于30FPS)
            _maxStateDuration = SharedRandom.Next(90, 180);

            // 调试信息
            try
            {
                string debugFile = Path.Combine(Directory.GetCurrentDirectory(), "pet_debug.txt");
                File.AppendAllText(debugFile, $"[{DateTime.Now:HH:mm:ss}] 进入PlayState，持续时间: {_maxStateDuration}帧\n");
            }
            catch { }
        }

        /// <summary>
        /// 加载玩耍动画的图片帧
        /// </summary>
        private void LoadAnimationFrames()
        {
            try
            {
                string resourcesPath = GetResourcesPath();
                
                if (resourcesPath != null)
                {
                    // 加载玩耍动画图片
                    string[] playImages = { "enjoy_oneself_1.png", "enjoy_oneself_2.png", "enjoy_oneself_3.png", "enjoy_oneself_4.png" };
                    
                    foreach (string imageName in playImages)
                    {
                        string imagePath = Path.Combine(resourcesPath, imageName);
                        if (File.Exists(imagePath))
                        {
                            _animationFrames.Add(Image.FromFile(imagePath));
                        }
                    }
                    
                    // 如果没有找到新的玩耍图片，使用旧的图片作为备用
                    if (_animationFrames.Count == 0)
                    {
                        for (int i = 4; i <= 7; i++)
                        {
                            string imagePath = Path.Combine(resourcesPath, $"shime{i}.png");
                            if (File.Exists(imagePath))
                            {
                                _animationFrames.Add(Image.FromFile(imagePath));
                            }
                        }
                    }
                }

                // 如果还是没有找到任何图片，创建默认图片
                if (_animationFrames.Count == 0)
                {
                    CreateDefaultFrames();
                }
            }
            catch (Exception)
            {
                CreateDefaultFrames();
            }
        }

        private void CreateDefaultFrames()
        {
            // 创建默认的玩耍图片
            Bitmap playImage = new Bitmap(64, 64);
            using (Graphics g = Graphics.FromImage(playImage))
            {
                g.FillEllipse(Brushes.LightGreen, 0, 0, 64, 64);
                g.DrawString("🎮", new Font("Arial", 20), Brushes.Black, 15, 20);
            }
            _animationFrames.Add(playImage);
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
            _stateDuration++;
            
            // 玩耍时间到了，切换回待机状态
            if (_stateDuration > _maxStateDuration)
            {
                core.SetState(new IdleState());
                return;
            }

            // 控制动画帧更新频率
            _animationTimer++;
            if (_animationFrames.Count > 0 && _animationTimer >= ANIMATION_SPEED)
            {
                _animationTimer = 0;
                _currentFrameIndex++;
                if (_currentFrameIndex >= _animationFrames.Count)
                {
                    _currentFrameIndex = 0;
                }
            }
        }

        public Image GetImage()
        {
            if (_animationFrames.Count == 0) return null;
            return _animationFrames[_currentFrameIndex % _animationFrames.Count];
        }
    }
}
