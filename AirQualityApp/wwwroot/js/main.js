import { initMap } from './map.js';
import { updateMarkers } from './markers.js';
import { updateElementTextContent, updatePanelWithReceivedData, formatMeasurement } from './utils.js';

window.init = function (dotNetObjectRef) {
    window.dotNetHelper = dotNetObjectRef;
}

window.getMeasurementsFromCache = async function (location) {
    let measurements = await window.dotNetHelper.invokeMethodAsync('GetDetailedMeasurementsAsync', location);
    console.log("Received measurements: ", measurements);
    updatePanelWithReceivedData(measurements);
};




window.updateInfoPanel = function (location) {
    document.getElementById('location-name').textContent = location.name;

    updateElementTextContent('pm10', 'PM10: ', formatMeasurement(location.pm10), 'μg/m³');
    updateElementTextContent('pm25', 'PM2.5: ', formatMeasurement(location.pm25), 'μg/m³');
    updateElementTextContent('no2', 'NO2: ', formatMeasurement(location.no2), 'ppb');
    updateElementTextContent('o3', 'O3: ', formatMeasurement(location.o3), 'ppb');
    updateElementTextContent('pm1', 'PM1: ', formatMeasurement(location.pm1), 'μg/m³');
    updateElementTextContent('pm4', 'PM4: ', formatMeasurement(location.pm4), 'μg/m³');
    updateElementTextContent('bc', 'BC: ', formatMeasurement(location.bc), 'μg/m³');
    updateElementTextContent('co', 'CO: ', formatMeasurement(location.co), 'ppm');
    updateElementTextContent('so2', 'SO2: ', formatMeasurement(location.so2), 'ppb');
    updateElementTextContent('no', 'NO: ', formatMeasurement(location.no), 'ppb');
    updateElementTextContent('nox', 'NOx: ', formatMeasurement(location.nox), 'ppb');
    updateElementTextContent('ch4', 'CH4: ', formatMeasurement(location.ch4), 'ppm');
    updateElementTextContent('co2', 'CO2: ', formatMeasurement(location.co2), 'ppm');

    var lastUpdateUtc = location.date && location.date.utc;

    if (location.date && location.date.utc) {
        console.log("UTC date string: ", location.date.utc);
        var lastUpdate = new Date(location.date.utc);
        document.getElementById('last-update').textContent = 'Last update: ' + lastUpdate.toLocaleDateString() + ' ' + lastUpdate.toLocaleTimeString();
    } else {
        console.log("Date not available");
        document.getElementById('last-update').textContent = 'Last update: N/A';
    }
}



window.initializeAirQualityMap = async function () {
    const map = initMap();

    return map;
}
