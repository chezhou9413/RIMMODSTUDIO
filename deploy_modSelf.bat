@echo off
setlocal enabledelayedexpansion

REM ==========================================
REM Mod Deployment Script - Simplified
REM ==========================================

REM ========== Configuration ==========
REM Mod folder name (current project folder name)
set MOD_FOLDER_NAME=RimBiochemistry

REM Target path (RimWorld Mods directory)
set TARGET_PATH=E:\huanshijie\1.6\RimWorld\Mods

REM ========== Path Setup ==========
REM Current script directory (project root)
set CURRENT_DIR=%~dp0

REM Source mod path (the bluearchive-newcentury folder)
set SOURCE_MOD_PATH=%CURRENT_DIR%%MOD_FOLDER_NAME%

REM Target mod path (destination in Mods folder)
set TARGET_MOD_PATH=%TARGET_PATH%\%MOD_FOLDER_NAME%

REM ========== Display Configuration ==========
echo ==========================================
echo Mod Deployment Script - Simplified
echo ==========================================
echo Mod Name: %MOD_FOLDER_NAME%
echo Source Path: %SOURCE_MOD_PATH%
echo Target Path: %TARGET_MOD_PATH%
echo ==========================================
echo.

REM ========== Check if source path exists ==========
if not exist "%SOURCE_MOD_PATH%" (
    echo [ERROR] Source mod folder does not exist: %SOURCE_MOD_PATH%
    echo Please ensure the bluearchive-newcentury folder exists in the current directory.
    pause
    exit /b 1
)

REM ========== Check if target root directory exists ==========
if not exist "%TARGET_PATH%" (
    echo [ERROR] Target path does not exist: %TARGET_PATH%
    echo Please ensure RimWorld is installed and path is correct.
    pause
    exit /b 1
)

REM ========== Direct Delete and Copy ==========
echo [INFO] Starting mod deployment...

REM Delete target folder if exists
if exist "%TARGET_MOD_PATH%" (
    echo [INFO] Removing existing mod: %TARGET_MOD_PATH%
    rd /s /q "%TARGET_MOD_PATH%" >nul 2>&1
    
    REM Wait for deletion to complete
    timeout /t 1 /nobreak >nul 2>&1
    
    REM Force delete if still exists
    if exist "%TARGET_MOD_PATH%" (
        echo [INFO] Force deleting existing mod...
        rmdir /s /q "%TARGET_MOD_PATH%" >nul 2>&1
        del /f /s /q "%TARGET_MOD_PATH%" >nul 2>&1
    )
)

REM Copy the entire bluearchive-newcentury folder to Mods directory
echo [INFO] Copying entire mod folder...
echo Copying: %SOURCE_MOD_PATH% to %TARGET_PATH%
robocopy "%SOURCE_MOD_PATH%" "%TARGET_MOD_PATH%" /e /is /it /r:1 /w:1 >nul 2>&1

REM ========== Verify Results ==========
echo.
echo [INFO] Verifying copy results...

if exist "%TARGET_MOD_PATH%" (
    echo [SUCCESS] Mod folder copied successfully
) else (
    echo [ERROR] Failed to copy mod folder
    pause
    exit /b 1
)

if exist "%TARGET_MOD_PATH%\About\About.xml" (
    echo [SUCCESS] About.xml file exists
) else (
    echo [WARNING] About.xml file not found, mod may not load properly
)

if exist "%TARGET_MOD_PATH%\Defs" (
    echo [SUCCESS] Defs folder exists
) else (
    echo [WARNING] Defs folder not found
)

if exist "%TARGET_MOD_PATH%\Assemblies" (
    echo [SUCCESS] Assemblies folder exists
) else (
    echo [INFO] Assemblies folder not found (normal if mod has no code)
)

REM ========== Complete ==========
echo.
echo ==========================================
echo Mod Deployment Complete!
echo ==========================================
echo Mod successfully deployed to: %TARGET_MOD_PATH%
echo.
echo Notes:
echo 1. If RimWorld is running, restart the game to load new version
echo 2. Enable "%MOD_FOLDER_NAME%" mod in the mod list
echo 3. The entire bluearchive-newcentury folder has been copied
echo.
echo Press any key to exit...
pause >nul 