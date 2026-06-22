// Manejar el registro y la consulta de medidas corporales.

import { apiFetch, jsonBody } from './api.js';

// Traduce una medida del backend al formato corto de la app.
function normalizeMeasurement(m) {
  return {
    clientId: m.clientId,
    // El backend manda la fecha como DateTime ("...T00:00:00"); se conserva solo "aaaa-mm-dd".
    date: m.measurementDate ? String(m.measurementDate).slice(0, 10) : m.measurementDate,
    weight: m.bodyWeight,
    bmi: m.bodyMassIndex,
    waist: m.waist,
    neck: m.neck,
    hips: m.hip,
    muscle: m.musclePercentage,
    fat: m.fatPercentage,
  };
}

// Convierte una medida de la app al cuerpo que espera el backend.
function toMeasurementRequest(m, clientId) {
  return {
    clientId,
    measurementDate: m.date,
    bodyWeight: m.weight,
    bodyMassIndex: m.bmi,
    waist: m.waist,
    neck: m.neck,
    hip: m.hips,
    musclePercentage: m.muscle,
    fatPercentage: m.fat,
  };
}

// POST /api/measurements
export async function createMeasurement(measurement, clientId) {
  const data = await apiFetch('/measurements', jsonBody('POST', toMeasurementRequest(measurement, clientId)));
  return normalizeMeasurement(data.measurement ?? data);
}

// PUT /api/measurements/{clientId}/{date}
export async function updateMeasurement(clientId, date, measurement) {
  const data = await apiFetch(`/measurements/${clientId}/${date}`, jsonBody('PUT', toMeasurementRequest(measurement, clientId)));
  return normalizeMeasurement(data.measurement ?? data);
}

// GET /api/measurements/client/{clientId}
export async function getHistory(clientId) {
  const data = await apiFetch(`/measurements/client/${clientId}`);
  return data.map(normalizeMeasurement);
}

// GET /api/measurements/report/{clientId}?startDate={startDate}&endDate={endDate}
export async function getReport(clientId, startDate, endDate) {
  const data = await apiFetch(`/measurements/report/${clientId}?startDate=${startDate}&endDate=${endDate}`);
  return {
    clientId: data.clientId,
    startDate: data.startDate,
    endDate: data.endDate,
    recordCount: data.recordCount,
    measurements: (data.measurements ?? []).map(normalizeMeasurement),
  };
}
