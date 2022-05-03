class Listings extends Page {
  constructor() {
    super("listings", "Listings");

    sendRequest(Requests.Search);
  }

  setElements() {
    super.setElements();

    var searchBar = getElement("search-bar");

    getElement("button-search").style.display = "block";

    // This will allow the user to scroll on the listings, as well as the description of each listing
    document.querySelector("body").style.overflowY = "scroll";

    if (!searchBar.classList.contains("slidedown")) {
      searchBar.classList.add("slidedown");

      getElement("listings").style.marginTop = "70px";
      
      getElement("header").style.boxShadow = "0 0 0 0";

      getElement("button-settings").style.display = "none";
      getElement("button-back").style.display = "block";
      
      getElement("footer").classList.remove("slideup");
    } else {
      searchBar.classList.remove("slidedown");

      getElement("header").style.boxShadow = "0 0 10px 1px rgb(0 0 0 / 50%)";

      getElement("listings").style.marginTop = "0";
    }
    
    $("#input-search").on("change paste", function() {
      sendRequest(Requests.Search);
    });
  }

  show(shouldSlide = true) {
    super.show();
    
    if (shouldSlide) {
      slide(this.name, 1);
    }
  }
}
