// Manejar el reporte de cobro: agrupa nutricionistas por tipo de pago y totaliza.

import { useState, useEffect } from 'react';
import Icon from '@nutritec/shared/components/Icon.jsx';
import { getBillingReport } from '@nutritec/shared/services/billingService.js';
import { calcBilling, BILLING_RULES } from '@nutritec/shared/utils/billing.js';
import { fmtUSD } from '@nutritec/shared/utils/format.js';

// Orden en que se muestran los grupos de cobro.
const GROUP_ORDER = ['Semanal', 'Mensual', 'Anual'];

// Grupo de un tipo de pago: una fila por nutricionista más el subtotal del grupo.
function CobroGroup({ type, items }) {
  const rows = items.map((n) => ({ n, c: calcBilling(n) }));
  const sub = rows.reduce((a, { n, c }) => ({
    patients: a.patients + n.patients,
    gross: a.gross + c.gross,
    discount: a.discount + c.discount,
    total: a.total + c.total,
  }), { patients: 0, gross: 0, discount: 0, total: 0 });

  return (
    <>
      <tr className="nt-table-group">
        <th colSpan={7}>
          <div className="d-flex align-items-center gap-2 py-1">
            <Icon name="coins" size={16} /> Tipo de pago: {type}
            <span className="text-muted-soft fw-600" style={{ fontSize: '.82rem' }}>· {BILLING_RULES[type].detail}</span>
            <span className="nt-pill nt-pill-gray ms-auto">{items.length} nutricionistas</span>
          </div>
        </th>
      </tr>
      {rows.map(({ n, c }) => (
        <tr key={n.id}>
          <td className="fw-600">{n.email}</td>
          <td className="fw-700">{n.name}</td>
          <td className="mono text-muted-soft">{n.card}</td>
          <td className="text-center mono">{n.patients}</td>
          <td className="text-end mono">{fmtUSD(c.gross)}</td>
          <td className="text-end mono text-danger-soft">{c.discount > 0 ? '−' + fmtUSD(c.discount) : '—'}</td>
          <td className="text-end mono fw-800">{fmtUSD(c.total)}</td>
        </tr>
      ))}
      <tr className="nt-table-sub">
        <td colSpan={3} className="text-end">Subtotal {type}</td>
        <td className="text-center mono">{sub.patients}</td>
        <td className="text-end mono">{fmtUSD(sub.gross)}</td>
        <td className="text-end mono text-danger-soft">{sub.discount > 0 ? '−' + fmtUSD(sub.discount) : '—'}</td>
        <td className="text-end mono text-teal">{fmtUSD(sub.total)}</td>
      </tr>
    </>
  );
}

// Vista principal: carga los nutricionistas y arma el reporte agrupado.
export default function CobroPage() {
  const [nutritionists, setNutritionists] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  // Carga los nutricionistas con su información de cobro.
  useEffect(() => {
    setLoading(true); setError(null);
    getBillingReport()
      .then(setNutritionists)
      .catch((err) => setError(err.message))
      .finally(() => setLoading(false));
  }, []);

  if (loading) {
    return <div className="nt-content"><div className="text-center py-5"><div className="spinner-border text-teal" role="status" /></div></div>;
  }
  if (error) {
    return <div className="nt-content"><div className="alert alert-danger">{error}</div></div>;
  }
  if (nutritionists.length === 0) {
    return <div className="nt-content"><div className="nt-empty"><Icon name="coins" size={28} /><div className="mt-2 fw-700">Sin nutricionistas que cobrar</div></div></div>;
  }

  const groups = GROUP_ORDER
    .map((type) => ({ type, items: nutritionists.filter((n) => n.billingType === type) }))
    .filter((g) => g.items.length > 0);

  const grand = nutritionists.reduce((a, n) => a + calcBilling(n).total, 0);

  return (
    <div className="nt-content">
      <div className="d-flex align-items-center justify-content-between mb-3 d-print-none">
        <div className="text-muted-soft" style={{ fontSize: '.9rem' }}>
          {nutritionists.length} nutricionistas · cobro calculado según el tipo de pago de cada uno.
        </div>
        <button className="btn btn-soft" onClick={() => window.print()}><Icon name="pdf" size={16} /> Imprimir / PDF</button>
      </div>

      <div className="nt-card">
        <div className="table-responsive">
          <table className="table mb-0 align-middle">
            <thead>
              <tr>
                <th>Correo</th>
                <th>Nutricionista</th>
                <th>Tarjeta</th>
                <th className="text-center">Pacientes</th>
                <th className="text-end">Bruto</th>
                <th className="text-end">Descuento</th>
                <th className="text-end">Total</th>
              </tr>
            </thead>
            <tbody>
              {groups.map((g) => <CobroGroup key={g.type} type={g.type} items={g.items} />)}
            </tbody>
            <tfoot>
              <tr className="nt-table-sub">
                <td colSpan={6} className="text-end">Total general</td>
                <td className="text-end mono text-teal">{fmtUSD(grand)}</td>
              </tr>
            </tfoot>
          </table>
        </div>
      </div>
    </div>
  );
}
