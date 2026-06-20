// Manejar el inicio de sesión del nutricionista.

import { useState } from 'react';
import logo from '@nutritec/shared/assets/logo.png';
import { getNutritionistProfile } from '@nutritec/shared/services/profileService.js';

// Pantalla de acceso: valida credenciales y entrega la sesión al componente raíz.
export default function LoginPage({ onLogin, onGoRegister }) {
  const [email, setEmail] = useState('carlos.mendez@nutritec.cr');
  const [password, setPassword] = useState('123456');
  const [loading, setLoading] = useState(false);

  // Inicia sesión: en la fase mock siembra la sesión desde el perfil del nutricionista.
  async function submit(e) {
    e.preventDefault();
    setLoading(true);
    // MOCK_LOGIN — reemplazar por authService.login(email, password) al conectar el backend.
    const profile = await getNutritionistProfile();
    onLogin(profile);
  }

  return (
    <div className="nt-auth">
      <div className="nt-auth-card">
        <div className="nt-auth-brand">
          <img className="nt-brand-mark" src={logo} alt="NutriTEC" />
          <div className="nt-brand-name">Nutri<span>TEC</span></div>
        </div>
        <h2 className="fw-800 mb-1 text-center">Iniciar sesión</h2>
        <p className="text-muted-soft mb-4 text-center">Ingresa con tu correo y contraseña.</p>
        <form onSubmit={submit}>
          <div className="mb-3">
            <label className="form-label">Correo electrónico</label>
            <input type="email" className="form-control form-control-lg" value={email} onChange={(e) => setEmail(e.target.value)} placeholder="tucorreo@nutritec.cr" />
          </div>
          <div className="mb-4">
            <label className="form-label">Contraseña</label>
            <input type="password" className="form-control form-control-lg" value={password} onChange={(e) => setPassword(e.target.value)} placeholder="••••••••" />
          </div>
          <button type="submit" className="btn btn-primary btn-lg w-100 mb-3" disabled={loading}>Entrar</button>
        </form>
        <div className="text-center text-muted-soft" style={{ fontSize: '.92rem' }}>
          ¿No tienes cuenta?{' '}
          <a href="#" className="text-teal fw-800" style={{ textDecoration: 'none' }}
            onClick={(e) => { e.preventDefault(); onGoRegister(); }}>Regístrate</a>
        </div>
      </div>
    </div>
  );
}
