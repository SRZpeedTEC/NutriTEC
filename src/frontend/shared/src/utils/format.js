// Manejar el formato de montos.

// Formatea un número como monto en dólares: "$48.00".
export function fmtUSD(n) {
  return "$" + n.toFixed(2);
}
