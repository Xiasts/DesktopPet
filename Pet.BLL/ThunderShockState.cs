using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Pet.Common;

namespace Pet.BLL
{
    /// <summary>
    /// 放电状态 - 宠物放电的动画状态
    /// </summary>
    public class ThunderShockState : IPetState
    {
        private List<Image> _animationFrames = new List<Image>();
        private int _currentFrameIndex = 0;
        private int _stateDuration = 0;
        private int _maxStateDuration;
        private int _animationTimer = 0; // 动画帧计时器
        private const int ANIMATION_SPEED = 8; // 每8帧更新一次动画（约0.27秒一帧，放电动作较快）

        public ThunderShockState()
        {
            LoadAnimationFrames();
            // 随机放电时间，2-4秒 (基于30FPS)
            _maxStateDuration = SharedRandom.Next(60, 120);

            // 调试信息
            try
            {
                string debugFile = Path.Combine(Directory.GetCurrentDirectory(), "pet_debug.txt");
                File.AppendAllText(debugFile, $"[{DateTime.Now:HH:mm:ss}] 进入ThunderShockState，持续时间: {_maxStateDuration}帧\n");
            }
            catch { }
        }

        /// <summary>
        /// 加载放电动画的图片帧
        /// </summary>
        private void LoadAnimationFrames()
        {
            try
            {
                string resourcesPath = GetResourcesPath();
                
                if (resourcesPath != null)
                {
                    // 加载放电动画图片
                    string[] thunderShockImages = { 
                        "Special_ThunderShock_1.png", 
                        "Special_ThunderShock_2.png", 
                        "Special_ThunderShock_3.png", 
                        "Special_ThunderShock_4.png", 
                        "Special_ThunderShock_5.png" 
                    };
                    
                    foreach (string imageName in thunderShockImages)
                    {
                        string imagePath = Path.Combine(resourcesPath, imageName);
                        if (File.Exists(imagePath))
                        {
                            _animationFrames.Add(Image.FromFile(imagePath));
                        }
                    }
                    

                }
            }
            catch (Exception)
            {
                // 创建错误提示图片
                Bitmap errorImage = new Bitmap(64, 64);
                using (Graphics g = Graphics.FromImage(errorImage))
                {
                    g.FillEllipse(Brushes.Yellow, 0, 0, 64, 64);
                    g.DrawString("⚡", new Font("Arial", 20), Brushes.Black, 20, 15);
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

        public void Update(PetCore core)
        {
            _stateDuration++;
            
            // 放电时间到了，切换回待机状态
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
