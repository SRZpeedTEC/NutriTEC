// Manejar las recetas del cliente: constructor con totales y listado de recetas.

import { useState, useEffect } from 'react';
import Icon from '@nutritec/shared/components/Icon.jsx';
import Pill from '@nutritec/shared/components/Pill.jsx';
import SectionTitle from '@nutritec/shared/components/SectionTitle.jsx';
import { getByClient, createRecipe, deleteRecipe } from '@nutritec/shared/services/recipeService.js';
import { searchProducts } from '@nutritec/shared/services/dailyConsumeService.js';
import { byBarcode, totals } from '@nutritec/shared/utils/nutrition.js';

// Vista principal: arma recetas a partir del catálogo y lista las del cliente.
export default function RecipesPage({ clientId }) {
  const [recipes, setRecipes] = useState([]);
  const [catalog, setCatalog] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  const [name, setName] = useState('');
  const [ingredients, setIngredients] = useState([]);
  const [ingredientQuery, setIngredientQuery] = useState('');
  const [searchResults, setSearchResults] = useState([]);
  const [searching, setSearching] = useState(false);
  const [toast, setToast] = useState(null);

  // Carga las recetas del cliente.
  useEffect(() => {
    setLoading(true); setError(null);
    getByClient(clientId)
      .then(setRecipes)
      .catch((err) => setError(err.message))
      .finally(() => setLoading(false));
  }, [clientId]);

  // Busca productos aprobados para agregarlos como ingredientes.
  useEffect(() => {
    const term = ingredientQuery.trim();
    if (term.length < 2) {
      setSearchResults([]);
      setSearching(false);
      return;
    }

    const timer = setTimeout(() => {
      setSearching(true);
      searchProducts(term)
        .then((items) => {
          setSearchResults(items);
          setCatalog((prev) => mergeProducts(prev, items));
        })
        .catch((err) => {
          setSearchResults([]);
          flash(err.message || 'No se pudo buscar ingredientes', true);
        })
        .finally(() => setSearching(false));
    }, 350);

    return () => clearTimeout(timer);
  }, [ingredientQuery]);

  // Muestra un aviso flotante temporal.
  function flash(message, warn) {
    setToast({ message, warn });
    setTimeout(() => setToast(null), 2600);
  }

  function mergeProducts(current, incoming) {
    const byCode = new Map(current.map((p) => [p.barcode, p]));
    incoming.forEach((p) => byCode.set(p.barcode, p));
    return Array.from(byCode.values());
  }

  const addIngredient = (product) => {
    setCatalog((prev) => mergeProducts(prev, [product]));
    setIngredients((prev) => prev.some((i) => i.barcode === product.barcode)
      ? prev
      : [...prev, { barcode: product.barcode, portions: 1 }]);
  };
  const setPortions = (bc, v) => setIngredients((p) => p.map((i) => i.barcode === bc ? { ...i, portions: parseFloat(v) || 0 } : i));
  const removeIngredient = (bc) => setIngredients((p) => p.filter((i) => i.barcode !== bc));

  // Guarda la receta contra el backend y recarga el listado.
  async function save() {
    if (!name.trim() || ingredients.length === 0) return;
    try {
      await createRecipe({ clientId, name: name.trim(), ingredients });
      setRecipes(await getByClient(clientId));
      setName(''); setIngredients([]);
      flash('Receta guardada');
    } catch (err) {
      flash(err.message || 'No se pudo guardar la receta', true);
    }
  }

  // Elimina una receta y recarga el listado.
  async function remove(recipe) {
    try {
      await deleteRecipe(recipe.recipeId, clientId);
      setRecipes(await getByClient(clientId));
      flash('Receta eliminada');
    } catch (err) {
      flash(err.message || 'No se pudo eliminar la receta', true);
    }
  }

  if (loading) {
    return <div className="nt-content"><div className="text-center py-5"><div className="spinner-border text-teal" role="status" /></div></div>;
  }
  if (error) {
    return <div className="nt-content"><div className="alert alert-danger">{error}</div></div>;
  }

  const tot = totals(ingredients, catalog);

  return (
    <div className="nt-content">
      <SectionTitle sub="Combina productos existentes; el sistema totaliza la información nutricional">Gestión de recetas</SectionTitle>
      <div className="row g-4">
        <div className="col-lg-7">
          <div className="nt-card nt-card-pad">
            <div className="mb-3">
              <label className="form-label">Nombre de la receta</label>
              <input className="form-control form-control-lg" value={name} onChange={(e) => setName(e.target.value)} placeholder="Ej. Gallo Pinto" />
            </div>
            <label className="form-label">Agregar ingrediente</label>
            <div className="input-group mb-2">
              <span className="input-group-text"><Icon name="search" size={16} /></span>
              <input className="form-control" value={ingredientQuery} onChange={(e) => setIngredientQuery(e.target.value)} placeholder="Buscar por nombre o código" />
              {searching && <span className="input-group-text"><span className="spinner-border spinner-border-sm" /></span>}
            </div>
            <div className="d-flex flex-column gap-2 mb-3">
              {ingredientQuery.trim().length === 1 && (
                <div className="nt-empty">Escribe al menos 2 caracteres para buscar ingredientes.</div>
              )}
              {ingredientQuery.trim().length >= 2 && !searching && searchResults.length === 0 && (
                <div className="nt-empty">Sin ingredientes aprobados para «{ingredientQuery}».</div>
              )}
              {searchResults.map((p) => (
                <div key={p.barcode} className="d-flex align-items-center justify-content-between p-2 rounded-3" style={{ border: '1px solid var(--nt-line)' }}>
                  <div className="min-w-0">
                    <div className="fw-700 text-truncate">{p.name}</div>
                    <div className="text-muted-soft" style={{ fontSize: '.8rem' }}>{p.portion}{p.unit} · {p.energy} kcal · {p.barcode}</div>
                  </div>
                  <button className="btn btn-primary btn-sm px-3" onClick={() => addIngredient(p)}><Icon name="plus" size={15} /></button>
                </div>
              ))}
            </div>

            {ingredients.length === 0 ? <div className="nt-empty">Agrega productos para crear tu receta.</div> : (
              <table className="table align-middle mb-0">
                <thead><tr><th>Producto</th><th style={{ width: 120 }}>Porciones</th><th style={{ width: 90 }}>Kcal</th><th style={{ width: 48 }} /></tr></thead>
                <tbody>
                  {ingredients.map((it) => {
                    const p = byBarcode(catalog, it.barcode);
                    if (!p) return null;
                    return (
                      <tr key={it.barcode}>
                        <td className="fw-700">{p.name}</td>
                        <td><input type="number" step="0.5" min="0" className="form-control form-control-sm" value={it.portions} onChange={(e) => setPortions(it.barcode, e.target.value)} /></td>
                        <td className="fw-700">{Math.round(p.energy * it.portions)}</td>
                        <td><button className="btn btn-soft btn-sm" onClick={() => removeIngredient(it.barcode)}><Icon name="trash" size={15} /></button></td>
                      </tr>
                    );
                  })}
                </tbody>
              </table>
            )}
            <button className="btn btn-primary mt-3" onClick={save} disabled={!name.trim() || ingredients.length === 0}><Icon name="check" size={16} /> Guardar receta</button>
          </div>
        </div>

        <div className="col-lg-5">
          <div className="nt-card nt-card-pad" style={{ background: 'var(--nt-teal-50)' }}>
            <div className="mb-3">
              <div className="fw-800" style={{ fontSize: '1.15rem' }}>{name || 'Nueva receta'}</div>
              <div className="text-muted-soft" style={{ fontSize: '.82rem' }}>{ingredients.length} ingredientes</div>
            </div>
            <div className="nt-card nt-card-pad" style={{ borderRadius: 12 }}>
              <div className="text-center mb-3">
                <div className="fw-800 text-teal" style={{ fontSize: '2.3rem', lineHeight: 1 }}>{Math.round(tot.energy)}</div>
                <div className="text-muted-soft fw-700">Kcal totales</div>
              </div>
              <div className="row g-2 text-center">
                {[['Carbohid.', tot.carbs, 'g'], ['Proteína', tot.protein, 'g'], ['Grasa', tot.fat, 'g'], ['Sodio', tot.sodium, 'mg'], ['Calcio', tot.calcium, 'mg'], ['Hierro', tot.iron, 'mg']].map((m, i) => (
                  <div className="col-4" key={i}>
                    <div className="p-2 rounded-3" style={{ background: 'var(--nt-teal-50)' }}>
                      <div className="fw-800">{Math.round(m[1] * 10) / 10}</div>
                      <div className="text-muted-soft" style={{ fontSize: '.72rem' }}>{m[0]} ({m[2]})</div>
                    </div>
                  </div>
                ))}
              </div>
            </div>
          </div>

          <div className="nt-card nt-card-pad mt-3">
            <div className="fw-800 mb-2">Mis recetas</div>
            {recipes.length === 0 && <div className="text-muted-soft" style={{ fontSize: '.88rem' }}>Aún no tienes recetas.</div>}
            {recipes.map((r, i) => (
              <div key={r.recipeId ?? r.name ?? i} className="d-flex justify-content-between align-items-center py-2" style={{ borderTop: i ? '1px solid var(--nt-line)' : 'none' }}>
                <div>
                  <span className="fw-700">{r.name}</span>
                  <span className="text-muted-soft ms-2" style={{ fontSize: '.8rem' }}>{r.ingredientCount ?? r.ingredients?.length ?? 0} ingr.</span>
                </div>
                <div className="d-flex align-items-center gap-2">
                  {r.status && <Pill tone={r.status === 'Aprobada' ? 'teal' : 'gray'}>{r.status === 'Aprobada' ? <Icon name="check" size={13} /> : <Icon name="clock" size={13} />} {r.status}</Pill>}
                  <button className="btn btn-soft btn-sm px-2 py-1" title="Eliminar" onClick={() => remove(r)}><Icon name="trash" size={13} /></button>
                </div>
              </div>
            ))}
          </div>
        </div>
      </div>

      {toast && <div className={'nt-toast' + (toast.warn ? ' warn' : '')}><Icon name={toast.warn ? 'x' : 'check'} size={16} /> {toast.message}</div>}
    </div>
  );
}
