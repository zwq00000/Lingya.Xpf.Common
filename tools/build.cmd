Set OutPath=D:\Projects\packages\WPF

set Configuration=Release
set YYYYmmdd=%date:~0,4%%date:~5,2%%date:~8,2%
Set suffix=build%YYYYmmdd%

Set version-suffix=-Suffix %suffix%

cd ..\src\Lingya.Xpf.Common
call Nuget pack -Build -Properties Configuration=Release -OutputDirectory %OutPath% %version-suffix%
cd ../../tools

