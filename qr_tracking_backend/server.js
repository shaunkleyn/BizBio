const express = require('express');
const mysql = require('mysql2');
const cors = require('cors');

const app = express();
const PORT = 3000;
app.use(cors());

// MySQL Connection
const db = mysql.createConnection({
  host: '169.239.218.60',
  port: 3306,
  user: 'bizbioco_qr_user',
  password: 'j@tVt@k1Wbk#kzR$4$8D',
  database: 'bizbioco_qr_tracking'
});

db.connect(err => {
  if (err) {
    console.error('Database connection failed:', err.stack);
    return;
  }
  console.log('Connected to MySQL database');
});

// Endpoint to track QR scans
app.get('/track', (req, res) => {
  const { id } = req.query;
  const ip = req.headers['x-forwarded-for'] || req.socket.remoteAddress;
  const userAgent = req.headers['user-agent'];
  const referrer = req.get('Referrer') || 'Direct';
  const timestamp = new Date();

  if (!id) {
    return res.status(400).json({ error: 'Missing tracking ID' });
  }

  // Store visit in database
  const query = 'INSERT INTO visits (qr_id, ip, user_agent, referrer, timestamp) VALUES (?, ?, ?, ?, ?)';
  db.query(query, [id, ip, userAgent, referrer, timestamp], (err) => {
    if (err) {
      console.error('Error logging visit:', err);
      return res.status(500).json({ error: 'Database error' });
    }

    // Fetch the target URL
    db.query('SELECT target_url FROM qr_codes WHERE id = ?', [id], (err, results) => {
      if (err || results.length === 0) {
        return res.status(404).json({ error: 'QR code not found' });
      }
      res.redirect(results[0].target_url);
    });
  });
});

// API to get analytics
app.get('/analytics', (req, res) => {
  const query = 'SELECT qr_id, COUNT(*) AS scans, COUNT(DISTINCT ip) AS unique_visitors FROM visits GROUP BY qr_id';
  db.query(query, (err, results) => {
    if (err) {
      console.error('Error fetching analytics:', err);
      return res.status(500).json({ error: 'Database error' });
    }
    res.json(results);
  });
});

app.listen(PORT, () => {
  console.log(`Server running on http://localhost:${PORT}`);
});
