// Manejar los pacientes del nutricionista y el catálogo de clientes (sin endpoint: datos mock).

import PATIENTS from '../data/patients.js';
import CLIENTS from '../data/clients.js';

// Devuelve los pacientes asociados al nutricionista.
export async function getPatients(nutritionistId) {
  // MOCK_FALLBACK — sin endpoint de pacientes; reemplazar al exponer GET de pacientes.
  return PATIENTS;
}

// Devuelve los clientes disponibles para buscar y asociar.
export async function getClients(nutritionistId) {
  // MOCK_FALLBACK — sin endpoint de clientes; reemplazar al exponer GET de clientes.
  return CLIENTS;
}
