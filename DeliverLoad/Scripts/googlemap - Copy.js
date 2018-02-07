//function initMap() {
//    var markerArray = [];

//    // Instantiate a directions service.
//    var directionsService = new google.maps.DirectionsService;

//    // Create a map and center it on Manhattan.
//    var map = new google.maps.Map(document.getElementById('map'), {
//        zoom: 13,
//        center: { lat: 40.771, lng: -73.974 }
//    });

//    // Create a renderer for directions and bind it to the map.
//    var directionsDisplay = new google.maps.DirectionsRenderer({ map: map });

//    // Instantiate an info window to hold step text.
//    var stepDisplay = new google.maps.InfoWindow;

//    // Display the route between the initial start and end selections.
//    calculateAndDisplayRoute(
//        directionsDisplay, directionsService, markerArray, stepDisplay, map);
//    // Listen to change events from the start and end lists.
//    var onChangeHandler = function () {
//        calculateAndDisplayRoute(
//            directionsDisplay, directionsService, markerArray, stepDisplay, map);
//    };
//    document.getElementById('start').addEventListener('change', onChangeHandler);
//    document.getElementById('end').addEventListener('change', onChangeHandler);
//}

//function calculateAndDisplayRoute(directionsDisplay, directionsService,
//    markerArray, stepDisplay, map) {
//    // First, remove any existing markers from the map.
//    for (var i = 0; i < markerArray.length; i++) {
//        markerArray[i].setMap(null);
//    }

//    // Retrieve the start and end locations and create a DirectionsRequest using
//    // WALKING directions.
//    directionsService.route({
//        origin: document.getElementById('start').value,
//        destination: document.getElementById('end').value,
//        travelMode: 'WALKING'
//    }, function (response, status) {
//        // Route the directions and pass the response to a function to create
//        // markers for each step.
//        if (status === 'OK') {
//            document.getElementById('warnings-panel').innerHTML =
//                '<b>' + response.routes[0].warnings + '</b>';
//            directionsDisplay.setDirections(response);
//            showSteps(response, markerArray, stepDisplay, map);
//        } else {
//            window.alert('Directions request failed due to ' + status);
//        }
//    });
//}

//function showSteps(directionResult, markerArray, stepDisplay, map) {
//    // For each step, place a marker, and add the text to the marker's infowindow.
//    // Also attach the marker to an array so we can keep track of it and remove it
//    // when calculating new routes.
//    var myRoute = directionResult.routes[0].legs[0];
//    for (var i = 0; i < myRoute.steps.length; i++) {
//        var marker = markerArray[i] = markerArray[i] || new google.maps.Marker;
//        marker.setMap(map);
//        marker.setPosition(myRoute.steps[i].start_location);
//        attachInstructionText(
//            stepDisplay, marker, myRoute.steps[i].instructions, map);
//    }
//}

//function attachInstructionText(stepDisplay, marker, text, map) {
//    google.maps.event.addListener(marker, 'click', function () {
//        // Open an info window when the marker is clicked on, containing the text
//        // of the step.
//        stepDisplay.setContent(text);
//        stepDisplay.open(map, marker);
//    });
//}

      // This example requires the Places library. Include the libraries=places
      // parameter when you first load the API. For example:
      // <script src="https://maps.googleapis.com/maps/api/js?key=YOUR_API_KEY&libraries=places">
    
//function initMap() {
//        var map = new google.maps.Map(document.getElementById('map'), {
//          center: {lat: -33.8688, lng: 151.2195},
//          zoom: 13
//        });
//        var card = document.getElementById('pac-card');
//        var input = document.getElementById('from');
//        var types = document.getElementById('type-selector');
//        var strictBounds = document.getElementById('strict-bounds-selector');

//        //map.controls[google.maps.ControlPosition.TOP_RIGHT].push(card);

//        var autocomplete = new google.maps.places.Autocomplete(input);

//        // Bind the map's bounds (viewport) property to the autocomplete object,
//        // so that the autocomplete requests use the current map bounds for the
//        // bounds option in the request.
//        autocomplete.bindTo('bounds', map);

