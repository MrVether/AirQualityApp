let markersByLocation = {};
window.airQualityMap = null;

function initMap() {
    if (window.airQualityMap) {
        return window.airQualityMap;
    }

    const map = L.map("map", {
        minZoom: 2,
        worldCopyJump: true,
        maxBounds: [
            [-90, -Infinity],
            [90, Infinity]
        ]
    }).setView([0, 0], 2);

    L.tileLayer("https://{s}.basemaps.cartocdn.com/dark_all/{z}/{x}/{y}{r}.png", {
        attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors &copy; <a href="https://carto.com/attributions">CARTO</a>',
        maxZoom: 19,
    }).addTo(map);
    let baseRadius = 20000;

    map.on('zoomend', function () {
        let currentZoom = map.getZoom();
        let newRadius = baseRadius / Math.pow(2, currentZoom);

        for (const location in markersByLocation) {
            markersByLocation[location].setRadius(newRadius);
        }
    });
    L.Control.geocoder({
        defaultMarkGeocode: false,
        position: 'topleft',
        geocoder: new L.Control.Geocoder.Nominatim()
    })
        .on('markgeocode', function (e) {
            const bbox = e.geocode.bbox;
            const poly = L.polygon([
                bbox.getSouthEast(),
                bbox.getNorthEast(),
                bbox.getNorthWest(),
                bbox.getSouthWest()
            ]);
            map.fitBounds(poly.getBounds());
        })
        .addTo(map);

    window.airQualityMap = map;

    return map;
}

export { initMap };
