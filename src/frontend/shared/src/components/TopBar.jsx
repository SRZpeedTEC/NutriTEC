// Manejar el encabezado superior de cada vista.

// Encabezado con título, subtítulo y botón de menú en móvil.
export default function TopBar({ title, subtitle, onMenu }) {
  return (
    <div className="nt-topbar">
      <div className="d-flex align-items-center gap-3">
        <button className="btn btn-soft d-lg-none px-2 py-1" onClick={onMenu}>☰</button>
        <div>
          <h1>{title}</h1>
          {subtitle && <div className="text-muted-soft" style={{ fontSize: '.88rem' }}>{subtitle}</div>}
        </div>
      </div>
    </div>
  );
}
