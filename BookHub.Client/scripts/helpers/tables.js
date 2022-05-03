var listings = {};

var storedScrollY = 0;

/** Integer to condition map. */
const CONDITION = {
  0: "Poor",
  1: "Fair",
  2: "Good",
  3: "Excellent"
}

/** Condition string to a CSS color for the text */
const CONDITION_COLOR = {
  "Poor" : "red",
  "Fair" : "yellow",
  "Good" : "lawngreen",
  "Excellent" : "green"
}

/**
 * Adds a listing to the search results.
 * @param {*} book 
 */
function addListing(book) {
  var listing = makeListing(book);
  listings[listing.seed] = listing;

  var listingInfo = getFormattedListingInfo(listing);

  var tableItem = "<tr id='" + listing.seed + "' onclick='onListingClick(" + listing.seed + ");'>" + 
                  "<td onclick='tableItemClicked(this)' ontouchstart=\"this.className='touched';\" ontouchend=\"this.className='';\" class=''>" +
                  "<div style='float:left;display:block;width:60px;'><img src='" + listing.image + "' width='50' height='85' style='border-radius: 5px; transform: translate3d(0,0,0);'></div>" +
                  "<div class='text'>" + listingInfo.title + "</div><br /><div class='small-text'><i>authors: </i><font color='black'>" + listingInfo.authors + "</font></div><br />" +
                  "<div class='small-text'><i>seller: </i><font color='black'>" + listing.seller + "</font></div><br />" +
                  "<div class='small-text'><i>condition: </i><span style='color: " + CONDITION_COLOR[listingInfo.condition] + ";'>" + listingInfo.condition + "</span></div>" +
                  "<div class='price'>$" + listingInfo.price + "</div></td></tr>";

  add("books-list", listing.seed, tableItem);
}

/**
 * Returns a listing object created from the specified listing data.
 * @param {*} listing The listing JSON data received from the server
 */
function makeListing(listing) {
  return {
    seed: listing.Seed,
    isbn: listing.ISBN,
    title: listing.Title,
    authors: listing.Authors,
    sellerId: listing.Seller.Identifier,
    seller: listing.Seller.Name,
    sellersNotes: listing.SellersNotes,
    condition: listing.Condition,
    price: listing.Price,
    image: listing.SmallThumbnail,
    description: listing.Description
  };
}

/**
 * Returns formatted versions of certain fields (truncated title, multiple authors, etc.) from the specified listing.
 * @param {*} listing 
 * @returns 
 */
function getFormattedListingInfo(listing) {
  return {
    title: listing.title.length > 30 ? listing.title.substring(0, 27) + "..." : listing.title,
    authors: listing.authors.length > 0 ? listing.authors[0] + ", et al." : listing.authors[0],
    condition: CONDITION[listing.condition],
    price: listing.price.toString().indexOf(".") < 0 ? listing.price + ".00" : listing.price.toFixed(2)
  };
}

/**
 * Appends a table item to the specified table if it has not already been added.
 * @param {*} table The table to append the item to.
 * @param {*} seed The seed of the listing.
 * @param {*} tableItem The table item to append to the table.
 */
function add(table, seed, tableItem) {
  if (getElement(seed) == null) {
    getElement(table).innerHTML += tableItem;
  }
}

/**
 * Defines the actions that occur when a listing is clicked.
 * @param {*} seed The seed of the listing that was clicked
 */
function onListingClick(seed) {
  storedScrollY = window.scrollY; // Store the location of the vertical scroll bar on the listings page so we can go back to it

  getElement("search-bar").classList.remove("slidedown");
  getElement("footer").classList.remove("slideup");
  
  getElement("listings").style.marginTop = "0";

  getElement("button-back").style.display = "block";
  getElement("button-settings").style.display = "none";

  getElement("loader").style.display = "block";

  setTimeout(function () {
    getElement("loader").style.display = "none";
    getElement("button-search").style.display = "none";

    window.scrollTo(0, 0); // Scroll to top

    var listing = new Listing();

    listing.setElements();
    listing.show();

    var currentListing = listings[seed];

    // This null check is for when you click a listing from the Orders page so that we can reuse the page that shows when a regular listing is clicked.
    if (currentListing == null) {
      var order = userData.Orders.find((obj) => obj.Listing.Seed === seed);
      currentListing = makeListing(order.Listing);
    }

    listing.setListing(currentListing);

    $(".buy-button").on("click", function() {
      // Stops scrolling functionality from the listings and listing pages while on the buy page
      document.querySelector("body").style.overflowY = "hidden";

      var purchase = new Purchase();

      purchase.setElements();
      purchase.show();

      purchase.setListing(currentListing);
    });
  }, 400);
}
