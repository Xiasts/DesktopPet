# 贡献指南 (Contributing Guide)

感谢你对皮卡丘桌面宠物项目的关注！我们欢迎所有形式的贡献。

## 🤝 如何贡献

### 报告Bug
1. 在[Issues](https://github.com/Xiasts/DesktopPet/issues)中搜索是否已有相同问题
2. 如果没有，创建新的Issue
3. 使用Bug报告模板，提供详细信息：
   - 操作系统版本
   - .NET Framework版本
   - 重现步骤
   - 期望行为
   - 实际行为
   - 截图（如果适用）

### 功能建议
1. 在Issues中创建Feature Request
2. 详细描述建议的功能
3. 说明为什么这个功能有用
4. 如果可能，提供实现思路

### 代码贡献

#### 开发环境设置
1. Fork本项目到你的GitHub账户
2. 克隆你的Fork到本地
```bash
git clone https://github.com/你的用户名/DesktopPet.git
cd DesktopPet
```
3. 添加上游仓库
```bash
git remote add upstream https://github.com/Xiasts/DesktopPet.git
```
4. 安装开发依赖
   - Visual Studio 2019+
   - .NET Framework 4.7.2+

#### 开发流程
1. 创建新分支
```bash
git checkout -b feature/你的功能名称
```
2. 进行开发
3. 提交更改
```bash
git add .
git commit -m "feat: 添加新功能描述"
```
4. 推送到你的Fork
```bash
git push origin feature/你的功能名称
```
5. 创建Pull Request

## 📝 代码规范

### C#编码规范
- 使用PascalCase命名类、方法、属性
- 使用camelCase命名局部变量、参数
- 使用_camelCase命名私有字段
- 添加XML文档注释
- 保持代码整洁，避免过长的方法

### 示例代码
```csharp
/// <summary>
/// 宠物状态接口
/// </summary>
public interface IPetState
{
    /// <summary>
    /// 获取当前状态的图片
    /// </summary>
    /// <returns>状态图片</returns>
    Image GetImage();
    
    /// <summary>
    /// 更新状态逻辑
    /// </summary>
    /// <param name="core">宠物核心对象</param>
    void Update(PetCore core);
}
```

### 项目结构
- UI层：用户界面相关代码
- BLL层：业务逻辑实现
- DAL层：数据访问逻辑
- Model层：数据模型定义
- Common层：公共工具类

## 🎯 开发指导

### 添加新状态
1. 在`Pet.BLL`项目中创建新的状态类
2. 实现`IPetState`接口
3. 添加相应的动画资源
4. 在适当的地方调用状态切换

### 扩展AI服务
1. 实现`IDialogService`接口
2. 创建适配器类处理API调用
3. 在配置中添加服务选择选项

### 添加新功能
1. 遵循现有的架构模式
2. 保持代码的可测试性
3. 添加适当的错误处理
4. 更新相关文档

## 🧪 测试

### 手动测试
- 测试所有基本功能
- 验证UI交互
- 检查错误处理
- 测试边界情况

### 建议的测试场景
- 拖拽功能
- AI对话
- 日程管理
- 状态切换
- 屏幕边缘处理

## 📋 Pull Request检查清单

在提交PR之前，请确保：

- [ ] 代码遵循项目的编码规范
- [ ] 添加了适当的注释
- [ ] 功能已经过测试
- [ ] 没有引入新的警告或错误
- [ ] 更新了相关文档
- [ ] PR描述清楚说明了更改内容

## 🏷️ 提交信息规范

使用以下格式的提交信息：
```
类型(范围): 简短描述

详细描述（可选）

关闭的Issue（可选）
```

类型包括：
- `feat`: 新功能
- `fix`: Bug修复
- `docs`: 文档更新
- `style`: 代码格式调整
- `refactor`: 代码重构
- `test`: 测试相关
- `chore`: 构建过程或辅助工具的变动

示例：
```
feat(ai): 添加ChatGPT适配器支持

- 实现ChatGPT API接口
- 添加配置选项
- 更新文档

Closes #123
```

## 🎉 认可贡献者

所有贡献者都会在README中得到认可。感谢每一位为项目做出贡献的开发者！

## 📞 联系方式

如果你有任何问题，可以通过以下方式联系：
- 创建Issue讨论
- 在PR中留言

再次感谢你的贡献！🙏
