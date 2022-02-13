import * as utility from "./site.js";

const token = sessionStorage.getItem("token");

// setting initial page value
sessionStorage.setItem("currentPage", 1);

window.addEventListener("load", async e => {
    const userRole = sessionStorage.getItem("userRole");
    const currentPage = sessionStorage.getItem("currentPage");

    document.querySelector("#log-out-btn").innerHTML = `Log Out (${sessionStorage.getItem("username")})`;
    document.getElementById("curr-page").value = currentPage;

    await fetch(`../api/players/page/${currentPage}`, {
        method: "GET",
        headers: {
            "Authorization": `Bearer ${token}`
        }
    })
        .then(response => response.json())
        .then(data => {
            sessionStorage.setItem("pagesCount", data.pagesCount);
            sessionStorage.setItem("currentPage", data.currentPage);
            utility.displayItems(data.items);
        });

    if (userRole == "0") {
        const changeRoleDiv = document.querySelector("#change-role-div");
        changeRoleDiv.classList.remove("d-none");
        changeRoleDiv.classList.add("d-inline-block");
    }

    if (currentPage < sessionStorage.getItem("pagesCount")) {
        document.querySelector("#next-btn").style.display = "inline";
    }
});

// load players from the previous page
document.querySelector("#next-btn").addEventListener("click", async e => {
    const page = sessionStorage.getItem("currentPage");
    sessionStorage.setItem("currentPage", parseInt(page) + 1);

    await utility.getItems("players");
});

// load players from the next page
document.querySelector("#prev-btn").addEventListener("click", async e => {
    const page = sessionStorage.getItem("currentPage");
    sessionStorage.setItem("currentPage", parseInt(page) - 1);

    await utility.getItems("players");
});

// GET request to find a player
document.querySelector("#find-btn").addEventListener("click", async e => {
    await fetch(`../api/players/${document.getElementById("find-id").value}`, {
        method: "GET",
        headers: {
            "Authorization": `Bearer ${token}`
        }
    })
        .then(response => {
            if (response.ok) {
                if (response.url.endsWith("/")) {
                    throw new Error("Id is not provided");
                }

                return response.json();
            } else {
                throw new Error("The user with the provided id does not exist");
            }
        })
        .then(data => {
            utility.displayItems([data]);

            document.querySelector("#all-items-btn").style.display = "inline";
            document.querySelector("#prev-btn").style.display = "none";
            document.querySelector("#curr-page").style.display = "none";
            document.querySelector("#next-btn").style.display = "none";
        })
        .catch(error => alert(error.message));
});

// PUT request to update a player
document.querySelector("#edit-btn").addEventListener("click", async e => {
    const playerId = document.querySelector("#edit-id").value;
    const newName = document.querySelector("#edit-name").value;

    await fetch(`../api/players/${playerId}`, {
        method: "PUT",
        headers: {
            "Accept": "application/json",
            "Content-Type": "application/json",
            "Authorization": `Bearer ${token}`
        },
        body: JSON.stringify({
            name: newName
        })
    })
        .then(response => {
            if (response.ok) {
                document.getElementsByClassName(`${playerId}-tr`)[0].children[1].innerHTML = newName;
            } else {
                throw new Error("You provided incorrect data");
            }
        })
        .catch(error => alert(error.message));
});

// DELETE request to delete a player
document.querySelector("#del-btn").addEventListener("click", async e => {
    const playerId = document.querySelector("#del-id").value;

    await fetch(`../api/players/${playerId}`, {
        method: "DELETE",
        headers: {
            "Accept": "application/json",
            "Content-Type": "application/json",
            "Authorization": `Bearer ${token}`
        }
    })
        .then(response => {
            if (response.ok) {
                document.getElementsByClassName(`${playerId}-tr`)[0].remove();
            } else {
                throw new Error("You provided incorrect data");
            }
        })
        .catch(error => alert(error.message));
});

// POST request to change a player's role
document.querySelector("#change-role-btn").addEventListener("click", async e => {
    const playerId = document.getElementById("change-role-id").value;
    let newRole = document.getElementById("change-role-value").value;

    if (newRole == "" || newRole < 0 || newRole > 1) {
        newRole = 1;
    }

    await fetch("../api/players/changeRole", {
        method: "POST",
        headers: {
            "Accept": "application/json",
            "Content-Type": "application/json",
            "Authorization": `Bearer ${token}`
        },
        body: JSON.stringify({
            id: playerId,
            role: newRole
        })
    })
        .then(response => {
            if (response.ok) {
                document.getElementsByClassName(`${playerId}-tr`)[0].children[2].innerHTML = newRole;
            } else {
                throw new Error("You provided incorrect data");
            }
        })
        .catch(error => alert(error.message));
});

document.querySelector("#all-items-btn").addEventListener("click", async e => {
    await utility.getItems("players");
    document.querySelector("#all-items-btn").style.display = "none";
    document.querySelector("#curr-page").style.display = "inline";
});