// Manejar el registro y el historial de medidas corporales del cliente.

import { useState, useEffect } from 'react';
import Icon from '@nutritec/shared/components/Icon.jsx';
import DatePicker from '@nutritec/shared/components/DatePicker.jsx';
import SectionTitle from '@nutritec/shared/components/SectionTitle.jsx';
import { getHistory, createMeasurement } from '@nutritec/shared/services/measurementService.js';
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
  const [toast, setToast] = useState(null);

  const set = (key, value) => setForm((prev) => ({ ...prev, [key]: value }));

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

  // Registra la medida contra el backend y recarga el historial.
  async function save(e) {
    e.preventDefault();
    try {
      await createMeasurement(formToMeasurement(form), clientId);
      const data = await getHistory(clientId);
      setRows([...data].sort((a, b) => b.date.localeCompare(a.date)));
      setForm(EMPTY_FORM);
      setMode('historial');
      flash('Medidas guardadas');
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
        <button className={mode === 'registrar' ? 'on' : ''} onClick={() => setMode('registrar')}>Registrar</button>
        <button className={mode === 'historial' ? 'on' : ''} onClick={() => setMode('historial')}>Historial</button>
      </div>

      {mode === 'registrar' ? (
        <div style={{ maxWidth: 560 }}>
          <div className="nt-card nt-card-pad">
            <SectionTitle sub="Anota tus medidas para una fecha dada">Nuevo registro de medidas</SectionTitle>
            <form onSubmit={save}>
              <div className="mb-3">
                <label className="form-label">Fecha</label>
                <DatePicker value={form.date} onChange={(v) => set('date', v)} />
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
              <button type="submit" className="btn btn-primary w-100"><Icon name="plus" size={16} /> Guardar medidas</button>
            </form>
          </div>
        </div>
      ) : (
        <div className="nt-card nt-card-pad">
          <SectionTitle sub="Tus medidas registradas">Historial</SectionTitle>
          <div className="table-responsive">
            <table className="table align-middle mb-0">
              <thead><tr><th>Fecha</th><th>Peso</th><th>IMC</th><th>Cintura</th><th>Cuello</th><th>Caderas</th><th>% Músc.</th><th>% Grasa</th></tr></thead>
              <tbody>
                {rows.map((r, i) => (
                  <tr key={i}>
                    <td className="fw-700">{formatDate(r.date)}</td>
                    <td>{r.weight} kg</td><td>{r.bmi}</td>
                    <td>{r.waist} cm</td><td>{r.neck} cm</td><td>{r.hips} cm</td>
                    <td>{r.muscle}%</td><td>{r.fat}%</td>
                  </tr>
                ))}
                {rows.length === 0 && <tr><td colSpan="8"><div className="nt-empty">Aún no has registrado medidas.</div></td></tr>}
              </tbody>
            </table>
          </div>
        </div>
      )}

      {toast && <div className={'nt-toast' + (toast.warn ? ' warn' : '')}><Icon name={toast.warn ? 'x' : 'check'} size={16} /> {toast.message}</div>}
    </div>
  );
}
