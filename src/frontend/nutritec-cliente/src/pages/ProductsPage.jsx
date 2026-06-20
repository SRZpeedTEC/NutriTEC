// Manejar los productos del cliente: envío para aprobación y catálogo aprobado.

import { useState, useEffect } from 'react';
import Icon from '@nutritec/shared/components/Icon.jsx';
import Pill from '@nutritec/shared/components/Pill.jsx';
import SectionTitle from '@nutritec/shared/components/SectionTitle.jsx';
import { getPendingProducts, createProduct, updateProduct, deleteProduct } from '@nutritec/shared/services/productService.js';
import { searchProducts } from '@nutritec/shared/services/dailyConsumeService.js';

const EMPTY_FORM = { barcode: '', name: '', portion: '', unit: 'g', energy: '', fat: '', sodium: '', carbs: '', protein: '', vitamins: '', calcium: '', iron: '' };

// Campos numéricos del formulario con su etiqueta y unidad.
const NUM_FIELDS = [
  { k: 'energy', l: 'Energía', u: 'Kcal' }, { k: 'fat', l: 'Grasa', u: 'g' }, { k: 'sodium', l: 'Sodio', u: 'mg' },
  { k: 'carbs', l: 'Carbohidratos', u: 'g' }, { k: 'protein', l: 'Proteína', u: 'g' },
  { k: 'calcium', l: 'Calcio', u: 'mg' }, { k: 'iron', l: 'Hierro', u: 'mg' },
];

// Convierte el formulario al producto que esperan los servicios (numéricos como número).
function formToProduct(f) {
  return {
    barcode: f.barcode, name: f.name, unit: f.unit, vitamins: f.vitamins,
    portion: Number(f.portion), energy: Number(f.energy), fat: Number(f.fat), sodium: Number(f.sodium),
    carbs: Number(f.carbs), protein: Number(f.protein), calcium: Number(f.calcium), iron: Number(f.iron),
  };
}

