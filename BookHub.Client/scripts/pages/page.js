/**
 * Default Page model from which most of the other pages/screens are inherited.
 */
class Page {
  /**
   * Initializes a new instance of the Page class.
   */
  constructor(name, header) {
    this.name = name;
    this.header = header;
  }

  /**
   * Sets the shadow on the header bar.
   */
  setHeaderShadow() {
    // The listings page cannot have a box shadow on the header because it looks bizarre when the search bar pulls down
    if (this.name === "listings") {
      getElement("header").style.boxShadow = "0 0 0 0";
    } else {
      getElement("header").style.boxShadow = "0 0 10px 1px rgb(0 0 0 / 50%)";      
    }
  }

  /**
   * Sets the elements for this screen.
   */
  setElements() {
    this.setHeaderShadow();

    // The home and login screens are the only ones with the logo in the header
    if (this.name === "login" || this.name === "home") {
      getElement("header-txt").innerHTML = "<img src='assets/images/transparent-logo.png' width='100px' />";
    } else {
      getElement("header-txt").innerText = this.header;
    }
  }

  /**
   * Sets the elements for this screen.
   */
  show() {
    setHTML(this.name, getSnippet(this.name));
  }
}