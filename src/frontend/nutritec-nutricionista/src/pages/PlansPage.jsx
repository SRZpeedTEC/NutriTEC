import { useState, useEffect } from 'react';
import Icon from '@nutritec/shared/components/Icon.jsx';
import Pill from '@nutritec/shared/components/Pill.jsx';
import SectionTitle from '@nutritec/shared/components/SectionTitle.jsx';
import { getPlans, getPlanDetail, createPlan, updatePlan } from '@nutritec/shared/services/planService.js';
import { searchProducts } from '@nutritec/shared/services/dailyConsumeService.js';
import { MEAL_TIMES, byBarcode, mealKcal, planKcal } from '@nutritec/shared/utils/nutrition.js';

const MEAL_ICONS = { 'Desayuno': 'flame', 'Merienda Mañana': 'clock', 'Almuerzo': 'plate', 'Merienda Tarde': 'clock', 'Cena': 'chef' };
const DEFAULT_MAX = { 'Desayuno': 400, 'Merienda Mañana': 200, 'Almuerzo': 550, 'Merienda Tarde': 200, 'Cena': 450 };

function PlanCard({ plan, catalog, onEdit }) {
  const total = Math.round(planKcal(plan.meals, catalog));
  const items = plan.meals.reduce((a, m) => a + m.items.length, 0);
  return (
    <div className="nt-card nt-card-pad h-100 d-flex flex-column">
      <div className="d-flex justify-content-between align-items-start mb-2">
        <div>
          <div className="fw-800" style={{ fontSize: '1.15rem' }}>{plan.name}</div>
        </div>
        <Pill tone="teal"><Icon name="flame" size={13} /> {total} kcal</Pill>
      </div>
      <div className="d-flex flex-wrap gap-2 my-2">
        {plan.meals.map((m) => (
          <span key={m.mealTime} className="nt-chip" style={{ fontSize: '.74rem', padding: '.22rem .55rem' }}>
            {m.mealTime} · {Math.round(mealKcal(m.items, catalog))}
          </span>
        ))}
      </div>
      <div className="nt-divider" style={{ margin: '.5rem 0 .85rem' }} />
      <div className="d-flex align-items-center justify-content-between mt-auto">
        <div className="d-flex gap-3 text-muted-soft" style={{ fontSize: '.82rem' }}>
          <span><Icon name="plate" size={14} /> {items} alimentos</span>
        </div>
        <button className="btn btn-soft btn-sm px-3" onClick={() => onEdit(plan)}><Icon name="edit" size={14} /> Editar</button>
      </div>
    </div>
  );
}

