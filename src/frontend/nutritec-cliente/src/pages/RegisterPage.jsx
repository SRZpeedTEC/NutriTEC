import { useState } from 'react';
import logo from '@nutritec/shared/assets/logo.png';
import Field from '@nutritec/shared/components/Field.jsx';
import DatePicker from '@nutritec/shared/components/DatePicker.jsx';
import { registerClient } from '@nutritec/shared/services/authService.js';

const EMPTY_FORM = {
  firstName: '', lastName: '', age: '', birthDate: '1994-09-12', country: 'Costa Rica',
  weight: '', bmi: '', waist: '', neck: '', hips: '', muscle: '', fat: '',
  calorieGoal: '', email: '', password: '',
};

const STEPS = ['Datos personales', 'Medidas corporales', 'Meta y cuenta'];
const COUNTRIES = ['Costa Rica', 'Panamá', 'Nicaragua', 'Guatemala', 'México', 'Colombia'];

export default function RegisterPage({ onDone, onGoLogin }) {
  const [step, setStep] = useState(0);
  const [form, setForm] = useState(EMPTY_FORM);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const set = (key, value) => setForm((prev) => ({ ...prev, [key]: value }));

  async function finish() {
    setLoading(true);
    setError(null);
    try {
      const payload = {
        name: form.firstName,
        lastName: form.lastName,
        birthday: form.birthDate,
        country: form.country,
        bodyWeight: Number(form.weight) || 0,
        bodyMassIndex: Number(form.bmi) || 0,
        waist: Number(form.waist) || 0,
        neck: Number(form.neck) || 0,
        hip: Number(form.hips) || 0,
        musclePercentage: Number(form.muscle) || 0,
        fatPercentage: Number(form.fat) || 0,
        maxDailyCalories: Number(form.calorieGoal) || 0,
        email: form.email,
        password: form.password,
      };
      const session = await registerClient(payload);
      onDone(session);
    } catch (err) {
      setError(err.message || 'No se pudo crear la cuenta');
    } finally {
      setLoading(false);
    }
  }

  return (
    <div className="nt-auth">
      <div className="nt-auth-card" style={{ maxWidth: 540 }}>
        <div className="nt-auth-brand">
          <img className="nt-brand-mark" src={logo} alt="NutriTEC" />
          <div className="nt-brand-name">Nutri<span>TEC</span></div>
        </div>
        <h2 className="fw-800 mb-1 text-center">Crear cuenta</h2>
        <p className="text-muted-soft mb-3 text-center">Paso {step + 1} de 3 · {STEPS[step]}</p>
        <div className="nt-steps mb-4">
          {STEPS.map((s, i) => <div key={i} className={'nt-step ' + (i < step ? 'done' : i === step ? 'active' : '')} />)}
        </div>

        {error && <div className="alert alert-danger py-2 mb-3">{error}</div>}

        {step === 0 && (
          <div className="row">
            <Field label="Nombre" ph="María Fernanda" half value={form.firstName} onChange={(v) => set('firstName', v)} />
            <Field label="Apellidos" ph="Rojas Vargas" half value={form.lastName} onChange={(v) => set('lastName', v)} />
            <div className="mb-3 col-md-6">
              <label className="form-label">Fecha de nacimiento</label>
              <DatePicker value={form.birthDate} onChange={(v) => set('birthDate', v)} showToday={false} placeholder="Selecciona tu fecha" />
            </div>
            <div className="mb-3 col-md-6">
              <label className="form-label">País donde reside</label>
              <select className="form-select" value={form.country} onChange={(e) => set('country', e.target.value)}>
                {COUNTRIES.map((c) => <option key={c}>{c}</option>)}
              </select>
            </div>
          </div>
        )}

        {step === 1 && (
          <div className="row">
            <Field label="Peso actual" type="number" ph="73.2" suffix="kg" half value={form.weight} onChange={(v) => set('weight', v)} />
            <Field label="IMC" type="number" ph="24.1" half value={form.bmi} onChange={(v) => set('bmi', v)} />
            <Field label="Cintura" type="number" ph="80" suffix="cm" half value={form.waist} onChange={(v) => set('waist', v)} />
            <Field label="Cuello" type="number" ph="36.5" suffix="cm" half value={form.neck} onChange={(v) => set('neck', v)} />
            <Field label="Caderas" type="number" ph="96" suffix="cm" half value={form.hips} onChange={(v) => set('hips', v)} />
            <Field label="% de Músculo" type="number" ph="37.2" suffix="%" half value={form.muscle} onChange={(v) => set('muscle', v)} />
            <Field label="% de Grasa" type="number" ph="23.9" suffix="%" half value={form.fat} onChange={(v) => set('fat', v)} />
          </div>
        )}

        {step === 2 && (
          <div className="row">
            <Field label="Consumo diario máximo de calorías" type="number" ph="1800" suffix="kcal" col="col-12" value={form.calorieGoal} onChange={(v) => set('calorieGoal', v)} />
            <Field label="Correo electrónico" type="email" ph="maria.rojas@example.com" col="col-12" value={form.email} onChange={(v) => set('email', v)} />
            <Field label="Contraseña" type="password" ph="••••••••" col="col-12" value={form.password} onChange={(v) => set('password', v)} />
          </div>
        )}

        <div className="d-flex gap-2 mt-3">
          {step > 0 && <button className="btn btn-outline-primary px-4" onClick={() => setStep(step - 1)}>Atrás</button>}
          {step < 2
            ? <button className="btn btn-primary flex-fill" onClick={() => setStep(step + 1)}>Continuar</button>
            : <button className="btn btn-primary flex-fill" onClick={finish} disabled={loading}>
                {loading ? <span className="spinner-border spinner-border-sm me-2" /> : null}
                Crear mi cuenta
              </button>}
        </div>
        <div className="text-center text-muted-soft mt-4" style={{ fontSize: '.92rem' }}>
          ¿Ya tienes cuenta?{' '}
          <a href="#" className="text-teal fw-800" style={{ textDecoration: 'none' }}
            onClick={(e) => { e.preventDefault(); onGoLogin(); }}>Inicia sesión</a>
        </div>
      </div>
    </div>
  );
}
