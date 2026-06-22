// Manejar la búsqueda de productos y el registro diario de consumo.

import { apiFetch, jsonBody } from './api.js';
import { mealTypeToDisplay } from '../utils/nutrition.js';

// Traduce un producto de búsqueda del backend al formato corto de la app.
function normalizeSearch(p) {
  return {
    barcode: p.barCode,
    name: p.productName,
    portion: p.portionSize,
    unit: p.portionUnit,
    energy: p.caloriesPerPortion,
    protein: p.proteinPerPortion,
    carbs: p.carbohydratesPerPortion,
    fat: p.fatPerPortion,
    sodium: p.sodiumPerPortion,
  };
}

// Traduce el resumen diario del backend al formato de la app (comidas como array).
function normalizeToday(data) {
  return {
    date: data.consumeDate,
    totalCalories: data.totalCalories,
    maxDailyCalories: data.maxDailyCalories,
    remainingCalories: data.remainingCalories,
    meals: (data.mealTimes ?? []).map((m) => ({
      mealTimeId: m.mealTimeId,
      mealTime: mealTypeToDisplay(m.mealType),
      kcal: m.totalCalories,
      items: (m.products ?? []).map((p) => ({
        barcode: p.barCode,
        name: p.productName,
        portions: p.quantity,
        kcal: p.calories,
      })),
    })),
  };
}

// GET /api/daily-consume/products/search?query={query}
export async function searchProducts(query) {
  const term = query.trim();
  if (!term) return [];
  const data = await apiFetch(`/daily-consume/products/search?query=${encodeURIComponent(term)}`);
  return data.map(normalizeSearch);
}

// GET /api/daily-consume/today/{clientId}
export async function getToday(clientId) {
  return normalizeToday(await apiFetch(`/daily-consume/today/${clientId}`));
}

// POST /api/daily-consume/products — el tiempo de comida se manda por tipo (BREAKFAST, LUNCH, ...).
export async function addProduct({ clientId, mealType, productCode, quantity }) {
  return apiFetch('/daily-consume/products', jsonBody('POST', { clientId, mealType, productCode, quantity }));
}

// PUT /api/daily-consume/products/{mealTimeId}/{productCode}
export async function updateProduct(mealTimeId, productCode, { clientId, quantity }) {
  return apiFetch(`/daily-consume/products/${mealTimeId}/${productCode}`, jsonBody('PUT', { clientId, quantity }));
}

// DELETE /api/daily-consume/products/{mealTimeId}/{productCode}?clientId={clientId}
export async function deleteProduct(mealTimeId, productCode, clientId) {
  return apiFetch(`/daily-consume/products/${mealTimeId}/${productCode}?clientId=${clientId}`, { method: 'DELETE' });
}
