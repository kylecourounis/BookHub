/**
 * Uses ajax to get the contents of the specified screen.
 * @param {*} name The name of the HTML snippet file without the extension
 * @returns The contents of the HTML file
 */
function getSnippet(name) {
  var result = null;

  $.ajax({
    url: createPath(name),
    type: 'get',
    dataType: 'html',
    async: false,
    cache: false,
    success: function(data) {
      result = data;
    }
  });

  return result;
}

/**
 * Sets the innerHTML of the element with the specified id.
 * @param {*} id The element id
 * @param {*} data The data
 */
function setHTML(id, data) {
  $("#" + id).html(data);
}

/**
 * Creates the path to the HTML snippets that are loaded with ajax. 
 * @param {*} file 
 * @returns The path to an HTML file
 */
function createPath(file) {
  return "snippets/" + file + ".html";
}

/**
 * Helper function to easily call `document.getElementById(...)`
 * @param {*} id The element id
 */
function getElement(id) {
  return document.getElementById(id);
}

/**
 * Creates a slide-down alert with the specified type and message.
 * @param {*} type The type of alert (warning, error, etc.)
 * @param {*} message The message
 */
function createAlert(type, message) {
  var alert = "<div class='alert " + type + "'>" +
              "<span class='closebtn'>&times;</span>" +
                message + 
              "</div>";

  getElement("alert-container").style.marginTop = "50px";
  getElement("alert-container").innerHTML = alert;
  getElement("alert-container").classList.add("slidedown"); 

  // Alerts can be cleared by either clicking the close button or waiting two seconds
  $(".closebtn").on("click", clearAlert);
  setTimeout(clearAlert, 5000);
}

/**
 * Clears the contents of the `alert-container`.
 */
function clearAlert() {
  getElement("alert-container").classList.remove("slidedown");
  getElement("alert-container").style.marginTop = "-50px";

  setTimeout(function () {
    getElement("alert-container").innerHTML = "";
    getElement("alert-container").style.marginTop = "";

    $(".container").css("z-index", "");
  }, 200);
}

/**
 * Toggles the dark overlay behind a popup like the settings menu.
 * @returns A value indicating whether or not the dark element is showing
 */
function toggleDark() {
  var darken = getElement("darken");

  if (darken.style.display === "block") {
    $(".bar").css("z-index", 10);
    getElement("darken").style.display = "none";
    document.querySelector("body").style.overflow = "";
  } else {
    $(".bar").css("z-index", -1);
    getElement("darken").style.display = "block";
    document.querySelector("body").style.overflow = "hidden";
  }

  return darken.style.display === "block";
}
