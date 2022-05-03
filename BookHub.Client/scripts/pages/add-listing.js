class AddListing extends Page {
  constructor() {
    super("add-listing", "Add Listing");
  }

  setElements() {
    super.setElements();

    slide(this.name, 1);

    getElement("button-settings").style.display = "none";
    getElement("button-back").style.display = "block";
    getElement("button-search").style.display = "none";
    
    getElement("footer").classList.remove("slideup");
  }

  show() {
    super.show();

    while (getElement("input-price") == null) {}

    $("#button-add-listing").css("opacity", "0.5");
    $("#button-add-listing").css("pointer-events", "none");
    
    $("#input-isbn").on("change keydown keyup change paste", this.checkISBN);

    $("#input-condition").on("change", this.checkCondition);
        
    $("#input-condition").on("change", this.capPrice);
    $("#input-price").on("keydown keyup change paste", this.capPrice);

    // These two blur events were necessary to attain the functionality I sought
    $("#input-price").on("blur", function() {
      var priceStr = getElement("input-price").value;

      if (priceStr.length > 0) {
        if ((priceStr.length == 1 || priceStr.length == 2) && priceStr.indexOf(".") < 0) {
          getElement("input-price").value += ".";
        }
      }
    });

    $("#input-price").on("blur", function() {
      var priceStr = getElement("input-price").value.toString();

      if (priceStr.length > 0) {
        var cents = priceStr.substring(priceStr.indexOf(".") + 1);

        for (var i = 0; i < -cents.length + 2; i++) {
          getElement("input-price").value += "0";
        }
      }
    });
  }

  /**
   * Callback function for checking whether the ISBN is valid.
   */
  checkISBN() {
    var isbn = getElement("input-isbn").value;

    // ISBNs have a length of either 10 or 13
    if (isbn.length == 10 || isbn.length == 13) {
      getElement("input-isbn").style.color = "green";
    } else {
      // If it's not the right length, set the color to red and prevent the user from submitting their listing.
      getElement("input-isbn").style.color = "red";

      $("#button-add-listing").css("opacity", "0.5");
      $("#button-add-listing").css("pointer-events", "none");
    }
  }

  /**
   * Callback function for checking whether a condition has been selected.
   */
  checkCondition() {
    if (getElement("input-condition").value === "") {
      $("#button-add-listing").css("opacity", "0.5");
      $("#button-add-listing").css("pointer-events", "none");
    }
  }

  /**
   * Callback function for determing if the entered price is a valid a number or if it's within the limits imposed by the selected condition.
   */
  capPrice() {
    var priceStr = getElement("input-price").value;

    // Ensures the user has entered something to the price input
    if (priceStr.length > 0) {  
      var price = parseFloat(priceStr);

      // If the price is not a number, then set it the color to red
      if (isNaN(price)) {
        getElement("input-price").style.color = "red";
      } else {
        var condition = getElement("input-condition").value;
        
        // Check if the price and the selected condition are reasonable, and if the price is set too high based on the condition, set the text to red
        if (price < 1 || isNaN(price) || !(/^\d(?:[.]\d)?/.test(priceStr))) {
          getElement("input-price").style.color = "red";
        } else if (CONDITION[condition] === "Poor" && price > 39.99) {
          getElement("input-price").style.color = "red";
        } else if (CONDITION[condition] === "Fair" && price > 49.99) {
          getElement("input-price").style.color = "red";
        } else if (CONDITION[condition] === "Good" && price > 59.99) {
          getElement("input-price").style.color = "red";
        } else if (CONDITION[condition] === "Excellent" && price > 69.99) {
          getElement("input-price").style.color = "red";
        } else {
          getElement("input-price").style.color = "green";
        }
      }

      // By checking if the text color of the price input is red, we can then disable the add listing button until the price is changed again
      if (shouldDisableAddButton(priceStr)) {
        $("#button-add-listing").css("opacity", "0.5");
        $("#button-add-listing").css("pointer-events", "none");
      } else {        
        $("#button-add-listing").css("opacity", "1");
        $("#button-add-listing").css("pointer-events", "all");
      }
    }
  }
}

/**
 * Checks the validity of the price input, the ISBN input, and the condition input.
 * @param {*} priceStr The price input as a string.
 * @returns A value indicating whether or not we should disable the add button.
 */
function shouldDisableAddButton(priceStr) {
  var isbn = getElement("input-isbn").value; // Need the ISBN condition so that it checks it even if the user changes the ISBN after setting the price.
  return getElement("input-price").style.color === "red" || !(/^\d(?:[.]\d)?/.test(priceStr)) || !(isbn.length == 10 || isbn.length == 13) || getElement("input-condition").value === "";
}
