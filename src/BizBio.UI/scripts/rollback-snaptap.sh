#!/bin/bash
# SnapTap Rollback Script
# Usage: ./rollback-snaptap.sh
# Rolls back to the previous release

set -e

# Configuration
TARGET_DIR="/var/www/snaptap/ui"

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

echo -e "${YELLOW}==========================================${NC}"
echo -e "${YELLOW}SnapTap UI Rollback${NC}"
echo -e "${YELLOW}==========================================${NC}"

# Check if running as root or with sudo
if [ "$EUID" -ne 0 ]; then
  echo -e "${RED}Please run as root or with sudo${NC}"
  exit 1
fi

# Check if previous symlink exists
if [ ! -L "${TARGET_DIR}/previous" ]; then
  echo -e "${RED}No previous release found to rollback to${NC}"
  exit 1
fi

CURRENT_TARGET=$(readlink ${TARGET_DIR}/current 2>/dev/null || echo "none")
PREVIOUS_TARGET=$(readlink ${TARGET_DIR}/previous)

echo -e "Current:  ${CURRENT_TARGET}"
echo -e "Previous: ${PREVIOUS_TARGET}"

read -p "Are you sure you want to rollback? (y/N) " -n 1 -r
echo
if [[ ! $REPLY =~ ^[Yy]$ ]]; then
  echo -e "${YELLOW}Rollback cancelled${NC}"
  exit 0
fi

# --------------------------------------------
# Perform rollback
# --------------------------------------------
echo -e "${YELLOW}>>> Performing rollback...${NC}"

# Swap current and previous
rm -f ${TARGET_DIR}/current
mv ${TARGET_DIR}/previous ${TARGET_DIR}/current

echo -e "${GREEN}>>> Symlink updated: current -> $(readlink ${TARGET_DIR}/current)${NC}"

# --------------------------------------------
# Set permissions
# --------------------------------------------
echo -e "${YELLOW}>>> Setting permissions...${NC}"
chown -R www-data:www-data ${TARGET_DIR}/current

# --------------------------------------------
# Reload PM2
# --------------------------------------------
echo -e "${YELLOW}>>> Reloading PM2...${NC}"
cd ${TARGET_DIR}
pm2 reload ecosystem.config.cjs --only snaptap-frontend --env production
pm2 save

# --------------------------------------------
# Verify rollback
# --------------------------------------------
echo -e "${YELLOW}>>> Verifying rollback...${NC}"
sleep 3

HTTP_CODE=$(curl -s -o /dev/null -w "%{http_code}" http://localhost:3001/ || echo "000")
if [ "$HTTP_CODE" = "200" ]; then
  echo -e "${GREEN}>>> Application is responding (HTTP ${HTTP_CODE})${NC}"
else
  echo -e "${RED}>>> WARNING: Application returned HTTP ${HTTP_CODE}${NC}"
  pm2 logs snaptap-frontend --lines 20 --nostream
fi

echo -e "${GREEN}==========================================${NC}"
echo -e "${GREEN}Rollback Complete!${NC}"
echo -e "${GREEN}==========================================${NC}"
echo -e "Current: ${TARGET_DIR}/current -> $(readlink ${TARGET_DIR}/current)"
echo -e "${GREEN}==========================================${NC}"
pm2 status snaptap-frontend
