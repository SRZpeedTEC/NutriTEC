// Manejar el avatar con iniciales.

// Círculo con las iniciales del usuario, escalable por tamaño.
export default function Avatar({ initials, size = 44 }) {
  return (
    <div className="nt-avatar" style={{ width: size, height: size, flexBasis: size, fontSize: size * 0.36 }}>
      {initials}
    </div>
  );
}
