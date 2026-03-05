#!/bin/bash
# Usage: 2-create-build-info-file.sh <TargetDirectory> <BuildNumber> <ReleaseName> <AttemptNumber>
set -e

TARGET_DIR="$1"
BUILD_NUM="$2"
RELEASE_NAME="$3"
ATTEMPT="$4"
VERSION="${BUILD_NUM}_${RELEASE_NAME}_${ATTEMPT}"

printf '{"build":"%s","release":"%s","attempt":"%s","version":"%s"}\n' \
  "$BUILD_NUM" "$RELEASE_NAME" "$ATTEMPT" "$VERSION" \
  > $TARGET_DIR/current/public/build-info.json
