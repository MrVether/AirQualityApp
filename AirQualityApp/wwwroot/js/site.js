let markersByLocation = {};

async function fetchOpenAQData() {
    const url = "https://api.openaq.org/v2/measurements?limit=10000";
    const response = await fetch(url);
    const data = await response.json();

    if (Array.isArray(data.results)) {
        return data.results;
    } else {
        console.error("Unexpected data format:", data);
        return [];
    }
}

function initMap() {
    const map = L.map("map").setView([0, 0], 2);
    L.tileLayer("https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png", {
        attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
    }).addTo(map);
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

async function updateMarkers(map, measurementsByLocation) {
    console.log("Updating markers for:", measurementsByLocation);
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
                    markersByLocation[location] = marker;
                }

                let popupContent = `<b>${location}</b>:<br/>`;
                for (const [type, measurement] of Object.entries(measurementTypes)) {
                    let lastUpdate = measurement.date.utc;
                    popupContent += `${type}: ${measurement.value} ${measurement.unit}<br/>Last Update: ${lastUpdate}<br/>`;
                }

                marker.bindPopup(popupContent);
            } else {
                console.warn(`Unable to add marker for location "${location}" due to missing measurements.`);
            }
        } else {
            console.warn(`Unable to add marker for location "${location}" due to missing coordinates.`);
        }

        await new Promise(resolve => setTimeout(resolve, 0));
    }

    for (const location of Object.keys(markersByLocation)) {
        if (!measurementsByLocation[location]) {
            map.removeLayer(markersByLocation[location]);
            delete markersByLocation[location];
        }
    }
}


window.initializeAirQualityMap = function () {
    const map = initMap();
    fetchOpenAQData().then(measurements => updateMarkers(map, measurements));
    return map;
}
