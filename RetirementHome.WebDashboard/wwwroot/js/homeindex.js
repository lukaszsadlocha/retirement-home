const initializeSingalRConnection = () => {
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/residentHub")
        .build();

    connection.on("recievedNewResident", (newResident) => {
        console.log(newResident);

        var residentsDiv = document.getElementById("residents");
        var newResidentElem = document.createElement('p');
        newResidentElem.innerHTML = newResident.firstName + " " + newResident.lastName
        residentsDiv.appendChild(newResidentElem);
    });

    connection.start().catch(err => console.error(err.toString()));
    return connection;
}

const connection = initializeSingalRConnection();

const addResident = () => {
    const fn = document.getElementById("firstNameInput").value;
    const ln = document.getElementById("lastNameInput").value;

    fetch("/resident/new", {

        method: "POST",
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ firstName: fn, lastName: ln })
    }).then(jsonData => jsonData.json())
        .then(data => {
            connection.invoke("NotifyNewResident", JSON.stringify(data))
        });
}
