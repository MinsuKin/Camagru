document.getElementById('registerForm').addEventListener('submit', function(e) {
    e.preventDefault();

    var email = document.getElementById('registerEmail').value;
    var password = document.getElementById('registerPassword').value;

    fetch('/register', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ email: email, password: password })
    })
        .then(response => {
            if (response.ok) {
                alert('Registration successful');
                window.location.href = '/login';
            } else {
                alert('Registration failed');
            }
        })
        .catch(error => console.error('Error:', error));
});
