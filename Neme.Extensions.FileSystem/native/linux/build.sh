#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd -- "${SCRIPT_DIR}/../../.." && pwd)"
OUTPUT_ROOT="${REPO_ROOT}/runtimes"
SOURCE_FILE="${SCRIPT_DIR}/libneme_linux_shim.c"
LIB_NAME="libneme_linux_shim.so"
CC="${CC:-cc}"
UNIVERSAL_OUTPUT_DIR="${OUTPUT_ROOT}/linux/native"
HOST_ARCH="$(uname -m)"

case "${HOST_ARCH}" in
  x86_64)
	RID="linux-x64"
	;;
  aarch64|arm64)
	RID="linux-arm64"
	;;
  *)
	echo "Unsupported Linux host architecture: ${HOST_ARCH}" >&2
	exit 1
	;;
esac

RID_OUTPUT_DIR="${OUTPUT_ROOT}/${RID}/native"
UNIVERSAL_OUTPUT="${UNIVERSAL_OUTPUT_DIR}/${LIB_NAME}"
RID_OUTPUT="${RID_OUTPUT_DIR}/${LIB_NAME}"

mkdir -p "${UNIVERSAL_OUTPUT_DIR}" "${RID_OUTPUT_DIR}"

"${CC}" -shared -fPIC -o "${RID_OUTPUT}" "${SOURCE_FILE}"
cp "${RID_OUTPUT}" "${UNIVERSAL_OUTPUT}"

echo "Built ${UNIVERSAL_OUTPUT}"
echo "Built ${RID_OUTPUT}"
