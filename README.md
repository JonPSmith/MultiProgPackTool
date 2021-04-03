# `MultiProjPack` Tool

This dotnet tool is designed to turn a significant section of code from an application into NuGet package. This allows you to break up a large application into sub-application based on the Domain-Driven Design (DDD)'s [bounded context](https://martinfowler.com/bliki/BoundedContext.html) approach, and turn them into NuGet packages.

*NOTE: I cover this bounded context sub-application approach in [part 2 of my evolving modular monolith](#) series.*

This modular approach is useful for two reasons:

- It breaks up a complex application into bounded context sub-applications, which are managed via NuGet packages.
- It allows a development team to work on a bounded context sub-application separately for the main application.

The `MultiProjPack` tool is needed because some of the projects in a sub-application can have projects that aren't linked to a 'top' project, and some projects might not be linked to any other project because they are only linked via dependency injection.

## Overview of how to use the `MultiProjPack` tool

### 1. Install the `MultiProjPack` tool

```console
dotnet tool install -g JonPSmith.`MultiProjPack`
```

### 2. Organize the your code by namespaces

The `MultiProjPack` tool finds the projects to combine by the projects' name/namespace. It will find all the projects with a name staring with a given string and combine them into a single NuGet package.

 For instance, in [part 1 of my evolving modular monolith](#) I describe a naming convention where the namespace is `<NameOfApp>.<BoundedContextName>.<other-namespaces...>`, and below is an example of an application called `BookApp` and a bounded context of `Books`. These would

```c#
//Bounded context called "Books" in the application "BookApp"
BookApp.Books.Domain
BookApp.Books.Infrastructure.CachedValues
BookApp.Books.Infrastructure.Seeding
BookApp.Books.Persistence
BookApp.Books.ServiceLayer.Cached
BookApp.Books.ServiceLayer.Common
//etc...
//New bounded context called "Orders"
BookApp.Orders.Domain       
//etc...
```

By telling the `MultiProjPack` tool to look for namespaces starting with "`BookApp.Books.`" it would create a NuGet packages that contains all the projects with names starting with "`BookApp.Books.`".

### 3. Create a file containing the NuGet package info

To create a NuGet package you need to define some NuGet information, such as the name (id) of the created NuGet package, its version, its authors and so on.

The `MultiProjPack` tool looks for a file called `MultiProjPack.xml`, which contains NuGet info and settings for the `MultiProjPack` tool. Here is a link to a [example MultiProjPack.xml file](https://github.com/JonPSmith/MultiProgPackTool/blob/main/MultiProjPackTool/SettingHandling/TypicalMultiProjPack.xml), but I give more information on the `MultiProjPack.xml` file later (LINK!!!)

*NOTE: The `MultiProjPack` tool will create an `MultiProjPack.xml` for you with the typical parts that you want to fill in. See !!!LINK on how to do that.*

The `MultiProjPack.xml` file should be placed in the folder containing the projects in your solution, for instance `C:\Users\YourUser\source\repos\BookApp.Books>`

### 4. Run the `MultiProjPack` tool

You run the `MultiProjPack` tool from a command line in the folder containing the projects in your solution.

*NOTE: If you want to create a new version of the NuGet package you must update the NuGet `version` and most likely the `releaseNotes` in the `MultiProjPack.xml` before you call the `MultiProjPack` tool.*

```console
MultiProjPack <what to do>
```

Where `<what to do>` is either:

- `D`(ebug): This creates a NuGet package using the `Debug` version of the code.
- `R`(elease): This creates a NuGet package using the `Release` version of the code.
- `U`(pdate): This builds a NuGet package using the `Debug` code, but also updates the `.dll`s in the NuGet cache - See section [???](???) for more of this.

### 5. Use the NuGet package

At this point you have a new or updated NuGet package. You can use the package locally to check it works with the main application, or push it to a private NuGet server for others to use.

---

Now we look at the settings and options that MultiProjPack tool has.

1. The `MultiProjPack.xml` file
2. The MultiProjPack tool parameters and options.

## 1. The `MultiProjPack.xml` file content

The `MultiProjPack.xml` file has two sections

```xml
<allsettings>
  <metadata>
    <!-- this contains the NuGet settings -->
  </metadata>
  <toolSettings>
    <!-- this contains the tool settings -->
  </toolSettings>
</allsettings>
```

### 1. The `<metadata>` section

This holds the the information that NuGet needs when building a package. This follows the [nuspec metadata format](https://docs.microsoft.com/en-us/nuget/reference/nuspec) because the `MultiProjPack` tool create a `.nuspec` file.

*NOTE: The `<metadata>` section uses same commands as you would add to a .csproj file to create a NuGet package, but the xml names are camelCase, while the .csproj version is PascalCase.

The [this file](https://github.com/JonPSmith/MultiProgPackTool/blob/main/MultiProjPackTool/SettingHandling/EveryPropertyFilledIn.xml) contains all the NeGet information you can define.

### 2. The `<toolSettings>` section

This contains options that are particular to the `MultiProjPack` tool and control the the tool works. Here is each list of the the options, with the most useful first.

#### `<NamespacePrefix>` - defines the namespace to look for (optional)

The `MultiProjPack` tool will look for projects who's name starts with $"{NamespacePrefix}.". An example would be `NamespacePrefix>BookApp.Books</NamespacePrefix>`.

*NOTE: If you don't fill this in, or leave the setting out it will use the NuGet `<id>` from the `<metadata>` section.*

#### `<ExcludeProjects>` - excluding certain projects

If you have projects starting with the `<NamespacePrefix>`, but you don't want them included in the NuGet package, then you can define them in this setting as a comma delimited list of the ending of their name.

Example setting

```xml
<allsettings>
        <!-- metadata left out -->
  <toolSettings>
    <ExcludeProjects>Test,AnotherProject</ExcludeProjects>
  </toolSettings>
</allsettings>
```

Assuming the `<id>` or `<NamespacePrefix>` is set to "`BookApp.Books`" then the `BookApp.Books.Test` and `BookApp.Books.AnotherProject` projects will be excluded from the NuGet package.

#### `<CopyNuGetTo>` - used to auto-copy new NuGet to local NuGet source

The NuGet package is created in the folder the the `MultiProjPack` tool was run in. That's OK, but its useful to have your new NuGet available on your dev computer for local testing, and (via Visual Studio's NuGet Package Manager ) you can have a local NuGet package source so that you can immediately add it to your test application.

You need to set up a local NuGet package source (which I explain in [part 4 of my evolving modular monolith](#) series on testing or this [useful article](https://spin.atomicobject.com/2021/01/05/local-nuget-package/)) and then fill the `<CopyNuGetTo>` setting with the local NuGet package source folder. When you run the `MultiProjPack` tool it will automatically copy your new NeGet package to your local NuGet package source.

Example settings

```xml
<allsettings>
        <!-- metadata left out -->
  <toolSettings>
    <CopyNuGetTo>{USERPROFILE}\LocalNuGet</CopyNuGetTo>
  </toolSettings>
</allsettings>
```

NOTE: The `{USERPROFILE}` is optional, but makes this setting work for any user. `USERPROFILE` is an environment value that contains your user directory - for me that's `C:\User\JonPSmith`.

#### `<AddSymbols>` - will add a symbols package

When you are unit testing its useful to have the full information for unit testing. Personally I prefer embedding the code into the `.dll` file when in Debug mode (I explain why in [part 4 of my evolving modular monolith](#) series on testing).

But if you want NuGet to create a symbol package then you need to a) add `<IncludeSymbols>true</IncludeSymbols>` in all of your projects and b) set the correct setting for the `<AddSymbols>` setting. The possible values are: "None", "Debug", "Release" or "Always".

If you don't provide a value it defaults to "None".

#### `<NuGetCachePath>` - override default cache path settings

?????? !!!!!!!!!!!!!!!!!!!!!!!

## 2. The `MultiProjPack` tool options

- `-h`, `--help`: 
- `--CreateSettings`: 
- `--source=..\MultiProjPackDifferentName.xml`: 
- 
