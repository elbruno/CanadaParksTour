import { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { getParks, searchParks, filterParks, getActivities } from '../services/api';
import { ParkListDto, ActivityDto } from '../types';
import { useFavoritesContext } from '../context/FavoritesContext';

export default function Parks() {
  const [parks, setParks] = useState<ParkListDto[]>([]);
  const [activities, setActivities] = useState<ActivityDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [searchQuery, setSearchQuery] = useState('');
  const [searchTimeout, setSearchTimeout] = useState<number | null>(null);
  const [selectedActivities, setSelectedActivities] = useState<string[]>([]);
  const { isFavorite, toggleFavorite } = useFavoritesContext();

  const pageSize = 12;

  useEffect(() => {
    loadActivities();
  }, []);

  useEffect(() => {
    loadParks();
  }, [page, selectedActivities]);

  const loadActivities = async () => {
    try {
      const data = await getActivities();
      setActivities(data);
    } catch (err) {
      console.error('Failed to load activities:', err);
    }
  };

  const loadParks = async () => {
    setLoading(true);
    setError(null);
    try {
      let result;
      if (searchQuery) {
        result = await searchParks(searchQuery, page, pageSize);
      } else if (selectedActivities.length > 0) {
        result = await filterParks(selectedActivities, 'any', page, pageSize);
      } else {
        result = await getParks(page, pageSize);
      }
      setParks(result.items);
      setTotalPages(result.totalPages);
    } catch (err) {
      setError('Failed to load parks. Please try again.');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const handleSearchChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const value = e.target.value;
    setSearchQuery(value);

    if (searchTimeout) {
      clearTimeout(searchTimeout);
    }

    const timeout = setTimeout(() => {
      setPage(1);
      loadParks();
    }, 300);

    setSearchTimeout(timeout);
  };

  const clearSearch = () => {
    setSearchQuery('');
    setPage(1);
    loadParks();
  };

  const handleActivityToggle = (activityName: string) => {
    setSelectedActivities(prev => {
      const newSelection = prev.includes(activityName)
        ? prev.filter(a => a !== activityName)
        : [...prev, activityName];
      return newSelection;
    });
    setPage(1);
  };

  const removeActivityFilter = (activityName: string) => {
    setSelectedActivities(prev => prev.filter(a => a !== activityName));
    setPage(1);
  };

  return (
    <div className="page">
      <h2>Browse Parks</h2>
      
      <div className="filters-section">
        <div className="search-box">
          <input
            type="text"
            placeholder="Search parks..."
            value={searchQuery}
            onChange={handleSearchChange}
            className="search-input"
            aria-label="Search parks by name or description"
          />
          {searchQuery && (
            <button onClick={clearSearch} className="clear-button" aria-label="Clear search">×</button>
          )}
        </div>

        <div className="activity-filters">
          <label id="activity-filter-label">Filter by Activities:</label>
          <div className="activity-checkboxes" role="group" aria-labelledby="activity-filter-label">
            {activities.map(activity => (
              <label key={activity.id} className="activity-checkbox">
                <input
                  type="checkbox"
                  checked={selectedActivities.includes(activity.name)}
                  onChange={() => handleActivityToggle(activity.name)}
                  aria-label={`Filter by ${activity.name}`}
                />
                {activity.name}
              </label>
            ))}
          </div>
        </div>

        {selectedActivities.length > 0 && (
          <div className="active-filters" role="status" aria-live="polite">
            {selectedActivities.map(activity => (
              <span key={activity} className="filter-chip">
                {activity}
                <button onClick={() => removeActivityFilter(activity)} aria-label={`Remove ${activity} filter`}>×</button>
              </span>
            ))}
          </div>
        )}
      </div>

      {loading && (
        <div className="parks-grid">
          {[...Array(pageSize)].map((_, i) => (
            <div key={i} className="park-card skeleton">
              <div className="skeleton-image"></div>
              <div className="skeleton-text"></div>
              <div className="skeleton-text short"></div>
            </div>
          ))}
        </div>
      )}

      {error && <div className="error-message">{error}</div>}

      {!loading && !error && parks.length === 0 && (
        <div className="no-results">
          <p>No parks found. Try adjusting your search or filters.</p>
        </div>
      )}

      {!loading && !error && parks.length > 0 && (
        <>
          <div className="parks-grid">
            {parks.map(park => (
              <div key={park.id} className="park-card">
                <Link to={`/parks/${park.id}`} className="park-card-link" aria-label={`View details for ${park.name}`}>
                  <div className="park-image">
                    <img src={park.mainImageUrl} alt={`Photo of ${park.name}`} />
                    {park.isFeatured && <span className="featured-badge">Featured</span>}
                  </div>
                  <div className="park-info">
                    <h3>{park.name}</h3>
                    <p className="region">{park.region}</p>
                    <div className="activity-tags">
                      {park.activityNames.slice(0, 3).map(activity => (
                        <span key={activity} className="activity-tag">{activity}</span>
                      ))}
                      {park.activityNames.length > 3 && (
                        <span className="activity-tag">+{park.activityNames.length - 3}</span>
                      )}
                    </div>
                  </div>
                </Link>
                <button
                  className={`favorite-button ${isFavorite(park.id) ? 'active' : ''}`}
                  onClick={(e) => {
                    e.preventDefault();
                    toggleFavorite(park.id);
                  }}
                  title={isFavorite(park.id) ? 'Remove from favorites' : 'Add to favorites'}
                >
                  {isFavorite(park.id) ? '❤️' : '♡'}
                </button>
              </div>
            ))}
          </div>

          {totalPages > 1 && (
            <nav className="pagination" aria-label="Park list pagination">
              <button
                onClick={() => setPage(p => Math.max(1, p - 1))}
                disabled={page === 1}
                aria-label="Go to previous page"
              >
                Previous
              </button>
              <span className="page-info" aria-current="page">
                Page {page} of {totalPages}
              </span>
              <button
                onClick={() => setPage(p => Math.min(totalPages, p + 1))}
                disabled={page === totalPages}
                aria-label="Go to next page"
              >
                Next
              </button>
            </nav>
          )}
        </>
      )}
    </div>
  );
}
