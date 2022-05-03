class Purchase extends Page {
  constructor() {
    super("purchase", "Purchase");
  }

  setElements() {
    super.setElements();
  }
  
  setListing(currentListing) {
    var listingInfo = getFormattedListingInfo(currentListing);
    
    getElement("seed-storage").innerText = currentListing.seed;
    getElement("book-title2").innerText = currentListing.title + " (" + currentListing.isbn + ")";
    getElement("author-names2").innerText = currentListing.authors;
    getElement("seller-name2").innerText = currentListing.seller;
    getElement("condition2").style.color = CONDITION_COLOR[listingInfo.condition];
    getElement("condition2").innerText = listingInfo.condition;
    getElement("book-thumbnail2").src = currentListing.image;  
    getElement("price2").innerText = "$" + listingInfo.price;
  }

  show() {
    super.show();

    slide(this.name, 1);
  }
}