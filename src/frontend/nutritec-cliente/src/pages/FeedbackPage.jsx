// Manejar la retroalimentación del cliente: conversación con su nutricionista.

import { useState, useEffect, useRef } from 'react';
import Icon from '@nutritec/shared/components/Icon.jsx';
import Pill from '@nutritec/shared/components/Pill.jsx';
import SectionTitle from '@nutritec/shared/components/SectionTitle.jsx';
import { getClientThread, sendClientMessage } from '@nutritec/shared/services/feedbackService.js';
import { formatDate } from '@nutritec/shared/utils/dates.js';

// Vista principal: carga el hilo del cliente y permite enviar mensajes.
export default function FeedbackPage({ clientId }) {
  const [thread, setThread] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [text, setText] = useState('');
  const [toast, setToast] = useState(null);
  const endRef = useRef(null);

  // Carga el hilo de retroalimentación del cliente.
  useEffect(() => {
    setLoading(true); setError(null);
    getClientThread(clientId)
      .then(setThread)
      .catch((err) => setError(err.message))
      .finally(() => setLoading(false));
  }, [clientId]);

  // Mantiene el hilo desplazado al final cuando llegan mensajes.
  useEffect(() => { if (endRef.current) endRef.current.scrollTop = endRef.current.scrollHeight; }, [thread.length]);

  // Envía un mensaje al nutricionista y recarga el hilo.
  async function send(e) {
    e.preventDefault();
    if (!text.trim()) return;
    try {
      await sendClientMessage(clientId, text.trim());
      setThread(await getClientThread(clientId));
      setText('');
    } catch (err) {
      setToast(err.message || 'No se pudo enviar el mensaje');
      setTimeout(() => setToast(null), 2600);
    }
  }

  if (loading) {
    return <div className="nt-content"><div className="text-center py-5"><div className="spinner-border text-teal" role="status" /></div></div>;
  }
  if (error) {
    return <div className="nt-content"><div className="alert alert-danger">{error}</div></div>;
  }

  return (
    <div className="nt-content">
      <div className="mx-auto" style={{ maxWidth: 720 }}>
        <div className="nt-card nt-card-pad d-flex flex-column" style={{ height: 'calc(100vh - 170px)', minHeight: 480 }}>
          <div className="d-flex align-items-center justify-content-between mb-1">
            <SectionTitle sub="Conversación con tu nutricionista">Retroalimentación</SectionTitle>
            <Pill tone="teal"><Icon name="chat" size={13} /> {thread.length}</Pill>
          </div>

          <div className="nt-thread" ref={endRef}>
            {thread.length === 0 && <div className="nt-empty">Aún no tienes retroalimentación de tu nutricionista.</div>}
            {thread.map((m, i) => {
              const mine = m.author === 'patient';
              return (
                <div key={i} className={'nt-msg' + (mine ? ' mine' : '')}>
                  <div className="nt-msg-bubble">
                    <div className="d-flex align-items-center justify-content-between gap-3 mb-1">
                      <span className="fw-700" style={{ fontSize: '.82rem' }}>{mine ? 'Tú' : m.name}</span>
                      <span style={{ fontSize: '.72rem', opacity: .7 }}>{formatDate(m.date)}</span>
                    </div>
                    <div style={{ fontSize: '.9rem', lineHeight: 1.5 }}>{m.text}</div>
                  </div>
                </div>
              );
            })}
          </div>

          <form onSubmit={send} className="nt-thread-compose">
            <textarea className="form-control" rows={2} placeholder="Escribe un mensaje para tu nutricionista…"
              value={text} onChange={(e) => setText(e.target.value)}
              onKeyDown={(e) => { if (e.key === 'Enter' && (e.metaKey || e.ctrlKey)) send(e); }} />
            <div className="d-flex justify-content-between align-items-center mt-2">
              <span className="text-muted-soft" style={{ fontSize: '.76rem' }}>Campo de prosa libre, sin límite de extensión.</span>
              <button type="submit" className="btn btn-primary px-3" disabled={!text.trim()}><Icon name="send" size={15} /> Enviar</button>
            </div>
          </form>
        </div>
      </div>
      {toast && <div className="nt-toast warn"><Icon name="x" size={16} /> {toast}</div>}
    </div>
  );
}
