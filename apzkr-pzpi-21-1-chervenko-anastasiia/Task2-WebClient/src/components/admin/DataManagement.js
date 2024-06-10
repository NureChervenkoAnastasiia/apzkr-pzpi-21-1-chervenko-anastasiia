document.addEventListener("DOMContentLoaded", function () {
    const backupTableBody = document.querySelector("#backups-table tbody");
    const createBackupBtn = document.querySelector(".btn-add");
    const apiUrl = 'https://localhost:7206/api/backup';
    const localizedText = {};

    async function fetchBackups() {
        try {
            const token = localStorage.getItem('token');

            const response = await fetch(`${apiUrl}/list`, {
                headers: {
                    'Authorization': `Bearer ${token}`
                }
            });

            if (!response.ok) {
                throw new Error(`Error fetching backups: ${response.statusText}`);
            }

            const backups = await response.json();
            populateBackupTable(backups);
        } catch (error) {
            console.error('Error fetching backups:', error);
            alert(`Error fetching backups: ${error.message}`);
        }
    }

    function populateBackupTable(backups) {
        console.log("Populating backup table with:", backups);
        backupTableBody.innerHTML = '';
        backups.forEach(backup => {
            const row = document.createElement('tr');
    
            const nameCell = document.createElement('td');
            nameCell.textContent = backup;
    
            const restoreCell = document.createElement('td');
            const restoreButton = document.createElement('button');
            restoreButton.textContent = 'Restore';
            restoreButton.classList.add('restore-button'); // Добавление класса к кнопке
            restoreButton.addEventListener('click', () => restoreBackup(backup));
            restoreCell.appendChild(restoreButton);
    
            row.appendChild(nameCell);
            row.appendChild(restoreCell);
    
            backupTableBody.appendChild(row);
        });
    }
    

    async function createBackup() {
        try {
            console.log("Creating a new backup...");
            const response = await fetch(`${apiUrl}/export-data`, {
                method: 'POST',
                headers: {
                    'Authorization': `Bearer ${localStorage.getItem('token')}`
                }
            });

            if (!response.ok) {
                const errorData = await response.json();
                throw new Error(`Error creating backup: ${errorData.message}`);
            }

            alert('Backup created successfully');
            fetchBackups(); // Refresh the backup list
            location.reload();
        } catch (error) {
            console.error('Error creating backup:', error);
            alert(`Error creating backup: ${error.message}`);
        }
    }

    async function restoreBackup(backupFileName) {
        const formData = new FormData();
        formData.append('backupFileName', backupFileName);

        try {
            console.log(`Restoring backup: ${backupFileName}...`);
            const response = await fetch(`${apiUrl}/import-data`, {
                method: 'POST',
                headers: {
                    'Authorization': `Bearer ${localStorage.getItem('token')}`
                },
                body: formData
            });

            if (!response.ok) {
                const errorData = await response.json();
                throw new Error(`Error restoring database: ${errorData.message}`);
            }

            alert('Database restored successfully');
        } catch (error) {
            console.error('Error restoring database:', error);
            alert(`Error restoring database: ${error.message}`);
        }
    }

    createBackupBtn.addEventListener('click', createBackup);

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
    
    // Fetch and display backups on page load
    fetchBackups();
});
