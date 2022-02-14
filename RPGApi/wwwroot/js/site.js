const token = sessionStorage.getItem("token");
const currentPageElem = document.getElementById("curr-page");

sessionStorage.setItem("currentPage", 1);

// asynchronously gets the items on a specified page
export async function getItems(itemNames) {
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

    await fetch(`../api/${itemNames}/page/${currentPage}`, {
        method: "GET",
        headers: {
            "Authorization": `Bearer ${token}`
        }
    })
        .then(response => response.json())
        .then(data => {
            currentPageElem.value = sessionStorage.getItem("currentPage");
            displayItems(data.items);
        });
}

// adds an item to a table
export function addItemToTable(...itemProps) {
    const tbody = document.getElementsByTagName("tbody")[0];
    let newItemTr = tbody.insertRow();
    let td;

    newItemTr.classList.add(`${itemProps[0]}-tr`);

    for (let i = 0; i < itemProps.length; i++) {
        const itemProperty = itemProps[i];
        let hr;

        td = newItemTr.insertCell(i);

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

// prints all the items to a table
export function displayItems(items) {
    const tbody = document.getElementsByTagName("tbody")[0];
    tbody.innerHTML = "";

    items.forEach(i => addItemToTable(...Object.values(i)));
}

// updates an item value in a table
export function updateItemValue(trElem, ...itemProps) {
    for (let i = 1; i < itemProps.length + 1; i++) {
        trElem.children[i].innerHTML = itemProps[i - 1];
    }
}

// shows items when the page is loaded for the first time
export function showItemsOnLoad(itemNames) {
    window.addEventListener("load", async e => {
        const userRole = sessionStorage.getItem("userRole");
        const currentPage = sessionStorage.getItem("currentPage");

        document.querySelector("#log-out-btn").innerHTML = `Log Out (${sessionStorage.getItem("username")})`;
        document.getElementById("curr-page").value = currentPage;

        await fetch(`../api/${itemNames}/page/${currentPage}`, {
            method: "GET",
            headers: {
                "Authorization": `Bearer ${token}`
            }
        })
            .then(response => response.json())
            .then(data => {
                sessionStorage.setItem("pagesCount", data.pagesCount);
                sessionStorage.setItem("currentPage", data.currentPage);
                displayItems(data.items);
            });

        if (userRole == "0") {
            const manageDivs = document.querySelectorAll(".manage-div");

            for (let div of manageDivs) {
                div.classList.remove("d-none");
                div.classList.add("d-flex");
            }
        }

        if (currentPage < sessionStorage.getItem("pagesCount")) {
            document.querySelector("#next-btn").style.display = "inline";
        }
    });
}

// loads the items from the next page
export function loadNextPageOnClick(itemNames) {
    document.querySelector("#next-btn").addEventListener("click", async e => {
        const page = sessionStorage.getItem("currentPage");
        sessionStorage.setItem("currentPage", parseInt(page) + 1);

        await getItems(itemNames);
    });
}

// loads the items from the previous page
export function loadPreviousPageOnClick(itemNames) {
    document.querySelector("#prev-btn").addEventListener("click", async e => {
        const page = sessionStorage.getItem("currentPage");
        sessionStorage.setItem("currentPage", parseInt(page) - 1);

        await getItems(itemNames);
    });
}

// loads all the items on a page
export function loadAllItemsOnClick(itemNames) {
    document.querySelector("#all-items-btn").addEventListener("click", async e => {
        await getItems(itemNames);
        document.querySelector("#all-items-btn").style.display = "none";
        document.querySelector("#curr-page").style.display = "inline";
    });
}

// makes a get request
export function makeGetRequest(itemNames) {
    document.querySelector("#find-btn").addEventListener("click", async e => {
        await fetch(`../api/${itemNames}/${document.getElementById("find-id").value}`, {
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
                    throw new Error(`The ${itemNames.slice(0, itemNames.length - 1)}` + 
                        " with the provided id does not exist");
                }
            })
            .then(data => {
                displayItems([data]);

                document.querySelector("#all-items-btn").style.display = "inline";
                document.querySelector("#prev-btn").style.display = "none";
                document.querySelector("#curr-page").style.display = "none";
                document.querySelector("#next-btn").style.display = "none";
            })
            .catch(error => alert(error.message));
    });
}

// makes a DELETE request
export function makeDeleteRequest(itemNames) {
    document.querySelector("#del-btn").addEventListener("click", async e => {
        const itemId = document.querySelector("#del-id").value;

        await fetch(`../api/${itemNames}/${itemId}`, {
            method: "DELETE",
            headers: {
                "Accept": "application/json",
                "Content-Type": "application/json",
                "Authorization": `Bearer ${token}`
            }
        })
            .then(response => {
                if (response.ok) {
                    document.getElementsByClassName(`${itemId}-tr`)[0].remove();
                } else {
                    throw new Error("You provided incorrect id");
                }
            })
            .catch(error => alert(error.message));
    });
}