document.addEventListener('DOMContentLoaded', async () => {
    const apiUrl = 'https://localhost:7206/api/Menu/';
    const filterButton = document.getElementById('filter-button');
    const menuTableBody = document.querySelector('#menu-table tbody');
    const menuTableContainer = document.querySelector('.table-container');
    const popularityContainer = document.getElementById('popularity-container');
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
        return staffData ? staffData.restaurantId : null;
    };

    const fetchMenu = async (endpoint) => {
        const data = await fetchWithAuth(endpoint);
        if (data) {
            if (endpoint.includes('dishes-rating')) {
                displayPopularity(data);
            } else {
                displayMenu(data);
            }
        }
    };

    const displayMenu = (menuItems) => {
        menuTableBody.innerHTML = '';
        popularityContainer.style.display = 'none';
        menuTableContainer.style.display = 'block';

        menuItems.forEach(item => {
            const row = document.createElement('tr');
            row.innerHTML = `
                <td>${item.name}</td>
                <td>${item.size}</td>
                <td>${item.price}</td>
                <td>${item.info}</td>
                <td>${item.type}</td>
                <td><button class="btn-edit" data-menuid="${item.id}">Edit</button></td>
                <td><button class="btn-delete" data-menuid="${item.id}">Delete</button></td>
            `;
            menuTableBody.appendChild(row);
        });
    };

    const displayPopularity = (popularityItems) => {
        menuTableContainer.style.display = 'none';
        popularityContainer.style.display = 'block';
        popularityContainer.innerHTML = '';

        popularityItems.forEach(item => {
            const popularityEntry = document.createElement('div');
            popularityEntry.textContent = `${item.name} - ${item.ordersCount} замовлень`;
            popularityContainer.appendChild(popularityEntry);
        });
    };

    const getSelectedFilter = () => document.querySelector('input[name="filter"]:checked').value;

    const getEndpoint = (filter, restaurantId) => {
        const endpoints = {
            'first-dishes': `${apiUrl}restaurant/${restaurantId}/first-dishes`,
            'second-dishes': `${apiUrl}restaurant/${restaurantId}/second-dishes`,
            'drinks': `${apiUrl}restaurant/${restaurantId}/drinks`,
            'popularity': `${apiUrl}restaurant/${restaurantId}/dishes-rating`,
            'default': `${apiUrl}restaurant/${restaurantId}/menu`,
        };
        return endpoints[filter] || endpoints['default'];
    };

    const handleDelete = async (menuId) => {
        if (!menuId) {
            console.error('Error: Menu ID is undefined');
            return;
        }

        const response = await fetchWithAuth(`${apiUrl}${menuId}`, { method: 'DELETE' });
        if (response) {
            alert('Dish or drink was deleted successfully!');
            await fetchMenu();
            location.reload();
        }
    };

    const handleEdit = (menuId) => {
        if (!menuId) {
            console.error('Error: Menu ID is undefined');
            return;
        }

        const row = document.querySelector(`button[data-menuid="${menuId}"]`).parentNode.parentNode;
        const [nameCell, sizeCell, priceCell, infoCell, typeCell, editButtonCell, deleteButtonCell] = row.cells;

        nameCell.innerHTML = `<input type="text" value="${nameCell.textContent}">`;
        sizeCell.innerHTML = `<input type="text" value="${sizeCell.textContent}">`;
        priceCell.innerHTML = `<input type="text" value="${priceCell.textContent}">`;
        infoCell.innerHTML = `<input type="text" value="${infoCell.textContent}">`;
        typeCell.innerHTML = `
            <select id="input-type">
                <option value="Перші страви" ${typeCell.textContent === 'Перші страви' ? 'selected' : ''}>Перші страви</option>
                <option value="Другі страви" ${typeCell.textContent === 'Другі страви' ? 'selected' : ''}>Другі страви</option>
                <option value="Напої" ${typeCell.textContent === 'Напої' ? 'selected' : ''}>Напої</option>
            </select>
        `;

        editButtonCell.innerHTML = `<button class="btn-save" data-menuid="${menuId}">Save</button>`;
        deleteButtonCell.innerHTML = `<button class="btn-cancel" data-menuid="${menuId}">Cancel</button>`;
    };

    const handleSave = async (menuId) => {
        if (!menuId) {
            console.error('Error: Menu ID is undefined');
            return;
        }

        const row = document.querySelector(`button[data-menuid="${menuId}"]`).parentNode.parentNode;
        const name = row.cells[0].querySelector('input').value;
        const size = row.cells[1].querySelector('input').value;
        const price = row.cells[2].querySelector('input').value;
        const info = row.cells[3].querySelector('input').value;
        const type = row.cells[4].querySelector('select').value;

        if (!name || !size || !price || !info || !type) {
            alert('Please fill in all fields');
            return;
        }

        const restaurantId = await fetchStaff();
        if (!restaurantId) {
            console.error('Error: Could not fetch restaurant ID');
            return;
        }

        const response = await fetchWithAuth(`${apiUrl}${menuId}`, {
            method: 'PUT',
            body: JSON.stringify({ restaurantId, name, size, price, info, type })
        });

        if (response) {
            alert('Dish or drink was updated successfully!');
            await fetchMenu();
            location.reload();
        }
    };

    const handleCancel = (menuId) => {
        if (!menuId) {
            console.error('Error: Menu ID is undefined');
            return;
        }

        const row = document.querySelector(`button[data-menuid="${menuId}"]`).parentNode.parentNode;
        const cells = row.cells;
        const [name, size, price, info, type] = [cells[0].querySelector('input').value, cells[1].querySelector('input').value, cells[2].querySelector('input').value, cells[3].querySelector('input').value, cells[4].querySelector('select').value];

        row.innerHTML = `
            <td>${name}</td>
            <td>${size}</td>
            <td>${price}</td>
            <td>${info}</td>
            <td>${type}</td>
            <td><button class="btn-edit" data-menuid="${menuId}">Edit</button></td>
            <td><button class="btn-delete" data-menuid="${menuId}">Delete</button></td>
        `;
    };

    const handleAdd = async () => {
        const name = document.getElementById('input-name').value;
        const size = document.getElementById('input-size').value;
        const price = document.getElementById('input-price').value;
        const info = document.getElementById('input-info').value;
        const type = document.getElementById('input-type').value;

        if (!name || !size || !price || !info || !type) {
            alert('Please fill in all fields');
            return;
        }

        const restaurantId = await fetchStaff();
        if (!restaurantId) {
            console.error('Error: Could not fetch restaurant ID');
            return;
        }

        const response = await fetchWithAuth(apiUrl, {
            method: 'POST',
            body: JSON.stringify({ restaurantId, name, size, price, info, type })
        });

        if (response) {
            alert('Dish or drink was added successfully!');
            await fetchMenu();
            location.reload();
        }
    };

    filterButton.addEventListener('click', async () => {
        const filter = getSelectedFilter();
        const restaurantId = await fetchStaff();
        if (restaurantId) {
            fetchMenu(getEndpoint(filter, restaurantId));
        } else {
            console.error('Error: Could not fetch restaurant ID');
        }
    });

    menuTableBody.addEventListener('click', (event) => {
        const target = event.target;
        const menuId = target.dataset.menuid;

        if (target.classList.contains('btn-edit')) {
            handleEdit(menuId);
        } else if (target.classList.contains('btn-delete')) {
            handleDelete(menuId);
        } else if (target.classList.contains('btn-save')) {
            handleSave(menuId);
        } else if (target.classList.contains('btn-cancel')) {
            handleCancel(menuId);
        }
    });

    const addButton = document.querySelector('.btn-add');
    addButton.addEventListener('click', handleAdd);

    const restaurantId = await fetchStaff();
    if (restaurantId) {
        fetchMenu(getEndpoint('menu', restaurantId));
    } else {
        console.error('Error: Could not fetch restaurant ID');
    }

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
