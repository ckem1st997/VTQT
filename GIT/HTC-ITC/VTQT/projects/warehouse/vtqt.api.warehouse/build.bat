echo "Update submodule"
git submodule sync --recursive
git submodule update --init --recursive --remote
echo "Pull code"
git pull
echo "Build dotnet project ..."
dotnet publish Apis/VTQT.Api.Warehouse/VTQT.Api.Warehouse.csproj -c Release -o  ./Deploy/
echo "Build docker image ...."
docker build -t 192.168.100.43:5000/vtqt.warehouse.api:1.0.2 --rm=true -f "Dockerfile" .
echo "Push docker image to registry ..."
docker push 192.168.100.43:5000/vtqt.warehouse.api:1.0.2