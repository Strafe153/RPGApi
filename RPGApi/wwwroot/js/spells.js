import * as utility from "./site.js";

const token = sessionStorage.getItem("token");

// setting initial page value
sessionStorage.setItem("currentPage", 1);

window.addEventListener("load", async e => {
    const userRole = sessionStorage.getItem("userRole");
    const currentPage = sessionStorage.getItem("currentPage");

    document.querySelector("#log-out-btn").innerHTML = `Log Out (${sessionStorage.getItem("username")})`;
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
            utility.displayItems(data.items);
        });

    if (userRole == "0") {
        const manageDiv = document.querySelector("#manage-div");
        manageDiv.classList.remove("d-none");
        manageDiv.classList.add("d-flex");
    }

    if (currentPage < sessionStorage.getItem("pagesCount")) {
        document.querySelector("#next-btn").style.display = "inline";
    }
});

// load spells from the previous page
document.querySelector("#next-btn").addEventListener("click", async e => {
    const page = sessionStorage.getItem("currentPage");
    sessionStorage.setItem("currentPage", parseInt(page) + 1);

    await utility.getItems("spells");
});

// load spells from the next page
document.querySelector("#prev-btn").addEventListener("click", async e => {
    const page = sessionStorage.getItem("currentPage");
    sessionStorage.setItem("currentPage", parseInt(page) - 1);

    await utility.getItems("spells");
});

// GET request to find a spell
document.querySelector("#find-btn").addEventListener("click", async e => {
    await fetch(`../api/spells/${document.getElementById("find-id").value}`, {
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
                throw new Error("The spell with the provided id does not exist");
            }
        })
        .then(data => {
            utility.displayItems([data]);

            document.querySelector("#all-items-btn").style.display = "inline";
            document.querySelector("#prev-btn").style.display = "none";
            document.querySelector("#curr-page").style.display = "none";
            document.querySelector("#next-btn").style.display = "none";
        })
        .catch(error => alert(error.message));
});

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
    })
        .then(response => {
            if (response.ok) {
                document.getElementsByClassName(`${spellId}-tr`)[0].remove();
            } else {
                throw new Error("You provided incorrect id");
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

document.querySelector("#all-items-btn").addEventListener("click", async e => {
    await utility.getItems("spells");
    document.querySelector("#all-items-btn").style.display = "none";
    document.querySelector("#curr-page").style.display = "inline";
});