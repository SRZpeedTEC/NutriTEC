// Manejar el envío, la consulta y la moderación de productos.

import { apiFetch, jsonBody } from './api.js';
import PENDING_PRODUCTS from '../data/pendingProducts.js';

// Traduce un producto del backend al formato corto que usa la app.
function normalizeProduct(p) {
  return {
    barcode: p.barCode,
    name: p.productName,
    portion: p.portionSize,
    unit: p.portionUnit,
    energy: p.calories,
    fat: p.fat,
    sodium: p.sodium,
    carbs: p.carbohydrates,
    protein: p.protein,
    vitamins: p.vitamins,
    calcium: p.calcium,
    iron: p.iron,
    status: p.productStatus,
    userId: p.userId,
  };
}

// Convierte un producto de la app al cuerpo que espera el backend.
function toProductRequest(product, userId) {
  return {
    barCode: product.barcode,
    productName: product.name,
    portionUnit: product.unit,
    portionSize: product.portion,
    calories: product.energy,
    fat: product.fat,
    sodium: product.sodium,
    carbohydrates: product.carbs,
    protein: product.protein,
    vitamins: product.vitamins,
    calcium: product.calcium,
    iron: product.iron,
    userId,
  };
}

// POST /api/product
export async function createProduct(product, userId) {
  const data = await apiFetch('/product', jsonBody('POST', toProductRequest(product, userId)));
  return normalizeProduct(data.product ?? data);
}

// GET /api/product/pending/{userId}
export async function getPendingProducts(userId) {
  try {
    const data = await apiFetch(`/product/pending/${userId}`);
    const list = Array.isArray(data) ? data : data.products ?? [];
    return list.map(normalizeProduct);
  } catch {
    // MOCK_FALLBACK — quitar al conectar GET /api/product/pending/{userId}
    return PENDING_PRODUCTS;
  }
}

// PUT /api/product/{barCode}
export async function updateProduct(barCode, product, userId) {
  const data = await apiFetch(`/product/${barCode}`, jsonBody('PUT', toProductRequest(product, userId)));
  return normalizeProduct(data.product ?? data);
}

// PUT /api/product/{barCode} — moderación del admin: aprueba o rechaza un producto.
// Sin fallback (escritura): hoy falla sin backend y funcionará solo al exponer el endpoint.
// El cuerpo/ruta exactos de moderación se confirman cuando el endpoint esté disponible.
export async function setProductStatus(barCode, status, userId, reason) {
  const data = await apiFetch(`/product/${barCode}`, jsonBody('PUT', {
    barCode,
    productStatus: status,
    rejectionReason: reason,
    userId,
  }));
  return normalizeProduct(data.product ?? data);
}

// DELETE /api/product/{barCode}?userId={userId}
export async function deleteProduct(barCode, userId) {
  return apiFetch(`/product/${barCode}?userId=${userId}`, { method: 'DELETE' });
}
