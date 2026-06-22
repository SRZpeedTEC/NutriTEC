// Manejar el reporte de cobro de nutricionistas contra el endpoint real del backend.

import { apiFetch } from './api.js';

// Etiquetas en español por frecuencia de cobro del backend (WEEKLY/MONTHLY/ANNUAL).
export const FREQUENCY_LABELS = { WEEKLY: 'Semanal', MONTHLY: 'Mensual', ANNUAL: 'Anual' };

// Normaliza un nutricionista del reporte a la forma que consume la vista.
// Los montos ya vienen calculados por el backend (no se recalculan en el cliente).
function normalizeNutritionist(n) {
  return {
    code: n.nutritionistCode,
    name: n.nutritionistName,
    email: n.nutritionistEmail,
    frequency: FREQUENCY_LABELS[n.billingFrequency] ?? n.billingFrequency,
    patients: n.patientCount,
    pricePerPatient: n.pricePerPatient,
    discountRate: n.discountRate,
    gross: n.totalAmountBeforeDiscount,
    discount: n.discountApplied,
    total: n.finalAmount,
  };
}

// GET /api/admin/billing-report — reporte de cobro del ciclo indicado.
// frequency es opcional (vacío o nulo => todas las frecuencias).
export async function getBillingReport({ cycleStartDate, cycleEndDate, frequency } = {}) {
  const params = new URLSearchParams({ cycleStartDate, cycleEndDate });
  if (frequency) params.set('frequency', frequency);

  const data = await apiFetch(`/admin/billing-report?${params}`);
  const list = data?.nutritionists ?? [];

  return {
    cycleStartDate: data?.cycleStartDate,
    cycleEndDate: data?.cycleEndDate,
    cycleDays: data?.cycleDays,
    totalGross: data?.totalAmountBeforeDiscount ?? 0,
    totalDiscount: data?.totalDiscountApplied ?? 0,
    total: data?.finalAmount ?? 0,
    nutritionists: list.map(normalizeNutritionist),
  };
}
