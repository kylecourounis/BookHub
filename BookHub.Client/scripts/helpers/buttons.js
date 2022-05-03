/**
 * Defines the sequence of events that occur when the add button is clicked.
 */
function addButtonClick() {
  if (!isMobile()) {
    glowOnClick("button-add");
  }

  getElement("loader").style.display = "block";

  setTimeout(function () {
    getElement("loader").style.display = "none";

    var page = new AddListing();

    page.setElements();
    page.show();
  }, 400);
}

/**
 * Defines the sequence of events that occur when the back button is clicked.
 */
function backButtonClick() {
  if (!isMobile()) {
    glowOnClick("button-back");
  }

  // Removes and returns the last screen in the layers array.
  var current = layers.pop();

  // -- Check if we're on any dynamically created pages --
  goHomeFrom("purchase-confirmation");
  // -----------------------------------------------------

  // Since we removed the current screen, we can gets the previous screen by obtaining the last element in the layers array.
  var previous = layers[layers.length - 1];
  
  slide(current, 0);
  slide(previous, 1, false);

  var page = null;

  if (previous === "home") {
    if (current === "add-listing") {
      // For the sake of performance, let's unbind these resource-heavy events that happen on the Add Listing page
      $("#input-condition").unbind("change");
      $("#input-price").unbind("keydown keyup change paste");
      $("#input-price").unbind("blur");
    }
    
    storedScrollY = 0;

    page = new Home();
  }

  if (current === "listing" && previous === "orders") {
    page = new Orders();
    page.show(false); // false because we're already appending the page in this function
  }

  if (previous === "listings") {
    page = new Listings();
    page.show(false); // false because we're already appending the page in this function
  } else {
    getElement("search-bar").classList.remove("slidedown");
    getElement("header").style.boxShadow = "0 0 10px 1px rgb(0 0 0 / 50%)";
  }

  if (previous === "listings" || previous === "listing") {
    document.querySelector("body").style.overflowY = "scroll";
  } else {
    document.querySelector("body").style.overflowY = "hidden";
  }

  if (previous === "listing") {
    getElement("header-txt").innerText = "Listing";
  }
  
  if (page != null) {
    page.setElements();
  }

  window.scrollTo(0, storedScrollY); // Go back to the previously stored scroll position
}

/**
 * Defines the sequence of events that occur when the search button is clicked.
 */
function searchButtonClick() {
  if (!isMobile()) {
    glowOnClick("button-search");
  }
  
  var listings = new Listings();
  listings.setElements();

  // This check prevents a duplicate screen layer from being created in the array
  if (layers[layers.length - 1] !== "listings") {
    listings.show();
  }
}

/**
 * Defines the sequence of events that occur when the settings button is clicked.
 */
function settingsButtonClick() {
  if (!isMobile()) {
    glowOnClick("button-settings");
  }

  getElement("loader").style.display = "block";

  setTimeout(function () {
    getElement("loader").style.display = "none";
    getElement("popup-container").innerHTML = getSnippet("settings");

    $(".carousel").css("display", "none"); // Hide carousel after opening settings

    getElement("input-name").value = userData.Name;
    getElement("input-email").value = userData.Email;
    getElement("input-password").value = userData.Password;
    
    sendRequest(Requests.GetColleges);

    toggleDark();
  }, 400);
}

/**
 * Defines the sequence of events that occur when the orders button is clicked.
 */
function ordersButtonClicked() {
  if (!isMobile()) {
    glowOnClick("button-orders");
  }

  getElement("loader").style.display = "block";

  setTimeout(function () {
    getElement("loader").style.display = "none";
    
    var orders = new Orders();
    orders.show();
    orders.setElements();
  }, 400);
}

/**
 * Checks whether the specified element exists on the page and then returns to the second to last page, which should be the home page.
 * @param {*} element The element to check 
 */
 function goHomeFrom(element) {
  // Check if the current element is present on the page
  if (getElement(element) != null) {
    // Iterates till it hits the home screen
    for (var i = layers.length - 1; i > 1; i--) {
      layers.pop();
    }
  }
}

/**
 * Closes a popup. 
 */
function closePopup() {
  if (isMobile()) {
    clearPopup();
  } else {
    setTimeout(function () {
      clearPopup();
    }, 100);
  }
}

/**
 * Clears the popup-container element.
 */
function clearPopup() {
  $(".carousel").css("display", ""); // Unhide carousel after closing settings

  getElement("popup-container").innerHTML = "";
  getElement("loader").style.display = "none";

  toggleDark();
}

/**
 * Called when the specified table item element is clicked.
 * @param {*} element The table item element
 */
function tableItemClicked(element) {
  if (!isMobile()) {
    element.className = "touched";

    setTimeout(function () {
      element.className = "";
    }, 100);
  }
}

/**
 * Ensures the div buttons highlight on click when viewing the site on a non-mobile device
 * @param {*} id The id of the element
 */
function lightupOnClick(id) {
  if (!isMobile()) {
    getElement(id).className = "custom-button-over";

    setTimeout(function () {
      getElement(id).className = "custom-button";
    }, 100);
  }
}

/**
 * Ensures that `glow(where)` is still called when viewing the site on a non-mobile device
 * @param {*} where The location of the element to place the glow on top of.
 */
function glowOnClick(where) {
  glow(where);

  setTimeout(function () {
    noGlow();
  }, 100);
}

/**
 * Places the glow element at the specified location and shows it
 * @param {*} where The location of the element to place the glow on top of.
 */
function glow(where) {
  var offset = $("#" + where + "").offset();
  var width = $("#" + where + "").width();
  getElement("glow").style.left = offset.left - ((80-width) / 2) + 'px';
  
  var height = $("#" + where + "").height();
  getElement("glow").style.top = offset.top - ((80-height) / 2) + 'px';
  getElement("glow").style.display = "block";
}

/**
 * Hides the glow element.
 */
function noGlow() {
  getElement("glow").style.display = "none";
}