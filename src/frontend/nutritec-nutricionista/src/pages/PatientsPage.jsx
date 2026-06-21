import { useState, useEffect, useCallback } from 'react';
import Icon from '@nutritec/shared/components/Icon.jsx';
import Pill from '@nutritec/shared/components/Pill.jsx';
import Modal from '@nutritec/shared/components/Modal.jsx';
import Avatar from '@nutritec/shared/components/Avatar.jsx';
import DatePicker from '@nutritec/shared/components/DatePicker.jsx';
import SectionTitle from '@nutritec/shared/components/SectionTitle.jsx';
import { getPatients, searchClients, associateClient, assignPlan } from '@nutritec/shared/services/patientService.js';
import { getPlans } from '@nutritec/shared/services/planService.js';
import { getAdminProducts } from '@nutritec/shared/services/productService.js';
import { byId, planKcal } from '@nutritec/shared/utils/nutrition.js';
import { formatDate } from '@nutritec/shared/utils/dates.js';

function AssignModal({ patient, plans, catalog, onClose, onAssign }) {
  const [planId, setPlanId] = useState(plans[0]?.id ?? '');
  const [start, setStart] = useState(new Date().toISOString().slice(0, 10));
  const [end, setEnd] = useState('');
  const plan = byId(plans, Number(planId));
  const total = plan ? Math.round(planKcal(plan.meals, catalog)) : 0;

  return (
    <Modal title="Asignar plan a paciente" sub={patient.name} onClose={onClose}
      footer={<>
        <button className="btn btn-soft" onClick={onClose}>Cancelar</button>
        <button className="btn btn-primary px-4" onClick={() => onAssign(Number(planId), start, end || null)} disabled={!planId}>
          <Icon name="check" size={16} /> Asignar plan
        </button>
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
          <label className="form-label">Fecha final (opcional)</label>
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

function PatientCard({ p, onOpen, onAssign }) {
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
      </div>
      <div className="p-2 rounded-3 mb-3" style={{ background: p.hasActivePlan ? 'var(--nt-teal-50)' : '#F2F4F4', border: '1px solid var(--nt-line)' }}>
        <div className="d-flex align-items-center gap-2">
          <Icon name="plate" size={16} />
          <span className="fw-700" style={{ fontSize: '.88rem' }}>
            {p.hasActivePlan ? 'Tiene plan activo' : 'Sin plan asignado'}
          </span>
        </div>
      </div>
      <div className="d-flex gap-2 mt-auto">
        <button className="btn btn-outline-primary btn-sm flex-fill" onClick={() => onOpen(p.id)}><Icon name="chat" size={15} /> Seguimiento</button>
        <button className="btn btn-primary btn-sm flex-fill" onClick={() => onAssign(p)}><Icon name="plate" size={15} /> {p.hasActivePlan ? 'Reasignar' : 'Asignar plan'}</button>
      </div>
    </div>
  );
}

export default function PatientsPage({ nutritionistId, onOpenPatient }) {
  const [patients, setPatients] = useState([]);
  const [searchResults, setSearchResults] = useState([]);
  const [plans, setPlans] = useState([]);
  const [catalog, setCatalog] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  const [q, setQ] = useState('');
  const [searching, setSearching] = useState(false);
  const [assignFor, setAssignFor] = useState(null);
  const [toast, setToast] = useState(null);

  // Carga pacientes y planes al montar.
  useEffect(() => {
    setLoading(true); setError(null);
    Promise.all([getPatients(nutritionistId), getPlans(nutritionistId), getAdminProducts('ACTIVE')])
      .then(([pts, pls, cat]) => { setPatients(pts); setPlans(pls); setCatalog(cat); })
      .catch((err) => setError(err.message))
      .finally(() => setLoading(false));
  }, [nutritionistId]);

  // Búsqueda de clientes con debounce.
  useEffect(() => {
    if (!q.trim() || q.trim().length < 2) { setSearchResults([]); return; }
    const t = setTimeout(() => {
      setSearching(true);
      searchClients(q.trim())
        .then(setSearchResults)
        .catch(() => setSearchResults([]))
        .finally(() => setSearching(false));
    }, 350);
    return () => clearTimeout(t);
  }, [q]);

  function flash(message) {
    setToast(message);
    setTimeout(() => setToast(null), 2600);
  }

  async function associate(client) {
    try {
      await associateClient(nutritionistId, client.id);
      const pts = await getPatients(nutritionistId);
      setPatients(pts);
      setQ(''); setSearchResults([]);
      flash(`${client.name} fue asociado a tu lista de pacientes`);
    } catch (err) {
      flash(err.message || 'No se pudo asociar al cliente');
    }
  }

  async function applyAssign(planId, start, end) {
    const patient = assignFor;
    setAssignFor(null);
    try {
      await assignPlan(planId, { clientId: patient.id, nutritionistCode: nutritionistId, startDate: start, endDate: end });
      const pts = await getPatients(nutritionistId);
      setPatients(pts);
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

  return (
    <div className="nt-content">
      <div className="nt-card nt-card-pad mb-4">
        <SectionTitle sub="Encuentra un cliente registrado en la plataforma y agrégalo a tu lista de pacientes">Buscar y asociar clientes</SectionTitle>
        <div className="input-group input-group-lg mb-3">
          <span className="input-group-text"><Icon name="search" size={18} /></span>
          <input className="form-control" placeholder="Buscar por nombre o correo… (mín. 2 caracteres)" value={q} onChange={(e) => setQ(e.target.value)} />
          {searching && <span className="input-group-text"><span className="spinner-border spinner-border-sm" /></span>}
        </div>
        {q.trim().length < 2 ? (
          <div className="nt-empty">
            <Icon name="users" size={26} />
            <div className="mt-2 fw-700">Escribe para buscar clientes</div>
            <div style={{ fontSize: '.88rem' }}>Solo los clientes que asocies podrán recibir planes y seguimiento tuyos.</div>
          </div>
        ) : searchResults.length === 0 && !searching ? (
          <div className="nt-empty">No se encontraron clientes para «{q}».</div>
        ) : (
          <div className="row g-2">
            {searchResults.map((c) => (
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
      {patients.length === 0 ? (
        <div className="nt-empty"><Icon name="users" size={26} /><div className="mt-2 fw-700">Aún no tienes pacientes asociados</div></div>
      ) : (
        <div className="row g-3">
          {patients.map((p) => (
            <div className="col-md-6 col-xl-4" key={p.id}>
              <PatientCard p={p} onOpen={onOpenPatient} onAssign={setAssignFor} />
            </div>
          ))}
        </div>
      )}

      {assignFor && <AssignModal patient={assignFor} plans={plans} catalog={catalog} onClose={() => setAssignFor(null)} onAssign={applyAssign} />}
      {toast && <div className="nt-toast"><Icon name="check" size={16} /> {toast}</div>}
    </div>
  );
}
