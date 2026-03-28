import { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { getActivities, getRecommendations } from '../services/api';
import { ActivityDto, ParkRecommendation } from '../types';

export default function Recommendations() {
  const [activities, setActivities] = useState<ActivityDto[]>([]);
  const [selectedActivities, setSelectedActivities] = useState<string[]>([]);
  const [region, setRegion] = useState('');
  const [preferenceText, setPreferenceText] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [recommendations, setRecommendations] = useState<ParkRecommendation[]>([]);

  useEffect(() => {
    loadActivities();
  }, []);

  const loadActivities = async () => {
    try {
      const data = await getActivities();
      setActivities(data);
    } catch (err) {
      console.error('Failed to load activities:', err);
    }
  };

  const toggleActivity = (activityName: string) => {
    setSelectedActivities(prev =>
      prev.includes(activityName)
        ? prev.filter(a => a !== activityName)
        : [...prev, activityName]
    );
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    if (selectedActivities.length === 0 && !preferenceText) {
      setError('Please select at least one activity or provide preferences');
      return;
    }

    setLoading(true);
    setError(null);
    try {
      const recs = await getRecommendations({
        activities: selectedActivities,
        region: region || undefined,
        preferenceText: preferenceText || undefined
      });
      setRecommendations(recs);
    } catch (err) {
      setError('Failed to get recommendations. Please try again.');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="page">
      <h1>🎯 AI Park Recommendations</h1>
      <p className="page-description">
        Tell us what you're looking for and we'll recommend the perfect parks for you!
      </p>

      <form onSubmit={handleSubmit} className="recommendations-form">
        <div className="form-group">
          <label>Activities</label>
          <div className="activity-select-grid">
            {activities.map(activity => (
              <label key={activity.id} className="checkbox-label">
                <input
                  type="checkbox"
                  checked={selectedActivities.includes(activity.name)}
                  onChange={() => toggleActivity(activity.name)}
                />
                {activity.name}
              </label>
            ))}
          </div>
        </div>

        <div className="form-group">
          <label htmlFor="region">Region (optional)</label>
          <input
            type="text"
            id="region"
            value={region}
            onChange={(e) => setRegion(e.target.value)}
            placeholder="e.g., Northern Ontario, Southern Ontario"
            className="form-control"
          />
        </div>

        <div className="form-group">
          <label htmlFor="preferences">Additional Preferences (optional)</label>
          <textarea
            id="preferences"
            value={preferenceText}
            onChange={(e) => setPreferenceText(e.target.value)}
            placeholder="Tell us more about what you're looking for... (e.g., family-friendly, remote, scenic views)"
            className="form-control"
            rows={4}
          />
        </div>

        {error && <div className="error-message">{error}</div>}

        <button type="submit" className="btn btn-primary" disabled={loading}>
          {loading ? '🤖 Finding Recommendations...' : '✨ Get Recommendations'}
        </button>
      </form>

      {recommendations.length > 0 && (
        <div className="recommendations-results">
          <h2>Recommended Parks</h2>
          <div className="recommendations-grid">
            {recommendations.map((rec, idx) => (
              <div key={idx} className="recommendation-card">
                <h3>{rec.parkName}</h3>
                <p className="recommendation-reason">{rec.reason}</p>
                <Link to={`/parks/${rec.parkId}`} className="btn btn-small">
                  View Details →
                </Link>
              </div>
            ))}
          </div>
        </div>
      )}
    </div>
  );
}
