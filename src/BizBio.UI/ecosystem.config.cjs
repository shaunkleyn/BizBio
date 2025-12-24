module.exports = {
  apps: [
    {
      name: 'bizbio-frontend',
      port: '3000',
      exec_mode: 'cluster',
      instances: 2, // Adjust based on VPS CPU cores (2 for 1-2 cores, 4 for 4+ cores)
      script: './.output/server/index.mjs',
      env: {
        NODE_ENV: 'production',
        PORT: 3000,
        HOST: '0.0.0.0', // Listen on all interfaces for cloudflared
        NITRO_PORT: 3000,
        NITRO_HOST: '0.0.0.0',
        NUXT_PUBLIC_API_URL: 'https://api.bizbio.co.za/api/v1',
        NUXT_PUBLIC_APP_INSIGHTS_CONNECTION_STRING: 'InstrumentationKey=cf8e0ef0-0844-4cfe-b49c-0838a880835b;IngestionEndpoint=https://southafricanorth-1.in.applicationinsights.azure.com/;LiveEndpoint=https://southafricanorth.livediagnostics.monitor.azure.com/;ApplicationId=12ed36f7-1a54-438d-b149-fac495a0c94d',
        NUXT_PUBLIC_GOOGLE_ANALYTICS_ID: 'G-FSFC9WQ181',
      },
      error_file: './logs/err.log',
      out_file: './logs/out.log',
      log_file: './logs/combined.log',
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
        NUXT_PUBLIC_API_URL: 'https://api.bizbio.co.za/api/v1',
        NUXT_PUBLIC_APP_INSIGHTS_CONNECTION_STRING: 'InstrumentationKey=cf8e0ef0-0844-4cfe-b49c-0838a880835b;IngestionEndpoint=https://southafricanorth-1.in.applicationinsights.azure.com/;LiveEndpoint=https://southafricanorth.livediagnostics.monitor.azure.com/;ApplicationId=12ed36f7-1a54-438d-b149-fac495a0c94d',
        NUXT_PUBLIC_GOOGLE_ANALYTICS_ID: 'G-FSFC9WQ181',
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
