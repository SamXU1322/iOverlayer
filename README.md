# iOverlayer

《A Dance of Fire and Ice》（冰与火之舞）的可视化多功能 Overlay 及编辑 Mod，基于 MelonLoader 框架开发。

## 🌟 功能简介 (Features)

## 🛠️ 环境准备 (Prerequisites)

本项目作为 Mod 运行，必须先为游戏安装 **MelonLoader** 框架。

**安装 MelonLoader：**
1. **自动安装（推荐）**：下载 [MelonLoader Installer](https://melonwiki.xyz/#/?id=automated-installation)，选择游戏的 `.exe` 运行程序并点击 Install 即可。
2. **手动安装**：直接下载 MelonLoader 对应的架构资源包，解压并在游戏根目录覆盖。
3. **自行编译（仅限开发者）**：如果有特定需求，也可以克隆 MelonLoader 官方仓库进行手动编译。

---

## 📦 玩家安装说明 (Installation)

1. 前往本仓库的 [Releases](https://github.com/SamXU1322/iOverlayer/releases) 页面，下载最新版的 `iOverlayer.dll` 文件及可能的附加资源（如 AssetBundle 文件夹）。
2. 将 `iOverlayer.dll` 放置到游戏根目录下的 `Mods` 文件夹内（首次安装 MelonLoader 并运行游戏后会自动生成该文件夹）。
3. 如果 Mod 附带了其他 UI 资源文件夹，请按照 Release 页面的说明放置在指定路径。
4. 启动游戏，并在游戏内按下 `F5` 以调出 iOverlayer 菜单。

---

## 💻 开发者指南 (For Developers)

若你希望参与开发或自行编译此 Mod：

1. 克隆代码仓库：
   ```bash
   git clone https://github.com/SamXU1322/iOverlayer.git
   ```
2. 使用 Visual Studio 2022 或 JetBrains Rider 打开 `iOverlayer.sln`。
3. **修正依赖路径**：目前 `.csproj` 中的引用路径是指向了硬盘内的特定位置（如 `..\..\A Dance of Fire and Ice\A Dance of Fire and Ice_Data\Managed\`）。你需要确保这些路径与你本地的游戏安装路径一致，或者重新绑定到正确的游戏 Managed 文件夹上。
4. **编译与部署**：编译（Build）项目时，项目内的 `PostBuildEvent` （生成后事件）会自动将生成的 DLL 复制到 `Mods` 文件夹。你需要将 `.csproj` 中 `xcopy` 这句脚本的目标路径修改为你自己电脑上冰与火之舞的真实路径。

## 📝 制作名单 (Credits)

- **作者**: Sam1nA & StarRiver
- **开源地址**: [https://github.com/SamXU1322/iOverlayer](https://github.com/SamXU1322/iOverlayer)
