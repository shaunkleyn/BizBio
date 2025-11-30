#!/bin/bash

# BizBio Frontend Deployment Script
# This script automates the deployment process

set -e  # Exit on any error

echo "🚀 Starting BizBio Frontend deployment..."
echo "----------------------------------------"

# Navigate to project directory
cd /var/www/bizbio/ui

# Pull latest changes
echo "📥 Pulling latest changes from Git..."
git pull origin nervous-goldberg

# Install dependencies
echo "📦 Installing dependencies..."
npm install --production=false

# Build application
echo "🔨 Building application for production..."
npm run build

# Create logs directory if it doesn't exist
mkdir -p logs

# Restart PM2
echo "♻️  Restarting application with PM2..."
pm2 restart bizbio-frontend || pm2 start ecosystem.config.cjs

# Save PM2 configuration
pm2 save

# Show status
echo "----------------------------------------"
echo "✅ Deployment complete!"
echo "----------------------------------------"
pm2 status bizbio-frontend

echo ""
echo "📊 Recent logs:"
pm2 logs bizbio-frontend --lines 20 --nostream

echo ""
echo "💡 Tip: Run 'pm2 logs bizbio-frontend' to view live logs"
