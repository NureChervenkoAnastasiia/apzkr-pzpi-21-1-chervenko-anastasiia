document.addEventListener('DOMContentLoaded', async () => {
    const apiUrl = 'https://localhost:7206/api/Schedule/';
    const staffApiUrl = 'https://localhost:7206/api/Staff/';
    const scheduleTableBody = document.querySelector('#schedule tbody');
    const addButton = document.querySelector('.btn-add');
    const localizedText = {};

    const getToken = () => localStorage.getItem('token');

    const fetchData = async (url, options = {}) => {
        try {
            const response = await fetch(url, {
                headers: {
                    'Authorization': `Bearer ${getToken()}`,
                    'Content-Type': 'application/json',
                    ...options.headers
                },
                ...options
            });
            if (response.ok) return response.json();
            console.error('Error:', await response.text());
        } catch (error) {
            console.error('Error:', error.message);
        }
        return null;
    };

    const fetchSchedules = async () => {
        const schedules = await fetchData(apiUrl);
        if (schedules) displaySchedules(schedules);
    };

    const fetchStaff = async () => {
        const staff = await fetchData(staffApiUrl);
        if (staff) populateStaffDropdown(staff);
    };

    const populateStaffDropdown = (staff) => {
        const staffDropdown = document.getElementById('input-staff');
        staffDropdown.innerHTML = staff.map(member => `<option value="${member.id}">${member.name}</option>`).join('');
    };

    const displaySchedules = async (schedules) => {
        scheduleTableBody.innerHTML = '';
        for (const schedule of schedules) {
            const staffName = await getStaffNameById(schedule.staffId);
            if (!staffName) {
                console.error(`Staff member not found for schedule with ID ${schedule.id}`);
                continue;
            }
            const row = document.createElement('tr');
            row.innerHTML = `
                <td>${staffName}</td>
                <td>${formatDateTime(schedule.startDateTime)}</td>
                <td>${formatDateTime(schedule.finishDateTime)}</td>
                <td><button class="btn-edit" data-scheduleid="${schedule.id}">Edit</button></td>
                <td><button class="btn-delete" data-scheduleid="${schedule.id}">Delete</button></td>
            `;
            scheduleTableBody.appendChild(row);
        }
    };

    const getStaffNameById = async (staffId) => {
        const staff = await fetchData(`${staffApiUrl}${staffId}`);
        return staff ? staff.name : null;
    };

    const formatDateTime = (dateTime) => {
        const date = new Date(dateTime);
        const formattedDate = date.toLocaleDateString('en-CA');
        const formattedTime = date.toLocaleTimeString('en-GB');
        return `Дата: ${formattedDate}<br>Час: ${formattedTime}`;
    };

    const handleDelete = async (scheduleId) => {
        const response = await fetchData(`${apiUrl}${scheduleId}`, { method: 'DELETE' });
        if (response !== null) {
            alert('Schedule was deleted successfully!');
            await fetchSchedules();
            location.reload();
        }
    };

    const handleEdit = (scheduleId) => {
        const row = document.querySelector(`button[data-scheduleid="${scheduleId}"]`).parentNode.parentNode;
        const [nameCell, startCell, endCell, editButtonCell, deleteButtonCell] = row.cells;

        const staffName = nameCell.textContent;
        const startDateTime = startCell.textContent.replace('Дата: ', '').replace(' Час: ', 'T');
        const finishDateTime = endCell.textContent.replace('Дата: ', '').replace(' Час: ', 'T');

        nameCell.innerHTML = `<select>${document.getElementById('input-staff').innerHTML}</select>`;
        nameCell.querySelector('select').value = staffName;
        startCell.innerHTML = `<input type="datetime-local" value="${startDateTime}">`;
        endCell.innerHTML = `<input type="datetime-local" value="${finishDateTime}">`;

        editButtonCell.innerHTML = `<button class="btn-save" data-scheduleid="${scheduleId}">Save</button>`;
        deleteButtonCell.innerHTML = `<button class="btn-cancel" data-scheduleid="${scheduleId}">Cancel</button>`;
    };

    const handleSave = async (scheduleId) => {
        const row = document.querySelector(`button[data-scheduleid="${scheduleId}"]`).parentNode.parentNode;
        const staffId = row.cells[0].querySelector('select').value;
        const startDateTime = row.cells[1].querySelector('input').value;
        const finishDateTime = row.cells[2].querySelector('input').value;

        if (!staffId || !startDateTime || !finishDateTime) {
            alert('Please fill in all fields');
            return;
        }

        const response = await fetchData(`${apiUrl}${scheduleId}`, {
            method: 'PUT',
            body: JSON.stringify({ staffId, startDateTime, finishDateTime })
        });
        if (response !== null) {
            alert('Schedule was updated successfully!');
            await fetchSchedules();
            location.reload();
        }
    };

    const handleCancel = () => fetchSchedules();

    const handleAdd = async () => {
        const staffId = document.getElementById('input-staff').value;
        const startDateTime = document.getElementById('input-start').value;
        const finishDateTime = document.getElementById('input-end').value;

        if (!staffId || !startDateTime || !finishDateTime) {
            alert('Please fill in all fields');
            return;
        }

        const response = await fetchData(apiUrl, {
            method: 'POST',
            body: JSON.stringify({ staffId, startDateTime, finishDateTime })
        });
        if (response !== null) {
            alert('Schedule was added successfully!');
            await fetchSchedules();
            location.reload();
        }
    };

    scheduleTableBody.addEventListener('click', (event) => {
        const target = event.target;
        const scheduleId = target.getAttribute('data-scheduleid');

        if (target.classList.contains('btn-delete')) {
            handleDelete(scheduleId);
        } else if (target.classList.contains('btn-edit')) {
            handleEdit(scheduleId);
        } else if (target.classList.contains('btn-save')) {
            handleSave(scheduleId);
        } else if (target.classList.contains('btn-cancel')) {
            handleCancel(scheduleId);
        }
    });

    addButton.addEventListener('click', handleAdd);

    const loadLanguage = async (lang) => {
        try {
            const response = await fetch(`../../public/locales/${lang}/${lang}.json`);
            const translations = await response.json();
            Object.assign(localizedText, translations);
            applyTranslations();
        } catch (error) {
            console.error('Error loading language file:', error);
        }
    };

    const applyTranslations = () => {
        document.querySelectorAll('[data-translate]').forEach(element => {
            const key = element.getAttribute('data-translate');
            if (localizedText[key]) {
                element.textContent = localizedText[key];
            }
        });
    };

    const languageSelect = document.getElementById('language-select');
    languageSelect.addEventListener('change', (event) => {
        const selectedLanguage = event.target.value;
        loadLanguage(selectedLanguage);
    });

    // Load default language
    loadLanguage(languageSelect.value);

    await fetchStaff();
    await fetchSchedules();
});