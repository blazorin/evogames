function onSignIn(googleUser) {
    var id_token = googleUser.getAuthResponse().id_token;
    evoGoogleState.callEvoServer(id_token);
}

window.evoGoogleState = (function () {
    var reference = null;
    return {
        initialize: function (ref) {
            reference = ref;
        },
        callEvoServer: function (token) {
            reference.invokeMethodAsync("ReceiveGoogleIdToken", token);
        }
    };
})();