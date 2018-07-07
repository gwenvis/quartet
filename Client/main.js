let textbox = document.getElementById("name");
let button = document.getElementById("button");
let websocket;

let myId = "", myTurn = false, selectedID, latestCardID = 0;

let screens = {
    connection : document.querySelector(".connection_screen"),
    game : document.querySelector(".game_screen")
}

let gameScreens = {
    startGameButton : document.querySelector("#startgamebutton"),
    questionform : document.querySelector("#questionform"),
    waitingturn : document.querySelector("#waitingturn"),
    cardholder : document.querySelector("#cardholder")
}

let cardsInGame = [];

function join()
{
    let ip = document.getElementById("IP").value;
    console.log(ip);
    websocket = new WebSocket("ws://" + ip + ":42069");

    websocket.onopen = function(ev)
    {
        console.log("Opened connection");

        websocket.send(
            JSON.stringify(
                {
                    "status":"join",
                    "data":{
                        "name":textbox.value
                    }
                }
            )
        );
    };

    websocket.onmessage = function(ev)
    {
        console.log(ev.data);
        receiveData(JSON.parse(ev.data));
    };

    websocket.onerror = function(ev)
    {
        console.log(ev);
    };
}

function startGame()
{
    websocket.send(
        JSON.stringify(
            {
                "status":"startgame",
                "data":{}
            }
        )
    )
}

function receiveData(jsondata)
{

    switch(jsondata.status.toLowerCase())
    {
        case "joininfo":

            if(jsondata.data.joinorder === 1)
            {
                document.querySelector("#startgamebutton").hidden = false;
            }
            
            myId = jsondata.data.id;

            screens.connection.hidden = true;
            screens.game.hidden = false;

            break;

        case "turnstarted":

            gameScreens.questionform.hidden = false;
            gameScreens.questionform.innerHTML = "";
            gameScreens.waitingturn.hidden = true;

            console.log(jsondata);

            for(let i = 0; i < jsondata.data.players.length; i++) {

                let player = jsondata.data.players[i];

                console.log("My id: " + myId.toString() + " Player id: " + player.id.toString());
                if(player.id === myId) continue;

                let button = document.createElement("button");
                button.innerHTML = player.playername;
                button.setAttribute("playerId", player.id);

                button.onclick = function() {
                    gameScreens.questionform.hidden = true;
                    gameScreens.cardholder.style.bottom = "0";
                    selectedID = button.getAttribute("playerId");
                }

                gameScreens.questionform.appendChild(button);

                myTurn = true;
            }

            break;

        case "turnended":

            gameScreens.questionform.hidden = false;
            gameScreens.waitingturn.hidden = true;
            gameScreens.cardholder.style.bottom = "-100px";
            document.querySelector("#startgamebutton").hidden = true;

            break;

        case "gamestarted":
            document.querySelector("#startgamebutton").style.display= "none";
            document.body.removeChild(document.querySelector("#startgamebutton"));

            gameScreens.waitingturn.hidden = false;
            gameScreens.cardholder.style.bottom = "-100px";
            break;

        case "getcard":
            let cards = jsondata.data["cards"];
            console.log(cards);
            for(let i = 0; i < cards.length; i++) {
                let c = cards[i];
                let card = new Card(c.category, c.cardname, c.cardsinsamecategory);
                addCard(card);
            }

            console.log(cardsInGame);

            break;

        case "givecard":

            let removedCards = jsondata.data["cards"];
            console.log(removedCards);

            for(let i = 0; i < removedCards.length; i++)
            {
                let ca = removedCards[i];
                let cardtoremove = findCard(ca.cardname, ca.category);
                removeCard(cardtoremove);
            }

            break;
    }
}

function findCard(name, category)
{

    for(let i = 0; i < cardsInGame.length; i++)
    {
        let cardInGame = cardsInGame[i];

        if(cardInGame.cardObject.name.toLowerCase() === name.toLowerCase() &&
            cardInGame.cardObject.category.toLowerCase() === category.toLowerCase())
            return cardInGame;
    }

    return null;
}

let get = function(x) {
    return document.querySelector(x);
}

function addCard(card) {
    let copy = document.getElementById("cardtemplate").cloneNode(true);
    let cardId = "card" + (++latestCardID);
    copy.id = cardId;
    copy.setAttribute("id", "card")
    copy.setAttribute("class", cardId);

    copy.style.display = "inline-block";
    let image = copy.querySelector("#card_image");
    image.setAttribute("href", "img/" + card.filename);

    copy.querySelector("#card_category").innerHTML = card.category;
    let cardnames = copy.querySelector("#card_names");
    let tspantemplate = cardnames.querySelector("tspan");

    let d = screens.game.querySelector("#cardholder").appendChild(copy);

    let startingY = 240;
    let increment = 35;

    for(let i = 0; i < card.othersInCategory.length; i++) {
        let item = card.othersInCategory[i];
        let index = i;

        let cardName = tspantemplate.cloneNode();
        let text = d.querySelector("#card_names").appendChild(cardName);

        // if clicked you request this card
        text.onclick = () =>
        {
            console.log("Clicked on: " + item);

            if(!myTurn) return;

            websocket.send(JSON.stringify({
                    "status": "question",
                    "data" : {
                        "to" : selectedID,
                        "cardname" : item.toLowerCase(),
                        "cardcategory" : card.category.toLowerCase()
                    }
                })
            )

            myTurn = false;
        };

        text.setAttribute("y", startingY + index * increment);
        text.setAttribute("visibility", "normal");
        text.setAttribute("ass", "nice");
        text.innerHTML = item;


        let t;
        if(card.name.toLowerCase() === item.toLowerCase())
            t = "bold";
        else
            t = "normal";

        text.setAttribute("font-weight", t);
    }

    let cardGameObject = new CardGameObject(card, cardId);
    cardsInGame.push(cardGameObject);
    return cardGameObject;
}

function removeCard(cardGameObject) {
    // document.body.removeChild(cardGameObject.domElement);
    let c = get("." + cardGameObject.cardID);
    c.parentNode.removeChild(c);
    cardsInGame = cardsInGame.filter(function(el) {
        let c = cardGameObject.cardObject;
        let cc = el.cardObject;
        return !(c.name === cc.name && c.category === cc.category);
    });
}

function leave()
{
    websocket.send(
        JSON.stringify(
            {
                "status":"leave",
                "data":{}
            }
        )
    );
    websocket.close();
    // button.onclick = join;
    // button.value = "Join";
}




