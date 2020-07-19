
test:
	dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover

watch:
	dotnet watch test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover

report:
	dotnet reportgenerator -reports:coverage.opencover.xml -targetdir:coveragereport

clean:
	rm -f coverage.opencover.xml
	rm -rf coveragereport

