#!/bin/bash
# SnapTap Deployment Script
# Usage: ./deploy-snaptap.sh <release_id>
# This script can be used for manual deployments

set -e

# Configuration
TARGET_DIR="/var/www/snaptap/ui"
RELEASE_ID="${1:-$(date +%Y%m%d%H%M%S)}"
RELEASE_DIR="${TARGET_DIR}/releases/${RELEASE_ID}"

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

echo -e "${GREEN}==========================================${NC}"
echo -e "${GREEN}SnapTap UI Deployment - Release ${RELEASE_ID}${NC}"
echo -e "${GREEN}==========================================${NC}"

# Check if running as root or with sudo
if [ "$EUID" -ne 0 ]; then
  echo -e "${RED}Please run as root or with sudo${NC}"
  exit 1
fi

# Check if release directory exists
if [ ! -d "${RELEASE_DIR}" ]; then
  echo -e "${RED}Release directory not found: ${RELEASE_DIR}${NC}"
  exit 1
fi

# Check if ui-build exists
if [ ! -d "${RELEASE_DIR}/ui-build" ]; then
  echo -e "${YELLOW}Extracting build artifact...${NC}"
  if [ -f "${RELEASE_DIR}/ui.zip" ]; then
    cd ${RELEASE_DIR}
    unzip -q ui.zip -d ui-build
    rm ui.zip
  else
    echo -e "${RED}No ui-build directory or ui.zip found${NC}"
    exit 1
  fi
fi

echo -e "${GREEN}>>> Build contents:${NC}"
ls -la ${RELEASE_DIR}/ui-build/

# --------------------------------------------
# Update Nginx configuration
# --------------------------------------------
echo -e "${YELLOW}>>> Updating Nginx configuration...${NC}"
if [ -f "${RELEASE_DIR}/config/snaptap.co.za.conf" ]; then
  cp ${RELEASE_DIR}/config/snaptap.co.za.conf /etc/nginx/sites-available/snaptap.co.za
  nginx -t
  if [ $? -eq 0 ]; then
    systemctl reload nginx
    echo -e "${GREEN}>>> Nginx configuration updated and reloaded${NC}"
  else
    echo -e "${RED}>>> ERROR: Nginx config test failed${NC}"
    exit 1
  fi
else
  echo -e "${YELLOW}>>> No nginx config in release, keeping existing${NC}"
fi

# --------------------------------------------
# Update ecosystem.config.cjs
# --------------------------------------------
echo -e "${YELLOW}>>> Updating PM2 ecosystem config...${NC}"
if [ -f "${RELEASE_DIR}/config/ecosystem.config.cjs" ]; then
  cp ${RELEASE_DIR}/config/ecosystem.config.cjs ${TARGET_DIR}/ecosystem.config.cjs
  chown www-data:www-data ${TARGET_DIR}/ecosystem.config.cjs
  echo -e "${GREEN}>>> Ecosystem config updated${NC}"
fi

# --------------------------------------------
# Backup current release
# --------------------------------------------
echo -e "${YELLOW}>>> Managing release symlinks...${NC}"
if [ -L ${TARGET_DIR}/current ]; then
  CURRENT_TARGET=$(readlink ${TARGET_DIR}/current)
  echo -e "${YELLOW}>>> Current points to: ${CURRENT_TARGET}${NC}"
  echo -e "${YELLOW}>>> Backing up current to previous...${NC}"
  rm -dfr ${TARGET_DIR}/previous || true
  mv ${TARGET_DIR}/current ${TARGET_DIR}/previous
fi

# --------------------------------------------
# Create symlink to new release
# --------------------------------------------
echo -e "${YELLOW}>>> Creating symlink to new release...${NC}"
ln -sfn ${RELEASE_DIR}/ui-build/. ${TARGET_DIR}/current
echo -e "${GREEN}>>> Symlink created: current -> ${RELEASE_DIR}/ui-build${NC}"

# --------------------------------------------
# Set permissions
# --------------------------------------------
echo -e "${YELLOW}>>> Setting permissions...${NC}"
chown -R www-data:www-data ${TARGET_DIR}/current

# --------------------------------------------
# Ensure profiles directory exists
# --------------------------------------------
echo -e "${YELLOW}>>> Ensuring profiles directory exists...${NC}"
if [ ! -d "${TARGET_DIR}/profiles" ]; then
  mkdir -p ${TARGET_DIR}/profiles
  chown -R www-data:www-data ${TARGET_DIR}/profiles
  echo -e "${GREEN}>>> Created profiles directory${NC}"
fi

# --------------------------------------------
# Ensure logs directory exists
# --------------------------------------------
echo -e "${YELLOW}>>> Ensuring logs directory exists...${NC}"
mkdir -p ${TARGET_DIR}/logs
chown -R www-data:www-data ${TARGET_DIR}/logs

# --------------------------------------------
# Reload PM2
# --------------------------------------------
echo -e "${YELLOW}>>> Reloading PM2...${NC}"
cd ${TARGET_DIR}

# Run PM2 commands as the appropriate user
if pm2 list 2>/dev/null | grep -q "snaptap-frontend"; then
  pm2 reload ecosystem.config.cjs --only snaptap-frontend --env production
else
  pm2 start ecosystem.config.cjs --only snaptap-frontend --env production
fi
pm2 save

# --------------------------------------------
# Cleanup old releases (keep last 5)
# --------------------------------------------
echo -e "${YELLOW}>>> Cleaning up old releases...${NC}"
cd ${TARGET_DIR}/releases
RELEASE_COUNT=$(ls -d */ 2>/dev/null | wc -l)
if [ "$RELEASE_COUNT" -gt 5 ]; then
  ls -dt */ | tail -n +6 | xargs -r rm -rf
  echo -e "${GREEN}>>> Removed old releases, keeping last 5${NC}"
fi
echo -e "${GREEN}>>> Remaining releases:${NC}"
ls -la ${TARGET_DIR}/releases/

# --------------------------------------------
# Verify deployment
# --------------------------------------------
echo -e "${YELLOW}>>> Verifying deployment...${NC}"
sleep 3

# Check if app is responding
HTTP_CODE=$(curl -s -o /dev/null -w "%{http_code}" http://localhost:3001/ || echo "000")
if [ "$HTTP_CODE" = "200" ]; then
  echo -e "${GREEN}>>> Application is responding (HTTP ${HTTP_CODE})${NC}"
else
  echo -e "${RED}>>> WARNING: Application returned HTTP ${HTTP_CODE}${NC}"
  echo -e "${YELLOW}>>> PM2 logs:${NC}"
  pm2 logs snaptap-frontend --lines 20 --nostream
fi

# Check static asset
ASSET_CODE=$(curl -s -o /dev/null -w "%{http_code}" http://localhost:3001/profile-assets/bizbio-engine.js || echo "000")
if [ "$ASSET_CODE" = "200" ]; then
  echo -e "${GREEN}>>> Profile assets are accessible (HTTP ${ASSET_CODE})${NC}"
else
  echo -e "${RED}>>> WARNING: Profile assets returned HTTP ${ASSET_CODE}${NC}"
fi

echo -e "${GREEN}==========================================${NC}"
echo -e "${GREEN}Deployment Complete!${NC}"
echo -e "${GREEN}==========================================${NC}"
echo -e "Release: ${RELEASE_ID}"
echo -e "Current: ${TARGET_DIR}/current -> $(readlink ${TARGET_DIR}/current)"
echo -e "${GREEN}==========================================${NC}"
pm2 status snaptap-frontend
