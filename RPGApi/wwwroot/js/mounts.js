import * as utility from "./site.js"

const token = sessionStorage.getItem("token");

// setting initial page value
sessionStorage.setItem("currentPage", 1);

utility.showItemsOnLoad("mounts");
utility.loadNextPageOnClick("mounts");
utility.loadPreviousPageOnClick("mounts");
utility.loadAllItemsOnClick("mounts");
utility.makeGetRequest("mounts");
utility.makeDeleteRequest("mounts");

// POST request to create a mount
document.querySelector("#create-btn").addEventListener("click", async e => {
    const mountName = document.querySelector("#create-name").value;
    const mountType = document.querySelector("#create-type").value;
    const mountSpeed = document.querySelector("#create-speed").value;

    await fetch("../api/mounts", {
        method: "POST",
        headers: {
            "Accept": "application/json",
            "Content-Type": "application/json",
            "Authorization": `Bearer ${token}`
        },
        body: JSON.stringify({
            name: mountName,
            type: mountType,
            speed: mountSpeed
        })
    })
        .then(response => {
            if (response.ok) {
                return response.json();
            } else {
                throw new Error("You provided incorrect data");
            }
        })
        .then(data => utility.addItemToTable(data["id"], mountName, mountType, mountSpeed, []))
        .catch(error => alert(error.message));
});

// PUT request to edit a mount
document.querySelector("#edit-btn").addEventListener("click", async e => {
    const mountId = document.getElementById("edit-id").value;
    const mountTr = document.getElementsByClassName(`${mountId}-tr`)[0];
    const newName = document.getElementById("edit-name").value;
    const newType = document.getElementById("edit-type").value;
    const newSpeed = document.getElementById("edit-speed").value;

    await fetch(`../api/mounts/${mountId}`, {
        method: "PUT",
        headers: {
            "Accept": "application/json",
            "Content-Type": "application/json",
            "Authorization": `Bearer ${token}`
        },
        body: JSON.stringify({
            name: newName,
            type: newType,
            speed: newSpeed
        })
    })
        .then(response => {
            if (response.ok) {
                mountTr.children[1].innerHTML = newName;
                mountTr.children[2].innerHTML = newType;
                mountTr.children[3].innerHTML = newSpeed;
            } else {
                throw new Error("You provided incorrect data");
            }
        })
        .catch(error => alert(error.message));
});