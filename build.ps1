# Ensure build error would be reported
$ErrorActionPreference = "Stop";

# Restore tools
dotnet tool restore

# Run cake build
dotnet tool run dotnet-cake build.cake