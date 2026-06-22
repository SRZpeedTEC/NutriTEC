// Manejar el diálogo modal.

import { useEffect } from 'react';

// Diálogo centrado que cierra con Escape o clic en el fondo.
export default function Modal({ title, sub, onClose, children, footer, size = 'md' }) {
  useEffect(() => {
    function onKey(e) { if (e.key === 'Escape') onClose(); }
    document.addEventListener('keydown', onKey);
    return () => document.removeEventListener('keydown', onKey);
  }, [onClose]);

  const maxW = size === 'lg' ? 720 : size === 'sm' ? 420 : 560;
  return (
    <div className="nt-modal-backdrop" onMouseDown={onClose}>
      <div className="nt-modal" style={{ maxWidth: maxW }} onMouseDown={(e) => e.stopPropagation()}>
        <div className="nt-modal-head">
          <div>
            <h3 className="h5 fw-800 mb-0">{title}</h3>
            {sub && <div className="text-muted-soft" style={{ fontSize: '.88rem' }}>{sub}</div>}
          </div>
          <button className="nt-modal-x" onClick={onClose} aria-label="Cerrar">×</button>
        </div>
        <div className="nt-modal-body">{children}</div>
        {footer && <div className="nt-modal-foot">{footer}</div>}
      </div>
    </div>
  );
}
