let markersByLocation = {};
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

function calculateAQI(measurements) {
    let breakpoints = {
        'o3': [
            { lo: 0.000, hi: 0.054, aqiLo: 0, aqiHi: 50 },
            { lo: 0.055, hi: 0.070, aqiLo: 51, aqiHi: 100 },
            { lo: 0.071, hi: 0.085, aqiLo: 101, aqiHi: 150 },
            { lo: 0.086, hi: 0.105, aqiLo: 151, aqiHi: 200 },
            { lo: 0.106, hi: 0.200, aqiLo: 201, aqiHi: 300 },
            { lo: 0.405, hi: 0.504, aqiLo: 301, aqiHi: 400 },
            { lo: 0.505, hi: 0.604, aqiLo: 401, aqiHi: 500 }
        ],
        'pm25': [
            { lo: 0.0, hi: 12.0, aqiLo: 0, aqiHi: 50 },
            { lo: 12.1, hi: 35.4, aqiLo: 51, aqiHi: 100 },
            { lo: 35.5, hi: 55.4, aqiLo: 101, aqiHi: 150 },
            { lo: 55.5, hi: 150.4, aqiLo: 151, aqiHi: 200 },
            { lo: 150.5, hi: 250.4, aqiLo: 201, aqiHi: 300 },
            { lo: 250.5, hi: 350.4, aqiLo: 301, aqiHi: 400 },
            { lo: 350.5, hi: 500.4, aqiLo: 401, aqiHi: 500 }
        ],
        'pm10': [
            { lo: 0, hi: 54, aqiLo: 0, aqiHi: 50 },
            { lo: 55, hi: 154, aqiLo: 51, aqiHi: 100 },
            { lo: 155, hi: 254, aqiLo: 101, aqiHi: 150 },
            { lo: 255, hi: 354, aqiLo: 151, aqiHi: 200 },
            { lo: 355, hi: 424, aqiLo: 201, aqiHi: 300 },
            { lo: 425, hi: 504, aqiLo: 301, aqiHi: 400 },
            { lo: 505, hi: 604, aqiLo: 401, aqiHi: 500 }
        ],
        'co': [
            { lo: 0.0, hi: 4.4, aqiLo: 0, aqiHi: 50 },
            { lo: 4.5, hi: 9.4, aqiLo: 51, aqiHi: 100 },
            { lo: 9.5, hi: 12.4, aqiLo: 101, aqiHi: 150 },
            { lo: 12.5, hi: 15.4, aqiLo: 151, aqiHi: 200 },
            { lo: 15.5, hi: 30.4, aqiLo: 201, aqiHi: 300 },
            { lo: 30.5, hi: 40.4, aqiLo: 301, aqiHi: 400 },
            { lo: 40.5, hi: 50.4, aqiLo: 401, aqiHi: 500 }
        ],
        'so2': [
            { lo: 0, hi: 35, aqiLo: 0, aqiHi: 50 },
            { lo: 36, hi: 75, aqiLo: 51, aqiHi: 100 },
            { lo: 76, hi: 185, aqiLo: 101, aqiHi: 150 },
            { lo: 186, hi: 304, aqiLo: 151, aqiHi: 200 },
            { lo: 305, hi: 604, aqiLo: 201, aqiHi: 300 },
            { lo: 605, hi: 804, aqiLo: 301, aqiHi: 400 },
            { lo: 805, hi: 1004, aqiLo: 401, aqiHi: 500 }
        ],
        'no2': [
            { lo: 0, hi: 53, aqiLo: 0, aqiHi: 50 },
            { lo: 54, hi: 100, aqiLo: 51, aqiHi: 100 },
            { lo: 101, hi: 360, aqiLo: 101, aqiHi: 150 },
            { lo: 361, hi: 649, aqiLo: 151, aqiHi: 200 },
            { lo: 650, hi: 1249, aqiLo: 201, aqiHi: 300 },
            { lo: 1250, hi: 1649, aqiLo: 301, aqiHi: 400 },
            { lo: 1650, hi: 2049, aqiLo: 401, aqiHi: 500 }
        ]
    };

    let highestAqi = 0;
    let pollutant = '';

    Object.keys(measurements).forEach(key => {
        console.log(key, measurements[key]);
        let concentration = measurements[key].value;
        if (typeof concentration === 'object') {
            console.error('Expected number but got object for concentration value: ' + JSON.stringify(concentration));
            return;
        }
        if (isNaN(concentration)) {
            console.error('Invalid concentration value: ' + concentration);
            return;
        }
        concentration = truncateConcentration(key, concentration);

        if (breakpoints[key]) {
            let index = calculateIndex(breakpoints[key], concentration);

            if (index > highestAqi) {
                highestAqi = index;
                pollutant = key;
            }
        } else {
            console.error('No breakpoints defined for pollutant: ' + key);
        }
    });





    return {
        aqi: highestAqi,
        pollutant: pollutant
    };
}

function truncateConcentration(pollutant, concentration) {
    console.log("Pollutant: ", pollutant); 
    console.log("Concentration (Before Parsing): ", concentration); 

    concentration = Number(concentration);
    console.log("Concentration (After Parsing): ", concentration); 

    if (isNaN(concentration)) {
        throw new Error('Invalid concentration value: ' + concentration);
    }

    switch (pollutant) {
        case 'o3':
            return Number(concentration.toFixed(3));
        case 'pm25':
            return Number(concentration.toFixed(1));
        case 'pm10':
        case 'so2':
        case 'no2':
            return Math.floor(concentration);
        case 'co':
            return Number(concentration.toFixed(1));
        default:
            return concentration;
    }
}


function calculateIndex(bpArray, concentration) {
    for (let i = 0; i < bpArray.length; i++) {
        if (concentration >= bpArray[i].lo && concentration <= bpArray[i].hi) {
            let { lo, hi, aqiLo, aqiHi } = bpArray[i];
            return Math.round(((aqiHi - aqiLo) / (hi - lo)) * (concentration - lo) + aqiLo);
        }
    }
}

function getColorForMeasurement(measurements) {
    const { aqi } = calculateAQI(measurements);

    if (aqi <= 50) {
        return 'green';
    } else if (aqi <= 100) {
        return 'yellow';
    } else if (aqi <= 150) {
        return 'orange';
    } else if (aqi <= 200) {
        return 'red';
    } else if (aqi <= 300) {
        return 'purple';
    } else {
        return 'maroon';
    }
}

function getUS_AQI(measurements) {
    const { aqi } = calculateAQI(measurements);

    if (aqi <= 50) {
        return "Good";
    } else if (aqi <= 100) {
        return "Moderate";
    } else if (aqi <= 150) {
        return "Unhealthy for Sensitive Groups";
    } else if (aqi <= 200) {
        return "Unhealthy";
    } else if (aqi <= 300) {
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

                marker.on('click', function () {
                    let lastUpdate = new Date(firstMeasurement.date.utc).toLocaleString();
                    document.getElementById('location-name').textContent = `Location: ${location}`;
                    document.getElementById('last-update').textContent = `Last update: ${lastUpdate}`;

                    const measurementTypesKeys = Object.keys(measurementTypes);

                    for (let key of measurementTypesKeys) {
                        if (measurementTypes[key]) {
                            document.getElementById(key).textContent = `${key.toUpperCase()}: ${measurementTypes[key].value.toFixed(2)} µg/m³`;
                        } else {
                            document.getElementById(key).style.display = "none";
                        }
                    }

                    document.getElementById('air-quality-info').textContent = `Air Quality: ${getUS_AQI(measurementTypes)}`;  
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
