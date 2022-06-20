@ECHO OFF

ECHO git submodule update --init --recursive --remote
git submodule sync --recursive
git submodule update --init --recursive --remote

ECHO git pull
git pull

ECHO Done.
ECHO.

PAUSE