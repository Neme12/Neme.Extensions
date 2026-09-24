# Copilot Instructions

## Project Guidelines
- In this codebase's Unix delete semantics, Move and Delete is only expected to guarantee race-free access to the parent directory; races on the file name itself are acceptable because neither renameat nor unlinkat support AT_EMPTY_PATH on Linux.
- For Linux persistent file IDs in this codebase, use the mount path as the persistent identifier instead of resolving a UUID.
- Before the first release in this codebase, serialization/version markers do not need to be bumped for unshipped format changes.

## File Handling
- In this workspace, Mono.Unix.Native.Stat does not expose a st_flags field, so Darwin file flags require separate interop rather than relying on Stat.
- For the Neme.Extensions.FileSystem API, prefer naming the handle-backed wrapper types as file system entries rather than emphasizing raw handle semantics, since they support sharing without implying locking. Use names that convey a stable reference to a specific file, such as FileReference.

## Testing Guidelines
- When adding tests in this repository, create a test class for the class being tested, with a nested class named after each method being tested, following the patterns used in SqlServerMigrationBuilderExtensionsTests and FileIOTests. Within each nested class, name the test methods after the scenario or expected behavior without repeating the method name already captured by the nested class.
- When updating paired sync/async test files, keep them fully symmetrical and place equivalent checks in the same relative locations in both files.
- Do *not* use Path.GetTempFileName to create temporary files. Use FileIO.CreateTempFileHandle or FileSession.CreateTempFile and a using statement instead, which will automatically delete the file afterwards.

## General Guidelines
- When identifying the root cause of an issue, trust that diagnosis and avoid changing unrelated logic such as equality implementations unless evidence requires it.