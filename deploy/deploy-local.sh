#!/bin/bash
# Local Deployment Script for BizBio API
# This script builds and deploys the BizBio API from your local machine to the VPS

set -e  # Exit on error

# Configuration - Update these values
VPS_HOST="${VPS_HOST:-169.239.218.60}"
VPS_USER="${VPS_USER:-root}"
SSH_KEY="${SSH_KEY:-$HOME/.ssh/id_rsa}"
SOLUTION_PATH="BizBio.sln"
PROJECT_PATH="src/BizBio.API/BizBio.API.csproj"

# Colors
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m'

log_info() {
    echo -e "${GREEN}[INFO]${NC} $1"
}

log_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

log_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

log_step() {
    echo -e "${BLUE}[STEP]${NC} $1"
}

# Check prerequisites
check_prerequisites() {
    log_step "Checking prerequisites..."

    # Check if dotnet is installed
    if ! command -v dotnet &> /dev/null; then
        log_error "dotnet CLI not found. Please install .NET 8 SDK"
        exit 1
    fi

    # Check if ssh is available
    if ! command -v ssh &> /dev/null; then
        log_error "ssh not found"
        exit 1
    fi

    # Check if SSH key exists
    if [ ! -f "$SSH_KEY" ]; then
        log_error "SSH key not found at $SSH_KEY"
        log_info "Please set SSH_KEY environment variable or create SSH key"
        exit 1
    fi

    # Test SSH connection
    log_info "Testing SSH connection to VPS..."
    if ! ssh -i "$SSH_KEY" -o BatchMode=yes -o ConnectTimeout=5 "$VPS_USER@$VPS_HOST" "echo 'Connection successful'" 2>/dev/null; then
        log_error "Cannot connect to VPS. Please check SSH configuration"
        exit 1
    fi

    log_info "✓ All prerequisites met"
}

# Build the application
build_application() {
    log_step "Building application..."

    # Clean previous builds
    log_info "Cleaning previous builds..."
    dotnet clean $SOLUTION_PATH --configuration Release

    # Restore dependencies
    log_info "Restoring dependencies..."
    dotnet restore $SOLUTION_PATH

    # Build solution
    log_info "Building solution..."
    dotnet build $SOLUTION_PATH --configuration Release --no-restore

    # Publish API
    log_info "Publishing API..."
    dotnet publish $PROJECT_PATH \
        --configuration Release \
        --output ./publish \
        --no-build

    log_info "✓ Build completed successfully"
}

# Create deployment package
create_package() {
    log_step "Creating deployment package..."

    cd publish
    zip -r ../BizBio.API.zip . > /dev/null
    cd ..

    log_info "✓ Deployment package created: BizBio.API.zip ($(du -h BizBio.API.zip | cut -f1))"
}

# Upload to VPS
upload_to_vps() {
    log_step "Uploading to VPS..."

    # Create temporary directory on VPS
    ssh -i "$SSH_KEY" "$VPS_USER@$VPS_HOST" "mkdir -p /tmp/bizbio-deploy"

    # Upload deployment package
    log_info "Uploading deployment package..."
    scp -i "$SSH_KEY" BizBio.API.zip "$VPS_USER@$VPS_HOST:/tmp/bizbio-deploy/"

    # Upload deployment script
    log_info "Uploading deployment script..."
    scp -i "$SSH_KEY" deploy/deploy.sh "$VPS_USER@$VPS_HOST:/tmp/bizbio-deploy/"

    log_info "✓ Upload completed"
}

# Execute deployment on VPS
deploy_on_vps() {
    log_step "Executing deployment on VPS..."

    ssh -i "$SSH_KEY" "$VPS_USER@$VPS_HOST" << 'ENDSSH'
        cd /tmp/bizbio-deploy
        chmod +x deploy.sh
        sudo ./deploy.sh
ENDSSH

    log_info "✓ Deployment completed on VPS"
}

# Verify deployment
verify_deployment() {
    log_step "Verifying deployment..."

    sleep 10

    log_info "Testing API health endpoint..."
    if curl -f -s https://api.bizbio.co.za/health > /dev/null; then
        log_info "✓ API is responding correctly"
    else
        log_warning "Health check failed. API might still be starting..."
        log_info "Please check manually: https://api.bizbio.co.za/health"
    fi
}

# Cleanup
cleanup() {
    log_step "Cleaning up..."

    # Remove local build artifacts
    rm -rf publish
    rm -f BizBio.API.zip

    # Cleanup remote temporary files
    ssh -i "$SSH_KEY" "$VPS_USER@$VPS_HOST" "rm -rf /tmp/bizbio-deploy" || true

    log_info "✓ Cleanup completed"
}

# Show deployment info
show_info() {
    log_info ""
    log_info "=========================================="
    log_info "Deployment Information"
    log_info "=========================================="
    log_info "VPS Host: $VPS_HOST"
    log_info "VPS User: $VPS_USER"
    log_info "API URL: https://api.bizbio.co.za"
    log_info "Swagger: https://api.bizbio.co.za/index.html"
    log_info "Health: https://api.bizbio.co.za/health"
    log_info "=========================================="
    log_info ""
}

# Main function
main() {
    echo ""
    echo "=========================================="
    echo "BizBio API - Local Deployment Script"
    echo "=========================================="
    echo ""

    # Show configuration
    log_info "Configuration:"
    log_info "  VPS: $VPS_USER@$VPS_HOST"
    log_info "  SSH Key: $SSH_KEY"
    echo ""

    # Confirm deployment
    read -p "Do you want to proceed with deployment? (yes/no): " -r
    echo
    if [[ ! $REPLY =~ ^[Yy]es$ ]]; then
        log_warning "Deployment cancelled"
        exit 0
    fi

    # Set trap for cleanup on exit
    trap cleanup EXIT

    # Execute deployment steps
    check_prerequisites
    build_application
    create_package
    upload_to_vps
    deploy_on_vps
    verify_deployment

    echo ""
    echo "=========================================="
    log_info "✅ Deployment completed successfully!"
    echo "=========================================="
    echo ""

    show_info
}

# Run main function
main
