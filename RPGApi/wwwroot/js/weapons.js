import * as utility from "./site.js";

const token = sessionStorage.getItem("token");

// setting initial page value
sessionStorage.setItem("currentPage", 1);

utility.showItemsOnLoadAsync("weapons");
utility.loadNextPageAsync("weapons");
utility.loadPreviousPageAsync("weapons");
utility.loadAllItemsAsync("weapons");
utility.getItemAsync("weapons");
utility.deleteItemAsync("weapons");
utility.hitCharacterAsync("weapons");

// POST request to create a weapon
document.querySelector("#create-btn").addEventListener("click", async e => {
    const weaponName = document.querySelector("#create-name").value;
    const weaponType = document.querySelector("#create-type").value;
    let weaponDamage = document.querySelector("#create-damage").value;

    if (weaponDamage == "") {
        weaponDamage = 30;
    }

    await fetch("../api/weapons", {
        method: "POST",
        headers: {
            "Accept": "application/json",
            "Content-Type": "application/json",
            "Authorization": `Bearer ${token}`
        },
        body: JSON.stringify({
            name: weaponName,
            type: weaponType,
            damage: weaponDamage
        })
    })
        .then(response => {
            if (response.ok) {
                return response.json();
            } else {
                throw new Error("You provided incorrect data");
            }
        })
        .then(data => utility.addItemToTableAsync(data["id"], weaponName, weaponType, weaponDamage, []))
        .catch(error => alert(error.message));
});

// PUT request to edit a weapon
document.querySelector("#edit-btn").addEventListener("click", async e => {
    const weaponId = document.getElementById("edit-id").value;
    const weaponTr = document.getElementsByClassName(`${weaponId}-tr`)[0];
    const newName = document.getElementById("edit-name").value;
    const newType = document.getElementById("edit-type").value;
    let newDamage = document.getElementById("edit-damage").value;

    if (newDamage == "") {
        newDamage = 30;
    }

    await fetch(`../api/weapons/${weaponId}`, {
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
                weaponTr.children[1].innerHTML = newName;
                weaponTr.children[2].innerHTML = newType;
                weaponTr.children[3].innerHTML = newDamage;
            } else {
                throw new Error("You provided incorrect data");
            }
        })
        .catch(error => alert(error.message));
});