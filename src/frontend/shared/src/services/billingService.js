// Manejar el reporte de cobro de nutricionistas (sin endpoint en el backend: datos mock).

import NUTRITIONISTS from '../data/nutritionists.js';

// Devuelve los nutricionistas con su info de cobro (el monto se calcula en utils).
export async function getBillingReport() {
  // MOCK_FALLBACK — sin endpoint de cobro; reemplazar al exponer GET del reporte.
  return NUTRITIONISTS;
}
