DISPLAY_NAME=Magitech API
MAIN=Magitech API.dll
MEMORY=512
VERSION=recommended
START=dotnet Magitech\ API.dll
BUILD=dotnet publish -c Release -o out && cp -r out/* .
