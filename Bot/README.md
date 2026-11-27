# VHVLOSE Telegram order bot

A serverless Telegram bot for collecting VHVLOSE orders and letting one configured admin manually confirm bank transfers. It runs as a Cloudflare Worker with a Cloudflare D1 database; no always-on PC or Termux is required.

## Order flow

1. Buyer sends `/start` in a private chat.
2. Buyer chooses 1–5 devices.
3. Bot creates an order number and sends the exact amount, bank-card details, and asks for the sender name shown by the bank.
4. Buyer replies with the sender name. Only then does the order become `pending_review` and appear in the admin's `/check` list.
5. Admin checks their bank app independently and presses **Подтвердить оплату** or **Отменить**.
6. On confirmation, the bot marks the order paid and sends a generated license key with the selected device limit.

The bot does not treat screenshots or a buyer's message as proof of payment. The admin must verify the actual incoming transfer in their bank app. The bot's confirmation button only records the admin's decision.

## Pricing

Current formula: `100 ₽ × device count + PRICE_SURCHARGE_RUB`.

Set `PRICE_VERSION` and `PRICE_SURCHARGE_RUB` for each release. For example, version 1.1 uses surcharge 20, so 1 device costs 120 ₽ and 5 devices cost 520 ₽. Increase the surcharge by 20 ₽ for each later paid update as agreed.

## Required secrets

Set these as Cloudflare Worker secrets (never commit them to GitHub):

- `BOT_TOKEN` — token from BotFather.
- `ADMIN_TELEGRAM_ID` — your numeric Telegram user ID.
- `BANK_CARD` — payment card number / transfer details shown to buyers.
- `WEBHOOK_SECRET` — long random secret used in the webhook URL.

Worker variables in `wrangler.toml`:

- `PRICE_VERSION` — e.g. `1.1`.
- `PRICE_SURCHARGE_RUB` — e.g. `20`.

## Deploy with Cloudflare dashboard

1. Create a Cloudflare account and a Worker named `vhvlose-telegram-bot`.
2. Create a D1 database named `vhvlose-orders`.
3. In D1 Console, run the contents of `schema.sql`.
4. Deploy the code in `src/index.ts` as a TypeScript Worker and bind D1 as `DB`.
5. Add the four secrets above and the two pricing variables.
6. Copy the deployed Worker URL. Set the Telegram webhook by opening this URL in a browser, replacing placeholders:

```text
https://api.telegram.org/bot<BOT_TOKEN>/setWebhook?url=https://<WORKER_SUBDOMAIN>.workers.dev/telegram/<WEBHOOK_SECRET>
```

For production, call `setWebhook` using a POST request with Telegram's `secret_token` option as well, and configure a matching header check in the Worker. Do not share the bot token, webhook secret, or bank-card details publicly.

## Important limitations

- This first bot release issues random order/license codes and stores the selected activation limit. The existing Windows client has not yet been connected to this bot's license database, so these keys are not yet accepted by the desktop app. Client-side secure activation and server-side device counting are a separate integration step.
- There is no automatic bank API verification. Admin confirmation is manual, exactly as requested.
- A bank-card number is sensitive payment information. Only configure a card you are authorized to use, and confirm with your bank/payment provider that this payment method is permitted for your sales and location.
- Cloudflare free-plan limits and eligibility can change. Check current Cloudflare terms before launch.
