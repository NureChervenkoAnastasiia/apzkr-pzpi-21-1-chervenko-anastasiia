document.addEventListener('DOMContentLoaded', async () => {
    const apiUrl = 'https://localhost:7206/api/Menu/';
    const filterButton = document.getElementById('filter-button');
    const menuTableBody = document.querySelector('#menu-table tbody');
    const menuTableContainer = document.querySelector('.table-container');
    const popularityContainer = document.getElementById('popularity-container');
    const localizedText = {};

    const getToken = () => localStorage.getItem('token');

    const getUserData = () => {
        try {
            const userData = JSON.parse(localStorage.getItem('userData'));
            if (!userData || !userData.nameid) {
                throw new Error('User data not found or invalid in localStorage');
            }
            return userData;
        } catch (error) {
            console.error('Error:', error.message);
            return null;
        }
    };

    const fetchWithAuth = async (url, options = {}) => {
        const token = getToken();
        if (!token) {
            console.error('Error: Token not found in localStorage');
            return null;
        }

        const headers = {
            'Authorization': `Bearer ${token}`,
            'Content-Type': 'application/json',
            ...options.headers,
        };

        try {
            const response = await fetch(url, { ...options, headers });
            if (!response.ok) {
                const error = await response.text();
                console.error('Error:', error);
                return null;
            }
            return await response.json();
        } catch (error) {
            console.error('Error:', error.message);
            return null;
        }
    };

    const fetchStaff = async () => {
        const userData = getUserData();
        if (!userData) return null;

        const staffData = await fetchWithAuth(`https://localhost:7206/api/staff/${userData.nameid}`);
        if (staffData && staffData.restaurantId) {
            return staffData.restaurantId;
        } else {
            console.error('Error: Invalid staff data received');
            return null;
        }
    };

    const fetchMenu = async (endpoint) => {
        const data = await fetchWithAuth(endpoint);
        if (data) {
            displayMenu(data);
        } else {
            console.error('Error: Failed to fetch menu data');
        }
    };

    const displayMenu = (menuItems) => {
        menuTableBody.innerHTML = '';
        popularityContainer.style.display = 'none';
        menuTableContainer.style.display = 'block';

        if (menuItems.length === 0) {
            console.log('No menu items to display');
            return;
        }

        menuItems.forEach(item => {
            const row = document.createElement('tr');
            row.innerHTML = `
                <td>${item.name}</td>
                <td>${item.size}</td>
                <td>${item.price}</td>
                <td>${item.info}</td>
                <td>${item.type}</td>
            `;
            menuTableBody.appendChild(row);
        });
    };

    const getSelectedFilter = () => document.querySelector('input[name="filter"]:checked')?.value || 'default';

    const getEndpoint = (filter, restaurantId) => {
        const endpoints = {
            'first-dishes': `${apiUrl}restaurant/${restaurantId}/first-dishes`,
            'second-dishes': `${apiUrl}restaurant/${restaurantId}/second-dishes`,
            'drinks': `${apiUrl}restaurant/${restaurantId}/drinks`,
            'default': `${apiUrl}restaurant/${restaurantId}/menu`,
        };
        return endpoints[filter] || endpoints['default'];
    };

    filterButton.addEventListener('click', async () => {
        const filter = getSelectedFilter();
        const restaurantId = await fetchStaff();
        if (restaurantId) {
            await fetchMenu(getEndpoint(filter, restaurantId));
        } else {
            console.error('Error: Could not fetch restaurant ID');
        }
    });

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

    const restaurantId = await fetchStaff();
    if (restaurantId) {
        await fetchMenu(getEndpoint('default', restaurantId));
    } else {
        console.error('Error: Could not fetch restaurant ID');
    }
});