// Manejar el inicio de sesión y el registro de clientes.

import { apiFetch, jsonBody } from './api.js';

// Normaliza la sesión devuelta por el backend al formato que usa la app.
function normalizeSession(data) {
  return {
    userId: data.userId,
    clientId: data.clientId,
    age: data.age,
    email: data.email,
    name: data.fullName,
    accountType: data.accountType,
    activePlan: data.activePlan ?? null,
    message: data.message,
  };
}

// POST /api/auth/login
export async function login(credentials) {
  return normalizeSession(await apiFetch('/auth/login', jsonBody('POST', credentials)));
}

// POST /api/auth/register/client
export async function registerClient(payload) {
  return normalizeSession(await apiFetch('/auth/register/client', jsonBody('POST', payload)));
}
