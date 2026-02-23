#### first, make sure the project has the needed package
```shell
<PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="10.0.3">
  <PrivateAssets>all</PrivateAssets>
</PackageReference>

<PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="10.0.0" />
```

#### Then make sure dotnet-ef is installed as a tool:
```shell
dotnet tool list
```
#### if not, install it (locally):
```shell
dotnet new tool-manifest
dotnet tool install dotnet-ef --version 10.0.3
```

#### Npgsql connection string
```shell
Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=your_password
```


-------------------------------------
#### for global installation of dotnet-ef, use:
```shell
dotnet tool install --global dotnet-ef --version 10.0.3
```
#### check
```shell
echo $PATH
ls ~/.dotnet/tools
```

#### If you see dotnet-ef in that folder but dotnet ef still fails, add this to your shell profile:

##### zsh (most common on macOS)

```shell
echo 'export PATH="$PATH:$HOME/.dotnet/tools"' >> ~/.zshrc
source ~/.zshrc
```

##### bash

```shell
echo 'export PATH="$PATH:$HOME/.dotnet/tools"' >> ~/.bash_profile
source ~/.bash_profile
```

##### then verify installation again
```shell
dotnet tool list --global
dotnet ef --version
```


--------------------------------------manually run
##### note - this will need to be run inside the project folder where the .csproj file is located, and the connection string should be updated to match your database configuration
```shell
dotnet ef dbcontext scaffold \
"Name=ConnectionStrings:Default" \
Npgsql.EntityFrameworkCore.PostgreSQL \
--output-dir InfrastructureModule/Persistence/Entities \
--context-dir InfrastructureModule/Persistence \
--context AppDbContext \
--use-database-names \
--no-onconfiguring
```