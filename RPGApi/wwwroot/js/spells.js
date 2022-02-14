import * as utility from "./site.js";

const token = sessionStorage.getItem("token");

// setting initial page value
sessionStorage.setItem("currentPage", 1);

utility.showItemsOnLoadAsync("spells");
utility.loadNextPageAsync("spells");
utility.loadPreviousPageAsync("spells");
utility.loadAllItemsAsync("spells");
utility.getItemAsync("spells");
utility.deleteItemAsync("spells");
utility.hitCharacterAsync("spells");

// POST request to create a spell
document.querySelector("#create-btn").addEventListener("click", async e => {
    const spellName = document.querySelector("#create-name").value;
    const spellType = document.querySelector("#create-type").value;
    let spellDamage = document.querySelector("#create-damage").value;

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
        .then(data => utility.addItemToTableAsync(data["id"], spellName, spellType, spellDamage, []))
        .catch(error => alert(error.message));
});

// PUT request to edit a spell
document.querySelector("#edit-btn").addEventListener("click", async e => {
    const spellId = document.getElementById("edit-id").value;
    const spellTr = document.getElementsByClassName(`${spellId}-tr`)[0];
    const newName = document.getElementById("edit-name").value;
    const newType = document.getElementById("edit-type").value;
    const newDamage = document.getElementById("edit-damage").value;

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