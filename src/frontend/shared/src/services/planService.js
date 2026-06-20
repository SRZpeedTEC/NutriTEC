// Manejar los planes de alimentación (sin endpoint en el backend: datos mock).

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
