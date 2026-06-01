# Copilot Instructions

## Project Guidelines
- In this codebase's Unix delete semantics, Delete is only expected to guarantee race-free access to the parent directory; races on the file name itself are acceptable because unlinkat does not support AT_EMPTY_PATH on Linux.