//        var infowindow = new google.maps.InfoWindow();
//        var infowindowContent = document.getElementById('infowindow-content');
//        infowindow.setContent(infowindowContent);
//        var marker = new google.maps.Marker({
//          map: map,
//          anchorPoint: new google.maps.Point(0, -29)
//        });

//        autocomplete.addListener('place_changed', function() {
//          infowindow.close();
//          marker.setVisible(false);
//          var place = autocomplete.getPlace();
//          if (!place.geometry) {
//            // User entered the name of a Place that was not suggested and
//            // pressed the Enter key, or the Place Details request failed.
//            window.alert("No details available for input: '" + place.name + "'");
//            return;
//          }

//          // If the place has a geometry, then present it on a map.
//          if (place.geometry.viewport) {
//            map.fitBounds(place.geometry.viewport);
//          } else {
//            map.setCenter(place.geometry.location);
//            map.setZoom(17);  // Why 17? Because it looks good.
//          }
//          marker.setPosition(place.geometry.location);
//          marker.setVisible(true);

//          var address = '';
//          if (place.address_components) {
//            address = [
//              (place.address_components[0] && place.address_components[0].short_name || ''),
//              (place.address_components[1] && place.address_components[1].short_name || ''),
//              (place.address_components[2] && place.address_components[2].short_name || '')
//            ].join(' ');
//          }

//          infowindowContent.children['place-icon'].src = place.icon;
//          infowindowContent.children['place-name'].textContent = place.name;
//          infowindowContent.children['place-address'].textContent = address;
//          infowindow.open(map, marker);
//        });

//        // Sets a listener on a radio button to change the filter type on Places
//        // Autocomplete.
//        function setupClickListener(id, types) {
//          var radioButton = document.getElementById(id);
//          radioButton.addEventListener('click', function() {
//            autocomplete.setTypes(types);
//          });
//        }

//        setupClickListener('changetype-all', []);
//        setupClickListener('changetype-address', ['address']);
//        setupClickListener('changetype-establishment', ['establishment']);
//        setupClickListener('changetype-geocode', ['geocode']);

