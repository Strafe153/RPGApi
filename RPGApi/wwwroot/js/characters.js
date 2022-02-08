﻿const token = sessionStorage.getItem("token");
const currentPageElem = document.getElementById("curr-page");

// setting initial page value
sessionStorage.setItem("currentPage", 1);

async function getCharacters() {
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

    await fetch(`../api/characters/page/${currentPage}`, {
        method: "GET",
        headers: {
            "Authorization": `Bearer ${token}`
        }
    })
        .then(response => response.json())
        .then(data => {
            currentPageElem.value = sessionStorage.getItem("currentPage");
            displayCharacters(data.items, "characters-tbody");
        });
}

function displayCharacters(characters, tbodyId) {
    const tbody = document.querySelector(`#${tbodyId}`);
    tbody.innerHTML = "";

    characters.forEach(c => addCharacterToTable(tbodyId, c.id, c.name,
        c.race, c.health, c.playerId, c.weapons, c.spells, c.mounts));
}

function addCharacterToTable(tbodyId, ...charProps) {
    const tbody = document.querySelector(`#${tbodyId}`);
    let newCharTr = tbody.insertRow();
    let td;

    newCharTr.classList.add(`${charProps[0]}-tr`);

    for (let i = 0; i < charProps.length; i++) {
        const itemProperty = charProps[i];
        let hr;

        td = newCharTr.insertCell(i);

        if (typeof (itemProperty) === "object") {
            for (let propChild of itemProperty) {
                const span = document.createElement("span");
                span.innerHTML = propChild.id;

                hr = document.createElement("hr");
                td.appendChild(span);
                td.appendChild(hr);
            }

            if (hr != null) {
                td.removeChild(hr);
            }
        } else {
            td.appendChild(document.createTextNode(itemProperty));
        }
    }
}

window.addEventListener("load", async e => {
    const currentPage = sessionStorage.getItem("currentPage");

    document.getElementById("curr-page").value = currentPage;

    await fetch(`../api/characters/page/${currentPage}`, {
        method: "GET",
        headers: {
            "Authorization": `Bearer ${token}`
        }
    })
        .then(response => response.json())
        .then(data => {
            sessionStorage.setItem("pagesCount", data.pagesCount);
            sessionStorage.setItem("currentPage", data.currentPage);
            displayCharacters(data.items, "characters-tbody");
        });

    if (currentPage < sessionStorage.getItem("pagesCount")) {
        document.querySelector("#next-btn").style.display = "inline";
    }
});

// load characters from the previous page
document.querySelector("#next-btn").addEventListener("click", async e => {
    const page = sessionStorage.getItem("currentPage");
    sessionStorage.setItem("currentPage", parseInt(page) + 1);

    await getCharacters();
});

// load characters from the next page
document.querySelector("#prev-btn").addEventListener("click", async e => {
    const page = sessionStorage.getItem("currentPage");
    sessionStorage.setItem("currentPage", parseInt(page) - 1);

    await getCharacters();
});

// GET request to find a character
document.querySelector("#find-char-btn").addEventListener("click", async e => {
    await fetch(`../api/characters/${document.getElementById("find-char-id").value}`, {
        method: "GET",
        headers: {
            "Authorization": `Bearer ${token}`
        }
    })
        .then(response => response.json())
        .then(data => {
            displayCharacters([data], "characters-tbody");
            document.querySelector("#all-chars-btn").style.display = "inline";
            document.querySelector("#prev-btn").style.display = "none";
            document.querySelector("#curr-page").style.display = "none";
            document.querySelector("#next-btn").style.display = "none";
        });
});

// POST request to create a character
document.querySelector("#create-btn").addEventListener("click", async e => {
    const charName = document.querySelector("#create-name").value;
    const charRace = document.querySelector("#create-race").value;
    const charPlayerId = document.querySelector("#create-player-id").value;
    let charId;

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
        .then(response => response.json())
        .then(data => charId = data["id"]);

    addCharacterToTable("characters-tbody", charId, charName, charRace, 100, charPlayerId, [], [], []);
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
    });

    charTr.children[1].innerHTML = newName;
    charTr.children[2].innerHTML = newRace;
});

// DELETE request to delete a character
document.querySelector("#del-btn").addEventListener("click", async e => {
    const charId = document.querySelector("#del-id").value;

    await fetch(`../api/characters/${charId}`, {
        method: "DELETE",
        headers: {
            "Accept": "application/json",
            "Content-Type": "application/json",
            "Authorization": `Bearer ${token}`
        }
    });

    document.getElementsByClassName(`${charId}-tr`)[0].remove();
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
    });

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
});

document.querySelector("#all-chars-btn").addEventListener("click", async e => {
    await getCharacters();
    document.querySelector("#all-chars-btn").style.display = "none";
    document.querySelector("#curr-page").style.display = "inline";
});