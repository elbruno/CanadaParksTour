import { useState, useEffect } from 'react';
import { useParams, Link } from 'react-router-dom';
import { getParkById, generateSummary } from '../services/api';
import { ParkDetailDto } from '../types';
import { useFavoritesContext } from '../context/FavoritesContext';
import VisitPlanner from '../components/VisitPlanner';

export default function ParkDetail() {
  const { id } = useParams();
  const [park, setPark] = useState<ParkDetailDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [currentImageIndex, setCurrentImageIndex] = useState(0);
  const [aiSummary, setAiSummary] = useState<string | null>(null);
  const [generatingSummary, setGeneratingSummary] = useState(false);
  const [summaryError, setSummaryError] = useState<string | null>(null);
  const [showPlanner, setShowPlanner] = useState(false);
  const { isFavorite, toggleFavorite } = useFavoritesContext();

  useEffect(() => {
    if (id) {
      loadParkDetails(parseInt(id));
    }
  }, [id]);

  const loadParkDetails = async (parkId: number) => {
    setLoading(true);
    setError(null);
    try {
      const data = await getParkById(parkId);
      setPark(data);
    } catch (err) {
      setError('Failed to load park details. Please try again.');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const nextImage = () => {
    if (park && park.images.length > 0) {
      setCurrentImageIndex((prev) => (prev + 1) % park.images.length);
    }
  };

  const prevImage = () => {
    if (park && park.images.length > 0) {
      setCurrentImageIndex((prev) => (prev - 1 + park.images.length) % park.images.length);
    }
  };

  const handleGenerateSummary = async () => {
    if (!park) return;
    
    setGeneratingSummary(true);
    setSummaryError(null);
    try {
      const summary = await generateSummary(park.id);
      setAiSummary(summary);
    } catch (err) {
      setSummaryError('Failed to generate AI summary. Please try again.');
      console.error(err);
    } finally {
      setGeneratingSummary(false);
    }
  };

  if (loading) {
    return (
      <div className="page">
        <div className="loading">Loading park details...</div>
      </div>
    );
  }

  if (error || !park) {
    return (
      <div className="page">
        <div className="error-message">{error || 'Park not found'}</div>
        <Link to="/parks" className="btn">Back to Parks</Link>
      </div>
    );
  }

  return (
    <div className="page park-detail">
      <nav className="breadcrumb">
        <Link to="/parks">Parks</Link>
        <span> &gt; </span>
        <span>{park.name}</span>
      </nav>

      <div className="detail-header">
        <h1>{park.name}</h1>
        <button
          className={`favorite-button large ${isFavorite(park.id) ? 'active' : ''}`}
          onClick={() => toggleFavorite(park.id)}
          title={isFavorite(park.id) ? 'Remove from favorites' : 'Add to favorites'}
        >
          {isFavorite(park.id) ? '❤️' : '♡'}
        </button>
      </div>

      {park.isFeatured && <span className="featured-badge large">Featured Park</span>}

      {park.images.length > 0 && (
        <div className="image-gallery">
          <div className="main-image">
            <img 
              src={park.images[currentImageIndex].url} 
              alt={park.images[currentImageIndex].altText} 
            />
            {park.images.length > 1 && (
              <>
                <button className="gallery-btn prev" onClick={prevImage}>‹</button>
                <button className="gallery-btn next" onClick={nextImage}>›</button>
                <div className="image-counter">
                  {currentImageIndex + 1} / {park.images.length}
                </div>
              </>
            )}
          </div>
          {park.images.length > 1 && (
            <div className="thumbnail-strip">
              {park.images.map((image, index) => (
                <img
                  key={image.id}
                  src={image.url}
                  alt={image.altText}
                  className={index === currentImageIndex ? 'active' : ''}
                  onClick={() => setCurrentImageIndex(index)}
                />
              ))}
            </div>
          )}
        </div>
      )}

      <div className="detail-content">
        <div className="detail-section">
          <h2>About</h2>
          <p>{park.description}</p>
          
          {!aiSummary && (
            <button 
              className="btn btn-ai" 
              onClick={handleGenerateSummary}
              disabled={generatingSummary}
            >
              {generatingSummary ? '⏳ Generating...' : '✨ Generate AI Summary'}
            </button>
          )}
          
          {summaryError && <div className="error-message">{summaryError}</div>}
          
          {aiSummary && (
            <div className="ai-summary-card">
              <div className="ai-badge">🤖 AI Generated Summary</div>
              <p>{aiSummary}</p>
            </div>
          )}
        </div>

        <div className="detail-section">
          <h2>Location</h2>
          <p><strong>Address:</strong> {park.location}</p>
          <p><strong>Region:</strong> {park.region}</p>
          <p><strong>Coordinates:</strong> {park.latitude.toFixed(4)}, {park.longitude.toFixed(4)}</p>
        </div>

        <div className="detail-section">
          <h2>Activities</h2>
          <div className="activity-chips">
            {park.activities.map(activity => (
              <span key={activity.id} className="activity-chip">
                {activity.name}
              </span>
            ))}
          </div>
        </div>

        {park.website && (
          <div className="detail-section">
            <h2>Visit Website</h2>
            <a href={park.website} target="_blank" rel="noopener noreferrer" className="btn">
              Official Website →
            </a>
          </div>
        )}

        <div className="detail-section">
          <h2>Plan Your Visit</h2>
          <button className="btn btn-planner" onClick={() => setShowPlanner(true)}>
            📅 Plan My Visit
          </button>
        </div>

        <div className="detail-section small-map">
          <h2>Location Map</h2>
          <div className="map-placeholder">
            <p>📍 {park.name}</p>
            <p className="coordinates">{park.latitude.toFixed(4)}, {park.longitude.toFixed(4)}</p>
            <p className="map-note">Full map view available on <Link to="/map">Map Page</Link></p>
          </div>
        </div>
      </div>

      {showPlanner && (
        <VisitPlanner 
          park={park} 
          onClose={() => setShowPlanner(false)} 
        />
      )}
    </div>
  );
}
