class Listing extends Page {
  constructor() {
    super("listing", "Listing");
  }

  setElements() {
    super.setElements();
  }

  setListing(currentListing) {
    document.querySelector("body").style.overflowY = "scroll";
    
    var listingInfo = getFormattedListingInfo(currentListing);
  
    getElement("book-title").innerText = currentListing.title + " (" + currentListing.isbn + ")";
    getElement("author-names").innerText = currentListing.authors;
    getElement("seller-name").innerText = currentListing.seller;
    getElement("condition").style.color = CONDITION_COLOR[listingInfo.condition];
    getElement("condition").innerText = listingInfo.condition;

    if (currentListing.sellersNotes == null || currentListing.sellersNotes === "") {
      getElement("sellers-notes").innerText = "None";
    } else {
      getElement("sellers-notes").innerText = currentListing.sellersNotes;
    }

    getElement("description").innerHTML = currentListing.description;

    getElement("book-thumbnail").src = currentListing.image;

    listing.getElementsByClassName("buy-button")[0].id = currentListing.isbn;

    // First condition checks whether we clicked it from the listing and not the orders page
    // The second condition prevent people from buying their own listing by hiding the buy button.
    if (listings[currentListing.seed] != null && currentListing.sellerId !== userData.Identifier) {
      getElement("price").innerText = "$" + listingInfo.price;
    } else {
      getElement(currentListing.isbn).style.display = "none";
    }
  }

  show() {
    super.show();

    slide(this.name, 1);
  }
}
