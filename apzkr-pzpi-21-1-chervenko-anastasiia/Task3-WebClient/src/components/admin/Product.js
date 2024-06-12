document.addEventListener('DOMContentLoaded', async () => {
    const apiUrl = 'https://localhost:7206/api/Product/';
    const sortButton = document.getElementById('sort-button');
    const productsTableBody = document.querySelector('#tables-table tbody');
    const addButton = document.querySelector('.btn-add');
    const token = localStorage.getItem('token');
    const localizedText = {};
    let products = [];

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

    const fetchProducts = async () => {
        products = await fetchData(apiUrl) || [];
        displayProducts(products);
    };

    const displayProducts = (products) => {
        productsTableBody.innerHTML = products.map(product => `
            <tr>
                <td>${product.name}</td>
                <td>${product.amount}</td>
                <td><button class="btn-edit" data-productid="${product.id}">Edit</button></td>
                <td><button class="btn-delete" data-productid="${product.id}">Delete</button></td>
            </tr>
        `).join('');
    };

    const handleDelete = async (productId) => {
        const response = await fetchData(`${apiUrl}${productId}`, { method: 'DELETE' });
        if (response) {
            alert('Product was deleted successfully!');
            fetchProducts();
        }
    };

    const handleEdit = (productId) => {
        const row = document.querySelector(`button[data-productid="${productId}"]`).closest('tr');
        const [nameCell, amountCell] = row.cells;

        nameCell.innerHTML = `<input type="text" value="${nameCell.textContent}">`;
        amountCell.innerHTML = `<input type="number" value="${amountCell.textContent}">`;

        row.cells[2].innerHTML = `<button class="btn-save" data-productid="${productId}">Save</button>`;
        row.cells[3].innerHTML = `<button class="btn-cancel" data-productid="${productId}">Cancel</button>`;
    };

    const handleSave = async (productId) => {
        const row = document.querySelector(`button[data-productid="${productId}"]`).closest('tr');
        const name = row.cells[0].querySelector('input').value;
        const amount = row.cells[1].querySelector('input').value;

        if (!name || !amount) {
            alert('Please fill in all fields');
            return;
        }

        const response = await fetchData(`${apiUrl}${productId}`, {
            method: 'PUT',
            body: JSON.stringify({ name, amount })
        });

        if (response) {
            alert('Product was updated successfully!');
            fetchProducts();
            location.reload();
        }
    };

    const handleCancel = () => fetchProducts();

    const handleAdd = async () => {
        const name = document.getElementById('input-name').value;
        const amount = document.getElementById('input-amount').value;

        if (!name || !amount) {
            alert('Please fill in all fields');
            return;
        }

        const response = await fetchData(apiUrl, {
            method: 'POST',
            body: JSON.stringify({ name, amount })
        });

        if (response) {
            alert('Product was added successfully!');
            fetchProducts();
            location.reload();
        }
    };

    productsTableBody.addEventListener('click', (event) => {
        const target = event.target;
        const productId = target.getAttribute('data-productid');

        if (target.classList.contains('btn-delete')) handleDelete(productId);
        else if (target.classList.contains('btn-edit')) handleEdit(productId);
        else if (target.classList.contains('btn-save')) handleSave(productId);
        else if (target.classList.contains('btn-cancel')) handleCancel(productId);
    });

    addButton.addEventListener('click', handleAdd);

    const sortProducts = (products, sortParam) => {
        return products.sort((a, b) => {
            switch (sortParam) {
                case 'name-asc':
                    return a.name.localeCompare(b.name);
                case 'name-desc':
                    return b.name.localeCompare(a.name);
                case 'amount-asc':
                    return a.amount - b.amount;
                case 'amount-desc':
                    return b.amount - a.amount;
                default:
                    return 0;
            }
        });
    };

    const handleSort = () => {
        const selectedSort = document.querySelector('input[name="sort"]:checked').value;
        const sortedProducts = sortProducts(products, selectedSort);
        displayProducts(sortedProducts);
    };

    sortButton.addEventListener('click', handleSort);

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

    await fetchProducts();
});