// Cliente HTTP base: arma la URL del API y centraliza el manejo de errores y el parseo JSON.

// En Vite import.meta.env trae las variables; el "?? {}" deja el módulo usable también fuera del bundler.
const env = import.meta.env ?? {};

// URL base del API. En dev vale "/api" y el proxy de Vite la redirige al backend.
export const BASE_URL = env.VITE_API_BASE_URL ?? '/api';

// Hace la petición y devuelve el JSON; lanza Error con el mensaje del servidor si el status no es 2xx.
export async function apiFetch(path, options) {
  const res = await fetch(`${BASE_URL}${path}`, options);

  if (!res.ok) {
    let message = `Error ${res.status}`;
    try {
      const body = await res.json();
      if (body?.message) message = body.message;
    } catch {
      // El cuerpo no era JSON: se mantiene el mensaje genérico.
    }
    throw new Error(message);
  }

  return res.json();
}

// Arma las opciones de una petición con cuerpo JSON (POST/PUT).
export function jsonBody(method, payload) {
  return {
    method,
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(payload),
  };
}
