# Copilot Instructions

## Project Guidelines
- In this codebase's Unix delete semantics, Move and Delete is only expected to guarantee race-free access to the parent directory; races on the file name itself are acceptable because neither renameat nor unlinkat support AT_EMPTY_PATH on Linux.
- For Linux persistent file IDs in this codebase, use the mount path as the persistent identifier instead of resolving a UUID.
- Before the first release in this codebase, serialization/version markers do not need to be bumped for unshipped format changes.

## File Handling
- In this workspace, Mono.Unix.Native.Stat does not expose a st_flags field, so Darwin file flags require separate interop rather than relying on Stat.

## General Guidelines
- When identifying the root cause of an issue, trust that diagnosis and avoid changing unrelated logic such as equality implementations unless evidence requires it.