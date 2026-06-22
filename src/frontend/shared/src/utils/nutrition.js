// Manejar los cálculos nutricionales del plan y el consumo.

// Tiempos de comida en orden, comunes a todas las vistas.
export const MEAL_TIMES = ["Desayuno", "Merienda Mañana", "Almuerzo", "Merienda Tarde", "Cena"];

// Mapeo de los tipos de comida del backend (enum en inglés) a los nombres mostrados en la UI.
export const MEAL_TYPE_TO_DISPLAY = {
  BREAKFAST: "Desayuno",
  SNACK: "Merienda Mañana",
  LUNCH: "Almuerzo",
  OTHER: "Merienda Tarde",
  DINNER: "Cena",
};

// Mapeo inverso: del nombre de la UI al enum del backend.
export const DISPLAY_TO_MEAL_TYPE = Object.fromEntries(
  Object.entries(MEAL_TYPE_TO_DISPLAY).map(([k, v]) => [v, k])
);

// Traduce un tipo de comida del backend al nombre de la UI (deja pasar lo desconocido).
export function mealTypeToDisplay(mealType) {
  return MEAL_TYPE_TO_DISPLAY[mealType] ?? mealType;
}

// Busca un producto del catálogo por su código de barras.
export function byBarcode(catalog, code) {
  return catalog.find((p) => p.barcode === code);
}

// Busca un elemento de una lista por su id.
export function byId(list, id) {
  return list.find((x) => x.id === id);
}

// Suma los macronutrientes de los items resolviendo cada barcode en el catálogo.
export function totals(items, catalog) {
  return items.reduce((acc, it) => {
    const p = byBarcode(catalog, it.barcode);
    if (!p) return acc;
    const f = it.portions || 1;
    acc.energy += p.energy * f;
    acc.fat += p.fat * f;
    acc.sodium += p.sodium * f;
    acc.carbs += p.carbs * f;
    acc.protein += p.protein * f;
    acc.calcium += p.calcium * f;
    acc.iron += p.iron * f;
    return acc;
  }, { energy: 0, fat: 0, sodium: 0, carbs: 0, protein: 0, calcium: 0, iron: 0 });
}

// Calcula las kcal de una comida a partir del catálogo.
export function mealKcal(items, catalog) {
  return totals(items, catalog).energy;
}

// Calcula las kcal totales de un plan sumando sus comidas.
export function planKcal(meals, catalog) {
  return meals.reduce((acc, meal) => acc + mealKcal(meal.items, catalog), 0);
}

// Suma las kcal de items que ya las traen (registro diario).
export function sumKcal(items) {
  return items.reduce((acc, it) => acc + (it.kcal || 0), 0);
}
