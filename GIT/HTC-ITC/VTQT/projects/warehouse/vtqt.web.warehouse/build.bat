echo "Build dotnet project ..."
dotnet publish VTQT.Web.Warehouse/VTQT.Web.Warehouse.csproj -c Release -o  ./Deploy/
echo "Build docker image ...."
docker build -t 192.168.100.43:5000/vtqt.warehouse.web:1.0.2 --rm=true -f "Dockerfile" .
echo "Push docker image to registry ..."
docker push 192.168.100.43:5000/vtqt.warehouse.web:1.0.2