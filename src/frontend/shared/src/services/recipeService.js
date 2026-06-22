// Manejar la creación, consulta y edición de recetas del cliente.

import { apiFetch, jsonBody } from './api.js';

// Traduce un ingrediente de receta del backend al formato corto de la app.
function normalizeIngredient(i) {
  return {
    barcode: i.barCode,
    name: i.productName,
    portion: i.portionSize,
    unit: i.portionUnit,
    portions: i.quantity,
    energy: i.calories,
    fat: i.fat,
    sodium: i.sodium,
    carbs: i.carbohydrates,
    protein: i.protein,
    vitamins: i.vitamins,
    calcium: i.calcium,
    iron: i.iron,
  };
}

// Traduce el detalle de una receta del backend al formato de la app.
function normalizeDetail(r) {
  return {
    recipeId: r.recipeId,
    clientId: r.clientId,
    name: r.recipeName,
    totalCalories: r.totalCalories,
    totals: r.nutritionTotals,
    ingredients: (r.products ?? []).map(normalizeIngredient),
  };
}

// Traduce un resumen de receta (lista) del backend al formato de la app.
function normalizeSummary(r) {
  return {
    recipeId: r.recipeId,
    name: r.recipeName,
    totalCalories: r.totalCalories,
    ingredientCount: r.ingredientCount,
  };
}

// Convierte los ingredientes de la app al cuerpo que espera el backend.
function toIngredients(ingredients) {
  return ingredients.map((i) => ({ productCode: i.barcode, quantity: i.portions }));
}

// POST /api/recipes
export async function createRecipe({ clientId, name, ingredients }) {
  const data = await apiFetch('/recipes', jsonBody('POST', {
    clientId,
    recipeName: name,
    products: toIngredients(ingredients),
  }));
  return normalizeDetail(data.recipe ?? data);
}

// GET /api/recipes/client/{clientId}
export async function getByClient(clientId) {
  const data = await apiFetch(`/recipes/client/${clientId}`);
  return data.map(normalizeSummary);
}

// GET /api/recipes/{recipeId}?clientId={clientId}
export async function getDetail(recipeId, clientId) {
  return normalizeDetail(await apiFetch(`/recipes/${recipeId}?clientId=${clientId}`));
}

// PUT /api/recipes/{recipeId}
export async function updateRecipe(recipeId, { name, ingredients }) {
  const data = await apiFetch(`/recipes/${recipeId}`, jsonBody('PUT', {
    recipeName: name,
    products: toIngredients(ingredients),
  }));
  return normalizeDetail(data.recipe ?? data);
}

// DELETE /api/recipes/{recipeId}?clientId={clientId}
export async function deleteRecipe(recipeId, clientId) {
  return apiFetch(`/recipes/${recipeId}?clientId=${clientId}`, { method: 'DELETE' });
}

// POST /api/recipes/{recipeId}/daily-consume
export async function addToDailyConsume(recipeId, payload) {
  return apiFetch(`/recipes/${recipeId}/daily-consume`, jsonBody('POST', payload));
}