function MealBuilder({ meal, catalog, onAdd, onSetPortions, onRemove }) {
  const [sel, setSel] = useState(catalog[0]?.barcode);
  const total = Math.round(mealKcal(meal.items, catalog));
  const over = total > meal.maxKcal;
  return (
    <div className="nt-meal mb-3">
      <div className="nt-meal-head">
        <div className="d-flex align-items-center gap-2">
          <div className="nt-meal-ico"><Icon name={MEAL_ICONS[meal.mealTime] || 'plate'} size={16} /></div>
          <div>
            <div className="fw-700">{meal.mealTime}</div>
            <div className="text-muted-soft" style={{ fontSize: '.78rem' }}>Máx. {meal.maxKcal} kcal · {meal.items.length} alimentos</div>
          </div>
        </div>
        <Pill tone={over ? 'red' : 'teal'}>{total} / {meal.maxKcal} kcal</Pill>
      </div>
      <div className="p-3">
        {meal.items.length === 0 ? (
          <div className="text-muted-soft text-center py-2" style={{ fontSize: '.85rem' }}>Sin alimentos en este tiempo.</div>
        ) : (
          <div className="d-flex flex-column gap-1 mb-2">
            {meal.items.map((it) => {
              const p = byBarcode(catalog, it.barcode);
              if (!p) return null;
              return (
                <div key={it.barcode} className="d-flex align-items-center gap-2 py-1">
                  <span className="fw-600 flex-fill text-truncate">{p.name}</span>
                  <div className="input-group input-group-sm" style={{ width: 92 }}>
                    <input type="number" step="0.5" min="0" className="form-control" value={it.portions}
                      onChange={(e) => onSetPortions(meal.mealTime, it.barcode, e.target.value)} />
                    <span className="input-group-text">×</span>
                  </div>
                  <span className="text-muted-soft text-end" style={{ width: 64, fontSize: '.85rem' }}>{Math.round(p.energy * it.portions)} kcal</span>
                  <button className="btn btn-soft btn-sm" onClick={() => onRemove(meal.mealTime, it.barcode)}><Icon name="trash" size={14} /></button>
                </div>
              );
            })}
          </div>
        )}
        <div className="input-group input-group-sm">
          <select className="form-select" value={sel} onChange={(e) => setSel(e.target.value)}>
            {catalog.map((p) => <option key={p.barcode} value={p.barcode}>{p.name} ({p.energy} kcal)</option>)}
          </select>
          <button className="btn btn-primary px-3" onClick={() => onAdd(meal.mealTime, sel)} disabled={!sel}><Icon name="plus" size={14} /> Agregar</button>
        </div>
      </div>
    </div>
  );
}

function PlanEditor({ base, nutritionistId, catalog, onClose, onSave }) {
  const [name, setName] = useState(base ? base.name : '');
  const [meals, setMeals] = useState(() =>
    base
      ? JSON.parse(JSON.stringify(base.meals))
      : MEAL_TIMES.map((t) => ({ mealTime: t, maxKcal: DEFAULT_MAX[t], items: [] }))
  );

  const setMeal = (mealTime, fn) => setMeals((prev) => prev.map((m) => m.mealTime === mealTime ? fn(m) : m));
  const onAdd = (mealTime, bc) => setMeal(mealTime, (m) => m.items.some((i) => i.barcode === bc) ? m : { ...m, items: [...m.items, { barcode: bc, portions: 1 }] });
  const onSetPortions = (mealTime, bc, v) => setMeal(mealTime, (m) => ({ ...m, items: m.items.map((i) => i.barcode === bc ? { ...i, portions: parseFloat(v) || 0 } : i) }));
  const onRemove = (mealTime, bc) => setMeal(mealTime, (m) => ({ ...m, items: m.items.filter((i) => i.barcode !== bc) }));

  const total = Math.round(planKcal(meals, catalog));
  const totalMax = meals.reduce((a, m) => a + m.maxKcal, 0);

  return (
    <div className="nt-content">
      <button className="btn btn-soft btn-sm mb-3" onClick={onClose}>‹ Volver a planes</button>
      <div className="row g-4">
        <div className="col-lg-8">
          <div className="nt-card nt-card-pad mb-3">
            <label className="form-label">Nombre del plan</label>
            <input className="form-control form-control-lg" value={name} onChange={(e) => setName(e.target.value)} placeholder="Ej. Plan Pérdida Saludable" />
          </div>
          {meals.map((m) => (
            <MealBuilder key={m.mealTime} meal={m} catalog={catalog} onAdd={onAdd} onSetPortions={onSetPortions} onRemove={onRemove} />
          ))}
        </div>

        <div className="col-lg-4">
          <div style={{ position: 'sticky', top: 90 }}>
            <div className="nt-card nt-card-pad mb-3" style={{ background: 'var(--nt-teal-50)' }}>
              <div className="text-center mb-3">
                <div className="text-muted-soft fw-700 text-uppercase" style={{ fontSize: '.74rem', letterSpacing: '.05em' }}>Total del plan</div>
                <div className="fw-800 text-teal" style={{ fontSize: '2.4rem', lineHeight: 1 }}>{total}</div>
                <div className="text-muted-soft fw-700">Kcal totales por día</div>
              </div>
              <div className="d-flex flex-column gap-2">
                {meals.map((m) => {
                  const k = Math.round(mealKcal(m.items, catalog));
                  const pct = totalMax ? (k / totalMax) * 100 : 0;
                  return (
                    <div key={m.mealTime}>
                      <div className="d-flex justify-content-between" style={{ fontSize: '.82rem' }}>
                        <span className="fw-600">{m.mealTime}</span>
                        <span className="text-muted-soft">{k} kcal</span>
                      </div>
                      <div className="progress mt-1" style={{ height: 7 }}>
                        <div className="progress-bar" style={{ width: Math.min(100, pct) + '%' }} />
                      </div>
                    </div>
                  );
                })}
              </div>
              <div className="nt-divider" style={{ margin: '.9rem 0' }} />
              <div className="d-flex justify-content-between fw-700">
                <span>Meta diaria sugerida</span><span className="text-teal">{totalMax} kcal</span>
              </div>
            </div>
            <button className="btn btn-primary w-100 btn-lg" onClick={() => onSave({ name, meals })} disabled={!name.trim()}>
              <Icon name="check" size={18} /> Guardar plan
            </button>
          </div>
        </div>
      </div>
    </div>
  );
}

