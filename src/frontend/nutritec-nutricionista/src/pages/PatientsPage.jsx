// Manejar los pacientes: búsqueda y asociación de clientes y asignación de planes.

import { useState, useEffect } from 'react';
import Icon from '@nutritec/shared/components/Icon.jsx';
import Pill from '@nutritec/shared/components/Pill.jsx';
import Modal from '@nutritec/shared/components/Modal.jsx';
import Avatar from '@nutritec/shared/components/Avatar.jsx';
import DatePicker from '@nutritec/shared/components/DatePicker.jsx';
import SectionTitle from '@nutritec/shared/components/SectionTitle.jsx';
import { getPatients, getClients, associateClient, assignPlan } from '@nutritec/shared/services/patientService.js';
import { getPlans } from '@nutritec/shared/services/planService.js';
import { searchProducts } from '@nutritec/shared/services/dailyConsumeService.js';
import { byId, planKcal } from '@nutritec/shared/utils/nutrition.js';
import { formatDate } from '@nutritec/shared/utils/dates.js';

// Modal para asignar un plan a un paciente en un periodo.
function AssignModal({ patient, plans, catalog, onClose, onAssign }) {
  const [planId, setPlanId] = useState(patient.planId || plans[0]?.id);
  const [start, setStart] = useState('2026-06-08');
  const [end, setEnd] = useState('2026-07-08');
  const plan = byId(plans, planId);
  const total = plan ? Math.round(planKcal(plan.meals, catalog)) : 0;

  return (
    <Modal title="Asignar plan a paciente" sub={patient.name} onClose={onClose}
      footer={<>
        <button className="btn btn-soft" onClick={onClose}>Cancelar</button>
        <button className="btn btn-primary px-4" onClick={() => onAssign(planId, start, end)}><Icon name="check" size={16} /> Asignar plan</button>
      </>}>
      <div className="mb-3">
        <label className="form-label">Plan de alimentación</label>
        <select className="form-select" value={planId} onChange={(e) => setPlanId(e.target.value)}>
          {plans.map((p) => <option key={p.id} value={p.id}>{p.name}</option>)}
        </select>
      </div>
      <div className="row">
        <div className="mb-3 col-md-6">
          <label className="form-label">Fecha de inicio</label>
          <DatePicker value={start} onChange={setStart} showToday={false} />
        </div>
        <div className="mb-3 col-md-6">
          <label className="form-label">Fecha final</label>
          <DatePicker value={end} onChange={setEnd} showToday={false} />
        </div>
      </div>
      {plan && (
        <div className="p-3 rounded-3" style={{ background: 'var(--nt-teal-50)', border: '1px solid var(--nt-teal-100)' }}>
          <div className="d-flex justify-content-between align-items-center">
            <div>
              <div className="fw-700">{plan.name}</div>
              <div className="text-muted-soft" style={{ fontSize: '.82rem' }}>{plan.meals.length} tiempos de comida</div>
            </div>
            <Pill tone="teal"><Icon name="flame" size={13} /> {total} kcal/día</Pill>
          </div>
        </div>
      )}
    </Modal>
  );
}

// Tarjeta de un paciente con su plan asignado y acciones.
function PatientCard({ p, plan, onOpen, onAssign }) {
  return (
    <div className="nt-card nt-card-pad h-100 d-flex flex-column">
      <div className="d-flex align-items-center gap-3 mb-3">
        <Avatar initials={p.initials} size={48} />
        <div className="flex-fill min-w-0">
          <div className="fw-700 text-truncate">{p.name}</div>
          <div className="text-muted-soft text-truncate" style={{ fontSize: '.82rem' }}>{p.email}</div>
        </div>
      </div>
      <div className="d-flex flex-wrap gap-2 mb-3">
        <span className="nt-chip">{p.age} años</span>
        <span className="nt-chip">{p.country}</span>
        <span className="nt-chip"><Icon name="flame" size={13} /> {p.calorieGoal} kcal</span>
      </div>
      <div className="p-2 rounded-3 mb-3" style={{ background: plan ? 'var(--nt-teal-50)' : '#F2F4F4', border: '1px solid var(--nt-line)' }}>
        {plan ? (
          <div className="d-flex align-items-center gap-2">
            <Icon name="plate" size={16} />
            <div className="min-w-0">
              <div className="fw-700 text-truncate" style={{ fontSize: '.88rem' }}>{plan.name}</div>
              <div className="text-muted-soft" style={{ fontSize: '.78rem' }}>{p.period}</div>
            </div>
          </div>
        ) : (
          <div className="d-flex align-items-center gap-2 text-muted-soft">
            <Icon name="plate" size={16} /><span style={{ fontSize: '.85rem' }}>Sin plan asignado</span>
          </div>
        )}
      </div>
      <div className="d-flex gap-2 mt-auto">
        <button className="btn btn-outline-primary btn-sm flex-fill" onClick={() => onOpen(p.id)}><Icon name="chat" size={15} /> Seguimiento</button>
        <button className="btn btn-primary btn-sm flex-fill" onClick={() => onAssign(p)}><Icon name="plate" size={15} /> {plan ? 'Reasignar' : 'Asignar plan'}</button>
      </div>
    </div>
  );
}

