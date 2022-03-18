@echo off
rem
echo [GIT] Adding everything...
git add .

echo.
echo.

echo [GIT] Please review the commit and input a note:
git status

set /p note="Enter a note: "
cls

echo [GIT] Committing to git as %date:/=% %time:/=%: %note%...
git commit -m "%date:/=% %time:/=%: %note%"

echo [GIT] Pushing to git...
git push origin master

echo [GIT] Done!

pause
cls
exit