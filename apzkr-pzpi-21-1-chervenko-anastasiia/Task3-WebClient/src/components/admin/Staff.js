document.addEventListener('DOMContentLoaded', async function () {
    const apiUrl = 'https://localhost:7206/api/Staff/';
    const restaurantsApiUrl = 'https://localhost:7206/api/Restaurants/';
    const workingHoursApiUrl = 'https://localhost:7206/api/Staff/weekly-working-hours';
    
    const staffTableBody = document.querySelector('#staff-table tbody');
    const addButton = document.querySelector('.btn-add');
    const fetchHoursButton = document.querySelector('.btn-fetch-hours');
    const inputDate = document.getElementById('input-date');
    const workingHoursContainer = document.getElementById('working-hours-container');
    const localizedText = {};

    const getToken = () => localStorage.getItem('token');

    const fetchApiData = async (url, options = {}) => {
        try {
            const response = await fetch(url, options);
            if (response.ok) {
                return await response.json();
            } else {
                console.error('Error:', await response.text());
                return null;
            }
        } catch (error) {
            console.error('Error:', error.message);
            return null;
        }
    };

    const fetchStaff = async () => {
        const staff = await fetchApiData(apiUrl, {
            headers: {
                'Authorization': `Bearer ${getToken()}`,
                'Content-Type': 'application/json',
            },
        });
        if (staff) displayStaff(staff);
    };

    const fetchRestaurants = async () => {
        const restaurants = await fetchApiData(restaurantsApiUrl, {
            headers: {
                'Authorization': `Bearer ${getToken()}`,
                'Content-Type': 'application/json',
            },
        });
        if (restaurants) populateDropdown('input-restaurant', restaurants);
    };

    const populateDropdown = (elementId, items, valueField = 'id', textField = 'name') => {
        const dropdown = document.getElementById(elementId);
        dropdown.innerHTML = '';
        items.forEach(item => {
            const option = document.createElement('option');
            option.value = item[valueField];
            option.textContent = item[textField];
            dropdown.appendChild(option);
        });
    };

    const getRestaurantNameById = async (restaurantId) => {
        const restaurant = await fetchApiData(`${restaurantsApiUrl}${restaurantId}`, {
            headers: {
                'Authorization': `Bearer ${getToken()}`,
                'Content-Type': 'application/json',
            },
        });
        return restaurant ? restaurant.name : null;
    };

    const displayStaff = async (staff) => {
        staffTableBody.innerHTML = '';
        for (const member of staff) {
            const restaurantName = await getRestaurantNameById(member.restaurantId);
            if (!restaurantName) {
                console.error(`Restaurant not found for staff member with ID ${member.id}`);
                continue;
            }

            const row = document.createElement('tr');
            row.innerHTML = `
                <td>${member.name}</td>
                <td>${member.position}</td>
                <td>${member.hourlySalary}</td>
                <td>${member.phone}</td>
                <td>${member.attendanceCard}</td>
                <td>${member.login}</td>
                <td>${restaurantName}</td>
                <td><button class="btn-edit" data-staffid="${member.id}">Edit</button></td>
                <td><button class="btn-delete" data-staffid="${member.id}">Delete</button></td>
            `;
            staffTableBody.appendChild(row);
        }
    };

    const handleDelete = async (staffId) => {
        if (!confirm('Are you sure you want to delete this staff member?')) return;

        const response = await fetchApiData(`${apiUrl}${staffId}`, {
            method: 'DELETE',
            headers: {
                'Authorization': `Bearer ${getToken()}`,
                'Content-Type': 'application/json',
            },
        });
        if (response) {
            alert('Staff member was deleted successfully!');
            await fetchStaff();
            location.reload();
        }
    };

    const handleEdit = (staffId) => {
        const row = document.querySelector(`button[data-staffid="${staffId}"]`).closest('tr');
        const cells = row.cells;

        ['name', 'position', 'hourlySalary', 'phone', 'attendanceCard', 'login', 'restaurantId'].forEach((field, index) => {
            const value = cells[index].textContent;
            if (field === 'position' || field === 'restaurantId') {
                cells[index].innerHTML = `<select>${document.getElementById(`input-${field}`).innerHTML}</select>`;
                cells[index].querySelector('select').value = value;
            } else {
                cells[index].innerHTML = `<input type="text" value="${value}">`;
            }
        });

        cells[7].innerHTML = `<button class="btn-save" data-staffid="${staffId}">Save</button>`;
        cells[8].innerHTML = `<button class="btn-cancel" data-staffid="${staffId}">Cancel</button>`;
    };

    const handleSave = async (staffId) => {
        const row = document.querySelector(`button[data-staffid="${staffId}"]`).closest('tr');
        const cells = row.cells;

        const payload = {};
        ['name', 'position', 'hourlySalary', 'phone', 'attendanceCard', 'login', 'restaurantId'].forEach((field, index) => {
            payload[field] = field === 'position' || field === 'restaurantId' ? cells[index].querySelector('select').value : cells[index].querySelector('input').value;
        });

        if (!Object.values(payload).every(value => value)) {
            alert('Please fill in all fields');
            return;
        }

        if (confirm('Do you want to change the password?')) {
            payload.password = prompt('Please enter the new password:');
        }

        const response = await fetchApiData(`${apiUrl}${staffId}`, {
            method: 'PUT',
            headers: {
                'Authorization': `Bearer ${getToken()}`,
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(payload),
        });

        if (response) {
            alert('Staff member was updated successfully!');
            await fetchStaff();
            location.reload();
        }
    };

    const handleCancel = async () => {
        await fetchStaff();
    };

    const handleAdd = async () => {
        const payload = {
            name: document.getElementById('input-name').value,
            position: document.getElementById('input-position').value,
            hourlySalary: document.getElementById('input-salary').value,
            phone: document.getElementById('input-phone').value,
            attendanceCard: document.getElementById('input-card').value,
            login: document.getElementById('input-login').value,
            password: prompt('Please enter a password for the new staff member:'),
            restaurantId: document.getElementById('input-restaurant').value,
        };

        if (!Object.values(payload).every(value => value)) {
            alert('Please fill in all fields');
            return;
        }

        const response = await fetchApiData(`${apiUrl}register`, {
            method: 'POST',
            headers: {
                'Authorization': `Bearer ${getToken()}`,
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(payload),
        });

        if (response) {
            alert('Staff member was added successfully!');
            document.querySelectorAll('#staff-form input, #staff-form select').forEach(input => input.value = '');
            await fetchStaff();
            location.reload();
        }
    };

    const handleFetchHours = async () => {
        const selectedDate = inputDate.value;
        if (!selectedDate) {
            alert('Please select a date');
            return;
        }

        const workingHours = await fetchApiData(`${workingHoursApiUrl}?date=${encodeURIComponent(selectedDate)}`, {
            headers: {
                'Authorization': `Bearer ${getToken()}`,
                'Content-Type': 'application/json',
            },
        });

        if (workingHours) displayWorkingHours(workingHours);
    };

    const displayWorkingHours = (workingHours) => {
        workingHoursContainer.innerHTML = '';
        workingHoursContainer.style.display = 'block';

        workingHours.forEach(entry => {
            const p = document.createElement('p');
            p.textContent = `${entry.name} - ${entry.totalWorkingHours} годин`;
            workingHoursContainer.appendChild(p);
        });
    };

    staffTableBody.addEventListener('click', async (event) => {
        const { target } = event;
        const staffId = target.getAttribute('data-staffid');

        if (target.classList.contains('btn-delete')) {
            await handleDelete(staffId);
        } else if (target.classList.contains('btn-edit')) {
            handleEdit(staffId);
        } else if (target.classList.contains('btn-save')) {
            await handleSave(staffId);
        } else if (target.classList.contains('btn-cancel')) {
            handleCancel();
        }
    });

    addButton.addEventListener('click', handleAdd);
    fetchHoursButton.addEventListener('click', handleFetchHours);

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

    await Promise.all([fetchStaff(), fetchRestaurants()]);
});