class Orders extends Page {
  constructor() {
    super("orders", "Orders");
  }

  setElements() {
    super.setElements();

    document.querySelector("body").style.overflowY = "scroll";

    getElement("button-search").style.display = "none";
    getElement("button-settings").style.display = "none";
    
    getElement("button-back").style.display = "block";
      
    getElement("footer").classList.remove("slideup");

    if (userData.Orders.length > 0) {
      setHTML("orders-list", "");

      for (var i = 0; i < userData.Orders.length; i++) {
        var order = userData.Orders[i];

        var listing = makeListing(order.Listing);
        var listingInfo = getFormattedListingInfo(listing);
  
        var tableItem = "<tr id='" + i + "' onclick='onListingClick(" + listing.seed + ");'>" + 
                        "<td onclick='tableItemClicked(this)' ontouchstart=\"this.className='touched';\" ontouchend=\"this.className='';\" class=''>" +
                        "<div style='float:left;display:block;width:60px;'><img src='" + listing.image + "' width='50' height='85' style='border-radius: 5px; transform: translate3d(0,0,0);'></div>" +
                        "<div class='text'>" + listingInfo.title + "</div><br /><div class='small-text'><i>order #: </i><font color='black'>" + order.OrderNumber + "</font></div><br />" +
                        "<div class='small-text'><i>seller: </i><font color='black'>" + listing.seller + "</font></div><br />" +
                        "<div class='small-text'><i>condition: </i><span style='color: " + CONDITION_COLOR[listingInfo.condition] + ";'>" + listingInfo.condition + "</span></div>" +
                        "<div class='price'>$" + listingInfo.price + "</div></td></tr>";

        add("orders-list", i, tableItem);
      }
    } else {
      var noResults = "<div id='error' style='margin: 250px 50px; color: grey; text-align: center;'>You have not purchased any books yet.</div>";
      setHTML("orders-list", noResults);
    }
  }

  show(shouldSlide = true) {
    super.show();

    if (shouldSlide) {
      slide(this.name, 1);
    }
  }
}