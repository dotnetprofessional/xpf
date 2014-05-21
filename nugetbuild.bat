cd xpf.http
..\.nuget\nuget.exe pack
xcopy *.nupkg "C:\Users\Garry\SkyDrive\Public\nuget" /F /Y
cd ..

cd xpf.FileSystem
..\.nuget\nuget.exe pack
xcopy *.nupkg "C:\Users\Garry\SkyDrive\Public\nuget" /F /Y
cd ..

cd xpf.IO
..\.nuget\nuget.exe pack
xcopy *.nupkg "C:\Users\Garry\SkyDrive\Public\nuget" /F /Y
cd ..

cd xpf.IO.Script
..\.nuget\nuget.exe pack
xcopy *.nupkg "C:\Users\Garry\SkyDrive\Public\nuget" /F /Y
cd ..

cd xpf.Script
..\.nuget\nuget.exe pack
xcopy *.nupkg "C:\Users\Garry\SkyDrive\Public\nuget" /F /Y
cd ..

cd xpf.Script.SQLServer
..\.nuget\nuget.exe pack
xcopy *.nupkg "C:\Users\Garry\SkyDrive\Public\nuget" /F /Y
cd ..