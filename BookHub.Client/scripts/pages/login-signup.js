class Login extends Page {
  constructor() {
    super("login", "Login");
  }

  setElements() {
    super.setElements();
  }

  show() {
    super.show();

    if (firstLogin) {
      $("#input-email").attr("placeholder", "Enter your email address");
      $("#input-password").attr("placeholder", "Enter a password");

      getElement("login-button").innerText = "Sign Up";

      getElement("login-table").innerHTML += "<br /><br />" +
                                            "<a id='login-link' href='#' onclick='formatLoginPage()'>" +
                                            "Already have an account? Sign in.</a>";
    }

    getElement(this.name).style.webkitTransform = "translate3d(0px, 0px, 0px)";

    if (userData != null) {
      getElement("input-email").value = userData.Email;
      getElement("input-password").value = userData.Password;
    }
  
    $("#input-password").on("keydown", function(e) {
      if(e.keyCode == 13 && layers.length == 1) {
        sendRequest(Requests.Login);
      }
    });
  }
}

/**
 * Formats the login page based on whether or not this is the user's first time in the app.
 */
function formatLoginPage() {
  firstLogin = false; // Set global firstLogin variable to false

  var page = new Login();
  page.setElements();
  page.show();
}

class SignUp extends Page {
  constructor() {
    super("sign-up", "Update Account Information");
  }

  setElements() {
    super.setElements();
  }

  show() {
    setHTML("login", getSnippet("sign-up"));
    sendRequest(Requests.GetColleges);
  }
}