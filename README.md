# DVD Subtitle Extractor

A wizard-style Windows application that converts subtitles from DVDs and PGS (Blu-ray `.sup`) files into Advanced SubStation Alpha (`.ssa`) and SRT (`.srt`) text format using OCR (optical character recognition).

## About

DVD Subtitle Extractor allows you to pick program chains, angles, audio and subtitle tracks from an unencrypted DVD folder and create `.mpg`, `.d2v` and `.bin` files for each. DGIndex is used to help line up subtitles to the video since DVD programs often have discontinuities that affect sync. If you already have the subtitle track in a separate file, you can skip ahead and begin at the OCR step of the wizard.

The OCR engine uses exact pattern matching for DVD subtitles, with fuzzy logic for HD subtitles. A large built-in OCR database means most DVDs require manual matching of only a few characters. Some ambiguous characters (`l`, `I`, `1`, `o`) must be manually confirmed per disc to avoid false positives.

Features include:

- DVD subtitle extraction from unencrypted disc folders
- PGS (Blu-ray `.sup`) and Sub/Idx file conversion
- Advanced SubStation Alpha and SRT output formats
- Sophisticated line and word layout analysis
- Support for accents and non-English character sets, including Thai (the application UI is English only)

## History

DVD Subtitle Extractor (SubExtractor) was originally created by **Christopher R Meadowcroft** and hosted on [CodePlex](https://web.archive.org/web/20171224054057/http://subextractor.codeplex.com/). The last release was version 1.0.3.2, targeting .NET Framework 4.0. After CodePlex was shut down, the source code was recovered from the archive.

## What's new in 2.0.0

Version 2.0.0 is a major modernization of the codebase, bringing the application from .NET Framework to .NET 10 with extensive code and UI improvements.

### Platform upgrade

- Retargeted from .NET Framework 4.0 to 4.8 (required to open in Visual Studio 2026)
- Migrated all 3 projects (~31,000 LOC) from classic `.csproj` to SDK-style format targeting .NET 10 (`net10.0-windows`)
- Removed legacy ClickOnce, bootstrapper properties, and the Visual Studio installer project

### UI modernization

- Light/dark theme support with restart-to-apply behaviour
- High-DPI support with configurable DPI mode setting:
  - **Per-Monitor** - Each monitor uses its own DPI scale; the application rescales when moved between monitors
  - **System** - Uses the DPI of the primary monitor for all displays
  - **Disabled** - No DPI scaling applied
- All forms switched to `AutoScaleMode.Dpi` with 96 DPI baseline for consistent scaling
- Modern fonts: Segoe UI, Consolas, and Segoe UI Symbol
- Theme-aware system colors replacing hardcoded colour values
- Dynamic listbox widths, aligned Settings dropdowns, Unicode toolbar buttons
- Option to hide the help text panel
- Various layout fixes and control anchoring improvements

### Code modernization

- C# records for data classes (`CellIdVobId`, `DemuxResult`, `EncodeMatch`, `CreateSubOptions`, `PartOfTitle`)
- `BinaryPrimitives` for big-endian binary reads/writes, replacing manual bit shifting
- `Span<T>` and managed arrays replacing `unsafe` pointer code
- Task-based `ReadAsync` replacing APM `BeginRead`/`EndRead`
- `using` declarations and `Dispose()` replacing manual `Close()`/`try-finally`
- File-scoped namespaces and collection expressions
- `FrozenDictionary` for static lookup tables
- `SortedSet` replacing `SortedDictionary` with null values
- Global usings to reduce per-file boilerplate
- Switch expressions, pattern matching, string interpolation, null-conditional event invocation
- Removed `GC.Collect()` calls and OOM-retry allocation patterns
- Dead code removal and project cleanup

### Backwards compatibility

- Legacy OcrMap.bin and OCR database files from version 1.0.x are fully compatible with version 2.0.0

## Requirements

- Windows 7 or later, 64-bit

No additional runtime installation is required - the .NET 10 runtime is bundled in the release builds.

## Building from source

Requires the [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0).

```
dotnet build
```

## License

This project is licensed under the [GNU General Public License v2.0](LICENSE), as established by the [original CodePlex listing](https://web.archive.org/web/20171224054057/http://subextractor.codeplex.com/license).

## Credits

- **Christopher R Meadowcroft** - Original author (SubExtractor 1.0, 2009-2012)
- .NET 10 migration supported by [GitHub Copilot Application Modernization](https://learn.microsoft.com/en-us/dotnet/core/porting/github-copilot-app-modernization/overview)
- Further development supported by [Claude Code](https://claude.ai/claude-code)
