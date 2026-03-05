#!/bin/bash
# Usage: 3-finalise-deployment.sh <TargetDirectory>
set -e

TARGET_DIR="$1"

echo "=========================================="
echo "Reloading PM2"
echo "=========================================="
cd $TARGET_DIR
if pm2 list | grep -q "snaptap-frontend"; then
  pm2 reload ecosystem.config.cjs --only snaptap-frontend --env production
else
  pm2 start ecosystem.config.cjs --only snaptap-frontend --env production
fi
pm2 save

echo "=========================================="
echo "Removing redundant releases (keep last 5)"
echo "=========================================="
cd $TARGET_DIR/releases
ls -dt */ | tail -n +6 | xargs -r rm -rf

echo "=========================================="
echo "Deployment Complete!"
echo "=========================================="
