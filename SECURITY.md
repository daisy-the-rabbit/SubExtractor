# Security policy

## Reporting a vulnerability

Please report security issues privately rather than opening a public issue.

This repository has GitHub private vulnerability reporting enabled. To report:

- Open the repository's [Security tab](https://github.com/daisy-the-rabbit/SubExtractor/security/advisories) and choose **Report a vulnerability**, or go straight to https://github.com/daisy-the-rabbit/SubExtractor/security/advisories/new.

The report stays confidential as a draft security advisory until a fix is available. You will get an acknowledgement once it has been seen.

## What to include

- The version you are running (Help > About, or the release filename).
- The kind of input that triggers it, for example a specific DVD/Blu-ray subtitle or `.sup` file, and a minimal sample if you can share one.
- Steps to reproduce and what you observed (crash, hang, or a write outside the chosen output folder).

SubExtractor parses untrusted binary input: VOB and IFO files, MPEG program streams, and PGS/SUP subtitle data. Malformed-file crashes and parsing bugs are the most likely security-relevant reports. The OCR path uses unsafe code, so memory-safety issues there are in scope.

## Supported versions

Fixes are made against the latest release. Older versions are not maintained separately.

## Scope

SubExtractor is a local desktop application and runs no network services. The reports that matter most are those where opening or processing a crafted input file leads to a crash, a hang, a memory-safety issue, or a write outside the output location you selected.
