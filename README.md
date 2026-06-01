# Neme.Extensions

## Neme.Extensions
These packages contain my custom extensions and utilities for .NET that I have needed as some point or another and in my opinion should be part of .NET.

### Neme.Extensions.FileSystem
The goal of this project (specifically the class FileIO) is to provide race-free file operations. Because the file system is inherently concurrent, performing operations by path alone is fundamentally subject to race conditions. This project aims to:

1. Provide file operations on a file handle instead of a path.
2. Provide file operations by a file ID, which does not change when the file is moved.
3. Add the ability to open files/directories via a handle to a root directory and subpath.
4. Add the ability to open files/directories via a file ID, which does not change when the file is moved.

## Neme.Polyfills
This package contains what it says - polyfills for .NET APIs on previous target frameworks.

## Can I use this?
Feel free use any of this code or the packages in your project, but I make no guarantees that the code is correct and doesn't contain bugs. No attribution necessary for my code, unless you're using code I copied from .NET repositories.

## Can I contribute?
Issues and PRs are welcome, especially for fixing bugs, converting extensions to the new C# 14 extension syntax, and implementing classes on Unix that currently only have Windows implementations.

## License

This repository is licensed under the [MIT](LICENSE) license. See [THIRD-PARTY-NOTICES](THIRD-PARTY-NOTICES.txt) for license notices about third-party code used in this repository.
