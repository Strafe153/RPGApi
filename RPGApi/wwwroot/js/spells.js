﻿const token = sessionStorage.getItem("token");
const currentPageElem = document.getElementById("curr-page");

// setting initial page value
sessionStorage.setItem("currentPage", 1);

async function getSpells() {
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

    await fetch(`../api/spells/page/${currentPage}`, {
        method: "GET",
        headers: {
            "Authorization": `Bearer ${token}`
        }
    })
        .then(response => response.json())
        .then(data => {
            currentPageElem.value = sessionStorage.getItem("currentPage");
            displaySpells(data.items, "spells-tbody");
        });
}

function displaySpells(spells, tbodyId) {
    const tbody = document.querySelector(`#${tbodyId}`);
    tbody.innerHTML = "";

    spells.forEach(s => addSpellToTable(tbodyId, s.id,
        s.name, s.type, s.damage, s.characters));
}

function addSpellToTable(tbodyId, ...spellProps) {
    const tbody = document.querySelector(`#${tbodyId}`);
    let newSpellTr = tbody.insertRow();
    let td;

    newSpellTr.classList.add(`${spellProps[0]}-tr`);

    for (let i = 0; i < spellProps.length; i++) {
        const itemProperty = spellProps[i];
        let hr;

        td = newSpellTr.insertCell(i);

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

    await fetch(`../api/spells/page/${currentPage}`, {
        method: "GET",
        headers: {
            "Authorization": `Bearer ${token}`
        }
    })
        .then(response => response.json())
        .then(data => {
            sessionStorage.setItem("pagesCount", data.pagesCount);
            sessionStorage.setItem("currentPage", data.currentPage);
            displaySpells(data.items, "spells-tbody");
        });

    if (currentPage < sessionStorage.getItem("pagesCount")) {
        document.querySelector("#next-btn").style.display = "inline";
    }
});

// load spells from the previous page
document.querySelector("#next-btn").addEventListener("click", async e => {
    const page = sessionStorage.getItem("currentPage");
    sessionStorage.setItem("currentPage", parseInt(page) + 1);

    await getSpells();
});

// load spells from the next page
document.querySelector("#prev-btn").addEventListener("click", async e => {
    const page = sessionStorage.getItem("currentPage");
    sessionStorage.setItem("currentPage", parseInt(page) - 1);

    await getSpells();
});

// GET request to find a spell
document.querySelector("#find-spell-btn").addEventListener("click", async e => {
    await fetch(`../api/spells/${document.getElementById("find-spell-id").value}`, {
        method: "GET",
        headers: {
            "Authorization": `Bearer ${token}`
        }
    })
        .then(response => response.json())
        .then(data => {
            displaySpells([data], "spells-tbody");
            document.querySelector("#all-spells-btn").style.display = "inline";
            document.querySelector("#prev-btn").style.display = "none";
            document.querySelector("#curr-page").style.display = "none";
            document.querySelector("#next-btn").style.display = "none";
        });
});

// POST request to create a spell
document.querySelector("#create-btn").addEventListener("click", async e => {
    const spellName = document.querySelector("#create-name").value;
    const spellType = document.querySelector("#create-type").value;
    const spellDamage = document.querySelector("#create-damage").value;
    let spellId;

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
        .then(response => response.json())
        .then(data => spellId = data["id"]);

    addSpellToTable("spells-tbody", spellId, spellName, spellType, spellDamage, []);
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
    });

    spellTr.children[1].innerHTML = newName;
    spellTr.children[2].innerHTML = newType;
    spellTr.children[3].innerHTML = newDamage;
});

// DELETE request to delete a spell
document.querySelector("#del-btn").addEventListener("click", async e => {
    const spellId = document.querySelector("#del-id").value;

    await fetch(`../api/spells/${spellId}`, {
        method: "DELETE",
        headers: {
            "Accept": "application/json",
            "Content-Type": "application/json",
            "Authorization": `Bearer ${token}`
        }
    });

    document.getElementsByClassName(`${spellId}-tr`)[0].remove();
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
    });
});

document.querySelector("#all-spells-btn").addEventListener("click", async e => {
    await getSpells();
    document.querySelector("#all-spells-btn").style.display = "none";
    document.querySelector("#curr-page").style.display = "inline";
});