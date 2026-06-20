// Manejar los planes de alimentación (lecturas mock; escrituras sin endpoint aún).

import { apiFetch, jsonBody } from './api.js';
import MEAL_PLAN from '../data/mealPlan.js';
import MEAL_PLANS from '../data/mealPlans.js';

// Devuelve el plan de alimentación activo del cliente.
export async function getActivePlan(clientId) {
  // MOCK_FALLBACK — sin endpoint de planes; reemplazar al exponer GET del plan del cliente.
  return MEAL_PLAN;
}

// Devuelve los planes creados por el nutricionista.
export async function getPlans(nutritionistId) {
  // MOCK_FALLBACK — sin endpoint de planes; reemplazar al exponer GET de planes del nutricionista.
  return MEAL_PLANS;
}

// POST /api/... — crea un plan de alimentación. (ruta TBD: confirmar al exponer el endpoint)
export async function createPlan(plan, nutritionistId) {
  return apiFetch('/plans', jsonBody('POST', { ...plan, nutritionistId }));
}

// PUT /api/... — actualiza un plan de alimentación. (ruta TBD: confirmar al exponer el endpoint)
export async function updatePlan(planId, plan) {
  return apiFetch(`/plans/${planId}`, jsonBody('PUT', plan));
}
