import * as utility from "./site.js";

const token = sessionStorage.getItem("token");

// setting initial page value
sessionStorage.setItem("currentPage", 1);

utility.showItemsOnLoad("spells");
utility.loadNextPageOnClick("spells");
utility.loadPreviousPageOnClick("spells");
utility.loadAllItemsOnClick("spells");
utility.makeGetRequest("spells");
utility.makeDeleteRequest("spells");

// POST request to create a spell
document.querySelector("#create-btn").addEventListener("click", async e => {
    const spellName = document.querySelector("#create-name").value;
    const spellType = document.querySelector("#create-type").value;
    let spellDamage = document.querySelector("#create-damage").value;

    if (spellDamage == "") {
        spellDamage = 15;
    }

    await fetch("../api/spells", {
        method: "POST",
        headers: {
            "Accept": "application/json",
            "Content-Type": "application/json",
            "Authorization": `Bearer ${token}`
        },
        body: JSON.stringify({
            name: spellName,
            type: spellType,
            damage: spellDamage
        })
    })
        .then(response => {
            if (response.ok) {
                return response.json();
            } else {
                throw new Error("You provided incorrect data");
            }
        })
        .then(data => utility.addItemToTable(data["id"], spellName, spellType, spellDamage, []))
        .catch(error => alert(error.message));
});

// PUT request to edit a spell
document.querySelector("#edit-btn").addEventListener("click", async e => {
    const spellId = document.getElementById("edit-id").value;
    const spellTr = document.getElementsByClassName(`${spellId}-tr`)[0];
    const newName = document.getElementById("edit-name").value;
    const newType = document.getElementById("edit-type").value;
    let newDamage = document.getElementById("edit-damage").value;

    if (newDamage == "") {
        newDamage = 15;
    }

    await fetch(`../api/spells/${spellId}`, {
        method: "PUT",
        headers: {
            "Accept": "application/json",
            "Content-Type": "application/json",
            "Authorization": `Bearer ${token}`
        },
        body: JSON.stringify({
            name: newName,
            type: newType,
            damage: newDamage
        })
    })
        .then(response => {
            if (response.ok) {
                spellTr.children[1].innerHTML = newName;
                spellTr.children[2].innerHTML = newType;
                spellTr.children[3].innerHTML = newDamage;
            } else {
                throw new Error("You provided incorrect data");
            }
        })
        .catch(error => alert(error.message));
});

// PUT request to hit a character
document.querySelector("#hit-btn").addEventListener("click", async e => {
    const dealer = document.querySelector("#hit-dealer-id").value;
    const receiver = document.querySelector("#hit-receiver-id").value;
    const item = document.querySelector("#hit-item-id").value;

    await fetch("../api/spells/hit", {
        method: "PUT",
        headers: {
            "Accept": "application/json",
            "Content-Type": "application/json",
            "Authorization": `Bearer ${token}`
        },
        body: JSON.stringify({
            dealerId: dealer,
            receiverId: receiver,
            itemId: item
        })
    })
        .then(response => {
            if (!response.ok) {
                throw new Error("You provided incorrect data");
            }
        })
        .catch(error => alert(error.message));
});