// Manejar la sesión, la navegación y el ensamblado de las vistas del nutricionista.

import { useState, useEffect } from 'react';
import Sidebar from '@nutritec/shared/components/Sidebar.jsx';
import TopBar from '@nutritec/shared/components/TopBar.jsx';
import LoginPage from './pages/LoginPage.jsx';
import RegisterPage from './pages/RegisterPage.jsx';
import PatientsPage from './pages/PatientsPage.jsx';
import PlansPage from './pages/PlansPage.jsx';
import FollowupPage from './pages/FollowupPage.jsx';
import ProductsPage from './pages/ProductsPage.jsx';

// Clave bajo la que se persiste la sesión en el navegador.
const SESSION_KEY = 'nutritec_session';

// Menú lateral del rol: cada entrada es una sección navegable.
const NAV = [
  { key: 'pacientes', label: 'Pacientes', icon: 'users' },
  { key: 'planes', label: 'Planes de alimentación', icon: 'plate' },
  { key: 'seguimiento', label: 'Seguimiento', icon: 'chat' },
  { key: 'productos', label: 'Productos', icon: 'box' },
];

// Título y subtítulo del encabezado por sección.
const TITLES = {
  pacientes: ['Pacientes', 'Busca, asocia y gestiona a tus pacientes'],
  planes: ['Planes de alimentación', 'Crea y administra planes de 5 tiempos de comida'],
  seguimiento: ['Seguimiento', 'Revisa el registro diario, el avance y da retroalimentación'],
  productos: ['Gestión de productos', 'Agrega productos / platillos a la comunidad'],
};

// Componente raíz: decide entre login/registro y panel, y orquesta qué vista se muestra.
export default function App() {
  const [session, setSession] = useState(() => {
    try {
      const saved = localStorage.getItem(SESSION_KEY);
      return saved ? JSON.parse(saved) : null;
    } catch { return null; }
  });
  const [authView, setAuthView] = useState('login');
  const [screen, setScreen] = useState('pacientes');
  const [menuOpen, setMenuOpen] = useState(false);
  const [selectedPatient, setSelectedPatient] = useState(null);

  // Persiste la sesión para sobrevivir reinicios; al cerrar sesión la borra.
  useEffect(() => {
    if (session) localStorage.setItem(SESSION_KEY, JSON.stringify(session));
    else localStorage.removeItem(SESSION_KEY);
  }, [session]);

  if (!session) {
    return authView === 'register'
      ? <RegisterPage onDone={setSession} onGoLogin={() => setAuthView('login')} />
      : <LoginPage onLogin={setSession} onGoRegister={() => setAuthView('register')} />;
  }

  const [title, sub] = TITLES[screen];
  const go = (key) => { setScreen(key); setMenuOpen(false); };
  const openPatient = (id) => { setSelectedPatient(id); setScreen('seguimiento'); setMenuOpen(false); };
  const user = { initials: session.initials, name: session.name, subtitle: session.email, photo: session.photo };

  return (
    <div className="nt-app">
      <Sidebar
        items={NAV}
        current={screen}
        onNav={go}
        onLogout={() => setSession(null)}
        open={menuOpen}
        role="Nutricionista"
        navLabel="Mi consulta"
        user={user}
      />
      {menuOpen && (
        <div className="position-fixed top-0 start-0 w-100 h-100 d-lg-none" style={{ background: 'rgba(0,0,0,.3)', zIndex: 1040 }} onClick={() => setMenuOpen(false)} />
      )}
      <main className="nt-main">
        <TopBar title={title} subtitle={sub} onMenu={() => setMenuOpen(true)} />
        {screen === 'pacientes' && <PatientsPage nutritionistId={session.nutritionistCode} onOpenPatient={openPatient} />}
        {screen === 'planes' && <PlansPage nutritionistId={session.nutritionistCode} />}
        {screen === 'seguimiento' && <FollowupPage nutritionistId={session.nutritionistCode} initialPatient={selectedPatient} />}
        {screen === 'productos' && <ProductsPage userId={session.userId} />}
      </main>
    </div>
  );
}
