// Manejar el registro y el historial de medidas corporales del cliente.

import { useState, useEffect } from 'react';
import Icon from '@nutritec/shared/components/Icon.jsx';
import DatePicker from '@nutritec/shared/components/DatePicker.jsx';
import SectionTitle from '@nutritec/shared/components/SectionTitle.jsx';
import { getHistory, createMeasurement, updateMeasurement } from '@nutritec/shared/services/measurementService.js';
import { formatDate } from '@nutritec/shared/utils/dates.js';

const EMPTY_FORM = { date: '2026-06-08', weight: '', bmi: '', waist: '', neck: '', hips: '', muscle: '', fat: '' };

// Campos numéricos del formulario con su etiqueta y unidad.
const FIELDS = [
  { k: 'weight', l: 'Peso', u: 'kg' }, { k: 'bmi', l: 'IMC', u: 'kg/m²' },
  { k: 'waist', l: 'Cintura', u: 'cm' }, { k: 'neck', l: 'Cuello', u: 'cm' },
  { k: 'hips', l: 'Caderas', u: 'cm' }, { k: 'muscle', l: '% Músculo', u: '%' }, { k: 'fat', l: '% Grasa', u: '%' },
];

// Convierte el formulario a la medida que espera el servicio (numéricos como número).
function formToMeasurement(f) {
  return {
    date: f.date,
    weight: Number(f.weight), bmi: Number(f.bmi),
    waist: Number(f.waist), neck: Number(f.neck), hips: Number(f.hips),
    muscle: Number(f.muscle), fat: Number(f.fat),
  };
}

// Vista principal: registra una medida nueva y consulta el historial.
export default function MeasurementsPage({ clientId }) {
  const [rows, setRows] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  const [form, setForm] = useState(EMPTY_FORM);
  const [mode, setMode] = useState('registrar');
  const [editing, setEditing] = useState(false);
  const [toast, setToast] = useState(null);

  const set = (key, value) => setForm((prev) => ({ ...prev, [key]: value }));

  // Carga una medida en el formulario para editarla (solo la más reciente es editable).
  function startEdit(row) {
    setForm({
      date: row.date,
      weight: row.weight, bmi: row.bmi,
      waist: row.waist, neck: row.neck, hips: row.hips,
      muscle: row.muscle, fat: row.fat,
    });
    setEditing(true);
    setMode('registrar');
  }

  // Sale del modo edición y deja el formulario listo para un registro nuevo.
  function newEntry() {
    setForm(EMPTY_FORM);
    setEditing(false);
    setMode('registrar');
  }

  // Carga el historial de medidas del cliente (más reciente primero).
  useEffect(() => {
    setLoading(true); setError(null);
    getHistory(clientId)
      .then((data) => setRows([...data].sort((a, b) => b.date.localeCompare(a.date))))
      .catch((err) => setError(err.message))
      .finally(() => setLoading(false));
  }, [clientId]);

  // Muestra un aviso flotante temporal.
  function flash(message, warn) {
    setToast({ message, warn });
    setTimeout(() => setToast(null), 2600);
  }

  // Registra una medida nueva o actualiza la más reciente, y recarga el historial.
  async function save(e) {
    e.preventDefault();
    try {
      if (editing) {
        await updateMeasurement(clientId, form.date, formToMeasurement(form));
      } else {
        await createMeasurement(formToMeasurement(form), clientId);
      }
      const data = await getHistory(clientId);
      setRows([...data].sort((a, b) => b.date.localeCompare(a.date)));
      setForm(EMPTY_FORM);
      setEditing(false);
      setMode('historial');
      flash(editing ? 'Medidas actualizadas' : 'Medidas guardadas');
    } catch (err) {
      flash(err.message || 'No se pudieron guardar las medidas', true);
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
      <div className="nt-seg mb-4">
        <button className={mode === 'registrar' ? 'on' : ''} onClick={newEntry}>Registrar</button>
        <button className={mode === 'historial' ? 'on' : ''} onClick={() => setMode('historial')}>Historial</button>
      </div>

      {mode === 'registrar' ? (
        <div style={{ maxWidth: 560 }}>
          <div className="nt-card nt-card-pad">
            <SectionTitle sub={editing ? 'Modifica la última medición registrada' : 'Anota tus medidas para una fecha dada'}>
              {editing ? 'Editar última medida' : 'Nuevo registro de medidas'}
            </SectionTitle>
            <form onSubmit={save}>
              <div className="mb-3">
                <label className="form-label">Fecha</label>
                {editing ? (
                  // La fecha identifica el registro y no puede cambiarse al editar.
                  <input type="text" className="form-control" value={formatDate(form.date)} disabled readOnly />
                ) : (
                  <DatePicker value={form.date} onChange={(v) => set('date', v)} />
                )}
              </div>
              <div className="row">
                {FIELDS.map((c) => (
                  <div className="mb-3 col-6" key={c.k}>
                    <label className="form-label">{c.l}</label>
                    <div className="input-group">
                      <input type="number" step="0.1" className="form-control" placeholder="0" value={form[c.k]} onChange={(e) => set(c.k, e.target.value)} />
                      <span className="input-group-text">{c.u}</span>
                    </div>
                  </div>
                ))}
              </div>
              <div className="d-flex gap-2">
                <button type="submit" className="btn btn-primary flex-grow-1">
                  <Icon name={editing ? 'edit' : 'plus'} size={16} /> {editing ? 'Actualizar medidas' : 'Guardar medidas'}
                </button>
                {editing && (
                  <button type="button" className="btn btn-soft" onClick={newEntry}><Icon name="x" size={16} /> Cancelar</button>
                )}
              </div>
            </form>
          </div>
        </div>
      ) : (
        <div className="nt-card nt-card-pad">
          <SectionTitle sub="Tus medidas registradas">Historial</SectionTitle>
          <div className="table-responsive">
            <table className="table align-middle mb-0">
              <thead><tr><th>Fecha</th><th>Peso</th><th>IMC</th><th>Cintura</th><th>Cuello</th><th>Caderas</th><th>% Músc.</th><th>% Grasa</th><th></th></tr></thead>
              <tbody>
                {rows.map((r, i) => (
                  <tr key={i}>
                    <td className="fw-700">{formatDate(r.date)}</td>
                    <td>{r.weight} kg</td><td>{r.bmi}</td>
                    <td>{r.waist} cm</td><td>{r.neck} cm</td><td>{r.hips} cm</td>
                    <td>{r.muscle}%</td><td>{r.fat}%</td>
                    {/* Solo la medición más reciente (primera fila) puede editarse. */}
                    <td className="text-end">
                      {i === 0 && (
                        <button className="btn btn-sm btn-soft" onClick={() => startEdit(r)} title="Editar última medida"><Icon name="edit" size={14} /> Editar</button>
                      )}
                    </td>
                  </tr>
                ))}
                {rows.length === 0 && <tr><td colSpan="9"><div className="nt-empty">Aún no has registrado medidas.</div></td></tr>}
              </tbody>
            </table>
          </div>
        </div>
      )}

      {toast && <div className={'nt-toast' + (toast.warn ? ' warn' : '')}><Icon name={toast.warn ? 'x' : 'check'} size={16} /> {toast.message}</div>}
    </div>
  );
}