export default function PlansPage({ nutritionistId }) {
  const [plans, setPlans] = useState([]);
  const [catalog, setCatalog] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [editing, setEditing] = useState(null);
  const [toast, setToast] = useState(null);

  useEffect(() => {
    setLoading(true); setError(null);
    Promise.all([getPlans(nutritionistId), searchProducts('')])
      .then(([pls, cat]) => { setPlans(pls); setCatalog(cat); })
      .catch((err) => setError(err.message))
      .finally(() => setLoading(false));
  }, [nutritionistId]);

  function flash(message) {
    setToast(message);
    setTimeout(() => setToast(null), 2600);
  }

  async function openEdit(plan) {
    try {
      const detail = await getPlanDetail(plan.id);
      setEditing(detail);
    } catch {
      setEditing(plan);
    }
  }

  async function save({ name, meals }) {
    const target = editing;
    setEditing(null);
    try {
      if (target === 'new') await createPlan({ name, meals }, nutritionistId);
      else await updatePlan(target.id, { name, meals }, nutritionistId);
      setPlans(await getPlans(nutritionistId));
      flash('Plan guardado correctamente');
    } catch (err) {
      flash(err.message || 'No se pudo guardar el plan');
    }
  }

  if (loading) {
    return <div className="nt-content"><div className="text-center py-5"><div className="spinner-border text-teal" role="status" /></div></div>;
  }
  if (error) {
    return <div className="nt-content"><div className="alert alert-danger">{error}</div></div>;
  }

  if (editing) {
    return <PlanEditor base={editing === 'new' ? null : editing} nutritionistId={nutritionistId} catalog={catalog} onClose={() => setEditing(null)} onSave={save} />;
  }

  return (
    <div className="nt-content">
      <div className="d-flex justify-content-between align-items-center mb-3">
        <SectionTitle sub="Cada plan se compone de 5 tiempos de comida y totaliza las calorías">Planes de alimentación</SectionTitle>
        <button className="btn btn-primary" onClick={() => setEditing('new')}><Icon name="plus" size={16} /> Nuevo plan</button>
      </div>
      <div className="row g-3">
        {plans.length === 0
          ? <div className="nt-empty"><Icon name="plate" size={26} /><div className="mt-2 fw-700">No tienes planes creados</div></div>
          : plans.map((p) => (
            <div className="col-md-6" key={p.id}>
              <PlanCard plan={p} catalog={catalog} onEdit={openEdit} />
            </div>
          ))}
      </div>
      {toast && <div className="nt-toast"><Icon name="check" size={16} /> {toast}</div>}
    </div>
  );
}
