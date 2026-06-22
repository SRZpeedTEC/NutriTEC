import { useState, useEffect, useRef } from 'react';
import Icon from '@nutritec/shared/components/Icon.jsx';
import Pill from '@nutritec/shared/components/Pill.jsx';
import Avatar from '@nutritec/shared/components/Avatar.jsx';
import SectionTitle from '@nutritec/shared/components/SectionTitle.jsx';
import LineChart from '@nutritec/shared/components/LineChart.jsx';
import { getPatients } from '@nutritec/shared/services/patientService.js';
import { getActivePlan } from '@nutritec/shared/services/planService.js';
import { getToday } from '@nutritec/shared/services/dailyConsumeService.js';
import { getHistory } from '@nutritec/shared/services/measurementService.js';
import { getPatientThread, sendPatientMessage, markAsRead } from '@nutritec/shared/services/feedbackService.js';
import { MEAL_TIMES, byId, sumKcal } from '@nutritec/shared/utils/nutrition.js';
import { formatDate } from '@nutritec/shared/utils/dates.js';

const MEAL_ICONS = { 'Desayuno': 'flame', 'Merienda Mañana': 'clock', 'Almuerzo': 'plate', 'Merienda Tarde': 'clock', 'Cena': 'chef' };

const METRICS = [
  { k: 'weight', l: 'Peso', u: 'kg' }, { k: 'waist', l: 'Cintura', u: 'cm' },
  { k: 'hips', l: 'Caderas', u: 'cm' }, { k: 'muscle', l: '% Músculo', u: '%' }, { k: 'fat', l: '% Grasa', u: '%' },
];

function PatientPicker({ patients, sel, onSel }) {
  return (
    <div className="nt-card nt-card-pad">
      <div className="fw-700 mb-2">Mis pacientes</div>
      <div className="d-flex flex-column gap-1">
        {patients.map((p) => (
          <button key={p.id} className={'nt-patient-item' + (sel === p.id ? ' active' : '')} onClick={() => onSel(p.id)}>
            <Avatar initials={p.initials} size={36} />
            <div className="flex-fill min-w-0 text-start">
              <div className="fw-700 text-truncate" style={{ fontSize: '.88rem' }}>{p.name}</div>
              <div className="text-truncate text-muted-soft" style={{ fontSize: '.76rem' }}>{p.hasActivePlan ? 'Plan activo' : 'Sin plan'}</div>
            </div>
          </button>
        ))}
      </div>
    </div>
  );
}

function PatientLog({ log, goal, plan }) {
  const total = log.reduce((a, m) => a + sumKcal(m.items), 0);
  return (
    <div className="nt-card nt-card-pad">
      <div className="d-flex justify-content-between align-items-start mb-3">
        <SectionTitle sub={plan ? `Plan: ${plan.name}` : 'Sin plan asignado'}>Registro diario</SectionTitle>
        <Pill tone={total > goal ? 'red' : 'teal'}>{total} / {goal} kcal</Pill>
      </div>
      <div className="progress mb-3">
        <div className={'progress-bar' + (total > goal ? ' over' : '')} style={{ width: Math.min(100, goal > 0 ? (total / goal) * 100 : 0) + '%' }} />
      </div>
      <div className="d-flex flex-column gap-2">
        {MEAL_TIMES.map((t) => {
          const items = log.find((m) => m.mealTime === t)?.items ?? [];
          const planMeal = plan ? plan.meals?.find((m) => m.mealTime === t) : null;
          const sum = sumKcal(items);
          const max = planMeal ? planMeal.maxKcal : null;
          return (
            <div className="nt-meal" key={t}>
              <div className="nt-meal-head">
                <div className="d-flex align-items-center gap-2">
                  <div className="nt-meal-ico"><Icon name={MEAL_ICONS[t]} size={15} /></div>
                  <div>
                    <div className="fw-700">{t}</div>
                    <div className="text-muted-soft" style={{ fontSize: '.76rem' }}>{max ? `Máx. ${max} kcal` : 'Sin meta'} · {items.length} alimentos</div>
                  </div>
                </div>
                <Pill tone={max && sum > max ? 'red' : 'gray'}>{sum} kcal</Pill>
              </div>
              {items.length > 0 && (
                <div>
                  {items.map((it, i) => (
                    <div className="nt-food-row" key={i}>
                      <div className="d-flex align-items-center gap-2">
                        <span className="text-muted-soft" style={{ fontSize: '.85rem' }}>{it.portions}×</span>
                        <span className="fw-600">{it.name}</span>
                      </div>
                      <span className="text-muted-soft">{it.kcal} kcal</span>
                    </div>
                  ))}
                </div>
              )}
            </div>
          );
        })}
      </div>
    </div>
  );
}

