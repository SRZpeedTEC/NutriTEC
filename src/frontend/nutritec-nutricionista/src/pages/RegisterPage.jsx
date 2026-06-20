// Manejar el registro de un nutricionista (wizard de tres pasos).

import { useState } from 'react';
import logo from '@nutritec/shared/assets/logo.png';
import Icon from '@nutritec/shared/components/Icon.jsx';
import Field from '@nutritec/shared/components/Field.jsx';
import DatePicker from '@nutritec/shared/components/DatePicker.jsx';
import { getNutritionistProfile } from '@nutritec/shared/services/profileService.js';

// Slot para subir la foto de perfil (presentacional).
function PhotoUpload() {
  return (
    <div className="d-flex align-items-center gap-3 mb-3">
      <div className="nt-photo-slot"><Icon name="user" size={28} /></div>
      <div>
        <div className="fw-700" style={{ fontSize: '.95rem' }}>Foto de perfil</div>
        <div className="text-muted-soft mb-2" style={{ fontSize: '.82rem' }}>JPG o PNG, máx. 2 MB (opcional)</div>
        <button type="button" className="btn btn-soft btn-sm">Subir foto</button>
      </div>
    </div>
  );
}

// Opción seleccionable de tipo de cobro.
function CobroOption({ value, current, onSelect, title, price, detail }) {
  const active = value === current;
  return (
    <button type="button" className={'nt-cobro-opt' + (active ? ' active' : '')} onClick={() => onSelect(value)}>
      <span className={'nt-radio' + (active ? ' on' : '')}>{active && <Icon name="check" size={12} />}</span>
      <div className="flex-fill text-start">
        <div className="fw-700">{title}</div>
        <div className="text-muted-soft" style={{ fontSize: '.8rem' }}>{detail}</div>
      </div>
      <div className="text-end">
        <div className="fw-800 text-teal">{price}</div>
        <div className="text-muted-soft" style={{ fontSize: '.72rem' }}>por paciente</div>
      </div>
    </button>
  );
}

const EMPTY_FORM = {
  idNumber: '', firstName: '', lastName: '', age: '', birthDate: '1988-02-14',
  weight: '', bmi: '', address: '', code: '', email: '', password: '',
  card: '', cardExpiry: '', cvv: '', billingType: 'Mensual',
};

const STEPS = ['Datos personales', 'Credenciales', 'Método de cobro'];

// Pantalla de registro: recoge los datos en tres pasos y entrega la sesión.
export default function RegisterPage({ onDone, onGoLogin }) {
  const [step, setStep] = useState(0);
  const [form, setForm] = useState(EMPTY_FORM);
  const set = (key, value) => setForm((prev) => ({ ...prev, [key]: value }));

  // Crea la cuenta: en la fase mock siembra la sesión desde el perfil del nutricionista.
  async function finish() {
    // MOCK_REGISTER — reemplazar por authService.registerNutritionist(form) al conectar el backend.
    const profile = await getNutritionistProfile();
    onDone(profile);
  }

  return (
    <div className="nt-auth">
      <div className="nt-auth-card" style={{ maxWidth: 560 }}>
        <div className="nt-auth-brand">
          <img className="nt-brand-mark" src={logo} alt="NutriTEC" />
          <div className="nt-brand-name">Nutri<span>TEC</span></div>
        </div>
        <h2 className="fw-800 mb-1 text-center">Registro de nutricionista</h2>
        <p className="text-muted-soft mb-3 text-center">Paso {step + 1} de 3 · {STEPS[step]}</p>
        <div className="nt-steps mb-4">
          {STEPS.map((s, i) => <div key={i} className={'nt-step ' + (i < step ? 'done' : i === step ? 'active' : '')} />)}
        </div>

        {step === 0 && (
          <>
            <PhotoUpload />
            <div className="row">
              <Field label="Número de cédula" ph="1-1234-5678" icon="card" col="col-12" value={form.idNumber} onChange={(v) => set('idNumber', v)} />
              <Field label="Nombre" ph="Carlos" half value={form.firstName} onChange={(v) => set('firstName', v)} />
              <Field label="Apellidos" ph="Méndez Quirós" half value={form.lastName} onChange={(v) => set('lastName', v)} />
              <Field label="Edad" type="number" ph="38" half value={form.age} onChange={(v) => set('age', v)} />
              <div className="mb-3 col-md-6">
                <label className="form-label">Fecha de nacimiento</label>
                <DatePicker value={form.birthDate} onChange={(v) => set('birthDate', v)} showToday={false} placeholder="Selecciona tu fecha" />
              </div>
              <Field label="Peso" type="number" ph="79" suffix="kg" half value={form.weight} onChange={(v) => set('weight', v)} />
              <Field label="IMC" type="number" ph="24.6" half value={form.bmi} onChange={(v) => set('bmi', v)} />
              <Field label="Dirección de oficina" ph="San Pedro, Montes de Oca, San José" col="col-12" value={form.address} onChange={(v) => set('address', v)} />
            </div>
          </>
        )}

        {step === 1 && (
          <div className="row">
            <Field label="Código de nutricionista" ph="NUT-0421" icon="user" col="col-12" value={form.code} onChange={(v) => set('code', v)} />
            <Field label="Correo electrónico" type="email" ph="carlos.mendez@nutritec.cr" col="col-12" value={form.email} onChange={(v) => set('email', v)} />
            <Field label="Contraseña" type="password" ph="••••••••" col="col-12" value={form.password} onChange={(v) => set('password', v)} />
            <div className="col-12">
              <div className="nt-note mt-1">
                <Icon name="check" size={15} />
                <span>Tu contraseña se almacenará <strong>encriptada</strong>.</span>
              </div>
            </div>
          </div>
        )}

        {step === 2 && (
          <>
            <div className="row">
              <Field label="Número de tarjeta de crédito" ph="4821 1234 5678 4821" icon="card" col="col-12" value={form.card} onChange={(v) => set('card', v)} />
              <Field label="Vence" ph="08/29" half value={form.cardExpiry} onChange={(v) => set('cardExpiry', v)} />
              <Field label="CVV" ph="•••" half value={form.cvv} onChange={(v) => set('cvv', v)} />
            </div>
            <label className="form-label">Tipo de cobro de la mensualidad</label>
            <div className="d-flex flex-column gap-2 mb-2">
              <CobroOption value="Semanal" current={form.billingType} onSelect={(v) => set('billingType', v)} title="Semanal" price="$1" detail="Cobro semanal por cada paciente asociado" />
              <CobroOption value="Mensual" current={form.billingType} onSelect={(v) => set('billingType', v)} title="Mensual" price="$4" detail="Cobro mensual · 5% de descuento sobre el total" />
              <CobroOption value="Anual" current={form.billingType} onSelect={(v) => set('billingType', v)} title="Anual" price="$52" detail="Cobro anual · 10% de descuento sobre el total" />
            </div>
          </>
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
