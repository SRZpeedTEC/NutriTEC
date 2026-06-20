// Mock de los productos enviados por el nutricionista, pendientes de aprobación (fallback del servicio de productos).

export default [
  { barcode: "7441008000013", name: "Granola artesanal NutriTEC", portion: 45, unit: "g", energy: 190, fat: 7, sodium: 12, carbs: 28, protein: 5, vitamins: "E, B1", calcium: 35, iron: 1.5, status: "Pendiente" },
  { barcode: "7441008000020", name: "Crema de maní natural", portion: 32, unit: "g", energy: 188, fat: 16, sodium: 5, carbs: 7, protein: 8, vitamins: "E, B3", calcium: 20, iron: 0.6, status: "Aprobado" },
];