// Vista principal: mis productos enviados y el catálogo aprobado de la comunidad.
export default function ProductsPage({ userId }) {
  const [myProducts, setMyProducts] = useState([]);
  const [catalog, setCatalog] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  const [open, setOpen] = useState(false);
  const [editing, setEditing] = useState(null); // null | barcode original
  const [form, setForm] = useState(EMPTY_FORM);
  const [q, setQ] = useState('');
  const [toast, setToast] = useState(null);

  const set = (key, value) => setForm((prev) => ({ ...prev, [key]: value }));

  // Carga mis productos enviados y el catálogo aprobado.
  useEffect(() => {
    setLoading(true); setError(null);
    Promise.all([getPendingProducts(userId), searchProducts('')])
      .then(([mine, cat]) => { setMyProducts(mine); setCatalog(cat); })
      .catch((err) => setError(err.message))
      .finally(() => setLoading(false));
  }, [userId]);

  // Muestra un aviso flotante temporal.
  function flash(message, warn) {
    setToast({ message, warn });
    setTimeout(() => setToast(null), 2600);
  }

  // Recarga la lista de mis productos.
  async function reload() {
    setMyProducts(await getPendingProducts(userId));
  }

  function openNew() { setForm(EMPTY_FORM); setEditing(null); setOpen(true); }
  function openEdit(p) { setForm({ ...EMPTY_FORM, ...p }); setEditing(p.barcode); setOpen(true); }

  // Crea o actualiza un producto contra el backend y recarga la lista.
  async function submit(e) {
    e.preventDefault();
    const product = formToProduct(form);
    try {
      if (editing) await updateProduct(editing, product, userId);
      else await createProduct(product, userId);
      setOpen(false); setEditing(null); setForm(EMPTY_FORM);
      await reload();
      flash(editing ? 'Producto actualizado' : 'Producto enviado para aprobación');
    } catch (err) {
      flash(err.message || 'No se pudo guardar el producto', true);
    }
  }

  // Elimina un producto y recarga la lista.
  async function remove(p) {
    try {
      await deleteProduct(p.barcode, userId);
      await reload();
      flash('Producto eliminado');
    } catch (err) {
      flash(err.message || 'No se pudo eliminar el producto', true);
    }
  }

  if (loading) {
    return <div className="nt-content"><div className="text-center py-5"><div className="spinner-border text-teal" role="status" /></div></div>;
  }
  if (error) {
    return <div className="nt-content"><div className="alert alert-danger">{error}</div></div>;
  }

  const filtered = catalog.filter((p) => {
    const t = q.trim().toLowerCase();
    if (!t) return true;
    return p.name.toLowerCase().includes(t) || p.barcode.includes(t);
  });

  return (
    <div className="nt-content">
      <div className="d-flex justify-content-between align-items-center mb-3">
        <SectionTitle sub="Los productos nuevos quedan en espera de aprobación del administrador antes de estar disponibles para la comunidad">Mis productos</SectionTitle>
        <button className="btn btn-primary" onClick={open ? () => setOpen(false) : openNew}><Icon name="plus" size={16} /> Nuevo producto</button>
      </div>

      {open && (
        <div className="nt-card nt-card-pad mb-4">
          <h3 className="h6 fw-800 mb-3">{editing ? 'Editar producto' : 'Agregar producto / platillo'}</h3>
          <form onSubmit={submit}>
            <div className="row">
              <div className="mb-3 col-md-4">
                <label className="form-label">Código de barras</label>
                <div className="input-group"><span className="input-group-text"><Icon name="barcode" size={16} /></span>
                  <input className="form-control" required value={form.barcode} onChange={(e) => set('barcode', e.target.value)} placeholder="7441…" /></div>
              </div>
              <div className="mb-3 col-md-5">
                <label className="form-label">Descripción</label>
                <input className="form-control" required value={form.name} onChange={(e) => set('name', e.target.value)} placeholder="Nombre del producto" />
              </div>
              <div className="mb-3 col-md-3">
                <label className="form-label">Tamaño de porción</label>
                <div className="input-group">
                  <input type="number" className="form-control" value={form.portion} onChange={(e) => set('portion', e.target.value)} placeholder="100" />
                  <select className="form-select" style={{ maxWidth: 80 }} value={form.unit} onChange={(e) => set('unit', e.target.value)}><option>g</option><option>ml</option></select>
                </div>
              </div>
              {NUM_FIELDS.map((c) => (
                <div className="mb-3 col-6 col-md-3" key={c.k}>
                  <label className="form-label">{c.l}</label>
                  <div className="input-group">
                    <input type="number" step="0.1" className="form-control" value={form[c.k]} onChange={(e) => set(c.k, e.target.value)} placeholder="0" />
                    <span className="input-group-text">{c.u}</span>
                  </div>
                </div>
              ))}
              <div className="mb-3 col-md-6">
                <label className="form-label">Vitaminas</label>
                <input className="form-control" value={form.vitamins} onChange={(e) => set('vitamins', e.target.value)} placeholder="Ej. A, C, B12" />
              </div>
            </div>
            <div className="d-flex gap-2">
              <button type="submit" className="btn btn-primary px-4">{editing ? 'Guardar cambios' : 'Enviar para aprobación'}</button>
              <button type="button" className="btn btn-soft" onClick={() => { setOpen(false); setEditing(null); }}>Cancelar</button>
            </div>
          </form>
        </div>
      )}

      <div className="nt-card nt-card-pad mb-4">
        <div className="table-responsive">
          <table className="table align-middle mb-0">
            <thead><tr><th>Producto</th><th>Código</th><th>Porción</th><th>Energía</th><th>Estado</th><th></th></tr></thead>
            <tbody>
              {myProducts.map((p) => (
                <tr key={p.barcode}>
                  <td className="fw-700">{p.name}<div className="text-muted-soft fw-normal" style={{ fontSize: '.78rem' }}>{p.vitamins && 'Vit. ' + p.vitamins}</div></td>
                  <td>{p.barcode}</td>
                  <td>{p.portion}{p.unit}</td>
                  <td>{p.energy} kcal</td>
                  <td><Pill tone={p.status === 'Aprobado' ? 'teal' : 'gray'}>{p.status === 'Aprobado' ? <Icon name="check" size={13} /> : <Icon name="clock" size={13} />} {p.status}</Pill></td>
                  <td className="text-end">
                    <div className="d-flex gap-1 justify-content-end">
                      <button className="btn btn-soft btn-sm px-2 py-1" title="Editar" onClick={() => openEdit(p)}><Icon name="edit" size={14} /></button>
                      <button className="btn btn-soft btn-sm px-2 py-1" title="Eliminar" onClick={() => remove(p)}><Icon name="trash" size={14} /></button>
                    </div>
                  </td>
                </tr>
              ))}
              {myProducts.length === 0 && <tr><td colSpan="6"><div className="nt-empty">No has agregado productos.</div></td></tr>}
            </tbody>
          </table>
        </div>
      </div>

      <SectionTitle sub="Productos aprobados disponibles para registrar consumo y crear recetas">Catálogo aprobado</SectionTitle>
      <div className="nt-card nt-card-pad">
        <div className="input-group mb-3" style={{ maxWidth: 360 }}>
          <span className="input-group-text"><Icon name="search" size={16} /></span>
          <input className="form-control" placeholder="Buscar por nombre o código" value={q} onChange={(e) => setQ(e.target.value)} />
        </div>
        <div className="table-responsive">
          <table className="table align-middle mb-0">
            <thead><tr><th>Producto</th><th>Código</th><th>Porción</th><th>Energía</th><th>Carbs</th><th>Proteína</th><th>Grasa</th></tr></thead>
            <tbody>
              {filtered.map((p) => (
                <tr key={p.barcode}>
                  <td className="fw-700">{p.name}</td>
                  <td>{p.barcode}</td>
                  <td>{p.portion}{p.unit}</td>
                  <td>{p.energy} kcal</td>
                  <td>{p.carbs} g</td><td>{p.protein} g</td><td>{p.fat} g</td>
                </tr>
              ))}
              {filtered.length === 0 && <tr><td colSpan="7"><div className="nt-empty">Sin coincidencias.</div></td></tr>}
            </tbody>
          </table>
        </div>
      </div>

      {toast && <div className={'nt-toast' + (toast.warn ? ' warn' : '')}><Icon name={toast.warn ? 'x' : 'check'} size={16} /> {toast.message}</div>}
    </div>
  );
}
