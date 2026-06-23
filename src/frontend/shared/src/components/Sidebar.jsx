// Manejar la barra lateral de navegación.

import Icon from './Icon.jsx';
import logo from '../assets/logo.png';

// Barra lateral con la marca, el menú del rol y los datos del usuario.
export default function Sidebar({ items, current, onNav, onLogout, open, role, navLabel = 'Menú', user, badges = {} }) {
  return (
    <aside className={'nt-sidebar' + (open ? ' open' : '')}>
      <div className="nt-brand">
        <img className="nt-brand-mark" src={logo} alt="NutriTEC" />
        <div>
          <div className="nt-brand-name">Nutri<span>TEC</span></div>
          {role && <div className="nt-brand-role">{role}</div>}
        </div>
      </div>
      <nav className="nt-nav">
        <div className="nt-nav-label">{navLabel}</div>
        {items.map((nv) => {
          const count = nv.badge ? badges[nv.badge] : 0;
          return (
            <button key={nv.key} className={'nt-nav-item' + (current === nv.key ? ' active' : '')} onClick={() => onNav(nv.key)}>
              <span className="ico"><Icon name={nv.icon} /></span>{nv.label}
              {count > 0 && <span className="nt-nav-badge">{count}</span>}
            </button>
          );
        })}
        <div className="nt-nav-label">Cuenta</div>
        <button className="nt-nav-item" onClick={onLogout}>
          <span className="ico"><Icon name="logout" /></span>Cerrar sesión
        </button>
      </nav>
      {user && (
        <div className="nt-side-user">
          {user.photo
            ? <img src={user.photo} alt={user.name} className="nt-avatar" style={{ objectFit: 'cover', padding: 0 }} />
            : <div className="nt-avatar">{user.initials}</div>}
          <div style={{ minWidth: 0 }}>
            <div className="fw-700 text-truncate" style={{ fontSize: '.9rem' }}>{user.name}</div>
            <div className="text-truncate text-muted-soft" style={{ fontSize: '.78rem' }}>{user.subtitle}</div>
          </div>
        </div>
      )}
    </aside>
  );
}
