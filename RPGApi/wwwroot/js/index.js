const registerDiv = document.querySelector("#register-div");
const loginDiv = document.querySelector("#login-div");
const registerBtn = document.querySelector("#register-btn");
const loginBtn = document.querySelector("#login-btn");

function showRegisterForm() {
    registerDiv.style.display = "block";
    registerBtn.style.display = "none";
    loginDiv.style.display = "none";
    loginBtn.style.display = "inline";
}

function showLoginForm() {
    registerDiv.style.display = "none";
    registerBtn.style.display = "inline";
    loginDiv.style.display = "block";
    loginBtn.style.display = "none";
}

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
        .then(response => response.json())
        .then(data => {
            sessionStorage.setItem("token", data.token);
            sessionStorage.setItem("userRole", data.role);
        });

    window.location.href = "../html/home.html";
});

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
        .then(response => response.json())
        .then(() => showLoginForm());
});