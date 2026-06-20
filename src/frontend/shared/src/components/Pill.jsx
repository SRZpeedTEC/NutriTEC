// Manejar la etiqueta de estado por tono.

// Etiqueta compacta coloreada según el tono indicado.
export default function Pill({ tone = 'teal', children }) {
  return <span className={'nt-pill nt-pill-' + tone}>{children}</span>;
}
