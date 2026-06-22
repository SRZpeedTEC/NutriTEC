import { useState } from 'react';
import logo from '@nutritec/shared/assets/logo.png';
import { login } from '@nutritec/shared/services/authService.js';

export default function LoginPage({ onLogin, onGoRegister }) {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  async function submit(e) {
    e.preventDefault();
    setLoading(true);
    setError(null);
    try {
      const session = await login({ email, password });
      onLogin(session);
    } catch (err) {
      setError(err.message || 'Credenciales incorrectas');
    } finally {
      setLoading(false);
    }
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
        {error && <div className="alert alert-danger py-2 mb-3">{error}</div>}
        <form onSubmit={submit}>
          <div className="mb-3">
            <label className="form-label">Correo electrónico</label>
            <input type="email" className="form-control form-control-lg" value={email} onChange={(e) => setEmail(e.target.value)} placeholder="tucorreo@nutritec.cr" required />
          </div>
          <div className="mb-4">
            <label className="form-label">Contraseña</label>
            <input type="password" className="form-control form-control-lg" value={password} onChange={(e) => setPassword(e.target.value)} placeholder="••••••••" required />
          </div>
          <button type="submit" className="btn btn-primary btn-lg w-100 mb-3" disabled={loading}>
            {loading ? <span className="spinner-border spinner-border-sm me-2" /> : null}
            Entrar
          </button>
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
