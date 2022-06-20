echo "Build dotnet project ..."
dotnet publish VTQT.Web.Ticket/VTQT.Web.Ticket.csproj -c Release -o  ./Deploy/
echo "Build docker image ...."
docker build -t 192.168.100.43:5000/vtqt.ticket.web:1.0.0 --rm=true -f "Dockerfile" .
echo "Push docker image to registry ..."
docker push 192.168.100.43:5000/vtqt.ticket.web:1.0.0