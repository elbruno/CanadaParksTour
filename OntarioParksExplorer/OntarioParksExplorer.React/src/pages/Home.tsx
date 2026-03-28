export default function Home() {
  return (
    <div className="page">
      <h2>Welcome to Ontario Parks Explorer</h2>
      <div className="card">
        <h3>Discover the Natural Beauty of Ontario</h3>
        <p>
          Explore Ontario's stunning provincial parks, from pristine wilderness to accessible recreational areas.
          Plan your next adventure with our comprehensive park information, interactive maps, and personalized favorites.
        </p>
        <p>
          Get started by browsing our parks, saving your favorites, or exploring the interactive map view.
        </p>
        <div className="button-group">
          <a href="/parks" className="button primary">Browse Parks</a>
          <a href="/map" className="button secondary">View Map</a>
        </div>
      </div>
      <div className="features">
        <div className="feature-card">
          <div className="feature-icon">🔍</div>
          <h4>Browse & Search</h4>
          <p>Find the perfect park for your next outdoor adventure. Filter by activities, amenities, and location.</p>
        </div>
        <div className="feature-card">
          <div className="feature-icon">❤️</div>
          <h4>Save Favorites</h4>
          <p>Keep track of parks you want to visit or have visited. Build your personal collection of favorites.</p>
        </div>
        <div className="feature-card">
          <div className="feature-icon">🗺️</div>
          <h4>Interactive Maps</h4>
          <p>Explore parks on an interactive map. See locations, distances, and plan your route.</p>
        </div>
      </div>
    </div>
  );
}
