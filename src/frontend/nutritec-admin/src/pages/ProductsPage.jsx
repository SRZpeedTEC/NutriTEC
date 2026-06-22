import { useState, useEffect } from 'react';
import Icon from '@nutritec/shared/components/Icon.jsx';
import Pill from '@nutritec/shared/components/Pill.jsx';
import Modal from '@nutritec/shared/components/Modal.jsx';
import { getAdminProducts, approveProduct, rejectProduct } from '@nutritec/shared/services/productService.js';
import { formatDate } from '@nutritec/shared/utils/dates.js';

function NutriCell({ k, v, u }) {
  return (
    <div className="nt-nutri-cell">
      <div className="k">{k}</div>
      <div className="v">{v}<span className="text-muted-soft fw-700" style={{ fontSize: '.78rem' }}>{u ? ' ' + u : ''}</span></div>
    </div>
  );
}

function StatusPill({ status }) {
  if (status === 'Aprobado') return <Pill tone="teal"><Icon name="check" size={13} /> Aprobado</Pill>;
  if (status === 'Rechazado') return <Pill tone="red"><Icon name="x" size={13} /> Rechazado</Pill>;
  return <Pill tone="gray"><Icon name="clock" size={13} /> Pendiente</Pill>;
}

function NutriGrid({ p, pad }) {
  return (
    <div className="nt-nutri-grid" style={pad ? undefined : { padding: 0 }}>
      <NutriCell k="Energía" v={p.energy} u="kcal" />
      <NutriCell k="Grasa" v={p.fat} u="g" />
      <NutriCell k="Sodio" v={p.sodium} u="mg" />
      <NutriCell k="Carbos" v={p.carbs} u="g" />
      <NutriCell k="Proteína" v={p.protein} u="g" />
      <NutriCell k="Calcio" v={p.calcium} u="mg" />
      <NutriCell k="Hierro" v={p.iron} u="mg" />
      <NutriCell k="Vitaminas" v={p.vitamins} />
    </div>
  );
}

function ProductCard({ p, onApprove, onReject, onDetail }) {
  return (
    <div className="nt-prod h-100 d-flex flex-column">
      <div className="nt-prod-head">
        <div className="nt-prod-ico"><Icon name="box" size={22} /></div>
        <div className="flex-fill min-w-0">
          <div className="fw-800 text-truncate" style={{ fontSize: '1.02rem' }}>{p.name}</div>
          <div className="text-muted-soft d-flex align-items-center gap-2 flex-wrap" style={{ fontSize: '.8rem' }}>
            <span><Icon name="barcode" size={13} /> {p.barcode}</span>
            <span>·</span>
            <span>{p.portion}{p.unit}</span>
          </div>
        </div>
        <StatusPill status={p.status} />
      </div>

      {(p.author || p.date) && (
        <div className="px-3 pb-2 d-flex flex-wrap gap-2">
          {p.author && <span className="nt-chip"><Icon name={p.authorType === 'Nutricionista' ? 'users' : 'user'} size={13} /> {p.authorType}: {p.author}</span>}
          {p.date && <span className="nt-chip"><Icon name="cal" size={13} /> {formatDate(p.date)}</span>}
        </div>
      )}

      <NutriGrid p={p} pad />

      <div className="mt-auto px-3 pb-3 pt-1 d-flex gap-2">
        <button className="btn btn-soft btn-sm" onClick={() => onDetail(p)}><Icon name="search" size={14} /> Detalle</button>
        {p.status === 'Pendiente' ? (
          <>
            <button className="btn btn-danger-soft btn-sm flex-fill" onClick={() => onReject(p)}><Icon name="x" size={15} /> Rechazar</button>
            <button className="btn btn-primary btn-sm flex-fill" onClick={() => onApprove(p)}><Icon name="check" size={15} /> Aprobar</button>
          </>
        ) : (
          <span className="text-muted-soft ms-auto align-self-center" style={{ fontSize: '.8rem' }}>Revisado</span>
        )}
      </div>
    </div>
  );
}

function RejectModal({ product, onClose, onConfirm }) {
  return (
    <Modal title="Rechazar producto" sub={product.name} onClose={onClose}
      footer={<>
        <button className="btn btn-soft" onClick={onClose}>Cancelar</button>
        <button className="btn btn-danger-soft" onClick={onConfirm}><Icon name="x" size={15} /> Confirmar rechazo</button>
      </>}>
      <p className="text-muted-soft mb-0">El producto <strong>«{product.name}»</strong> quedará rechazado y no estará disponible para la comunidad. ¿Continuar?</p>
    </Modal>
  );
}

function DetailModal({ product, onClose }) {
  const p = product;
  return (
    <Modal title={p.name} sub={`Código ${p.barcode} · porción ${p.portion}${p.unit}`} onClose={onClose} size="lg"
      footer={<button className="btn btn-primary" onClick={onClose}>Cerrar</button>}>
      {(p.author || p.date) && (
        <div className="d-flex flex-wrap gap-2 mb-3">
          {p.author && <span className="nt-chip"><Icon name={p.authorType === 'Nutricionista' ? 'users' : 'user'} size={13} /> {p.authorType}: {p.author}</span>}
          {p.date && <span className="nt-chip"><Icon name="cal" size={13} /> Enviado el {formatDate(p.date)}</span>}
        </div>
      )}
      <NutriGrid p={p} />
    </Modal>
  );
}

