document.addEventListener('DOMContentLoaded', async function() {
    const apiUrl = 'https://localhost:7206/api/Staff/';
    const staffInfoContainer = document.getElementById('staff-info');
    const editButton = document.getElementById('edit-button');
    const saveButton = document.getElementById('save-button');
    const localizedText = {};

    const getToken = () => localStorage.getItem('token');

    const getUserData = () => {
        const userData = JSON.parse(localStorage.getItem('userData'));
        if (!userData || !userData.nameid) {
            console.error('Error: User data not found in localStorage');
            return null;
        }
        return userData;
    };

    const fetchStaffInfo = async () => {
        const userData = getUserData();
        if (!userData) return null;

        const token = getToken();
        if (!token) {
            console.error('Error: Token not found in localStorage');
            return null;
        }

        try {
            const response = await fetch(`${apiUrl}${userData.nameid}`, {
                headers: {
                    'Authorization': `Bearer ${token}`,
                    'Content-Type': 'application/json'
                }
            });

            if (response.ok) {
                const staffData = await response.json();
                displayStaffInfo(staffData);
                return staffData;
            } else {
                throw new Error(`Failed to fetch staff info: ${response.status} ${response.statusText}`);
            }
        } catch (error) {
            console.error('Error:', error.message);
            return null;
        }
    };

    const displayStaffInfo = (staff) => {
        staffInfoContainer.innerHTML = `
            <p><strong>Name:</strong> ${staff.name}</p>
            <p><strong>Position:</strong> ${staff.position}</p>
            <p><strong>Hourly Salary:</strong> ${staff.hourlySalary}</p>
            <p><strong>Phone:</strong> ${staff.phone}</p>
            <p><strong>Attendance Card:</strong> ${staff.attendanceCard}</p>
            <p><strong>Login:</strong> ${staff.login}</p>
        `;
    };

    const enableEditing = (staff) => {
        staffInfoContainer.innerHTML = `
            <p><strong>Name:</strong> <input type="text" id="edit-name" value="${staff.name}"></p>
            <p><strong>Position:</strong> <input type="text" id="edit-position" value="${staff.position}"></p>
            <p><strong>Hourly Salary:</strong> <input type="number" id="edit-salary" value="${staff.hourlySalary}"></p>
            <p><strong>Phone:</strong> <input type="number" id="edit-phone" value="${staff.phone}"></p>
            <p><strong>Attendance Card:</strong> <input type="number" id="edit-card" value="${staff.attendanceCard}"></p>
            <p><strong>Login:</strong> <input type="text" id="edit-login" value="${staff.login}"></p>
        `;
    };

    const handleEdit = () => {
        editButton.style.display = 'none';
        saveButton.style.display = 'block';
    };

    const handleSave = async () => {
        const userData = getUserData();
        if (!userData) return;

        const token = getToken();
        if (!token) {
            console.error('Error: Token not found in localStorage');
            return;
        }

        const updatedStaff = {
            name: document.getElementById('edit-name').value,
            position: document.getElementById('edit-position').value,
            hourlySalary: document.getElementById('edit-salary').value,
            phone: document.getElementById('edit-phone').value,
            attendanceCard: document.getElementById('edit-card').value,
            login: document.getElementById('edit-login').value
        };

        try {
            const response = await fetch(`${apiUrl}${userData.nameid}`, {
                method: 'PUT',
                headers: {
                    'Authorization': `Bearer ${token}`,
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(updatedStaff)
            });

            if (response.ok) {
                const updatedStaffData = await response.json();
                displayStaffInfo(updatedStaffData);
                editButton.style.display = 'block';
                saveButton.style.display = 'none';
                alert('Information updated successfully!');
            } else {
                throw new Error(`Failed to update staff info: ${response.status} ${response.statusText}`);
            }
        } catch (error) {
            console.error('Error:', error.message);
        }
    };

    const staffData = await fetchStaffInfo();

    editButton.addEventListener('click', () => {
        enableEditing(staffData);
        handleEdit();
    });

    saveButton.addEventListener('click', handleSave);

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

});