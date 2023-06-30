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


function calculateAQI(measurements) {
    console.log("Calculating AQI for measurements: ", measurements);
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
    console.log(measurements);
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

var markersGroup = L.markerClusterGroup();

function updateMarkers(map, markers) {
    console.log("Updating markers for:", markers);

    let markersGroup = L.markerClusterGroup();
    markersGroup.clearLayers();

    for (const location of Object.keys(markers)) {
        console.log("Updating marker for location:", location);
        const measurements = markers[location];

        const anyMeasurement = measurements[Object.keys(measurements)[0]];
        const latitude = anyMeasurement.latitude;
        const longitude = anyMeasurement.longitude;

        if (isNaN(latitude) || isNaN(longitude)) {
            console.log(location, " has invalid coordinates - Latitude: ", latitude, " Longitude: ", longitude);
            continue;
        }

        const color = getColorForMeasurement(measurements);
        console.log("Latitude: ", latitude, " Longitude: ", longitude);

        const marker = L.circleMarker([latitude, longitude], {
            color: color,
            fillColor: color,
            fillOpacity: 0.5,
            radius: 20
        }).on('click', function (e) {
            window.getMeasurementsFromCache(location);
        });

        markersGroup.addLayer(marker);
    }

    map.addLayer(markersGroup);

    return markersGroup;
}

window.init = function (dotNetObjectRef) {
    window.dotNetHelper = dotNetObjectRef;
}

window.getMeasurementsFromCache = async function (location) {
    let measurements = await window.dotNetHelper.invokeMethodAsync('GetMeasurementsAsync', location);
    updateInfoPanel(measurements);
};


window.updateInfoPanel = function (location) {
    document.getElementById('location-name').textContent = location.name;
    document.getElementById('pm10').textContent = 'PM10: ' + location.pm10;
    document.getElementById('pm25').textContent = 'PM2.5: ' + location.pm25;
    document.getElementById('no2').textContent = 'NO2: ' + location.no2;
    document.getElementById('o3').textContent = 'O3: ' + location.o3;
    document.getElementById('pm1').textContent = 'PM1: ' + location.pm1;
    document.getElementById('pm4').textContent = 'PM4: ' + location.pm4;
    document.getElementById('bc').textContent = 'BC: ' + location.bc;
    document.getElementById('co').textContent = 'CO: ' + location.co;
    document.getElementById('so2').textContent = 'SO2: ' + location.so2;
    document.getElementById('no').textContent = 'NO: ' + location.no;
    document.getElementById('nox').textContent = 'NOx: ' + location.nox;
    document.getElementById('ch4').textContent = 'CH4: ' + location.ch4;
    document.getElementById('co2').textContent = 'CO2: ' + location.co2;
    document.getElementById('air-quality-info').textContent = 'Air Quality: ' + location.airQualityInfo;
    var lastUpdate = new Date(location.lastUpdate);
    document.getElementById('last-update').textContent = 'Last update: ' + lastUpdate.toLocaleDateString() + ' ' + lastUpdate.toLocaleTimeString();
}







window.initializeAirQualityMap = async function () {
    const map = initMap();

    return map;
}
