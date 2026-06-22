import { jsonBody } from './api.js';

// URL base de la API MongoDB (mensajería). El proxy /mongo-api apunta a localhost:5191.
const MONGO_BASE = (import.meta.env ?? {}).VITE_MONGO_API_BASE_URL ?? '/mongo-api';

async function mongoFetch(path, options) {
  const res = await fetch(`${MONGO_BASE}${path}`, options);
  if (!res.ok) {
    let message = `Error ${res.status}`;
    try {
      const body = await res.json();
      if (body?.message) message = body.message;
    } catch { /* cuerpo no JSON */ }
    throw new Error(message);
  }
  if (res.status === 204) return null;
  return res.json();
}

function normalizeMessages(data) {
  const messages = data?.messages ?? (Array.isArray(data) ? data : []);
  return messages.map((m) => ({
    author: m.senderType === 'NUTRITIONIST' ? 'nutritionist' : 'patient',
    name: m.senderType === 'NUTRITIONIST' ? 'Nutricionista' : 'Paciente',
    text: m.content,
    date: m.sentAt,
    isRead: m.isRead,
  }));
}

// GET /mongo-api/messages/{nutritionistCode}/{clientId}
export async function getThread(nutritionistCode, clientId) {
  if (!nutritionistCode || !clientId) return [];
  try {
    const data = await mongoFetch(`/messages/${nutritionistCode}/${clientId}`);
    return normalizeMessages(data);
  } catch {
    return [];
  }
}

// POST /mongo-api/messages
export async function sendMessage({ nutritionistCode, clientId, senderId, senderType, content }) {
  return mongoFetch('/messages', jsonBody('POST', { nutritionistCode, clientId, senderId, senderType, content }));
}

// PATCH /mongo-api/messages/{nutritionistCode}/{clientId}/read?readerId={readerId}
export async function markAsRead(nutritionistCode, clientId, readerId) {
  return mongoFetch(`/messages/${nutritionistCode}/${clientId}/read?readerId=${readerId}`, { method: 'PATCH' });
}

// Compatibilidad para la vista del cliente.
export async function getClientThread(nutritionistCode, clientId) {
  return getThread(nutritionistCode, clientId);
}

export async function sendClientMessage(nutritionistCode, clientId, senderId, text) {
  return sendMessage({ nutritionistCode, clientId, senderId, senderType: 'CLIENT', content: text });
}

// Compatibilidad para la vista del nutricionista.
export async function getPatientThread(nutritionistCode, clientId) {
  return getThread(nutritionistCode, clientId);
}

export async function sendPatientMessage(nutritionistCode, clientId, senderId, text) {
  return sendMessage({ nutritionistCode, clientId, senderId, senderType: 'NUTRITIONIST', content: text });
}
