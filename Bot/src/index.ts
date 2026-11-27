interface Env {
  DB: D1Database;
  BOT_TOKEN: string;
  ADMIN_TELEGRAM_ID: string;
  BANK_CARD: string;
  WEBHOOK_SECRET: string;
  PRICE_VERSION: string;
  PRICE_SURCHARGE_RUB: string;
  CURRENCY_LABEL: string;
}

type TelegramUser = { id: number; first_name?: string; username?: string };
type TelegramMessage = {
  message_id: number;
  chat: { id: number; type?: string };
  from?: TelegramUser;
  text?: string;
};
type TelegramCallback = {
  id: string;
  from: TelegramUser;
  message?: TelegramMessage;
  data?: string;
};
type TelegramUpdate = {
  message?: TelegramMessage;
  callback_query?: TelegramCallback;
};

const json = (value: unknown, status = 200) =>
  new Response(JSON.stringify(value), {
    status,
    headers: { "content-type": "application/json; charset=utf-8" },
  });

export default {
  async fetch(request: Request, env: Env): Promise<Response> {
    const url = new URL(request.url);
    if (url.pathname === "/health") return json({ ok: true, service: "VHVLOSE bot" });

    if (url.pathname !== `/telegram/${env.WEBHOOK_SECRET}`) {
      return new Response("Not found", { status: 404 });
    }
    if (request.method !== "POST") return new Response("Method not allowed", { status: 405 });
    if (request.headers.get("X-Telegram-Bot-Api-Secret-Token") !== env.WEBHOOK_SECRET) {
      return new Response("Unauthorized", { status: 401 });
    }

    let update: TelegramUpdate;
    try {
      update = await request.json<TelegramUpdate>();
    } catch {
      return json({ ok: false, error: "Invalid JSON" }, 400);
    }

    try {
      if (update.callback_query) await handleCallback(update.callback_query, env);
      if (update.message) await handleMessage(update.message, env);
      return json({ ok: true });
    } catch (error) {
      console.error("Telegram update failed", error instanceof Error ? error.message : "unknown");
      return json({ ok: false }, 500);
    }
  },
};

async function telegram(env: Env, method: string, payload: Record<string, unknown>) {
  const response = await fetch(`https://api.telegram.org/bot${env.BOT_TOKEN}/${method}`, {
    method: "POST",
    headers: { "content-type": "application/json" },
    body: JSON.stringify(payload),
  });
  if (!response.ok) throw new Error(`Telegram API error: ${method} ${response.status}`);
  return response.json();
}

async function sendMessage(env: Env, chatId: number | string, text: string, replyMarkup?: unknown) {
  return telegram(env, "sendMessage", {
    chat_id: chatId,
    text,
    ...(replyMarkup ? { reply_markup: replyMarkup } : {}),
  });
}

async function answerCallback(env: Env, callbackId: string, text: string) {
  return telegram(env, "answerCallbackQuery", { callback_query_id: callbackId, text });
}

function isAdmin(user: TelegramUser | undefined, env: Env): boolean {
  return !!user && String(user.id) === env.ADMIN_TELEGRAM_ID;
}

function priceFor(devices: number, env: Env): number {
  const surcharge = Number.parseInt(env.PRICE_SURCHARGE_RUB || "0", 10);
  if (!Number.isInteger(surcharge) || surcharge < 0) throw new Error("Invalid price surcharge");
  return devices * 100 + surcharge;
}

function makeLicenseKey(): string {
  const bytes = new Uint8Array(18);
  crypto.getRandomValues(bytes);
  const alphabet = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
  let output = "VHV-";
  for (let i = 0; i < bytes.length; i++) {
    output += alphabet[bytes[i] % alphabet.length];
    if (i % 6 === 5 && i !== bytes.length - 1) output += "-";
  }
  return output;
}

