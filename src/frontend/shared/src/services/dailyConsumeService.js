// Manejar la búsqueda de productos y el registro diario de consumo.

import { apiFetch, jsonBody } from './api.js';
import PRODUCTS from '../data/products.js';
import TODAY from '../data/todayConsumption.js';

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
      mealTime: m.mealType,
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
  try {
    const data = await apiFetch(`/daily-consume/products/search?query=${encodeURIComponent(query)}`);
    return data.map(normalizeSearch);
  } catch {
    // MOCK_FALLBACK — quitar al conectar GET /api/daily-consume/products/search
    const needle = query.trim().toLowerCase();
    return PRODUCTS.filter((p) => p.name.toLowerCase().includes(needle));
  }
}

// GET /api/daily-consume/today/{clientId}
export async function getToday(clientId) {
  try {
    return normalizeToday(await apiFetch(`/daily-consume/today/${clientId}`));
  } catch {
    // MOCK_FALLBACK — quitar al conectar GET /api/daily-consume/today/{clientId}
    return TODAY;
  }
}

// POST /api/daily-consume/products
export async function addProduct({ clientId, mealTimeId, productCode, quantity }) {
  return apiFetch('/daily-consume/products', jsonBody('POST', { clientId, mealTimeId, productCode, quantity }));
}

// PUT /api/daily-consume/products/{mealTimeId}/{productCode}
export async function updateProduct(mealTimeId, productCode, { clientId, quantity }) {
  return apiFetch(`/daily-consume/products/${mealTimeId}/${productCode}`, jsonBody('PUT', { clientId, quantity }));
}

// DELETE /api/daily-consume/products/{mealTimeId}/{productCode}?clientId={clientId}
export async function deleteProduct(mealTimeId, productCode, clientId) {
  return apiFetch(`/daily-consume/products/${mealTimeId}/${productCode}?clientId=${clientId}`, { method: 'DELETE' });
}
