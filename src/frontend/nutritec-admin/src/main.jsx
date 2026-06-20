// Punto de entrada de React: monta la app en el div#root del HTML.
import { createRoot } from 'react-dom/client';
import App from './App.jsx';
import './styles/main.css';

createRoot(document.getElementById('root')).render(<App />);
