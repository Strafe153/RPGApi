﻿const token = sessionStorage.getItem("token");
const currentPageElem = document.getElementById("curr-page");

// setting initial page value
sessionStorage.setItem("currentPage", 1);

async function getWeapons() {
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

    await fetch(`../api/weapons/page/${currentPage}`, {
        method: "GET",
        headers: {
            "Authorization": `Bearer ${token}`
        }
    })
        .then(response => response.json())
        .then(data => {
            currentPageElem.value = sessionStorage.getItem("currentPage");
            displayWeapons(data.items, "weapons-tbody");
        });
}

function displayWeapons(weapons, tbodyId) {
    const tbody = document.querySelector(`#${tbodyId}`);
    tbody.innerHTML = "";

    weapons.forEach(w => addCharacterToTable(tbodyId, w.id,
        w.name, w.type, w.damage, w.characters));
}

function addCharacterToTable(tbodyId, ...weaponProps) {
    const tbody = document.querySelector(`#${tbodyId}`);
    let newWeaponTr = tbody.insertRow();
    let td;

    newWeaponTr.classList.add(`${weaponProps[0]}-tr`);

    for (let i = 0; i < weaponProps.length; i++) {
        const itemProperty = weaponProps[i];
        let hr;

        td = newWeaponTr.insertCell(i);

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

    await fetch(`../api/weapons/page/${currentPage}`, {
        method: "GET",
        headers: {
            "Authorization": `Bearer ${token}`
        }
    })
        .then(response => response.json())
        .then(data => {
            sessionStorage.setItem("pagesCount", data.pagesCount);
            sessionStorage.setItem("currentPage", data.currentPage);
            displayWeapons(data.items, "weapons-tbody");
        });

    if (currentPage < sessionStorage.getItem("pagesCount")) {
        document.querySelector("#next-btn").style.display = "inline";
    }
});

// load weapons from the previous page
document.querySelector("#next-btn").addEventListener("click", async e => {
    const page = sessionStorage.getItem("currentPage");
    sessionStorage.setItem("currentPage", parseInt(page) + 1);

    await getWeapons();
});

// load weapons from the next page
document.querySelector("#prev-btn").addEventListener("click", async e => {
    const page = sessionStorage.getItem("currentPage");
    sessionStorage.setItem("currentPage", parseInt(page) - 1);

    await getWeapons();
});

// GET request to find a weapon
document.querySelector("#find-weapon-btn").addEventListener("click", async e => {
    await fetch(`../api/weapons/${document.getElementById("find-weapon-id").value}`, {
        method: "GET",
        headers: {
            "Authorization": `Bearer ${token}`
        }
    })
        .then(response => response.json())
        .then(data => {
            displayWeapons([data], "weapons-tbody");
            document.querySelector("#all-weapons-btn").style.display = "inline";
            document.querySelector("#prev-btn").style.display = "none";
            document.querySelector("#curr-page").style.display = "none";
            document.querySelector("#next-btn").style.display = "none";
        });
});

// POST request to create a weapon
document.querySelector("#create-btn").addEventListener("click", async e => {
    const weaponName = document.querySelector("#create-name").value;
    const weaponType = document.querySelector("#create-type").value;
    const weaponDamage = document.querySelector("#create-damage").value;
    let weaponId;

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
        .then(response => response.json())
        .then(data => weaponId = data["id"]);

    addCharacterToTable("weapons-tbody", weaponId, weaponName, weaponType, weaponDamage, []);
});

// PUT request to edit a weapon
document.querySelector("#edit-btn").addEventListener("click", async e => {
    const weaponId = document.getElementById("edit-id").value;
    const weaponTr = document.getElementsByClassName(`${weaponId}-tr`)[0];
    const newName = document.getElementById("edit-name").value;
    const newType = document.getElementById("edit-type").value;
    const newDamage = document.getElementById("edit-damage").value;

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
    });

    weaponTr.children[1].innerHTML = newName;
    weaponTr.children[2].innerHTML = newType;
    weaponTr.children[3].innerHTML = newDamage;
});

// DELETE request to delete a weapon
document.querySelector("#del-btn").addEventListener("click", async e => {
    const weaponId = document.querySelector("#del-id").value;

    await fetch(`../api/weapons/${weaponId}`, {
        method: "DELETE",
        headers: {
            "Accept": "application/json",
            "Content-Type": "application/json",
            "Authorization": `Bearer ${token}`
        }
    });

    document.getElementsByClassName(`${weaponId}-tr`)[0].remove();
});

// PUT request to hit a character
document.querySelector("#hit-btn").addEventListener("click", async e => {
    const dealer = document.querySelector("#hit-dealer-id").value;
    const receiver = document.querySelector("#hit-receiver-id").value;
    const item = document.querySelector("#hit-item-id").value;

    await fetch("../api/weapons/hit", {
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
    });
});

document.querySelector("#all-weapons-btn").addEventListener("click", async e => {
    await getWeapons();
    document.querySelector("#all-weapons-btn").style.display = "none";
    document.querySelector("#curr-page").style.display = "inline";
});