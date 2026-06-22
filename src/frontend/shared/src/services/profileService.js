// Manejar los perfiles de cada rol (sin endpoint en el backend: datos mock).

import CLIENT_PROFILE from '../data/clientProfile.js';
import NUTRITIONIST_PROFILE from '../data/nutritionistProfile.js';
import ADMIN_PROFILE from '../data/adminProfile.js';

// Devuelve el perfil del cliente autenticado.
export async function getClientProfile(clientId) {
  // MOCK_FALLBACK — sin endpoint de perfil; reemplazar al exponer GET del perfil del cliente.
  return CLIENT_PROFILE;
}

// Devuelve el perfil del nutricionista autenticado.
export async function getNutritionistProfile(nutritionistId) {
  // MOCK_FALLBACK — sin endpoint de perfil; reemplazar al exponer GET del perfil del nutricionista.
  return NUTRITIONIST_PROFILE;
}

// Devuelve el perfil del administrador autenticado.
export async function getAdminProfile() {
  // MOCK_FALLBACK — sin endpoint de perfil; reemplazar al exponer GET del perfil del admin.
  return ADMIN_PROFILE;
}