async function handleMessage(message: TelegramMessage, env: Env) {
  const user = message.from;
  const text = (message.text || "").trim();
  if (!user || !message.chat || message.chat.type !== "private") return;

  if (text.startsWith("/check") || text.startsWith("/проверка")) {
    if (!isAdmin(user, env)) {
      await sendMessage(env, message.chat.id, "Команда недоступна.");
      return;
    }
    await showPendingOrders(env, message.chat.id);
    return;
  }

  if (text.startsWith("/start")) {
    await sendMessage(
      env,
      message.chat.id,
      "Добро пожаловать в VHVLOSE!\n\nВыберите количество устройств для лицензии. Цена: 100 ₽ за устройство + надбавка текущей версии."
      , {
        inline_keyboard: [[1, 2, 3, 4, 5].map((count) => ({
          text: `${count} ${count === 1 ? "устройство" : "устройств"} — ${priceFor(count, env)} ₽`,
          callback_data: `buy:${count}`,
        }))],
      }
    );
    return;
  }

  const session = await env.DB.prepare(
    "SELECT awaiting_order_id FROM user_sessions WHERE telegram_user_id = ?"
  ).bind(String(user.id)).first<{ awaiting_order_id: number | null }>();

  if (session?.awaiting_order_id) {
    const senderName = text.replace(/\\s+/g, " ").slice(0, 100);
    if (senderName.length < 2 || text.startsWith("/")) {
      await sendMessage(env, message.chat.id, "Введите имя отправителя так, как оно указано в банковском переводе.");
      return;
    }
    const result = await env.DB.prepare(
      "UPDATE orders SET sender_name = ?, status = 'pending_review' WHERE id = ? AND telegram_user_id = ? AND status = 'awaiting_sender'"
    ).bind(senderName, session.awaiting_order_id, String(user.id)).run();

    await env.DB.prepare(
      "UPDATE user_sessions SET awaiting_order_id = NULL, updated_at = strftime('%Y-%m-%dT%H:%M:%fZ', 'now') WHERE telegram_user_id = ?"
    ).bind(String(user.id)).run();

    if (!result.meta.changes) {
      await sendMessage(env, message.chat.id, "Этот заказ уже обработан. Нажмите /start, чтобы создать новый.");
      return;
    }

    const order = await env.DB.prepare("SELECT order_code, device_count, amount_rub FROM orders WHERE id = ?")
      .bind(session.awaiting_order_id).first<{ order_code: string; device_count: number; amount_rub: number }>();
    await sendMessage(env, message.chat.id, `Спасибо! Заказ ${order?.order_code} отправлен администратору на проверку оплаты. Ключ будет выдан после подтверждения поступления денег.`);
    await sendMessage(env, env.ADMIN_TELEGRAM_ID,
      `Новый заказ ожидает проверки\nЗаказ: ${order?.order_code}\nУстройств: ${order?.device_count}\nСумма: ${order?.amount_rub} ₽\nИмя отправителя: ${senderName}\n\nОткройте /check для проверки.`);
    return;
  }

  await sendMessage(env, message.chat.id, "Нажмите /start, чтобы оформить заказ.");
}

