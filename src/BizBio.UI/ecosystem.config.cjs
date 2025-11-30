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
