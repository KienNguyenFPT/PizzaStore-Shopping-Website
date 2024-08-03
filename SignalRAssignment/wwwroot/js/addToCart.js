/* 
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 * Click nbfs://nbhost/SystemFileSystem/Templates/JSP_Servlet/JavaScript.js to edit this template
 */


function addToCart(productId) {
    var dataToSend = {
        pID: productId
    };
    fetch('./AddToCartController', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(dataToSend)
    })
            .then(response => {
                if (!response.ok) {
                    throw new Error('Network response was not ok. Status: ' + response.status);
                }
                return response.text();
            })
            .then(data => {
                var statusDiv;
                var result = data.toString();
                if (result === "success") {
                    statusDiv = document.createElement("div");
                    statusDiv.textContent = "Add cart success!";
                    document.getElementById("addCartStatus").appendChild(statusDiv);
                    window.setTimeout(function () {
                        statusDiv.remove();
                    }, 2000);
                    return true;
                } else {
                    statusDiv = document.createElement("div");
                    statusDiv.textContent = "Add cart fail!";
                    document.getElementById("addCartStatus").appendChild(statusDiv);
                    window.setTimeout(function () {
                        statusDiv.remove();
                    }, 2000);
                    return false;
                }
            })
            .catch(error => {
                console.error('ERROR:', error);
                return false;
            });

}