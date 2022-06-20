echo "Build dotnet project ..."
dotnet publish VTQT.Web.Master/VTQT.Web.Master.csproj -c Release -o  ./Deploy/
echo "Build docker image ..."
docker build -t 192.168.100.43:5000/vtqt.master.web:1.0.0 --rm=true -f "VTQT.Web.Master/Dockerfile" .
echo "Push docker image to registry ..."
docker push 192.168.100.43:5000/vtqt.master.web:1.0.0
