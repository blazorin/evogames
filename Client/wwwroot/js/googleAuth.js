function onSignIn(googleUser) {
    var id_token = googleUser.getAuthResponse().id_token;
    evoGoogleState.callEvoServer(id_token);

    setTimeout(() => {
        window.gapi.auth2.getAuthInstance().disconnect();
    }, 3500);
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