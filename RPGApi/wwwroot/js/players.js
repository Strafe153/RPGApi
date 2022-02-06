﻿const token = sessionStorage.getItem("token");
const currentPageElem = document.getElementById("curr-page");

// setting initial page value
sessionStorage.setItem("currentPage", 1);

async function getPlayers() {
    const currentPage = sessionStorage.getItem("currentPage");
    const prevButton = document.querySelector("#prev-btn");
    const nextButton = document.querySelector("#next-btn");

    if (currentPage > 1) {
        prevButton.style.display = "inline";
    } else {
        prevButton.style.display = "none";
    }

    if (currentPage < sessionStorage.getItem("pagesCount")) {
        nextButton.style.display = "inline";
    } else {
        nextButton.style.display = "none";
    }

    await fetch(`../api/players/page/${currentPage}`, {
        method: "GET",
        headers: {
            "Authorization": `Bearer ${token}`
        }
    })
        .then(response => response.json())
        .then(data => {
            currentPageElem.value = sessionStorage.getItem("currentPage");
            displayPlayers(data.items, "players-tbody");
        });
}

function displayPlayers(players, tbodyId) {
    const tbody = document.querySelector(`#${tbodyId}`);
    tbody.innerHTML = "";

    players.forEach(player => {
        let tr = tbody.insertRow();
        tr.classList.add(`${player.id}-tr`);
        let td;

        td = tr.insertCell(0);
        td.appendChild(document.createTextNode(player.id));

        td = tr.insertCell(1);
        td.appendChild(document.createTextNode(player.name));

        td = tr.insertCell(2);
        td.appendChild(document.createTextNode(player.role));
    });
}

window.addEventListener("load", async e => {
    const userRole = sessionStorage.getItem("userRole");
    const currentPage = sessionStorage.getItem("currentPage");

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
            displayPlayers(data.items, "players-tbody");
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

    await getPlayers();
});

// load players from the next page
document.querySelector("#prev-btn").addEventListener("click", async e => {
    const page = sessionStorage.getItem("currentPage");
    sessionStorage.setItem("currentPage", parseInt(page) - 1);

    await getPlayers();
});

// GET request to find a user
document.querySelector("#find-player-btn").addEventListener("click", async e => {
    await fetch(`../api/players/${document.getElementById("find-player-id").value}`, {
        method: "GET",
        headers: {
            "Authorization": `Bearer ${token}`
        }
    })
        .then(response => response.json())
        .then(data => {
            displayPlayers([data], "players-tbody");
            document.querySelector("#all-players-btn").style.display = "inline";
            document.querySelector("#prev-btn").style.display = "none";
            document.querySelector("#curr-page").style.display = "none";
            document.querySelector("#next-btn").style.display = "none";
        });
});

// PUT request to update a user
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
    });

    document.getElementsByClassName(`${playerId}-tr`)[0].children[1].innerHTML = newName;
});

// DELETE request to delete a user
document.querySelector("#del-btn").addEventListener("click", async e => {
    const playerId = document.querySelector("#del-id").value;

    await fetch(`../api/players/${playerId}`, {
        method: "DELETE",
        headers: {
            "Accept": "application/json",
            "Content-Type": "application/json",
            "Authorization": `Bearer ${token}`
        }
    });

    document.getElementsByClassName(`${playerId}-tr`)[0].remove();
});

// POST request to change a user's role
document.querySelector("#change-role-btn").addEventListener("click", async e => {
    const playerId = document.getElementById("change-role-id").value;
    const newRole = document.getElementById("change-role-value").value;

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
    });

    document.getElementsByClassName(`${playerId}-tr`)[0].children[2].innerHTML = newRole;
});

document.querySelector("#all-players-btn").addEventListener("click", async e => {
    await getPlayers();
    document.querySelector("#all-players-btn").style.display = "none";
    document.querySelector("#curr-page").style.display = "inline";
});