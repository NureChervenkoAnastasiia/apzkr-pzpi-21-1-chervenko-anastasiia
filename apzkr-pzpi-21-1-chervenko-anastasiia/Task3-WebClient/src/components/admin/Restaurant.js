document.addEventListener('DOMContentLoaded', async () => {
    const apiUrl = 'https://localhost:7206/api/Restaurants/';
    const restaurantContainer = document.querySelector('.restaurant-container');
    const localizedText = {};

    const getToken = () => localStorage.getItem('token');

    const getUserData = () => {
        try {
            const userData = JSON.parse(localStorage.getItem('userData'));
            return userData?.nameid ? userData : null;
        } catch (error) {
            console.error('Error parsing user data:', error);
            return null;
        }
    };

    const fetchData = async (url, headers) => {
        try {
            const response = await fetch(url, { headers });
            if (response.ok) return response.json();
            console.error('Error:', await response.text());
        } catch (error) {
            console.error('Error:', error.message);
        }
        return null;
    };

    const fetchRestaurantId = async () => {
        const userData = getUserData();
        if (!userData) {
            console.error('Error: User data not found in localStorage');
            return null;
        }

        const token = getToken();
        if (!token) {
            console.error('Error: Token not found in localStorage');
            return null;
        }

        const url = `https://localhost:7206/api/staff/${userData.nameid}`;
        const headers = {
            'Authorization': `Bearer ${token}`,
            'Content-Type': 'application/json'
        };

        const staffData = await fetchData(url, headers);
        return staffData ? staffData.restaurantId : null;
    };

    const fetchRestaurantInfo = async (restaurantId) => {
        const token = getToken();
        if (!token) {
            console.error('Error: Token not found in localStorage');
            return;
        }

        const url = `${apiUrl}${restaurantId}`;
        const headers = {
            'Authorization': `Bearer ${token}`,
            'Content-Type': 'application/json'
        };

        const restaurantData = await fetchData(url, headers);
        if (restaurantData) displayRestaurantInfo(restaurantData);
    };

    const displayRestaurantInfo = (restaurant) => {
        restaurantContainer.innerHTML = `
            <h2>${restaurant.name}</h2>
            <p><strong>Address:</strong> ${restaurant.address}</p>
            <p><strong>Phone:</strong> ${restaurant.phone}</p>
            <p><strong>Email:</strong> <a href="mailto:${restaurant.email}">${restaurant.email}</a></p>
            <p><strong>Description:</strong> ${restaurant.info}</p>
            <p><strong>Cuisine:</strong> ${restaurant.cuisine.join(', ')}</p>
        `;
    };

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

    const restaurantId = await fetchRestaurantId();
    if (restaurantId) {
        await fetchRestaurantInfo(restaurantId);
    } else {
        console.error('Error: Could not fetch restaurant ID');
    }
});