import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { MapContainer, TileLayer, Marker, Popup } from 'react-leaflet';
import { getParks } from '../services/api';
import { ParkListDto } from '../types';
import 'leaflet/dist/leaflet.css';
import L from 'leaflet';

// Fix for default marker icons in react-leaflet
import icon from 'leaflet/dist/images/marker-icon.png';
import iconShadow from 'leaflet/dist/images/marker-shadow.png';

let DefaultIcon = L.icon({
  iconUrl: icon,
  shadowUrl: iconShadow,
  iconSize: [25, 41],
  iconAnchor: [12, 41]
});

L.Marker.prototype.options.icon = DefaultIcon;

export default function Map() {
  const [parks, setParks] = useState<ParkListDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const navigate = useNavigate();

  useEffect(() => {
    loadAllParks();
  }, []);

  const loadAllParks = async () => {
    setLoading(true);
    setError(null);
    try {
      // Load first page to get total count
      const firstPage = await getParks(1, 100);
      const totalPages = firstPage.totalPages;
      
      // If there are more pages, load them all
      if (totalPages > 1) {
        const remainingPages = [];
        for (let page = 2; page <= totalPages; page++) {
          remainingPages.push(getParks(page, 100));
        }
        const results = await Promise.all(remainingPages);
        const allParks = [
          ...firstPage.items,
          ...results.flatMap(r => r.items)
        ];
        setParks(allParks);
      } else {
        setParks(firstPage.items);
      }
    } catch (err) {
      setError('Failed to load parks for map. Please try again.');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return (
      <div className="page">
        <h2>Map View</h2>
        <div className="loading">Loading map...</div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="page">
        <h2>Map View</h2>
        <div className="error-message">{error}</div>
      </div>
    );
  }

  return (
    <div className="page map-page">
      <h2>Ontario Parks Map</h2>
      <p className="subtitle">Showing {parks.length} parks</p>
      
      <div className="map-container" role="region" aria-label="Interactive map showing locations of Ontario parks">
        <MapContainer 
          center={[50.0, -85.0]} 
          zoom={5} 
          style={{ height: '600px', width: '100%' }}
          aria-label="Interactive map of Ontario parks"
        >
          <TileLayer
            attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
            url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
          />
          {parks.map(park => (
            <Marker 
              key={park.id} 
              position={[park.latitude, park.longitude]}
              aria-label={`Marker for ${park.name}`}
            >
              <Popup>
                <div className="map-popup">
                  <h3>{park.name}</h3>
                  <p>{park.region}</p>
                  {park.isFeatured && <span className="featured-badge">Featured</span>}
                  <button 
                    className="btn small"
                    onClick={() => navigate(`/parks/${park.id}`)}
                  >
                    View Details
                  </button>
                </div>
              </Popup>
            </Marker>
          ))}
        </MapContainer>
        <p className="visually-hidden">An interactive map displaying {parks.length} Ontario parks. Each park is represented by a marker showing its location. Click markers to view park details or use the Browse Parks page for keyboard-accessible list view.</p>
      </div>
    </div>
  );
}
