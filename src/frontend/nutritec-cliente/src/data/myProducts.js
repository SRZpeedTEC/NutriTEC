// Mock de los productos enviados por el cliente, pendientes de aprobación (fallback del servicio de productos).

export default [
  { barcode: "7441009000011", name: "Batido proteico casero", portion: 250, unit: "ml", energy: 220, fat: 4, sodium: 90, carbs: 28, protein: 20, vitamins: "B12, D", calcium: 300, iron: 1.2, status: "Pendiente" },
  { barcode: "7441009000028", name: "Galleta de avena casera", portion: 35, unit: "g", energy: 140, fat: 5, sodium: 70, carbs: 22, protein: 3, vitamins: "B1", calcium: 15, iron: 0.9, status: "Aprobado" },
];
