import { apiFetch, jsonBody } from './api.js';

function initials(fullName) {
  return (fullName || '').split(' ').filter(Boolean).slice(0, 2).map((w) => w[0].toUpperCase()).join('');
}

function normalizeSession(data) {
  return {
    id: data.nutritionistCode ?? data.clientId ?? data.userId,
    userId: data.userId,
    clientId: data.clientId ?? null,
    nutritionistCode: data.nutritionistCode ?? null,
    age: data.age,
    email: data.email,
    name: data.fullName,
    initials: initials(data.fullName),
    accountType: data.accountType,
    activePlan: data.activePlan ?? null,
    photo: data.photo ?? null,
    message: data.message,
  };
}

// POST /api/auth/login — clientes y nutricionistas.
export async function login(credentials) {
  return normalizeSession(await apiFetch('/auth/login', jsonBody('POST', credentials)));
}

// POST /api/admin/login — administradores.
export async function loginAdmin(credentials) {
  return normalizeSession(await apiFetch('/admin/login', jsonBody('POST', credentials)));
}

// POST /api/auth/register/client
export async function registerClient(payload) {
  return normalizeSession(await apiFetch('/auth/register/client', jsonBody('POST', payload)));
}

// POST /api/auth/register/nutritionist
export async function registerNutritionist(payload) {
  return normalizeSession(await apiFetch('/auth/register/nutritionist', jsonBody('POST', payload)));
}
