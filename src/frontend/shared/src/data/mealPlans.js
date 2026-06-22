// Mock de los planes de alimentación creados por el nutricionista (sin endpoint en el backend actual: servicio mock-puro).

export default [
  {
    id: "pl1", name: "Plan Pérdida Saludable", author: "Lic. Carlos Méndez Quirós", patients: 2,
    meals: [
      { mealTime: "Desayuno", maxKcal: 400, items: [
        { barcode: "7441001000017", portions: 1 },
        { barcode: "7441001000024", portions: 1 },
        { barcode: "7441001000086", portions: 1 },
      ]},
      { mealTime: "Merienda Mañana", maxKcal: 200, items: [
        { barcode: "7441001000079", portions: 1 },
        { barcode: "7441001000093", portions: 0.5 },
      ]},
      { mealTime: "Almuerzo", maxKcal: 550, items: [
        { barcode: "7441001000048", portions: 1 },
        { barcode: "7441001000055", portions: 1 },
        { barcode: "7441001000062", portions: 1 },
        { barcode: "7441001000109", portions: 1 },
      ]},
      { mealTime: "Merienda Tarde", maxKcal: 200, items: [
        { barcode: "7441001000086", portions: 1 },
      ]},
      { mealTime: "Cena", maxKcal: 450, items: [
        { barcode: "7441001000116", portions: 1 },
        { barcode: "7441001000109", portions: 1 },
        { barcode: "7441001000123", portions: 1 },
      ]},
    ],
  },
  {
    id: "pl2", name: "Plan Ganancia Muscular", author: "Lic. Carlos Méndez Quirós", patients: 1,
    meals: [
      { mealTime: "Desayuno", maxKcal: 550, items: [
        { barcode: "7441001000031", portions: 2 },
        { barcode: "7441001000123", portions: 2 },
        { barcode: "7441001000024", portions: 1 },
      ]},
      { mealTime: "Merienda Mañana", maxKcal: 250, items: [
        { barcode: "7441001000093", portions: 1 },
      ]},
      { mealTime: "Almuerzo", maxKcal: 650, items: [
        { barcode: "7441001000048", portions: 1.5 },
        { barcode: "7441001000055", portions: 1.5 },
        { barcode: "7441001000062", portions: 1 },
      ]},
      { mealTime: "Merienda Tarde", maxKcal: 250, items: [
        { barcode: "7441001000086", portions: 1 },
        { barcode: "7441001000079", portions: 1 },
      ]},
      { mealTime: "Cena", maxKcal: 500, items: [
        { barcode: "7441001000116", portions: 1 },
        { barcode: "7441001000055", portions: 1 },
      ]},
    ],
  },
];
