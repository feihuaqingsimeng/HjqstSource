@echo build pblua
cd ../_protos  
rem !convert to luapb
mkdir lua
for %%i in (*.proto) do (    
echo %%i  
"..\genpblua\protoc.exe" --plugin=protoc-gen-lua="..\genpblua\plugin\pp.bat" --lua_out=.\lua %%i  
  
)  
