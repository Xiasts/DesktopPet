using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Pet.Common;

namespace Pet.BLL
{
    /// <summary>
    /// 待机状态 - 宠物的默认状态，播放待机动画
    /// </summary>
    public class IdleState : IPetState
    {
        private List<Image> _animationFrames = new List<Image>();
        private int _currentFrameIndex = 0;
        private int _idleTimer = 0;
        private int _timeToAct; // 不再是"感到无聊"，而是"该行动了"
        private int _animationTimer = 0; // 动画帧计时器
        private const int ANIMATION_SPEED = 15; // 每15帧更新一次动画（约0.5秒一帧）

        public IdleState()
        {
            LoadAnimationFrames();
            // 随机一个下一次行动的时间，5到12秒
            // Timer的Interval是33ms (约30fps)，所以1秒约30帧
            // 5秒约150帧，12秒约360帧
            _timeToAct = SharedRandom.Next(100, 200);
        }

        /// <summary>
        /// 加载待机动画的所有图片帧
        /// </summary>
        private void LoadAnimationFrames()
        {
            try
            {
                // 获取当前工作目录
                string currentDir = Directory.GetCurrentDirectory();

                // 写入调试信息到文件
                string debugFile = Path.Combine(currentDir, "debug.txt");
                File.WriteAllText(debugFile, $"当前工作目录: {currentDir}\n");

                // 尝试多种路径
                string[] possiblePaths = {
                    Path.Combine(currentDir, "Resources"),
                    Path.Combine(currentDir, "..", "..", "..", "Pet.UI", "Resources"),
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources"),
                    Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "Resources")
                };

                File.AppendAllText(debugFile, "尝试的路径:\n");
                foreach (string path in possiblePaths)
                {
                    File.AppendAllText(debugFile, $"- {path} (存在: {Directory.Exists(path)})\n");
                }

                string resourcesPath = null;
                foreach (string path in possiblePaths)
                {
                    if (Directory.Exists(path))
                    {
                        resourcesPath = path;
                        break;
                    }
                }

                if (resourcesPath != null)
                {
                    File.AppendAllText(debugFile, $"使用路径: {resourcesPath}\n");

                    // 加载皮卡丘待机动画图片
                    string[] idleImages = { "Sit_Idle.png" };

                    foreach (string imageName in idleImages)
                    {
                        string imagePath = Path.Combine(resourcesPath, imageName);
                        File.AppendAllText(debugFile, $"检查图片: {imagePath} (存在: {File.Exists(imagePath)})\n");

                        if (File.Exists(imagePath))
                        {
                            _animationFrames.Add(Image.FromFile(imagePath));
                            File.AppendAllText(debugFile, $"成功加载: {imagePath}\n");
                        }
                    }


                }

                // 如果没有找到图片，创建默认占位图片
                if (_animationFrames.Count == 0)
                {
                    File.AppendAllText(debugFile, "没有找到任何图片文件，使用默认占位图片\n");
                    Bitmap defaultImage = new Bitmap(64, 64);
                    using (Graphics g = Graphics.FromImage(defaultImage))
                    {
                        g.FillEllipse(Brushes.Pink, 0, 0, 64, 64);
                        g.DrawString("Pet", SystemFonts.DefaultFont, Brushes.Black, 20, 25);
                    }
                    _animationFrames.Add(defaultImage);
                }
                else
                {
                    File.AppendAllText(debugFile, $"总共加载了 {_animationFrames.Count} 帧动画\n");
                }
            }
            catch (Exception ex)
            {
                // 创建错误提示图片
                Bitmap errorImage = new Bitmap(100, 50);
                using (Graphics g = Graphics.FromImage(errorImage))
                {
                    g.FillRectangle(Brushes.Red, 0, 0, 100, 50);
                    g.DrawString("Error", SystemFonts.DefaultFont, Brushes.White, 10, 15);
                }
                _animationFrames.Add(errorImage);

                // 写入错误信息
                try
                {
                    string debugFile = Path.Combine(Directory.GetCurrentDirectory(), "debug.txt");
                    File.AppendAllText(debugFile, $"错误: {ex.Message}\n{ex.StackTrace}\n");
                }
                catch { }
            }
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

            _idleTimer++;

            // 当计时器到达了我们预设的"行动时间"
            if (_idleTimer > _timeToAct)
            {
                // 决定下一步做什么
                ChooseNextAction(core);
            }
        }

        /// <summary>
        /// 选择下一个行动 - 保证宠物会有行为变化
        /// </summary>
        private void ChooseNextAction(PetCore core)
        {
            int actionChoice = SharedRandom.Next(0, 100); // 摇一次奖，决定做什么

            // 调试信息
            try
            {
                string debugFile = Path.Combine(Directory.GetCurrentDirectory(), "pet_debug.txt");
                File.AppendAllText(debugFile, $"[{DateTime.Now:HH:mm:ss}] 选择行动，计时器: {_idleTimer}，随机数: {actionChoice}\n");
            }
            catch { }

            // 根据随机数分配概率
            if (actionChoice < 50) // 50% 概率吃Cookie
            {
                core.SetState(new CookieState());
            }
            else if (actionChoice < 90) // 40% 概率玩耍
            {
                core.SetState(new PlayState());
            }
            else // 10% 概率继续发呆 (什么都不做，重置计时器)
            {
                _idleTimer = 0;
                _timeToAct = SharedRandom.Next(120, 240); // 下次发呆时间短一点，4-8秒

                // 调试信息
                try
                {
                    string debugFile = Path.Combine(Directory.GetCurrentDirectory(), "pet_debug.txt");
                    File.AppendAllText(debugFile, $"[{DateTime.Now:HH:mm:ss}] 选择继续发呆，新的行动时间: {_timeToAct}帧\n");
                }
                catch { }
            }
            // 一旦决定了动作并切换状态，这个IdleState实例就会被销毁，
            // 当宠物再次进入IdleState时，会创建一个新的实例，计时器会自动重置。
        }
    }
}
