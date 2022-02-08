// test with players, characters, etc...
function addItemToTable(tbodyId, ...itemProps) {
    const tbody = document.querySelector(`#${tbodyId}`);
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