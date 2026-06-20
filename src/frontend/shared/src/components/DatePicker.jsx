// Manejar el selector de fecha con calendario.

import { useState, useEffect, useRef } from 'react';
import Icon from './Icon.jsx';
import { parseDate, formatDate, toISODate, MONTHS, MONTHS_SHORT, WEEKDAYS } from '../utils/dates.js';

// Campo de fecha con calendario emergente y atajo a hoy.
export default function DatePicker({ value, onChange, showToday = true, placeholder = 'Seleccionar fecha' }) {
  const [open, setOpen] = useState(false);
  const sel = parseDate(value);
  const [view, setView] = useState(() => sel || new Date());
  const ref = useRef(null);

  // Reposiciona el calendario al mes de la fecha seleccionada.
  useEffect(() => { if (sel) setView(sel); }, [value]);

  // Cierra el calendario al hacer clic fuera.
  useEffect(() => {
    function onDoc(e) { if (ref.current && !ref.current.contains(e.target)) setOpen(false); }
    if (open) document.addEventListener('mousedown', onDoc);
    return () => document.removeEventListener('mousedown', onDoc);
  }, [open]);

  const today = new Date();
  const todayISO = toISODate(today);
  const y = view.getFullYear(), m = view.getMonth();
  const startDow = (new Date(y, m, 1).getDay() + 6) % 7;
  const last = new Date(y, m + 1, 0).getDate();
  const cells = [];
  for (let i = 0; i < startDow; i++) cells.push(null);
  for (let d = 1; d <= last; d++) cells.push(new Date(y, m, d));
  while (cells.length % 7 !== 0) cells.push(null);

  return (
    <div className="nt-dp" ref={ref}>
      <div className="d-flex gap-2">
        <button type="button" className={'nt-dp-trigger form-control d-flex align-items-center justify-content-between' + (open ? ' focus' : '')} onClick={() => setOpen((o) => !o)}>
          <span className={sel ? '' : 'text-muted-soft'}>{sel ? formatDate(value) : placeholder}</span>
          <Icon name="cal" size={16} />
        </button>
        {showToday && (
          <button type="button" className="btn btn-soft px-3 text-nowrap d-flex align-items-center gap-1" title="Usar la fecha de hoy" onClick={() => onChange(todayISO)}>
            <Icon name="cal" size={15} /> Hoy
          </button>
        )}
      </div>
      {open && (
        <div className="nt-dp-pop">
          <div className="nt-dp-head">
            <button type="button" className="nt-dp-nav" onClick={() => setView(new Date(y, m - 1, 1))}>‹</button>
            <div className="nt-dp-title">{MONTHS[m]} {y}</div>
            <button type="button" className="nt-dp-nav" onClick={() => setView(new Date(y, m + 1, 1))}>›</button>
          </div>
          <div className="nt-dp-grid nt-dp-dow">
            {WEEKDAYS.map((d) => <div key={d} className="nt-dp-dowcell">{d}</div>)}
          </div>
          <div className="nt-dp-grid">
            {cells.map((d, i) => {
              if (!d) return <div key={i} />;
              const iso = toISODate(d);
              const isSel = value === iso, isToday = todayISO === iso;
              return (
                <button type="button" key={i} className={'nt-dp-day' + (isSel ? ' sel' : '') + (isToday && !isSel ? ' today' : '')}
                  onClick={() => { onChange(iso); setOpen(false); }}>{d.getDate()}</button>
              );
            })}
          </div>
          <div className="nt-dp-foot">
            <button type="button" className="nt-dp-todaybtn" onClick={() => { onChange(todayISO); setView(today); setOpen(false); }}>
              <Icon name="cal" size={14} /> Hoy · {today.getDate()} {MONTHS_SHORT[today.getMonth()]} {today.getFullYear()}
            </button>
          </div>
        </div>
      )}
    </div>
  );
}
