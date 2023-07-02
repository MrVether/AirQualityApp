import { calculateAQI, getUS_AQI } from './aqiCalculator.js';

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
            const airQualityInfo = getUS_AQI(measurements);
            document.getElementById('air-quality-info').textContent = 'Air Quality: ' + airQualityInfo;
        });

        markersGroup.addLayer(marker);
    }

    map.addLayer(markersGroup);

    return markersGroup;
}
window.updateMarkers = updateMarkers;


export { updateMarkers, getColorForMeasurement };
