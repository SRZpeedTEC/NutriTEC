// Manejar el registro de un cliente (wizard de tres pasos).

import { useState } from 'react';
import logo from '@nutritec/shared/assets/logo.png';
import Field from '@nutritec/shared/components/Field.jsx';
import DatePicker from '@nutritec/shared/components/DatePicker.jsx';
import { getClientProfile } from '@nutritec/shared/services/profileService.js';

const EMPTY_FORM = {
  firstName: '', lastName: '', age: '', birthDate: '1994-09-12', country: 'Costa Rica',
  weight: '', bmi: '', waist: '', neck: '', hips: '', muscle: '', fat: '',
  calorieGoal: '', email: '', password: '',
};

const STEPS = ['Datos personales', 'Medidas corporales', 'Meta y cuenta'];
const COUNTRIES = ['Costa Rica', 'Panamá', 'Nicaragua', 'Guatemala', 'México', 'Colombia'];

// Pantalla de registro: recoge los datos en tres pasos y entrega la sesión.
export default function RegisterPage({ onDone, onGoLogin }) {
  const [step, setStep] = useState(0);
  const [form, setForm] = useState(EMPTY_FORM);
  const set = (key, value) => setForm((prev) => ({ ...prev, [key]: value }));

  // Crea la cuenta: en la fase mock siembra la sesión desde el perfil del cliente.
  async function finish() {
    // MOCK_REGISTER — reemplazar por authService.registerClient(form) al conectar el backend.
    const profile = await getClientProfile();
    onDone(profile);
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

        {step === 0 && (
          <div className="row">
            <Field label="Nombre" ph="María Fernanda" half value={form.firstName} onChange={(v) => set('firstName', v)} />
            <Field label="Apellidos" ph="Rojas Vargas" half value={form.lastName} onChange={(v) => set('lastName', v)} />
            <Field label="Edad" type="number" ph="31" half value={form.age} onChange={(v) => set('age', v)} />
            <div className="mb-3 col-md-6">
              <label className="form-label">Fecha de nacimiento</label>
              <DatePicker value={form.birthDate} onChange={(v) => set('birthDate', v)} showToday={false} placeholder="Selecciona tu fecha" />
            </div>
            <div className="mb-3 col-12">
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
            : <button className="btn btn-primary flex-fill" onClick={finish}>Crear mi cuenta</button>}
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
