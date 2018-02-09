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
    
    //document.getElementsByTagName('input')[0]
    autocompleteFrom = new google.maps.places.Autocomplete(
            /** @type {!HTMLInputElement} */($('input#from')[0]),
            { types: ['geocode'] });

    autocompleteTo = new google.maps.places.Autocomplete(
            /** @type {!HTMLInputElement} */($('input#to')[0]),
        { types: ['geocode'] });

    autocompletePickupLocation = new google.maps.places.Autocomplete(
           /** @type {!HTMLInputElement} */($('input#PickupLocation')[0]),
           { types: ['geocode'] });

    autocompleteDropOffLocation = new google.maps.places.Autocomplete(
            /** @type {!HTMLInputElement} */($('input#DropOffLocation')[0]),
        { types: ['geocode'] });

    // When the user selects an address from the dropdown, populate the address
    // fields in the form.
    autocompleteFrom.addListener('place_changed', fillInAddress);
    autocompleteTo.addListener('place_changed', fillInAddress);
    autocompletePickupLocation.addListener('place_changed', fillInAddress);
    autocompleteDropOffLocation.addListener('place_changed', fillInAddress);
    var mapId = document.getElementById('map');
    if (mapId != null) {
        var map = new google.maps.Map(mapId, {
            mapTypeControl: false,
            center: { lat: -33.8688, lng: 151.2195 },
            zoom: 13
        });
        new AutocompleteDirectionsHandler(map)
    };
}

function fillInAddress() {
    // Get the place details from the autocomplete object.
    var placeFrom = autocompleteFrom.getPlace();
    var placeTo = autocompleteFrom.getPlace();
    var placeDropOffLocation = autocompleteDropOffLocation.getPlace();
    var placePickupLocation = autocompletePickupLocation.getPlace();
    //for (var component in componentForm) {
    //    document.getElementById(component).value = '';
    //    document.getElementById(component).disabled = false;
    //}

    // Get each component of the address from the place details
    // and fill the corresponding field on the form.

    //for (var i = 0; i < place.address_components.length; i++) {
    //    var addressType = place.address_components[i].types[0];
    //    if (componentForm[addressType]) {
    //        var val = place.address_components[i][componentForm[addressType]];
    //        document.getElementById(addressType).value = val;
    //    }
    //}
}

// Bias the autocomplete object to the user's geographical location,
// as supplied by the browser's 'navigator.geolocation' object.
function geolocate() {
    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(function (position) {
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
        originInput, { placeIdOnly: true });
    var destinationAutocomplete = new google.maps.places.Autocomplete(
        destinationInput, { placeIdOnly: true });

    //this.setupClickListener('changemode-walking', 'WALKING');
    //this.setupClickListener('changemode-transit', 'TRANSIT');
    //this.setupClickListener('changemode-driving', 'DRIVING');

    this.setupPlaceChangedListener(originAutocomplete, 'ORIG');
    this.setupPlaceChangedListener(destinationAutocomplete, 'DEST');

    //this.map.controls[google.maps.ControlPosition.TOP_LEFT].push(originInput);
    //this.map.controls[google.maps.ControlPosition.TOP_LEFT].push(destinationInput);
    //this.map.controls[google.maps.ControlPosition.TOP_LEFT].push(modeSelector);
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

AutocompleteDirectionsHandler.prototype.setupPlaceChangedListener = function (autocomplete, mode) {
    var me = this;
    autocomplete.bindTo('bounds', this.map);
    autocomplete.addListener('place_changed', function () {
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

AutocompleteDirectionsHandler.prototype.route = function () {
    if (!this.originPlaceId || !this.destinationPlaceId) {
        return;
    }
    var me = this;

    this.directionsService.route({
        origin: { 'placeId': this.originPlaceId },
        destination: { 'placeId': this.destinationPlaceId },
        travelMode: this.travelMode
    }, function (response, status) {
        if (status === 'OK') {
            me.directionsDisplay.setDirections(response);
        } else {
            window.alert('Directions request failed due to ' + status);
        }
    });
};