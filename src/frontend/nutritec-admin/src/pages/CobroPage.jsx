// Manejar el reporte de cobro: muestra un tipo de cobro a la vez con su ventana de ciclo automática.

import { useState, useEffect } from 'react';
import Icon from '@nutritec/shared/components/Icon.jsx';
import { getBillingReport, FREQUENCY_LABELS } from '@nutritec/shared/services/billingService.js';
import { fmtUSD } from '@nutritec/shared/utils/format.js';
import { toISODate, MONTHS, MONTHS_SHORT } from '@nutritec/shared/utils/dates.js';

// Capitaliza la primera letra (los meses vienen en minúscula).
function capitalize(text) {
  return text.charAt(0).toUpperCase() + text.slice(1);
}

// Ventana de cobro (inicio, fin y etiqueta) para una frecuencia y una fecha ancla.
// El denominador del prorrateo queda coherente con la tarifa: semana=7, mes, año.
function billingCycle(frequency, anchor) {
  const y = anchor.getFullYear();
  const m = anchor.getMonth();
  const d = anchor.getDate();

  if (frequency === 'WEEKLY') {
    const dow = (anchor.getDay() + 6) % 7; // 0 = lunes
    const start = new Date(y, m, d - dow);
    const end = new Date(y, m, d - dow + 6);
    const label = `${start.getDate()} ${MONTHS_SHORT[start.getMonth()]} – ${end.getDate()} ${MONTHS_SHORT[end.getMonth()]} ${end.getFullYear()}`;
    return { start, end, label };
  }
  if (frequency === 'ANNUAL') {
    return { start: new Date(y, 0, 1), end: new Date(y, 11, 31), label: `Año ${y}` };
  }
  // MONTHLY
  return { start: new Date(y, m, 1), end: new Date(y, m + 1, 0), label: `${capitalize(MONTHS[m])} ${y}` };
}

// Mueve el ancla al periodo anterior (-1) o siguiente (+1) según la frecuencia.
function shiftAnchor(frequency, cycleStart, dir) {
  const y = cycleStart.getFullYear();
  const m = cycleStart.getMonth();
  const d = cycleStart.getDate();
  if (frequency === 'WEEKLY') return new Date(y, m, d + dir * 7);
  if (frequency === 'ANNUAL') return new Date(y + dir, 0, 1);
  return new Date(y, m + dir, 1); // MONTHLY
}

// Vista principal: elige tipo de cobro y periodo, y muestra el reporte de ese tipo.
export default function CobroPage() {
  const [frequency, setFrequency] = useState('MONTHLY');
  const [anchor, setAnchor] = useState(() => new Date());
  const [report, setReport] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  const cycle = billingCycle(frequency, anchor);
  const startISO = toISODate(cycle.start);
  const endISO = toISODate(cycle.end);

  // Recarga el reporte cada vez que cambian el tipo de cobro o el periodo.
  useEffect(() => {
    setLoading(true); setError(null);
    getBillingReport({ cycleStartDate: startISO, cycleEndDate: endISO, frequency })
      .then(setReport)
      .catch((err) => { setError(err.message); setReport(null); })
      .finally(() => setLoading(false));
  }, [startISO, endISO, frequency]);

  const navigate = (dir) => setAnchor(shiftAnchor(frequency, cycle.start, dir));

  const nutritionists = report?.nutritionists ?? [];
  const first = nutritionists[0];
  const typeLabel = FREQUENCY_LABELS[frequency];

  return (
    <div className="nt-content">
      <div className="d-flex flex-wrap gap-3 align-items-center justify-content-between mb-3 d-print-none">
        <div className="nt-seg">
          {Object.entries(FREQUENCY_LABELS).map(([code, label]) => (
            <button key={code} className={frequency === code ? 'on' : ''} onClick={() => setFrequency(code)}>{label}</button>
          ))}
        </div>
        <div className="d-flex align-items-center gap-2">
          <button className="btn btn-soft btn-sm" title="Periodo anterior" onClick={() => navigate(-1)}>‹</button>
          <div className="fw-700 text-center" style={{ minWidth: 150 }}>{cycle.label}</div>
          <button className="btn btn-soft btn-sm" title="Periodo siguiente" onClick={() => navigate(1)}>›</button>
        </div>
        <button className="btn btn-soft" onClick={() => window.print()}><Icon name="pdf" size={16} /> Imprimir / PDF</button>
      </div>

      {loading ? (
        <div className="text-center py-5"><div className="spinner-border text-teal" role="status" /></div>
      ) : error ? (
        <div className="alert alert-danger">{error}</div>
      ) : nutritionists.length === 0 ? (
        <div className="nt-empty"><Icon name="coins" size={28} /><div className="mt-2 fw-700">Sin nutricionistas que cobrar en {cycle.label}</div></div>
      ) : (
        <>
          <div className="text-muted-soft mb-3" style={{ fontSize: '.9rem' }}>
            <span className="fw-700">{typeLabel} · {cycle.label}</span>
            {' · '}{fmtUSD(first.pricePerPatient)}/paciente
            {first.discountRate > 0 && ` · ${Math.round(first.discountRate * 100)}% de descuento`}
            {' · '}{report.cycleDays} días · {nutritionists.length} nutricionistas
          </div>
          <div className="nt-card">
            <div className="table-responsive">
              <table className="table mb-0 align-middle">
                <thead>
                  <tr>
                    <th>Nutricionista</th>
                    <th>Correo</th>
                    <th className="text-center">Pacientes</th>
                    <th className="text-end">Total</th>
                  </tr>
                </thead>
                <tbody>
                  {nutritionists.map((n) => (
                    <tr key={n.code}>
                      <td className="fw-700">{n.name}</td>
                      <td className="text-muted-soft">{n.email}</td>
                      <td className="text-center mono">{n.patients}</td>
                      <td className="text-end mono fw-800">{fmtUSD(n.total)}</td>
                    </tr>
                  ))}
                </tbody>
                <tfoot>
                  <tr className="nt-table-sub">
                    <td colSpan={2} className="text-muted-soft">
                      {report.totalDiscount > 0 && (
                        <>Bruto {fmtUSD(report.totalGross)} · Descuento <span className="text-danger-soft">−{fmtUSD(report.totalDiscount)}</span></>
                      )}
                    </td>
                    <td className="text-end">Total general</td>
                    <td className="text-end mono text-teal fw-800">{fmtUSD(report.total)}</td>
                  </tr>
                </tfoot>
              </table>
            </div>
          </div>
        </>
      )}
    </div>
  );
}
