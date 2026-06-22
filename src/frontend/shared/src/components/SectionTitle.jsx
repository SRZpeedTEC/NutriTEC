// Manejar el título de una sección.

// Título de sección con subtítulo opcional.
export default function SectionTitle({ children, sub }) {
  return (
    <div className="mb-3">
      <h2 className="h5 mb-0 fw-800">{children}</h2>
      {sub && <div className="text-muted-soft" style={{ fontSize: '.9rem' }}>{sub}</div>}
    </div>
  );
}
