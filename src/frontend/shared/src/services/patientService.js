// Manejar los pacientes del nutricionista y el catálogo de clientes (lecturas mock; escrituras sin endpoint aún).

import { apiFetch, jsonBody } from './api.js';
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

// POST /api/... — asocia un cliente como paciente del nutricionista. (ruta TBD: confirmar al exponer el endpoint)
export async function associateClient(nutritionistId, clientId) {
  return apiFetch('/patients', jsonBody('POST', { nutritionistId, clientId }));
}

// PUT /api/... — asigna/actualiza el plan de un paciente en un periodo. (ruta TBD: confirmar al exponer el endpoint)
export async function assignPlan(patientId, { planId, startDate, endDate }) {
  return apiFetch(`/patients/${patientId}/plan`, jsonBody('PUT', { planId, startDate, endDate }));
}
