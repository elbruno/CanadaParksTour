import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import { FavoritesProvider } from './context/FavoritesContext';
import Layout from './components/Layout';
import Home from './pages/Home';
import Parks from './pages/Parks';
import ParkDetail from './pages/ParkDetail';
import Favorites from './pages/Favorites';
import Map from './pages/Map';
import Recommendations from './pages/Recommendations';
import AiChat from './pages/AiChat';
import './styles/app.css';

function App() {
  return (
    <FavoritesProvider>
      <Router>
        <Layout>
          <Routes>
            <Route path="/" element={<Home />} />
            <Route path="/parks" element={<Parks />} />
            <Route path="/parks/:id" element={<ParkDetail />} />
            <Route path="/favorites" element={<Favorites />} />
            <Route path="/map" element={<Map />} />
            <Route path="/recommendations" element={<Recommendations />} />
            <Route path="/chat" element={<AiChat />} />
          </Routes>
        </Layout>
      </Router>
    </FavoritesProvider>
  );
}

export default App;
