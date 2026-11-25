# Repository Guidelines

## Project Structure & Module Organization
This solution is centered on `Arcemi.SaveGameEditor.sln` and split into three main projects:
- `Arcemi.Models/`: shared domain models, game accessors, mappings, and serialization helpers.
- `Arcemi.SaveGameEditor/`: ASP.NET Core + Blazor UI host with ElectronNET.Core packaging (`Pages/`, `Components/`, `wwwroot/`, `_Defs/`).
- `Arcemi.Tests/`: xUnit test project with fixture JSON under `Arcemi.Tests/Files/`.

Build and release automation lives in `.github/workflows/` (Windows, Linux, macOS Electron builds).

## Build, Test, and Development Commands
- `dotnet restore Arcemi.SaveGameEditor.sln` - restore NuGet packages for all projects.
- `dotnet build Arcemi.SaveGameEditor.sln -c Release` - compile all projects.
- `dotnet test Arcemi.Tests/Arcemi.Tests.csproj` - run the unit test suite.
- `cd Arcemi.SaveGameEditor && dotnet run` - run the desktop/web host locally.
- `cd Arcemi.SaveGameEditor && dotnet publish -c Release /p:ElectronBuildTarget=win /p:ElectronPlatform=win32 /p:ElectronArch=x64` - package Windows desktop build.
- `cd Arcemi.SaveGameEditor && dotnet publish -c Release /p:ElectronBuildTarget=linux /p:ElectronPlatform=linux /p:ElectronArch=x64` (or `osx` + `darwin`) - package Linux/macOS builds.

## Coding Style & Naming Conventions
Follow `.editorconfig`:
- 4-space indentation, CRLF line endings, and explicit braces.
- C# naming: `PascalCase` for types/members, interfaces prefixed with `I`, private fields as `_camelCase`.
- Prefer clear, strongly typed C# over `var` when type is not obvious.

Keep file and type names aligned (for example `WotrGameAccessor.cs` -> `WotrGameAccessor`).

## Testing Guidelines
Tests use xUnit (`xunit`, `Microsoft.NET.Test.Sdk`, `coverlet.collector`).
- Name test files/classes with the `*Tests` suffix (for example `CharacterViewModelTests.cs`).
- Keep test data in `Arcemi.Tests/Files/` and embed/copy fixtures through the test project file.
- Run `dotnet test` before opening a PR; include coverage collection when needed:  
  `dotnet test --collect:"XPlat Code Coverage"`.

## Commit & Pull Request Guidelines
Recent history favors short, imperative commit messages (for example: `Fixed issue with path in manifest`, `Added missing portraits for W40KRT`).
- Keep commits focused and descriptive; separate refactors from behavior changes.
- PRs should include: change summary, impacted game/module, test evidence (`dotnet test` output), and screenshots for UI updates in `Arcemi.SaveGameEditor`.
- Link related issues and note any platform-specific packaging effects (Windows/Linux/macOS).
