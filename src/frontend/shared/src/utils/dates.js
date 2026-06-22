// Manejar el parseo y formato de fechas.

// Meses y días en español para mostrar y construir el calendario.
export const MONTHS = ["enero", "febrero", "marzo", "abril", "mayo", "junio", "julio", "agosto", "septiembre", "octubre", "noviembre", "diciembre"];
export const MONTHS_SHORT = ["ene", "feb", "mar", "abr", "may", "jun", "jul", "ago", "sep", "oct", "nov", "dic"];
export const WEEKDAYS = ["Lu", "Ma", "Mi", "Ju", "Vi", "Sá", "Do"];

// Convierte una fecha ISO en un Date (o null si es inválida).
// Acepta "aaaa-mm-dd" y también ISO con hora ("aaaa-mm-ddTHH:MM:SS"): usa solo la parte de fecha.
export function parseDate(value) {
  if (!value) return null;
  const [y, m, d] = String(value).slice(0, 10).split("-").map(Number);
  if (!y || !m || !d) return null;
  return new Date(y, m - 1, d);
}

// Formatea una fecha ISO como "5 jun 2026" (o null si es inválida).
export function formatDate(value) {
  const d = parseDate(value);
  if (!d) return null;
  return `${d.getDate()} ${MONTHS_SHORT[d.getMonth()]} ${d.getFullYear()}`;
}

// Convierte un Date en su forma ISO "aaaa-mm-dd".
export function toISODate(date) {
  const y = date.getFullYear();
  const m = String(date.getMonth() + 1).padStart(2, "0");
  const d = String(date.getDate()).padStart(2, "0");
  return `${y}-${m}-${d}`;
}
