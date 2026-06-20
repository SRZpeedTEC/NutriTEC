// Manejar la gráfica de línea del avance de medidas.

// Dibuja la serie data[keyName] como una línea sobre una grilla simple.
export default function LineChart({ data, keyName, color = 'var(--nt-teal-600)' }) {
  const w = 640, h = 200, pad = 30;
  if (data.length < 2) return <div className="nt-empty">Se requieren al menos 2 registros de medidas para graficar.</div>;

  const vals = data.map((d) => d[keyName]);
  const min = Math.min(...vals), max = Math.max(...vals);
  const rng = (max - min) || 1;
  const x = (i) => pad + (i * (w - pad * 2)) / (data.length - 1);
  const y = (v) => h - pad - ((v - min) / rng) * (h - pad * 2);
  const pts = data.map((d, i) => `${x(i)},${y(d[keyName])}`).join(' ');

  return (
    <svg viewBox={`0 0 ${w} ${h}`} style={{ width: '100%', height: 'auto' }}>
      {[0, 0.5, 1].map((t, i) => (
        <line key={i} x1={pad} x2={w - pad} y1={pad + t * (h - pad * 2)} y2={pad + t * (h - pad * 2)} stroke="var(--nt-line)" strokeWidth="1" />
      ))}
      <polyline points={pts} fill="none" stroke={color} strokeWidth="2.5" strokeLinecap="round" strokeLinejoin="round" />
      {data.map((d, i) => (
        <circle key={i} cx={x(i)} cy={y(d[keyName])} r="3.5" fill="#fff" stroke={color} strokeWidth="2" />
      ))}
    </svg>
  );
}
