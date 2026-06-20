// Manejar la retroalimentación entre cliente y nutricionista (sin endpoint: datos mock).

import CLIENT_THREAD from '../data/feedbackClient.js';
import PATIENT_THREADS from '../data/feedbackNutri.js';

// Devuelve el hilo de retroalimentación del cliente.
export async function getClientThread(clientId) {
  // MOCK_FALLBACK — sin endpoint de retroalimentación; reemplazar al exponer GET del hilo del cliente.
  return CLIENT_THREAD;
}

// Devuelve el hilo de retroalimentación de un paciente para el nutricionista.
export async function getPatientThread(patientId) {
  // MOCK_FALLBACK — sin endpoint de retroalimentación; reemplazar al exponer GET del hilo del paciente.
  return PATIENT_THREADS[patientId] ?? [];
}
