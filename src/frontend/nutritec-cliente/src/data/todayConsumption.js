// Mock del registro diario de consumo de hoy del cliente (fallback del servicio de consumo diario).

export default {
  date: "2026-06-01",
  meals: [
    { mealTime: "Desayuno", items: [
      { barcode: "7441001000017", name: "Avena en hojuelas", portions: 1, kcal: 150 },
      { barcode: "7441001000024", name: "Banano", portions: 1, kcal: 105 },
    ]},
    { mealTime: "Merienda Mañana", items: [
      { barcode: "7441001000079", name: "Manzana", portions: 1, kcal: 78 },
    ]},
    { mealTime: "Almuerzo", items: [
      { barcode: "7441001000048", name: "Pechuga de pollo a la plancha", portions: 1, kcal: 165 },
      { barcode: "7441001000055", name: "Arroz blanco cocido", portions: 1, kcal: 130 },
    ]},
    { mealTime: "Merienda Tarde", items: [] },
    { mealTime: "Cena", items: [] },
  ],
};
