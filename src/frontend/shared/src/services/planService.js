import { apiFetch, jsonBody } from './api.js';
import {
  MEAL_TYPE_TO_DISPLAY as BACKEND_TO_DISPLAY,
  DISPLAY_TO_MEAL_TYPE as DISPLAY_TO_BACKEND,
} from '../utils/nutrition.js';

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
  // El resumen no trae el detalle por comida; sí trae el total de calorías y el número de tiempos.
  return list.map((p) => ({
    id: p.planId,
    name: p.planName,
    author: '',
    patients: 0,
    meals: [],
    totalCalories: Number(p.totalCalories) || 0,
    mealTimeCount: p.mealTimeCount ?? 0,
  }));
}

// GET /api/nutrition-plans/{planId} — detalle completo de un plan.
export async function getPlanDetail(planId) {
  const data = await apiFetch(`/nutrition-plans/${planId}`);
  return normalizePlanDetail(data);
}

// GET /api/nutrition-plans/client/{clientId}/active — plan activo asignado al cliente.
export async function getActivePlan(clientId) {
  if (!clientId) return null;
  const data = await apiFetch(`/nutrition-plans/client/${clientId}/active`);
  if (!data) return null;
  return {
    id: data.planId,
    name: data.planName,
    nutritionistCode: data.nutritionistCode,
    nutritionist: '',
    totalGoal: Number(data.totalCalories),
    period: data.endDate
      ? `${data.startDate} – ${data.endDate}`
      : `Desde ${data.startDate}`,
    meals: (data.mealTimes ?? []).map((mt) => ({
      mealTime: BACKEND_TO_DISPLAY[mt.mealType] ?? mt.mealType,
      maxKcal: Number(mt.totalCalories),
      items: (mt.products ?? []).map((pr) => ({
        barcode: pr.productCode,
        name: pr.productName,
        portions: Number(pr.quantity),
        kcal: Math.round(Number(pr.calories) * Number(pr.quantity)),
      })),
    })),
  };
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
