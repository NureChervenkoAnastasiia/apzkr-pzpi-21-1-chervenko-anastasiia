document.addEventListener('DOMContentLoaded', async function () {
    const apiUrl = 'https://localhost:7206/api/Table/';
    const tablesTableBody = document.querySelector('#tables-table tbody');
    const addButton = document.querySelector('.btn-add');
    const localizedText = {};

    const getToken = () => localStorage.getItem('token');

    const fetchTables = async () => {
        try {
            const response = await fetch(apiUrl, {
                headers: {
                    'Authorization': `Bearer ${getToken()}`,
                    'Content-Type': 'application/json'
                }
            });

            if (response.ok) {
                const tables = await response.json();
                displayTables(Array.isArray(tables) ? tables : []);
            } else {
                console.error('Error:', await response.text());
            }
        } catch (error) {
            console.error('Error:', error.message);
        }
    };

    const displayTables = (tables) => {
        tablesTableBody.innerHTML = '';
        tables.forEach(table => {
            const row = document.createElement('tr');
            row.innerHTML = `
                <td>${table.number}</td>
                <td>${table.status}</td>
                <td><button class="btn-edit" data-tableid="${table.id}">Edit</button></td>
                <td><button class="btn-delete" data-tableid="${table.id}">Delete</button></td>
            `;
            tablesTableBody.appendChild(row);
        });
    };

    const handleDelete = async (tableId) => {
        if (!tableId) return console.error('Error: Table ID is undefined');
        try {
            const response = await fetch(`${apiUrl}${tableId}`, {
                method: 'DELETE',
                headers: {
                    'Authorization': `Bearer ${getToken()}`,
                    'Content-Type': 'application/json'
                }
            });

            if (response.ok) {
                alert('Table was deleted successfully!');
                await fetchTables();
                location.reload();
            } else {
                console.error('Error:', await response.text());
            }
        } catch (error) {
            console.error('Error:', error.message);
        }
    };

    const handleEdit = (tableId) => {
        if (!tableId) return console.error('Error: Table ID is undefined');
        const row = document.querySelector(`button[data-tableid="${tableId}"]`).closest('tr');
        const [numberCell, statusCell] = row.cells;

        numberCell.innerHTML = `<input type="text" value="${numberCell.textContent}">`;
        statusCell.innerHTML = `<input type="text" value="${statusCell.textContent}">`;
        row.cells[2].innerHTML = `<button class="btn-save" data-tableid="${tableId}">Save</button>`;
        row.cells[3].innerHTML = `<button class="btn-cancel" data-tableid="${tableId}">Cancel</button>`;
    };

    const handleSave = async (tableId) => {
        if (!tableId) return console.error('Error: Table ID is undefined');
        const row = document.querySelector(`button[data-tableid="${tableId}"]`).closest('tr');
        const number = row.cells[0].querySelector('input').value;
        const status = row.cells[1].querySelector('input').value;

        if (!number || !status) {
            alert('Please fill in all fields');
            return;
        }

        try {
            const response = await fetch(`${apiUrl}${tableId}`, {
                method: 'PUT',
                headers: {
                    'Authorization': `Bearer ${getToken()}`,
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ number, status })
            });

            if (response.ok) {
                alert('Table was updated successfully!');
                await fetchTables();
                location.reload();
            } else {
                console.error('Error:', await response.json());
            }
        } catch (error) {
            console.error('Error:', error.message);
        }
    };

    const handleCancel = async () => {
        await fetchTables();
        location.reload();
    };

    const handleAdd = async () => {
        const number = document.getElementById('input-number').value;
        const status = document.getElementById('input-status').value;

        if (!number || !status) {
            alert('Please fill in all fields');
            return;
        }

        try {
            const response = await fetch(apiUrl, {
                method: 'POST',
                headers: {
                    'Authorization': `Bearer ${getToken()}`,
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ number, status })
            });

            if (response.ok) {
                alert('Table was added successfully!');
                await fetchTables();
                location.reload();
            } else {
                console.error('Error:', await response.text());
            }
        } catch (error) {
            console.error('Error:', error.message);
        }
    };

    // Event delegation for dynamic buttons
    tablesTableBody.addEventListener('click', async (event) => {
        const { target } = event;
        const tableId = target.getAttribute('data-tableid');

        if (target.classList.contains('btn-delete')) {
            await handleDelete(tableId);
        } else if (target.classList.contains('btn-edit')) {
            handleEdit(tableId);
        } else if (target.classList.contains('btn-save')) {
            await handleSave(tableId);
        } else if (target.classList.contains('btn-cancel')) {
            await handleCancel();
        }
    });

    // Add button event listener
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

    // Fetch tables initially
    await fetchTables();
});