// Manejar la retroalimentación entre cliente y nutricionista (lecturas mock; envío sin endpoint aún).

import { apiFetch, jsonBody } from './api.js';
import CLIENT_THREAD from '../data/feedbackClient.js';
import PATIENT_THREADS from '../data/feedbackNutri.js';

// Devuelve el hilo de retroalimentación del cliente.
export async function getClientThread(clientId) {
  // MOCK_FALLBACK — sin endpoint de retroalimentación; reemplazar al exponer GET del hilo del cliente.
  return CLIENT_THREAD;
}

// POST /api/... — envía un mensaje de retroalimentación al nutricionista. (ruta TBD: confirmar al exponer el endpoint)
export async function sendClientMessage(clientId, text) {
  return apiFetch(`/feedback/client/${clientId}`, jsonBody('POST', { text }));
}

// Devuelve el hilo de retroalimentación de un paciente para el nutricionista.
export async function getPatientThread(patientId) {
  // MOCK_FALLBACK — sin endpoint de retroalimentación; reemplazar al exponer GET del hilo del paciente.
  return PATIENT_THREADS[patientId] ?? [];
}

// POST /api/... — envía un mensaje de retroalimentación a un paciente. (ruta TBD: confirmar al exponer el endpoint)
export async function sendPatientMessage(patientId, text) {
  return apiFetch(`/feedback/${patientId}`, jsonBody('POST', { text }));
}
