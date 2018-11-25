class Card {

    constructor(category, name, othersInCategory) {
        this.category = category
        this.name = name
        let filename = name.toLowerCase();
        let split = filename.split(" ");
        filename = split.join("-");

        this.filename = filename + ".png"
        this.othersInCategory = othersInCategory
    }


}
class CardGameObject {

    constructor(card, cardID) {
        this.cardObject = card;
        this.cardID = cardID
    }
}