function PatientProgress({ p, measurements }) {
  const rows = [...(measurements || [])].sort((a, b) => (a.date || '').localeCompare(b.date || ''));
  const [metric, setMetric] = useState('weight');
  const mm = METRICS.find((x) => x.k === metric);

  return (
    <div>
      <div className="d-flex flex-wrap gap-2 mb-3 d-print-none align-items-center justify-content-between">
        <div className="d-flex flex-wrap gap-2">
          {METRICS.map((m) => (
            <button key={m.k} className={'btn btn-sm ' + (metric === m.k ? 'btn-primary' : 'btn-soft')} onClick={() => setMetric(m.k)}>{m.l}</button>
          ))}
        </div>
        <button className="btn btn-primary btn-sm" onClick={() => window.print()}><Icon name="pdf" size={15} /> Generar PDF</button>
      </div>
      <div className="nt-card nt-card-pad">
        <div className="d-flex justify-content-between align-items-start mb-4">
          <div>
            <h2 className="fw-800 mb-1">Reporte de Avance</h2>
            <div className="text-muted-soft">{p.name} · {rows.length} registros de medidas</div>
          </div>
          <div className="nt-brand-name" style={{ color: 'var(--nt-teal-700)' }}>Nutri<span>TEC</span></div>
        </div>
        {rows.length === 0 ? (
          <div className="nt-empty">El paciente aún no ha registrado medidas.</div>
        ) : (
          <>
            <div className="mb-4">
              <div className="fw-700 mb-2">{mm.l} ({mm.u})</div>
              <LineChart data={rows} keyName={metric} />
            </div>
            <div className="table-responsive">
              <table className="table table-striped align-middle mb-0">
                <thead><tr><th>Fecha</th><th>Peso</th><th>Cintura</th><th>Cuello</th><th>Caderas</th><th>% Músculo</th><th>% Grasa</th></tr></thead>
                <tbody>
                  {rows.map((m, i) => (
                    <tr key={i}>
                      <td className="fw-700">{formatDate(m.date)}</td><td>{m.weight} kg</td><td>{m.waist} cm</td>
                      <td>{m.neck} cm</td><td>{m.hips} cm</td><td>{m.muscle}%</td><td>{m.fat}%</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          </>
        )}
      </div>
    </div>
  );
}

function FeedbackThread({ p, nutritionistId, thread, onSend }) {
  const [text, setText] = useState('');
  const endRef = useRef(null);
  useEffect(() => { if (endRef.current) endRef.current.scrollTop = endRef.current.scrollHeight; }, [thread.length, p.id]);

  function send(e) {
    e.preventDefault();
    if (!text.trim()) return;
    onSend(text.trim());
    setText('');
  }

  return (
    <div className="nt-card nt-card-pad d-flex flex-column" style={{ height: 'calc(100vh - 230px)', minHeight: 480 }}>
      <div className="d-flex align-items-center justify-content-between mb-1">
        <SectionTitle sub="Foro de retroalimentación · se guarda en MongoDB">Retroalimentación</SectionTitle>
        <Pill tone="teal"><Icon name="chat" size={13} /> {thread.length}</Pill>
      </div>
      <div className="nt-thread" ref={endRef}>
        {thread.length === 0 && <div className="nt-empty">Aún no hay retroalimentación para {p.name.split(' ')[0]}.</div>}
        {thread.map((m, i) => {
          const mine = m.author === 'nutritionist';
          return (
            <div key={i} className={'nt-msg' + (mine ? ' mine' : '')}>
              <div className="nt-msg-bubble">
                <div className="d-flex align-items-center justify-content-between gap-3 mb-1">
                  <span className="fw-700" style={{ fontSize: '.82rem' }}>{mine ? 'Tú' : m.name}</span>
                  <span style={{ fontSize: '.72rem', opacity: .7 }}>{formatDate(m.date)}</span>
                </div>
                <div style={{ fontSize: '.9rem', lineHeight: 1.5 }}>{m.text}</div>
              </div>
            </div>
          );
        })}
      </div>
      <form onSubmit={send} className="nt-thread-compose">
        <textarea className="form-control" rows={2} placeholder={`Escribe retroalimentación para ${p.name.split(' ')[0]}…`}
          value={text} onChange={(e) => setText(e.target.value)}
          onKeyDown={(e) => { if (e.key === 'Enter' && (e.metaKey || e.ctrlKey)) send(e); }} />
        <div className="d-flex justify-content-between align-items-center mt-2">
          <span className="text-muted-soft" style={{ fontSize: '.76rem' }}>Campo de prosa libre, sin límite de extensión.</span>
          <button type="submit" className="btn btn-primary px-3" disabled={!text.trim()}><Icon name="send" size={15} /> Enviar</button>
        </div>
      </form>
    </div>
  );
}

function SegToggle({ view, onView }) {
  const opts = [
    { key: 'registro', label: 'Registro diario', icon: 'plate' },
    { key: 'avance', label: 'Avance', icon: 'chart' },
    { key: 'feedback', label: 'Retroalimentación', icon: 'chat' },
  ];
  return (
    <div className="nt-seg mb-3 d-print-none">
      {opts.map((o) => (
        <button key={o.key} type="button" className={view === o.key ? 'on' : ''} onClick={() => onView(o.key)}>
          <Icon name={o.icon} size={16} /> {o.label}
        </button>
      ))}
    </div>
  );
}

export default function FollowupPage({ nutritionistId, initialPatient }) {
  const [patients, setPatients] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  const [sel, setSel] = useState(initialPatient);
  const [view, setView] = useState('registro');
  const [thread, setThread] = useState([]);
  const [today, setToday] = useState(null);
  const [measurements, setMeasurements] = useState([]);
  const [activePlan, setActivePlan] = useState(null);
  const [toast, setToast] = useState(null);

  useEffect(() => {
    setLoading(true); setError(null);
    getPatients(nutritionistId)
      .then((pts) => {
        setPatients(pts);
        setSel((cur) => cur ?? pts[0]?.id);
      })
      .catch((err) => setError(err.message))
      .finally(() => setLoading(false));
  }, [nutritionistId]);

  useEffect(() => { if (initialPatient) setSel(initialPatient); }, [initialPatient]);

  // Carga los datos del paciente seleccionado: registro de hoy, medidas, plan activo e hilo.
  useEffect(() => {
    if (!sel) return;
    let alive = true;
    setToday(null); setMeasurements([]); setActivePlan(null);
    getToday(sel).then((d) => { if (alive) setToday(d); }).catch(() => { if (alive) setToday(null); });
    getHistory(sel).then((m) => { if (alive) setMeasurements(m); }).catch(() => { if (alive) setMeasurements([]); });
    getActivePlan(sel).then((pl) => { if (alive) setActivePlan(pl); }).catch(() => { if (alive) setActivePlan(null); });
    getPatientThread(nutritionistId, sel).then((t) => { if (alive) setThread(t); }).catch(() => { if (alive) setThread([]); });
    // Marca como leídos los mensajes del paciente al abrir el hilo (no bloquea la vista).
    markAsRead(nutritionistId, sel, nutritionistId).catch(() => {});
    return () => { alive = false; };
  }, [sel, nutritionistId]);

  async function send(text) {
    try {
      await sendPatientMessage(nutritionistId, sel, nutritionistId, text);
      setThread(await getPatientThread(nutritionistId, sel));
    } catch (err) {
      setToast(err.message || 'No se pudo enviar el mensaje');
      setTimeout(() => setToast(null), 2600);
    }
  }

  if (loading) {
    return <div className="nt-content"><div className="text-center py-5"><div className="spinner-border text-teal" role="status" /></div></div>;
  }
  if (error) {
    return <div className="nt-content"><div className="alert alert-danger">{error}</div></div>;
  }

  const p = byId(patients, sel);
  if (!p) {
    return <div className="nt-content"><div className="nt-empty"><Icon name="users" size={28} /><div className="mt-2 fw-700">Aún no tienes pacientes asociados</div></div></div>;
  }
  // La meta calórica sale del consumo de hoy; si no hay, del total del plan activo.
  const goal = today?.maxDailyCalories || activePlan?.totalGoal || 0;

  return (
    <div className="nt-content">
      <div className="row g-4">
        <div className="col-lg-3 d-print-none">
          <div style={{ position: 'sticky', top: 90 }}>
            <PatientPicker patients={patients} sel={sel} onSel={setSel} />
            <div className="nt-card nt-card-pad mt-3">
              <div className="d-flex align-items-center gap-2 mb-2">
                <Avatar initials={p.initials} size={42} />
                <div className="min-w-0">
                  <div className="fw-700 text-truncate">{p.name}</div>
                  <div className="text-muted-soft text-truncate" style={{ fontSize: '.78rem' }}>{p.email}</div>
                </div>
              </div>
              <div className="row g-2 text-center mt-1">
                {[['Edad', p.age + ' años'], ['País', p.country]].map((m, i) => (
                  <div className="col-6" key={i}>
                    <div className="p-2 rounded-3" style={{ background: 'var(--nt-teal-50)' }}>
                      <div className="fw-800" style={{ fontSize: '.95rem' }}>{m[1]}</div>
                      <div className="text-muted-soft" style={{ fontSize: '.72rem' }}>{m[0]}</div>
                    </div>
                  </div>
                ))}
              </div>
            </div>
          </div>
        </div>
        <div className="col-lg-9">
          <SegToggle view={view} onView={setView} />
          {view === 'registro' && <PatientLog log={today?.meals ?? []} goal={goal} plan={activePlan} />}
          {view === 'avance' && <PatientProgress p={p} measurements={measurements} />}
          {view === 'feedback' && <FeedbackThread p={p} nutritionistId={nutritionistId} thread={thread} onSend={send} />}
        </div>
      </div>
      {toast && <div className="nt-toast warn"><Icon name="x" size={16} /> {toast}</div>}
    </div>
  );
}
