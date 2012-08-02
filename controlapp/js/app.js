BALLOONCONTROL = {
  common: {
    init: function() {
      // establish our websocket connection
    }
  },
 
  mastercontrols: {
    init: function() {
      // bind the buttons

      // CREATE
  	  $("#master_controls_create").bind( "click", function(event, ui) {
        // enter create mode
        var buttonText = $("#master_controls_create .ui-btn-text").text();
        if (buttonText == "Create") {
          // fire off create event
          buttonText = "Stop Creating";
        }
        else {
          // fire off end create mode event
          buttonText = "Create"
          clicked = null;
        }

        $("#master_controls_create .ui-btn-text").text(buttonText);
      });


      // MOVE
      $("#master_controls_move").bind( "click", function(event, ui) {
        // enter create mode
        var buttonText = $("#master_controls_move .ui-btn-text").text();
        if (buttonText == "Move") {
          // fire off create event
          buttonText = "Stop Moving";
        }
        else {
          // fire off end create mode event
          buttonText = "Move"
        }
        $("#master_controls_move .ui-btn-text").text(buttonText);
      });


      // CREATE
      $("#master_controls_delete").bind( "click", function(event, ui) {
        // delete whatever is the nearest cube to this hand


      });





    },
 
  }
};
 
UTIL = {
  exec: function( controller, action ) {
    var ns = BALLOONCONTROL,
        action = ( action === undefined ) ? "init" : action;
 
    if ( controller !== "" && ns[controller] && typeof ns[controller][action] == "function" ) {
      ns[controller][action]();
    }
  },
 
  init: function() {
    var body = document.body,
        controller = body.getAttribute( "data-controller" ),
        action = body.getAttribute( "data-action" );
 
    UTIL.exec( "common" );
    UTIL.exec( controller );
    UTIL.exec( controller, action );
  }
};
 
$( document ).ready( UTIL.init );