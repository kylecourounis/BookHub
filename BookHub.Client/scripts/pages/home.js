var carouselIdx = 0;
var carouselScrollPositions = [];

class Home extends Page {
  constructor() {
    super("home", "Home");

    sendRequest(Requests.Search);
  }

  setElements() {
    super.setElements();

    getElement("button-back").style.display = "none";
    
    getElement("button-settings").style.display = "block";
    getElement("button-search").style.display = "block";
    
    getElement("footer").classList.add("slideup");
  }

  show() {
    super.show();
    slide(this.name, 1);
  }
}

/**
 * Creates a carousel item from the specified listing.
 * @param {*} listing The listing for which to create this carousel item.
 * @param {*} idx The index of the new carousel slide
 */
function createCarouselItem(listing, idx) {
  var listingInfo = getFormattedListingInfo(listing);
  
  var listItem = "<li id='carousel-slide" + (idx) + "' tabindex='0' class='carousel-slide' onclick='onListingClick(" + listing.seed + ");'>" +
                 "<div class='carousel-snapper'></div><br/>" +
                 "<img src='" + listing.image + "' style='float: left; padding-left: 20px; padding-right: 5px; padding-top: 5px; width:60px; height: 85px;' />" +
                 "<div style='padding-top: 15px; padding-right: 15px;'>" + listingInfo.title + "<br /><br />" +
                 "<i>authors: </i>" + listingInfo.authors + "</div><br />" + 
                 "<span style='color: green;'>$" + listingInfo.price + "</div></li>";

  var navItem = "<li class='carousel-navigation-item'>" +
                "<span class='carousel-navigation-button' id='slide" + idx + "' onclick='changeCarouselSlide(this)'></a>" +
                "</li>";
                
  document.querySelector(".carousel-viewport").innerHTML += listItem;
  document.querySelector(".carousel-navigation-list").innerHTML += navItem;

  carouselScrollPositions.push(window.innerWidth * idx);
}

/**
 * Listens for animation & scrolling events in the carousel.
 */
function updateNavButtonColor() {
  // This is for when we hit the back button and need it to set to the correct navigation position
  getElement("slide" + (carouselIdx)).style.backgroundColor = "#4D4D4D";
  
  document.querySelector(".carousel-viewport").addEventListener("scroll", () => {
    var scrollLeft = document.querySelector(".carousel-viewport").scrollLeft;
    var idx = carouselScrollPositions.findIndex((val) => val == scrollLeft);

    if (scrollLeft == carouselScrollPositions[idx]) {
      updateCarouselIndex(idx);
    }
  });
}

/**
 * Changes the carousel slide when you click a navigation button.
 * @param {*} navButton The navigation button that was pressed.
 */
function changeCarouselSlide(navButton) {
  var idx = parseInt(navButton.id.substring(5)); // The span id is formatted as slide{index}, so if we skip 5 characters, we get the index

  document.querySelector(".carousel-viewport").scrollLeft = carouselScrollPositions[idx]; // Set the scroll position to the position of the correct carousel item

  if (idx == (carouselScrollPositions.length - 1)) {
    setTimeout(function() {
      updateCarouselIndex(idx);
    }, 3000); // The animation is 3 seconds long, so wait for the animation to finish before changing back to the first button
  } else {
    updateCarouselIndex(idx);
  }
}

/**
 * Updates the carousel navigation indicator.
 * @param {*} idx The index the carousel should be set to.
 */
function updateCarouselIndex(idx) {
  if (carouselIdx == (carouselScrollPositions.length - 1)) {
    getElement("slide" + (carouselIdx)).style.backgroundColor = "#888";
    carouselIdx = 0;
    getElement("slide" + (carouselIdx)).style.backgroundColor = "#4D4D4D";
  } else {
    getElement("slide" + (carouselIdx)).style.backgroundColor = "#888";
    carouselIdx = idx;
    getElement("slide" + (carouselIdx)).style.backgroundColor = "#4D4D4D";
  }
}