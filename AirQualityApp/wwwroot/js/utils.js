
function updateElementTextContent(elementId, prefix, value, unit = '') {
    const element = document.getElementById(elementId);
    if (value === 'N/A') {
        element.style.display = 'none';
    } else {
        element.style.display = 'block';
        element.textContent = `${prefix}${value}${value !== 'N/A' ? ` ${unit}` : ''}`;
    }
}

function formatMeasurement(measurement) {
    if (measurement !== undefined && !isNaN(measurement)) {
        return parseFloat(measurement).toFixed(2);
    } else {
        return 'N/A';
    }
}

function updatePanelWithReceivedData(receivedData) {
    let locationInfo = {};

    if (receivedData.length > 0) {
        locationInfo.name = receivedData[0].location;
        locationInfo.date = receivedData[0].date; 
    }

    for (let measurement of receivedData) {
        let parameterName = measurement.parameter;
        locationInfo[parameterName] = measurement.value;
    }

    window.updateInfoPanel(locationInfo);
}

export { updateElementTextContent, updatePanelWithReceivedData, formatMeasurement };
