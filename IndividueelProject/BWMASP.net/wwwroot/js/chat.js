"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

// Disable the send button until connection is established.
document.getElementById("sendButton").disabled = true;

connection.on("ReceiveMessage", function (user, message) {
    var li = document.createElement("li");
    var isCurrentUser = user === document.getElementById("Username").value;
    var containerClass = isCurrentUser ? "current" : "other";
    li.innerHTML = `
        <div class="message-container ${containerClass}">
            <div class="row">
                <div class="col-12">
                    <span class="username">${user}</span>
                </div>
                <div class="col">
                    <div class="message-text">${message}</div>
                </div>
            </div>
        </div>`;
    document.getElementById("messagesList").appendChild(li);
});


connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;

    // Join the group based on the club ID when connection is established
    var clubId = parseInt(document.getElementById("clubId").value);
    var userId = parseInt(document.getElementById("userInput").value);
    if (!isNaN(clubId) && !isNaN(userId)) {
        connection.invoke("JoinClubGroup", clubId, userId).catch(function (err) {
            return console.error(err);
        });
    } else {
        console.error("Invalid clubId: Club ID must be a number.");
    }
}).catch(function (err) {
    return console.error(err);
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    var userId = parseInt(document.getElementById("userInput").value);
    var message = document.getElementById("messageInput").value;
    var clubId = parseInt(document.getElementById("clubId").value);
    console.log("User ID: " + userId);
    console.log("Message: " + message);
    console.log("Club ID: " + clubId);

    if (!isNaN(userId) && !isNaN(clubId)) {
        connection.invoke("SendMessage", message, userId, clubId).catch(function (err) {
            return console.error(err.toString());
        });
    } else {
        console.error("Invalid input: userId and clubId must be numbers.");
    }

    event.preventDefault();
});
