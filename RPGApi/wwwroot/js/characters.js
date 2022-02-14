import * as utility from "./site.js";

const token = sessionStorage.getItem("token");

// setting initial page value
sessionStorage.setItem("currentPage", 1);

utility.showItemsOnLoad("characters");
utility.loadNextPageOnClick("characters");
utility.loadPreviousPageOnClick("characters");
utility.loadAllItemsOnClick("characters");
utility.makeGetRequest("characters");
utility.makeDeleteRequest("characters");

// POST request to create a character
document.querySelector("#create-btn").addEventListener("click", async e => {
    const charName = document.querySelector("#create-name").value;
    const charPlayerId = document.querySelector("#create-player-id").value;
    const charRace = document.querySelector("#create-race").value;

    await fetch("../api/characters", {
        method: "POST",
        headers: {
            "Accept": "application/json",
            "Content-Type": "application/json",
            "Authorization": `Bearer ${token}`
        },
        body: JSON.stringify({
            name: charName,
            race: charRace,
            playerId: charPlayerId
        })
    })
        .then(response => {
            if (response.ok) {
                return response.json();
            } else {
                throw new Error("You provided incorrect data");
            }
        })
        .then(data => utility.addItemToTable(data["id"], charName,
            charRace, 100, charPlayerId, [], [], []))
        .catch(error => alert(error.message));
});

// PUT request to edit a character
document.querySelector("#edit-btn").addEventListener("click", async e => {
    const charId = document.getElementById("edit-id").value;
    const charTr = document.getElementsByClassName(`${charId}-tr`)[0];
    const newName = document.getElementById("edit-name").value;
    const newRace = document.getElementById("edit-race").value;

    await fetch(`../api/characters/${charId}`, {
        method: "PUT",
        headers: {
            "Accept": "application/json",
            "Content-Type": "application/json",
            "Authorization": `Bearer ${token}`
        },
        body: JSON.stringify({
            name: newName,
            race: newRace
        })
    })
        .then(response => {
            if (response.ok) {
                charTr.children[1].innerHTML = newName;
                charTr.children[2].innerHTML = newRace;
            } else {
                throw new Error("You provided incorrect data");
            }
        })
        .catch(error => alert(error.message));
});

// PUT request to add an item to a character
document.querySelector("#add-remove-item-btn").addEventListener("click", async e => {
    const actionType = document.querySelector("#action-type").value;
    const itemType = document.querySelector("#item-type").value;
    const charId = document.getElementById("add-remove-item-charId").value;
    const itemId = document.getElementById("add-remove-item-itemId").value;
    const charTr = document.getElementsByClassName(`${charId}-tr`)[0];
    let itemTd;

    await fetch(`../api/characters/${actionType}/${itemType}`, {
        method: "PUT",
        headers: {
            "Accept": "application/json",
            "Content-Type": "application/json",
            "Authorization": `Bearer ${token}`
        },
        body: JSON.stringify({
            characterId: charId,
            itemId: itemId
        })
    })
        .then(response => {
            if (response.ok) {
                if (itemType == "weapon") {
                    itemTd = charTr.children[charTr.children.length - 3];
                } else if (itemType == "spell") {
                    itemTd = charTr.children[charTr.children.length - 2];
                } else {
                    itemTd = charTr.children[charTr.children.length - 1];
                }

                if (actionType == "add") {
                    const span = document.createElement("span");
                    span.innerHTML = itemId;

                    if (itemTd.children.length > 0) {
                        itemTd.appendChild(document.createElement("hr"));
                    }

                    itemTd.appendChild(span);
                } else {
                    itemTd.removeChild(itemTd.lastChild);

                    if (itemTd.children.length > 0) {
                        itemTd.removeChild(itemTd.lastChild);
                    }
                }
            } else {
                throw new Error("You provided incorrect data");
            }
        })
        .catch(error => alert(error.message));
});