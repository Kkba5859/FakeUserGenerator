window.initializeInfiniteScroll = function (dotnetObject) {
    if (!dotnetObject) {
        throw new Error("dotnetObject is null or undefined");
    }

    window.addEventListener('scroll', function () {
        const isAtBottom = window.innerHeight + window.scrollY >= document.body.offsetHeight;
        if (isAtBottom) {
            dotnetObject.invokeMethodAsync('LoadMoreData');
        }
    });
};

window.downloadFile = function (fileName, base64Data) {
    if (!fileName || !base64Data) {
        throw new Error("fileName or base64Data is null or undefined");
    }

    const link = document.createElement('a');
    link.download = fileName;
    link.href = `data:text/csv;base64,${base64Data}`;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
};

window.showLoading = function () {
    const loadingElement = document.getElementById("loading");
    if (loadingElement) {
        loadingElement.style.display = "block";
    }
};

window.hideLoading = function () {
    const loadingElement = document.getElementById("loading");
    if (loadingElement) {
        loadingElement.style.display = "none";
    }
};
