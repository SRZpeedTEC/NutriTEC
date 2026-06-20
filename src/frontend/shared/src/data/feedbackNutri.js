// Mock de la retroalimentación por paciente (sin endpoint en el backend actual: servicio mock-puro).
// Estructura: { [patientId]: mensajes[] }.

export default {
  p1: [
    { author: "nutritionist", name: "Lic. Carlos Méndez", date: "2026-05-26", text: "María, excelente avance esta semana. Mantuviste el almuerzo dentro de la meta de calorías casi todos los días. Te recomiendo aumentar un poco la proteína en la cena." },
    { author: "patient", name: "María Fernanda", date: "2026-05-27", text: "Gracias Carlos. ¿Puedo sustituir el salmón por atún cuando no lo consiga en el súper?" },
    { author: "nutritionist", name: "Lic. Carlos Méndez", date: "2026-05-28", text: "Sí, perfecto. El atún en agua es buena opción y aporta proteína similar con menos grasa. Lo dejo anotado en tu plan." },
  ],
  p2: [
    { author: "nutritionist", name: "Lic. Carlos Méndez", date: "2026-05-30", text: "Diego, noto que has saltado varias meriendas. Vamos a reforzar la merienda de la mañana para sostener tu energía en los entrenamientos." },
  ],
  p4: [
    { author: "nutritionist", name: "Lic. Carlos Méndez", date: "2026-05-31", text: "Andrés, tu avance es excelente. Sigamos así una semana más y revisamos medidas." },
    { author: "patient", name: "Andrés Vargas", date: "2026-06-01", text: "Perfecto, me siento con más energía. Gracias." },
  ],
};
