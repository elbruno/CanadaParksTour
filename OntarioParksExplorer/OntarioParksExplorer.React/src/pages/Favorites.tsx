import { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { getParkById } from '../services/api';
import { ParkDetailDto } from '../types';
import { useFavoritesContext } from '../context/FavoritesContext';

export default function Favorites() {
  const [parks, setParks] = useState<ParkDetailDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const { favorites, toggleFavorite } = useFavoritesContext();

  useEffect(() => {
    loadFavoriteParks();
  }, [favorites]);

  const loadFavoriteParks = async () => {
    setLoading(true);
    setError(null);
    
    if (favorites.size === 0) {
      setParks([]);
      setLoading(false);
      return;
    }

    try {
      const parkPromises = Array.from(favorites).map(id => getParkById(id));
      const parksData = await Promise.all(parkPromises);
      setParks(parksData);
    } catch (err) {
      setError('Failed to load favorite parks. Please try again.');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return (
      <div className="page">
        <h2>My Favorites</h2>
        <div className="loading">Loading your favorite parks...</div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="page">
        <h2>My Favorites</h2>
        <div className="error-message">{error}</div>
      </div>
    );
  }

  if (parks.length === 0) {
    return (
      <div className="page">
        <h2>My Favorites</h2>
        <div className="empty-favorites">
          <p>You haven't added any favorite parks yet.</p>
          <p>Browse parks and click the heart icon to add them to your favorites!</p>
          <Link to="/parks" className="btn">Browse Parks</Link>
        </div>
      </div>
    );
  }

  return (
    <div className="page">
      <h2>My Favorites</h2>
      <p className="subtitle">{parks.length} {parks.length === 1 ? 'park' : 'parks'} saved</p>
      
      <div className="parks-grid">
        {parks.map(park => (
          <div key={park.id} className="park-card">
            <Link to={`/parks/${park.id}`} className="park-card-link">
              <div className="park-image">
                {park.images.length > 0 && (
                  <img src={park.images[0].url} alt={park.images[0].altText} />
                )}
                {park.isFeatured && <span className="featured-badge">Featured</span>}
              </div>
              <div className="park-info">
                <h3>{park.name}</h3>
                <p className="region">{park.region}</p>
                <div className="activity-tags">
                  {park.activities.slice(0, 3).map(activity => (
                    <span key={activity.id} className="activity-tag">{activity.name}</span>
                  ))}
                  {park.activities.length > 3 && (
                    <span className="activity-tag">+{park.activities.length - 3}</span>
                  )}
                </div>
              </div>
            </Link>
            <button
              className="favorite-button active"
              onClick={(e) => {
                e.preventDefault();
                toggleFavorite(park.id);
              }}
              title="Remove from favorites"
            >
              ❤️
            </button>
          </div>
        ))}
      </div>
    </div>
  );
}
