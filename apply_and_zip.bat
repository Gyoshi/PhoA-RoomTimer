echo off

rem Create temporary folder
set temp_folder=%TEMP%\build_temp
md %temp_folder%
md %temp_folder%\ModifiedLevels

rem Copy files to temporary folder
copy bin\Debug\net3.5\RoomTimer.dll %temp_folder%
copy Info.json %temp_folder%
copy README.md %temp_folder%
copy assets\StreamingAssets\ModifiedLevels %temp_folder%\ModifiedLevels\

rem Convert to xml 
ren %temp_folder%\ModifiedLevels\*.tmx *.xml

rem Convert to old file format if newer version of Tiled is used
setlocal EnableDelayedExpansion
for %%f in ("%temp_folder%\ModifiedLevels\*.xml") do (
	powershell -Command "(gc %%f) -replace 'class', 'type' | Out-File -encoding ASCII %%f"
)

copy "bin\Debug\net3.5\RoomTimer.dll" "C:\Program Files (x86)\Steam\steamapps\common\Phoenotopia Awakening\Mods\RoomTimer\"
copy Info.json "C:\Program Files (x86)\Steam\steamapps\common\Phoenotopia Awakening\Mods\RoomTimer\"
copy %temp_folder%\ModifiedLevels "C:\Program Files (x86)\Steam\steamapps\common\Phoenotopia Awakening\Mods\RoomTimer\ModifiedLevels\"

rem Zip folder
powershell Compress-Archive -Update -Path %temp_folder%\* -DestinationPath PhoA-RoomTimer.zip

rem Delete temporary folder
rmdir /s /q %temp_folder%