// Mapeo de tabs a valores de status del backend.
const STATUS_FILTER_MAP = {
  Pendiente: 'PENDING_REVIEW',
  Aprobado: 'ACTIVE',
  Rechazado: 'REJECTED',
  Todos: null,
};

const TABS = [['Pendiente', 'Pendientes'], ['Aprobado', 'Aprobados'], ['Rechazado', 'Rechazados'], ['Todos', 'Todos']];

export default function ProductsPage({ userId, onPendingCount }) {
  const [products, setProducts] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  const [filter, setFilter] = useState('Pendiente');
  const [authorFilter, setAuthorFilter] = useState('Todos');
  const [q, setQ] = useState('');
  const [rejecting, setRejecting] = useState(null);
  const [detail, setDetail] = useState(null);
  const [toast, setToast] = useState(null);

  function load() {
    setLoading(true); setError(null);
    getAdminProducts()
      .then(setProducts)
      .catch((err) => setError(err.message))
      .finally(() => setLoading(false));
  }

  useEffect(load, []);

  useEffect(() => {
    onPendingCount?.(products.filter((p) => p.status === 'Pendiente').length);
  }, [products, onPendingCount]);

  function flash(message, warn) {
    setToast({ message, warn });
    setTimeout(() => setToast(null), 2600);
  }

  async function moderate(product, action) {
    try {
      if (action === 'Aprobado') {
        await approveProduct(product.barcode, userId);
      } else {
        await rejectProduct(product.barcode, userId);
      }
      setProducts((prev) => prev.map((x) => x.barcode === product.barcode ? { ...x, status: action } : x));
      flash(action === 'Aprobado' ? `«${product.name}» aprobado y publicado` : `«${product.name}» fue rechazado`, action === 'Rechazado');
    } catch (err) {
      flash(err.message || 'No se pudo actualizar el producto', true);
    }
  }

  const counts = {
    Pendiente: products.filter((p) => p.status === 'Pendiente').length,
    Aprobado: products.filter((p) => p.status === 'Aprobado').length,
    Rechazado: products.filter((p) => p.status === 'Rechazado').length,
    Todos: products.length,
  };

  const list = products.filter((p) => {
    if (filter !== 'Todos' && p.status !== filter) return false;
    if (authorFilter !== 'Todos' && p.authorType !== authorFilter) return false;
    const t = q.trim().toLowerCase();
    if (t && !((p.name || '').toLowerCase().includes(t) || (p.barcode || '').includes(t) || (p.author || '').toLowerCase().includes(t))) return false;
    return true;
  });

  if (loading) {
    return <div className="nt-content"><div className="text-center py-5"><div className="spinner-border text-teal" role="status" /></div></div>;
  }
  if (error) {
    return <div className="nt-content"><div className="alert alert-danger">{error}</div></div>;
  }

  return (
    <div className="nt-content">
      <div className="d-flex flex-wrap gap-3 align-items-center justify-content-between mb-3">
        <div className="nt-seg">
          {TABS.map(([k, l]) => (
            <button key={k} className={filter === k ? 'on' : ''} onClick={() => setFilter(k)}>
              {l} <span className="cnt">{counts[k]}</span>
            </button>
          ))}
        </div>
        <div className="d-flex gap-2 flex-wrap">
          <select className="form-select" style={{ width: 180 }} value={authorFilter} onChange={(e) => setAuthorFilter(e.target.value)}>
            <option value="Todos">Todos los autores</option>
            <option value="Cliente">Clientes</option>
            <option value="Nutricionista">Nutricionistas</option>
          </select>
          <div className="input-group" style={{ width: 260 }}>
            <span className="input-group-text"><Icon name="search" size={16} /></span>
            <input className="form-control" placeholder="Buscar producto o código…" value={q} onChange={(e) => setQ(e.target.value)} />
          </div>
        </div>
      </div>

      {list.length === 0 ? (
        <div className="nt-empty">
          <Icon name="box" size={28} />
          <div className="mt-2 fw-700">No hay productos en esta vista</div>
          <div style={{ fontSize: '.88rem' }}>Ajusta los filtros o la búsqueda.</div>
        </div>
      ) : (
        <div className="row g-3">
          {list.map((p) => (
            <div className="col-md-6 col-xl-4" key={p.barcode}>
              <ProductCard p={p} onApprove={(x) => moderate(x, 'Aprobado')} onReject={setRejecting} onDetail={setDetail} />
            </div>
          ))}
        </div>
      )}

      {rejecting && (
        <RejectModal
          product={rejecting}
          onClose={() => setRejecting(null)}
          onConfirm={() => { const p = rejecting; setRejecting(null); moderate(p, 'Rechazado'); }}
        />
      )}
      {detail && <DetailModal product={detail} onClose={() => setDetail(null)} />}
      {toast && <div className={'nt-toast' + (toast.warn ? ' warn' : '')}><Icon name={toast.warn ? 'x' : 'check'} size={16} /> {toast.message}</div>}
    </div>
  );
}
