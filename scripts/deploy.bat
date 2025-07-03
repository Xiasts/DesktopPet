@echo off
echo ========================================
echo 皮卡丘桌面宠物 - 部署脚本
echo ========================================

:: 设置变量
set BUILD_CONFIG=Release
set OUTPUT_DIR=.\Release
set PROJECT_DIR=.

:: 清理之前的构建
echo 正在清理之前的构建...
if exist "%OUTPUT_DIR%" rmdir /s /q "%OUTPUT_DIR%"
mkdir "%OUTPUT_DIR%"

:: 还原NuGet包
echo 正在还原NuGet包...
nuget restore

:: 构建项目
echo 正在构建项目...
msbuild /p:Configuration=%BUILD_CONFIG% /p:Platform="Any CPU" /p:OutputPath="%OUTPUT_DIR%"

if %ERRORLEVEL% neq 0 (
    echo 构建失败！
    pause
    exit /b 1
)

:: 复制必要文件
echo 正在复制文件...
copy "README.md" "%OUTPUT_DIR%\"
copy "LICENSE" "%OUTPUT_DIR%\"
copy "CHANGELOG.md" "%OUTPUT_DIR%\"

:: 复制资源文件
xcopy "Pet.UI\Resources" "%OUTPUT_DIR%\Resources\" /E /I /Y

:: 创建启动脚本
echo @echo off > "%OUTPUT_DIR%\启动皮卡丘.bat"
echo echo 正在启动皮卡丘桌面宠物... >> "%OUTPUT_DIR%\启动皮卡丘.bat"
echo Pet.UI.exe >> "%OUTPUT_DIR%\启动皮卡丘.bat"

echo ========================================
echo 部署完成！
echo 输出目录: %OUTPUT_DIR%
echo ========================================
pause
