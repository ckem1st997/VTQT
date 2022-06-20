echo "Update submodule"
git submodule sync --recursive
git submodule update --init --recursive --remote
echo "Pull code"
git pull
echo "Build dotnet project ..."
dotnet publish Apis/VTQT.Api.Ticket/VTQT.Api.Ticket.csproj -c Release -o  ./Deploy/
echo "Build docker image ...."
docker build -t 192.168.100.43:5000/vtqt.ticket.api:1.0.0 --rm=true -f "Dockerfile" .
echo "Push docker image to registry ..."
docker push 192.168.100.43:5000/vtqt.ticket.api:1.0.0
