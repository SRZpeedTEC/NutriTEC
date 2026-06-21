import { apiFetch, jsonBody } from './api.js';
import { MEAL_TIMES } from '../utils/nutrition.js';

// Mapeo bidireccional entre los tipos de comida del backend y los nombres de la UI.
const BACKEND_TO_DISPLAY = {
  BREAKFAST: 'Desayuno',
  SNACK: 'Merienda Mañana',
  LUNCH: 'Almuerzo',
  OTHER: 'Merienda Tarde',
  DINNER: 'Cena',
};
const DISPLAY_TO_BACKEND = Object.fromEntries(
  Object.entries(BACKEND_TO_DISPLAY).map(([k, v]) => [v, k])
);

function normalizePlanDetail(p) {
  return {
    id: p.planId,
    name: p.planName,
    author: '',
    patients: 0,
    meals: (p.mealTimes ?? []).map((mt) => ({
      mealTime: BACKEND_TO_DISPLAY[mt.mealType] ?? mt.mealType,
      maxKcal: Number(mt.totalCalories),
      items: (mt.products ?? []).map((pr) => ({
        barcode: pr.productCode,
        portions: Number(pr.quantity),
      })),
    })),
  };
}

function toApiRequest(plan, nutritionistCode) {
  return {
    planName: plan.name,
    nutritionistCode,
    mealTimes: (plan.meals ?? []).map((m) => ({
      mealType: DISPLAY_TO_BACKEND[m.mealTime] ?? m.mealTime.toUpperCase(),
      products: (m.items ?? []).map((it) => ({
        productCode: it.barcode,
        quantity: it.portions,
      })),
    })),
  };
}

// GET /api/nutrition-plans/nutritionist/{nutritionistCode} — resumen de planes.
export async function getPlans(nutritionistCode) {
  const data = await apiFetch(`/nutrition-plans/nutritionist/${nutritionistCode}`);
  const list = Array.isArray(data) ? data : [];
  return list.map((p) => ({
    id: p.planId,
    name: p.planName,
    author: '',
    patients: 0,
    meals: MEAL_TIMES.map((t) => ({ mealTime: t, maxKcal: 0, items: [] })),
    mealTimeCount: p.mealTimeCount ?? 0,
  }));
}

// GET /api/nutrition-plans/{planId} — detalle completo de un plan.
export async function getPlanDetail(planId) {
  const data = await apiFetch(`/nutrition-plans/${planId}`);
  return normalizePlanDetail(data);
}

// GET /api/nutrition-plans/{planId} — plan activo del cliente (por id de plan).
export async function getActivePlan(planId) {
  if (!planId) return null;
  return getPlanDetail(planId);
}

// POST /api/nutrition-plans
export async function createPlan(plan, nutritionistCode) {
  const data = await apiFetch('/nutrition-plans', jsonBody('POST', toApiRequest(plan, nutritionistCode)));
  return normalizePlanDetail(data.plan ?? data);
}

// PUT /api/nutrition-plans/{planId}
export async function updatePlan(planId, plan, nutritionistCode) {
  const data = await apiFetch(`/nutrition-plans/${planId}`, jsonBody('PUT', toApiRequest(plan, nutritionistCode)));
  return normalizePlanDetail(data.plan ?? data);
}

// DELETE /api/nutrition-plans/{planId}?nutritionistCode={code}
export async function deletePlan(planId, nutritionistCode) {
  return apiFetch(`/nutrition-plans/${planId}?nutritionistCode=${nutritionistCode}`, { method: 'DELETE' });
}
