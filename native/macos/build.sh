#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd -- "${SCRIPT_DIR}/../.." && pwd)"
OUTPUT_ROOT="${REPO_ROOT}/runtimes"
SOURCE_FILE="${SCRIPT_DIR}/libneme_macos_shim.c"
LIB_NAME="libneme_macos_shim.dylib"
CC="${CC:-$(xcrun --sdk macosx --find clang)}"
SDKROOT="${SDKROOT:-$(xcrun --sdk macosx --show-sdk-path)}"

mkdir -p "${OUTPUT_ROOT}/osx-x64/native" "${OUTPUT_ROOT}/osx-arm64/native"

"${CC}" -isysroot "${SDKROOT}" -dynamiclib -arch x86_64 -o "${OUTPUT_ROOT}/osx-x64/native/${LIB_NAME}" "${SOURCE_FILE}"
"${CC}" -isysroot "${SDKROOT}" -dynamiclib -arch arm64 -o "${OUTPUT_ROOT}/osx-arm64/native/${LIB_NAME}" "${SOURCE_FILE}"

echo "Built ${OUTPUT_ROOT}/osx-x64/native/${LIB_NAME}"
echo "Built ${OUTPUT_ROOT}/osx-arm64/native/${LIB_NAME}"
