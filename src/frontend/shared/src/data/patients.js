// Mock de los pacientes asociados al nutricionista, con su registro diario y medidas (sin endpoint: servicio mock-puro).

export default [
  {
    id: "p1", name: "María Fernanda Rojas", initials: "MR", email: "maria.rojas@example.com",
    age: 31, country: "Costa Rica", weight: 73.2, bmi: 24.1, fat: 23.9, muscle: 37.2, calorieGoal: 1800,
    planId: "pl1", period: "1 jun – 30 jun 2026", lastLog: "2026-06-01", status: "Activo",
    dailyLog: [
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
        { barcode: "7441001000062", name: "Frijoles negros cocidos", portions: 1, kcal: 132 },
      ]},
      { mealTime: "Merienda Tarde", items: [
        { barcode: "7441001000086", name: "Yogurt natural descremado", portions: 1, kcal: 100 },
      ]},
      { mealTime: "Cena", items: [
        { barcode: "7441001000116", name: "Salmón al horno", portions: 1, kcal: 208 },
      ]},
    ],
    measurements: [
      { date: "2026-03-01", waist: 86, neck: 38, hips: 102, muscle: 34.0, fat: 28.5, weight: 78.4 },
      { date: "2026-04-01", waist: 84, neck: 37.5, hips: 100, muscle: 35.0, fat: 26.9, weight: 76.5 },
      { date: "2026-05-01", waist: 82, neck: 37, hips: 98, muscle: 36.1, fat: 25.3, weight: 74.8 },
      { date: "2026-06-01", waist: 80, neck: 36.5, hips: 96, muscle: 37.2, fat: 23.9, weight: 73.2 },
    ],
  },
  {
    id: "p2", name: "Diego Alvarado Soto", initials: "DA", email: "diego.alvarado@example.com",
    age: 27, country: "Costa Rica", weight: 88.5, bmi: 27.3, fat: 26.1, muscle: 39.0, calorieGoal: 2200,
    planId: "pl2", period: "20 may – 20 jun 2026", lastLog: "2026-05-31", status: "Activo",
    dailyLog: [
      { mealTime: "Desayuno", items: [
        { barcode: "7441001000031", name: "Huevo entero", portions: 2, kcal: 156 },
        { barcode: "7441001000123", name: "Pan integral", portions: 2, kcal: 160 },
      ]},
      { mealTime: "Merienda Mañana", items: [] },
      { mealTime: "Almuerzo", items: [
        { barcode: "7441001000048", name: "Pechuga de pollo a la plancha", portions: 1.5, kcal: 248 },
        { barcode: "7441001000055", name: "Arroz blanco cocido", portions: 1.5, kcal: 195 },
      ]},
      { mealTime: "Merienda Tarde", items: [
        { barcode: "7441001000093", name: "Almendras", portions: 1, kcal: 164 },
      ]},
      { mealTime: "Cena", items: [] },
    ],
    measurements: [
      { date: "2026-04-01", waist: 96, neck: 41, hips: 106, muscle: 38.0, fat: 28.0, weight: 91.0 },
      { date: "2026-05-01", waist: 94, neck: 40.5, hips: 105, muscle: 38.6, fat: 27.0, weight: 89.7 },
      { date: "2026-05-31", waist: 92, neck: 40, hips: 104, muscle: 39.0, fat: 26.1, weight: 88.5 },
    ],
  },
  {
    id: "p3", name: "Laura Jiménez Mora", initials: "LJ", email: "laura.jimenez@example.com",
    age: 42, country: "Panamá", weight: 65.0, bmi: 22.4, fat: 21.0, muscle: 35.8, calorieGoal: 1600,
    planId: null, period: null, lastLog: "2026-05-29", status: "Sin plan",
    dailyLog: [
      { mealTime: "Desayuno", items: [
        { barcode: "7441001000086", name: "Yogurt natural descremado", portions: 1, kcal: 100 },
      ]},
      { mealTime: "Merienda Mañana", items: [] },
      { mealTime: "Almuerzo", items: [] },
      { mealTime: "Merienda Tarde", items: [] },
      { mealTime: "Cena", items: [] },
    ],
    measurements: [
      { date: "2026-05-01", waist: 74, neck: 33, hips: 95, muscle: 35.2, fat: 21.8, weight: 65.8 },
      { date: "2026-05-29", waist: 73, neck: 33, hips: 94, muscle: 35.8, fat: 21.0, weight: 65.0 },
    ],
  },
  {
    id: "p4", name: "Andrés Vargas León", initials: "AV", email: "andres.vargas@example.com",
    age: 35, country: "Costa Rica", weight: 81.0, bmi: 25.7, fat: 24.4, muscle: 38.1, calorieGoal: 2000,
    planId: "pl1", period: "5 jun – 5 jul 2026", lastLog: "2026-06-01", status: "Activo",
    dailyLog: [
      { mealTime: "Desayuno", items: [
        { barcode: "7441001000017", name: "Avena en hojuelas", portions: 1, kcal: 150 },
      ]},
      { mealTime: "Merienda Mañana", items: [
        { barcode: "7441001000079", name: "Manzana", portions: 1, kcal: 78 },
      ]},
      { mealTime: "Almuerzo", items: [
        { barcode: "7441001000147", name: "Atún en agua", portions: 1, kcal: 92 },
      ]},
      { mealTime: "Merienda Tarde", items: [] },
      { mealTime: "Cena", items: [] },
    ],
    measurements: [
      { date: "2026-04-05", waist: 90, neck: 39, hips: 101, muscle: 37.0, fat: 26.0, weight: 83.4 },
      { date: "2026-05-05", waist: 88, neck: 38.5, hips: 100, muscle: 37.6, fat: 25.1, weight: 82.1 },
      { date: "2026-06-01", waist: 86, neck: 38, hips: 99, muscle: 38.1, fat: 24.4, weight: 81.0 },
    ],
  },
];
