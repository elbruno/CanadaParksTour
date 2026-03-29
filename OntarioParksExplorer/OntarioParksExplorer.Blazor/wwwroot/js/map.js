let map = null;
let markers = [];

window.mapInterop = {
    initMap: function (elementId, lat, lon, zoom) {
        if (map) {
            map.remove();
        }
        map = L.map(elementId).setView([lat, lon], zoom);
        L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
            attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
        }).addTo(map);
        markers = [];
        
        // Force Leaflet to recalculate container size after initialization
        setTimeout(function () {
            map.invalidateSize();
        }, 100);
    },

    addMarker: function (lat, lon, title, popupContent) {
        if (!map) return;
        const marker = L.marker([lat, lon]).addTo(map);
        if (popupContent) {
            marker.bindPopup(popupContent);
        }
        if (title) {
            marker.bindTooltip(title);
        }
        markers.push(marker);
    },

    clearMarkers: function () {
        if (!map) return;
        markers.forEach(marker => marker.remove());
        markers = [];
    },

    flyTo: function (lat, lon, zoom) {
        if (!map) return;
        map.flyTo([lat, lon], zoom || 13);
    }
};
