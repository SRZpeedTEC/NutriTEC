// Manejar la vista del plan de alimentación asignado al cliente.

import { useState, useEffect } from 'react';
import Icon from '@nutritec/shared/components/Icon.jsx';
import Pill from '@nutritec/shared/components/Pill.jsx';
import { getActivePlan } from '@nutritec/shared/services/planService.js';
import { sumKcal } from '@nutritec/shared/utils/nutrition.js';

// Vista principal: carga el plan activo y lo muestra por tiempo de comida.
export default function PlanPage({ clientId }) {
  const [plan, setPlan] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  // Carga el plan de alimentación activo del cliente.
  useEffect(() => {
    setLoading(true); setError(null);
    getActivePlan(clientId)
      .then(setPlan)
      .catch((err) => setError(err.message))
      .finally(() => setLoading(false));
  }, [clientId]);

  if (loading) {
    return <div className="nt-content"><div className="text-center py-5"><div className="spinner-border text-teal" role="status" /></div></div>;
  }
  if (error) {
    return <div className="nt-content"><div className="alert alert-danger">{error}</div></div>;
  }
  if (!plan) {
    return <div className="nt-content"><div className="nt-empty"><Icon name="plan" size={28} /><div className="mt-2 fw-700">Aún no tienes un plan asignado</div></div></div>;
  }

  const total = plan.meals.reduce((a, m) => a + sumKcal(m.items), 0);

  return (
    <div className="nt-content">
      <div className="nt-card nt-card-pad mb-4">
        <div className="d-flex flex-wrap justify-content-between align-items-start gap-3">
          <div>
            <Pill tone="teal">Plan asignado por tu nutricionista</Pill>
            <h2 className="fw-800 mt-2 mb-1" style={{ fontSize: '1.6rem' }}>{plan.name}</h2>
            <div className="text-muted-soft">Creado por {plan.nutritionist} · {plan.nutritionistCode}</div>
            <div className="d-flex flex-wrap gap-2 mt-3">
              <span className="nt-chip"><Icon name="cal" size={15} /> {plan.period}</span>
              <span className="nt-chip"><Icon name="flame" size={15} /> Meta {plan.totalGoal} kcal/día</span>
            </div>
          </div>
          <div className="text-end">
            <div className="text-muted-soft" style={{ fontSize: '.8rem', fontWeight: 700, textTransform: 'uppercase', letterSpacing: '.04em' }}>Total del plan</div>
            <div className="fw-800 text-teal" style={{ fontSize: '2.2rem', lineHeight: 1.1 }}>{total}</div>
            <div className="text-muted-soft" style={{ fontSize: '.82rem' }}>kcal / día</div>
          </div>
        </div>
      </div>

      <div className="d-flex flex-column gap-3">
        {plan.meals.map((m) => {
          const sum = sumKcal(m.items);
          return (
            <div className="nt-meal" key={m.mealTime}>
              <div className="nt-meal-head">
                <div>
                  <div className="fw-700">{m.mealTime}</div>
                  <div className="text-muted-soft" style={{ fontSize: '.8rem' }}>Máximo {m.maxKcal} kcal</div>
                </div>
                <Pill tone="teal">{sum} kcal</Pill>
              </div>
              <div>
                {m.items.map((it, i) => (
                  <div className="nt-food-row" key={i}>
                    <div className="d-flex align-items-center gap-2">
                      <span className="text-muted-soft" style={{ fontSize: '.85rem' }}>{it.portions}×</span>
                      <span className="fw-600">{it.name}</span>
                    </div>
                    <span className="text-muted-soft">{it.kcal} kcal</span>
                  </div>
                ))}
              </div>
            </div>
          );
        })}
      </div>
    </div>
  );
}
