#!/bin/bash
# Usage: 1-deploy-files.sh <TargetDirectory> <BuildId>
set -e

TARGET_DIR="$1"
RELEASE_ID="$2"
  RELEASE_DIR="$TARGET_DIR/releases/$RELEASE_ID"

  echo "=========================================="
  echo "Extracting and Deploying Release $RELEASE_ID"
  echo "=========================================="
  echo ">>> Extracting build artifact..."
  cd $RELEASE_DIR
  unzip -qo ui.zip -d ui-build
  rm ui.zip

  echo "=========================================="
  echo "Deploying fonts (persistent)"
  echo "=========================================="
  if [ -f "$RELEASE_DIR/fonts.tar.gz" ]; then
    mkdir -p $TARGET_DIR/fonts
    tar -xzf $RELEASE_DIR/fonts.tar.gz -C $TARGET_DIR/fonts
    echo ">>> Fonts deployed to $TARGET_DIR/fonts"
    rm $RELEASE_DIR/fonts.tar.gz
  else
    echo ">>> No fonts.tar.gz — keeping existing fonts"
  fi

  echo "=========================================="
  echo "Updating ecosystem config"
  echo "=========================================="
  if [ -f "$RELEASE_DIR/ecosystem.config.cjs" ]; then
    cp $RELEASE_DIR/ecosystem.config.cjs $TARGET_DIR/ecosystem.config.cjs
    echo ">>> Ecosystem config updated"
  fi

  echo "=========================================="
  echo "Backing up current release"
  echo "=========================================="
  if [ -L $TARGET_DIR/current ]; then
    rm -rf $TARGET_DIR/previous || true
    mv $TARGET_DIR/current $TARGET_DIR/previous
    echo ">>> Backed up current to previous"
  fi

  ln -sfn $RELEASE_DIR/ui-build/. $TARGET_DIR/current
  echo ">>> Symlink created"

  mkdir -p $TARGET_DIR/profiles
  mkdir -p $TARGET_DIR/logs

  echo "=========================================="
  echo "Copying profiles..."
  echo "=========================================="
  cp -a $RELEASE_DIR/ui-build/public/profiles/. $TARGET_DIR/profiles

  echo "=========================================="
  echo "Updating nginx config"
  echo "=========================================="
  if [ -f "$RELEASE_DIR/config/nginx/snaptap.co.za" ]; then
    cp $RELEASE_DIR/config/nginx/snaptap.co.za /etc/nginx/sites-available/snaptap.co.za
    nginx -t && systemctl reload nginx
    echo ">>> Nginx config updated and reloaded"
  fi