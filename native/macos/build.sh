#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd -- "${SCRIPT_DIR}/../.." && pwd)"
OUTPUT_ROOT="${REPO_ROOT}/runtimes"
SOURCE_FILE="${SCRIPT_DIR}/libneme_macos_shim.c"
LIB_NAME="libneme_macos_shim.dylib"
CC="${CC:-$(xcrun --sdk macosx --find clang)}"
SDKROOT="${SDKROOT:-$(xcrun --sdk macosx --show-sdk-path)}"
UNIVERSAL_OUTPUT_DIR="${OUTPUT_ROOT}/osx/native"
HOST_ARCH="$(uname -m)"
X64_OUTPUT="${OUTPUT_ROOT}/osx-x64/native/${LIB_NAME}"
ARM64_OUTPUT="${OUTPUT_ROOT}/osx-arm64/native/${LIB_NAME}"
UNIVERSAL_OUTPUT="${UNIVERSAL_OUTPUT_DIR}/${LIB_NAME}"

mkdir -p "${UNIVERSAL_OUTPUT_DIR}" "${OUTPUT_ROOT}/osx-x64/native" "${OUTPUT_ROOT}/osx-arm64/native"

"${CC}" -isysroot "${SDKROOT}" -dynamiclib -arch x86_64 -o "${X64_OUTPUT}" "${SOURCE_FILE}"
"${CC}" -isysroot "${SDKROOT}" -dynamiclib -arch arm64 -o "${ARM64_OUTPUT}" "${SOURCE_FILE}"

case "${HOST_ARCH}" in
  x86_64)
	cp "${X64_OUTPUT}" "${UNIVERSAL_OUTPUT}"
	;;
  arm64)
	cp "${ARM64_OUTPUT}" "${UNIVERSAL_OUTPUT}"
	;;
  *)
	echo "Unsupported macOS host architecture: ${HOST_ARCH}" >&2
	exit 1
	;;
esac

echo "Built ${UNIVERSAL_OUTPUT}"
echo "Built ${X64_OUTPUT}"
echo "Built ${ARM64_OUTPUT}"
