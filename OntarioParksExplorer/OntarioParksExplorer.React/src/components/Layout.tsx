import { type ReactNode, useState } from 'react';
import { Link } from 'react-router-dom';
import '../styles/layout.css';

interface LayoutProps {
  children: ReactNode;
}

export default function Layout({ children }: LayoutProps) {
  const [menuOpen, setMenuOpen] = useState(false);

  const toggleMenu = () => setMenuOpen(!menuOpen);

  return (
    <div className="layout">
      <header className="header">
        <button className="hamburger" onClick={toggleMenu} aria-label="Toggle navigation menu" aria-expanded={menuOpen}>
          ☰
        </button>
        <h1 className="title">🌲 Ontario Parks Explorer</h1>
      </header>
      <nav className={`nav ${menuOpen ? 'open' : ''}`} role="navigation" aria-label="Main navigation">
        <Link to="/" onClick={() => setMenuOpen(false)}>Home</Link>
        <Link to="/parks" onClick={() => setMenuOpen(false)}>Browse Parks</Link>
        <Link to="/favorites" onClick={() => setMenuOpen(false)}>My Favorites</Link>
        <Link to="/map" onClick={() => setMenuOpen(false)}>Map View</Link>
        <Link to="/recommendations" onClick={() => setMenuOpen(false)}>AI Recommendations</Link>
        <Link to="/chat" onClick={() => setMenuOpen(false)}>AI Chat</Link>
      </nav>
      <main className="main" role="main">
        {children}
      </main>
    </div>
  );
}
