export let bootstrapModal = {
    show: function (element) {
        $(element).modal("show");
    }
};
export let bootstrapToast = {
    show: function (element) {
        $(element).toast("show");
    },
    hide: function (element) {
        $(element).toast("hide");
    }
}