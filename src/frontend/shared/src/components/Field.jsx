// Manejar un campo de formulario controlado.

import Icon from './Icon.jsx';

// Campo con label, icono o sufijo opcional; controlado por value/onChange.
export default function Field({ label, type = 'text', ph, suffix, half, col, value, onChange, icon }) {
  const input = (
    <input type={type} className="form-control" placeholder={ph} value={value ?? ''} onChange={(e) => onChange?.(e.target.value)} />
  );
  return (
    <div className={'mb-3 ' + (col || (half ? 'col-md-6' : 'col-12'))}>
      <label className="form-label">{label}</label>
      {suffix || icon ? (
        <div className="input-group">
          {icon && <span className="input-group-text"><Icon name={icon} size={16} /></span>}
          {input}
          {suffix && <span className="input-group-text">{suffix}</span>}
        </div>
      ) : input}
    </div>
  );
}
