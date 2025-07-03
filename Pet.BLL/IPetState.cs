using System.Drawing;

namespace Pet.BLL
{
    /// <summary>
    /// 宠物状态接口 - 状态模式的核心接口
    /// </summary>
    public interface IPetState
    {
        /// <summary>
        /// 获取当前状态要显示的图片
        /// </summary>
        /// <returns>当前帧的图片</returns>
        Image GetImage();

        /// <summary>
        /// 状态自身的逻辑更新（例如，切换到下一个动画帧）
        /// </summary>
        /// <param name="core">PetCore实例，允许状态访问和修改核心属性</param>
        void Update(PetCore core);
    }
}
