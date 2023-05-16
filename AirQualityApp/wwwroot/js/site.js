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

async function addMarkers(map, measurementsByLocation) {
    console.log("Dodawanie znaczników dla:", measurementsByLocation); // Logowanie danych wejœciowych
    for (const [location, measurementTypes] of Object.entries(measurementsByLocation)) {
        console.log("Dodawanie znaczników dla lokalizacji:", location); // Logowanie dla ka¿dej lokalizacji
        let firstMeasurement = Object.values(measurementTypes)[0];
        const { coordinates } = firstMeasurement;

        if (coordinates) {
            const latLng = [coordinates.latitude, coordinates.longitude];
            const marker = L.circle(latLng, { color: getColorForMeasurement(measurementTypes), radius: 500 }).addTo(map);

            let popupContent = `<b>${location}</b>:<br/>`;
            for (const [type, measurement] of Object.entries(measurementTypes)) {
                popupContent += `${type}: ${measurement.value} ${measurement.unit}<br/>`;
            }

            marker.bindPopup(popupContent);
        } else {
            console.warn(`Nie mo¿na dodaæ znacznika dla lokalizacji "${location}" z powodu braku wspó³rzêdnych.`);
        }

        await new Promise(resolve => setTimeout(resolve, 0)); // Daje czas na przetworzenie innych zadañ
    }
}






function addAirQualityMarkers(map, measurements) {
    Object.entries(measurements).forEach(([measurementType, measurementList]) => {
        addMarkers(map, measurementList, measurementType);
    });
}

window.initializeAirQualityMap = function () {
    return initMap();
}


//function groupMeasurementsByLocation(measurements) {
//    console.log("Inside groupMeasurementsByLocation"); // Log
//    const locations = {};

//    measurements.forEach(measurement => {
//        const location = measurement.location;

//        if (!locations[location]) {
//            locations[location] = {};
//        }

//        const type = measurement.parameter;

//        if (!locations[location][type]) {
//            locations[location][type] = [];
//        }

//        locations[location][type].push(measurement);
//    });

//    for (const location in locations) {
//        for (const type in locations[location]) {
//            locations[location][type].sort((a, b) => new Date(b.date.utc) - new Date(a.date.utc));
//            locations[location][type] = locations[location][type][0];
//        }
//    }

//    console.log("Final Locations: ", locations); // Log
//    console.log("Returning...."); // Log
//    return locations;
//}

