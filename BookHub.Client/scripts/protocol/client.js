var url = "http://127.0.0.1:8080/";

/** The requests and responses are in JSON format, and sent with 'jsonp' as the data type. */

/** 
 * The requests the client can send to the server.
 */
const Requests = {
  Login: {
    getID() {
      return 1001;
    },

    create() {
      var userId = userData != null ? userData.Identifier : -1;
      var email = getElement("input-email").value;
      var password = getElement("input-password").value;

      return JSON.stringify({
        id: this.getID(),
        userId: userId,
        data: { "id": userId, "email": email, "password": password }
      });
    }
  },
  AddListing: {
    getID() {
      return 1002;
    },

    create() {
      var isbn = getElement("input-isbn").value;
      var condition = getElement("input-condition").value;
      var price = getElement("input-price").value;
      var sellersNotes = getElement("input-sellers-notes").value;

      return JSON.stringify({
        id: this.getID(),
        userId: userData.Identifier,
        data: { "isbn": isbn, "condition": condition, "price": price, "sellersNotes": sellersNotes }
      });
    }
  },
  Search: {
    getID() {
      return 1003;
    },

    create() {
      var query = getElement("input-search").value;

      return JSON.stringify({
        id: this.getID(),
        userId: userData.Identifier,
        data: { "query": query }
      });
    }
  },
  BuyListing: {
    getID() {
      return 1004;
    },

    create() {
      var seed = getElement("seed-storage").innerText;

      return JSON.stringify({
        id: this.getID(),
        userId: userData.Identifier,
        data: { "seed": seed }
      });
    }
  },
  UpdateAccountInfo: {
    getID() {
      return 1005;
    },

    create() {
      var name = getElement("input-name").value;
      var college = getElement("input-college").value;

      return JSON.stringify({
        id: this.getID(),
        userId: userData.Identifier,
        data: { "email": userData.Email, "password": userData.Password, "name": name, "college": college, "firstLogin": firstLogin }
      });
    }
  },
  GetColleges: {
    getID() {
      return 1006;
    },

    create() {
      return JSON.stringify({
        id: this.getID(),
        userId: userData.Identifier,
        data: {}
      });
    }
  }
}

/** 
 * The responses the client can receive from the server, as well as what to do with the information received.
 * The decode method sets the values. The process method calls the decode method and then uses that information to carry out whatever tasks need to be carried out.
 */
