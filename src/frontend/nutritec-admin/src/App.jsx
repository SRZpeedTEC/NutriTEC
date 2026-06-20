// Manejar la sesión, la navegación y el ensamblado de las vistas del administrador.

import { useState, useEffect } from 'react';
import Sidebar from '@nutritec/shared/components/Sidebar.jsx';
import TopBar from '@nutritec/shared/components/TopBar.jsx';
import LoginPage from './pages/LoginPage.jsx';
import ProductsPage from './pages/ProductsPage.jsx';
import CobroPage from './pages/CobroPage.jsx';

// Clave bajo la que se persiste la sesión en el navegador.
const SESSION_KEY = 'nutritec_session';

// Menú lateral del rol: cada entrada es una sección navegable.
const NAV = [
  { key: 'productos', label: 'Aprobación de productos', icon: 'box', badge: 'prod' },
  { key: 'cobro', label: 'Reporte de cobro', icon: 'card' },
];

// Título y subtítulo del encabezado por sección.
const TITLES = {
  productos: ['Aprobación de productos', 'Verifica y aprueba los productos de la comunidad'],
  cobro: ['Reporte de cobro', 'Cobro por nutricionista según su tipo de pago'],
};

// Componente raíz: decide entre login y panel, y orquesta qué vista se muestra.
export default function App() {
  const [session, setSession] = useState(() => {
    try {
      const saved = localStorage.getItem(SESSION_KEY);
      return saved ? JSON.parse(saved) : null;
    } catch { return null; }
  });
  const [screen, setScreen] = useState('productos');
  const [menuOpen, setMenuOpen] = useState(false);
  const [pendingCount, setPendingCount] = useState(0);

  // Persiste la sesión para sobrevivir reinicios; al cerrar sesión la borra.
  useEffect(() => {
    if (session) localStorage.setItem(SESSION_KEY, JSON.stringify(session));
    else localStorage.removeItem(SESSION_KEY);
  }, [session]);

  if (!session) return <LoginPage onLogin={setSession} />;

  const [title, sub] = TITLES[screen];
  const go = (key) => { setScreen(key); setMenuOpen(false); };
  const user = { initials: session.initials, name: session.name, subtitle: session.email };

  return (
    <div className="nt-app">
      <Sidebar
        items={NAV}
        current={screen}
        onNav={go}
        onLogout={() => setSession(null)}
        open={menuOpen}
        role="Administrador"
        navLabel="Administración"
        user={user}
        badges={{ prod: pendingCount }}
      />
      {menuOpen && (
        <div className="position-fixed top-0 start-0 w-100 h-100 d-lg-none" style={{ background: 'rgba(0,0,0,.3)', zIndex: 1040 }} onClick={() => setMenuOpen(false)} />
      )}
      <main className="nt-main">
        <TopBar title={title} subtitle={sub} onMenu={() => setMenuOpen(true)} />
        {screen === 'productos' && <ProductsPage userId={session.id} onPendingCount={setPendingCount} />}
        {screen === 'cobro' && <CobroPage />}
      </main>
    </div>
  );
}
