@echo off
chcp 65001 >nul
echo 开始重命名LSM_Halo文件为LSM_Headgear文件...
echo.

REM East方向重命名
echo 重命名East方向文件...
ren "LSM_Halo_east-1.png" "LSM_Headgear_east.png"
ren "LSM_Halo_east-2.png" "LSM_Headgear1_east.png"
ren "LSM_Halo_east-3.png" "LSM_Headgear2_east.png"
ren "LSM_Halo_east-4.png" "LSM_Headgear3_east.png"
ren "LSM_Halo_east-5.png" "LSM_Headgear4_east.png"
ren "LSM_Halo_east-6.png" "LSM_Headgear5_east.png"
ren "LSM_Halo_east-7.png" "LSM_Headgear6_east.png"
ren "LSM_Halo_east-8.png" "LSM_Headgear7_east.png"
echo East方向重命名完成！
echo.

REM North方向重命名
echo 重命名North方向文件...
ren "LSM_Halo_north-1.png" "LSM_Headgear_north.png"
ren "LSM_Halo_north-2.png" "LSM_Headgear1_north.png"
ren "LSM_Halo_north-3.png" "LSM_Headgear2_north.png"
ren "LSM_Halo_north-4.png" "LSM_Headgear3_north.png"
ren "LSM_Halo_north-5.png" "LSM_Headgear4_north.png"
ren "LSM_Halo_north-6.png" "LSM_Headgear5_north.png"
ren "LSM_Halo_north-7.png" "LSM_Headgear6_north.png"
ren "LSM_Halo_north-8.png" "LSM_Headgear7_north.png"
echo North方向重命名完成！
echo.

REM South方向重命名
echo 重命名South方向文件...
ren "LSM_Halo_south-1.png" "LSM_Headgear_south.png"
ren "LSM_Halo_south-2.png" "LSM_Headgear1_south.png"
ren "LSM_Halo_south-3.png" "LSM_Headgear2_south.png"
ren "LSM_Halo_south-4.png" "LSM_Headgear3_south.png"
ren "LSM_Halo_south-5.png" "LSM_Headgear4_south.png"
ren "LSM_Halo_south-6.png" "LSM_Headgear5_south.png"
ren "LSM_Halo_south-7.png" "LSM_Headgear6_south.png"
ren "LSM_Halo_south-8.png" "LSM_Headgear7_south.png"
echo South方向重命名完成！
echo.

echo 所有LSM_Halo文件重命名完成！
echo 重命名后的文件列表：
dir LSM_Headgear*.png
echo.
pause
