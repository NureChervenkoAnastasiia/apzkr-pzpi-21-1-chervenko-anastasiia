document.addEventListener('DOMContentLoaded', async function() {
    const apiUrl = 'https://localhost:7206/api/Order/';
    const tablesApiUrl = 'https://localhost:7206/api/Table/';
    const ordersTableBody = document.querySelector('#orders-table tbody');
    const addButton = document.querySelector('.btn-add');
    const inputTable = document.getElementById('input-table');
    const inputStatus = document.getElementById('input-status');
    const localizedText = {};

    const getToken = () => localStorage.getItem('token');

    const fetchOrders = async () => {
        try {
            const response = await fetch(apiUrl, {
                headers: {
                    'Authorization': `Bearer ${getToken()}`,
                    'Content-Type': 'application/json'
                }
            });

            if (!response.ok) {
                throw new Error(`Failed to fetch orders: ${response.status} ${response.statusText}`);
            }

            const orders = await response.json();
            displayOrders(orders);
        } catch (error) {
            console.error('Error fetching orders:', error.message);
        }
    };

    const fetchTables = async () => {
        try {
            const response = await fetch(tablesApiUrl, {
                headers: {
                    'Authorization': `Bearer ${getToken()}`,
                    'Content-Type': 'application/json'
                }
            });

            if (!response.ok) {
                throw new Error(`Failed to fetch tables: ${response.status} ${response.statusText}`);
            }

            const tables = await response.json();
            populateTablesDropdown(tables);
        } catch (error) {
            console.error('Error fetching tables:', error.message);
        }
    };

    const populateTablesDropdown = (tables) => {
        inputTable.innerHTML = '';
        tables.forEach(table => {
            const option = document.createElement('option');
            option.value = table.id;
            option.textContent = table.number;
            inputTable.appendChild(option);
        });
    };

    const getTableNumberById = async (tableId) => {
        try {
            const response = await fetch(`${tablesApiUrl}${tableId}`, {
                headers: {
                    'Authorization': `Bearer ${getToken()}`,
                    'Content-Type': 'application/json'
                }
            });

            if (!response.ok) {
                throw new Error(`Failed to fetch table: ${response.status} ${response.statusText}`);
            }

            const table = await response.json();
            return table.number;
        } catch (error) {
            console.error('Error fetching table number:', error.message);
            return null;
        }
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
            `;
            ordersTableBody.appendChild(row);
        }
    };

    const formatDateTime = (dateTime) => {
        const date = new Date(dateTime);
        const formattedDate = date.toLocaleDateString('en-CA');
        const formattedTime = date.toLocaleTimeString('en-GB');
        return `Дата: ${formattedDate}<br>Час: ${formattedTime}`;
    };

    const handleEdit = (orderId) => {
        const row = document.querySelector(`button[data-orderid="${orderId}"]`).parentNode.parentNode;
        const [numberCell, tableCell, dateTimeCell, commentCell, statusCell, editButtonCell, deleteButtonCell] = row.cells;

        const number = numberCell.textContent;
        const tableNumber = tableCell.textContent;
        const dateTime = dateTimeCell.textContent.replace('Дата: ', '').replace(' Час: ', 'T');
        const comment = commentCell.textContent;
        const status = statusCell.textContent;

        numberCell.innerHTML = `<input type="number" value="${number}">`;
        tableCell.innerHTML = `<select>${inputTable.innerHTML}</select>`;
        tableCell.querySelector('select').value = tableNumber;
        dateTimeCell.innerHTML = `<input type="datetime-local" value="${dateTime}">`;
        commentCell.innerHTML = `<input type="text" value="${comment}">`;
        statusCell.innerHTML = `<select>${inputStatus.innerHTML}</select>`;
        statusCell.querySelector('select').value = status;

        editButtonCell.innerHTML = `<button class="btn-save" data-orderid="${orderId}">Save</button>`;
        deleteButtonCell.innerHTML = `<button class="btn-cancel" data-orderid="${orderId}">Cancel</button>`;
    };

    const handleSave = async (orderId) => {
        const row = document.querySelector(`button[data-orderid="${orderId}"]`).parentNode.parentNode;
        const number = row.cells[0].querySelector('input').value;
        const tableId = row.cells[1].querySelector('select').value;
        const orderDateTime = row.cells[2].querySelector('input').value;
        const comment = row.cells[3].querySelector('input').value;
        const status = row.cells[4].querySelector('select').value;

        if (!number || !tableId || !orderDateTime || !status) {
            alert('Please fill in all fields');
            return;
        }

        try {
            const response = await fetch(`${apiUrl}${orderId}`, {
                method: 'PUT',
                headers: {
                    'Authorization': `Bearer ${getToken()}`,
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ number, tableId, orderDateTime, comment, status })
            });

            if (response.ok) {
                alert('Order was updated successfully!');
                await fetchOrders();
            } else {
                console.error('Error saving order:', await response.json());
            }
        } catch (error) {
            console.error('Error saving order:', error.message);
        }
    };

    const handleCancel = (orderId) => {
        fetchOrders();
    };

    ordersTableBody.addEventListener('click', function(event) {
        const target = event.target;
        const orderId = target.getAttribute('data-orderid');

        if (target.classList.contains('btn-edit')) {
            handleEdit(orderId);
        } else if (target.classList.contains('btn-save')) {
            handleSave(orderId);
        } else if (target.classList.contains('btn-cancel')) {
            handleCancel(orderId);
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

    await fetchTables();
    await fetchOrders();
});