async function handleCallback(callback: TelegramCallback, env: Env) {
  const data = callback.data || "";
  const chatId = callback.message?.chat.id;
  if (!chatId) {
    await answerCallback(env, callback.id, "Сообщение недоступно");
    return;
  }

  if (data.startsWith("buy:")) {
    const count = Number.parseInt(data.slice(4), 10);
    if (!Number.isInteger(count) || count < 1 || count > 5) {
      await answerCallback(env, callback.id, "Неверное количество устройств");
      return;
    }
    const amount = priceFor(count, env);
    const insert = await env.DB.prepare(
      "INSERT INTO orders (order_code, telegram_user_id, chat_id, device_count, amount_rub, price_version, status) VALUES (?, ?, ?, ?, ?, ?, 'awaiting_sender')"
    ).bind("TMP-" + crypto.randomUUID(), String(callback.from.id), String(chatId), count, amount, env.PRICE_VERSION || "1.0").run();
    const id = Number(insert.meta.last_row_id);
    const orderCode = `VHV-${String(id).padStart(4, "0")}`;
    await env.DB.prepare("UPDATE orders SET order_code = ? WHERE id = ?").bind(orderCode, id).run();
    await env.DB.prepare(
      "INSERT INTO user_sessions (telegram_user_id, chat_id, awaiting_order_id, updated_at) VALUES (?, ?, ?, strftime('%Y-%m-%dT%H:%M:%fZ', 'now')) ON CONFLICT(telegram_user_id) DO UPDATE SET chat_id = excluded.chat_id, awaiting_order_id = excluded.awaiting_order_id, updated_at = excluded.updated_at"
    ).bind(String(callback.from.id), String(chatId), id).run();

    await answerCallback(env, callback.id, "Заказ создан");
    await sendMessage(env, chatId,
      `Ваш заказ: ${orderCode}\nВерсия: ${env.PRICE_VERSION || "1.0"}\nУстройств: ${count}\nК оплате: ${amount} ₽\n\nПереведите точную сумму на карту:\n${env.BANK_CARD}\n\nПосле перевода отправьте следующим сообщением имя отправителя, как оно указано в банковском приложении.`);
    return;
  }

  if (data.startsWith("confirm:") || data.startsWith("cancel:")) {
    if (!isAdmin(callback.from, env)) {
      await answerCallback(env, callback.id, "Нет доступа");
      return;
    }
    const [action, rawId] = data.split(":");
    const orderId = Number.parseInt(rawId, 10);
    if (!Number.isInteger(orderId)) {
      await answerCallback(env, callback.id, "Неверный номер заказа");
      return;
    }
    if (action === "confirm") {
      const order = await env.DB.prepare(
        "SELECT id, order_code, telegram_user_id, chat_id, device_count, amount_rub FROM orders WHERE id = ? AND status = 'pending_review'"
      ).bind(orderId).first<{
        id: number; order_code: string; telegram_user_id: string; chat_id: string;
        device_count: number; amount_rub: number;
      }>();
      if (!order) {
        await answerCallback(env, callback.id, "Заказ уже обработан или не найден");
        return;
      }
      const licenseKey = makeLicenseKey();
      const update = await env.DB.prepare(
        "UPDATE orders SET status = 'paid', reviewed_at = strftime('%Y-%m-%dT%H:%M:%fZ', 'now') WHERE id = ? AND status = 'pending_review'"
      ).bind(orderId).run();
      if (!update.meta.changes) {
        await answerCallback(env, callback.id, "Заказ уже обработан");
        return;
      }
      await env.DB.prepare(
        "INSERT INTO licenses (order_id, license_key, activation_limit) VALUES (?, ?, ?)"
      ).bind(orderId, licenseKey, order.device_count).run();
      await sendMessage(env, order.chat_id,
        `Оплата заказа ${order.order_code} подтверждена!\nВаш лицензионный ключ VHVLOSE:\n${licenseKey}\nЛимит устройств: ${order.device_count}\nСохраните ключ.`);
      await answerCallback(env, callback.id, "Оплата подтверждена, ключ выдан");
      if (callback.message) {
        await telegram(env, "editMessageReplyMarkup", {
          chat_id: chatId, message_id: callback.message.message_id, reply_markup: { inline_keyboard: [] },
        });
      }
      return;
    }

    const cancelled = await env.DB.prepare(
      "UPDATE orders SET status = 'cancelled', reviewed_at = strftime('%Y-%m-%dT%H:%M:%fZ', 'now') WHERE id = ? AND status = 'pending_review'"
    ).bind(orderId).run();
    await answerCallback(env, callback.id, cancelled.meta.changes ? "Заказ отменён" : "Заказ уже обработан");
    if (callback.message) {
      await telegram(env, "editMessageReplyMarkup", {
        chat_id: chatId, message_id: callback.message.message_id, reply_markup: { inline_keyboard: [] },
      });
    }
  }
}

async function showPendingOrders(env: Env, chatId: number) {
  const result = await env.DB.prepare(
    "SELECT id, order_code, device_count, amount_rub, sender_name, created_at FROM orders WHERE status = 'pending_review' ORDER BY id ASC LIMIT 30"
  ).all<{
    id: number; order_code: string; device_count: number; amount_rub: number;
    sender_name: string | null; created_at: string;
  }>();

  if (!result.results.length) {
    await sendMessage(env, chatId, "Заказов, ожидающих проверки, сейчас нет.");
    return;
  }

  await sendMessage(env, chatId, `Заказы, ожидающие проверки: ${result.results.length}`);
  for (const order of result.results) {
    await sendMessage(env, chatId,
      `Заказ ${order.order_code}\nОтправитель: ${order.sender_name || "не указан"}\nУстройств: ${order.device_count}\nСумма: ${order.amount_rub} ₽\nСоздан: ${order.created_at}`,
      {
        inline_keyboard: [[
          { text: "Подтвердить оплату", callback_data: `confirm:${order.id}` },
          { text: "Отменить", callback_data: `cancel:${order.id}` },
        ]],
      }
    );
  }
}
