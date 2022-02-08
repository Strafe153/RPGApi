const token = sessionStorage.getItem("token");
const currentPageElem = document.getElementById("curr-page");

// setting initial page value
sessionStorage.setItem("currentPage", 1);

async function getMounts() {
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

    await fetch(`../api/mounts/page/${currentPage}`, {
        method: "GET",
        headers: {
            "Authorization": `Bearer ${token}`
        }
    })
        .then(response => response.json())
        .then(data => {
            currentPageElem.value = sessionStorage.getItem("currentPage");
            displayMounts(data.items, "mounts-tbody");
        });
}

function displayMounts(mounts, tbodyId) {
    const tbody = document.querySelector(`#${tbodyId}`);
    tbody.innerHTML = "";

    mounts.forEach(m => addMountToTable(tbodyId, m.id,
        m.name, m.type, m.speed, m.characters));
}

function addMountToTable(tbodyId, ...mountProps) {
    const tbody = document.querySelector(`#${tbodyId}`);
    let newMountTr = tbody.insertRow();
    let td;

    newMountTr.classList.add(`${mountProps[0]}-tr`);

    for (let i = 0; i < mountProps.length; i++) {
        const itemProperty = mountProps[i];
        let hr;

        td = newMountTr.insertCell(i);

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
            displayMounts(data.items, "mounts-tbody");
        });

    if (currentPage < sessionStorage.getItem("pagesCount")) {
        document.querySelector("#next-btn").style.display = "inline";
    }
});

// load mounts from the previous page
document.querySelector("#next-btn").addEventListener("click", async e => {
    const page = sessionStorage.getItem("currentPage");
    sessionStorage.setItem("currentPage", parseInt(page) + 1);

    await getMounts();
});

// load mounts from the next page
document.querySelector("#prev-btn").addEventListener("click", async e => {
    const page = sessionStorage.getItem("currentPage");
    sessionStorage.setItem("currentPage", parseInt(page) - 1);

    await getMounts();
});

// GET request to find a mount
document.querySelector("#find-mount-btn").addEventListener("click", async e => {
    await fetch(`../api/mounts/${document.getElementById("find-mount-id").value}`, {
        method: "GET",
        headers: {
            "Authorization": `Bearer ${token}`
        }
    })
        .then(response => response.json())
        .then(data => {
            displayMounts([data], "mounts-tbody");
            document.querySelector("#all-mounts-btn").style.display = "inline";
            document.querySelector("#prev-btn").style.display = "none";
            document.querySelector("#curr-page").style.display = "none";
            document.querySelector("#next-btn").style.display = "none";
        });
});

// POST request to create a mount
document.querySelector("#create-btn").addEventListener("click", async e => {
    const mountName = document.querySelector("#create-name").value;
    const mountType = document.querySelector("#create-type").value;
    const mountSpeed = document.querySelector("#create-speed").value;
    let mountId;

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
        .then(response => response.json())
        .then(data => mountId = data["id"]);

    addMountToTable("mounts-tbody", mountId, mountName, mountType, mountSpeed, []);
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
    });

    mountTr.children[1].innerHTML = newName;
    mountTr.children[2].innerHTML = newType;
    mountTr.children[3].innerHTML = newSpeed;
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
    });

    document.getElementsByClassName(`${mountId}-tr`)[0].remove();
});

document.querySelector("#all-mounts-btn").addEventListener("click", async e => {
    await getMounts();
    document.querySelector("#all-mounts-btn").style.display = "none";
    document.querySelector("#curr-page").style.display = "inline";
});