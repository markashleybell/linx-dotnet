// Send a message containing the page details back to the popup
chrome.runtime.sendMessage({
    'title': document.title,
    'url': window.location.href,
    'abstract': window.getSelection().toString()
});
