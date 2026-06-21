import { useState } from 'react';
import logo from '@nutritec/shared/assets/logo.png';
import Icon from '@nutritec/shared/components/Icon.jsx';
import Field from '@nutritec/shared/components/Field.jsx';
import DatePicker from '@nutritec/shared/components/DatePicker.jsx';
import { registerNutritionist } from '@nutritec/shared/services/authService.js';

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
  weight: '', bmi: '', address: '', email: '', password: '',
  card: '', paymentMethod: 'CARD',
};

const STEPS = ['Datos personales', 'Credenciales', 'Método de cobro'];

const PAYMENT_LABELS = {
  CARD: { title: 'Tarjeta de crédito', price: 'Mensual', detail: 'Débito automático mensual a tu tarjeta' },
  SINPE: { title: 'SINPE Móvil', price: 'Mensual', detail: 'Transferencia SINPE mensual' },
  TRANSFER: { title: 'Transferencia', price: 'Mensual', detail: 'Transferencia bancaria mensual' },
  CASH: { title: 'Efectivo', price: 'Mensual', detail: 'Pago en efectivo mensual' },
  PAYPAL: { title: 'PayPal', price: 'Mensual', detail: 'Débito automático mensual vía PayPal' },
};

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
        email: form.email,
        password: form.password,
        idNumber: form.idNumber,
        weight: Number(form.weight) || 0,
        bodyMassIndex: Number(form.bmi) || 0,
        address: form.address,
        photo: null,
        encryptedCreditCard: form.card,
        paymentMethod: form.paymentMethod,
      };
      const session = await registerNutritionist(payload);
      onDone(session);
    } catch (err) {
      setError(err.message || 'No se pudo crear la cuenta');
    } finally {
      setLoading(false);
    }
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

        {error && <div className="alert alert-danger py-2 mb-3">{error}</div>}

        {step === 0 && (
          <>
            <PhotoUpload />
            <div className="row">
              <Field label="Número de cédula" ph="1-1234-5678" icon="card" col="col-12" value={form.idNumber} onChange={(v) => set('idNumber', v)} />
              <Field label="Nombre" ph="Carlos" half value={form.firstName} onChange={(v) => set('firstName', v)} />
              <Field label="Apellidos" ph="Méndez Quirós" half value={form.lastName} onChange={(v) => set('lastName', v)} />
              <Field label="Peso" type="number" ph="79" suffix="kg" half value={form.weight} onChange={(v) => set('weight', v)} />
              <Field label="IMC" type="number" ph="24.6" half value={form.bmi} onChange={(v) => set('bmi', v)} />
              <div className="mb-3 col-md-6">
                <label className="form-label">Fecha de nacimiento</label>
                <DatePicker value={form.birthDate} onChange={(v) => set('birthDate', v)} showToday={false} placeholder="Selecciona tu fecha" />
              </div>
              <Field label="Dirección de oficina" ph="San Pedro, Montes de Oca, San José" col="col-md-6" value={form.address} onChange={(v) => set('address', v)} />
            </div>
          </>
        )}

        {step === 1 && (
          <div className="row">
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
            <Field label="Número de tarjeta de crédito (opcional)" ph="4821 1234 5678 4821" icon="card" col="col-12" value={form.card} onChange={(v) => set('card', v)} />
            <label className="form-label mt-2">Método de cobro de la suscripción</label>
            <div className="d-flex flex-column gap-2 mb-2">
              {Object.entries(PAYMENT_LABELS).map(([k, v]) => (
                <CobroOption key={k} value={k} current={form.paymentMethod} onSelect={(val) => set('paymentMethod', val)}
                  title={v.title} price={v.price} detail={v.detail} />
              ))}
            </div>
          </>
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
