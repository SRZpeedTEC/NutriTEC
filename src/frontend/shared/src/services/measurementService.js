// Manejar el registro y la consulta de medidas corporales.

import { apiFetch, jsonBody } from './api.js';
import MEASUREMENTS from '../data/measurements.js';

// Traduce una medida del backend al formato corto de la app.
function normalizeMeasurement(m) {
  return {
    clientId: m.clientId,
    date: m.measurementDate,
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
  try {
    const data = await apiFetch(`/measurements/client/${clientId}`);
    return data.map(normalizeMeasurement);
  } catch {
    // MOCK_FALLBACK — quitar al conectar GET /api/measurements/client/{clientId}
    return MEASUREMENTS;
  }
}

// GET /api/measurements/report/{clientId}?startDate={startDate}&endDate={endDate}
export async function getReport(clientId, startDate, endDate) {
  try {
    const data = await apiFetch(`/measurements/report/${clientId}?startDate=${startDate}&endDate=${endDate}`);
    return {
      clientId: data.clientId,
      startDate: data.startDate,
      endDate: data.endDate,
      recordCount: data.recordCount,
      measurements: (data.measurements ?? []).map(normalizeMeasurement),
    };
  } catch {
    // MOCK_FALLBACK — quitar al conectar GET /api/measurements/report/{clientId}
    const measurements = MEASUREMENTS.filter((m) => m.date >= startDate && m.date <= endDate);
    return { clientId, startDate, endDate, recordCount: measurements.length, measurements };
  }
}
