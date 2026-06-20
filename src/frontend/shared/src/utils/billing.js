// Manejar el cálculo del cobro de los nutricionistas.

// Reglas de cobro por tipo de pago: tarifa por paciente y descuento aplicable.
export const BILLING_RULES = {
  Semanal: { rate: 1, discount: 0, detail: "$1 por paciente / semana" },
  Mensual: { rate: 4, discount: 0.05, detail: "$4 por paciente · 5% de descuento" },
  Anual: { rate: 52, discount: 0.10, detail: "$52 por paciente · 10% de descuento" },
};

// Calcula el cobro de un nutricionista según su tipo de pago y número de pacientes.
export function calcBilling(nutritionist) {
  const rule = BILLING_RULES[nutritionist.billingType];
  const gross = nutritionist.patients * rule.rate;
  const discount = gross * rule.discount;
  return { gross, discount, total: gross - discount, rule };
}
