// Manejar el set de iconos SVG de la interfaz.

// Trazos SVG de cada icono sobre una grilla de 24×24.
const PATHS = {
  plan: <><path d="M4 4h16v16H4z" /><path d="M8 9h8M8 13h8M8 17h5" /></>,
  plate: <><circle cx="12" cy="12" r="8.5" /><circle cx="12" cy="12" r="4" /></>,
  ruler: <><rect x="3" y="7" width="18" height="10" rx="2" /><path d="M7 7v3M11 7v3M15 7v3M19 7v3" /></>,
  box: <><path d="M21 8 12 3 3 8l9 5 9-5Z" /><path d="M3 8v8l9 5 9-5V8" /><path d="M12 13v8" /></>,
  chef: <><path d="M7 14h10v5a1 1 0 0 1-1 1H8a1 1 0 0 1-1-1v-5Z" /><path d="M7 14a4 4 0 1 1 1.5-7.7A3.5 3.5 0 0 1 15.5 6 4 4 0 1 1 17 14" /></>,
  chart: <><path d="M4 19V5" /><path d="M4 19h16" /><path d="m7 14 3-4 3 2 4-6" /></>,
  logout: <><path d="M9 21H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h4" /><path d="m16 17 5-5-5-5" /><path d="M21 12H9" /></>,
  search: <><circle cx="11" cy="11" r="7" /><path d="m21 21-4.3-4.3" /></>,
  plus: <><path d="M12 5v14M5 12h14" /></>,
  pdf: <><path d="M14 3v5h5" /><path d="M7 3h7l5 5v11a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2Z" /><path d="M9 13h1.5a1.5 1.5 0 0 1 0 3H9v-3Zm0 0v4" /></>,
  barcode: <><path d="M4 6v12M7 6v12M10 6v12M14 6v12M17 6v12M20 6v12" /></>,
  check: <><path d="m5 12 4.5 4.5L19 7" /></>,
  clock: <><circle cx="12" cy="12" r="9" /><path d="M12 7v5l3 2" /></>,
  user: <><circle cx="12" cy="8" r="4" /><path d="M4 21a8 8 0 0 1 16 0" /></>,
  chat: <><path d="M21 12a8 8 0 0 1-11.6 7.1L4 20l1-4.5A8 8 0 1 1 21 12Z" /><path d="M8.5 11h7M8.5 14h4" /></>,
  send: <><path d="M21 4 3 11l7 2 2 7 9-16Z" /><path d="M10 13 21 4" /></>,
  edit: <><path d="M14 4l6 6M4 20l1.2-4.2L16 5a2 2 0 0 1 3 3L8.2 18.8 4 20Z" /></>,
  trash: <><path d="M4 7h16M9 7V5a1 1 0 0 1 1-1h4a1 1 0 0 1 1 1v2m2 0v12a2 2 0 0 1-2 2H8a2 2 0 0 1-2-2V7" /></>,
  flame: <><path d="M12 3s5 4 5 9a5 5 0 0 1-10 0c0-2 1-3 1-3s.5 2 2 2c0-3 2-5 2-8Z" /></>,
  cal: <><rect x="3" y="5" width="18" height="16" rx="2" /><path d="M3 9h18M8 3v4M16 3v4" /></>,
  arrow: <><path d="M5 12h14M13 6l6 6-6 6" /></>,
  card: <><rect x="2.5" y="5" width="19" height="14" rx="2.5" /><path d="M2.5 9.5h19M6 15h4" /></>,
  coins: <><ellipse cx="9" cy="7" rx="6" ry="3" /><path d="M3 7v5c0 1.7 2.7 3 6 3s6-1.3 6-3V7" /><path d="M9 12c0 1.7 2.7 3 6 3s6-1.3 6-3" /><path d="M15 9.5c0 1.7 2.7 3 6 3" /></>,
  db: <><ellipse cx="12" cy="6" rx="8" ry="3" /><path d="M4 6v6c0 1.7 3.6 3 8 3s8-1.3 8-3V6" /><path d="M4 12v6c0 1.7 3.6 3 8 3s8-1.3 8-3v-6" /></>,
  info: <><circle cx="12" cy="12" r="9" /><path d="M12 11v5M12 8h.01" /></>,
  link: <><path d="M9.5 13.5a3.5 3.5 0 0 0 5 0l3-3a3.5 3.5 0 0 0-5-5l-1 1" /><path d="M14.5 10.5a3.5 3.5 0 0 0-5 0l-3 3a3.5 3.5 0 0 0 5 5l1-1" /></>,
  shield: <><path d="M12 3 5 6v5c0 4.5 3 8 7 10 4-2 7-5.5 7-10V6l-7-3Z" /><path d="m9 12 2 2 4-4" /></>,
  users: <><circle cx="9" cy="8" r="3.4" /><path d="M3 21a6 6 0 0 1 12 0" /><path d="M16 5.2a3.4 3.4 0 0 1 0 6.6" /><path d="M18 14.4A6 6 0 0 1 21.5 21" /></>,
  x: <><path d="M6 6l12 12M18 6 6 18" /></>,
};

// Dibuja el icono indicado por nombre con el tamaño dado.
export default function Icon({ name, size = 20 }) {
  const common = {
    width: size, height: size, viewBox: "0 0 24 24", fill: "none",
    stroke: "currentColor", strokeWidth: 1.8, strokeLinecap: "round", strokeLinejoin: "round",
  };
  return <svg {...common}>{PATHS[name] || null}</svg>;
}
