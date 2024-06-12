document.addEventListener('DOMContentLoaded', async () => {
    const apiUrl = 'https://localhost:7206/api/Order/';
    const tablesApiUrl = 'https://localhost:7206/api/Table/';
    const ordersTableBody = document.querySelector('#orders-table tbody');
    const addButton = document.querySelector('.btn-add');
    const token = localStorage.getItem('token');
    const localizedText = {};

    const headers = {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
    };

    const fetchData = async (url, options = {}) => {
        try {
            const response = await fetch(url, { headers, ...options });
            if (response.ok) return response.json();
            console.error('Error:', await response.text());
        } catch (error) {
            console.error('Error:', error.message);
        }
    };

    const fetchOrders = async () => {
        const orders = await fetchData(apiUrl);
        if (orders) displayOrders(orders);
    };

    const fetchTables = async () => {
        const tables = await fetchData(tablesApiUrl);
        if (tables) populateTablesDropdown(tables);
    };

    const populateTablesDropdown = (tables) => {
        const tableDropdown = document.getElementById('input-table');
        tableDropdown.innerHTML = tables.map(table => 
            `<option value="${table.id}">${table.number}</option>`
        ).join('');
    };

    const getTableNumberById = async (tableId) => {
        const table = await fetchData(`${tablesApiUrl}${tableId}`);
        return table ? table.number : null;
    };

    const displayOrders = async (orders) => {
        ordersTableBody.innerHTML = '';
        for (const order of orders) {
            const tableNumber = await getTableNumberById(order.tableId);
            if (!tableNumber) {
                console.error(`Table not found for order with ID ${order.id}`);
                continue;
            }

            const formattedDateTime = formatDateTime(order.orderDateTime);
            const row = document.createElement('tr');
            row.innerHTML = `
                <td>${order.number}</td>
                <td>${tableNumber}</td>
                <td>${formattedDateTime}</td>
                <td>${order.comment}</td>
                <td>${order.status}</td>
                <td><button class="btn-edit" data-orderid="${order.id}">Edit</button></td>
                <td><button class="btn-delete" data-orderid="${order.id}">Delete</button></td>
            `;
            ordersTableBody.appendChild(row);  // Добавляем строку в тело таблицы
        }
    };

    const formatDateTime = (dateTime) => {
        const date = new Date(dateTime);
        const formattedDate = date.toLocaleDateString('en-CA'); // YYYY-MM-DD format
        const formattedTime = date.toLocaleTimeString('en-GB'); // HH:MM:SS format
        return `Дата: ${formattedDate}<br>Час: ${formattedTime}`;
    };

    const handleDelete = async (orderId) => {
        const response = await fetchData(`${apiUrl}${orderId}`, { method: 'DELETE' });
        if (response) {
            alert('Order was deleted successfully!');
            fetchOrders();
        }
    };

    const handleEdit = (orderId) => {
        const row = document.querySelector(`button[data-orderid="${orderId}"]`).closest('tr');
        const cells = row.cells;
        const [numberCell, tableCell, dateTimeCell, commentCell, statusCell] = cells;

        const number = numberCell.textContent;
        const tableNumber = tableCell.textContent;
        const dateTime = dateTimeCell.textContent.replace('Дата: ', '').replace(' Час: ', 'T');
        const comment = commentCell.textContent;
        const status = statusCell.textContent;

        numberCell.innerHTML = `<input type="number" value="${number}">`;
        tableCell.innerHTML = `<select>${document.getElementById('input-table').innerHTML}</select>`;
        tableCell.querySelector('select').value = tableNumber;
        dateTimeCell.innerHTML = `<input type="datetime-local" value="${dateTime}">`;
        commentCell.innerHTML = `<input type="text" value="${comment}">`;
        statusCell.innerHTML = `<select>${document.getElementById('input-status').innerHTML}</select>`;
        statusCell.querySelector('select').value = status;

        cells[5].innerHTML = `<button class="btn-save" data-orderid="${orderId}">Save</button>`;
        cells[6].innerHTML = `<button class="btn-cancel" data-orderid="${orderId}">Cancel</button>`;
    };

    const handleSave = async (orderId) => {
        const row = document.querySelector(`button[data-orderid="${orderId}"]`).closest('tr');
        const cells = row.cells;
        const number = cells[0].querySelector('input').value;
        const tableId = cells[1].querySelector('select').value;
        const orderDateTime = cells[2].querySelector('input').value;
        const comment = cells[3].querySelector('input').value;
        const status = cells[4].querySelector('select').value;

        if (!number || !tableId || !orderDateTime || !status) {
            alert('Please fill in all fields');
            return;
        }

        const response = await fetchData(`${apiUrl}${orderId}`, {
            method: 'PUT',
            body: JSON.stringify({ number, tableId, orderDateTime, comment, status })
        });

        if (response) {
            alert('Order was updated successfully!');
            fetchOrders();
        }
    };

    const handleCancel = () => fetchOrders();

    const handleAdd = async () => {
        const number = document.getElementById('input-number').value;
        const tableId = document.getElementById('input-table').value;
        const orderDateTime = document.getElementById('input-datetime').value;
        const comment = document.getElementById('input-comment').value;
        const status = document.getElementById('input-status').value;

        if (!number || !tableId || !orderDateTime || !status) {
            alert('Please fill in all fields');
            return;
        }

        const response = await fetchData(apiUrl, {
            method: 'POST',
            body: JSON.stringify({ number, tableId, orderDateTime, comment, status })
        });

        if (response) {
            alert('Order was added successfully!');
            fetchOrders();
        }
    };

    ordersTableBody.addEventListener('click', (event) => {
        const target = event.target;
        const orderId = target.getAttribute('data-orderid');

        if (target.classList.contains('btn-delete')) handleDelete(orderId);
        else if (target.classList.contains('btn-edit')) handleEdit(orderId);
        else if (target.classList.contains('btn-save')) handleSave(orderId);
        else if (target.classList.contains('btn-cancel')) handleCancel(orderId);
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

    await fetchTables();
    await fetchOrders();
});