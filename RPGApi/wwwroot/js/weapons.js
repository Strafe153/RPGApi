import * as utility from "./site.js";

const token = sessionStorage.getItem("token");

// setting initial page value
sessionStorage.setItem("currentPage", 1);

window.addEventListener("load", async e => {
    const userRole = sessionStorage.getItem("userRole");
    const currentPage = sessionStorage.getItem("currentPage");

    document.querySelector("#log-out-btn").innerHTML = `Log Out (${sessionStorage.getItem("username")})`;
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

// load weapons from the previous page
document.querySelector("#next-btn").addEventListener("click", async e => {
    const page = sessionStorage.getItem("currentPage");
    sessionStorage.setItem("currentPage", parseInt(page) + 1);

    await utility.getItems("weapons");
});

// load weapons from the next page
document.querySelector("#prev-btn").addEventListener("click", async e => {
    const page = sessionStorage.getItem("currentPage");
    sessionStorage.setItem("currentPage", parseInt(page) - 1);

    await utility.getItems("weapons");
});

// GET request to find a weapon
document.querySelector("#find-btn").addEventListener("click", async e => {
    await fetch(`../api/weapons/${document.getElementById("find-id").value}`, {
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
                throw new Error("The weapon with the provided id does not exist");
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
        .then(data => utility.addItemToTable(data["id"], weaponName, weaponType, weaponDamage, []))
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
    })
        .then(response => {
            if (response.ok) {
                document.getElementsByClassName(`${weaponId}-tr`)[0].remove();
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
    })
        .then(response => {
            if (!response.ok) {
                throw new Error("You provided incorrect data");
            }
        })
        .catch(error => alert(error.message));
});

document.querySelector("#all-items-btn").addEventListener("click", async e => {
    await utility.getItems("weapons");
    document.querySelector("#all-items-btn").style.display = "none";
    document.querySelector("#curr-page").style.display = "inline";
});