// Vista principal: carga pacientes, clientes y planes, y gestiona asociaciones.
export default function PatientsPage({ nutritionistId, onOpenPatient }) {
  const [patients, setPatients] = useState([]);
  const [clients, setClients] = useState([]);
  const [plans, setPlans] = useState([]);
  const [catalog, setCatalog] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  const [q, setQ] = useState('');
  const [assignFor, setAssignFor] = useState(null);
  const [toast, setToast] = useState(null);

  // Carga pacientes, clientes, planes y el catálogo (para el total de kcal del plan).
  useEffect(() => {
    setLoading(true); setError(null);
    Promise.all([getPatients(nutritionistId), getClients(nutritionistId), getPlans(nutritionistId), searchProducts('')])
      .then(([pts, cls, pls, cat]) => { setPatients(pts); setClients(cls); setPlans(pls); setCatalog(cat); })
      .catch((err) => setError(err.message))
      .finally(() => setLoading(false));
  }, [nutritionistId]);

  // Muestra un aviso flotante temporal.
  function flash(message) {
    setToast(message);
    setTimeout(() => setToast(null), 2600);
  }

  // Asocia un cliente como paciente del nutricionista.
  async function associate(client) {
    try {
      await associateClient(nutritionistId, client.id);
      const [pts, cls] = await Promise.all([getPatients(nutritionistId), getClients(nutritionistId)]);
      setPatients(pts); setClients(cls);
      flash(`${client.name} fue asociado a tu lista de pacientes`);
    } catch (err) {
      flash(err.message || 'No se pudo asociar al cliente');
    }
  }

  // Asigna un plan al paciente seleccionado en un periodo.
  async function applyAssign(planId, start, end) {
    const patient = assignFor;
    setAssignFor(null);
    try {
      await assignPlan(patient.id, { planId, startDate: start, endDate: end });
      setPatients(await getPatients(nutritionistId));
      flash(`Plan «${byId(plans, planId)?.name}» asignado a ${patient.name}`);
    } catch (err) {
      flash(err.message || 'No se pudo asignar el plan');
    }
  }

  if (loading) {
    return <div className="nt-content"><div className="text-center py-5"><div className="spinner-border text-teal" role="status" /></div></div>;
  }
  if (error) {
    return <div className="nt-content"><div className="alert alert-danger">{error}</div></div>;
  }

  const results = clients.filter((c) => {
    const t = q.trim().toLowerCase();
    if (!t) return false;
    return c.name.toLowerCase().includes(t) || c.email.toLowerCase().includes(t);
  });

  return (
    <div className="nt-content">
      <div className="nt-card nt-card-pad mb-4">
        <SectionTitle sub="Encuentra un cliente registrado en la plataforma y agrégalo a tu lista de pacientes">Buscar y asociar clientes</SectionTitle>
        <div className="input-group input-group-lg mb-3">
          <span className="input-group-text"><Icon name="search" size={18} /></span>
          <input className="form-control" placeholder="Buscar por nombre o correo… (ej. Sofía o mateo)" value={q} onChange={(e) => setQ(e.target.value)} />
        </div>
        {q.trim() === '' ? (
          <div className="nt-empty">
            <Icon name="users" size={26} />
            <div className="mt-2 fw-700">Escribe para buscar clientes</div>
            <div style={{ fontSize: '.88rem' }}>Solo los clientes que asocies podrán recibir planes y seguimiento tuyos.</div>
          </div>
        ) : results.length === 0 ? (
          <div className="nt-empty">No se encontraron clientes para «{q}».</div>
        ) : (
          <div className="row g-2">
            {results.map((c) => (
              <div className="col-md-6" key={c.id}>
                <div className="d-flex align-items-center gap-3 p-2 rounded-3" style={{ border: '1px solid var(--nt-line)' }}>
                  <Avatar initials={c.initials} size={42} />
                  <div className="flex-fill min-w-0">
                    <div className="fw-700 text-truncate">{c.name}</div>
                    <div className="text-muted-soft text-truncate" style={{ fontSize: '.8rem' }}>{c.age} años · {c.country}</div>
                  </div>
                  <button className="btn btn-primary btn-sm px-3 text-nowrap" onClick={() => associate(c)}><Icon name="link" size={15} /> Asociar</button>
                </div>
              </div>
            ))}
          </div>
        )}
      </div>

      <div className="d-flex justify-content-between align-items-center mb-3">
        <SectionTitle sub={`${patients.length} pacientes asociados`}>Mis pacientes</SectionTitle>
      </div>
      <div className="row g-3">
        {patients.map((p) => (
          <div className="col-md-6 col-xl-4" key={p.id}>
            <PatientCard p={p} plan={p.planId ? byId(plans, p.planId) : null} onOpen={onOpenPatient} onAssign={setAssignFor} />
          </div>
        ))}
      </div>

      {assignFor && <AssignModal patient={assignFor} plans={plans} catalog={catalog} onClose={() => setAssignFor(null)} onAssign={applyAssign} />}
      {toast && <div className="nt-toast"><Icon name="check" size={16} /> {toast}</div>}
    </div>
  );
}
