import * as utility from "./site.js";

const token = sessionStorage.getItem("token");

// setting initial page value
sessionStorage.setItem("currentPage", 1);

utility.showItemsOnLoadAsync("players");
utility.loadNextPageAsync("players");
utility.loadPreviousPageAsync("players");
utility.loadAllItemsAsync("players");
utility.getItemAsync("players");
utility.deleteItemAsync("players");

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

// POST request to change a player's role
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