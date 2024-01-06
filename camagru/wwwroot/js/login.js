document.getElementById('loginForm').addEventListener('submit', function(e) {
    e.preventDefault();

    var email = document.getElementById('email').value;
    var password = document.getElementById('password').value;
    var useCookies = true;

    var url = '/login';
    if (useCookies) {
        url += '?useCookies=true';
    }

    fetch(url, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ Email: email, Password: password })
    })

        .then(response => {
            if (response.ok) {
                window.location.href = '/index.html';
            } else {
                alert('Login failed');
            }
        })
        .catch(error => console.error('Error:', error));
});
