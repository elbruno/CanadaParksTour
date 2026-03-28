import { useState } from 'react';
import { planVisit } from '../services/api';
import { ParkDetailDto, VisitPlan } from '../types';

interface VisitPlannerProps {
  park: ParkDetailDto;
  onClose: () => void;
}

const SEASONS = ['Spring', 'Summer', 'Fall', 'Winter'];
const COMMON_INTERESTS = [
  'Hiking',
  'Camping',
  'Wildlife Viewing',
  'Photography',
  'Fishing',
  'Canoeing',
  'Swimming',
  'Picnicking',
  'Biking',
  'Nature Study'
];

export default function VisitPlanner({ park, onClose }: VisitPlannerProps) {
  const [durationDays, setDurationDays] = useState(2);
  const [selectedInterests, setSelectedInterests] = useState<string[]>([]);
  const [season, setSeason] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [visitPlan, setVisitPlan] = useState<VisitPlan | null>(null);

  const toggleInterest = (interest: string) => {
    setSelectedInterests(prev =>
      prev.includes(interest)
        ? prev.filter(i => i !== interest)
        : [...prev, interest]
    );
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    if (selectedInterests.length === 0) {
      setError('Please select at least one interest');
      return;
    }

    setLoading(true);
    setError(null);
    try {
      const plan = await planVisit({
        parkId: park.id,
        durationDays,
        interests: selectedInterests,
        season: season || undefined
      });
      setVisitPlan(plan);
    } catch (err) {
      setError('Failed to generate visit plan. Please try again.');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const handleClose = () => {
    setVisitPlan(null);
    onClose();
  };

  return (
    <div className="modal-overlay" onClick={handleClose}>
      <div className="modal-content" onClick={(e) => e.stopPropagation()}>
        <div className="modal-header">
          <h2>📅 Plan Your Visit to {park.name}</h2>
          <button className="modal-close" onClick={handleClose}>×</button>
        </div>

        {!visitPlan ? (
          <form onSubmit={handleSubmit} className="planner-form">
            <div className="form-group">
              <label htmlFor="duration">Duration (days)</label>
              <input
                type="number"
                id="duration"
                min="1"
                max="14"
                value={durationDays}
                onChange={(e) => setDurationDays(parseInt(e.target.value))}
                className="form-control"
              />
            </div>

            <div className="form-group">
              <label>Interests</label>
              <div className="checkbox-grid">
                {COMMON_INTERESTS.map(interest => (
                  <label key={interest} className="checkbox-label">
                    <input
                      type="checkbox"
                      checked={selectedInterests.includes(interest)}
                      onChange={() => toggleInterest(interest)}
                    />
                    {interest}
                  </label>
                ))}
              </div>
            </div>

            <div className="form-group">
              <label htmlFor="season">Season (optional)</label>
              <select
                id="season"
                value={season}
                onChange={(e) => setSeason(e.target.value)}
                className="form-control"
              >
                <option value="">Any Season</option>
                {SEASONS.map(s => (
                  <option key={s} value={s}>{s}</option>
                ))}
              </select>
            </div>

            {error && <div className="error-message">{error}</div>}

            <div className="modal-actions">
              <button type="button" className="btn btn-secondary" onClick={handleClose}>
                Cancel
              </button>
              <button type="submit" className="btn btn-primary" disabled={loading}>
                {loading ? 'Generating Plan...' : 'Generate Plan'}
              </button>
            </div>
          </form>
        ) : (
          <div className="visit-plan">
            <div className="plan-overview">
              <h3>Overview</h3>
              <p>{visitPlan.overview}</p>
            </div>

            <div className="plan-itinerary">
              <h3>Itinerary</h3>
              {visitPlan.itinerary.map(day => (
                <div key={day.day} className="day-card">
                  <h4>Day {day.day}: {day.title}</h4>
                  <div className="day-section">
                    <strong>Activities:</strong>
                    <ul>
                      {day.activities.map((activity, idx) => (
                        <li key={idx}>{activity}</li>
                      ))}
                    </ul>
                  </div>
                  {day.tips.length > 0 && (
                    <div className="day-section">
                      <strong>Tips:</strong>
                      <ul>
                        {day.tips.map((tip, idx) => (
                          <li key={idx}>{tip}</li>
                        ))}
                      </ul>
                    </div>
                  )}
                </div>
              ))}
            </div>

            {visitPlan.packingList.length > 0 && (
              <div className="plan-section">
                <h3>Packing List</h3>
                <ul className="packing-list">
                  {visitPlan.packingList.map((item, idx) => (
                    <li key={idx}>{item}</li>
                  ))}
                </ul>
              </div>
            )}

            {visitPlan.tips.length > 0 && (
              <div className="plan-section">
                <h3>General Tips</h3>
                <ul>
                  {visitPlan.tips.map((tip, idx) => (
                    <li key={idx}>{tip}</li>
                  ))}
                </ul>
              </div>
            )}

            <div className="modal-actions">
              <button className="btn btn-primary" onClick={handleClose}>
                Close
              </button>
            </div>
          </div>
        )}
      </div>
    </div>
  );
}
