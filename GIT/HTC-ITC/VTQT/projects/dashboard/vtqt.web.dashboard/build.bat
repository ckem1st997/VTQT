echo "Build dotnet project ..."
dotnet publish VTQT.Web.Dashboard/VTQT.Web.Dashboard.csproj -c Release -o  ./Deploy/
echo "Build docker image ...."
docker build -t 192.168.100.43:5000/vtqt.dashboard.web:1.0.0 --rm=true -f "Dockerfile" .
echo "Push docker image to registry ..."
docker push 192.168.100.43:5000/vtqt.dashboard.web:1.0.0