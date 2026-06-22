// Manejar la sesión, la navegación y el ensamblado de las vistas del cliente.

import { useState, useEffect } from 'react';
import Sidebar from '@nutritec/shared/components/Sidebar.jsx';
import TopBar from '@nutritec/shared/components/TopBar.jsx';
import LoginPage from './pages/LoginPage.jsx';
import RegisterPage from './pages/RegisterPage.jsx';
import PlanPage from './pages/PlanPage.jsx';
import DailyConsumptionPage from './pages/DailyConsumptionPage.jsx';
import MeasurementsPage from './pages/MeasurementsPage.jsx';
import ProgressReportPage from './pages/ProgressReportPage.jsx';
import ProductsPage from './pages/ProductsPage.jsx';
import RecipesPage from './pages/RecipesPage.jsx';
import FeedbackPage from './pages/FeedbackPage.jsx';

// Clave bajo la que se persiste la sesión en el navegador.
const SESSION_KEY = 'nutritec_session';

// Menú lateral del rol: cada entrada es una sección navegable.
const NAV = [
  { key: 'plan', label: 'Mi plan', icon: 'plan' },
  { key: 'consumo', label: 'Registro diario', icon: 'plate' },
  { key: 'medidas', label: 'Registro de medidas', icon: 'ruler' },
  { key: 'reporte', label: 'Reporte de avance', icon: 'chart' },
  { key: 'productos', label: 'Productos', icon: 'box' },
  { key: 'recetas', label: 'Recetas', icon: 'chef' },
  { key: 'seguimiento', label: 'Retroalimentación', icon: 'chat' },
];

// Título y subtítulo del encabezado por sección.
const TITLES = {
  plan: ['Mi plan', 'Plan de alimentación asignado por tu nutricionista'],
  consumo: ['Registro diario de consumo', 'Registra lo que comes en cada tiempo de comida'],
  medidas: ['Registro de medidas', 'Lleva el control de tus medidas corporales'],
  reporte: ['Reporte de avance', 'Visualiza y exporta tu progreso en PDF'],
  productos: ['Gestión de productos', 'Agrega, edita y consulta productos / platillos'],
  recetas: ['Gestión de recetas', 'Crea recetas a partir de productos existentes'],
  seguimiento: ['Retroalimentación', 'Conversación con tu nutricionista'],
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
  const [screen, setScreen] = useState('plan');
  const [menuOpen, setMenuOpen] = useState(false);

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
  const user = { initials: session.initials, name: session.name, subtitle: session.email };
  const clientId = session.clientId;

  return (
    <div className="nt-app">
      <Sidebar
        items={NAV}
        current={screen}
        onNav={go}
        onLogout={() => setSession(null)}
        open={menuOpen}
        navLabel="Mi nutrición"
        user={user}
      />
      {menuOpen && (
        <div className="position-fixed top-0 start-0 w-100 h-100 d-lg-none" style={{ background: 'rgba(0,0,0,.3)', zIndex: 1040 }} onClick={() => setMenuOpen(false)} />
      )}
      <main className="nt-main">
        <TopBar title={title} subtitle={sub} onMenu={() => setMenuOpen(true)} />
        {screen === 'plan' && <PlanPage clientId={clientId} />}
        {screen === 'consumo' && <DailyConsumptionPage clientId={clientId} />}
        {screen === 'medidas' && <MeasurementsPage clientId={clientId} />}
        {screen === 'reporte' && <ProgressReportPage clientId={clientId} userName={session.name} />}
        {screen === 'productos' && <ProductsPage userId={clientId} />}
        {screen === 'recetas' && <RecipesPage clientId={clientId} />}
        {screen === 'seguimiento' && <FeedbackPage clientId={clientId} nutritionistCode={session.activePlan?.nutritionistCode ?? null} />}
      </main>
    </div>
  );
}
