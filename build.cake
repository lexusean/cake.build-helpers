#addin nuget:?package=Cake.DocFx&version=0.4.1
#r "src/Cake.Helpers/bin/debug/net452/Cake.Helpers.dll"

var target = "Dummy";
var commandHelper = CommandHelper;

Task("Dummy")
	.Does(() =>
	{
		Information("Running Dummy");
		Information("Script Desc: {0}", commandHelper.ScriptDescription);
	});

if(target != "default")
	RunTarget(target);