CREATE TABLE IF NOT EXISTS orders (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  order_code TEXT NOT NULL UNIQUE,
  telegram_user_id TEXT NOT NULL,
  chat_id TEXT NOT NULL,
  device_count INTEGER NOT NULL CHECK (device_count BETWEEN 1 AND 5),
  amount_rub INTEGER NOT NULL,
  price_version TEXT NOT NULL,
  sender_name TEXT,
  status TEXT NOT NULL CHECK (status IN ('awaiting_sender', 'pending_review', 'paid', 'cancelled')),
  created_at TEXT NOT NULL DEFAULT (strftime('%Y-%m-%dT%H:%M:%fZ', 'now')),
  reviewed_at TEXT
);

CREATE INDEX IF NOT EXISTS idx_orders_status_created
  ON orders(status, created_at DESC);

CREATE TABLE IF NOT EXISTS user_sessions (
  telegram_user_id TEXT PRIMARY KEY,
  chat_id TEXT NOT NULL,
  awaiting_order_id INTEGER,
  updated_at TEXT NOT NULL DEFAULT (strftime('%Y-%m-%dT%H:%M:%fZ', 'now'))
);

CREATE TABLE IF NOT EXISTS licenses (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  order_id INTEGER NOT NULL UNIQUE REFERENCES orders(id),
  license_key TEXT NOT NULL UNIQUE,
  activation_limit INTEGER NOT NULL CHECK (activation_limit BETWEEN 1 AND 5),
  created_at TEXT NOT NULL DEFAULT (strftime('%Y-%m-%dT%H:%M:%fZ', 'now'))
);
