echo "Build dotnet project ..."
dotnet publish Apis/VTQT.Api.Master/VTQT.Api.Master.csproj -c Release -o  ./Deploy/
echo "Build docker image ...."
docker build -t 192.168.100.43:5000/vtqt.master.api:1.0.2 --rm=true -f "Apis/VTQT.Api.Master/Dockerfile1" .
echo "Push docker image to registry ..."
docker push 192.168.100.43:5000/vtqt.master.api:1.0.2