//        document.getElementById('use-strict-bounds')
//            .addEventListener('click', function() {
//              console.log('Checkbox clicked! New state=' + this.checked);
//              autocomplete.setOptions({strictBounds: this.checked});
//            });
//      }

    //var prm = Sys.WebForms.PageRequestManager.getInstance();
    //prm.add_initializeRequest(InitializeRequest);
    //prm.add_endRequest(EndRequest);

    //function InitializeRequest(sender, args) {
    //}

    //// fires after the partial update of UpdatePanel
    //function EndRequest(sender, args) {
    //    initMap();
    //}

      function initMap() {
          var map = new google.maps.Map(document.getElementById('map'), {
          mapTypeControl: false,
          center: {lat: -33.8688, lng: 151.2195},
          zoom: 13
        });

        new AutocompleteDirectionsHandler(map);
      }

       /**
        * @constructor
       */
      function AutocompleteDirectionsHandler(map) {
        this.map = map;
        this.originPlaceId = null;
        this.destinationPlaceId = null;
        this.travelMode = 'DRIVING';
        var originInput = document.getElementById('from');
        var destinationInput = document.getElementById('to');
        var modeSelector = document.getElementById('mode-selector');
        this.directionsService = new google.maps.DirectionsService;
        this.directionsDisplay = new google.maps.DirectionsRenderer;
        this.directionsDisplay.setMap(map);

        var originAutocomplete = new google.maps.places.Autocomplete(
            originInput, {placeIdOnly: true});
        var destinationAutocomplete = new google.maps.places.Autocomplete(
            destinationInput, {placeIdOnly: true});

        //this.setupClickListener('changemode-walking', 'WALKING');
        //this.setupClickListener('changemode-transit', 'TRANSIT');
        //this.setupClickListener('changemode-driving', 'DRIVING');

        this.setupPlaceChangedListener(originAutocomplete, 'ORIG');
        this.setupPlaceChangedListener(destinationAutocomplete, 'DEST');

        this.map.controls[google.maps.ControlPosition.TOP_LEFT].push(originInput);
        this.map.controls[google.maps.ControlPosition.TOP_LEFT].push(destinationInput);
        this.map.controls[google.maps.ControlPosition.TOP_LEFT].push(modeSelector);
      }

      // Sets a listener on a radio button to change the filter type on Places
      // Autocomplete.
      //AutocompleteDirectionsHandler.prototype.setupClickListener = function(id, mode) {
      //  var radioButton = document.getElementById(id);
      //  var me = this;
      //  radioButton.addEventListener('click', function() {
      //    me.travelMode = mode;
      //    me.route();
      //  });
      //};

      AutocompleteDirectionsHandler.prototype.setupPlaceChangedListener = function(autocomplete, mode) {
        var me = this;
        autocomplete.bindTo('bounds', this.map);
        autocomplete.addListener('place_changed', function() {
          var place = autocomplete.getPlace();
          if (!place.place_id) {
            window.alert("Please select an option from the dropdown list.");
            return;
          }
          if (mode === 'ORIG') {
            me.originPlaceId = place.place_id;
          } else {
            me.destinationPlaceId = place.place_id;
          }
          me.route();
        });

      };

      AutocompleteDirectionsHandler.prototype.route = function() {
        if (!this.originPlaceId || !this.destinationPlaceId) {
          return;
        }
        var me = this;

        this.directionsService.route({
          origin: {'placeId': this.originPlaceId},
          destination: {'placeId': this.destinationPlaceId},
          travelMode: this.travelMode
        }, function(response, status) {
          if (status === 'OK') {
            me.directionsDisplay.setDirections(response);
          } else {
            window.alert('Directions request failed due to ' + status);
          }
        });
      };

      // This example displays an address form, using the autocomplete feature
      // of the Google Places API to help users fill in the information.

      // This example requires the Places library. Include the libraries=places
      // parameter when you first load the API. For example:
      // <script src="https://maps.googleapis.com/maps/api/js?key=YOUR_API_KEY&libraries=places">

      var placeSearch, autocomplete;
      var componentForm = {
        street_number: 'short_name',
        route: 'long_name',
        locality: 'long_name',
        administrative_area_level_1: 'short_name',
        country: 'long_name',
        postal_code: 'short_name'
      };

      function initAutocomplete() {
        // Create the autocomplete object, restricting the search to geographical
        // location types.
        autocomplete = new google.maps.places.Autocomplete(
            /** @type {!HTMLInputElement} */(document.getElementById('autocomplete')),
            {types: ['geocode']});

        // When the user selects an address from the dropdown, populate the address
        // fields in the form.
        autocomplete.addListener('place_changed', fillInAddress);
      }

      function fillInAddress() {
        // Get the place details from the autocomplete object.
        var place = autocomplete.getPlace();

        for (var component in componentForm) {
          document.getElementById(component).value = '';
          document.getElementById(component).disabled = false;
        }

        // Get each component of the address from the place details
        // and fill the corresponding field on the form.
        for (var i = 0; i < place.address_components.length; i++) {
          var addressType = place.address_components[i].types[0];
          if (componentForm[addressType]) {
            var val = place.address_components[i][componentForm[addressType]];
            document.getElementById(addressType).value = val;
          }
        }
      }

      // Bias the autocomplete object to the user's geographical location,
      // as supplied by the browser's 'navigator.geolocation' object.
      function geolocate() {
        if (navigator.geolocation) {
          navigator.geolocation.getCurrentPosition(function(position) {
            var geolocation = {
              lat: position.coords.latitude,
              lng: position.coords.longitude
            };
            var circle = new google.maps.Circle({
              center: geolocation,
              radius: position.coords.accuracy
            });
            autocomplete.setBounds(circle.getBounds());
          });
        }
      }

