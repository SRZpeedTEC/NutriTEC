// Mock del plan de alimentación asignado al cliente (sin endpoint en el backend actual: servicio mock-puro).

export default {
  name: "Plan Pérdida Saludable",
  nutritionist: "Lic. Carlos Méndez",
  nutritionistCode: "NUT-0421",
  period: "1 jun – 30 jun 2026",
  totalGoal: 1800,
  meals: [
    { mealTime: "Desayuno", maxKcal: 400, items: [
      { barcode: "7441001000017", name: "Avena en hojuelas", portions: 1, kcal: 150 },
      { barcode: "7441001000024", name: "Banano", portions: 1, kcal: 105 },
      { barcode: "7441001000086", name: "Yogurt natural descremado", portions: 1, kcal: 100 },
    ]},
    { mealTime: "Merienda Mañana", maxKcal: 200, items: [
      { barcode: "7441001000079", name: "Manzana", portions: 1, kcal: 78 },
      { barcode: "7441001000093", name: "Almendras", portions: 0.5, kcal: 82 },
    ]},
    { mealTime: "Almuerzo", maxKcal: 550, items: [
      { barcode: "7441001000048", name: "Pechuga de pollo a la plancha", portions: 1, kcal: 165 },
      { barcode: "7441001000055", name: "Arroz blanco cocido", portions: 1, kcal: 130 },
      { barcode: "7441001000062", name: "Frijoles negros cocidos", portions: 1, kcal: 132 },
      { barcode: "7441001000109", name: "Ensalada verde mixta", portions: 1, kcal: 35 },
    ]},
    { mealTime: "Merienda Tarde", maxKcal: 200, items: [
      { barcode: "7441001000086", name: "Yogurt natural descremado", portions: 1, kcal: 100 },
    ]},
    { mealTime: "Cena", maxKcal: 450, items: [
      { barcode: "7441001000116", name: "Salmón al horno", portions: 1, kcal: 208 },
      { barcode: "7441001000109", name: "Ensalada verde mixta", portions: 1, kcal: 35 },
      { barcode: "7441001000123", name: "Pan integral", portions: 1, kcal: 80 },
    ]},
  ],
};
