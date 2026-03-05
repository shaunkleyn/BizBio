#!/bin/bash
# Usage: 4-purge-cloudflare-cache.sh <ZoneId> <ApiToken>
set -e

ZONE_ID="$1"
API_TOKEN="$2"

response=$(curl -s -X POST "https://api.cloudflare.com/client/v4/zones/$ZONE_ID/purge_cache" \
  -H "Authorization: Bearer $API_TOKEN" \
  -H "Content-Type: application/json" \
  --data '{"purge_everything":true}')

echo "$response"

if echo "$response" | grep -q '"success":[[:space:]]*true'; then
  echo "Cloudflare cache purged successfully"
else
  echo "Cloudflare purge failed"
  exit 1
fi
