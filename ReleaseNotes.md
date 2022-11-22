# Release Notes

## 2.1.0

- Add code to gives a warning of different versions of the same NuGet package.

## 2.0.0

- Updated to handle projects that have multiple frameworks, e.g. <TargetFrameworks>net6.0;net7.0</TargetFrameworks>
- Updated to NET 6

## 1.1.1 (bug fix)

- Updated to the new `<icon>images\someicon.png</icon>` format
- Updated to the new `<license type="expression">MIT</license>` format
- NOTE: does not support `<license type="file">LICENSE.txt</license>` format

## 1.1.0

- Updated to the new `<icon>someicon.png</icon>` format
- Updated to the new `<license type="expression">MIT</license>` format
- NOTE: does not support `<license type="file">LICENSE.txt</license>` format

## 1.0.1

- Added symbol files to NuGet package if there (doesn't need AddSymbols to be on)
- Added copy of symbol files to NuGet cache when using the U(pdate) command.

## 1.0.0

- First release