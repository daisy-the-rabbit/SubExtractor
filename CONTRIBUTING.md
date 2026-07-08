# Contributing

Thanks for your interest in improving SubExtractor. This is a small project, so the process is light.

## Building

SubExtractor targets .NET 10 on 64-bit Windows and uses WinForms and native Windows APIs.

- Install the .NET 10 SDK.
- Build the solution:

      dotnet build DvdSubOcr.sln

- Build a self-contained single-file release:

      dotnet publish DvdSubExtractor/DvdSubExtractor.csproj -c Release -r win-x64 --self-contained -p:PublishSingleFile=true -o publish

## Testing

There are no automated tests. Before opening a pull request, build the app and walk through the affected wizard steps by hand: load a DVD folder or a `.sup` file, run the extraction and OCR, and check the exported SSA/SRT output.

## Commit messages

Commits and pull request titles use conventional-commit prefixes: `feat`, `fix`, `docs`, `style`, `refactor`, `perf`, `test`, `build`, `ci`, `chore`, `revert`. Keep the subject a short, lowercase, imperative summary, for example `fix: handle empty subtitle packet`. Branch names follow `type/short-description`.

## Pull requests

- Keep a pull request focused on a single change.
- Update `CHANGELOG.md` when the change is user-facing; the release workflow builds its notes from it.
- Pull requests are squash-merged into `main`, and the required checks (build and formatting) must pass first.
