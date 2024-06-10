document.getElementById('loginForm').addEventListener('submit', async function(event) {
    event.preventDefault();

    const login = document.getElementById('login').value;
    const password = document.getElementById('password').value;
    const localizedText = {};

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
    
    if (!login || !password) {
        console.error('Error: Login and password fields cannot be empty');
        return;
    }

    try {
        const response = await fetch('https://localhost:7206/api/Staff/login', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ login, password })
        });

        if (!response.ok) {
            const error = await response.text();
            console.error('Error:', error);
            alert('Login failed. Please check your credentials and try again.');
            return;
        }

        const data = await response.json();
        if (data && data.token) {
            localStorage.setItem('token', data.token);

            const decodedToken = jwt_decode(data.token);
            localStorage.setItem('userData', JSON.stringify(decodedToken));

            if (decodedToken.role === 'admin') {
                console.log('Hello admin');
                window.location.href = './admin/MenuPage.html';
            } else {
                console.log('Hello staff');
                window.location.href = './staff/ProfilePage.html';
            }
        } else {
            console.error('Error: Token not found in response');
            alert('Login failed. Token not found in response.');
        }
    } catch (error) {
        console.error('Error:', error.message);
        alert('An unexpected error occurred. Please try again later.');
    }
});