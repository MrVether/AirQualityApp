async function fetchOpenAQData() {
    const url = "https://api.openaq.org/v2/measurements?city=Kraków&limit=100";
    const response = await fetch(url);
    const data = await response.json();

    if (Array.isArray(data.results)) {
        return data.results;
    } else {
        console.error("Unexpected data format:", data);
        return [];
    }
}


function addMarkers(map, measurementsByLocation) {
    console.log("Dodawanie znaczników dla:", measurementsByLocation); // Logowanie danych wejœciowych
    Object.entries(measurementsByLocation).forEach(([location, measurements]) => {
        console.log("Dodawanie znaczników dla lokalizacji:", location); // Logowanie dla ka¿dej lokalizacji
        const latLng = [measurements.pm10.coordinates.latitude, measurements.pm10.coordinates.longitude];
        const marker = L.marker(latLng).addTo(map);

        let popupContent = `<b>${location}</b>:<br/>`;
        Object.values(measurements).forEach(measurement => {
            popupContent += `${measurement.parameter}: ${measurement.value} ${measurement.unit}<br/>`;
        });

        marker.bindPopup(popupContent);
    });
}



function addAirQualityMarkers(map, measurements) {
    console.log("Dodawanie markerów dla pomiarów:", measurements); // Logowanie danych wejœciowych
    Object.entries(measurements).forEach(([measurementType, measurementList]) => {
        console.log("Dodawanie markerów dla typu pomiaru:", measurementType); // Logowanie dla ka¿dego typu pomiaru
        addMarkers(map, measurementList, measurementType);
    });
}



function initMap() {
    const map = L.map("map").setView([50.0647, 19.9450], 12);
    L.tileLayer("https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png", {
        attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
    }).addTo(map);
    return map;
}

window.initializeAirQualityMap = function () {
    return initMap();
}
function groupMeasurementsByLocation(measurements) {
    console.log("Inside groupMeasurementsByLocation"); // Log
    const locations = {};

    measurements.forEach(measurement => {
        const location = measurement.location;

        if (!locations[location]) {
            locations[location] = {};
        }

        const type = measurement.parameter;

        if (!locations[location][type]) {
            locations[location][type] = [];
        }

        locations[location][type].push(measurement);
    });

    for (const location in locations) {
        for (const type in locations[location]) {
            locations[location][type].sort((a, b) => new Date(b.date.utc) - new Date(a.date.utc));
            locations[location][type] = locations[location][type][0];
        }
    }

    console.log("Final Locations: ", locations); // Log
    return locations;
}

