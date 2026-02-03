# iOverLayer

一个基于 Unity 的 UI 覆盖层框架，用于在游戏中创建和管理自定义文本显示系统。

## 功能特性

- 🎨 **Canvas 管理**: 自动创建和管理系统画布
- 📝 **文本系统**: 支持动态创建、删除和管理文本对象
- 🔤 **字体支持**: 可加载自定义字体资源
- 📦 **资源加载**: 基于 AssetBundle 的资源管理系统
- 📊 **日志系统**: 完整的日志记录和调试功能

## 项目结构

```
iOverLayer/
├── Properties/
│   └── AssemblyInfo.cs          # 程序集信息配置
├── Text/
│   ├── Text.cs                  # 文本组件核心类
│   ├── TextFont.cs              # 字体管理器
│   └── TextManager.cs           # 文本管理器
├── AssetLoader.cs               # 资源加载器
├── Canvas.cs                    # 画布管理器
├── LogSystem.cs                 # 日志系统
└── Main.cs                      # 主入口点
```

## 核心组件

### 🖼️ Canvas 系统
```csharp
internal static class Canvas
{
    public static UnityEngine.Canvas Instance => _canvas;
    public static GameObject Root => _root;
    
    public static void Init(); // 初始化画布系统
}
```

### 📝 Text 管理系统

#### Text 组件
```csharp
public class Text: MonoBehaviour
{
    public int ID => _id;
    public void setId(int id);
}
```

#### TextManager 管理器
```csharp
public static class TextManager
{
    public static void Create();    // 创建新文本
    public static void Delete(int index); // 删除指定文本
}
```

### 🔤 字体系统
```csharp
public static class TextFont
{
    public static void LoadFontAsset(string fontName); // 加载字体资源
}
```

### 📦 资源加载
```csharp
public static class AssetLoader
{
    public static void Init(UnityModManager.ModEntry modEntry); // 初始化资源加载器
    public static GameObject LoadPrefabAssetBundle(string bundleName, string assetName);
    public static TMP_FontAsset LoadFontAssetBundle(string bundleName, string fontName);
}
```

### 📊 日志系统
```csharp
public static class LogSystem
{
    public static void Init(UnityModManager.ModEntry modEntry);
    public static void Info(string message);
    public static void Warning(string message);
    public static void Error(string message);
}
```

## 使用方法

### 基本初始化
```csharp
public static void Load(UnityModManager.ModEntry modEntry)
{
    LogSystem.Init(modEntry);
    AssetLoader.Init(modEntry);
    Canvas.Init();
    TextManager.Create();
}
```

### 创建文本
```csharp
TextManager.Create(); // 创建新的文本对象
```

### 删除文本
```csharp
TextManager.Delete(textId); // 删除指定ID的文本
```

### 加载自定义字体
```csharp
TextFont.LoadFontAsset("CustomFont"); // 加载名为CustomFont的字体
```

## 技术特点

### 🏗️ 架构设计
- **单例模式**: 核心管理器采用静态单例设计
- **模块化**: 各功能模块独立，职责分明
- **依赖注入**: 通过 UnityModManager 进行模块初始化

### ⚡ 性能优化
- **对象池**: 文本对象支持复用机制
- **资源管理**: 基于 AssetBundle 的高效资源加载
- **内存控制**: 自动化的对象生命周期管理

### 🔧 扩展性
- **插件化**: 基于 UnityModManager 的模块化设计
- **配置化**: 支持外部资源配置
- **热更新**: AssetBundle 支持运行时资源更新

## 依赖项

- Unity 2019.4+
- Unity Mod Manager
- TextMeshPro
- Harmony (代码补丁库)

## 安装说明

1. 确保已安装 Unity Mod Manager
2. 将编译后的 DLL 文件放入 Mods/iOverLayer/ 目录
3. 在游戏启动时自动加载

## 开发指南

### 添加新功能
1. 在相应命名空间下创建新类
2. 实现必要的接口和方法
3. 在 Main.cs 中添加初始化调用

### 调试建议
- 使用 LogSystem 记录关键操作
- 检查 UnityModManager 日志输出
- 查看生成的 iOverLayer.log 文件

## 版本历史

### v1.0.0 (2026)
- 初始版本发布
- 基础 Canvas 和文本管理系统
- AssetBundle 资源加载支持
- 完整的日志系统

## 许可证

MIT License

## 贡献

欢迎提交 Issue 和 Pull Request 来改进这个项目！