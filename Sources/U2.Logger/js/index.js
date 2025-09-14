function initializeApp() {
    const bands = [
        "160 m (1.8-2.0 MHz)",
        "80 m (3.5-4.0 MHz)",
        "60 m (5.3-5.4 MHz)",
        "40 m (7.0-7.3 MHz)",
        "30 m (10.1-10.15 MHz)",
        "20 m (14.0-14.35 MHz)",
        "17 m (18.068-18.168 MHz)",
        "15 m (21.0-21.45 MHz)",
        "12 m (24.89-24.99 MHz)",
        "10 m (28.0-29.7 MHz)",
        "6 m (50-54 MHz)",
        "2 m (144-148 MHz)",
        "1.25 m (222-225 MHz)",
        "70 cm (420-450 MHz)",
        "33 cm (902-928 MHz)",
        "23 cm (1240-1300 MHz)",
        "13 cm (2300-2450 MHz)",
        "6 cm (5650-5925 MHz)",
        "3 cm (10-10.5 GHz)",
        "1.25 cm (24-24.25 GHz)"
    ];

    const modes = [
        "AM", "ATV", "BPSK", "CW", "Data", "DIGITALVOICE", "DSTAR", "DV", "FAX", "FM", "FT8", "HELL", "MFSK", "MS", "MT63", "OLIVIA", "PACKET", "PSK31", "RTTY", "SSTV", "SSB", "THROB", "TOR", "V4", "VOI", "WSPR"
    ].sort();

    const propagations = [
        "", "Ionospheric", "Satellite", "EME", "Aurora", "Sporadic E", "Meteor Scatter", "Tropospheric Ducting", "Ground Wave"
    ].sort();

    const bandSelect = document.getElementById('band');
    const modeSelect = document.getElementById('mode');
    const propagationSelect = document.getElementById('propagation');
    const dateTimeDisplay = document.getElementById('date-time');
    const callsignInput = document.getElementById('callsign');
    const nameInput = document.getElementById('name');
    const commentInput = document.getElementById('comment');
    const sentInput = document.getElementById('sent');
    const receivedInput = document.getElementById('received');
    const realTimeCheckbox = document.getElementById('real-time');
    const utcCheckbox = document.getElementById('utc');
    const realTimeControls = document.getElementById('real-time-controls');
    const manualDateTimeControls = document.getElementById('manual-date-time-controls');
    const datePicker = document.getElementById('date-picker');
    const timePicker = document.getElementById('time-picker');
    const qsoTableContainer = document.getElementById('qso-table-container');
    const qsoTableBody = document.getElementById('qso-table-body');
    
    // Populate bands
    bands.forEach(band => {
        const option = document.createElement('option');
        option.value = band;
        option.textContent = band;
        bandSelect.appendChild(option);
    });

    // Populate modes
    modes.forEach(mode => {
        const option = document.createElement('option');
        option.value = mode;
        option.textContent = mode;
        modeSelect.appendChild(option);
    });

    // Populate propagations
    propagations.forEach(prop => {
        const option = document.createElement('option');
        option.value = prop;
        option.textContent = prop;
        propagationSelect.appendChild(option);
    });
    
    // Set initial values
    modeSelect.value = "CW";
    bandSelect.value = "160 m (1.8-2.0 MHz)";
    
    // Function to update sent and received fields based on mode
    const updateSignalReports = () => {
        const mode = modeSelect.value;
        if (mode === "CW") {
            sentInput.value = "599";
            receivedInput.value = "599";
        } else if (["AM", "DIGITALVOICE", "SSB", "FM"].includes(mode)) {
            sentInput.value = "59";
            receivedInput.value = "59";
        } else {
            sentInput.value = "";
            receivedInput.value = "";
        }
    };
    
    // Event listener for mode change
    modeSelect.addEventListener('change', updateSignalReports);

    // Update date and time for the real-time display
    let dateTimeInterval;
    function updateDateTimeDisplay() {
        const now = new Date();
        
        const year = utcCheckbox.checked ? now.getUTCFullYear() : now.getFullYear();
        const month = (utcCheckbox.checked ? now.getUTCMonth() + 1 : now.getMonth() + 1).toString().padStart(2, '0');
        const day = (utcCheckbox.checked ? now.getUTCDate() : now.getDate()).toString().padStart(2, '0');
        const hours = (utcCheckbox.checked ? now.getUTCHours() : now.getHours()).toString().padStart(2, '0');
        const minutes = (utcCheckbox.checked ? now.getUTCMinutes() : now.getMinutes()).toString().padStart(2, '0');
        const seconds = (utcCheckbox.checked ? now.getUTCSeconds() : now.getSeconds()).toString().padStart(2, '0');
        
        dateTimeDisplay.value = `${day}.${month}.${year} ${hours}:${minutes}:${seconds}`;
    }

    function updateManualDateTimePickers() {
        const now = new Date();
        let date, hours, minutes;
    
        if (utcCheckbox.checked) {
            date = `${now.getUTCFullYear()}-${(now.getUTCMonth() + 1).toString().padStart(2, '0')}-${now.getUTCDate().toString().padStart(2, '0')}`;
            hours = now.getUTCHours().toString().padStart(2, '0');
            minutes = now.getUTCMinutes().toString().padStart(2, '0');
        } else {
            date = `${now.getFullYear()}-${(now.getMonth() + 1).toString().padStart(2, '0')}-${now.getDate().toString().padStart(2, '0')}`;
            hours = now.getHours().toString().padStart(2, '0');
            minutes = now.getMinutes().toString().padStart(2, '0');
        }
    
        datePicker.value = date;
        timePicker.value = `${hours}:${minutes}`;
    }

    function toggleDateTimeControls() {
        if (realTimeCheckbox.checked) {
            realTimeControls.classList.remove('hidden');
            manualDateTimeControls.classList.add('hidden');
            if (!dateTimeInterval) {
                updateDateTimeDisplay();
                dateTimeInterval = setInterval(updateDateTimeDisplay, 1000);
            }
        } else {
            realTimeControls.classList.add('hidden');
            manualDateTimeControls.classList.remove('hidden');
            clearInterval(dateTimeInterval);
            dateTimeInterval = null;
            updateManualDateTimePickers(); // Initialize pickers when switching to manual mode
        }
    }

    realTimeCheckbox.addEventListener('change', toggleDateTimeControls);
    utcCheckbox.addEventListener('change', () => {
        // If real-time is enabled, just update the display
        if (realTimeCheckbox.checked) {
            updateDateTimeDisplay();
        } else {
            // If manual mode, update the pickers
            updateManualDateTimePickers();
        }
    });

    // Set initial UI state
    toggleDateTimeControls();
    updateSignalReports();
    
    // Callsign input validation
    const allowedChars = '0123456789/QWERTYUIOPASDFGHJKLZXCVBNM';
    callsignInput.addEventListener('input', (event) => {
        let filteredValue = '';
        const inputValue = event.target.value.toUpperCase();
        for (let i = 0; i < inputValue.length; i++) {
            if (allowedChars.includes(inputValue[i])) {
                filteredValue += inputValue[i];
            }
        }
        event.target.value = filteredValue;
    });
    
    // Function to save the QSO
    const saveQSO = async () => {
        let qsoDateTime;
        if (realTimeCheckbox.checked) {
            qsoDateTime = new Date().toISOString();
        } else {
            const dateVal = datePicker.value;
            const timeVal = timePicker.value;
            if (dateVal && timeVal) {
                if (utcCheckbox.checked) {
                    // User input is already in UTC
                    qsoDateTime = new Date(`${dateVal}T${timeVal}:00Z`).toISOString();
                } else {
                    // User input is in local time, convert to UTC
                    const localDateTime = new Date(`${dateVal}T${timeVal}:00`);
                    qsoDateTime = localDateTime.toISOString();
                }
            } else {
                console.error('Manual date and time fields must be filled.');
                return;
            }
        }

        const qso = {
            Callsign: callsignInput.value,
            Snt: sentInput.value,
            Rcvd: receivedInput.value,
            Name: nameInput.value,
            Comment: commentInput.value,
            Band: bandSelect.value,
            Mode: modeSelect.value,
            Propagation: propagationSelect.value,
            DateTime: qsoDateTime
        };

        try {
            const response = await fetch('https://localhost:7154/api/v1/QSOs', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(qso),
            });

            if (response.ok) {
                console.log('QSO saved successfully!');
                // Clear fields after successful save
                callsignInput.value = '';
                nameInput.value = '';
                commentInput.value = '';
                updateSignalReports(); // Set default values based on current mode
                callsignInput.focus(); // Set focus to callsign input

            } else {
                console.error('Failed to save QSO:', response.statusText);
            }
        } catch (error) {
            console.error('Error saving QSO:', error);
        }
    };
    
    // Function to wipe fields
    const wipeFields = () => {
        callsignInput.value = '';
        sentInput.value = '59';
        receivedInput.value = '59';
        nameInput.value = '';
        commentInput.value = '';
    };
    
    // Event listeners for Save buttons and F11/Enter keypress
    const f11Button = document.getElementById('f11Button');
    const saveButton2 = document.getElementById('saveButton2');
    const lastQsoButton = document.getElementById('lastQsoButton');
    f11Button.addEventListener('click', saveQSO);
    saveButton2.addEventListener('click', saveQSO);

    document.addEventListener('keydown', (event) => {
        if (event.key === 'F11' || event.key === 'Enter') {
            event.preventDefault(); 
            saveQSO();
        }
    });
    
    // Event listener for Wipe button
    const wipeButton = document.getElementById('wipeButton');
    wipeButton.addEventListener('click', wipeFields);
    
    // Event listener for List button - now calls the IPC function
    const listButton = document.getElementById('listButton');
    listButton.addEventListener('click', () => {
        if (window.api && window.api.openListWindow) {
            window.api.openListWindow();
        } else {
            console.error("IPC API not available. Make sure preload.js is configured correctly.");
        }
    });

    // New logic for toggling the window size and showing/hiding the QSO table
    let isTall = false;

    const fetchAndDisplayQSOs = async () => {
        try {
            // Fetch the first page of 25 QSOs, sorted by date descending
            const response = await fetch('https://localhost:7154/api/v1/QSOs?pageNumber=1&pageSize=25&sortKey=dateTime&sortDirection=desc');
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            const latestQSOs = await response.json();

            // Clear the existing table body
            qsoTableBody.innerHTML = '';

            // Populate the table with the QSOs
            latestQSOs.forEach(qso => {
                const row = document.createElement('tr');
                const qsoDateTime = new Date(qso.dateTime);
                const dateString = qsoDateTime.toLocaleDateString();
                const timeString = qsoDateTime.toLocaleTimeString();

                row.innerHTML = `
                    <td class="px-3 py-1 whitespace-nowrap text-white">${qso.callsign}</td>
                    <td class="px-3 py-1 whitespace-nowrap text-white">${dateString} ${timeString}</td>
                    <td class="px-3 py-1 whitespace-nowrap text-white">${qso.band}</td>
                    <td class="px-3 py-1 whitespace-nowrap text-white">${qso.mode}</td>
                `;
                qsoTableBody.appendChild(row);
            });
        } catch (error) {
            console.error('Error fetching QSOs:', error);
        }
    };

    lastQsoButton.addEventListener('click', async () => {
        if (window.api && window.api.resizeWindow && window.api.getWindowSize) {
            const [currentWidth, currentHeight] = await window.api.getWindowSize();
            let sizeDelta = 300;
            let newHeight;
            if (!isTall) {
                newHeight = currentHeight + sizeDelta;
                await fetchAndDisplayQSOs(); // Fetch and display table when expanding
                qsoTableContainer.classList.remove('hidden');
            } else {
                newHeight = currentHeight - sizeDelta;
                qsoTableContainer.classList.add('hidden'); // Hide table when shrinking
            }
            window.api.resizeWindow(currentWidth, newHeight);
            isTall = !isTall; // Toggle the state
        } else {
            console.error("IPC API for resize or get size not available.");
        }
    });
}
