import * as utility from "./site.js"

const token = sessionStorage.getItem("token");

// setting initial page value
sessionStorage.setItem("currentPage", 1);

window.addEventListener("load", async e => {
    const userRole = sessionStorage.getItem("userRole");
    const currentPage = sessionStorage.getItem("currentPage");

    document.querySelector("#log-out-btn").innerHTML = `Log Out (${sessionStorage.getItem("username")})`;
    document.getElementById("curr-page").value = currentPage;

    await fetch(`../api/mounts/page/${currentPage}`, {
        method: "GET",
        headers: {
            "Authorization": `Bearer ${token}`
        }
    })
        .then(response => response.json())
        .then(data => {
            sessionStorage.setItem("pagesCount", data.pagesCount);
            sessionStorage.setItem("currentPage", data.currentPage);
            utility.displayItems(data.items, "mounts-tbody");
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

// load mounts from the previous page
document.querySelector("#next-btn").addEventListener("click", async e => {
    const page = sessionStorage.getItem("currentPage");
    sessionStorage.setItem("currentPage", parseInt(page) + 1);

    await utility.getItems("mounts");
});

// load mounts from the next page
document.querySelector("#prev-btn").addEventListener("click", async e => {
    const page = sessionStorage.getItem("currentPage");
    sessionStorage.setItem("currentPage", parseInt(page) - 1);

    await utility.getItems("mounts");
});

// GET request to find a mount
document.querySelector("#find-mount-btn").addEventListener("click", async e => {
    await fetch(`../api/mounts/${document.getElementById("find-mount-id").value}`, {
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
                throw new Error("The mount with the provided id does not exist");
            }
        })
        .then(data => {
            utility.displayItems([data], "mounts-tbody");

            document.querySelector("#all-items-btn").style.display = "inline";
            document.querySelector("#prev-btn").style.display = "none";
            document.querySelector("#curr-page").style.display = "none";
            document.querySelector("#next-btn").style.display = "none";
        })
        .catch(error => alert(error.message));
});

// POST request to create a mount
document.querySelector("#create-btn").addEventListener("click", async e => {
    const mountName = document.querySelector("#create-name").value;
    const mountType = document.querySelector("#create-type").value;
    let mountSpeed = document.querySelector("#create-speed").value;

    if (mountSpeed == "") {
        mountSpeed = 8;
    }

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
        .then(data => utility.addItemToTable("mounts-tbody", data["id"], mountName, mountType, mountSpeed, []))
        .catch(error => alert(error.message));
});

// PUT request to edit a mount
document.querySelector("#edit-btn").addEventListener("click", async e => {
    const mountId = document.getElementById("edit-id").value;
    const mountTr = document.getElementsByClassName(`${mountId}-tr`)[0];
    const newName = document.getElementById("edit-name").value;
    const newType = document.getElementById("edit-type").value;
    let newSpeed = document.getElementById("edit-speed").value;

    if (newSpeed == "") {
        newSpeed = 8;
    }

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

// DELETE request to delete a spell
document.querySelector("#del-btn").addEventListener("click", async e => {
    const mountId = document.querySelector("#del-id").value;

    await fetch(`../api/mounts/${mountId}`, {
        method: "DELETE",
        headers: {
            "Accept": "application/json",
            "Content-Type": "application/json",
            "Authorization": `Bearer ${token}`
        }
    })
        .then(response => {
            if (response.ok) {
                document.getElementsByClassName(`${mountId}-tr`)[0].remove();
            } else {
                throw new Error("You provided incorrect id");
            }
        })
        .catch(error => alert(error.message));
});

document.querySelector("#all-items-btn").addEventListener("click", async e => {
    await utility.getItems("mounts");
    document.querySelector("#all-items-btn").style.display = "none";
    document.querySelector("#curr-page").style.display = "inline";
});