#!/bin/bash

# Health check script for BizBio Frontend
# Run this periodically to ensure the application is healthy

URL="http://localhost:3000"
LOG_FILE="/var/log/bizbio-health.log"
TIMESTAMP=$(date '+%Y-%m-%d %H:%M:%S')

# Function to log messages
log_message() {
    echo "[$TIMESTAMP] $1" | tee -a $LOG_FILE
}

# Check if application is responding
log_message "Starting health check..."

RESPONSE=$(curl -s -o /dev/null -w "%{http_code}" $URL --max-time 10)

if [ $RESPONSE -eq 200 ]; then
    log_message "✅ Application is healthy (HTTP $RESPONSE)"
    
    # Check memory usage
    MEMORY=$(pm2 jlist | grep -o '"memory":[0-9]*' | head -1 | grep -o '[0-9]*')
    if [ ! -z "$MEMORY" ]; then
        MEMORY_MB=$((MEMORY / 1024 / 1024))
        log_message "Memory usage: ${MEMORY_MB}MB"
        
        # Alert if memory usage is too high (over 1GB)
        if [ $MEMORY_MB -gt 1024 ]; then
            log_message "⚠️  WARNING: High memory usage detected!"
        fi
    fi
    
    exit 0
else
    log_message "❌ Application is unhealthy (HTTP $RESPONSE)"
    log_message "Attempting to restart application..."
    
    # Restart the application
    pm2 restart bizbio-frontend
    
    # Wait a few seconds and check again
    sleep 5
    
    RETRY_RESPONSE=$(curl -s -o /dev/null -w "%{http_code}" $URL --max-time 10)
    
    if [ $RETRY_RESPONSE -eq 200 ]; then
        log_message "✅ Application restarted successfully"
        exit 0
    else
        log_message "❌ Application restart failed (HTTP $RETRY_RESPONSE)"
        log_message "Manual intervention required!"
        exit 1
    fi
fi
