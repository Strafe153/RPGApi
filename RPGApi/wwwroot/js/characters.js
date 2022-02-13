import * as utility from "./site.js";

const token = sessionStorage.getItem("token");

// setting initial page value
sessionStorage.setItem("currentPage", 1);

window.addEventListener("load", async e => {
    const userRole = sessionStorage.getItem("userRole");
    const currentPage = sessionStorage.getItem("currentPage");

    document.querySelector("#log-out-btn").innerHTML = `Log Out (${sessionStorage.getItem("username")})`;
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
            utility.displayItems(data.items, "characters-tbody");
        });

    if (userRole == "0") {
        const manageDivs = document.querySelectorAll(".manage-div");

        for (let div of manageDivs) {
            div.classList.remove("d-none");
            div.classList.add("d-flex");
        }
    }

    if (currentPage < sessionStorage.getItem("pagesCount")) {
        document.querySelector("#next-btn").style.display = "inline";
    }
});

// load characters from the previous page
document.querySelector("#next-btn").addEventListener("click", async e => {
    const page = sessionStorage.getItem("currentPage");
    sessionStorage.setItem("currentPage", parseInt(page) + 1);

    await utility.getItems("characters");
});

// load characters from the next page
document.querySelector("#prev-btn").addEventListener("click", async e => {
    const page = sessionStorage.getItem("currentPage");
    sessionStorage.setItem("currentPage", parseInt(page) - 1);

    await utility.getItems("characters");
});

// GET request to find a character
document.querySelector("#find-char-btn").addEventListener("click", async e => {
    await fetch(`../api/characters/${document.getElementById("find-char-id").value}`, {
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
                throw new Error("The character with the provided id does not exist");
            }
        })
        .then(data => {
            utility.displayItems([data], "characters-tbody");

            document.querySelector("#all-items-btn").style.display = "inline";
            document.querySelector("#prev-btn").style.display = "none";
            document.querySelector("#curr-page").style.display = "none";
            document.querySelector("#next-btn").style.display = "none";
        })
        .catch(error => alert(error.message));
});

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
        .then(data => utility.addItemToTable("characters-tbody", data["id"],
            charName, charRace, 100, charPlayerId, [], [], []))
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
    })
        .then(response => {
            if (response.ok) {
                document.getElementsByClassName(`${charId}-tr`)[0].remove();
            } else {
                throw new Error("You provided incorrect id");
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

document.querySelector("#all-items-btn").addEventListener("click", async e => {
    await utility.getItems("characters");
    document.querySelector("#all-items-btn").style.display = "none";
    document.querySelector("#curr-page").style.display = "inline";
});