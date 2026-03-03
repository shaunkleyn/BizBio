module.exports = {
  apps: [
    {
      name: 'bizbio-frontend',
      port: '3000',
      exec_mode: 'cluster',
      instances: 2, // Adjust based on VPS CPU cores (2 for 1-2 cores, 4 for 4+ cores)
      script: './current/server/index.mjs',
      cwd: '/var/www/bizbio/ui',
      env: {
        NODE_ENV: 'production',
        PORT: 3000,
        HOST: '0.0.0.0', // Listen on all interfaces for cloudflared
        NITRO_PORT: 3000,
        NITRO_HOST: '0.0.0.0',
        PROFILES_DIR: '/var/www/bizbio/ui/profiles',
        NUXT_PUBLIC_API_URL: 'https://api.bizbio.co.za/api/v1',
        NUXT_PUBLIC_APP_INSIGHTS_CONNECTION_STRING: 'InstrumentationKey=cf8e0ef0-0844-4cfe-b49c-0838a880835b;IngestionEndpoint=https://southafricanorth-1.in.applicationinsights.azure.com/;LiveEndpoint=https://southafricanorth.livediagnostics.monitor.azure.com/;ApplicationId=12ed36f7-1a54-438d-b149-fac495a0c94d',
        NUXT_PUBLIC_GOOGLE_ANALYTICS_ID: 'G-FSFC9WQ181',
      },
      error_file: '/var/www/bizbio/ui/logs/err.log',
      out_file: '/var/www/bizbio/ui/logs/out.log',
      log_file: '/var/www/bizbio/ui/logs/combined.log',
      time: true,
      max_memory_restart: '512M', // Adjust based on VPS RAM (512M for 1GB VPS, 1G for 2GB+)
      node_args: '--max-old-space-size=1024', // Adjust based on VPS RAM
      autorestart: true,
      watch: false,
      max_restarts: 10,
      min_uptime: '10s',
      
      // Additional production settings
      kill_timeout: 5000,
      listen_timeout: 3000,
      wait_ready: true,
      
      // Log rotation
      merge_logs: true,
      log_date_format: 'YYYY-MM-DD HH:mm:ss Z',
      
      // Graceful shutdown
      shutdown_with_message: true,
      
      // Environment-specific settings
      env_production: {
        NODE_ENV: 'production',
        PORT: 3000,
        HOST: '0.0.0.0',
        PROFILES_DIR: '/var/www/bizbio/ui/profiles',
        NUXT_PUBLIC_API_URL: 'https://api.bizbio.co.za/api/v1',
        NUXT_PUBLIC_APP_INSIGHTS_CONNECTION_STRING: 'InstrumentationKey=cf8e0ef0-0844-4cfe-b49c-0838a880835b;IngestionEndpoint=https://southafricanorth-1.in.applicationinsights.azure.com/;LiveEndpoint=https://southafricanorth.livediagnostics.monitor.azure.com/;ApplicationId=12ed36f7-1a54-438d-b149-fac495a0c94d',
        NUXT_PUBLIC_GOOGLE_ANALYTICS_ID: 'G-FSFC9WQ181',
      }
    },
    // SnapTap instance — temporary port 3001 during brand transition (Phase A).
    // Once snaptap.* is verified, Nginx proxy_pass is switched to :3000 and
    // this entry is removed (see plan Phase 7.4 cutover steps).
    {
      name: 'snaptap-frontend',
      port: '3001',
      exec_mode: 'cluster',
      instances: 2,
      script: './server/index.mjs',
      cwd: '/var/www/snaptap/ui/current',
      env: {
        NODE_ENV: 'production',
        PORT: 3001,
        HOST: '0.0.0.0',
        NITRO_PORT: 3001,
        NITRO_HOST: '0.0.0.0',
        PROFILES_DIR: '/var/www/snaptap/ui/profiles',
        NUXT_PUBLIC_API_URL: 'https://api.bizbio.co.za/api/v1',
        NUXT_PUBLIC_APP_INSIGHTS_CONNECTION_STRING: 'InstrumentationKey=cf8e0ef0-0844-4cfe-b49c-0838a880835b;IngestionEndpoint=https://southafricanorth-1.in.applicationinsights.azure.com/;LiveEndpoint=https://southafricanorth.livediagnostics.monitor.azure.com/;ApplicationId=12ed36f7-1a54-438d-b149-fac495a0c94d',
        NUXT_PUBLIC_GOOGLE_ANALYTICS_ID: 'G-FSFC9WQ181',

	      NUXT_GOOGLE_WALLET_ISSUER_ID: '3388000000023074317',
	      NUXT_GOOGLE_WALLET_SERVICE_ACCOUNT_EMAIL: 'bizbio-wallet@ultra-climber-488417-m1.iam.gserviceaccount.com',
	      NUXT_GOOGLE_WALLET_SERVICE_ACCOUNT_KEY: '-----BEGIN PRIVATE KEY-----\\nMIIEvwIBADANBgkqhkiG9w0BAQEFAASCBKkwggSlAgEAAoIBAQCz+Db74+1re/YG\\n0+kbMx/srK1dOcmjlxvKHZBjizknEL/HxFvQi1QSQ77YZYvrcqXL2hOVPsT0MoDV\\nZOpkbWXDUVE7h5w57VCsgHtlsU+BbIxM0t090aj/HdwxFut87fi2ZpxZTilKTHOw\\n1fchboo5Hl5llMfwY22GPcpp1f8iFAN6R1jn0xrNc6G1Hi/q2i8JBFuHBhkulh/h\\nFV4iPzw6V0BNj/GFOGhhWPvcESoHJb3LZcEQpkdcgskcZcI7JR2WkSiMKKnoL1PN\\nx+GRKO6UHx0LuJmQijQvdNACiMFOVevCOL2P+xHAqIzHM8cHe4aiqQWVxPVQggw7\\nESPOaS2zAgMBAAECggEAGfyQOpywVnyz9fYZFZ+IZL07BphGuJOkDxy9ZypIyo5y\\nwCPnMBHrm1GN7U2dv8CHwTY67VTUIqtniqiuPGnG2nD+U6QNbRiKisYTS11S13jh\\nj2GZUbOu/TDbb86D8BT6p003a/Z1ztqC5WNtR7YK88bMpvVYo3+akS+QmpSsaPKQ\\nU8+Eop/lAdnee7uc8NWTCn6ZG/3bJunfJuMa8DjuR8oPhwC4ZSOcQ3+mrBiW6BHX\\nxl6K/ARybV2gmJd8XfY4kesAtnOaIQ104HYHAcedGVfiHAqbF8/J/utsERR7kYky\\nkWgl4HnmIzKlgX92EEXIaKjN62A42kTOZ5d6PripZQKBgQDpVBu3qnIu1Q++1cm/\\n63YGoleWXvKv9jGht7lQNjfXKgcjXofBW5HV7XiSFcqDiktQWVMBAhTLDOl+8H1I\\niPUHi6RFrG5SX6D3JsoOnWqJkVEVSSo2FS3PMSgnhpO35RtZG/yeF4rCzHx0eUkb\\nmYWRexSZ4W9BdwVGRKBx9/poTwKBgQDFdNaK/pNzqYlElRY549jdV7z/qeQ6XhSI\\nmerEze2CnMN227Nx+JrTNCqV+ckQj09FOgfI1EgUru3xGJYI5OGiOmwwy3Bv/30w\\nrlx/r50mj96IkRudoy9d4cng/w46N6KDibBasrFaPInSLAe1ZX9g0KxS4jUCGlvh\\nz6si8A7nXQKBgQC+gXrEAfmKfVGGyb0Y4uIR1pjW4J0byFKLrJs0f6AmIF2FplgH\\nKoGu/l4UQJCBx/doGhZoW0+o3dkDWM2h3JjbWUt9y6SjwwqE4PnW5vYmbiyayYsa\\nbz5gzczwQLP5UKl8nxop5iTYmeb/nktvqnBg8PJIFcUl2UzIK54oj4S3hwKBgQCs\\nFO4gxkqFA1xw26k6HFrUU/GPsmUHhRRKFDGpAHqcWLh0bnYNvoZXyF/Qwpa7Ctof\\nz7oXCQIknGsbsbyX1bjaZCYDxFiiErvs4BgW41qGz901kZjKofvWyr7gigfBLtk/\\n35BFCvRLWvUVqG+zY9hF7LYxqZhe08/9lLllwmaKtQKBgQC1b24OGwK62vC4oPYg\\n3IoERAv8/Lhr0C/WO8TVopjcQvKV8krqAfebwIGWOGQx2YL6NHiE0uWelBPrYW+m\\nojmuxnwdW83mP2of/E3P7OctoIH4fYfJW6Wgv1Zp6z/xNh9UY8qE64ont4zasT4R\\nSJjYoDtBPlMYvbR2mprJndQIWA==\\n-----END PRIVATE KEY-----'

      },
      error_file: '/var/www/snaptap/ui/logs/err.log',
      out_file: '/var/www/snaptap/ui/logs/out.log',
      log_file: '/var/www/snaptap/ui/logs/combined.log',
      time: true,
      max_memory_restart: '512M',
      node_args: '--max-old-space-size=1024',
      autorestart: true,
      watch: false,
      max_restarts: 10,
      min_uptime: '10s',
      kill_timeout: 5000,
      listen_timeout: 3000,
      wait_ready: true,
      merge_logs: true,
      log_date_format: 'YYYY-MM-DD HH:mm:ss Z',
      shutdown_with_message: true,
      env_production: {
        NODE_ENV: 'production',
        PORT: 3001,
        HOST: '0.0.0.0',
        PROFILES_DIR: '/var/www/snaptap/ui/profiles',
        NUXT_PUBLIC_API_URL: 'https://api.bizbio.co.za/api/v1',
        NUXT_PUBLIC_APP_INSIGHTS_CONNECTION_STRING: 'InstrumentationKey=cf8e0ef0-0844-4cfe-b49c-0838a880835b;IngestionEndpoint=https://southafricanorth-1.in.applicationinsights.azure.com/;LiveEndpoint=https://southafricanorth.livediagnostics.monitor.azure.com/;ApplicationId=12ed36f7-1a54-438d-b149-fac495a0c94d',
        NUXT_PUBLIC_GOOGLE_ANALYTICS_ID: 'G-FSFC9WQ181',
	      NUXT_GOOGLE_WALLET_ISSUER_ID: '3388000000023074317',
	      NUXT_GOOGLE_WALLET_SERVICE_ACCOUNT_EMAIL: 'bizbio-wallet@ultra-climber-488417-m1.iam.gserviceaccount.com',
	      NUXT_GOOGLE_WALLET_SERVICE_ACCOUNT_KEY: '-----BEGIN PRIVATE KEY-----\\nMIIEvwIBADANBgkqhkiG9w0BAQEFAASCBKkwggSlAgEAAoIBAQCz+Db74+1re/YG\\n0+kbMx/srK1dOcmjlxvKHZBjizknEL/HxFvQi1QSQ77YZYvrcqXL2hOVPsT0MoDV\\nZOpkbWXDUVE7h5w57VCsgHtlsU+BbIxM0t090aj/HdwxFut87fi2ZpxZTilKTHOw\\n1fchboo5Hl5llMfwY22GPcpp1f8iFAN6R1jn0xrNc6G1Hi/q2i8JBFuHBhkulh/h\\nFV4iPzw6V0BNj/GFOGhhWPvcESoHJb3LZcEQpkdcgskcZcI7JR2WkSiMKKnoL1PN\\nx+GRKO6UHx0LuJmQijQvdNACiMFOVevCOL2P+xHAqIzHM8cHe4aiqQWVxPVQggw7\\nESPOaS2zAgMBAAECggEAGfyQOpywVnyz9fYZFZ+IZL07BphGuJOkDxy9ZypIyo5y\\nwCPnMBHrm1GN7U2dv8CHwTY67VTUIqtniqiuPGnG2nD+U6QNbRiKisYTS11S13jh\\nj2GZUbOu/TDbb86D8BT6p003a/Z1ztqC5WNtR7YK88bMpvVYo3+akS+QmpSsaPKQ\\nU8+Eop/lAdnee7uc8NWTCn6ZG/3bJunfJuMa8DjuR8oPhwC4ZSOcQ3+mrBiW6BHX\\nxl6K/ARybV2gmJd8XfY4kesAtnOaIQ104HYHAcedGVfiHAqbF8/J/utsERR7kYky\\nkWgl4HnmIzKlgX92EEXIaKjN62A42kTOZ5d6PripZQKBgQDpVBu3qnIu1Q++1cm/\\n63YGoleWXvKv9jGht7lQNjfXKgcjXofBW5HV7XiSFcqDiktQWVMBAhTLDOl+8H1I\\niPUHi6RFrG5SX6D3JsoOnWqJkVEVSSo2FS3PMSgnhpO35RtZG/yeF4rCzHx0eUkb\\nmYWRexSZ4W9BdwVGRKBx9/poTwKBgQDFdNaK/pNzqYlElRY549jdV7z/qeQ6XhSI\\nmerEze2CnMN227Nx+JrTNCqV+ckQj09FOgfI1EgUru3xGJYI5OGiOmwwy3Bv/30w\\nrlx/r50mj96IkRudoy9d4cng/w46N6KDibBasrFaPInSLAe1ZX9g0KxS4jUCGlvh\\nz6si8A7nXQKBgQC+gXrEAfmKfVGGyb0Y4uIR1pjW4J0byFKLrJs0f6AmIF2FplgH\\nKoGu/l4UQJCBx/doGhZoW0+o3dkDWM2h3JjbWUt9y6SjwwqE4PnW5vYmbiyayYsa\\nbz5gzczwQLP5UKl8nxop5iTYmeb/nktvqnBg8PJIFcUl2UzIK54oj4S3hwKBgQCs\\nFO4gxkqFA1xw26k6HFrUU/GPsmUHhRRKFDGpAHqcWLh0bnYNvoZXyF/Qwpa7Ctof\\nz7oXCQIknGsbsbyX1bjaZCYDxFiiErvs4BgW41qGz901kZjKofvWyr7gigfBLtk/\\n35BFCvRLWvUVqG+zY9hF7LYxqZhe08/9lLllwmaKtQKBgQC1b24OGwK62vC4oPYg\\n3IoERAv8/Lhr0C/WO8TVopjcQvKV8krqAfebwIGWOGQx2YL6NHiE0uWelBPrYW+m\\nojmuxnwdW83mP2of/E3P7OctoIH4fYfJW6Wgv1Zp6z/xNh9UY8qE64ont4zasT4R\\nSJjYoDtBPlMYvbR2mprJndQIWA==\\n-----END PRIVATE KEY-----'

      }
    }
  ],
  
  // Deployment configuration (optional - for PM2 deploy)
  deploy: {
    production: {
      user: 'SSH_USERNAME',
      host: 'SSH_HOSTMACHINE',
      ref: 'origin/main',
      repo: 'git@github.com:shaunkleyn/BizBio.Frontend.git',
      path: '/var/www/bizbio/ui',
      'pre-deploy-local': '',
      'post-deploy': 'npm install && npm run build && pm2 reload ecosystem.config.cjs --env production',
      'pre-setup': '',
      'ssh_options': 'ForwardAgent=yes'
    }
  }
}