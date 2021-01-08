interface LinxChromeExtension {
    getPageDetails(callback: (message: any) => void): void;
}

interface Window {
    LinxChromeExtension: LinxChromeExtension;
}

interface PageDetails {
    title: string;
    url: string;
    abstract: string;
}

const titleInput = document.getElementById('title') as HTMLInputElement;
const urlInput = document.getElementById('url') as HTMLInputElement;
const abstractInput = document.getElementById('abstract') as HTMLInputElement;
const tagsInput = document.getElementById('tags') as HTMLInputElement;

// This callback function is called when the content script has been 
// injected and returned its results
function onPageDetailsReceived(pageDetails: PageDetails)  { 
    titleInput.value = pageDetails.title; 
    urlInput.value = pageDetails.url; 
    abstractInput.value = pageDetails.abstract; 
} 

let statusIndicator: HTMLSpanElement = null;

// POST the data to the server using XMLHttpRequest
function addBookmark(event: Event) {

    // Cancel the form submit
    event.preventDefault();

    // Get the REST endpoint URL from the extension config
    var postUrl = localStorage['post_url'];
    if (!postUrl) {
        alert('POST Url is not set');
        return;
    }

    // Build up an asynchronous AJAX POST request
    var xhr = new XMLHttpRequest();
    xhr.open('POST', postUrl, true);

    // URLEncode each field's contents
    var params = 'link_id=0' + 
                 '&title=' + encodeURIComponent(titleInput.value) + 
                 '&url=' + encodeURIComponent(urlInput.value) + 
                 '&abstract=' + encodeURIComponent(abstractInput.value) +
                 '&tags=' + encodeURIComponent(tagsInput.value);
    
    // Replace any instances of the URLEncoded space char with +
    params = params.replace(/%20/g, '+');

    // Set correct header for form data
    xhr.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');
    
    // Handle request state change events
    xhr.onreadystatechange = function() { 
        // If the request completed
        if (xhr.readyState == 4) {
            statusIndicator.classList.remove('label-default', 'label-success', 'label-danger');
            // If the request was successful
            if (xhr.status == 200) {
                var json = JSON.parse(xhr.responseText);
                // If an error message was returned, show it
                if(typeof json.error !== 'undefined') {
                    statusIndicator.innerText = json.error;
                    statusIndicator.classList.add('label-danger');  
                } else { // If the data was successfully saved
                    statusIndicator.innerText = 'Saved!';
                    statusIndicator.classList.add('label-success');
                    // Close the popup after a short delay
                    window.setTimeout(window.close, 1000);
                }
            } else {// Show what went wrong
                statusIndicator.innerText = 'Error saving: ' + xhr.statusText;
                statusIndicator.classList.add('label-danger');
            }
        }
    };

    // Send the request
    xhr.send(params);

    statusIndicator.innerText = 'Saving...';
    statusIndicator.classList.remove('label-default', 'label-success', 'label-danger');
    statusIndicator.classList.add('label-default');
    statusIndicator.style.display = 'inline';
}

// When the popup HTML has loaded
window.addEventListener('load', function(evt) {
    statusIndicator = document.getElementById('result') as HTMLSpanElement;

    // Handle the bookmark form submit event with our addBookmark function
    document.getElementById('addbookmark').addEventListener('submit', addBookmark);
    
    // Get the event page
    chrome.runtime.getBackgroundPage(function(eventPage) {
        // Call the getPageInfo function in the event page, passing in our onPageDetailsReceived 
        // function as the callback. This injects content.js into the current tab's HTML
        eventPage.LinxChromeExtension.getPageDetails(onPageDetailsReceived);
    });

    var tagJsonUrl = localStorage['tag_json_url'];
    if (!tagJsonUrl) {
        alert('Tag JSON Url is not set');
        return;
    }
});