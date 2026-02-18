FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

# Install CA tools
RUN apt-get update && apt-get install -y --no-install-recommends ca-certificates \
  && rm -rf /var/lib/apt/lists/*

# Trust corporate root CA (for TLS inspection/proxy)
COPY corp-root-ca.crt /usr/local/share/ca-certificates/corp-root-ca.crt
RUN update-ca-certificates


#COPY \*.csproj ./ copies all C# project files from your build context into the container's working directory. This is done early to leverage Docker layer caching.
#RUN dotnet restore HelloWorld.csproj restores NuGet dependencies for the HelloWorld.csproj project, preparing the project for build and publish steps.
# copy the project file ".csproj" only to leverage Docker's layer caching. This way, if you change your source code but not your project file, Docker can reuse the cached layers for restoring dependencies, which speeds up the build process.
COPY *.csproj ./
RUN dotnet restore HelloWorld.csproj


# copy the program files and publish the application. This step copies all the source code files into the container and then runs the dotnet publish command
COPY . ./
RUN dotnet publish HelloWorld.csproj -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./

# Set the ASP.NET Core URL environment variable to listen on port 8080. This tells the ASP.NET Core application to bind to port 8080 when it runs inside the container.
ENV ASPNETCORE_URLS=http://+:8080

# tells Docker that the container listens on port 8080 at runtime. This is important for networking and allows external clients to connect to the application running inside the container.
EXPOSE 8080 

ENTRYPOINT ["dotnet", "MyDotNetApp.dll"]
