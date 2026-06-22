import { apiFetch, jsonBody } from './api.js';

const STATUS_BACKEND_TO_DISPLAY = {
  PENDING_REVIEW: 'Pendiente',
  ACTIVE: 'Aprobado',
  REJECTED: 'Rechazado',
};

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
    status: STATUS_BACKEND_TO_DISPLAY[p.productStatus] ?? p.productStatus,
    userId: p.userId,
  };
}

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

// GET /api/product/pending/{userId} — productos enviados por el usuario.
export async function getPendingProducts(userId) {
  const data = await apiFetch(`/product/pending/${userId}`);
  const list = Array.isArray(data) ? data : data.products ?? [];
  return list.map(normalizeProduct);
}

// GET /api/admin/products?status={status} — todos los productos para el administrador.
export async function getAdminProducts(status) {
  const qs = status && status !== 'Todos' ? `?status=${encodeURIComponent(status)}` : '';
  const data = await apiFetch(`/admin/products${qs}`);
  const list = Array.isArray(data) ? data : data.products ?? [];
  return list.map(normalizeProduct);
}

// PUT /api/product/{barCode}
export async function updateProduct(barCode, product, userId) {
  const data = await apiFetch(`/product/${barCode}`, jsonBody('PUT', toProductRequest(product, userId)));
  return normalizeProduct(data.product ?? data);
}

// PUT /api/admin/products/{barCode}/approve?adminId={adminId}
export async function approveProduct(barCode, adminId) {
  return apiFetch(`/admin/products/${barCode}/approve?adminId=${adminId}`, { method: 'PUT' });
}

// PUT /api/admin/products/{barCode}/reject?adminId={adminId}
export async function rejectProduct(barCode, adminId) {
  return apiFetch(`/admin/products/${barCode}/reject?adminId=${adminId}`, { method: 'PUT' });
}

// DELETE /api/product/{barCode}?userId={userId}
export async function deleteProduct(barCode, userId) {
  return apiFetch(`/product/${barCode}?userId=${userId}`, { method: 'DELETE' });
}
