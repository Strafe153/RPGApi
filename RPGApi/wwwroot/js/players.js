import * as utility from "./site.js";

const token = sessionStorage.getItem("token");

// setting initial page value
sessionStorage.setItem("currentPage", 1);

utility.showItemsOnLoadAsync("players");
utility.loadNextPageAsync("players");
utility.loadPreviousPageAsync("players");
utility.loadAllItemsAsync("players");
utility.getItemAsync("players");

// DELETE request to delete a player
document.querySelector("#del-btn").addEventListener("click", async e => {
    const playerId = document.querySelector("#del-id").value;
    const playerTr = document.getElementsByClassName(`${playerId}-tr`)[0];

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
                playerTr.remove();

                if (sessionStorage.getItem("username") == playerTr.children[1].innerHTML) {
                    window.location.href = "../index.html";
                }
            } else {
                throw new Error("Incorrect id/Not enough rights");
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