# 🛠️ 开发者指南

## 开发环境设置

### 必需软件
- **Visual Studio 2019+** 或 **Visual Studio Code**
- **.NET Framework 4.7.2+**
- **Git** 版本控制
- **NuGet** 包管理器

### 推荐工具
- **ReSharper** - 代码分析和重构
- **GitKraken** - Git可视化工具
- **Postman** - API测试（AI服务调试）

## 🚀 快速开始

### 1. 克隆项目
```bash
git clone https://github.com/Xiasts/DesktopPet.git
cd DesktopPet
```

### 2. 还原依赖
```bash
nuget restore
```

### 3. 打开项目
- 使用Visual Studio打开 `DesktopPet.sln`
- 或使用VS Code打开项目文件夹

### 4. 配置API密钥
编辑 `Pet.BLL/DeepSeekAdapter.cs`，替换API密钥：
```csharp
_dialogService = new DeepSeekAdapter("your-api-key-here");
```

### 5. 构建和运行
- 按 `F5` 启动调试
- 或使用 `Ctrl+F5` 运行而不调试

## 📁 项目结构详解

```
DesktopPet/
├── 📁 Pet.UI/              # 用户界面层
│   ├── 🎨 Forms/           # 窗体文件
│   ├── 🖼️ Resources/       # 图片资源
│   └── 📄 *.cs             # UI逻辑代码
├── 📁 Pet.BLL/             # 业务逻辑层
│   ├── 🎭 States/          # 状态模式实现
│   ├── 🤖 AI/              # AI服务适配器
│   └── 📊 Managers/        # 业务管理器
├── 📁 Pet.DAL/             # 数据访问层
├── 📁 Pet.Model/           # 数据模型
├── 📁 Pet.Common/          # 公共工具
├── 📁 docs/                # 项目文档
├── 📁 scripts/             # 构建脚本
└── 📁 .github/             # GitHub配置
```

## 🔧 开发工作流

### 分支策略
- `main` - 主分支，稳定版本
- `develop` - 开发分支，最新功能
- `feature/*` - 功能分支
- `hotfix/*` - 热修复分支

### 开发流程
1. 从 `develop` 创建功能分支
2. 开发新功能
3. 提交代码并推送
4. 创建Pull Request到 `develop`
5. 代码审查和合并
6. 定期从 `develop` 合并到 `main`

## 🎨 代码规范

### 命名约定
```csharp
// 类名 - PascalCase
public class PetCore { }

// 方法名 - PascalCase
public void UpdateState() { }

// 属性 - PascalCase
public string ServiceName { get; set; }

// 私有字段 - _camelCase
private int _animationTimer;

// 常量 - UPPER_CASE
private const int ANIMATION_SPEED = 10;

// 局部变量 - camelCase
var currentState = GetState();
```

### 注释规范
```csharp
/// <summary>
/// 宠物状态接口，定义所有状态的基本行为
/// </summary>
public interface IPetState
{
    /// <summary>
    /// 获取当前状态的显示图片
    /// </summary>
    /// <returns>状态对应的图片对象</returns>
    Image GetImage();
    
    /// <summary>
    /// 更新状态逻辑
    /// </summary>
    /// <param name="core">宠物核心控制器</param>
    void Update(PetCore core);
}
```

## 🧪 测试指南

### 单元测试
```csharp
[TestClass]
public class PetCoreTests
{
    [TestMethod]
    public void SetState_ShouldChangeCurrentState()
    {
        // Arrange
        var core = new PetCore();
        var newState = new IdleState();
        
        // Act
        core.SetState(newState);
        
        // Assert
        Assert.IsInstanceOfType(core.CurrentState, typeof(IdleState));
    }
}
```

### 集成测试
- AI服务连接测试
- 数据持久化测试
- 事件通知测试

### 手动测试清单
- [ ] 拖拽功能正常
- [ ] 状态切换流畅
- [ ] AI对话响应
- [ ] 日程提醒准确
- [ ] 界面响应正常
- [ ] 错误处理得当

## 🐛 调试技巧

### 常用调试方法
1. **断点调试** - 在关键位置设置断点
2. **日志输出** - 使用Debug.WriteLine()
3. **异常捕获** - 合理使用try-catch
4. **性能分析** - 使用Visual Studio性能工具

### 常见问题
1. **动画卡顿** - 检查Timer间隔和动画速度设置
2. **内存泄漏** - 确保图片资源正确释放
3. **AI调用失败** - 检查网络连接和API密钥
4. **状态切换异常** - 验证状态转换逻辑

## 📦 构建和部署

### 本地构建
```bash
# 清理项目
msbuild /t:Clean

# 构建Release版本
msbuild /p:Configuration=Release

# 运行部署脚本
scripts\deploy.bat
```

### CI/CD流程
项目使用GitHub Actions自动构建：
- 推送到main/develop分支触发构建
- 自动运行测试
- 生成构建产物
- 创建Release（标签推送时）

## 🔌 扩展开发

### 添加新状态
1. 创建状态类实现`IPetState`
2. 添加动画资源
3. 实现状态逻辑
4. 在适当位置调用状态切换

```csharp
public class NewState : IPetState
{
    public Image GetImage() { /* 实现 */ }
    public void Update(PetCore core) { /* 实现 */ }
}
```

### 集成新AI服务
1. 实现`IDialogService`接口
2. 创建适配器类
3. 处理API调用逻辑
4. 添加配置选项

```csharp
public class NewAIAdapter : IDialogService
{
    public string ServiceName => "New AI Service";
    public bool IsAvailable => true;
    
    public async Task<string> GetResponseAsync(string userInput)
    {
        // 实现API调用逻辑
    }
}
```

### 扩展数据存储
1. 实现`IRepository`接口
2. 创建新的存储实现
3. 配置依赖注入
4. 测试数据操作

## 📊 性能优化

### 动画优化
- 使用合适的Timer间隔
- 避免不必要的重绘
- 优化图片加载和缓存

### 内存管理
- 及时释放资源
- 避免内存泄漏
- 合理使用缓存

### 网络优化
- 异步API调用
- 合理的超时设置
- 错误重试机制

## 🔒 安全考虑

### API密钥管理
- 不在代码中硬编码
- 使用配置文件
- 考虑加密存储

### 输入验证
- 验证用户输入
- 防止注入攻击
- 安全的错误处理

## 📚 学习资源

### 设计模式
- [状态模式详解](https://refactoring.guru/design-patterns/state)
- [单例模式最佳实践](https://csharpindepth.com/articles/singleton)
- [观察者模式应用](https://docs.microsoft.com/en-us/dotnet/standard/events/)

### C# WinForms
- [官方文档](https://docs.microsoft.com/en-us/dotnet/desktop/winforms/)
- [最佳实践指南](https://docs.microsoft.com/en-us/dotnet/desktop/winforms/advanced/)

### AI集成
- [DeepSeek API文档](https://api-docs.deepseek.com/)
- [异步编程模式](https://docs.microsoft.com/en-us/dotnet/csharp/async)

## 🤝 贡献指南

详见 [CONTRIBUTING.md](../CONTRIBUTING.md)

## 📞 获取帮助

- 📋 [创建Issue](https://github.com/Xiasts/DesktopPet/issues)
- 💬 [参与讨论](https://github.com/Xiasts/DesktopPet/discussions)
- 📧 联系维护者
