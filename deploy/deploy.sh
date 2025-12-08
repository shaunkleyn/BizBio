#!/bin/bash
# BizBio API Deployment Script
# This script deploys the BizBio API to the VPS server
# Can be run locally or by Azure DevOps pipeline

set -e  # Exit on error
set -u  # Exit on undefined variable

# Configuration
DEPLOYMENT_DIR="/var/www/bizbio-api"
BACKUP_DIR="/var/www"
UPLOADS_DIR="/var/www/uploads"
SERVICE_NAME="bizbio-api"
TEMP_DIR="/tmp/bizbio-deploy"
MAX_BACKUPS=5

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Helper functions
log_info() {
    echo -e "${GREEN}[INFO]${NC} $1"
}

log_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

log_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Check if script is run with sudo
check_sudo() {
    if [ "$EUID" -ne 0 ]; then
        log_error "This script must be run with sudo or as root"
        exit 1
    fi
}

# Stop the service
stop_service() {
    log_info "Stopping $SERVICE_NAME service..."
    systemctl stop $SERVICE_NAME || true
    sleep 2
}

# Create backup
create_backup() {
    if [ -d "$DEPLOYMENT_DIR" ]; then
        BACKUP_NAME="bizbio-api.backup.$(date +%Y%m%d_%H%M%S)"
        log_info "Creating backup: $BACKUP_NAME"
        cp -r $DEPLOYMENT_DIR $BACKUP_DIR/$BACKUP_NAME

        # Clean up old backups (keep only last MAX_BACKUPS)
        log_info "Cleaning up old backups (keeping last $MAX_BACKUPS)..."
        cd $BACKUP_DIR
        ls -t bizbio-api.backup.* 2>/dev/null | tail -n +$((MAX_BACKUPS + 1)) | xargs -r rm -rf
    else
        log_warning "No existing deployment found, skipping backup"
    fi
}

# Deploy new version
deploy() {
    log_info "Deploying new version..."

    # Create deployment directory
    mkdir -p $DEPLOYMENT_DIR

    # Check if deployment artifact exists
    if [ ! -f "$TEMP_DIR/BizBio.API.zip" ]; then
        log_error "Deployment artifact not found: $TEMP_DIR/BizBio.API.zip"
        exit 1
    fi

    # Extract files
    log_info "Extracting deployment artifact..."
    unzip -o $TEMP_DIR/BizBio.API.zip -d $DEPLOYMENT_DIR/

    # Set permissions
    log_info "Setting permissions..."
    chown -R www-data:www-data $DEPLOYMENT_DIR
    chmod -R 755 $DEPLOYMENT_DIR

    # Ensure uploads directory exists
    log_info "Ensuring uploads directory exists..."
    mkdir -p $UPLOADS_DIR
    chown -R www-data:www-data $UPLOADS_DIR
    chmod -R 755 $UPLOADS_DIR
}

# Run database migrations (optional)
run_migrations() {
    if [ "${RUN_MIGRATIONS:-false}" = "true" ]; then
        log_info "Running database migrations..."
        cd $DEPLOYMENT_DIR
        # Uncomment the line below when migrations are needed
        # sudo -u www-data dotnet BizBio.API.dll ef database update
        log_warning "Database migrations are commented out. Uncomment in deploy.sh if needed."
    else
        log_info "Skipping database migrations (RUN_MIGRATIONS not set to true)"
    fi
}

# Start the service
start_service() {
    log_info "Starting $SERVICE_NAME service..."
    systemctl start $SERVICE_NAME
    sleep 5

    # Check service status
    if systemctl is-active --quiet $SERVICE_NAME; then
        log_info "$SERVICE_NAME service started successfully"
        systemctl status $SERVICE_NAME --no-pager
    else
        log_error "$SERVICE_NAME service failed to start"
        systemctl status $SERVICE_NAME --no-pager || true
        exit 1
    fi
}

# Verify deployment
verify_deployment() {
    log_info "Verifying deployment..."

    # Wait a bit for the service to fully start
    sleep 10

    # Check if service is running
    if systemctl is-active --quiet $SERVICE_NAME; then
        log_info "✓ Service is running"
    else
        log_error "✗ Service is not running"
        return 1
    fi

    # Test health endpoint (if accessible locally)
    if command -v curl &> /dev/null; then
        log_info "Testing health endpoint..."
        if curl -f http://localhost:5000/health 2>/dev/null; then
            log_info "✓ Health check passed"
        else
            log_warning "Health check failed or endpoint not accessible locally"
            log_info "Check https://api.bizbio.co.za/health from external network"
        fi
    else
        log_warning "curl not found, skipping health check"
    fi
}

# Cleanup temporary files
cleanup() {
    log_info "Cleaning up temporary files..."
    rm -rf $TEMP_DIR
}

# Rollback to previous version
rollback() {
    log_error "Deployment failed! Rolling back to previous version..."

    stop_service

    # Find the most recent backup
    LATEST_BACKUP=$(ls -t $BACKUP_DIR/bizbio-api.backup.* 2>/dev/null | head -n 1)

    if [ -n "$LATEST_BACKUP" ]; then
        log_info "Restoring from backup: $LATEST_BACKUP"
        rm -rf $DEPLOYMENT_DIR
        cp -r $LATEST_BACKUP $DEPLOYMENT_DIR

        start_service
        log_info "Rollback completed"
    else
        log_error "No backup found for rollback!"
        exit 1
    fi
}

# Main deployment process
main() {
    log_info "=========================================="
    log_info "BizBio API Deployment Script"
    log_info "=========================================="
    log_info "Starting deployment at $(date)"
    log_info ""

    # Check prerequisites
    check_sudo

    # Set trap to rollback on error
    trap rollback ERR

    # Deployment steps
    stop_service
    create_backup
    deploy
    run_migrations
    start_service
    verify_deployment
    cleanup

    # Remove error trap
    trap - ERR

    log_info ""
    log_info "=========================================="
    log_info "✅ Deployment completed successfully!"
    log_info "=========================================="
    log_info "Completed at $(date)"
}

# Run main function
main
