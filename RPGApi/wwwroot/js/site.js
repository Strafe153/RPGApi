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

// updates an item value in table
export function updateItemValue(trElem, ...itemProps) {
    for (let i = 1; i < itemProps.length + 1; i++) {
        trElem.children[i].innerHTML = itemProps[i - 1];
    }
}