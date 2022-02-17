const registerDiv = document.querySelector("#register-div");
const loginDiv = document.querySelector("#login-div");
const registerBtn = document.querySelector("#register-btn");
const loginBtn = document.querySelector("#login-btn");

function showRegisterForm() {
    registerDiv.classList.remove("d-none");
    registerDiv.classList.add("d-flex");
    loginDiv.classList.remove("d-flex");
    loginDiv.classList.add("d-none");
}

function showLoginForm() {
    registerDiv.classList.remove("d-flex");
    registerDiv.classList.add("d-none");
    loginDiv.classList.remove("d-none");
    loginDiv.classList.add("d-flex");
}

// logins a user and redirects to Players
document.querySelector("#submit-login").addEventListener("click", async e => {
    const loginUsername = document.querySelector("#login-username").value;
    const loginPassword = document.querySelector("#login-password").value;

    await fetch("api/players/login", {
        method: "POST",
        headers: {
            "Accept": "application/json",
            "Content-Type": "application/json"
        },
        body: JSON.stringify({
            name: loginUsername,
            password: loginPassword
        })
    })
        .then(response => {
            if (response.ok) {
                return response.json();
            } else {
                throw new Error("Incorrect login and/or password");
            }
        })
        .then(data => {
            sessionStorage.setItem("token", data.token);
            sessionStorage.setItem("userRole", data.role);
            sessionStorage.setItem("username", loginUsername);
            sessionStorage.setItem("password", loginPassword);

            window.location.href = "../html/players.html";
        })
        .catch(error => alert(error.message));
});

// registers a new user
document.querySelector("#submit-register").addEventListener("click", async e => {
    const registerUsername = document.querySelector("#register-username").value;
    const registerPassword = document.querySelector("#register-password").value;

    await fetch("api/players/register", {
        method: "POST",
        headers: {
            "Accept": "application/json",
            "Content-Type": "application/json"
        },
        body: JSON.stringify({
            name: registerUsername,
            password: registerPassword
        })
    })
        .then(response => {
            if (response.ok) {
                showLoginForm();
            } else {
                throw new Error("The username is already taken");
            }
        })
        .catch(error => alert(error.message));
});