function showMenu() {
    var x = document.getElementsByTagName("ul");
    for (let index = 0; index < x.length; index++) {
        const element = x[index];
        if (element.className === "dropdown") {
            element.className = "";
        } else {
            element.className = "dropdown";
        }
    }
}