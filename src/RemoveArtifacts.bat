FOR /D /R . %%X IN (bin) DO RD /S /Q "%%X"
FOR /D /R . %%X IN (obj) DO RD /S /Q "%%X"
FOR /D /R . %%X IN (packages) DO RD /S /Q "%%X"
FOR /D /R . %%X IN (packages) DO RD /S /Q "%%X"
DEL /ah /s *.vs
echo RMDIR /S/Q %USERPROFILE%\.nuget\packages
