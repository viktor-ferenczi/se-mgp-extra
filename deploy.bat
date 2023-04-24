del /f /s /q "%AppData%\SpaceEngineers\Mods\Multigrid Projector Extra" 2>&1 >NUL
rd /s /q "%AppData%\SpaceEngineers\Mods\Multigrid Projector Extra" 2>&1 >NUL

mkdir "%AppData%\SpaceEngineers\Mods" 2>&1 >NUL
mkdir "%AppData%\SpaceEngineers\Mods\Multigrid Projector Extra" 2>&1 >NUL

xcopy /s /e /y Mod\ "%AppData%\SpaceEngineers\Mods\Multigrid Projector Extra\"
