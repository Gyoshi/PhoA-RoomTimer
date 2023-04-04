echo off

rem Create temporary folder
set temp_folder=%TEMP%\build_temp
md %temp_folder%

rem Copy files to temporary folder
copy bin\Debug\net3.5\RoomTimer.dll %temp_folder%
copy Info.json %temp_folder%
copy README.md %temp_folder%

copy "bin\Debug\net3.5\RoomTimer.dll" "C:\Program Files (x86)\Steam\steamapps\common\Phoenotopia Awakening\Mods\RoomTimer\"
copy Info.json "C:\Program Files (x86)\Steam\steamapps\common\Phoenotopia Awakening\Mods\RoomTimer\"

rem Zip folder
powershell Compress-Archive -Update -Path %temp_folder%\* -DestinationPath PhoA-RoomTimer.zip

rem Delete temporary folder
rmdir /s /q %temp_folder%