# Notes to myself when updating 

In the [.nuspec format changes](https://docs.microsoft.com/en-us/nuget/reference/nuspec) here is what you do.

1. Edit the `NuspecBuilder\Example.nuspec` to support the new format
2. Copy the `Example.nuspec` xml and "delete all + paste special -> xml" into the `NuspecXmlFormat.cs` file.
3. Edit the .nuspec part of  the `SettingHandler\EveryPropertyFilledIn.xml` to match the updated `Example.nuspec`
4. Copy the `EveryPropertyFilledIn.xml` xml and "delete all + paste special -> xml" into the `allsettings.cs` file.
5. Make sure the various files in the `Test\TestData` directory are updated too
6. If the nuspec property has multiple parts, then add extra code in the `SettingHelpers.CopySettingMetadataIntoNuspec` method.
7. If you need to copy more files into the NuGet (the nuspec `<icon>` is one case), then edit the 


To try a debug version use the dotnet command

```
dotnet tool update JonPSmith.MultiProjPack --global
```