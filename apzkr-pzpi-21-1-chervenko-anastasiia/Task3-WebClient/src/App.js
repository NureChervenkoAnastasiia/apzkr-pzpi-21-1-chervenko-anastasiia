document.addEventListener("DOMContentLoaded", function() {
  const ordersPage = document.getElementById("orders");
  const bookingsPage = document.getElementById("bookings");
  const tablesPage = document.getElementById("tables");
  const staffPage = document.getElementById("staff");
  const schedulePage = document.getElementById("schedule");
  const productsPage = document.getElementById("products");
  const restaurantPage = document.getElementById("restaurant");
  const menuPage = document.getElementById("menu");
  const profilePage = document.getElementById("profile");
  const databasePage = document.getElementById("database");
  const logoutPage = document.getElementById("logout");

  const role = getUserRole();
  console.log('User Role:', role);

  const basePath = window.location.pathname.split('/').slice(0, -2).join('/');

  function navigateTo(page) {
    const subdir = role === 'admin' ? 'admin' : 'staff';
    window.location.href = `${basePath}/${subdir}/${page}`;
  }

  if (role === 'admin') {
    ordersPage.addEventListener("click", () => navigateTo("OrdersPage.html"));
    bookingsPage.addEventListener("click", () => navigateTo("BookingsPage.html"));
    tablesPage.addEventListener("click", () => navigateTo("TablesPage.html"));
    staffPage.addEventListener("click", () => navigateTo("StaffPage.html"));
    schedulePage.addEventListener("click", () => navigateTo("SchedulePage.html"));
    productsPage.addEventListener("click", () => navigateTo("ProductsPage.html"));
    restaurantPage.addEventListener("click", () => navigateTo("RestaurantPage.html"));
    menuPage.addEventListener("click", () => navigateTo("MenuPage.html"));
    databasePage.addEventListener("click", () => navigateTo("DataPage.html"));
  } else if (role === 'worker') {
    ordersPage.addEventListener("click", () => navigateTo("OrdersPage.html"));
    menuPage.addEventListener("click", () => navigateTo("MenuPage.html"));
    profilePage.addEventListener("click", () => navigateTo("ProfilePage.html"));
    schedulePage.addEventListener("click", () => navigateTo("SchedulePage.html"));
    tablesPage.addEventListener("click", () => navigateTo("TablesPage.html"));
  } else {
    console.error('Unauthorized access');
  }

  logoutPage.addEventListener("click", function() {
    localStorage.removeItem('token');
    localStorage.removeItem('userData');
    window.location.href = `${basePath}/LoginPage.html`;
  });
});

const getToken = () => {
  const token = localStorage.getItem('token');
  return token;
};

const parseJwt = (token) => {
  try {
    const base64Url = token.split('.')[1];
    const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
    const jsonPayload = decodeURIComponent(atob(base64).split('').map(function(c) {
      return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
    }).join(''));
    return JSON.parse(jsonPayload);
  } catch (e) {
    console.error('Error parsing token:', e);
    return null;
  }
};

const getUserRole = () => {
  const token = getToken();
  if (!token) {
    console.error('Error: Token not found in localStorage');
    return null;
  }
  const decodedToken = parseJwt(token);
  return decodedToken ? decodedToken.role : null;
};
