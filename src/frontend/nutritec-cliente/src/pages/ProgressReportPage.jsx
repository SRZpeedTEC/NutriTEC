// Manejar el reporte de avance del cliente: rango de fechas, gráfica y exportación a PDF.

import { useState, useEffect } from 'react';
import Icon from '@nutritec/shared/components/Icon.jsx';
import DatePicker from '@nutritec/shared/components/DatePicker.jsx';
import LineChart from '@nutritec/shared/components/LineChart.jsx';
import { getReport } from '@nutritec/shared/services/measurementService.js';
import { formatDate } from '@nutritec/shared/utils/dates.js';

// Métricas graficables de las medidas.
const METRICS = [
  { k: 'weight', l: 'Peso', u: 'kg' }, { k: 'waist', l: 'Cintura', u: 'cm' },
  { k: 'hips', l: 'Caderas', u: 'cm' }, { k: 'muscle', l: '% Músculo', u: '%' }, { k: 'fat', l: '% Grasa', u: '%' },
];

// Vista principal: consulta el reporte de medidas del periodo y lo grafica.
export default function ProgressReportPage({ clientId, userName }) {
  const [start, setStart] = useState('2026-03-01');
  const [end, setEnd] = useState('2026-06-01');
  const [metric, setMetric] = useState('weight');
  const [measurements, setMeasurements] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  // Carga el reporte de medidas del periodo seleccionado.
  useEffect(() => {
    setLoading(true); setError(null);
    getReport(clientId, start, end)
      .then((report) => setMeasurements(report.measurements))
      .catch((err) => setError(err.message))
      .finally(() => setLoading(false));
  }, [clientId, start, end]);

  const mm = METRICS.find((x) => x.k === metric);

  return (
    <div className="nt-content">
      <div className="nt-card nt-card-pad mb-4 d-print-none">
        <div className="row g-3 align-items-end">
          <div className="col-md-4">
            <label className="form-label">Fecha inicio</label>
            <DatePicker value={start} onChange={setStart} />
          </div>
          <div className="col-md-4">
            <label className="form-label">Fecha final</label>
            <DatePicker value={end} onChange={setEnd} />
          </div>
          <div className="col-md-4">
            <button className="btn btn-primary w-100" onClick={() => window.print()}><Icon name="pdf" size={16} /> Generar PDF</button>
          </div>
        </div>
      </div>

      <div className="nt-card nt-card-pad">
        <div className="d-flex justify-content-between align-items-start mb-4">
          <div>
            <h2 className="fw-800 mb-1">Reporte de Avance</h2>
            <div className="text-muted-soft">{userName} · Periodo {formatDate(start)} a {formatDate(end)}</div>
          </div>
          <div className="nt-brand-name" style={{ color: 'var(--nt-teal-700)' }}>Nutri<span>TEC</span></div>
        </div>

        {loading ? (
          <div className="text-center py-5"><div className="spinner-border text-teal" role="status" /></div>
        ) : error ? (
          <div className="alert alert-danger">{error}</div>
        ) : measurements.length === 0 ? (
          <div className="nt-empty">No hay medidas registradas en este periodo.</div>
        ) : (
          <>
            <div className="d-flex flex-wrap gap-2 mb-3 d-print-none">
              {METRICS.map((m) => (
                <button key={m.k} className={'btn btn-sm ' + (metric === m.k ? 'btn-primary' : 'btn-soft')} onClick={() => setMetric(m.k)}>{m.l}</button>
              ))}
            </div>
            <div className="mb-4">
              <div className="fw-700 mb-2">{mm.l} ({mm.u})</div>
              <LineChart data={measurements} keyName={metric} />
            </div>
            <div className="table-responsive">
              <table className="table table-striped align-middle">
                <thead><tr><th>Fecha</th><th>Peso</th><th>Cintura</th><th>Cuello</th><th>Caderas</th><th>% Músculo</th><th>% Grasa</th></tr></thead>
                <tbody>
                  {measurements.map((m, i) => (
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