const Responses = {
  2001: {
    error: 0,
    user: null,

    getName() {
      return "LoginResponse";
    },

    getID() {
      return 2001;
    },

    decode(json) {
      this.error = json.errorCode;
      this.user = JSON.parse(json.user);
    },

    process(json) {
      this.decode(json);
      
      if (this.error == 0) {
        // If you're logging into an already existing account on a new device, then this check is necessary
        localStorage.setItem("user", JSON.stringify(this.user));
        userData = this.user;
        firstLogin = false;
      } else if (this.error == 1) {
        // Account creation
        localStorage.setItem("user", JSON.stringify(this.user));
        userData = this.user;
        firstLogin = true;
      } else if (this.error == 2) {
        createAlert("error", "Invalid login information!");
        return;
      }
      
      onLogin();
    }
  },
  2002: {
    getName() {
      return "AddListingResponse";
    },

    getID() {
      return 2002;
    },

    decode(json) {
      this.success = json.success;
    },

    process(json) {
      this.decode(json);

      if (this.success) {
        backButtonClick();
      } else {
        $("#add-listing").css("z-index", "-1");
        createAlert("error", "An error occured while trying to add the listing!");
      }
    }
  },
  2003: {
    keys: null,

    getName() {
      return "SearchResponse";
    },

    getID() {
      return 2003;
    },

    decode(json) {
      this.keys = Object.keys(json);
    },

    process(json) {
      this.decode(json);

      if (getElement("books-list")) {
        // Create the listings for the listings screen
        setHTML("books-list", "");

        listings = [];

        if (this.keys.length > 0) {
          for (var i = 0; i < this.keys.length; i++) {
            var seed = this.keys[i];
            var book = JSON.parse(json[seed]);
    
            addListing(book);
          }

          window.scrollTo(0, storedScrollY); // Go back to the previously stored scroll position
        } else {
          var noResults = "<div id='search-error' style='margin: 90px 10px; color: grey; text-align: center;'>No results found.</div>";
          setHTML("books-list", noResults);
        }
      } else {
        // Create the carousel for the home screen
        carouselIndexes = [];
        document.querySelector(".carousel-viewport").innerHTML = "";
        document.querySelector(".carousel-navigation-list").innerHTML = "";
        
        if (this.keys.length > 0) {
          var length = this.keys.length > 5 ? 5 : this.keys.length; // Show only a maximum of 5 listings

          for (var i = 0; i < length; i++) {
            var isbn = this.keys[i];
            var book = JSON.parse(json[isbn]);

            var listing = makeListing(book);
            listings[listing.seed] = listing;

            createCarouselItem(listing, i);
          }
          
          updateNavButtonColor();
        } else {
          var noResults = "<div id='no-new-releases' style='margin: 90px 10px; font-size: 18px; font-weight: normal; color: grey; text-align: center;'>No new releases.</div>"
          
          $(".carousel-viewport").css("flex-direction", "column");
          document.querySelector(".carousel-viewport").innerHTML = noResults;
          
          document.querySelector(".carousel-navigation-list").innerHTML = "";
        }
      }
    }
  },
  2004: {
    order: null,

    getName() {
      return "BuyListingResponse";
    },

    getID() {
      return 2004;
    },

    decode(json) {
      this.order = JSON.parse(json.order);
    },

    process(json) {
      this.decode(json);

      var purchaseConfirmation = "<div id='purchase-confirmation'><br><br>Thanks for purchasing!" + 
                                 "<br><br>Your order number is: <div id='order-num' style='color: red;'>" + this.order.OrderNumber + 
                                 "</div><br><br>Meet the seller at <span style='color: green;'>" + userData.College.DropLocation + "</span> to make the exchange.</div>";
      
      userData.Orders.push(this.order);

      listings[this.order.Listing.Seed] = null;

      setHTML("purchase", purchaseConfirmation);

      getElement("purchase-confirmation").style.fontSize = "17px";
      getElement("purchase-confirmation").style.textAlign = "center";
      getElement("purchase-confirmation").style.padding = "15px";                                  
    }
  },
  2005: {
    user: null,

    getName() {
      return "UpdateAccountInfoResponse";
    },

    getID() {
      return 2005;
    },

    decode(json) {
      this.user = JSON.parse(json.user);
    },

    process(json) {
      this.decode(json);

      localStorage.setItem("user", JSON.stringify(this.user));
      userData = this.user;

      if (firstLogin) {
        var home = new Home();
  
        home.setElements();
        home.show();
      } else {
        closePopup();
      }
    }
  },
  2006: {
    keys: null,

    getName() {
      return "GetCollegesResponse";
    },

    getID() {
      return 2006;
    },

    decode(json) {
      this.keys = Object.keys(json);
    },

    process(json) {
      this.decode(json);
      
      if (this.keys.length > 0) {
        for (var i = 0; i < this.keys.length; i++) {
          var id = this.keys[i];
          var college = JSON.parse(json[id]);
          
          getElement("input-college").innerHTML += "<option value=" + id + ">" + college.Name + "</option>";
        }

        if (userData.College.Identifier > 0) {
          // Set the selected option in the drop down box to be the college the user has specified they're attending
          $("#input-college").val(userData.College.Identifier).attr("selected", "selected").trigger("chosen:updated");
        }
      }
    }
  }
}

function sendRequest(message) {
  $.support.cors = true;

  getElement("loader").style.display = "block";

  setTimeout(function () {
    getElement("loader").style.display = "none";

    $.ajax({
        type: "POST",
        crossdomain: true,
        contentType: "application/json; charset=utf-8",
        url: url,
        dataType: "jsonp",
        data: { json: message.create(), "callback": "processJSON" },
        timeout: 5000
      }).done(processJSON).fail(function (jqXHR, textStatus, errorThrown) {
        // Because of the way I structured the client/server relationship, a 404 error means a connection could not be established
        // It's not merely a "not found" error
        if (jqXHR.status === 404) {
          if (message.getID() == Requests.Login.getID()) {
            createAlert("error", "Unable to connect to the server.\nPlease try again.");
          }
          if (message.getID() == Requests.Search.getID()) {
            var error = "<div id='search-error' style='margin: 90px 10px; color: red; text-align: center;'>"
                        + "An error occured while searching!<br><br>Please try again.</div>";

            setHTML("listings", error);
          }
        }
    });
  }, 500);
}

/** 
 * Callback for the ajax request in the function above. 
 */
function processJSON(json) {
  // Get the response id from the JSON data and then use that as the key for the Responses object to call the process method with the data array.
  Responses[json.id].process(json.data);
}
