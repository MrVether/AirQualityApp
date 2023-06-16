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

function getAirQualityInfo(pm10, pm25, no2, o3) {
    let caqi_pm10 = pm10 ? pm10 / 180 : 0; 
    let caqi_pm25= pm25 ? pm25 / 110 : 0;
    let caqi_no2 = no2 ? no2 / 400 : 0; 
    let caqi_o3 = o3 ? o3 / 240 : 0; 

    let highestCaqi = Math.max(caqi_pm10, caqi_pm25, caqi_no2, caqi_o3);

    if (highestCaqi <= 0.25) {
        return "Very Low";
    } else if (highestCaqi <= 0.5) {
        return "Low";
    } else if (highestCaqi <= 0.75) {
        return "Moderate";
    } else if (highestCaqi <= 1) {
        return "High";
    } else {
        return "Very High";
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
                let no2 = measurementTypes.no2 ? measurementTypes.no2.value : 0;
                let o3 = measurementTypes.o3 ? measurementTypes.o3.value : 0;
                let pm1 = measurementTypes.pm1 ? measurementTypes.pm1.value : 0;
                let pm4 = measurementTypes.pm4 ? measurementTypes.pm4.value : 0;
                let bc = measurementTypes.bc ? measurementTypes.bc.value : 0;
                let co = measurementTypes.co ? measurementTypes.co.value : 0;
                let so2 = measurementTypes.so2 ? measurementTypes.so2.value : 0;
                let no = measurementTypes.no ? measurementTypes.no.value : 0;
                let nox = measurementTypes.nox ? measurementTypes.nox.value : 0;
                let ch4 = measurementTypes.ch4 ? measurementTypes.ch4.value : 0;
                let co2 = measurementTypes.co2 ? measurementTypes.co2.value : 0;

                    






                marker.on('click', function () {
                    let lastUpdate = new Date(firstMeasurement.date.utc).toLocaleString();
                    document.getElementById('location-name').textContent = `Location: ${location}`;
                    document.getElementById('pm10').textContent = `PM10:  ${measurementTypes.pm10 ? measurementTypes.pm10.value : 'N/A'}  µg/m³`;
                    document.getElementById('pm25').textContent = `PM2.5: ${measurementTypes.pm25 ? measurementTypes.pm25.value : 'N/A'}  µg/m³`;
                    document.getElementById('pm1').textContent = `PM1: ${measurementTypes.pm1 ? measurementTypes.pm1.value : 'N/A'} µg/m³`;
                    document.getElementById('pm4').textContent = `PM4: ${measurementTypes.pm4 ? measurementTypes.pm4.value : 'N/A'} µg/m³`;
                    document.getElementById('bc').textContent = `BC: ${measurementTypes.bc ? measurementTypes.bc.value : 'N/A'} µg/m³`;
                    document.getElementById('no2').textContent = `NO2: ${measurementTypes.no2 ? measurementTypes.no2.value : 'N/A'} µg/m³`;
                    document.getElementById('o3').textContent = `O3: ${measurementTypes.o3 ? measurementTypes.o3.value : 'N/A'} µg/m³`;
                    document.getElementById('co').textContent = `CO: ${measurementTypes.co ? measurementTypes.co.value : 'N/A'} µg/m³`;
                    document.getElementById('so2').textContent = `SO2: ${measurementTypes.so2 ? measurementTypes.so2.value : 'N/A'} µg/m³`;
                    document.getElementById('no').textContent = `NO: ${measurementTypes.no ? measurementTypes.no.value : 'N/A'} µg/m³`;
                    document.getElementById('nox').textContent = `NOx: ${measurementTypes.nox ? measurementTypes.nox.value : 'N/A'} µg/m³`;
                    document.getElementById('ch4').textContent = `CH4: ${measurementTypes.ch4 ? measurementTypes.ch4.value : 'N/A'} µg/m³`;
                    document.getElementById('co2').textContent = `CO2: ${measurementTypes.co2 ? measurementTypes.co2.value : 'N/A'} µg/m³`;
                    document.getElementById('last-update').textContent = `Last update: ${lastUpdate}`;
                    document.getElementById('air-quality-info').textContent = `Air Quality: ${getAirQualityInfo(pm10, pm25,pm1,pm4,bc,no2,o3,co,so2,no,nox,ch4,co2)}`;
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
