import { apiFetch, jsonBody } from './api.js';

function toInitials(fullName) {
  return (fullName || '').split(' ').filter(Boolean).slice(0, 2).map((w) => w[0].toUpperCase()).join('');
}

function normalizePatient(p) {
  return {
    id: p.clientId,
    name: p.fullName,
    email: p.email,
    age: p.age,
    country: p.country,
    initials: toInitials(p.fullName),
    hasActivePlan: p.hasActivePlan ?? false,
    calorieGoal: 2000,
    dailyLog: [],
    measurements: [],
    weight: null,
    bmi: null,
    fat: null,
    muscle: null,
  };
}

// GET /api/nutritionist/{nutritionistCode}/patients
export async function getPatients(nutritionistCode) {
  const data = await apiFetch(`/nutritionist/${nutritionistCode}/patients`);
  const list = Array.isArray(data) ? data : data.patients ?? [];
  return list.map(normalizePatient);
}

// GET /api/nutritionist/clients/search?query={query}
export async function searchClients(query) {
  if (!query || query.trim().length < 2) return [];
  const data = await apiFetch(`/nutritionist/clients/search?query=${encodeURIComponent(query.trim())}`);
  const list = Array.isArray(data) ? data : [];
  return list.map((c) => ({
    id: c.clientId,
    name: c.fullName,
    email: c.email,
    age: c.age,
    country: c.country,
    initials: toInitials(c.fullName),
  }));
}

// POST /api/nutritionist/{nutritionistCode}/patients/{clientId}
export async function associateClient(nutritionistCode, clientId) {
  return apiFetch(`/nutritionist/${nutritionistCode}/patients/${clientId}`, { method: 'POST' });
}

// DELETE /api/nutritionist/{nutritionistCode}/patients/{clientId}
export async function disassociateClient(nutritionistCode, clientId) {
  return apiFetch(`/nutritionist/${nutritionistCode}/patients/${clientId}`, { method: 'DELETE' });
}

// POST /api/nutrition-plans/{planId}/assign
export async function assignPlan(planId, { clientId, nutritionistCode, startDate, endDate }) {
  return apiFetch(`/nutrition-plans/${planId}/assign`, jsonBody('POST', {
    clientId,
    nutritionistCode,
    startDate,
    endDate: endDate ?? null,
  }));
}
