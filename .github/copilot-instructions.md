# Copilot Instructions

## Project Guidelines
- In this codebase's Unix delete semantics, Move and Delete is only expected to guarantee race-free access to the parent directory; races on the file name itself are acceptable because neither renameat nor unlinkat support AT_EMPTY_PATH on Linux.
- For Linux persistent file IDs in this codebase, use the mount path as the persistent identifier instead of resolving a UUID.
- Before the first release in this codebase, serialization/version markers do not need to be bumped for unshipped format changes.

## File Handling
- In this workspace, Mono.Unix.Native.Stat does not expose a st_flags field, so Darwin file flags require separate interop rather than relying on Stat.
- For the Neme.Extensions.FileSystem API, prefer naming the handle-backed wrapper types as file system entries rather than emphasizing raw handle semantics, since they support sharing without implying locking. Use names that convey a stable reference to a specific file, such as FileReference.
- Do not use File.Exists checks as they are unreliable in the presence of races. Instead, attempt the operation and handle the exception if the operation fails. There are helpers such as `File.DeleteIfExists`, `Directory.DeleteIfExists` and `File.MoveIfExists` in `Neme.Extensions.IO` that do this for you.

## Testing Guidelines
- When adding tests in this repository, create a test class for the class being tested, with a nested class named after each method being tested, following the patterns used in SqlServerMigrationBuilderExtensionsTests and FileIOTests. Within each nested class, name the test methods after the scenario or expected behavior without repeating the method name already captured by the nested class.
- When updating paired sync/async test files, keep them fully symmetrical and place equivalent checks in the same relative locations in both files.
  - Do *not* use Path.GetTempFileName to create temporary files. Instead, use FileOperations.CreateTempFileHandle, FileSession.CreateTempFile or FileReference.CreateTempFile when any of these are available; otherwise, create a generated temp path under Path.GetTempPath() and open it with FileStream using FileMode.CreateNew plus FileOptions.DeleteOnClose for automatic cleanup without the need for a finally block.
- Prefer classic using statement over using declarations for disposable objects. The scope of each using statement must be as small as possible: only code that directly uses the disposable variable should be inside it. Move assertions, unrelated setup, captured-result checks, and any other code that does not use that variable outside the using block so resources are not held longer than necessary.
- When a using statement, if statement, or similar construct has exactly one statement in its body, and the language permits it, prefer omitting braces and keeping it as a single-line statement. Do not switch to using declarations for this; keep classic using statements.
- In this codebase's C# extension-members, extension properties can be invoked through their emitted accessor methods such as `get_IsAsync(handle)`; direct member syntax `handle.IsAsync` is also valid when appropriate.
- Do not skip tests because of suspected production bugs; instead, identify and report the production bugs immediately so they can be fixed right away.
- When deleting files in tests, use the shared `File.DeleteIfExists` helper from `Neme.Extensions.IO` instead of adding a local `DeleteIfExists` helper. Use the `FileUtilities.CreateTemporaryFileScope` test helper in a using scope so that the file is deleted automatically without needing a finally block. Better yet, when possible, prefer using the DeleteOnClose option when creating temporary files for automatic cleanup. This option is available in all of FileOperations, FileSession, and FileReference APIs.

## General Guidelines
- Prefer `var` by default wherever the language allows it. In particular, prefer `var x = new Foo();` over `Foo x = new();`. In cases where an explicit type is necessary, e.g. because the variable is uninitialized, use the explicit type and do not add unnecessary initializers.
- When identifying the root cause of an issue, trust that diagnosis and avoid changing unrelated logic such as equality implementations unless evidence requires it.