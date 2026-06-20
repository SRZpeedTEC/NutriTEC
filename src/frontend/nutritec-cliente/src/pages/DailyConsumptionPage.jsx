// Manejar el registro diario de consumo: búsqueda de alimentos y su registro por tiempo de comida.

import { useState, useEffect } from 'react';
import Icon from '@nutritec/shared/components/Icon.jsx';
import Pill from '@nutritec/shared/components/Pill.jsx';
import SectionTitle from '@nutritec/shared/components/SectionTitle.jsx';
import { getToday, searchProducts, addProduct, deleteProduct } from '@nutritec/shared/services/dailyConsumeService.js';
import { getActivePlan } from '@nutritec/shared/services/planService.js';
import { MEAL_TIMES, sumKcal } from '@nutritec/shared/utils/nutrition.js';
import { formatDate } from '@nutritec/shared/utils/dates.js';

// Vista principal: carga el consumo de hoy y el plan, y registra alimentos por tiempo de comida.
export default function DailyConsumptionPage({ clientId }) {
  const [today, setToday] = useState(null);
  const [plan, setPlan] = useState(null);
  const [catalog, setCatalog] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  const [active, setActive] = useState('Desayuno');
  const [q, setQ] = useState('');
  const [toast, setToast] = useState(null);

  // Carga el consumo de hoy, el plan y el catálogo de productos.
  useEffect(() => {
    setLoading(true); setError(null);
    Promise.all([getToday(clientId), getActivePlan(clientId), searchProducts('')])
      .then(([td, pl, cat]) => { setToday(td); setPlan(pl); setCatalog(cat); })
      .catch((err) => setError(err.message))
      .finally(() => setLoading(false));
  }, [clientId]);

  // Muestra un aviso flotante temporal.
  function flash(message, warn) {
    setToast({ message, warn });
    setTimeout(() => setToast(null), 2600);
  }

  // Recarga el consumo de hoy tras un cambio.
  async function reload() {
    setToday(await getToday(clientId));
  }

  // Agrega un producto al tiempo de comida activo.
  async function add(product) {
    const meal = today.meals.find((m) => m.mealTime === active);
    try {
      await addProduct({ clientId, mealTimeId: meal?.mealTimeId, productCode: product.barcode, quantity: 1 });
      setQ('');
      await reload();
    } catch (err) {
      flash(err.message || 'No se pudo registrar el alimento', true);
    }
  }

  // Quita un producto del tiempo de comida activo.
  async function remove(item) {
    const meal = today.meals.find((m) => m.mealTime === active);
    try {
      await deleteProduct(meal?.mealTimeId, item.barcode, clientId);
      await reload();
    } catch (err) {
      flash(err.message || 'No se pudo quitar el alimento', true);
    }
  }

  if (loading) {
    return <div className="nt-content"><div className="text-center py-5"><div className="spinner-border text-teal" role="status" /></div></div>;
  }
  if (error) {
    return <div className="nt-content"><div className="alert alert-danger">{error}</div></div>;
  }

  const goal = plan?.totalGoal ?? 0;
  const mealItems = (mt) => today.meals.find((m) => m.mealTime === mt)?.items ?? [];
  const activeItems = mealItems(active);
  const activeTotal = sumKcal(activeItems);
  const dayTotal = today.meals.reduce((a, m) => a + sumKcal(m.items), 0);
  const maxKcal = plan?.meals.find((m) => m.mealTime === active)?.maxKcal ?? 0;

  const results = catalog.filter((p) => {
    const t = q.trim().toLowerCase();
    if (!t) return false;
    return p.name.toLowerCase().includes(t) || p.barcode.includes(t);
  });

  return (
    <div className="nt-content">
      <div className="d-flex flex-wrap gap-2 align-items-center justify-content-between mb-3">
        <div className="nt-chip"><Icon name="cal" size={15} /> Hoy · {formatDate(today.date)}</div>
        <Pill tone={dayTotal > goal ? 'gray' : 'teal'}>{dayTotal} / {goal} kcal del día</Pill>
      </div>

      <div className="d-flex flex-wrap gap-2 mb-4">
        {MEAL_TIMES.map((t) => {
          const sum = sumKcal(mealItems(t));
          return (
            <button key={t} className={'btn ' + (active === t ? 'btn-primary' : 'btn-soft')} onClick={() => setActive(t)}>
              {t}
              <span className="ms-2 badge rounded-pill" style={{ background: active === t ? 'rgba(255,255,255,.2)' : 'var(--nt-teal-100)', color: active === t ? '#fff' : 'var(--nt-teal-700)' }}>{sum}</span>
            </button>
          );
        })}
      </div>

      <div className="row g-4">
        <div className="col-lg-5">
          <div className="nt-card nt-card-pad">
            <SectionTitle sub="Por nombre o código de barras">Buscar alimento</SectionTitle>
            <div className="input-group input-group-lg mb-3">
              <span className="input-group-text"><Icon name="search" size={18} /></span>
              <input className="form-control" placeholder="Ej. avena o 7441001000017" value={q} onChange={(e) => setQ(e.target.value)} />
            </div>
            {q.trim() === '' && (
              <div className="nt-empty">
                <Icon name="barcode" size={26} />
                <div className="mt-2 fw-700">Escribe para buscar</div>
                <div style={{ fontSize: '.88rem' }}>Encuentra alimentos aprobados y agrégalos a <strong>{active}</strong>.</div>
              </div>
            )}
            <div className="d-flex flex-column gap-2">
              {results.map((p) => (
                <div key={p.barcode} className="d-flex align-items-center justify-content-between p-2 rounded-3" style={{ border: '1px solid var(--nt-line)' }}>
                  <div className="min-w-0">
                    <div className="fw-700 text-truncate">{p.name}</div>
                    <div className="text-muted-soft" style={{ fontSize: '.8rem' }}>{p.portion}{p.unit} · {p.energy} kcal · {p.barcode}</div>
                  </div>
                  <button className="btn btn-primary btn-sm px-3" onClick={() => add(p)}><Icon name="plus" size={15} /></button>
                </div>
              ))}
              {q.trim() !== '' && results.length === 0 && (
                <div className="nt-empty">No se encontró «{q}».<br /><span style={{ fontSize: '.85rem' }}>Puedes crearlo en <strong>Productos</strong> o como <strong>Receta</strong>.</span></div>
              )}
            </div>
          </div>
        </div>

        <div className="col-lg-7">
          <div className="nt-card nt-card-pad">
            <div className="d-flex justify-content-between align-items-start mb-3">
              <SectionTitle sub={`Máximo del plan: ${maxKcal} kcal`}>{active}</SectionTitle>
              <Pill tone={activeTotal > maxKcal ? 'gray' : 'teal'}>{activeTotal} / {maxKcal} kcal</Pill>
            </div>
            <div className="progress mb-3">
              <div className={'progress-bar' + (activeTotal > maxKcal ? ' over' : '')} style={{ width: Math.min(100, maxKcal ? (activeTotal / maxKcal) * 100 : 0) + '%' }} />
            </div>

            {activeItems.length === 0 ? (
              <div className="nt-empty">Aún no has registrado alimentos en {active}.</div>
            ) : (
              <table className="table align-middle mb-0">
                <thead><tr><th>Alimento</th><th style={{ width: 90 }}>Porciones</th><th style={{ width: 80 }}>Kcal</th><th style={{ width: 48 }} /></tr></thead>
                <tbody>
                  {activeItems.map((it, i) => (
                    <tr key={i}>
                      <td><span className="fw-700">{it.name}</span><div className="text-muted-soft" style={{ fontSize: '.78rem' }}>{it.barcode}</div></td>
                      <td>{it.portions}×</td>
                      <td className="fw-700">{it.kcal}</td>
                      <td><button className="btn btn-soft btn-sm" onClick={() => remove(it)}><Icon name="trash" size={15} /></button></td>
                    </tr>
                  ))}
                </tbody>
                <tfoot>
                  <tr style={{ borderTop: '1.5px solid var(--nt-line)' }}>
                    <td className="fw-800">Total {active}</td><td /><td className="fw-800 text-teal">{activeTotal} kcal</td><td />
                  </tr>
                </tfoot>
              </table>
            )}
          </div>
        </div>
      </div>

      {toast && <div className={'nt-toast' + (toast.warn ? ' warn' : '')}><Icon name={toast.warn ? 'x' : 'check'} size={16} /> {toast.message}</div>}
    </div>
  );
}
