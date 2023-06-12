let markersByLocation = {};
let connection = new signalR.HubConnectionBuilder()
    .withUrl("/mapHub")
    .configureLogging(signalR.LogLevel.Information)
    .build();

connection.on("ReceiveMeasurements", function (measurementsByLocation) {
    const map = initMap();
    updateMarkers(map, measurementsByLocation);
});

connection.start().then(function () {
    console.log("connected");
}).catch(function (err) {
    return console.error(err.toString());
});


window.airQualityMap = null;

function initMap() {
    if (window.airQualityMap) {
        return window.airQualityMap;
    }

    const map = L.map("map", {
        minZoom: 2,
        maxBounds: [
            [-90, -180],
            [90, 180]
        ]
    }).setView([0, 0], 2);

    L.tileLayer("https://{s}.basemaps.cartocdn.com/dark_all/{z}/{x}/{y}{r}.png", {
        attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors &copy; <a href="https://carto.com/attributions">CARTO</a>',
        maxZoom: 19
    }).addTo(map);

    window.airQualityMap = map;

    return map;
}



function getColorForMeasurement(measurements) {
    let highestPmValue = 0;

    Object.values(measurements).forEach(measurement => {
        if (measurement.parameter === 'pm10' || measurement.parameter === 'pm25') {
            if (measurement.value > highestPmValue) {
                highestPmValue = measurement.value;
            }
        }
    });

    if (highestPmValue <= 20) {
        return 'green';
    } else if (highestPmValue <= 35) {
        return 'yellow';
    } else if (highestPmValue <= 50) {
        return 'orange';
    } else if (highestPmValue <= 100) {
        return 'red';
    } else {
        return 'purple';
    }
}

function getAirQualityInfo(pm10, pm25) {
    if (pm10 <= 20 && pm25 <= 10) {
        return "Good";
    } else if (pm10 <= 50 && pm25 <= 25) {
        return "Moderate";
    } else if (pm10 <= 80 && pm25 <= 50) {
        return "Unhealthy for sensitive groups";
    } else if (pm10 <= 120 && pm25 <= 75) {
        return "Unhealthy";
    } else if (pm10 <= 380 && pm25 <= 175) {
        return "Very Unhealthy";
    } else {
        return "Hazardous";
    }
}

async function updateMarkers(map, measurementsByLocation) {
    console.log("Updating markers for:", measurementsByLocation);

    const newMarkersByLocation = {};

    for (const [location, measurementTypes] of Object.entries(measurementsByLocation)) {
        console.log("Updating markers for location:", location);
        let firstMeasurement = Object.values(measurementTypes)[0];
        const { coordinates } = firstMeasurement;


        if (coordinates) {
            const latLng = [coordinates.latitude, coordinates.longitude];
            let marker = markersByLocation[location];

            if (Object.values(measurementTypes).length > 0) {
                let firstMeasurement = Object.values(measurementTypes)[0];

                if (marker) {
                    marker.setLatLng(latLng);
                    marker.setStyle({ color: getColorForMeasurement(measurementTypes) });
                } else {
                    marker = L.circle(latLng, { color: getColorForMeasurement(measurementTypes), radius: 500 }).addTo(map);
                }

                let pm10 = measurementTypes.pm10 ? measurementTypes.pm10.value : 0;
                let pm25 = measurementTypes.pm25 ? measurementTypes.pm25.value : 0;

                marker.on('click', function () {
                    let lastUpdate = new Date(firstMeasurement.date.utc).toLocaleString();
                    document.getElementById('location-name').textContent = `Location: ${location}`;
                    document.getElementById('pm10').textContent = `PM10: ${pm10} µg/m³`;
                    document.getElementById('pm25').textContent = `PM2.5: ${pm25} µg/m³`;
                    document.getElementById('last-update').textContent = `Last update: ${lastUpdate}`;
                    document.getElementById('air-quality-info').textContent = `Air Quality: ${getAirQualityInfo(pm10, pm25)}`;
                });

                newMarkersByLocation[location] = marker;

            } else {
                console.warn(`Unable to add marker for location "${location}" due to missing measurements.`);
            }
        } else {
            console.warn(`Unable to add marker for location "${location}" due to missing coordinates.`);
        }

    }

    for (const location of Object.keys(markersByLocation)) {
        if (!newMarkersByLocation[location]) {
            map.removeLayer(markersByLocation[location]);
        }
    }

    markersByLocation = newMarkersByLocation;
}

window.initializeAirQualityMap = function () {
    const map = initMap();

    return map;
}
