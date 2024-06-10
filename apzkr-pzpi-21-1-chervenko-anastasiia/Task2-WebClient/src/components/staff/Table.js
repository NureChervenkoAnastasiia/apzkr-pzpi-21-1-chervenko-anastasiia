document.addEventListener('DOMContentLoaded', async function() {
    const apiUrl = 'https://localhost:7206/api/Table/';
    const tablesTableBody = document.querySelector('#tables-table tbody');
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

            if (!response.ok) {
                const error = await response.text();
                throw new Error(error);
            }

            const tables = await response.json();
            if (!Array.isArray(tables)) {
                throw new Error('Fetched tables is not an array');
            }

            displayTables(tables);
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

    const handleEdit = (tableId) => {
        const row = document.querySelector(`button[data-tableid="${tableId}"]`).parentNode.parentNode;
        const [numberCell, statusCell, editButtonCell, deleteButtonCell] = row.cells;

        const number = numberCell.textContent;
        const status = statusCell.textContent;

        numberCell.innerHTML = `<input type="text" value="${number}">`;
        statusCell.innerHTML = `<input type="text" value="${status}">`;

        editButtonCell.innerHTML = `<button class="btn-save" data-tableid="${tableId}">Save</button>`;
        deleteButtonCell.innerHTML = `<button class="btn-cancel" data-tableid="${tableId}">Cancel</button>`;
    };

    const handleSave = async (tableId) => {
        const row = document.querySelector(`button[data-tableid="${tableId}"]`).parentNode.parentNode;
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

            if (!response.ok) {
                const error = await response.json();
                throw new Error(error);
            }

            alert('Table was updated successfully!');
            await fetchTables();
        } catch (error) {
            console.error('Error:', error.message);
        }
    };

    const handleCancel = (tableId) => {
        const row = document.querySelector(`button[data-tableid="${tableId}"]`).parentNode.parentNode;
        const number = row.cells[0].querySelector('input').value;
        const status = row.cells[1].querySelector('input').value;

        row.innerHTML = `
            <td>${number}</td>
            <td>${status}</td>
            <td><button class="btn-edit" data-tableid="${tableId}">Edit</button></td>
            <td><button class="btn-delete" data-tableid="${tableId}">Delete</button></td>
        `;
    };

    const handleDelete = async (tableId) => {
        if (!confirm('Are you sure you want to delete this table?')) {
            return;
        }

        try {
            const response = await fetch(`${apiUrl}${tableId}`, {
                method: 'DELETE',
                headers: {
                    'Authorization': `Bearer ${getToken()}`,
                    'Content-Type': 'application/json'
                }
            });

            if (!response.ok) {
                const error = await response.text();
                throw new Error(error);
            }

            alert('Table was deleted successfully!');
            await fetchTables();
        } catch (error) {
            console.error('Error:', error.message);
        }
    };

    tablesTableBody.addEventListener('click', function(event) {
        const target = event.target;
        const tableId = target.getAttribute('data-tableid');

        if (target.classList.contains('btn-edit')) {
            handleEdit(tableId);
        } else if (target.classList.contains('btn-save')) {
            handleSave(tableId);
        } else if (target.classList.contains('btn-cancel')) {
            handleCancel(tableId);
        } else if (target.classList.contains('btn-delete')) {
            handleDelete(tableId);
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
});